using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltMeshRenderer
	{
		[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
		public class LineRendererMesh : MonoBehaviour
		{
			private static readonly Vector2 uv1 = new Vector2(0f, 0f);

			private static readonly Vector2 uv2 = new Vector2(1f, 0f);

			private static readonly Vector2 uv3 = new Vector2(0f, 1f);

			private static readonly Vector2 uv4 = new Vector2(1f, 1f);

			private Mesh mesh;

			private MeshRenderer meshRenderer;

			private readonly List<int> indices = new List<int>();

			private readonly List<Vector2> texCoords = new List<Vector2>();

			private readonly List<Vector3> vertices = new List<Vector3>();

			private readonly List<Vector4> lineDirs = new List<Vector4>();

			private readonly List<Color> colors = new List<Color>();

			private readonly List<Vector2> glowModifiers = new List<Vector2>();

			private readonly List<Vector3> ends = new List<Vector3>();

			public Material Material
			{
				get
				{
					return GetComponent<MeshRenderer>().sharedMaterial;
				}
				set
				{
					GetComponent<MeshRenderer>().sharedMaterial = value;
				}
			}

			public string SortingLayerName
			{
				get
				{
					return GetComponent<MeshRenderer>().sortingLayerName;
				}
				set
				{
					GetComponent<MeshRenderer>().sortingLayerName = value;
				}
			}

			public int SortingOrder
			{
				get
				{
					return GetComponent<MeshRenderer>().sortingOrder;
				}
				set
				{
					GetComponent<MeshRenderer>().sortingOrder = value;
				}
			}

			private void Awake()
			{
				mesh = new Mesh();
				GetComponent<MeshFilter>().sharedMesh = mesh;
				meshRenderer = GetComponent<MeshRenderer>();
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer.useLightProbes = false;
				meshRenderer.receiveShadows = false;
				meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
				meshRenderer.enabled = false;
			}

			private void AddIndices()
			{
				int count = vertices.Count;
				indices.Add(count++);
				indices.Add(count++);
				indices.Add(count);
				indices.Add(count--);
				indices.Add(count);
				indices.Add(count += 2);
			}

			public void Begin()
			{
				meshRenderer.enabled = true;
				mesh.vertices = vertices.ToArray();
				mesh.tangents = lineDirs.ToArray();
				mesh.colors = colors.ToArray();
				mesh.uv = texCoords.ToArray();
				mesh.uv2 = glowModifiers.ToArray();
				mesh.normals = ends.ToArray();
				mesh.triangles = indices.ToArray();
			}

			public bool PrepareForLines(int lineCount)
			{
				int num = lineCount * 4;
				if (vertices.Count + num > 64999)
				{
					return false;
				}
				return true;
			}

			public void BeginLine(Vector3 start, Vector3 end, float radius, Color c, float glowWidthModifier, float glowIntensity)
			{
				AddIndices();
				Vector2 item = new Vector2(glowWidthModifier, glowIntensity);
				Vector4 vector = end - start;
				vector.w = radius;
				vertices.Add(start);
				texCoords.Add(uv1);
				lineDirs.Add(vector);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
				vertices.Add(end);
				texCoords.Add(uv2);
				lineDirs.Add(vector);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
				vector.w = 0f - radius;
				vertices.Add(start);
				texCoords.Add(uv3);
				lineDirs.Add(vector);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
				vertices.Add(end);
				texCoords.Add(uv4);
				lineDirs.Add(vector);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
			}

			public void AppendLine(Vector3 start, Vector3 end, float radius, Color c, float glowWidthModifier, float glowIntensity)
			{
				AddIndices();
				Vector2 item = new Vector2(glowWidthModifier, glowIntensity);
				Vector4 vector = end - start;
				vector.w = radius;
				vertices.Add(start);
				texCoords.Add(uv1);
				lineDirs.Add(lineDirs[lineDirs.Count - 3]);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
				vertices.Add(end);
				texCoords.Add(uv2);
				lineDirs.Add(vector);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
				vector.w = 0f - radius;
				vertices.Add(start);
				texCoords.Add(uv3);
				lineDirs.Add(lineDirs[lineDirs.Count - 3]);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
				vertices.Add(end);
				texCoords.Add(uv4);
				lineDirs.Add(vector);
				colors.Add(c);
				glowModifiers.Add(item);
				ends.Add(vector);
			}

			public void Reset()
			{
				meshRenderer.enabled = false;
				mesh.triangles = null;
				indices.Clear();
				vertices.Clear();
				colors.Clear();
				lineDirs.Clear();
				texCoords.Clear();
				glowModifiers.Clear();
				ends.Clear();
			}
		}

		private LineRendererMesh currentLineRenderer;

		private readonly Dictionary<LightningBolt, List<LineRendererMesh>> renderers = new Dictionary<LightningBolt, List<LineRendererMesh>>();

		private readonly List<LineRendererMesh> rendererCache = new List<LineRendererMesh>();

		public LightningBoltScript Script { get; set; }

		public Material Material { get; set; }

		public Material MaterialNoGlow { get; set; }

		public float GlowWidthMultiplier { get; set; }

		public float GlowIntensityMultiplier { get; set; }

		private IEnumerator EnableRenderer(LineRendererMesh renderer, LightningBolt lightningBolt)
		{
			yield return new WaitForSeconds(lightningBolt.MinimumDelay);
			if (renderer != null && lightningBolt.IsActive)
			{
				renderer.Begin();
			}
		}

		private LineRendererMesh CreateLineRenderer(LightningBolt lightningBolt, List<LineRendererMesh> lineRenderers)
		{
			LineRendererMesh lineRendererMesh;
			if (rendererCache.Count == 0)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "LightningBoltMeshRenderer";
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				lineRendererMesh = gameObject.AddComponent<LineRendererMesh>();
			}
			else
			{
				lineRendererMesh = rendererCache[rendererCache.Count - 1];
				rendererCache.RemoveAt(rendererCache.Count - 1);
			}
			lineRendererMesh.gameObject.transform.parent = lightningBolt.Parent.transform;
			if (lightningBolt.UseWorldSpace)
			{
				lineRendererMesh.gameObject.transform.position = Vector3.zero;
			}
			else
			{
				lineRendererMesh.gameObject.transform.localPosition = Vector3.zero;
			}
			lineRendererMesh.gameObject.transform.rotation = Quaternion.identity;
			lineRendererMesh.gameObject.transform.localScale = Vector3.one;
			lineRendererMesh.Material = ((!lightningBolt.HasGlow) ? MaterialNoGlow : Material);
			lineRendererMesh.SortingLayerName = Script.SortingLayerName;
			lineRendererMesh.SortingOrder = Script.SortingOrder;
			currentLineRenderer = lineRendererMesh;
			lineRenderers.Add(lineRendererMesh);
			return lineRendererMesh;
		}

		public void Begin(LightningBolt lightningBolt)
		{
			List<LineRendererMesh> value;
			if (!renderers.TryGetValue(lightningBolt, out value))
			{
				value = new List<LineRendererMesh>();
				renderers[lightningBolt] = value;
				CreateLineRenderer(lightningBolt, value);
			}
		}

		public void End(LightningBolt lightningBolt)
		{
			if (currentLineRenderer != null)
			{
				Script.StartCoroutine(EnableRenderer(currentLineRenderer, lightningBolt));
				currentLineRenderer = null;
			}
		}

		public void AddGroup(LightningBolt lightningBolt, LightningBoltSegmentGroup group, float growthMultiplier)
		{
			List<LineRendererMesh> list = renderers[lightningBolt];
			LineRendererMesh lineRendererMesh = list[list.Count - 1];
			float num = Time.timeSinceLevelLoad + group.Delay;
			Color c = new Color(num, num + group.PeakStart, num + group.PeakEnd, num + group.LifeTime);
			float num2 = group.LineWidth * 0.5f;
			int num3 = group.Segments.Count - group.StartIndex;
			float num4 = (num2 - num2 * group.EndWidthMultiplier) / (float)num3;
			float num5;
			float num6;
			if (growthMultiplier > 0f)
			{
				num5 = group.LifeTime / (float)num3 * growthMultiplier;
				num6 = 0f;
			}
			else
			{
				num5 = 0f;
				num6 = 0f;
			}
			if (!lineRendererMesh.PrepareForLines(num3))
			{
				Script.StartCoroutine(EnableRenderer(lineRendererMesh, lightningBolt));
				lineRendererMesh = CreateLineRenderer(lightningBolt, list);
			}
			lineRendererMesh.BeginLine(group.Segments[group.StartIndex].Start, group.Segments[group.StartIndex].End, num2, c, GlowWidthMultiplier, GlowIntensityMultiplier);
			for (int i = group.StartIndex + 1; i < group.Segments.Count; i++)
			{
				num2 -= num4;
				if (growthMultiplier < 1f)
				{
					num6 += num5;
					c = new Color(num + num6, num + group.PeakStart + num6, num + group.PeakEnd, num + group.LifeTime);
				}
				lineRendererMesh.AppendLine(group.Segments[i].Start, group.Segments[i].End, num2, c, GlowWidthMultiplier, GlowIntensityMultiplier);
			}
		}

		public void Cleanup(LightningBolt lightningBolt)
		{
			List<LineRendererMesh> value;
			if (!renderers.TryGetValue(lightningBolt, out value))
			{
				return;
			}
			renderers.Remove(lightningBolt);
			foreach (LineRendererMesh item in value)
			{
				rendererCache.Add(item);
				item.Reset();
			}
		}
	}
}
