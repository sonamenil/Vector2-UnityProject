using System;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	[Serializable]
	public class ProgressBarSegmentUIParameters
	{
		public Color BackgroundColor = ColorUtils.FromHex("3B697E");

		public Color ProgressColor = ColorUtils.FromHex("5EA4BB");

		public string BoostGlowSpriteName = "cards.boost_indicator_blue";

		public int LineHeight = 175;

		public int LineWidth = 6;

		public int AnimatedProgress = 4;

		public int MaxProgress = 5;

		public ProgressBarSegmentFillType FillType = ProgressBarSegmentFillType.UseProgress;

		public bool ShowAnimation;
	}
}
