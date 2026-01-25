using UnityEngine;

namespace Nekki.Vector.Core.CameraEffects
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	[AddComponentMenu("")]
	public abstract class CameraEffectBase : MonoBehaviour
	{
		[SerializeField]
		protected Shader _Shader;

		protected Material _Material;

		protected bool _HasTimer;

		private uint _EndFrame;

		public abstract CameraEffectType Type { get; }

		protected Material Material
		{
			get
			{
				if (_Material == null)
				{
					_Material = new Material(_Shader);
					_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return _Material;
			}
		}

		public virtual void Show(int frames = -1)
		{
			base.enabled = true;
			if (frames >= 0)
			{
				SetTimer(frames);
			}
		}

		public virtual void Hide()
		{
			base.enabled = false;
		}

		public virtual void Toggle()
		{
			base.enabled = !base.enabled;
		}

		protected virtual void SetTimer(int frames)
		{
			_HasTimer = true;
			_EndFrame = Scene.FrameCount + (uint)frames;
		}

		protected virtual void Start()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				base.enabled = false;
			}
			else if (!_Shader || !_Shader.isSupported)
			{
				base.enabled = false;
			}
		}

		protected virtual void OnDisable()
		{
			if ((bool)_Material)
			{
				Object.DestroyImmediate(_Material);
			}
		}

		protected virtual void FixedUpdate()
		{
			if (_HasTimer && Scene.FrameCount > _EndFrame)
			{
				base.enabled = false;
			}
		}
	}
}
