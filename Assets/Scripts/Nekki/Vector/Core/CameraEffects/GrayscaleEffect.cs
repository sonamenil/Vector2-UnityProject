using UnityEngine;

namespace Nekki.Vector.Core.CameraEffects
{
	[AddComponentMenu("Image Effects/Nekki/Vector/Grayscale")]
	[ExecuteInEditMode]
	public class GrayscaleEffect : CameraEffectBase
	{
		[SerializeField]
		private Texture _TextureRamp;

		[SerializeField]
		private float _RampOffset;

		public override CameraEffectType Type
		{
			get
			{
				return CameraEffectType.Grayscale;
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetTexture("_RampTex", _TextureRamp);
			base.Material.SetFloat("_RampOffset", _RampOffset);
			Graphics.Blit(source, destination, base.Material);
		}
	}
}
