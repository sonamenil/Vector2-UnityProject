using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public abstract class LightningBoltPrefabScriptBase : LightningBoltScript
	{
		private const float duration = 0.3f;

		private const float overlapMultiplier = 0.35f;

		[SingleLineClamp("How long to wait before creating another round of lightning bolts in seconds", 0.001, double.MaxValue)]
		public RangeOfFloats IntervalRange = new RangeOfFloats
		{
			Minimum = 0.05f,
			Maximum = 0.1f
		};

		[SingleLineClamp("How many lightning bolts to emit for each interval", 0.0, 100.0)]
		public RangeOfIntegers CountRange = new RangeOfIntegers
		{
			Minimum = 1,
			Maximum = 1
		};

		[SingleLineClamp("Delay in seconds (range) before each lightning bolt in count range is emitted", 0.0, 30.0)]
		public RangeOfFloats DelayRange = new RangeOfFloats
		{
			Minimum = 0f,
			Maximum = 0f
		};

		[SingleLineClamp("For each bolt emitted, how long should it stay in seconds", 0.01, 10.0)]
		public RangeOfFloats DurationRange = new RangeOfFloats
		{
			Minimum = 0.06f,
			Maximum = 0.12f
		};

		[SingleLineClamp("The trunk width range in unity units (x = min, y = max)", 0.0001, 100.0)]
		public RangeOfFloats TrunkWidthRange = new RangeOfFloats
		{
			Minimum = 0.1f,
			Maximum = 0.2f
		};

		[Tooltip("How long (in seconds) this game object should live before destroying itself. Leave as 0 for infinite.")]
		[Range(0f, 1000f)]
		public float LifeTime;

		[Tooltip("Generations (1 - 8, higher makes more detailed but more expensive lightning)")]
		[Range(1f, 8f)]
		public int Generations = 6;

		[Tooltip("The chaos factor determines how far the lightning can spread out, higher numbers spread out more. 0 - 1.")]
		[Range(0f, 1f)]
		public float ChaosFactor = 0.075f;

		[Range(0f, 1f)]
		[Tooltip("The intensity of the glow, 0 - 1")]
		public float GlowIntensity = 0.1f;

		[Range(0f, 64f)]
		[Tooltip("The width multiplier for the glow, 0 - 64")]
		public float GlowWidthMultiplier = 4f;

		[Tooltip("How forked should the lightning be? (0 - 1, 0 for none, 1 for lots of forks)")]
		[Range(0f, 1f)]
		public float Forkedness = 0.25f;

		[Range(0f, 0.5f)]
		[Tooltip("What percent of time the lightning should fade in and out. For example, 0.15 fades in 15% of the time and fades out 15% of the time, with full visibility 70% of the time.")]
		public float FadePercent = 0.15f;

		[Tooltip("0 - 1, how slowly the lightning should grow. 0 for instant, 1 for slow.")]
		[Range(0f, 1f)]
		public float GrowthMultiplier;

		[Range(0f, 10f)]
		[Tooltip("How much smaller the lightning should get as it goes towards the end of the bolt. For example, 0.5 will make the end 50% the width of the start.")]
		public float EndWidthMultiplier = 0.5f;

		[Tooltip("Light parameters")]
		public LightningLightParameters LightParameters;

		[Range(0f, 64f)]
		[Tooltip("Maximum number of lights that can be created per batch of lightning")]
		public int MaximumLightsPerBatch = 8;

		private readonly List<LightningBoltParameters> batchParameters = new List<LightningBoltParameters>();

		private static readonly System.Random random = new System.Random();

		private float nextArc;

		private float lifeTimeRemaining;

		private void CreateInterval(float offset)
		{
			nextArc = offset + (float)random.NextDouble() * (IntervalRange.Maximum - IntervalRange.Minimum) + IntervalRange.Minimum;
		}

		private void CallLightning()
		{
			int num = random.Next(CountRange.Minimum, CountRange.Maximum + 1);
			while (num-- > 0)
			{
				float lifeTime = (float)random.NextDouble() * (DurationRange.Maximum - DurationRange.Minimum) + DurationRange.Maximum;
				float trunkWidth = (float)random.NextDouble() * (TrunkWidthRange.Maximum - TrunkWidthRange.Minimum) + TrunkWidthRange.Maximum;
				LightningBoltParameters lightningBoltParameters = new LightningBoltParameters();
				lightningBoltParameters.Generations = Generations;
				lightningBoltParameters.LifeTime = lifeTime;
				lightningBoltParameters.ChaosFactor = ChaosFactor;
				lightningBoltParameters.TrunkWidth = trunkWidth;
				lightningBoltParameters.GlowIntensity = GlowIntensity;
				lightningBoltParameters.GlowWidthMultiplier = GlowWidthMultiplier;
				lightningBoltParameters.Forkedness = Forkedness;
				lightningBoltParameters.FadePercent = FadePercent;
				lightningBoltParameters.GrowthMultiplier = GrowthMultiplier;
				lightningBoltParameters.EndWidthMultiplier = EndWidthMultiplier;
				lightningBoltParameters.Random = random;
				lightningBoltParameters.Delay = UnityEngine.Random.Range(DelayRange.Minimum, DelayRange.Maximum);
				lightningBoltParameters.LightParameters = LightParameters;
				LightningBoltParameters p = lightningBoltParameters;
				CreateLightningBolt(p);
			}
			int maximumLightsPerBatch = LightningBolt.MaximumLightsPerBatch;
			LightningBolt.MaximumLightsPerBatch = MaximumLightsPerBatch;
			CreateLightningBolts(batchParameters);
			LightningBolt.MaximumLightsPerBatch = maximumLightsPerBatch;
			batchParameters.Clear();
		}

		protected override void Start()
		{
			base.Start();
			CreateInterval(0f);
			lifeTimeRemaining = ((!(LifeTime <= 0f)) ? LifeTime : float.MaxValue);
		}

		protected override void Update()
		{
			base.Update();
			if ((lifeTimeRemaining -= Time.deltaTime) < 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if ((nextArc -= Time.deltaTime) < 0f)
			{
				CreateInterval(nextArc);
				CallLightning();
			}
		}

		public override void CreateLightningBolt(LightningBoltParameters p)
		{
			batchParameters.Add(p);
		}
	}
}
