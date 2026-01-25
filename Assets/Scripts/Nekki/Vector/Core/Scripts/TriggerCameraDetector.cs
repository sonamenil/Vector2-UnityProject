using System;
using Nekki.Vector.Core.Runners;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nekki.Vector.Core.Scripts
{
	public class TriggerCameraDetector : MonoBehaviour
	{
		public Action OnBecameVisibleEvent = delegate
		{
		};

		public Action OnBecameInvisibleEvent = delegate
		{
		};

		private Rectangle _OldRect = new Rectangle(0f, 0f, 0f, 0f);

		private Mesh _Mesh;

		private TriggerRunner _Base;

		private bool _IsVisible;

		private static Shader _Shader;

		private static Material _SharedMaterial;

		public TriggerRunner Base
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

		public bool IsVisible
		{
			get
			{
				return _IsVisible;
			}
			set
			{
				_IsVisible = value;
			}
		}

		private void Start()
		{
			if (_Shader == null)
			{
				_Shader = Shader.Find("Standard");
				_SharedMaterial = new Material(_Shader);
			}
			_Mesh = new Mesh();
			base.gameObject.AddComponent<MeshFilter>().mesh = _Mesh;
			MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			meshRenderer.sharedMaterial = _SharedMaterial;
		}

		private void Update()
		{
			if (_Base != null && !(_OldRect == _Base.Rectangle))
			{
				_OldRect.Set(_Base.Rectangle);
				Vector3 vector = new Vector3(_Base.Rectangle.MinX, _Base.Rectangle.MinY);
				Vector3 vector2 = (Vector3)_Base.Point1 - vector;
				Vector3 vector3 = (Vector3)_Base.Point2 - vector;
				Vector3 vector4 = (Vector3)_Base.Point3 - vector;
				Vector3 vector5 = (Vector3)_Base.Point4 - vector;
				_Mesh.vertices = new Vector3[4] { vector2, vector3, vector4, vector5 };
				_Mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
				_Mesh.RecalculateBounds();
				base.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}

		private void OnBecameVisible()
		{
			if (RunMainController.IsRunNow && RunMainController.Scene != null && !_IsVisible)
			{
				_IsVisible = true;
				RunMainController.Location.TriggersInViewport.Add(Base);
				OnBecameVisibleEvent();
			}
		}

		private void OnBecameInvisible()
		{
			if (RunMainController.IsRunNow && RunMainController.Scene != null && _IsVisible)
			{
				_IsVisible = false;
				RunMainController.Location.TriggersInViewport.Remove(Base);
				OnBecameInvisibleEvent();
			}
		}

		public void End()
		{
			_Base = null;
			OnBecameVisibleEvent = null;
			OnBecameInvisibleEvent = null;
		}
	}
}
