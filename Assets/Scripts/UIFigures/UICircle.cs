using System;
using UIFigures.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UICircle : UIFigure
	{
		[Range(3f, 150f)]
		[SerializeField]
		private int _Segments = 10;

		protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
		{
			base.OnPopulateMesh(p_vertexHelper);
			Vector2 p_position = (_BottomLeft + _UpRight) * 0.5f;
			Vector2 p_radius = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			DrawFunctions.DrawArc(p_vertexHelper, p_position, p_radius, 0f, (float)Math.PI * 2f, _Segments, color);
		}
	}
}
