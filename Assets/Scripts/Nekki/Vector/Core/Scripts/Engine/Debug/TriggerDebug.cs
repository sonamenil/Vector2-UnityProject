using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Game;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts.Engine.Debug
{
	public class TriggerDebug : Quad
	{
		private Material _Material;

		private bool _IsElips = true;

		private bool _IsLine;

		protected TriggerRunner _Trigger;

		public override QuadRunner Base
		{
			get
			{
				return base.Base;
			}
			set
			{
				base.Base = value;
				_Trigger = value as TriggerRunner;
				_IsElips = _Trigger.IsElipsType || _Trigger.IsCircleType;
				_IsLine = _Trigger.IsDiagonal;
				_Color = Settings.Visual.Trigger.Border;
                _BackgoundColor = Settings.Visual.Trigger.Background;
            }
		}

		protected override void CreateMesh()
		{
			if (!_IsLine)
			{
				base.CreateMesh();
				if (_Base != null && _IsElips)
				{
					SetElipsShader();
				}
			}
		}

		protected override void CreateLines()
		{
			if (_IsLine)
			{
				_Lines.Add(CreateLine(0.5f));
			}
			else if (!_IsElips)
			{
				base.CreateLines();
			}
		}

		private void SetElipsShader()
		{
			_Mesh = new Mesh
			{
				vertices = new Vector3[4]
				{
					new Vector3(-0.5f, -0.5f, 0f),
					new Vector3(0.5f, -0.5f, 0f),
					new Vector3(-0.5f, 0.5f, 0f),
					new Vector3(0.5f, 0.5f, 0f)
				},
				uv = new Vector2[4]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f)
				},
				triangles = new int[6] { 0, 1, 2, 1, 3, 2 }
			};
            _Mesh.RecalculateBounds();
			_MeshObject.GetComponent<MeshFilter>().mesh = _Mesh;
			MeshRenderer meshRenderer = _MeshObject.GetComponent<MeshRenderer>() ?? _MeshObject.AddComponent<MeshRenderer>();
			meshRenderer.sortingLayerName = _SortingLayerName;
			meshRenderer.sortingOrder = _SortingOrder;
			_Material = meshRenderer.material;
			_Material.shader = Shader.Find("Sprites/Circle-Colored");
			_Material.SetFloat("_Border", 250f / Mathf.Max(_Trigger.Rectangle.Size.Height, _Trigger.Rectangle.Size.Width));
			_Material.SetFloat("_Radius", 100f);
			_Material.SetVector("_Color", _Color);
			_Material.SetVector("_BackgoundColor", _BackgoundColor);
		}

		protected override void UpdateMesh()
		{
			if (!_IsLine)
			{
				if (_IsElips)
				{
					_MeshObject.transform.localPosition = new Vector3(_Base.Rectangle.Size.Width / 2, _Base.Rectangle.Size.Height / 2);
					_MeshObject.transform.localScale = new Vector3(_Base.Rectangle.Size.Width, _Base.Rectangle.Size.Height, 1f);
				}
				else
				{
					base.UpdateMesh();
				}
			}
		}

		protected override void UpdateLines()
		{
			if (_IsLine)
			{
				if (_Base != null && _Trigger.IsUpDiagonal)
				{
					UpdateLine(_Lines[0], new Vector3(_OldRect.MinX, _OldRect.MaxY), new Vector3(_OldRect.MaxX, _OldRect.MinY));
				}
				if (_Base != null && _Trigger.IsDownDiagonal)
				{
					UpdateLine(_Lines[0], new Vector3(_OldRect.MinX, _OldRect.MinY), new Vector3(_OldRect.MaxX, _OldRect.MaxY));
				}
			}
			else
			{
				base.UpdateLines();
			}
		}

		protected override void SetTriangleToMesh()
		{
			base.SetTriangleToMesh();
			if (_Base != null && _Trigger.IsUpDiagonal)
			{
				_Mesh.triangles = new int[3] { 1, 3, 2 };
			}
			if (_Base != null && _Trigger.IsDownDiagonal)
			{
				_Mesh.triangles = new int[3] { 0, 1, 2 };
			}
		}
	}
}
