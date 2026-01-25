using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI
{
	public class UIParticleSettings : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter _MeshFilter;

		[SerializeField]
		private ParticleSystem _ParticleSystem;

		[SerializeField]
		[HideInInspector]
		private ParticleShape _Shape;

		[HideInInspector]
		[SerializeField]
		private float _CircleRadius;

		[SerializeField]
		[HideInInspector]
		private float _RectWidth;

		[HideInInspector]
		[SerializeField]
		private float _RectHeight;

		[SerializeField]
		[HideInInspector]
		private bool _FromEdge;

		public ParticleShape Shape
		{
			get
			{
				return _Shape;
			}
			set
			{
				_Shape = value;
			}
		}

		public float CircleRadius
		{
			get
			{
				return _CircleRadius;
			}
			set
			{
				_CircleRadius = value;
			}
		}

		public float RectWidth
		{
			get
			{
				return _RectWidth;
			}
			set
			{
				_RectWidth = value;
			}
		}

		public float RectHeight
		{
			get
			{
				return _RectHeight;
			}
			set
			{
				_RectHeight = value;
			}
		}

		public bool FromEdge
		{
			get
			{
				return _FromEdge;
			}
			set
			{
				_FromEdge = value;
			}
		}

		private void ResetMesh()
		{
			if (_MeshFilter.sharedMesh != null)
			{
				Object.DestroyImmediate(_MeshFilter.sharedMesh, true);
				_MeshFilter.sharedMesh = null;
			}
		}

		private void GenerateMesh()
		{
			Mesh mesh = new Mesh();
			Vector2 zero = Vector2.zero;
			List<Vector3> list = new List<Vector3>();
			list.Add(new Vector3(zero.x - _RectWidth * 0.5f, zero.y - _RectHeight * 0.5f, 0f));
			list.Add(new Vector3(zero.x + _RectWidth * 0.5f, zero.y + _RectHeight * 0.5f, 0f));
			list.Add(new Vector3(zero.x + _RectWidth * 0.5f, zero.y - _RectHeight * 0.5f, 0f));
			list.Add(new Vector3(zero.x - _RectWidth * 0.5f, zero.y + _RectHeight * 0.5f, 0f));
			mesh.vertices = list.ToArray();
			List<int> list2 = new List<int>();
			list2.Add(0);
			list2.Add(1);
			list2.Add(2);
			list2.Add(1);
			list2.Add(0);
			list2.Add(3);
			mesh.triangles = list2.ToArray();
			List<Vector3> list3 = new List<Vector3>();
			list3.Add(new Vector3(-1f, -1f, 0f));
			list3.Add(new Vector3(1f, 1f, 0f));
			list3.Add(new Vector3(1f, -1f, 0f));
			list3.Add(new Vector3(-1f, 1f, 0f));
			mesh.normals = list3.ToArray();
			mesh.RecalculateBounds();
			_MeshFilter.sharedMesh = mesh;
		}

		[ContextMenu("Force Refresh")]
		public void Refresh()
		{
			ResetMesh();
			if (_Shape == ParticleShape.Circle)
			{
				SetupShapeToCircle();
			}
			else if (_Shape == ParticleShape.Rect)
			{
				SetupShapeToRect();
			}
		}

		public void SetupShapeToCircle()
		{
			ParticleSystem.ShapeModule shape = _ParticleSystem.shape;
			shape.radius = _CircleRadius;
			shape.shapeType = ((!_FromEdge) ? ParticleSystemShapeType.Circle : ParticleSystemShapeType.CircleEdge);
		}

		public void SetupShapeToRect()
		{
			GenerateMesh();
			ParticleSystem.ShapeModule shape = _ParticleSystem.shape;
			shape.mesh = _MeshFilter.sharedMesh;
			shape.shapeType = ParticleSystemShapeType.Mesh;
			shape.meshShapeType = ParticleSystemMeshShapeType.Edge;
		}
	}
}
