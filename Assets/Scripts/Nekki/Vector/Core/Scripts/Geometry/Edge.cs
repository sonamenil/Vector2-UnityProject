using UnityEngine;

namespace Nekki.Vector.Core.Scripts.Geometry
{
	public class Edge : MonoBehaviour
	{
		protected Vector3fLine _Base;

		private string _SortingLayerName = string.Empty;

		private int _SortingOrder;

		private static Shader _Shader;

		private Material _Material;

		public Vector3fLine Base
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

		private void Start()
		{
			if (_Shader == null)
			{
				_Shader = Shader.Find("Sprites/Colored");
			}
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
			base.gameObject.AddComponent<MeshFilter>().mesh = mesh2;
			MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sortingLayerName = _SortingLayerName;
			meshRenderer.sortingOrder = _SortingOrder;
			_Material = meshRenderer.material;
			_Material.shader = _Shader;
			Update();
		}

		private void Update()
		{
			if (_Base != null && _Base.Start != null && _Base.End != null)
			{
				Vector3 vector = _Base.Start;
				Vector3 vector2 = _Base.End;
				vector.z = 0f;
				vector2.z = 0f;
				Vector3 up = vector - vector2;
				float magnitude = up.magnitude;
				base.transform.up = up;
				base.transform.localScale = new Vector3(_Base.Stroke, magnitude, 1f);
				Vector3 localPosition = (vector + vector2) / 2f;
				base.transform.localPosition = localPosition;
				_Material.SetVector("_Color", _Base.Color);
			}
		}
	}
}
