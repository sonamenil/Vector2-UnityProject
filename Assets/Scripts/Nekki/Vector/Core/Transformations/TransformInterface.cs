using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Transformations
{
	public abstract class TransformInterface
	{
		protected Transform _CachedTransform;

		protected GameObject _UnityObject;

		protected GameObject _SupportUnityObject;

		protected bool _SavedEnableState = true;

		protected bool _IsEnabled = true;

		protected bool _IsDeltaMoveChange;

		protected Vector3f _DeltaMove = new Vector3f(0f, 0f, 0f);

		protected List<Transformation> _TransformationData;

		public GameObject UnityObject
		{
			get
			{
				if (_SupportUnityObject != null)
				{
					return _SupportUnityObject;
				}
				return _UnityObject;
			}
		}

		public Vector3 Position
		{
			get
			{
				return _CachedTransform.position;
			}
		}

		public virtual Vector3 LocalPosition
		{
			get
			{
				return _CachedTransform.localPosition;
			}
		}

		public virtual bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
		}

		public List<Transformation> TransformationData
		{
			get
			{
				return _TransformationData;
			}
		}

		public void TransformMove(Vector3f p_delta)
		{
			_DeltaMove.Add(p_delta.X, p_delta.Y, 0f);
			_IsDeltaMoveChange = true;
		}

		protected void ResetDeltaMove()
		{
			_DeltaMove.Reset();
			_IsDeltaMoveChange = false;
		}

		public void TransformMove(float p_xDelta, float p_yDelta)
		{
			_DeltaMove.Add(p_xDelta, p_yDelta, 0f);
			_IsDeltaMoveChange = true;
		}

		public abstract void SetPosition(float x, float y);

		public abstract void SetDeltaMove();

		public abstract void TransformResetTween();

		public abstract void TransformColor(Color p_delta);

		public abstract void TransformColorEnd(Color p_color);

		public abstract void TransformResize(float p_w, float p_h);

		public abstract void TransformLayer(string p_layer);

		public abstract void TransformationStart();

		public abstract void TransformationEnd();

		public abstract void TransformRotateX(float p_angle);

		public abstract void TransformRotateY(float p_angle);

		public abstract void TransformRotateZ(float p_angle);

		public abstract void TransformExecute();

		public virtual void SetEnabled(bool p_enabled, bool p_restore = false, bool fromHierarchy = false)
		{
			if (p_enabled)
			{
				if (fromHierarchy && p_restore)
				{
					_IsEnabled = _SavedEnableState;
				}
				else
				{
					_SavedEnableState = (_IsEnabled = p_enabled);
				}
			}
			else if (fromHierarchy)
			{
				_SavedEnableState = _IsEnabled;
				_IsEnabled = p_enabled;
			}
			else
			{
				_SavedEnableState = (_IsEnabled = p_enabled);
			}
			if (!_IsEnabled)
			{
				TransformationManager.Current.RemoveTransformationByParent(this);
				OnDisabled();
			}
			else
			{
				OnEnabled();
			}
		}

		protected virtual void OnEnabled()
		{
		}

		protected virtual void OnDisabled()
		{
		}

		public void TransformationOn(bool p_restore)
		{
			SetEnabled(true, p_restore);
		}

		public void TransformationOff()
		{
			SetEnabled(false);
		}

		public void TransformationSwitch(bool p_restore)
		{
			SetEnabled(!_IsEnabled, p_restore);
		}

		private void AddTransformationData(Transformation p_data)
		{
			if (_TransformationData == null)
			{
				_TransformationData = new List<Transformation>();
			}
			_TransformationData.Add(p_data);
		}

		protected void ParseTransformation(XmlNode p_node)
		{
			if (p_node["Properties"] == null || p_node["Properties"]["Dynamic"] == null)
			{
				return;
			}
			p_node = p_node["Properties"]["Dynamic"];
			Transformation transformation = null;
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				transformation = Transformation.Create(childNode);
				if (transformation != null)
				{
					transformation.Parent = this;
					AddTransformationData(transformation);
				}
			}
		}
	}
}
