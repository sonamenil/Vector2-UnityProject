using System.Collections.Generic;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Run;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts.Engine.Debug
{
	public class Quad : MonoBehaviour
	{
		protected Color _Color = new Color(0f, 0f, 0f, 1f);

		protected Color _BackgoundColor = new Color(0f, 0f, 0f, 1f);

		private float _Border = 1f;

		private float _HalfBorder = 1f;

		protected QuadRunner _Base;

		protected string _SortingLayerName = "Debug";

		protected int _SortingOrder;

		private static Shader _SpriteShader;

		private static Shader _MeshShader;

		protected List<GameObject> _Lines = new List<GameObject>();

		protected GameObject _MeshObject;

		protected Mesh _Mesh;

		protected Rectangle _OldRect = new Rectangle();

		private BoxCollider2D _Collider;

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
					if (line != null)
					{
						line.GetComponent<Renderer>().material.SetVector("_Color", _Color);
					}
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
				if (_MeshObject != null)
				{
					_MeshObject.GetComponent<Renderer>().material.SetVector("_Color", _BackgoundColor);
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
				_HalfBorder = _Border / 2f;
			}
		}

		public virtual QuadRunner Base
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

		protected virtual void CreateMesh()
		{
			_MeshObject = new GameObject("Mesh");
			_MeshObject.transform.SetParent(base.transform, false);
			_Mesh = new Mesh();
			_MeshObject.AddComponent<MeshFilter>().mesh = _Mesh;
			MeshRenderer meshRenderer = _MeshObject.AddComponent<MeshRenderer>();
			meshRenderer.sortingLayerName = _SortingLayerName;
			meshRenderer.sortingOrder = _SortingOrder;
			Material material = meshRenderer.material;
			material.shader = _MeshShader;
			material.SetVector("_Color", _BackgoundColor);
		}

		protected GameObject CreateLine(float p_width)
		{
			GameObject gameObject = new GameObject("Line " + _Lines.Count);
			gameObject.transform.SetParent(base.transform, false);
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[4]
			{
				new Vector3(0f - p_width, 0f - p_width, 0f),
				new Vector3(p_width, 0f - p_width, 0f),
				new Vector3(0f - p_width, p_width, 0f),
				new Vector3(p_width, p_width, 0f)
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
			material.shader = _SpriteShader;
			material.SetVector("_Color", _Color);
			return gameObject;
		}

		protected virtual void CreateLines()
		{
			_Lines.Add(CreateLine(0.5f));
			_Lines.Add(CreateLine(0.5f));
			_Lines.Add(CreateLine(0.5f));
			_Lines.Add(CreateLine(0.5f));
		}

		private void Start()
		{
			if (_SpriteShader == null || _MeshShader == null)
			{
				_SpriteShader = Shader.Find("Sprites/Colored");
				_MeshShader = Shader.Find("Sprites/Colored");
			}
			CreateLines();
			CreateMesh();
			_Collider = base.gameObject.AddComponent<BoxCollider2D>();
			_Collider.transform.parent = base.gameObject.transform;
        }

		protected void UpdateLine(GameObject p_line, Vector3 p_start, Vector3 p_end)
		{
			Vector3 up = p_start - p_end;
			float magnitude = up.magnitude;
			p_line.transform.up = up;
			p_line.transform.localScale = new Vector3(4, magnitude + 4, 1f);
			p_line.transform.localPosition = (p_start + p_end) / 2f;
		}

		protected virtual void UpdateMesh()
		{
			Vector3 vector = new Vector3(_Base.Rectangle.MinX, _Base.Rectangle.MinY);
			Vector3 vector2 = (Vector3)_Base.Point1 - vector;
			Vector3 vector3 = (Vector3)_Base.Point2 - vector;
			Vector3 vector4 = (Vector3)_Base.Point3 - vector;
			Vector3 vector5 = (Vector3)_Base.Point4 - vector;
			vector2.x += _HalfBorder;
			vector2.y += _HalfBorder;
			vector3.x -= _HalfBorder;
			vector3.y += _HalfBorder;
			vector4.x -= _HalfBorder;
			vector4.y -= _HalfBorder;
			vector5.x += _HalfBorder;
			vector5.y -= _HalfBorder;
			_Mesh.vertices = new Vector3[4] { vector2, vector3, vector4, vector5 };
			SetTriangleToMesh();
			_Mesh.RecalculateBounds();
			_MeshObject.transform.localPosition = new Vector3(0f, 0f, 0f); ;
			_MeshObject.transform.localScale = new Vector3(1f, 1f, 1f);
		}

		protected virtual void SetTriangleToMesh()
		{
			_Mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		}

		public void Update()
		{
            //_Base.UpdatePosition();
            if (_Base != null && !(_OldRect == _Base.Rectangle))
			{
				_OldRect.Set(_Base.Rectangle);
				UpdateMesh();
				UpdateRect();
				if (_Lines.Count != 0)
				{
					UpdateLines();
				}


				
			}
		}

		private void UpdateRect()
		{
			if (_Collider != null && _Base != null)
			{
				Vector3f vector3f = new Vector3f(_Base.Point1);
				_Collider.offset = new Vector3(_Base.Rectangle.Size.Width / 2f, _Base.Rectangle.Size.Height / 2f);
				_Collider.size = new Vector3(_Base.Rectangle.Size.Width, _Base.Rectangle.Size.Height);
			}
		}

		protected virtual void UpdateLines()
		{
			UpdateLine(_Lines[0], new Vector3(_OldRect.Size.Width, 0), new Vector3(0, 0));
			UpdateLine(_Lines[1], new Vector3(_OldRect.Size.Width, _OldRect.Size.Height), new Vector3(0, _OldRect.Size.Height));
            UpdateLine(_Lines[2], new Vector3(_OldRect.Size.Width, 0), new Vector3(_OldRect.Size.Width, _OldRect.Size.Height));
            UpdateLine(_Lines[3], new Vector3(0, 0), new Vector3(0, _OldRect.Size.Height));
        }

        public void OnMouseDown()
        {
            DebugDataView.SetText(_Base.ToString());
        }
    }
}
