using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBolt
	{
		public static int GenerationWhereForksStopSubtractor;

		public static int MaximumLightCount;

		public static int MaximumLightsPerBatch;

		public static readonly Dictionary<int, LightningQualityMaximum> QualityMaximums;

		private float elapsedTime;

		private float lifeTime;

		private int generationWhereForksStop;

		private LightningBoltMeshRenderer lightningBoltRenderer;

		private LightningBoltScript script;

		private readonly List<LightningBoltSegmentGroup> segmentGroups = new List<LightningBoltSegmentGroup>();

		private readonly List<LightningBoltSegmentGroup> segmentGroupsWithLight = new List<LightningBoltSegmentGroup>();

		private bool hasLight;

		private static int lightCount;

		private static readonly List<LightningBoltSegmentGroup> groupCache;

		private static readonly List<Light> lightCache;

		private static readonly List<LightningBolt> lightningBoltCache;

		public GameObject Parent { get; private set; }

		public float MinimumDelay { get; private set; }

		public bool HasGlow { get; private set; }

		public bool IsActive
		{
			get
			{
				return elapsedTime < lifeTime;
			}
		}

		public Camera Camera { get; private set; }

		public bool UseWorldSpace { get; set; }

		static LightningBolt()
		{
			GenerationWhereForksStopSubtractor = 5;
			MaximumLightCount = 128;
			MaximumLightsPerBatch = 8;
			QualityMaximums = new Dictionary<int, LightningQualityMaximum>();
			groupCache = new List<LightningBoltSegmentGroup>();
			lightCache = new List<Light>();
			lightningBoltCache = new List<LightningBolt>();
			string[] names = QualitySettings.names;
			for (int i = 0; i < names.Length; i++)
			{
				switch (i)
				{
				case 0:
					QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 3,
						MaximumLightPercent = 0f,
						MaximumShadowPercent = 0f
					};
					break;
				case 1:
					QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 4,
						MaximumLightPercent = 0f,
						MaximumShadowPercent = 0f
					};
					break;
				case 2:
					QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 5,
						MaximumLightPercent = 0.1f,
						MaximumShadowPercent = 0f
					};
					break;
				case 3:
					QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 5,
						MaximumLightPercent = 0.1f,
						MaximumShadowPercent = 0f
					};
					break;
				case 4:
					QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 6,
						MaximumLightPercent = 0.05f,
						MaximumShadowPercent = 0.1f
					};
					break;
				case 5:
					QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 7,
						MaximumLightPercent = 0.025f,
						MaximumShadowPercent = 0.05f
					};
					break;
				default:
					QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 8,
						MaximumLightPercent = 0.025f,
						MaximumShadowPercent = 0.05f
					};
					break;
				}
			}
		}

		public static LightningBolt GetOrCreateLightningBolt()
		{
			if (lightningBoltCache.Count == 0)
			{
				return new LightningBolt();
			}
			LightningBolt result = lightningBoltCache[lightningBoltCache.Count - 1];
			lightningBoltCache.RemoveAt(lightningBoltCache.Count - 1);
			return result;
		}

		public void Initialize(Camera camera, bool useWorldSpace, LightningBoltQualitySetting quality, LightningBoltMeshRenderer lightningBoltRenderer, GameObject parent, LightningBoltScript script, ParticleSystem originParticleSystem, ParticleSystem destParticleSystem, ICollection<LightningBoltParameters> parameters)
		{
			if (parameters == null || lightningBoltRenderer == null || parameters.Count == 0 || script == null)
			{
				return;
			}
			UseWorldSpace = useWorldSpace;
			this.lightningBoltRenderer = lightningBoltRenderer;
			Parent = parent;
			this.script = script;
			CheckForGlow(parameters);
			lightningBoltRenderer.Begin(this);
			MinimumDelay = float.MaxValue;
			int maxLights = MaximumLightsPerBatch / parameters.Count;
			foreach (LightningBoltParameters parameter in parameters)
			{
				ProcessParameters(parameter, quality, originParticleSystem, destParticleSystem, maxLights);
			}
			lightningBoltRenderer.End(this);
		}

		public bool Update()
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > lifeTime)
			{
				Cleanup();
				return false;
			}
			if (hasLight)
			{
				UpdateLightsForGroups();
			}
			return true;
		}

		public void Cleanup()
		{
			foreach (LightningBoltSegmentGroup segmentGroup in segmentGroups)
			{
				foreach (Light light in segmentGroup.Lights)
				{
					lightCache.Add(light);
					light.gameObject.SetActive(false);
					lightCount--;
				}
				segmentGroup.LightParameters = null;
				segmentGroup.Segments.Clear();
				segmentGroup.Lights.Clear();
				segmentGroup.StartIndex = 0;
				groupCache.Add(segmentGroup);
			}
			segmentGroups.Clear();
			segmentGroupsWithLight.Clear();
			if (lightningBoltRenderer != null)
			{
				lightningBoltRenderer.Cleanup(this);
				lightningBoltRenderer = null;
			}
			hasLight = false;
			elapsedTime = 0f;
			lifeTime = 0f;
			lightningBoltCache.Add(this);
		}

		private void ProcessParameters(LightningBoltParameters p, LightningBoltQualitySetting quality, ParticleSystem sourceParticleSystem, ParticleSystem destinationParticleSystem, int maxLights)
		{
			MinimumDelay = Mathf.Min(p.Delay, MinimumDelay);
			generationWhereForksStop = p.Generations - GenerationWhereForksStopSubtractor;
			p.GlowIntensity = Mathf.Clamp(p.GlowIntensity, 0f, 1f);
			p.Random = p.Random ?? new System.Random(Environment.TickCount);
			p.GrowthMultiplier = Mathf.Clamp(p.GrowthMultiplier, 0f, 0.999f);
			lifeTime = Mathf.Max(p.LifeTime + p.Delay, lifeTime);
			LightningLightParameters lightningLightParameters = p.LightParameters;
			if (lightningLightParameters != null)
			{
				if (hasLight |= lightningLightParameters.HasLight)
				{
					lightningLightParameters.LightPercent = Mathf.Clamp(lightningLightParameters.LightPercent, 1E-07f, 1f);
					lightningLightParameters.LightShadowPercent = Mathf.Clamp(lightningLightParameters.LightShadowPercent, 0f, 1f);
				}
				else
				{
					lightningLightParameters = null;
				}
			}
			if (p.Generations < 1)
			{
				p.TrunkWidth = 0f;
				p.Generations = 1;
			}
			else if (p.Generations > 8)
			{
				p.Generations = 8;
			}
			int forkedness = (int)(p.Forkedness * (float)p.Generations);
			int count = segmentGroups.Count;
			int num;
			if (quality == LightningBoltQualitySetting.UseScript)
			{
				num = p.Generations;
			}
			else
			{
				int qualityLevel = QualitySettings.GetQualityLevel();
				LightningQualityMaximum value;
				if (QualityMaximums.TryGetValue(qualityLevel, out value))
				{
					num = Mathf.Min(value.MaximumGenerations, p.Generations);
				}
				else
				{
					num = p.Generations;
					Debug.LogError("Unable to read lightning quality settings from level " + qualityLevel);
				}
			}
			GenerateLightningBolt(p.Start, p.End, num, num, 0f, 0f, forkedness, p);
			RenderLightningBolt(quality, num, p.Start, p.End, count, sourceParticleSystem, destinationParticleSystem, p, lightningLightParameters, maxLights);
		}

		private void RenderLightningBolt(LightningBoltQualitySetting quality, int generations, Vector3 start, Vector3 end, int groupIndex, ParticleSystem originParticleSystem, ParticleSystem destParticleSystem, LightningBoltParameters parameters, LightningLightParameters lp, int maxLights)
		{
			if (segmentGroups.Count == 0 || groupIndex >= segmentGroups.Count)
			{
				return;
			}
			float num = parameters.LifeTime / (float)segmentGroups.Count;
			float num2 = num * 0.9f;
			float num3 = num * 1.1f;
			float num4 = num3 - num2;
			parameters.FadePercent = Mathf.Clamp(parameters.FadePercent, 0f, 0.5f);
			if (originParticleSystem != null)
			{
				script.StartCoroutine(GenerateParticle(originParticleSystem, start, parameters.Delay));
			}
			if (destParticleSystem != null)
			{
				script.StartCoroutine(GenerateParticle(destParticleSystem, end, parameters.Delay * 1.1f));
			}
			if (HasGlow)
			{
				lightningBoltRenderer.GlowIntensityMultiplier = parameters.GlowIntensity;
				lightningBoltRenderer.GlowWidthMultiplier = parameters.GlowWidthMultiplier;
			}
			float num5 = 0f;
			for (int i = groupIndex; i < segmentGroups.Count; i++)
			{
				LightningBoltSegmentGroup lightningBoltSegmentGroup = segmentGroups[i];
				lightningBoltSegmentGroup.Delay = num5 + parameters.Delay;
				lightningBoltSegmentGroup.LifeTime = parameters.LifeTime - num5;
				lightningBoltSegmentGroup.PeakStart = lightningBoltSegmentGroup.LifeTime * parameters.FadePercent;
				lightningBoltSegmentGroup.PeakEnd = lightningBoltSegmentGroup.LifeTime - lightningBoltSegmentGroup.PeakStart;
				lightningBoltSegmentGroup.LightParameters = lp;
				lightningBoltRenderer.AddGroup(this, lightningBoltSegmentGroup, parameters.GrowthMultiplier);
				num5 += (float)parameters.Random.NextDouble() * num2 + num4;
				if (lp != null && lightningBoltSegmentGroup.Generation == generations)
				{
					CreateLightsForGroup(lightningBoltSegmentGroup, lp, quality, maxLights, groupIndex);
				}
			}
		}

		private void CreateLightsForGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, LightningBoltQualitySetting quality, int maxLights, int groupIndex)
		{
			if (lightCount == MaximumLightCount || maxLights <= 0)
			{
				return;
			}
			segmentGroupsWithLight.Add(group);
			int segmentCount = group.SegmentCount;
			float num;
			float num2;
			if (quality == LightningBoltQualitySetting.LimitToQualitySetting)
			{
				int qualityLevel = QualitySettings.GetQualityLevel();
				LightningQualityMaximum value;
				if (QualityMaximums.TryGetValue(qualityLevel, out value))
				{
					num = Mathf.Min(lp.LightPercent, value.MaximumLightPercent);
					num2 = Mathf.Min(lp.LightShadowPercent, value.MaximumShadowPercent);
				}
				else
				{
					Debug.LogError("Unable to read lightning quality for level " + qualityLevel);
					num = lp.LightPercent;
					num2 = lp.LightShadowPercent;
				}
			}
			else
			{
				num = lp.LightPercent;
				num2 = lp.LightShadowPercent;
			}
			maxLights = Mathf.Max(1, Mathf.Min(maxLights, (int)((float)segmentCount * num)));
			int num3 = Mathf.Max(1, segmentCount / maxLights);
			int num4 = maxLights - (int)((float)maxLights * num2);
			int nthShadowCounter = num4;
			for (int i = group.StartIndex + (int)((float)num3 * 0.5f); i < group.Segments.Count && !AddLightToGroup(group, lp, i, num3, num4, ref maxLights, ref nthShadowCounter); i += num3)
			{
			}
		}

		private bool AddLightToGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, int segmentIndex, int nthLight, int nthShadows, ref int maxLights, ref int nthShadowCounter)
		{
			Light light = CreateLight(group, lp);
			light.gameObject.transform.position = (group.Segments[segmentIndex].Start + group.Segments[segmentIndex].End) * 0.5f;
			if (lp.LightShadowPercent == 0f || ++nthShadowCounter < nthShadows)
			{
				light.shadows = LightShadows.None;
			}
			else
			{
				light.shadows = LightShadows.Soft;
				nthShadowCounter = 0;
			}
			return ++lightCount == MaximumLightCount || --maxLights == 0;
		}

		private Light CreateLight(LightningBoltSegmentGroup group, LightningLightParameters lp)
		{
			Light light;
			do
			{
				if (lightCache.Count == 0)
				{
					GameObject gameObject = new GameObject();
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					gameObject.name = "LightningBoltLight";
					light = gameObject.AddComponent<Light>();
					light.type = LightType.Point;
					break;
				}
				light = lightCache[lightCache.Count - 1];
				lightCache.RemoveAt(lightCache.Count - 1);
			}
			while (light == null);
			light.color = lp.LightColor;
			light.renderMode = lp.RenderMode;
			light.range = lp.LightRange;
			light.bounceIntensity = lp.BounceIntensity;
			light.shadowStrength = lp.ShadowStrength;
			light.shadowBias = lp.ShadowBias;
			light.shadowNormalBias = lp.ShadowNormalBias;
			light.intensity = 0f;
			light.gameObject.transform.parent = Parent.transform;
			light.gameObject.SetActive(true);
			group.Lights.Add(light);
			return light;
		}

		private void UpdateLightsForGroups()
		{
			foreach (LightningBoltSegmentGroup item in segmentGroupsWithLight)
			{
				if (elapsedTime < item.Delay)
				{
					continue;
				}
				float num = elapsedTime - item.Delay;
				if (num >= item.PeakStart)
				{
					if (num <= item.PeakEnd)
					{
						foreach (Light light in item.Lights)
						{
							light.intensity = item.LightParameters.LightIntensity;
						}
						continue;
					}
					float t = (num - item.PeakEnd) / (item.LifeTime - item.PeakEnd);
					foreach (Light light2 in item.Lights)
					{
						light2.intensity = Mathf.Lerp(item.LightParameters.LightIntensity, 0f, t);
					}
					continue;
				}
				float t2 = num / item.PeakStart;
				foreach (Light light3 in item.Lights)
				{
					light3.intensity = Mathf.Lerp(0f, item.LightParameters.LightIntensity, t2);
				}
			}
		}

		private LightningBoltSegmentGroup CreateGroup()
		{
			LightningBoltSegmentGroup lightningBoltSegmentGroup;
			if (groupCache.Count == 0)
			{
				lightningBoltSegmentGroup = new LightningBoltSegmentGroup();
			}
			else
			{
				int index = groupCache.Count - 1;
				lightningBoltSegmentGroup = groupCache[index];
				groupCache.RemoveAt(index);
			}
			segmentGroups.Add(lightningBoltSegmentGroup);
			return lightningBoltSegmentGroup;
		}

		private IEnumerator GenerateParticle(ParticleSystem p, Vector3 pos, float delay)
		{
			yield return new WaitForSeconds(delay);
			p.transform.position = pos;
			p.Emit((int)p.emissionRate);
		}

		private void GenerateLightningBolt(Vector3 start, Vector3 end, int generation, int totalGenerations, float offsetAmount, float lineWidth, int forkedness, LightningBoltParameters parameters)
		{
			if (generation < 1)
			{
				return;
			}
			LightningBoltSegmentGroup lightningBoltSegmentGroup = CreateGroup();
			lightningBoltSegmentGroup.EndWidthMultiplier = parameters.EndWidthMultiplier;
			lightningBoltSegmentGroup.Segments.Add(new LightningBoltSegment
			{
				Start = start,
				End = end
			});
			float num = (float)generation / (float)totalGenerations;
			num *= num;
			if (offsetAmount <= 0f)
			{
				offsetAmount = (end - start).magnitude * parameters.ChaosFactor;
			}
			if (lineWidth <= 0f)
			{
				lightningBoltSegmentGroup.LineWidth = parameters.TrunkWidth;
			}
			else
			{
				lightningBoltSegmentGroup.LineWidth = lineWidth * num;
			}
			lightningBoltSegmentGroup.LineWidth *= num;
			lightningBoltSegmentGroup.Generation = generation;
			while (generation-- > 0)
			{
				int startIndex = lightningBoltSegmentGroup.StartIndex;
				lightningBoltSegmentGroup.StartIndex = lightningBoltSegmentGroup.Segments.Count;
				for (int i = startIndex; i < lightningBoltSegmentGroup.StartIndex; i++)
				{
					start = lightningBoltSegmentGroup.Segments[i].Start;
					end = lightningBoltSegmentGroup.Segments[i].End;
					Vector3 vector = (start + end) * 0.5f;
					Vector3 result;
					RandomVector(ref start, ref end, offsetAmount, parameters.Random, out result);
					vector += result;
					lightningBoltSegmentGroup.Segments.Add(new LightningBoltSegment
					{
						Start = start,
						End = vector
					});
					lightningBoltSegmentGroup.Segments.Add(new LightningBoltSegment
					{
						Start = vector,
						End = end
					});
					if (generation > generationWhereForksStop && generation >= totalGenerations - forkedness)
					{
						int num2 = parameters.Random.Next(0, generation);
						if (num2 < forkedness)
						{
							float num3 = (float)parameters.Random.NextDouble() * 0.2f + 0.6f;
							Vector3 vector2 = (vector - start) * num3;
							Vector3 end2 = vector + vector2;
							GenerateLightningBolt(vector, end2, generation, totalGenerations, 0f, lineWidth, forkedness, parameters);
						}
					}
				}
				offsetAmount *= 0.5f;
			}
		}

		private void RandomVector(ref Vector3 start, ref Vector3 end, float offsetAmount, System.Random random, out Vector3 result)
		{
			Vector3 normalized = (end - start).normalized;
			if (Camera == null || !Camera.orthographic)
			{
				Vector3 normalized2 = Vector3.Cross(start, end).normalized;
				float num = ((float)random.NextDouble() + 0.1f) * offsetAmount;
				float angle = (float)random.NextDouble() * 360f;
				result = Quaternion.AngleAxis(angle, normalized) * normalized2 * num;
			}
			else
			{
				Vector3 vector = new Vector3(0f - normalized.y, normalized.x, normalized.z);
				float num2 = (float)random.NextDouble() * offsetAmount * 2f - offsetAmount;
				result = vector * num2;
			}
		}

		private void CheckForGlow(IEnumerable<LightningBoltParameters> parameters)
		{
			foreach (LightningBoltParameters parameter in parameters)
			{
				HasGlow = parameter.GlowIntensity > 0.0001f && parameter.GlowWidthMultiplier >= 0.0001f;
				if (HasGlow)
				{
					break;
				}
			}
		}
	}
}
