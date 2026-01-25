using System;
using UnityEngine;

namespace Nekki.Vector.GUI.Scripts
{
	[ExecuteInEditMode]
	public class UIAspectRatioFitter : MonoBehaviour
	{
		[Serializable]
		public class AspectRatioTransform
		{
			public Vector2 AspectRatio;

			public Vector3 Position;

			public Vector3 Scale;

			public Vector2 Size;

			public float Aspect
			{
				get
				{
					return AspectRatio.x / AspectRatio.y;
				}
			}

			public AspectRatioTransform()
				: this(4f, 3f, Vector3.zero, Vector3.one)
			{
			}

			public AspectRatioTransform(float aspectRatioWidth, float aspectRatioHeight, Vector3 position, Vector3 scale)
			{
				AspectRatio.x = aspectRatioWidth;
				AspectRatio.y = aspectRatioHeight;
				Position = position;
				Scale = scale;
			}
		}

		[SerializeField]
		private bool _CalculateSize;

		[SerializeField]
		private bool _RunOnlyOnce = true;

		[SerializeField]
		private AspectRatioTransform _From = new AspectRatioTransform(4f, 3f, Vector3.zero, Vector3.one);

		[SerializeField]
		private AspectRatioTransform _To = new AspectRatioTransform(16f, 9f, Vector3.zero, Vector3.one);

		private RectTransform _RectTransform;

		private void Awake()
		{
			_RectTransform = GetComponent<RectTransform>();
			Calculate();
		}

		private void Start()
		{
			Calculate();
		}

		private void Update()
		{
			if (!_RunOnlyOnce)
			{
				Calculate();
			}
		}

		public void Calculate()
		{
			float num = Screen.width;
			float num2 = Screen.height;
			float num3 = Mathf.Clamp(num / num2, _From.Aspect, _To.Aspect);
			float t = Mathf.Abs(num3 - _From.Aspect) / Mathf.Abs(_To.Aspect - _From.Aspect);
			_RectTransform.localPosition = Vector3.Lerp(_From.Position, _To.Position, t);
			_RectTransform.localScale = Vector3.Lerp(_From.Scale, _To.Scale, t);
			if (_CalculateSize)
			{
				_RectTransform.sizeDelta = Vector2.Lerp(_From.Size, _To.Size, t);
			}
		}

		[ContextMenu("Save Current Transform As From")]
		private void SaveCurrentAsFrom()
		{
			_From.Position = _RectTransform.localPosition;
			_From.Scale = _RectTransform.localScale;
			_From.Size = _RectTransform.sizeDelta;
		}

		[ContextMenu("Save Current Transform As To")]
		private void SaveCurrentAsTo()
		{
			_To.Position = _RectTransform.localPosition;
			_To.Scale = _RectTransform.localScale;
			_To.Size = _RectTransform.sizeDelta;
		}
	}
}
