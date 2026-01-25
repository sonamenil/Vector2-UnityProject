using MKGlowSystem;
using UnityEngine;

namespace Nekki.Vector.Core.CameraEffects
{
	public class CameraGlowEffect : CameraEffectBase
	{
		[SerializeField]
		private MKGlow _GlowEffect;

		public override CameraEffectType Type
		{
			get
			{
				return CameraEffectType.Glow;
			}
		}

		private void Awake()
		{
			_GlowEffect.enabled = !DeviceInformation.IsiOS512mbDevice;
		}

		public override void Show(int frames = -1)
		{
			base.Show(frames);
			_GlowEffect.enabled = true;
		}

		public override void Hide()
		{
			base.Hide();
			_GlowEffect.enabled = false;
		}

		public override void Toggle()
		{
			base.Toggle();
			_GlowEffect.enabled = !_GlowEffect.enabled;
		}

		protected override void Start()
		{
			bool flag = base.enabled;
			base.Start();
			base.enabled = flag;
		}
	}
}
