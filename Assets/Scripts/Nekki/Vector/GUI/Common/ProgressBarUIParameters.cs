using System;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	[Serializable]
	public class ProgressBarUIParameters
	{
		public Color BackgroundColor = ColorUtils.FromHex("3B697E");

		public Color ProgressColor = ColorUtils.FromHex("5EA4BB");

		public int ProgressWidth = 6;

		public int LevelWidth = 6;

		public bool ShowNumbers;

		public bool ShowProgress = true;

		public bool ShowAnimation;

		public int AnimationCardCount = 1;

		public CardsGroupAttribute Card;
	}
}
