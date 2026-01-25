using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltScript : MonoBehaviour
	{
		private const LightningBoltRenderMode defaultRenderMode = LightningBoltRenderMode.MeshRendererLineGlow;

		private LightningBoltMeshRenderer lightningBoltRenderer;

		[Tooltip("The camera the lightning should be shown in. Defaults to the current camera, or the main camera if current camera is null. If you are using a different camera, you may want to put the lightning in it's own layer and cull that layer out of any other cameras.")]
		public Camera Camera;

		[Tooltip("True if you are using world space coordinates for the lightning bolt, false if you are using coordinates relative to the parent game object.")]
		public bool UseWorldSpace = true;

		[Tooltip("Lightning quality setting. This allows setting limits on generations, lights and shadow casting lights based on the global quality setting.")]
		public LightningBoltQualitySetting QualitySetting;

		[Tooltip("Determines how the lightning is rendererd - this needs to be setup before the script starts.")]
		public LightningBoltRenderMode RenderMode = LightningBoltRenderMode.MeshRendererLineGlow;

		[Tooltip("Lightning material for mesh renderer")]
		public Material LightningMaterialMesh;

		[Tooltip("Lightning material for mesh renderer, without glow")]
		public Material LightningMaterialMeshNoGlow;

		[Tooltip("The texture to use for the lightning bolts, or null for the material default texture.")]
		public Texture2D LightningTexture;

		[Tooltip("Particle system to play at the point of emission (start). 'Emission rate' particles will be emitted all at once.")]
		public ParticleSystem LightningOriginParticleSystem;

		[Tooltip("Particle system to play at the point of impact (end). 'Emission rate' particles will be emitted all at once.")]
		public ParticleSystem LightningDestinationParticleSystem;

		[Tooltip("Tint color for the lightning")]
		public Color LightningTintColor = Color.white;

		[Tooltip("Tint color for the lightning glow")]
		public Color GlowTintColor = new Color(0.1f, 0.2f, 1f, 1f);

		[Tooltip("Jitter multiplier to randomize lightning size. Jitter depends on trunk width and will make the lightning move rapidly and jaggedly, giving a more lively and sometimes cartoony feel. Jitter may be shared with other bolts depending on materials. If you need different jitters for the same material, create a second script object.")]
		public float JitterMultiplier;

		[Tooltip("Built in turbulance based on the direction of each segment. Small values usually work better, like 0.2.")]
		public float Turbulence;

		[Tooltip("Global turbulence velocity for this script")]
		public Vector3 TurbulenceVelocity = Vector3.zero;

		[Tooltip("The render queue for the lightning. -1 for default.")]
		public int RenderQueue = -1;

		[Tooltip("Sorting layer")]
		public string SortingLayerName = "Default";

		[Tooltip("Order in sorting layer")]
		public int SortingOrder;

		private Material lightningMaterialMeshInternal;

		private Material lightningMaterialMeshNoGlowInternal;

		private Texture2D lastLightningTexture;

		private LightningBoltRenderMode lastRenderMode = (LightningBoltRenderMode)2147483647;

		private readonly List<LightningBolt> bolts = new List<LightningBolt>();

		private readonly LightningBoltParameters[] oneParameterArray = new LightningBoltParameters[1];

		private void UpdateMaterialsForLastTexture()
		{
			if (Application.isPlaying)
			{
				lightningMaterialMeshInternal = new Material(LightningMaterialMesh);
				lightningMaterialMeshNoGlowInternal = new Material(LightningMaterialMeshNoGlow);
				if (LightningTexture != null)
				{
					lightningMaterialMeshInternal.SetTexture("_MainTex", LightningTexture);
					lightningMaterialMeshNoGlowInternal.SetTexture("_MainTex", LightningTexture);
				}
				lastRenderMode = (LightningBoltRenderMode)2147483647;
				CreateRenderer();
			}
		}

		private void CreateRenderer()
		{
			if (RenderMode != lastRenderMode)
			{
				lastRenderMode = RenderMode;
				lightningBoltRenderer = new LightningBoltMeshRenderer();
				lightningBoltRenderer.MaterialNoGlow = lightningMaterialMeshNoGlowInternal;
				if (RenderMode == LightningBoltRenderMode.MeshRendererSquareBillboardGlow)
				{
					lightningMaterialMeshInternal.DisableKeyword("USE_LINE_GLOW");
				}
				else
				{
					lightningMaterialMeshInternal.EnableKeyword("USE_LINE_GLOW");
				}
				lightningBoltRenderer.Script = this;
				lightningBoltRenderer.Material = lightningMaterialMeshInternal;
			}
		}

		private void UpdateTexture()
		{
			if (LightningTexture != null && LightningTexture != lastLightningTexture)
			{
				lastLightningTexture = LightningTexture;
				UpdateMaterialsForLastTexture();
			}
			else
			{
				CreateRenderer();
			}
		}

		private void UpdateShaderParameters()
		{
			lightningMaterialMeshInternal.SetColor("_TintColor", LightningTintColor);
			lightningMaterialMeshInternal.SetColor("_GlowTintColor", GlowTintColor);
			lightningMaterialMeshInternal.SetFloat("_JitterMultiplier", JitterMultiplier);
			lightningMaterialMeshInternal.SetFloat("_Turbulence", Turbulence);
			lightningMaterialMeshInternal.SetVector("_TurbulenceVelocity", TurbulenceVelocity);
			lightningMaterialMeshInternal.renderQueue = RenderQueue;
			lightningMaterialMeshNoGlowInternal.SetColor("_TintColor", LightningTintColor);
			lightningMaterialMeshNoGlowInternal.SetFloat("_JitterMultiplier", JitterMultiplier);
			lightningMaterialMeshNoGlowInternal.SetFloat("_Turbulence", Turbulence);
			lightningMaterialMeshNoGlowInternal.SetVector("_TurbulenceVelocity", TurbulenceVelocity);
			lightningMaterialMeshNoGlowInternal.renderQueue = RenderQueue;
		}

		private void OnDestroy()
		{
			foreach (LightningBolt bolt in bolts)
			{
				bolt.Cleanup();
			}
		}

		protected virtual void Start()
		{
			if (LightningMaterialMesh == null || LightningMaterialMeshNoGlow == null)
			{
				Debug.LogError("Must assign all lightning materials");
			}
			UpdateMaterialsForLastTexture();
			if (Camera == null)
			{
				Camera = Camera.current;
				if (Camera == null)
				{
					Camera = Camera.main;
				}
			}
		}

		protected virtual void Update()
		{
			if (bolts.Count == 0)
			{
				return;
			}
			UpdateShaderParameters();
			for (int num = bolts.Count - 1; num >= 0; num--)
			{
				if (!bolts[num].Update())
				{
					bolts.RemoveAt(num);
				}
			}
		}

		public virtual void CreateLightningBolt(LightningBoltParameters p)
		{
			if (p != null)
			{
				UpdateTexture();
				oneParameterArray[0] = p;
				LightningBolt orCreateLightningBolt = LightningBolt.GetOrCreateLightningBolt();
				orCreateLightningBolt.Initialize(Camera, UseWorldSpace, QualitySetting, lightningBoltRenderer, base.gameObject, this, LightningOriginParticleSystem, LightningDestinationParticleSystem, oneParameterArray);
				bolts.Add(orCreateLightningBolt);
			}
		}

		public void CreateLightningBolts(ICollection<LightningBoltParameters> parameters)
		{
			if (parameters != null)
			{
				UpdateTexture();
				LightningBolt orCreateLightningBolt = LightningBolt.GetOrCreateLightningBolt();
				orCreateLightningBolt.Initialize(Camera, UseWorldSpace, QualitySetting, lightningBoltRenderer, base.gameObject, this, LightningOriginParticleSystem, LightningDestinationParticleSystem, parameters);
				bolts.Add(orCreateLightningBolt);
			}
		}
	}
}
