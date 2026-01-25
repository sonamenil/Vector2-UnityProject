using Nekki.Vector.Core.Node;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nekki.Vector.Core.Scripts.Geometry
{
	public class Capsule : MonoBehaviour
	{
		private float _Stroke = 1f;

		protected ModelLine _Base;

		private string _SortingLayerName = string.Empty;

		private int _SortingOrder;

		private static Material _SharedQuadMaterial;

		private static Material _SharedCircleMaterial;

		private Transform _MiddleRect;

		private Transform _BeginCircle;

		private Transform _EndCircle;

		public float Stroke
		{
			get
			{
				return _Stroke;
			}
			set
			{
				_Stroke = value;
			}
		}

		public ModelLine Base
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

		private static Material SharedQuadMaterial
		{
			get
			{
				if (_SharedQuadMaterial == null)
				{
					_SharedQuadMaterial = new Material(Shader.Find("Sprites/Colored"));
				}
				return _SharedQuadMaterial;
			}
		}

		private static Material SharedCircleMaterial
		{
			get
			{
				if (_SharedCircleMaterial == null)
				{
					_SharedCircleMaterial = new Material(Shader.Find("Capsule/Circle"));
				}
				return _SharedCircleMaterial;
			}
		}

		private void Start()
		{
			_Stroke = _Base.Stroke;
			GameObject gameObject = new GameObject("CircleBegin");
			InitGameObject(gameObject, SharedCircleMaterial);
			_BeginCircle = gameObject.transform;
			_BeginCircle.SetParent(base.transform, false);
			gameObject = new GameObject("CircleEnd");
			InitGameObject(gameObject, SharedCircleMaterial);
			_EndCircle = gameObject.transform;
			_EndCircle.SetParent(base.transform, false);
			gameObject = new GameObject("Rect");
			InitGameObject(gameObject, SharedQuadMaterial);
			_MiddleRect = gameObject.transform;
			_MiddleRect.SetParent(base.transform, false);
			float num = _Stroke * 2f;
			_BeginCircle.localScale = new Vector3(num, num, 1f);
			_EndCircle.localScale = new Vector3(num, num, 1f);
			Render();
		}

		private void InitGameObject(GameObject p_object, Material p_material)
		{
			MeshFilter meshFilter = p_object.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = p_object.AddComponent<MeshRenderer>();
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
			mesh.RecalculateBounds();
			meshFilter.mesh = mesh;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			meshRenderer.sharedMaterial = p_material;
			meshRenderer.sortingLayerName = _SortingLayerName;
			meshRenderer.sortingOrder = _SortingOrder;
		}

		public void Render()
		{
			if (_Base != null && _Base.Start != null && _Base.Start.End != null && _Base.End != null && _Base.End.End != null && !(_BeginCircle == null) && !(_EndCircle == null) && !(_MiddleRect == null))
			{
				Vector3f start = _Base.Start.Start;
				Vector3f start2 = _Base.End.Start;
				float num = start.X - start2.X;
				float num2 = start.Y - start2.Y;
				float num3 = start.X - num * _Base.Margin1;
				float num4 = start.Y - num2 * _Base.Margin1;
				float num5 = start2.X + num * _Base.Margin2;
				float num6 = start2.Y + num2 * _Base.Margin2;
				_BeginCircle.localPosition = new Vector2(num3, num4);
				_EndCircle.localPosition = new Vector2(num5, num6);
				num = num3 - num5;
				num2 = num4 - num6;
				float y = Mathf.Sqrt(num * num + num2 * num2);
				_MiddleRect.transform.up = new Vector3(num, num2, 0f);
				num = (num3 + num5) / 2f;
				num2 = (num4 + num6) / 2f;
				_MiddleRect.localPosition = new Vector3(num, num2);
				if (_Stroke != _Base.Stroke)
				{
					_Stroke = _Base.Stroke;
					float num7 = _Stroke * 2f;
					_BeginCircle.localScale = new Vector3(num7, num7, 1f);
					_EndCircle.localScale = new Vector3(num7, num7, 1f);
				}
				_MiddleRect.localScale = new Vector3(_Stroke * 2f, y, 1f);
			}
		}
	}
}
