using System.Collections.Generic;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Result;
using Nekki.Vector.Core.Runners;
using UnityEngine;

namespace Nekki.Vector.Core.Models
{
	public class Model
	{
		public static float BoundingBoxSize = 300f;

		public static string ModelLayer;

		private string _Name;

		protected ModelObject _ModelObject;

		protected ControllerPhysics _ControllerPhysics;

		protected ControllerCollision _ControllerCollision;

		protected ControllerStrike _ControllerStrike;

		protected double _DeltaBox = BoundingBoxSize;

		protected ModelType _Type;

		protected Vector3 _DeathPosition = default(Vector3);

		public GameObject Layer
		{
			get
			{
				return ModelObject.Layer;
			}
			set
			{
				ModelObject.Layer = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _ModelObject.IsEnabled;
			}
			set
			{
				_ModelObject.IsEnabled = value;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
				_ModelObject.Name = _Name;
			}
		}

		public ModelObject ModelObject
		{
			get
			{
				return _ModelObject;
			}
		}

		public ControllerPhysics ControllerPhysics
		{
			get
			{
				return _ControllerPhysics;
			}
		}

		public ControllerCollision ControllerCollision
		{
			get
			{
				return _ControllerCollision;
			}
		}

		public ControllerStrike ControllerStrike
		{
			get
			{
				return _ControllerStrike;
			}
		}

		public double DeltaBox
		{
			get
			{
				return _DeltaBox;
			}
		}

		public ModelType Type
		{
			get
			{
				return _Type;
			}
			set
			{
				_Type = value;
			}
		}

		public Color Color
		{
			get
			{
				return _ModelObject.Color;
			}
			set
			{
				_ModelObject.Color = value;
			}
		}

		public virtual Rectangle Rectangle
		{
			get
			{
				return (_ModelObject != null) ? _ModelObject.Rectangle : new Rectangle(0f, 0f, 0f, 0f);
			}
		}

		public bool IsPhysics
		{
			get
			{
				return _ControllerPhysics != null && _ControllerPhysics.IsPhysics;
			}
		}

		public Vector3 DeathPosition
		{
			get
			{
				return _DeathPosition;
			}
		}

		public Model(List<string> p_skins, ModelType p_type)
		{
			_Type = p_type;
			_ModelObject = new ModelObject(p_skins)
			{
				Parent = this,
				IsAuxiliary = false
			};
		}

		public virtual void Init()
		{
			_ControllerPhysics = new ControllerPhysics(_ModelObject);
			_ControllerCollision = new ControllerCollision(this);
			_ControllerStrike = new ControllerStrike(this);
			StopPhysics();
		}

		public virtual void OnDeath()
		{
			_DeathPosition = Position("NPivot", true);
			_ControllerPhysics.Start();
		}

		public virtual void StartPhysics()
		{
			_ControllerPhysics.Start();
		}

		public void StopPhysics()
		{
			_ControllerPhysics.Stop();
		}

		public virtual void Strike(ModelLine p_edge, Vector3f p_point, Vector3f p_impulse)
		{
			if (_ControllerStrike != null)
			{
				_ControllerStrike.Striking(p_edge, p_point, p_impulse);
			}
		}

		public void ResetImpuls(float p_index)
		{
			foreach (ModelNode node in _ModelObject.Nodes)
			{
				node.End.X += (node.Start.X - node.End.X) * p_index;
				node.End.Y += (node.Start.Y - node.End.Y) * p_index;
				node.End.Z += (node.Start.Z - node.End.Z) * p_index;
			}
		}

		public void Stricke(float p_impuls, float p_R, Vector3 p_center)
		{
			float num = p_impuls;
			foreach (ModelNode node in _ModelObject.Nodes)
			{
				if (node.IsFixed)
				{
					break;
				}
				num = p_impuls;
				Vector3 a = new Vector3(node.Start.X, node.Start.Y, 0f) - p_center;
				num *= 1f - Vector3.Magnitude(a) / p_R;
				a.Normalize();
				num *= 1f / node.Weight;
				a.x *= num * -1f;
				a.y *= num * -1f;
				float p_x = node.End.X + a.x;
				float p_y = node.End.Y + a.y;
				node.End.Set(p_x, p_y, node.End.Z);
			}
		}

		public virtual void OnCollisionPlatform(Nekki.Vector.Core.Result.Collision p_collision)
		{
		}

		public virtual void OnCollisionModel(Nekki.Vector.Core.Result.Collision p_collision)
		{
		}

		public virtual void Render(List<QuadRunner> p_platforms = null)
		{
		}

		public void Reset()
		{
			if (_ModelObject != null)
			{
				_ModelObject.Reset();
			}
		}

		public void Position(Vector3f p_vector, string p_name = "NPivot")
		{
			if (_ModelObject != null)
			{
				_ModelObject.Position(p_vector, p_name);
			}
		}

		public Vector3f Position(string p_name = "NPivot", bool p_isCurrent = true)
		{
			return (_ModelObject != null) ? _ModelObject.Position(p_name, p_isCurrent) : null;
		}

		public ModelNode Node(string p_name = "NPivot")
		{
			return (_ModelObject == null) ? null : _ModelObject.GetNode(p_name);
		}

		public ModelNode NodeToLow(string p_name)
		{
			return (_ModelObject == null) ? null : _ModelObject.GetNodeToLow(p_name);
		}

		public virtual void End()
		{
		}
	}
}
