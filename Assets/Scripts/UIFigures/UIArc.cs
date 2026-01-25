using System;
using UIFigures.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIArc : UIFigure
	{
		[SerializeField]
		[Range(2f, 360f)]
		private int _Segments = 10;

		[Range(0f, (float)Math.PI * 2f)]
		[SerializeField]
		private float _From;

		[SerializeField]
		[Range(0f, (float)Math.PI * 2f)]
		private float _To = (float)Math.PI / 2f;

		protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
		{
			base.OnPopulateMesh(p_vertexHelper);
			Vector2 p_position = (_BottomLeft + _UpRight) * 0.5f;
			Vector2 p_radius = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			DrawFunctions.DrawArc(p_vertexHelper, p_position, p_radius, _To, _From, _Segments, color);
		}
	}
}
