using Nekki.Vector.Core.Camera;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts
{
	public class Parallax : MonoBehaviour
	{
		protected Transform _ParentTransform;

		protected Transform _CachedTransform;

		[SerializeField]
		protected float _FactorX;

		[SerializeField]
		protected float _FactorY;

		protected Vector3 _StartPosition;

		protected Vector3 _LastCameraPosition;

		protected Vector3 _LastCameraOffset;

		protected Vector3 _LastParentOffset;

		public void SetFactor(float factorX, float factorY)
		{
			_FactorX = factorX;
			_FactorY = factorY;
		}

		public void SetParent(GameObject obj)
		{
			_ParentTransform = obj.transform;
		}

		protected virtual void Start()
		{
			_CachedTransform = base.transform;
			_StartPosition = _CachedTransform.position;
			if (Nekki.Vector.Core.Camera.Camera.Current != null)
			{
				Nekki.Vector.Core.Camera.Camera.Current.OnRender += OnCameraRender;
				Vector3 position = Nekki.Vector.Core.Camera.Camera.Current.Position;
				float p_x = _FactorX * position.x;
				float p_y = _FactorY * position.y;
				_LastCameraPosition = new Vector3f(p_x, p_y, 0f);
			}
			_LastParentOffset = _ParentTransform.position * _FactorX;
			_CachedTransform.position = _StartPosition + _LastCameraPosition - _LastParentOffset;
		}

		protected virtual void OnDestory()
		{
			if (Nekki.Vector.Core.Camera.Camera.Current != null)
			{
				Nekki.Vector.Core.Camera.Camera.Current.OnRender -= OnCameraRender;
			}
		}

		protected virtual void UpdatePosition()
		{
			Vector3 position = Nekki.Vector.Core.Camera.Camera.Current.Position;
			float num = _FactorX * position.x;
			float num2 = _FactorY * position.y;
			Vector3 vector = new Vector3(num - _LastCameraPosition.x, num2 - _LastCameraPosition.y);
			Vector3 vector2 = _ParentTransform.position * _FactorX;
			Vector3 vector3 = _CachedTransform.parent.InverseTransformVector(vector - (vector2 - _LastParentOffset));
			_CachedTransform.localPosition += vector3;
			_LastCameraPosition = new Vector3(num, num2);
			_LastCameraOffset = vector;
			_LastParentOffset = vector2;
		}

		private void OnCameraRender()
		{
			if (Nekki.Vector.Core.Camera.Camera.Current != null && Mathf.Abs(_FactorX) > float.Epsilon)
			{
				UpdatePosition();
			}
		}
	}
}
