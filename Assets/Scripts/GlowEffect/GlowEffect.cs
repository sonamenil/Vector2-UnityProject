using UnityEngine;

namespace GlowEffect
{
	public class GlowEffect : MonoBehaviour
	{
		public enum GlowMode
		{
			Glow = 0,
			AlphaGlow = 1,
			SimpleGlow = 2,
			SimpleAlphaGlow = 3
		}

		public enum BlendMode
		{
			Additive = 0,
			Multiply = 1,
			Screen = 2,
			Subtract = 3
		}

		public Material glowMaterial;

		public Shader glowReplaceShader;

		public GlowMode glowMode;

		public BlendMode blendMode;

		public int downsampleSize = 256;

		public int blurIterations = 4;

		public float blurSpread = 1f;

		public float glowStrength = 1.2f;

		public LayerMask ignoreLayers;

		public Color glowColorMultiplier = Color.white;

		private Camera origCamera;

		private Camera shaderCamera;

		private int shaderCullingMask;

		private Rect normalizedRect;

		private RenderTexture replaceRenderTexture;

		public int BlurIterations
		{
			get
			{
				return blurIterations;
			}
			set
			{
				blurIterations = value;
			}
		}

		public float BlurSpread
		{
			get
			{
				return blurSpread;
			}
			set
			{
				blurSpread = value;
				UpdateGlowMaterial();
			}
		}

		public float GlowStrength
		{
			get
			{
				return glowStrength;
			}
			set
			{
				glowStrength = value;
				UpdateGlowMaterial();
			}
		}

		public Color GlowColorMultiplier
		{
			get
			{
				return glowColorMultiplier;
			}
			set
			{
				glowColorMultiplier = value;
				UpdateGlowMaterial();
			}
		}

		public void Awake()
		{
			origCamera = GetComponent<Camera>();
		}

