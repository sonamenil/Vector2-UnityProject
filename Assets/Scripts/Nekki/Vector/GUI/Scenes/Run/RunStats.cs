using Nekki.Vector.Core;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class RunStats : MonoBehaviour
	{
		public Text LabelTrack;

		public Text LabelRoom;

		public Text LabelChoicesData;

		public Text LabelAnimation;

		public static RunStats Current { get; private set; }

		public string TrackName { get; private set; }

		public string RoomName { get; private set; }

		public string RoomUniqueName { get; private set; }

		public string ChoicesData { get; private set; }

		public string AnimationName { get; private set; }

		public void Init()
		{
			Current = this;
			Clear();
			LabelChoicesData.gameObject.SetActive(Settings.Visual.ShowChoices);
			if (!Settings.Visual.DrawMusicLabel)
			{
				LabelTrack.gameObject.SetActive(false);
			}
			AudioManager.OnMusicStart += OnMusicStart;
		}

		public void Free()
		{
			AudioManager.OnMusicStart -= OnMusicStart;
			Current = null;
		}

		private void FixedUpdate()
		{
			if (RunMainController.Scene != null)
			{
				UpdateRoom();
				UpdateAnimation();
			}
		}

		private void Clear()
		{
			TrackName = string.Empty;
			RoomName = string.Empty;
			RoomUniqueName = string.Empty;
			ChoicesData = string.Empty;
			AnimationName = string.Empty;
			LabelTrack.text = string.Empty;
			LabelRoom.text = string.Empty;
			LabelChoicesData.text = string.Empty;
			LabelAnimation.text = string.Empty;
		}

		private void OnMusicStart(string p_name)
		{
			TrackName = p_name;
			LabelTrack.text = "Track:" + TrackName;
		}

		private void UpdateRoom()
		{
			Room currentRoom = RunMainController.Location.CurrentRoom;
			string text = ((currentRoom == null) ? "Outside any room" : currentRoom.UniqueName);
			if (RoomUniqueName != text)
			{
				RoomUniqueName = text;
				RoomName = ((currentRoom == null) ? "Outside any room" : currentRoom.Name);
				LabelRoom.text = RoomName;
				ChoicesData = RunMainController.Location.Sets.GetChoisesDebugInfo(RoomUniqueName);
				LabelChoicesData.text = ChoicesData;
			}
		}

		private void UpdateAnimation()
		{
			ModelHuman player = RunMainController.Player;
			if (player != null && player.ControllerAnimation != null && player.ControllerAnimation.Animation != null)
			{
				AnimationName = player.ControllerAnimation.CurrentFrame + " - " + player.ControllerAnimation.Animation.Name;
				LabelAnimation.text = AnimationName;
			}
			else
			{
				AnimationName = "null";
				LabelAnimation.text = AnimationName;
			}
		}
	}
}
