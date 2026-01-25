using System;
using UnityEngine;

namespace Nekki.Vector.Core.CameraEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Nekki/Vector/Darkness")]
	public class DarknessEffect : CameraEffectBase
	{
		[Serializable]
		public class AspectRatioScale
		{
			public Vector2 AspectRatio;

			public float Scale;

			public float Aspect
			{
				get
				{
					return AspectRatio.x / AspectRatio.y;
				}
			}

			public AspectRatioScale(float p_aspectRatioWidth, float p_aspectRatioHeight, float p_scale)
			{
				AspectRatio.x = p_aspectRatioWidth;
				AspectRatio.y = p_aspectRatioHeight;
				Scale = p_scale;
			}
		}

		[Range(0f, 100f)]
		[SerializeField]
		private int _Range = 50;

		[SerializeField]
		[Range(0f, 1f)]
		private float _CenterX = 0.5f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _CenterY = 0.5f;

		[Range(0f, 100f)]
		[SerializeField]
		private int _Alpha = 100;

		[SerializeField]
		private AspectRatioScale _From = new AspectRatioScale(4f, 3f, 1f);

		[SerializeField]
		private AspectRatioScale _To = new AspectRatioScale(16f, 9f, 1f);

		private float _AspectScale = 1f;

		public int Range
		{
			get
			{
				return _Range;
			}
			set
			{
				_Range = Mathf.Clamp(value, 0, 100);
			}
		}

		public float CenterX
		{
			get
			{
				return _CenterX;
			}
			set
			{
				_CenterX = Mathf.Clamp(value, 0f, 1f);
			}
		}

		public float CenterY
		{
			get
			{
				return _CenterY;
			}
			set
			{
				_CenterY = Mathf.Clamp(value, 0f, 1f);
			}
		}

		public Vector2 Center
		{
			get
			{
				return new Vector2(_CenterX, _CenterY);
			}
			set
			{
				CenterX = value.x;
				CenterY = value.y;
			}
		}

		public int Alpha
		{
			get
			{
				return _Alpha;
			}
			set
			{
				_Alpha = Mathf.Clamp(value, 0, 100);
			}
		}

		public override CameraEffectType Type
		{
			get
			{
				return CameraEffectType.Darkness;
			}
		}

		private void Awake()
		{
			CalculateAspectScale();
		}

		private void CalculateAspectScale()
		{
			float num = Mathf.Clamp((float)Screen.width / (float)Screen.height, _From.Aspect, _To.Aspect);
			float t = Mathf.Abs(num - _From.Aspect) / Mathf.Abs(_To.Aspect - _From.Aspect);
			_AspectScale = Mathf.Lerp(_From.Scale, _To.Scale, t);
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (_Alpha == 100)
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetFloat("_Range", (float)_Range * 0.01f);
			base.Material.SetFloat("_CenterX", _CenterX);
			base.Material.SetFloat("_CenterY", _CenterY);
			base.Material.SetFloat("_Alpha", (float)_Alpha * 0.01f);
			base.Material.SetFloat("_ScaleX", _AspectScale);
			Graphics.Blit(source, destination, base.Material);
		}
	}
}
