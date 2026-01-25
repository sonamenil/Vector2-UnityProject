using UnityEngine;

namespace MKGlowSystem
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class MKGlow : MonoBehaviour
	{
		private static float[] gaussFilter = new float[11]
		{
			0.402f, 0.623f, 0.877f, 1.12f, 1.297f, 1.362f, 1.297f, 1.12f, 0.877f, 0.623f,
			0.402f
		};

		[SerializeField]
		private Shader m_NoGarbageBlurShader;

		[SerializeField]
		private Shader m_BlurShader;

		[SerializeField]
		private Shader m_CompositeShader;

		[SerializeField]
		private Shader m_GlowRenderShader;

		[SerializeField]
		private Shader m_FSDSShader;

		private Material m_CompositeMaterial;

		private Material m_FastBlurMaterial;

		private Material m_BlurMaterial;

		private Material m_FSDSMaterial;

		private Camera m_GlowCamera;

		private GameObject m_GlowCameraObject;

		private RenderTexture m_GlowTexture;

		[Tooltip("The Glows blur calculation")]
		[SerializeField]
		private GlowBlurCurve m_GlowCurve = GlowBlurCurve.Gauss;

		[SerializeField]
		[Tooltip("Renderlayer that should glow (only selective glow)")]
		private LayerMask m_GlowRenderLayer = -1;

		[Tooltip("The resolution of the rendered glow")]
		[SerializeField]
		private MKGlowMode m_GlowResolution;

		[SerializeField]
		[Tooltip("Show glow through Cutout rendered objects")]
		private bool m_ShowCutoutGlow;

		[SerializeField]
		[Tooltip("Show glow through Transparent rendered objects")]
		private bool m_ShowTransparentGlow = true;

		[SerializeField]
		[Tooltip("Selective = to specifically bring objects to glow, Fullscreen = complete screen glows")]
		private MKGlowType m_GlowType;

		[SerializeField]
		[Tooltip("The main difference between Low and High is that Low has less Garbage Collection")]
		private MKGlowQuality m_GlowQuality = MKGlowQuality.High;

		[SerializeField]
		[Tooltip("The glows coloration in full screen mode (only FullscreenGlowType)")]
		private Color m_FullScreenGlowTint = new Color(1f, 1f, 1f, 0f);

		[SerializeField]
		[Tooltip("Width of the glow effect")]
		private float m_BlurSpread = 0.35f;

		[Tooltip("Number of used blurs")]
		[SerializeField]
		private int m_BlurIterations = 7;

		[Tooltip("The global luminous intensity")]
		[SerializeField]
		private float m_GlowIntensity = 0.3f;

		[SerializeField]
		[Tooltip("Distance to the object per blur")]
		private float m_BlurOffset;

		[SerializeField]
		[Tooltip("Significantly influences the blurs quality (recommended: 4)")]
		private int m_Samples = 2;

		private GameObject GlowCameraObject
		{
			get
			{
				if (!m_GlowCameraObject)
				{
					m_GlowCameraObject = new GameObject("m_GlowCameraObject");
					m_GlowCameraObject.hideFlags = HideFlags.HideAndDontSave;
					m_GlowCameraObject.AddComponent<Camera>();
					GlowCamera.orthographic = false;
					GlowCamera.enabled = false;
					GlowCamera.renderingPath = RenderingPath.VertexLit;
					GlowCamera.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_GlowCameraObject;
			}
		}

		private Camera GlowCamera
		{
			get
			{
				if (m_GlowCamera == null)
				{
					m_GlowCamera = GlowCameraObject.GetComponent<Camera>();
				}
				return m_GlowCamera;
			}
		}

		public GlowBlurCurve GlowCurve
		{
			get
			{
				return m_GlowCurve;
			}
			set
			{
				m_GlowCurve = value;
			}
		}

		public LayerMask GlowRenderLayer
		{
			get
			{
				return m_GlowRenderLayer;
			}
			set
			{
				m_GlowRenderLayer = value;
			}
		}

		public bool ShowCutoutGlow
		{
			get
			{
				return m_ShowCutoutGlow;
			}
			set
			{
				m_ShowCutoutGlow = value;
			}
		}

		public MKGlowMode GlowResolution
		{
			get
			{
				return m_GlowResolution;
			}
			set
			{
				m_GlowResolution = value;
			}
		}

		public bool ShowTransparentGlow
		{
			get
			{
				return m_ShowTransparentGlow;
			}
			set
			{
				m_ShowTransparentGlow = value;
			}
		}

		public MKGlowQuality GlowQuality
		{
			get
			{
				return m_GlowQuality;
			}
			set
			{
				m_GlowQuality = value;
			}
		}

		public MKGlowType GlowType
		{
			get
			{
				return m_GlowType;
			}
			set
			{
				m_GlowType = value;
			}
		}

		public Color FullScreenGlowTint
		{
			get
			{
				return m_FullScreenGlowTint;
			}
			set
			{
				m_FullScreenGlowTint = value;
			}
		}

		public int Samples
		{
			get
			{
				return m_Samples;
			}
			set
			{
				m_Samples = value;
			}
		}

		public int BlurIterations
		{
			get
			{
				return m_BlurIterations;
			}
			set
			{
				m_BlurIterations = value;
			}
		}

		public float BlurOffset
		{
			get
			{
				return m_BlurOffset;
			}
			set
			{
				m_BlurOffset = value;
			}
		}

		public float GlowIntensity
		{
			get
			{
				return m_GlowIntensity;
			}
			set
			{
				m_GlowIntensity = value;
			}
		}

		public float BlurSpread
		{
			get
			{
				return m_BlurSpread;
			}
			set
			{
				m_BlurSpread = value;
			}
		}

		internal Material FSDSMaterial
		{
			get
			{
				if (!m_FSDSMaterial)
				{
					m_FSDSMaterial = new Material(m_FSDSShader);
					m_FSDSMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_FSDSMaterial;
			}
		}

		internal Material FastBlurMaterial
		{
			get
			{
				if (!m_FastBlurMaterial)
				{
					m_FastBlurMaterial = new Material(m_NoGarbageBlurShader);
					m_FastBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_FastBlurMaterial;
			}
		}

		internal Material BlurMaterial
		{
			get
			{
				if (m_BlurMaterial == null)
				{
					m_BlurMaterial = new Material(m_BlurShader);
					m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_BlurMaterial;
			}
		}

		internal Material CompositeMaterial
		{
			get
			{
				if (m_CompositeMaterial == null)
				{
					m_CompositeMaterial = new Material(m_CompositeShader);
					m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_CompositeMaterial;
			}
		}

		private void Main()
		{
			if (m_GlowRenderShader == null)
			{
				base.enabled = false;
				Debug.LogWarning("Failed to load MKGlow Render Shader");
			}
			else if (m_CompositeShader == null)
			{
				base.enabled = false;
				Debug.LogWarning("Failed to load MKGlow Composite Shader");
			}
			else if (m_BlurShader == null)
			{
				base.enabled = false;
				Debug.LogWarning("Failed to load MKGlow Blur Shader");
			}
			else if (m_NoGarbageBlurShader == null)
			{
				base.enabled = false;
				Debug.LogWarning("Failed to load MKGlow Fast Blur Shader");
			}
		}

		private void OnEnable()
		{
			SetupShaders();
		}

		private void Reset()
		{
			SetupShaders();
		}

		private void SetupKeywords()
		{
			if (ShowTransparentGlow)
			{
				Shader.EnableKeyword("MKTRANSPARENT_ON");
				Shader.DisableKeyword("MKTRANSPARENT_OFF");
			}
			else
			{
				Shader.DisableKeyword("MKTRANSPARENT_ON");
				Shader.EnableKeyword("MKTRANSPARENT_OFF");
			}
			if (ShowCutoutGlow)
			{
				Shader.EnableKeyword("MKCUTOUT_ON");
				Shader.DisableKeyword("MKCUTOUT_OFF");
			}
			else
			{
				Shader.DisableKeyword("MKCUTOUT_ON");
				Shader.EnableKeyword("MKCUTOUT_OFF");
			}
		}

		private void SetupShaders()
		{
			if (!m_BlurShader)
			{
				m_BlurShader = Shader.Find("Hidden/MKGlowBlur");
			}
			if (!m_NoGarbageBlurShader)
			{
				m_NoGarbageBlurShader = Shader.Find("Hidden/MKGlowFastBlur");
			}
			if (!m_CompositeShader)
			{
				m_CompositeShader = Shader.Find("Hidden/MKGlowCompose");
			}
			if (!m_GlowRenderShader)
			{
				m_GlowRenderShader = Shader.Find("Hidden/MKGlowRender");
			}
			if (!m_FSDSShader)
			{
				m_FSDSShader = Shader.Find("Hidden/MKGlowFullScreenDownSample");
			}
		}

		private void OnDisable()
		{
			if ((bool)m_CompositeMaterial)
			{
				Object.DestroyImmediate(m_CompositeMaterial);
			}
			if ((bool)m_BlurMaterial)
			{
				Object.DestroyImmediate(m_BlurMaterial);
			}
			if ((bool)m_FastBlurMaterial)
			{
				Object.DestroyImmediate(m_FastBlurMaterial);
			}
			if ((bool)m_FSDSMaterial)
			{
				Object.DestroyImmediate(m_FSDSMaterial);
			}
			if ((bool)m_GlowCamera)
			{
				Object.DestroyImmediate(GlowCamera);
			}
			if ((bool)m_GlowCameraObject)
			{
				Object.DestroyImmediate(GlowCameraObject);
			}
			if ((bool)m_GlowTexture)
			{
				RenderTexture.ReleaseTemporary(m_GlowTexture);
				Object.DestroyImmediate(m_GlowTexture);
			}
		}

		private void SetupGlowCamera()
		{
			GlowCamera.CopyFrom(GetComponent<Camera>());
			GlowCamera.clearFlags = CameraClearFlags.Color;
			GlowCamera.rect = new Rect(0f, 0f, 1f, 1f);
			GlowCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			GlowCamera.cullingMask = GlowRenderLayer;
			GlowCamera.targetTexture = m_GlowTexture;
		}

		private void OnPreRender()
		{
			if (!base.gameObject.activeSelf || !base.enabled)
			{
				return;
			}
			if (m_GlowTexture != null)
			{
				RenderTexture.ReleaseTemporary(m_GlowTexture);
				m_GlowTexture = null;
			}
			if (GlowType == MKGlowType.Selective)
			{
				m_GlowTexture = RenderTexture.GetTemporary(GetComponent<Camera>().pixelWidth / CalculateSamples(ref m_GlowResolution), GetComponent<Camera>().pixelHeight / CalculateSamples(ref m_GlowResolution), 16);
				SetupGlowCamera();
				SetupKeywords();
				if (GlowCamera.actualRenderingPath != 0)
				{
					GlowCamera.renderingPath = RenderingPath.VertexLit;
				}
				GlowCamera.RenderWithShader(m_GlowRenderShader, "RenderType");
			}
			else
			{
				if ((bool)GlowCamera)
				{
					Object.DestroyImmediate(GlowCamera);
				}
				if ((bool)GlowCameraObject)
				{
					Object.DestroyImmediate(GlowCameraObject);
				}
			}
			Mathf.Clamp(BlurSpread, 0.2f, 2f);
			Mathf.Clamp(BlurIterations, 0, 11);
			Mathf.Clamp(BlurOffset, 0f, 4f);
			Mathf.Clamp(Samples, 2, 16);
			Mathf.Clamp(GlowIntensity, 0f, 1f);
		}

		protected virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (base.gameObject.activeSelf && base.enabled)
			{
				if (GlowType == MKGlowType.Selective)
				{
					PerformSelectiveGlow(ref src, ref dest);
				}
				else
				{
					PerformFullScreenGlow(ref src, ref dest);
				}
			}
		}

		private void PerformBlur(ref RenderTexture src, ref RenderTexture dest)
		{
			if (GlowQuality == MKGlowQuality.Low)
			{
				float value = BlurOffset + BlurSpread;
				FastBlurMaterial.SetTexture("_MainTex", src);
				FastBlurMaterial.SetFloat("_Shift", value);
				Graphics.Blit(src, dest, FastBlurMaterial);
			}
			else
			{
				int num = 1;
				Graphics.BlitMultiTap(src, dest, BlurMaterial, new Vector2(num, num), new Vector2(-num, num), new Vector2(num, -num), new Vector2(-num, -num));
			}
		}

		private void PerformBlur(ref RenderTexture src, ref RenderTexture dest, int iteration)
		{
			float num = BlurOffset + (float)iteration * BlurSpread;
			if (GlowCurve == GlowBlurCurve.Gauss)
			{
				num *= gaussFilter[iteration];
			}
			if (GlowQuality == MKGlowQuality.Low)
			{
				FastBlurMaterial.SetTexture("_MainTex", src);
				FastBlurMaterial.SetFloat("_Shift", num);
				Graphics.Blit(src, dest, FastBlurMaterial);
			}
			else
			{
				Graphics.BlitMultiTap(src, dest, BlurMaterial, new Vector2(num, num), new Vector2(0f - num, num), new Vector2(num, 0f - num), new Vector2(0f - num, 0f - num));
			}
		}

		private void PerformGlow(ref RenderTexture glowBuffer, ref RenderTexture dest, ref RenderTexture src)
		{
			CompositeMaterial.SetTexture("_GlowTex", src);
			Graphics.Blit(glowBuffer, dest, CompositeMaterial);
		}

		private void DSFS(RenderTexture source, RenderTexture dest)
		{
			FSDSMaterial.color = new Color(FullScreenGlowTint.r, FullScreenGlowTint.g, FullScreenGlowTint.b, FullScreenGlowTint.a);
			Graphics.Blit(source, dest, FSDSMaterial);
		}

		protected void PerformSelectiveGlow(ref RenderTexture source, ref RenderTexture dest)
		{
			Vector2 vector = default(Vector2);
			vector.x = source.width / Samples;
			vector.y = source.height / Samples;
			RenderTexture dest2 = RenderTexture.GetTemporary((int)vector.x, (int)vector.y, 0);
			if (GlowQuality == MKGlowQuality.Low)
			{
				FastBlurMaterial.color = new Color(1f, 1f, 1f, GlowIntensity);
			}
			else
			{
				BlurMaterial.color = new Color(1f, 1f, 1f, GlowIntensity);
			}
			PerformBlur(ref m_GlowTexture, ref dest2);
			for (int i = 0; i < BlurIterations; i++)
			{
				RenderTexture dest3 = RenderTexture.GetTemporary((int)vector.x, (int)vector.y, 0);
				PerformBlur(ref dest2, ref dest3, i);
				RenderTexture.ReleaseTemporary(dest2);
				dest2 = dest3;
			}
			PerformGlow(ref dest2, ref dest, ref source);
			RenderTexture.ReleaseTemporary(dest2);
			if (m_GlowTexture != null)
			{
				RenderTexture.ReleaseTemporary(m_GlowTexture);
				m_GlowTexture = null;
			}
		}

		protected void PerformFullScreenGlow(ref RenderTexture source, ref RenderTexture destination)
		{
			Vector2 vector = default(Vector2);
			vector.x = source.width / Samples;
			vector.y = source.height / Samples;
			RenderTexture src = RenderTexture.GetTemporary((int)vector.x, (int)vector.y, 0);
			if (GlowQuality == MKGlowQuality.Low)
			{
				FastBlurMaterial.color = new Color(1f, 1f, 1f, GlowIntensity);
			}
			else
			{
				BlurMaterial.color = new Color(1f, 1f, 1f, GlowIntensity);
			}
			DSFS(source, src);
			for (int i = 0; i < BlurIterations; i++)
			{
				RenderTexture dest = RenderTexture.GetTemporary((int)vector.x, (int)vector.y, 0);
				PerformBlur(ref src, ref dest, i);
				RenderTexture.ReleaseTemporary(src);
				src = dest;
			}
			Graphics.Blit(source, destination);
			CompositeMaterial.color = new Color(1f, 1f, 1f, Mathf.Clamp01(GlowIntensity));
			PerformGlow(ref src, ref destination, ref source);
			RenderTexture.ReleaseTemporary(src);
		}

		private int CalculateSamples(ref MKGlowMode resolution)
		{
			switch (GlowResolution)
			{
			case MKGlowMode.High:
				return 1;
			case MKGlowMode.Normal:
				return 2;
			case MKGlowMode.Mobile:
				return 4;
			default:
				return 1;
			}
		}
	}
}
