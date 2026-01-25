using System;
using System.Collections;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class ThunderAndLightningScript : MonoBehaviour
	{
		private class LightningBoltHandler
		{
			private ThunderAndLightningScript script;

			public LightningBoltHandler(ThunderAndLightningScript script)
			{
				this.script = script;
				CalculateNextLightningTime();
			}

			private void UpdateLighting()
			{
				if (script.lightningInProgress)
				{
					return;
				}
				if (script.ModifySkyboxExposure)
				{
					script.skyboxExposureStorm = 0.35f;
					if (script.skyboxMaterial != null)
					{
						script.skyboxMaterial.SetFloat("_Exposure", script.skyboxExposureStorm);
					}
				}
				CheckForLightning();
			}

			private void CalculateNextLightningTime()
			{
				script.nextLightningTime = Time.time + UnityEngine.Random.Range(script.LightningIntervalTimeRange.x, script.LightningIntervalTimeRange.y);
				script.lightningInProgress = false;
				if (script.ModifySkyboxExposure)
				{
					script.skyboxMaterial.SetFloat("_Exposure", script.skyboxExposureStorm);
				}
			}

			private IEnumerator CalculateNextLightningLater(float duration)
			{
				yield return new WaitForSeconds(duration);
				CalculateNextLightningTime();
			}

			public IEnumerator ProcessLightning(bool intense, bool visible)
			{
				script.lightningInProgress = true;
				float intensity;
				float sleepTime;
				AudioClip[] sounds;
				if (intense)
				{
					float percent = UnityEngine.Random.Range(0f, 1f);
					intensity = Mathf.Lerp(2f, 8f, percent);
					sleepTime = 5f / intensity;
					script.lightningDuration = UnityEngine.Random.Range(script.LightningShowTimeRange.x, script.LightningShowTimeRange.y);
					sounds = script.ThunderSoundsIntense;
				}
				else
				{
					float percent2 = UnityEngine.Random.Range(0f, 1f);
					intensity = Mathf.Lerp(0f, 2f, percent2);
					sleepTime = 30f / intensity;
					script.lightningDuration = UnityEngine.Random.Range(script.LightningShowTimeRange.x, script.LightningShowTimeRange.y);
					sounds = script.ThunderSoundsNormal;
				}
				if (script.skyboxMaterial != null && script.ModifySkyboxExposure)
				{
					script.skyboxMaterial.SetFloat("_Exposure", Mathf.Max(intensity * 0.5f, script.skyboxExposureStorm));
				}
				int num = UnityEngine.Random.Range(0, 100);
				int count = ((num > 95) ? 3 : ((num <= 80) ? 1 : 2));
				float duration = ((count != 1) ? (script.lightningDuration / (float)count * UnityEngine.Random.Range(1.05f, 1.25f)) : script.lightningDuration);
				script.lightningDuration *= count;
				Strike(intense, script.Generations, duration, intensity, script.ChaosFactor, script.GlowIntensity, script.GlowWidthMultiplier, script.Forkedness, count, script.Camera, (!visible) ? null : script.Camera);
				script.StartCoroutine(CalculateNextLightningLater(duration));
				if (intensity >= 1f && sounds != null && sounds.Length != 0)
				{
					yield return new WaitForSeconds(sleepTime);
					AudioClip clip = null;
					do
					{
						clip = sounds[UnityEngine.Random.Range(0, sounds.Length - 1)];
					}
					while (sounds.Length > 1 && clip == script.lastThunderSound);
					script.lastThunderSound = clip;
					script.audioSourceThunder.PlayOneShot(clip, intensity * 0.5f);
				}
			}

			private void Strike(bool intense, int generations, float duration, float intensity, float chaosFactor, float glowIntensity, float glowWidth, float forkedness, int count, Camera camera, Camera visibleInCamera)
			{
				if (count < 1)
				{
					return;
				}
				System.Random random = new System.Random();
				float min = ((!intense) ? (-5000f) : (-1000f));
				float max = ((!intense) ? 5000f : 1000f);
				float num = ((!intense) ? 2500f : 500f);
				float num2 = ((UnityEngine.Random.Range(0, 2) != 0) ? UnityEngine.Random.Range(num, max) : UnityEngine.Random.Range(min, 0f - num));
				float y = 620f;
				float num3 = ((UnityEngine.Random.Range(0, 2) != 0) ? UnityEngine.Random.Range(num, max) : UnityEngine.Random.Range(min, 0f - num));
				float num4 = 0f;
				Vector3 vector = script.Camera.transform.position;
				vector.x += num2;
				vector.y = y;
				vector.z += num3;
				if (visibleInCamera != null)
				{
					Quaternion rotation = visibleInCamera.transform.rotation;
					visibleInCamera.transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
					float x = UnityEngine.Random.Range((float)visibleInCamera.pixelWidth * 0.1f, (float)visibleInCamera.pixelWidth * 0.9f);
					float z = UnityEngine.Random.Range(visibleInCamera.nearClipPlane + num + num, max);
					Vector3 vector2 = visibleInCamera.ScreenToWorldPoint(new Vector3(x, 0f, z));
					vector = vector2;
					vector.y = y;
					visibleInCamera.transform.rotation = rotation;
				}
				while (count-- > 0)
				{
					Vector3 vector3 = vector;
					num2 = UnityEngine.Random.Range(-100f, 100f);
					y = ((UnityEngine.Random.Range(0, 4) != 0) ? (-1f) : UnityEngine.Random.Range(-1f, 600f));
					num3 += UnityEngine.Random.Range(-100f, 100f);
					vector3.x += num2;
					vector3.y = y;
					vector3.z += num3;
					vector3.x += num * camera.transform.forward.x;
					vector3.z += num * camera.transform.forward.z;
					while ((vector - vector3).magnitude < 500f)
					{
						vector3.x += num * camera.transform.forward.x;
						vector3.z += num * camera.transform.forward.z;
					}
					if (script.LightningBoltScript != null)
					{
						if (UnityEngine.Random.value < script.CloudLightningChance)
						{
							generations = 0;
						}
						LightningBoltParameters lightningBoltParameters = new LightningBoltParameters();
						lightningBoltParameters.Start = vector;
						lightningBoltParameters.End = vector3;
						lightningBoltParameters.Generations = generations;
						lightningBoltParameters.LifeTime = duration;
						lightningBoltParameters.Delay = num4;
						lightningBoltParameters.ChaosFactor = chaosFactor;
						lightningBoltParameters.TrunkWidth = 8f;
						lightningBoltParameters.EndWidthMultiplier = 0.25f;
						lightningBoltParameters.GlowIntensity = glowIntensity;
						lightningBoltParameters.GlowWidthMultiplier = glowWidth;
						lightningBoltParameters.Forkedness = forkedness;
						lightningBoltParameters.Random = random;
						lightningBoltParameters.LightParameters = new LightningLightParameters
						{
							LightIntensity = intensity,
							LightRange = 5000f,
							LightShadowPercent = 1f
						};
						LightningBoltParameters p = lightningBoltParameters;
						script.LightningBoltScript.CreateLightningBolt(p);
						num4 += duration / (float)count * UnityEngine.Random.Range(0.2f, 0.5f);
					}
				}
			}

			private void CheckForLightning()
			{
				if (Time.time >= script.nextLightningTime)
				{
					bool intense = UnityEngine.Random.value < script.LightningIntenseProbability;
					script.StartCoroutine(ProcessLightning(intense, script.LightningAlwaysVisible));
				}
			}

			public void Update()
			{
				UpdateLighting();
			}
		}

		[Tooltip("Lightning bolt script - optional, leave null if you don't want lightning bolts")]
		public LightningBoltScript LightningBoltScript;

		[Tooltip("Camera where the lightning should be centered over. Defaults to main camera.")]
		public Camera Camera;

		[Tooltip("Random duration that the scene will light up - intense lightning extends this a little.")]
		public Vector2 LightningShowTimeRange = new Vector2(0.2f, 0.5f);

		[Tooltip("Random interval between strikes.")]
		public Vector2 LightningIntervalTimeRange = new Vector2(10f, 25f);

		[Range(0f, 1f)]
		[Tooltip("Probability (0-1) of an intense lightning bolt that hits really close. Intense lightning has increased brightness and louder thunder compared to normal lightning, and the thunder sounds plays a lot sooner.")]
		public float LightningIntenseProbability = 0.2f;

		[Tooltip("Sounds to play for normal thunder. One will be chosen at random for each lightning strike. Depending on intensity, some normal lightning may not play a thunder sound.")]
		public AudioClip[] ThunderSoundsNormal;

		[Tooltip("Sounds to play for intense thunder. One will be chosen at random for each lightning strike.")]
		public AudioClip[] ThunderSoundsIntense;

		[Tooltip("Whether lightning strikes should always try to be in the camera view")]
		public bool LightningAlwaysVisible = true;

		[Range(0f, 1f)]
		[Tooltip("The chance lightning will simply be in the clouds with no visible bolt")]
		public float CloudLightningChance = 0.5f;

		[Tooltip("Whether to modify the skybox exposure when lightning is created")]
		public bool ModifySkyboxExposure;

		[Range(0f, 1f)]
		[Tooltip("How much the lightning should glow, 0 for none 1 for full glow")]
		public float GlowIntensity = 0.1f;

		[Tooltip("How the glow width should be multiplied, 0 for none, 64 is max")]
		[Range(0f, 64f)]
		public float GlowWidthMultiplier = 4f;

		[Range(0f, 1f)]
		[Tooltip("How forked the lightning should be. 0 for none, 1 for LOTS of forks.")]
		public float Forkedness = 0.5f;

		[Range(0f, 1f)]
		[Tooltip("How chaotic is the lightning? Higher numbers make more chaotic lightning.")]
		public float ChaosFactor = 0.2f;

		[Range(4f, 8f)]
		[Tooltip("Number of generations. The higher the number, the more detailed the lightning is, but more expensive to create.")]
		public int Generations = 6;

		private float skyboxExposureOriginal;

		private float skyboxExposureStorm;

		private float nextLightningTime;

		private float lightningDuration;

		private bool lightningInProgress;

		private AudioSource audioSourceThunder;

		private LightningBoltHandler lightningBoltHandler;

		private Material skyboxMaterial;

		private AudioClip lastThunderSound;

		public float SkyboxExposureOriginal
		{
			get
			{
				return skyboxExposureOriginal;
			}
		}

		private void Start()
		{
			if (Camera == null)
			{
				Camera = Camera.main;
			}
			if (Camera.farClipPlane < 10000f)
			{
				Debug.LogWarning("Far clip plane should be 10000+ for best lightning effects");
			}
			if (RenderSettings.skybox != null)
			{
				Material material2 = (RenderSettings.skybox = new Material(RenderSettings.skybox));
				skyboxMaterial = material2;
			}
			skyboxExposureOriginal = (skyboxExposureStorm = ((!(skyboxMaterial == null)) ? skyboxMaterial.GetFloat("_Exposure") : 1f));
			audioSourceThunder = base.gameObject.AddComponent<AudioSource>();
			lightningBoltHandler = new LightningBoltHandler(this);
		}

		private void Update()
		{
			if (lightningBoltHandler != null)
			{
				lightningBoltHandler.Update();
			}
		}

		public void CallNormalLightning()
		{
			StartCoroutine(lightningBoltHandler.ProcessLightning(false, true));
		}

		public void CallIntenseLightning()
		{
			StartCoroutine(lightningBoltHandler.ProcessLightning(true, true));
		}
	}
}
