using System.Collections.Generic;
using UIFigures;
using UIFigures.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIPoligon : UIFigure
{
	[SerializeField]
	private List<Vector2> _Points = new List<Vector2>();

	[SerializeField]
	private bool _UseTriangleFan = true;

	public List<Vector2> Points
	{
		get
		{
			return _Points;
		}
	}

	protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
	{
		base.OnPopulateMesh(p_vertexHelper);
		List<UIVertex> list = new List<UIVertex>(_Points.Count);
		for (int i = 0; i < _Points.Count; i++)
		{
			list.Add(AddVertex(_Points[i]));
		}
		p_vertexHelper.Clear();
		p_vertexHelper.AddUIVertexStream(list, (!_UseTriangleFan) ? FigureTopology.TringleStripIndices(list.Count - 2) : FigureTopology.TringleFanIndices(list.Count - 2));
	}

	private UIVertex AddVertex(Vector3 p_position)
	{
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.position = new Vector2(p_position.x, p_position.y);
		simpleVert.uv0 = new Vector2(0f, 0f);
		simpleVert.color = color;
		return simpleVert;
	}
}
