using System;
using UIFigures.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIArcBorder : UIFigure
	{
		[SerializeField]
		private bool _UseEndColor;

		[SerializeField]
		private Color _EndColor;

		[SerializeField]
		private float _Width = 10f;

		[Range(3f, 360f)]
		[SerializeField]
		private int _Segments = 10;

		[Range(0f, (float)Math.PI * 2f)]
		[SerializeField]
		private float _From;

		[SerializeField]
		[Range(0f, (float)Math.PI * 2f)]
		private float _To = (float)Math.PI / 2f;

		public float Width
		{
			get
			{
				return _Width;
			}
			set
			{
				_Width = value;
			}
		}

		public int Segments
		{
			set
			{
				_Segments = value;
			}
		}

		public float From
		{
			get
			{
				return _From;
			}
			set
			{
				_From = value;
			}
		}

		public float To
		{
			get
			{
				return _To;
			}
			set
			{
				_To = value;
			}
		}

		public Color SetAllColor
		{
			set
			{
				color = new Color(value.r, value.g, value.b, value.a);
				_EndColor = new Color(value.r, value.g, value.b, value.a);
			}
		}

		protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
		{
			base.OnPopulateMesh(p_vertexHelper);
			Vector2 p_position = (_BottomLeft + _UpRight) * 0.5f;
			Vector2 p_radius = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			DrawFunctions.DrawCone(p_vertexHelper, p_position, p_radius, _Width, _To, _From, _Segments, color, (!_UseEndColor) ? color : _EndColor);
		}
	}
}
