using System;
using System.Collections.Generic;
using UIFigures.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIRoundRectBorder : UIFigure
	{
		[SerializeField]
		protected float _Width = 10f;

		[SerializeField]
		protected float _ChangeAllRadius = 10f;

		private float _ChangeAllRadiusAll = 10f;

		[SerializeField]
		protected float _RadiusUpLeft = 10f;

		[SerializeField]
		protected float _RadiusUpRight = 10f;

		[SerializeField]
		protected float _RadiusBottomRight = 10f;

		[SerializeField]
		protected float _RadiusBottomLeft = 10f;

		[Range(1f, 50f)]
		[SerializeField]
		protected int _Sectors = 10;

		protected List<UIVertex> _Vertexes = new List<UIVertex>();

		protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
		{
			base.OnPopulateMesh(p_vertexHelper);
			Vector2 p_center = (_BottomLeft + _UpRight) * 0.5f;
			Vector2 p_size = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			Draw(p_vertexHelper, p_center, p_size);
		}

		public void Draw(VertexHelper p_vertexHelper, Vector2 p_center, Vector2 p_size)
		{
			p_vertexHelper.Clear();
			_Vertexes.Clear();
			AddArc(new Vector2(p_size.x - _RadiusUpRight, p_size.y - _RadiusUpRight), _RadiusUpRight, 0f);
			AddArc(new Vector2(0f - p_size.x + _RadiusUpLeft, p_size.y - _RadiusUpLeft), _RadiusUpLeft, (float)Math.PI / 2f);
			AddArc(new Vector2(0f - p_size.x + _RadiusBottomLeft, 0f - p_size.y + _RadiusBottomLeft), _RadiusBottomLeft, (float)Math.PI);
			AddArc(new Vector2(p_size.x - _RadiusBottomRight, 0f - p_size.y + _RadiusBottomRight), _RadiusBottomRight, 4.712389f);
			AddVertex(new Vector2(p_size.x, p_size.y - _RadiusUpRight));
			AddVertex(new Vector2(p_size.x - _Width, p_size.y - _RadiusUpRight));
			p_vertexHelper.AddUIVertexStream(_Vertexes, FigureTopology.TringleStripIndices((_Sectors + 1) * 8));
		}

		protected void AddArc(Vector3 p_center, float p_radius, float p_from, bool p_border = true)
		{
			float num = (float)Math.PI / 2f / (float)_Sectors;
			for (int i = 0; i < _Sectors + 1; i++)
			{
				float p_angle = p_from + (float)i * num;
				AddSegment(p_center, p_radius, p_angle, p_border);
			}
		}

		protected void AddSegment(Vector2 p_center, float p_radius, float p_angle, bool p_border = true)
		{
			float num = Mathf.Cos(p_angle);
			float num2 = Mathf.Sin(p_angle);
			float x = num * p_radius + p_center.x;
			float y = num2 * p_radius + p_center.y;
			AddVertex(new Vector2(x, y));
			if (p_border)
			{
				x = num * (p_radius - _Width) + p_center.x;
				y = num2 * (p_radius - _Width) + p_center.y;
				AddVertex(new Vector2(x, y));
			}
		}

		protected void AddVertex(Vector3 p_position)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.position = p_position;
			simpleVert.uv0 = new Vector2(0.5f + p_position.x / base.rectTransform.rect.width, 0.5f + p_position.y / base.rectTransform.rect.height);
			simpleVert.color = color;
			_Vertexes.Add(simpleVert);
		}

		protected void Update()
		{
			if (Math.Abs(_ChangeAllRadiusAll - _ChangeAllRadius) > 0.01f)
			{
				_ChangeAllRadiusAll = _ChangeAllRadius;
				_RadiusBottomLeft = _ChangeAllRadius;
				_RadiusBottomRight = _ChangeAllRadius;
				_RadiusUpLeft = _ChangeAllRadius;
				_RadiusUpRight = _ChangeAllRadius;
			}
		}
	}
}
