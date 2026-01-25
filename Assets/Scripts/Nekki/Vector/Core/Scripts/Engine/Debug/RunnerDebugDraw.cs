using Nekki.Vector.Core.Runners;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts.Engine.Debug
{
	public class RunnerDebugDraw : MonoBehaviour
	{
		private Color _Color = new Color(0f, 0f, 0f, 1f);

		private Color _BackgoundColor = new Color(0f, 0f, 0f, 1f);

		private float _Border;

		private float _Radius = 1f;

		protected Runner _Base;

		private string _SortingLayerName = "Debug";

		private int _SortingOrder;

		private static Shader _Shader;

		private Material _Material;

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
				if (_Material != null)
				{
					_Material.SetVector("_Color", _Color);
				}
			}
		}

		public Color BackgoundColor
		{
			get
			{
				return _BackgoundColor;
			}
			set
			{
				_BackgoundColor = value;
				if (_Material != null)
				{
					_Material.SetVector("_BackgoundColor", _BackgoundColor);
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
				if (_Material != null)
				{
					_Material.SetFloat("_Border", _Border);
				}
			}
		}

		public float Radius
		{
			get
			{
				return _Radius;
			}
			set
			{
				_Radius = value;
				if (_Material != null)
				{
					_Material.SetFloat("_Radius", _Radius);
				}
			}
		}

		public Runner Base
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
				_Shader = Shader.Find("Sprites/Circle-Colored");
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
			_Material.SetFloat("_Border", _Border);
			_Material.SetFloat("_Radius", _Radius);
			_Material.SetVector("_Color", _Color);
			_Material.SetVector("_BackgoundColor", _BackgoundColor);
			Update();
		}

		private void Update()
		{
			if (_Base != null)
			{
				base.transform.localPosition = _Base.Position;
				base.transform.localScale = new Vector3(_Radius, _Radius, _Radius);
			}
		}
	}
}
