using Nekki.Vector.Core.Node;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nekki.Vector.Core.Scripts.Projection
{
	public class Mesh : MonoBehaviour
	{
		private Color _Color = new Color(0f, 0f, 0f, 1f);

		protected MeshNode _MeshNode;

		private string _SortingLayerName = string.Empty;

		private int _SortingOrder;

		private UnityEngine.Mesh _Mesh;

		private static Shader _Shader;

		private static Material _SharedMaterial;

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
				if (_SharedMaterial != null)
				{
					_SharedMaterial.SetVector("_Color", _Color);
				}
			}
		}

		public MeshNode Base
		{
			set
			{
				_MeshNode = value;
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
				_Shader = Shader.Find("Mesh/Colored");
				_SharedMaterial = new Material(_Shader);
			}
			_Mesh = new UnityEngine.Mesh();
			base.gameObject.AddComponent<MeshFilter>().mesh = _Mesh;
			MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = _SharedMaterial;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			meshRenderer.sortingLayerName = _SortingLayerName;
			meshRenderer.sortingOrder = _SortingOrder;
			_SharedMaterial.SetVector("_Color", _Color);
			Init();
		}

		private void Init()
		{
			_Mesh.vertices = _MeshNode.Vertices;
			_Mesh.triangles = _MeshNode.Triangles;
		}

		private void Update()
		{
			if (_MeshNode != null)
			{
				_MeshNode.Render();
				_Mesh.vertices = _MeshNode.Vertices;
				_Mesh.RecalculateBounds();
			}
		}
	}
}
