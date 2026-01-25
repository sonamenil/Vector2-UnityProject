using System;

namespace Nekki.Vector.GUI.Common
{
	[Serializable]
	public class BaseCardUISettings
	{
		public const int DefaultCardSize = 175;

		public int CardSize = 175;

		public bool NeedShowSlot;

		public bool NeedShowProgressBar;

		public bool NeedShowCurrentLevelProgress = true;

		public bool NeedShowProgressNumbers;

		public bool NeedShowProgressAnimation;

		public bool NeedShowNoCardIcon = true;

		public bool NeedShowPlusIcon;

		public float NoCardBorderOpacity = 0.5f;

		public int SlotOffset;

		public bool NeedAnnounce;

		public bool NeedShowForMissionIcon;

		public bool NeedShowLevelUpAnimation;

		public bool NeedShowNoCardAnimation;

		public bool NeedShowStoryCount;

		public int AnimationCardCount;
	}
}
