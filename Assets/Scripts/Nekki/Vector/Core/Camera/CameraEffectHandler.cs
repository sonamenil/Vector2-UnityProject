using Nekki.Vector.Core.CameraEffects;
using UnityEngine;

namespace Nekki.Vector.Core.Camera
{
	public class CameraEffectHandler
	{
		private const float _LightSmooth = 0.3f;

		private Vector2 _LightV = Vector2.zero;

		protected DarknessEffect GetDarknessEffect()
		{
			return CameraEffectManager.GetEffect(CameraEffectType.Darkness) as DarknessEffect;
		}

		protected Vector2 CalculateLightTarget(Vector2 p_currentTarget, Vector2 p_screenPos)
		{
			Vector2 target = new Vector2(p_screenPos.x / (float)Screen.width, p_screenPos.y / (float)Screen.height);
			return Vector2.SmoothDamp(p_currentTarget, target, ref _LightV, 0.3f);
		}
	}
}
