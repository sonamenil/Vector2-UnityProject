using System.Xml;
using Nekki.Vector.Core.Transformations;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public abstract class Runner : TransformInterface
	{
		protected bool _IsDebug = true;

		protected int _Hash = -1;

		protected string _Name;

		protected TypeRunner _TypeClass;

		protected Vector3f _DefautPosition = new Vector3f(0f, 0f, 0f);

		protected int _ActiveTransformation;

		protected Element _ParentElements;

		public virtual bool IsDebug
		{
			get
			{
				return _IsDebug;
			}
			set
			{
				_IsDebug = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				return !(_UnityObject.GetComponent<Renderer>() == null) && _UnityObject.GetComponent<Renderer>().isVisible;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				return base.IsEnabled;
			}
		}

		public int Hash
		{
			get
			{
				return _Hash;
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
				if (_Name != null)
				{
					_Hash = _Name.GetHashCode();
				}
			}
		}

		public TypeRunner TypeClass
		{
			get
			{
				return _TypeClass;
			}
		}

		public virtual Element ParentElements
		{
			get
			{
				return _ParentElements;
			}
		}

		public Runner(float p_x, float p_y, Element p_parentElements)
		{
			_DefautPosition.X = p_x;
			_DefautPosition.Y = p_y;
			_ParentElements = p_parentElements;
		}

		public virtual void Generate()
		{
			if (!(_UnityObject != null))
			{
				GenerateObject();
			}
		}

		public override void SetEnabled(bool p_enabled, bool restore = false, bool fromHierarchy = false)
		{
			base.SetEnabled(p_enabled, restore, fromHierarchy);
			if (_UnityObject != null)
			{
				_UnityObject.SetActive(_IsEnabled);
			}
		}

		protected virtual void GenerateObject()
		{
			_UnityObject = new GameObject
			{
				name = GetType().Name + ":"
			};
			_CachedTransform = _UnityObject.transform;
			_CachedTransform.localPosition = new Vector3(_DefautPosition.X, _DefautPosition.Y);
			_CachedTransform.SetParent(_ParentElements.Parent.UnityObject.transform, false);
			if (!string.IsNullOrEmpty(_Name))
			{
				GameObject unityObject = _UnityObject;
				unityObject.name = unityObject.name + " " + _Name;
			}
		}

		public virtual void InitRunner()
		{
			if (!_IsEnabled)
			{
				SetEnabled(false);
			}
		}

		public void Shift(Vector3f Point)
		{
			Vector3 localPosition = _CachedTransform.localPosition;
			localPosition.x += Point.X;
			localPosition.y += Point.Y;
			_CachedTransform.localPosition = localPosition;
		}

		public override void SetPosition(float x, float y)
		{
			Vector3 localPosition = _CachedTransform.localPosition;
			localPosition.x = x;
			localPosition.y = y;
			_CachedTransform.localPosition = localPosition;
		}

		public virtual void Move(Vector3f Point)
		{
			Shift(Point);
		}

		public virtual void UpdatePosition(Vector3f Point)
		{
			Move(Point);
		}

		public virtual void UpdatePosition()
		{
		}

		public abstract bool Render();

		public void ParseProperty(XmlNode p_node)
		{
			ParseTransformation(p_node);
			ParseEnable(p_node);
		}

		private void ParseEnable(XmlNode p_node)
		{
			if (p_node["Properties"] != null && p_node["Properties"]["Static"] != null && p_node["Properties"]["Static"]["Enable"] != null)
			{
				SetEnabled(XmlUtils.ParseBool(p_node["Properties"]["Static"]["Enable"].Attributes["Value"], true));
			}
		}

		public override void TransformationStart()
		{
			_ActiveTransformation++;
		}

		public override void TransformationEnd()
		{
			_ActiveTransformation--;
		}

		public override void SetDeltaMove()
		{
			if (_IsDeltaMoveChange)
			{
				UpdatePosition(_DeltaMove);
				ResetDeltaMove();
			}
		}

		public override void TransformResetTween()
		{
		}

		public override void TransformColor(Color p_delta)
		{
		}

		public override void TransformColorEnd(Color p_color)
		{
		}

		public override void TransformRotateX(float p_angle)
		{
		}

		public override void TransformRotateY(float p_angle)
		{
		}

		public override void TransformRotateZ(float p_angle)
		{
		}

		public override void TransformResize(float p_w, float p_h)
		{
		}

		public virtual void End()
		{
		}

		public virtual void DestroyUnityObjects()
		{
			if (_UnityObject != null)
			{
				Object.DestroyObject(_UnityObject);
				_UnityObject = null;
			}
		}

		public override void TransformExecute()
		{
		}

		public override void TransformLayer(string p_layer)
		{
		}
	}
}
