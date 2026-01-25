using UnityEngine;

namespace Nekki.Vector.Core.Audio.Internal
{
	public class PlayCommand
	{
		private float _Volume;

		public int ChannelID { get; private set; }

		public string Sound { get; private set; }

		public bool Loop { get; private set; }

		public bool MultiSource { get; private set; }

		public bool IsMusic { get; private set; }

		public string MixerGroupName { get; private set; }

		public float Volume
		{
			get
			{
				return _Volume;
			}
			set
			{
				_Volume = Mathf.Clamp01(value);
			}
		}

		public PlayCommand(int p_channelID, string p_sound, bool p_loop, bool p_multiSource, float p_volume, string p_mixerGroupName)
		{
			ChannelID = p_channelID;
			Sound = p_sound;
			Loop = p_loop;
			MultiSource = p_multiSource;
			Volume = p_volume;
			MixerGroupName = p_mixerGroupName;
		}
	}
}
