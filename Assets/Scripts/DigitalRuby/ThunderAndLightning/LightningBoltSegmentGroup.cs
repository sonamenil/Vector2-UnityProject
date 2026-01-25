using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltSegmentGroup
	{
		public float LineWidth;

		public int StartIndex;

		public int Generation;

		public float Delay;

		public float PeakStart;

		public float PeakEnd;

		public float LifeTime;

		public float EndWidthMultiplier;

		public readonly List<LightningBoltSegment> Segments = new List<LightningBoltSegment>();

		public readonly List<Light> Lights = new List<Light>();

		public LightningLightParameters LightParameters;

		public int SegmentCount
		{
			get
			{
				return Segments.Count - StartIndex;
			}
		}
	}
}
