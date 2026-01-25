using System.Collections.Generic;
using UIFigures;
using UIFigures.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFrame : UIFigure
{
	[SerializeField]
	private float _Width = 10f;

	[SerializeField]
	private Vector2 _LeftTop = default(Vector2);

	[SerializeField]
	private Vector2 _RightTop = default(Vector2);

	[SerializeField]
	private Vector2 _LeftBottom = default(Vector2);

	[SerializeField]
	private Vector2 _RightBottom = default(Vector2);

	private List<Vector2> _Points = new List<Vector2>();

	protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
	{
		base.OnPopulateMesh(p_vertexHelper);
		Vector2 vector = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
		vector.x -= _Width;
		vector.y -= _Width;
		_Points.Clear();
		_Points.Add(new Vector2(0f, vector.y));
		_Points.Add(new Vector2(vector.x - _RightTop.x, vector.y));
		_Points.Add(new Vector2(vector.x, vector.y - _RightTop.y));
		_Points.Add(new Vector2(vector.x, 0f - vector.y + _RightBottom.y));
		_Points.Add(new Vector2(vector.x - _RightBottom.x, 0f - vector.y));
		_Points.Add(new Vector2(0f - vector.x + _LeftBottom.x, 0f - vector.y));
		_Points.Add(new Vector2(0f - vector.x, 0f - vector.y + _LeftBottom.y));
		_Points.Add(new Vector2(0f - vector.x, vector.y - _LeftTop.y));
		_Points.Add(new Vector2(0f - vector.x + _LeftTop.x, vector.y));
		_Points.Add(new Vector2(0f, vector.y));
		DrawFunctions.DrawLine(p_vertexHelper, _Points, _Width, color);
	}
}
