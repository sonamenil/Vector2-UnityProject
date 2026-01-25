using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	[Serializable]
	public class LightningBoltParameters
	{
		public Vector3 Start;

		public Vector3 End;

		public int Generations;

		public float LifeTime;

		public float Delay;

		public float ChaosFactor;

		public float TrunkWidth;

		public float EndWidthMultiplier = 0.5f;

		public float GlowIntensity;

		public float GlowWidthMultiplier;

		public float Forkedness;

		public System.Random Random;

		public float FadePercent;

		public float GrowthMultiplier;

		public LightningLightParameters LightParameters;
	}
}
