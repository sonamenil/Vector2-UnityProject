using System.Collections.Generic;
using Nekki.Vector.Core.Models;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts
{
	public class BoundingBox : MonoBehaviour
	{
		private Color _Color = new Color(0f, 0f, 0f, 1f);

		private float _Border = 1f;

		protected Model _Base;

		private string _SortingLayerName = string.Empty;

		private int _SortingOrder;

		private static Shader _Shader;

		private List<GameObject> _Lines = new List<GameObject>();

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
				foreach (GameObject line in _Lines)
				{
					line.GetComponent<Renderer>().material.SetVector("_Color", _Color);
				}
			}
		}

		public float Border
		{
			get
			{
				return _Border;
			}
			set
			{
				_Border = value;
			}
		}

		public Model Base
		{
			get
			{
				return _Base;
			}
			set
			{
				_Base = value;
			}
		}

		public string SortingLayerName
		{
			get
			{
				return _SortingLayerName;
			}
			set
			{
				_SortingLayerName = value;
			}
		}

		public int SortingOrder
		{
			get
			{
				return _SortingOrder;
			}
			set
			{
				_SortingOrder = value;
			}
		}

		private GameObject CreateLine()
		{
			GameObject gameObject = new GameObject("Line " + _Lines.Count);
			gameObject.transform.parent = base.transform;
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[4]
			{
				new Vector3(-0.5f, -0.5f, 0f),
				new Vector3(0.5f, -0.5f, 0f),
				new Vector3(-0.5f, 0.5f, 0f),
				new Vector3(0.5f, 0.5f, 0f)
			};
			mesh.uv = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};
			mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };
			Mesh mesh2 = mesh;
			mesh2.RecalculateBounds();
			gameObject.AddComponent<MeshFilter>().mesh = mesh2;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sortingLayerName = _SortingLayerName;
			meshRenderer.sortingOrder = _SortingOrder;
			Material material = meshRenderer.material;
			material.shader = _Shader;
			material.SetVector("_Color", _Color);
			return gameObject;
		}

		private void Start()
		{
			if (_Shader == null)
			{
				_Shader = Shader.Find("Sprites/Colored");
			}
			_Lines.Add(CreateLine());
			_Lines.Add(CreateLine());
			_Lines.Add(CreateLine());
			_Lines.Add(CreateLine());
			Update();
		}

		public void UpdateLine(GameObject p_line, Vector3 p_start, Vector3 p_end)
		{
			Vector3 up = p_start - p_end;
			float magnitude = up.magnitude;
			p_line.transform.up = up;
			p_line.transform.localScale = new Vector3(_Border, magnitude, 1f);
			p_line.transform.localPosition = (p_start + p_end) / 2f;
		}

		public void Update()
		{
			if (_Base != null)
			{
				Rectangle rectangle = _Base.Rectangle;
				if (!(rectangle == null))
				{
					UpdateLine(_Lines[0], new Vector3(rectangle.MinX, rectangle.MinY), new Vector3(rectangle.MaxX, rectangle.MinY));
					UpdateLine(_Lines[1], new Vector3(rectangle.MinX, rectangle.MinY), new Vector3(rectangle.MinX, rectangle.MaxY));
					UpdateLine(_Lines[2], new Vector3(rectangle.MinX, rectangle.MaxY), new Vector3(rectangle.MaxX, rectangle.MaxY));
					UpdateLine(_Lines[3], new Vector3(rectangle.MaxX, rectangle.MaxY), new Vector3(rectangle.MaxX, rectangle.MinY));
				}
			}
		}
	}
}
