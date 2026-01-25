using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Blur/Blur (Optimized)")]
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class BlurOptimized : PostEffectsBase
	{
		public enum BlurType
		{
			StandardGauss = 0,
			SgxGauss = 1
		}

		[Range(0f, 2f)]
		public int downsample = 1;

		[Range(0f, 10f)]
		public float blurSize = 3f;

		[Range(1f, 4f)]
		public int blurIterations = 2;

		public BlurType blurType;

		public Shader blurShader;

		private Material blurMaterial;

		private RawImage _Image;

		public void SetTexturAndTurnOff(RawImage p_image)
		{
			_Image = p_image;
			base.enabled = true;
		}

		public override bool CheckResources()
		{
			CheckSupport(false);
			blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
			if (!isSupported)
			{
				ReportAutoDisable();
			}
			return isSupported;
		}

		public void OnDisable()
		{
			if ((bool)blurMaterial)
			{
				UnityEngine.Object.DestroyImmediate(blurMaterial);
			}
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			float num = 1f / (1f * (float)(1 << downsample));
			blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num, (0f - blurSize) * num, 0f, 0f));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width >> downsample;
			int height = source.height >> downsample;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, blurMaterial, 0);
			int num2 = ((blurType != 0) ? 2 : 0);
			for (int i = 0; i < blurIterations; i++)
			{
				float num3 = (float)i * 1f;
				blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num + num3, (0f - blurSize) * num - num3, 0f, 0f));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, blurMaterial, 1 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, blurMaterial, 2 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			Graphics.Blit(renderTexture, destination);
			if (_Image != null)
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = renderTexture;
				Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
				texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
				texture2D.Compress(true);
				texture2D.filterMode = FilterMode.Point;
				texture2D.anisoLevel = 0;
				texture2D.Apply();
				RenderTexture.active = active;
				_Image.texture = texture2D;
				base.enabled = false;
				_Image.gameObject.SetActive(true);
				_Image = null;
			}
			RenderTexture.ReleaseTemporary(renderTexture);
		}

		public void Reset(RawImage p_image)
		{
			p_image.gameObject.SetActive(false);
			p_image.texture = null;
			GC.Collect();
		}
	}
}
