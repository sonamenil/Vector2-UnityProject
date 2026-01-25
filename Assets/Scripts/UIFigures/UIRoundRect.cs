using System;
using UIFigures.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIRoundRect : UIRoundRectBorder
	{
		protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
		{
			base.OnPopulateMesh(p_vertexHelper);
			Vector2 p_center = (_BottomLeft + _UpRight) * 0.5f;
			Vector2 p_size = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			Draw(p_vertexHelper, p_center, p_size);
		}

		public new void Draw(VertexHelper p_vertexHelper, Vector2 p_center, Vector2 p_size)
		{
			p_vertexHelper.Clear();
			_Vertexes.Clear();
			AddVertex(p_center);
			AddArc(new Vector2(p_size.x - _RadiusUpRight, p_size.y - _RadiusUpRight), _RadiusUpRight, 0f, false);
			AddArc(new Vector2(0f - p_size.x + _RadiusUpLeft, p_size.y - _RadiusUpLeft), _RadiusUpLeft, (float)Math.PI / 2f, false);
			AddArc(new Vector2(0f - p_size.x + _RadiusBottomLeft, 0f - p_size.y + _RadiusBottomLeft), _RadiusBottomLeft, (float)Math.PI, false);
			AddArc(new Vector2(p_size.x - _RadiusBottomRight, 0f - p_size.y + _RadiusBottomRight), _RadiusBottomRight, 4.712389f, false);
			AddVertex(new Vector2(p_size.x, p_size.y - _RadiusUpRight));
			p_vertexHelper.AddUIVertexStream(_Vertexes, FigureTopology.TringleFanIndices((_Sectors + 1) * 4));
		}
	}
}
