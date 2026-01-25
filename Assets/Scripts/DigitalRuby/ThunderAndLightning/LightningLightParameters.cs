using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	[Serializable]
	public class LightningLightParameters
	{
		[HideInInspector]
		[Tooltip("Light render mode - leave as auto unless you have special use cases")]
		public LightRenderMode RenderMode;

		[Tooltip("Color of the light")]
		public Color LightColor = Color.white;

		[Range(0f, 1f)]
		[Tooltip("What percent of segments should have a light? For performance you may want to keep this small.")]
		public float LightPercent = 1E-06f;

		[Range(0f, 1f)]
		[Tooltip("What percent of lights created should cast shadows?")]
		public float LightShadowPercent;

		[Tooltip("Light intensity")]
		[Range(0f, 8f)]
		public float LightIntensity = 0.5f;

		[Range(0f, 8f)]
		[Tooltip("Bounce intensity")]
		public float BounceIntensity;

		[Tooltip("Shadow strength, 0 means all light, 1 means all shadow")]
		[Range(0f, 1f)]
		public float ShadowStrength = 1f;

		[Tooltip("Shadow bias, 0 - 2")]
		[Range(0f, 2f)]
		public float ShadowBias = 0.05f;

		[Range(0f, 3f)]
		[Tooltip("Shadow normal bias, 0 - 3")]
		public float ShadowNormalBias = 0.4f;

		[Tooltip("The range of each light created")]
		public float LightRange;

		[Tooltip("Only light objects that match this layer mask")]
		public LayerMask CullingMask = -1;

		public bool HasLight
		{
			get
			{
				return LightColor.a > 0f && LightIntensity >= 0.01f && LightPercent >= 1E-07f && LightRange > 0.01f;
			}
		}
	}
}