		public void Start()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				AdvLog.Log("Disabling the Glow Effect. Image effects are not supported (do you have Unity Pro?)");
				base.enabled = false;
			}
			normalizedRect = new Rect(0f, 0f, 1f, 1f);
		}

		public void OnEnable()
		{
			switch (blendMode)
			{
			case BlendMode.Additive:
				Shader.EnableKeyword("GLOWEFFECT_BLEND_ADDITIVE");
				break;
			case BlendMode.Multiply:
				Shader.EnableKeyword("GLOWEFFECT_BLEND_MULTIPLY");
				break;
			case BlendMode.Screen:
				Shader.EnableKeyword("GLOWEFFECT_BLEND_SCREEN");
				break;
			case BlendMode.Subtract:
				Shader.EnableKeyword("GLOWEFFECT_BLEND_SUBTRACT");
				break;
			}
			if ((int)glowMode % 2 == 0)
			{
				replaceRenderTexture = new RenderTexture(origCamera.pixelWidth, origCamera.pixelHeight, 16, RenderTextureFormat.ARGB32);
				replaceRenderTexture.wrapMode = TextureWrapMode.Clamp;
				replaceRenderTexture.useMipMap = false;
				replaceRenderTexture.filterMode = FilterMode.Bilinear;
				replaceRenderTexture.Create();
				glowMaterial.SetTexture("_Glow", replaceRenderTexture);
				shaderCamera = new GameObject("Glow Effect", typeof(Camera)).GetComponent<Camera>();
				shaderCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
				shaderCullingMask = ~(int)ignoreLayers;
			}
			UpdateGlowMaterial();
		}

		private void UpdateGlowMaterial()
		{
			glowMaterial.SetFloat("_BlurSpread", blurSpread);
			glowMaterial.SetFloat("_GlowStrength", glowStrength);
			glowMaterial.SetColor("_GlowColorMultiplier", glowColorMultiplier);
		}

		public void OnDisable()
		{
			glowMaterial.mainTexture = null;
			origCamera.targetTexture = null;
			Object.DestroyObject(shaderCamera);
			DisableShaderKeywords();
		}

		public void OnPreRender()
		{
			if ((int)glowMode % 2 == 0)
			{
				shaderCamera.CopyFrom(origCamera);
				shaderCamera.backgroundColor = Color.clear;
				shaderCamera.clearFlags = CameraClearFlags.Color;
				shaderCamera.renderingPath = RenderingPath.Forward;
				shaderCamera.targetTexture = replaceRenderTexture;
				shaderCamera.rect = normalizedRect;
				shaderCamera.cullingMask = shaderCullingMask;
				shaderCamera.RenderWithShader(glowReplaceShader, "RenderType");
			}
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			CalculateGlow(source, destination);
		}

		private void CalculateGlow(RenderTexture source, RenderTexture destination)
		{
			if (glowMode < GlowMode.SimpleGlow)
			{
				RenderTexture temporary = RenderTexture.GetTemporary(downsampleSize, downsampleSize, 0, RenderTextureFormat.ARGB32);
				RenderTexture temporary2 = RenderTexture.GetTemporary(downsampleSize, downsampleSize, 0, RenderTextureFormat.ARGB32);
				if (blurIterations % 2 == 0)
				{
					glowMaterial.SetTexture("_Glow", temporary);
				}
				else
				{
					glowMaterial.SetTexture("_Glow", temporary2);
				}
				if ((int)glowMode % 2 == 1)
				{
					Graphics.Blit(source, temporary2, glowMaterial, 2);
				}
				else
				{
					Graphics.Blit(replaceRenderTexture, temporary2, glowMaterial, 1);
				}
				for (int i = 1; i < blurIterations; i++)
				{
					if (i % 2 == 0)
					{
						temporary2.DiscardContents();
						Graphics.Blit(temporary, temporary2, glowMaterial, 1);
					}
					else
					{
						temporary.DiscardContents();
						Graphics.Blit(temporary2, temporary, glowMaterial, 1);
					}
				}
				Graphics.Blit(source, destination, glowMaterial, 0);
				RenderTexture.ReleaseTemporary(temporary);
				RenderTexture.ReleaseTemporary(temporary2);
			}
			else
			{
				Graphics.Blit(source, destination, glowMaterial, ((int)glowMode % 2 != 1) ? 3 : 4);
			}
		}

		private void DisableShaderKeywords()
		{
			Shader.DisableKeyword("GLOWEFFECT_BLEND_ADDITIVE");
			Shader.DisableKeyword("GLOWEFFECT_BLEND_SCREEN");
			Shader.DisableKeyword("GLOWEFFECT_BLEND_MULTIPLY");
			Shader.DisableKeyword("GLOWEFFECT_BLEND_SUBTRACT");
			Shader.DisableKeyword("GLOWEFFECT_USE_MAINTEX");
			Shader.DisableKeyword("GLOWEFFECT_USE_MAINTEX_OFF");
			Shader.DisableKeyword("GLOWEFFECT_USE_GLOWTEX");
			Shader.DisableKeyword("GLOWEFFECT_USE_GLOWTEX_OFF");
			Shader.DisableKeyword("GLOWEFFECT_USE_GLOWCOLOR");
			Shader.DisableKeyword("GLOWEFFECT_USE_GLOWCOLOR_OFF");
			Shader.DisableKeyword("GLOWEFFECT_USE_VERTEXCOLOR");
			Shader.DisableKeyword("GLOWEFFECT_USE_VERTEXCOLOR_OFF");
			Shader.DisableKeyword("GLOWEFFECT_USE_GLOW_POWER_OFF");
			Shader.DisableKeyword("GLOWEFFECT_USE_GLOW_POWER");
			Shader.DisableKeyword("GLOWEFFECT_MULTIPLY_COLOR");
			Shader.DisableKeyword("GLOWEFFECT_MULTIPLY_COLOR_OFF");
		}
	}
}
