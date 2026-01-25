using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.Core.User
{
	public class UserSettings
	{
		public bool MuteSound;

		public bool MuteMusic;

		public float VolumeMusic;

		public float VolumeSound;

		public SystemLanguage CurrentLanguage;

		public bool UseLowResGraphics;

		public bool SubtitlesOn;

		public bool GameCenterOn;

		public Zone CurrentZone;

		public List<Zone> AvailableZones = new List<Zone>();

		public bool Fullscreen;

		public Resolution CurrentResolution;
	}
}
