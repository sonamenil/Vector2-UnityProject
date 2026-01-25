using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Nekki.Vector.Core.Audio.Internal
{
	public class AudioManager : MonoBehaviour
	{
		private static AudioManager _Instance;

		private static Dictionary<int, AudioChannel> _Chanels = new Dictionary<int, AudioChannel>();

		private static List<int> _MusicChanels;

		private static Dictionary<string, SoundData> _SoundData;

		private static AudioMixerGroup[] _MixerGroups;

		public static void Init(int p_musicChanel, int p_soundChannel, AudioMixerGroup[] p_mixerGroups)
		{
			Init(new int[1] { p_musicChanel }, new int[1] { p_soundChannel }, p_mixerGroups);
		}

		public static void Init(int[] p_musicChannels, int[] p_soundChannels, AudioMixerGroup[] p_mixerGroups)
		{
			if (_Instance != null)
			{
				Debug.LogWarning("AudioManager already exists!");
				return;
			}
			_MusicChanels = new List<int>(p_musicChannels);
			_SoundData = new Dictionary<string, SoundData>();
			_MixerGroups = p_mixerGroups;
			GameObject gameObject = new GameObject("[AudioManager]");
			_Instance = gameObject.AddComponent<AudioManager>();
			Object.DontDestroyOnLoad(gameObject);
			AudioUnitPool.Init(_Instance);
		}

		public static void AddSoundData(SoundData p_data)
		{
			if (_SoundData.ContainsKey(p_data.Name))
			{
				_SoundData[p_data.Name] = p_data;
			}
			else
			{
				_SoundData.Add(p_data.Name, p_data);
			}
		}

		public static SoundData GetSoundData(string p_name)
		{
			SoundData value = null;
			_SoundData.TryGetValue(p_name, out value);
			return value;
		}

		public static bool ContainsSoundData(string p_name)
		{
			return _SoundData.ContainsKey(p_name);
		}

		public static bool IsPlaying(int p_channelID)
		{
			AudioChannel channel = GetChannel(p_channelID);
			return channel != null && channel.IsPlaying;
		}

		public static void Play(int p_channelID, string p_sound, bool p_loop, bool p_multisource, float p_volume = 1f, string p_mixerGroupName = null)
		{
			if (ContainsSoundData(p_sound))
			{
				SoundData soundData = GetSoundData(p_sound);
				PlayCommand p_cmd = new PlayCommand(p_channelID, p_sound, p_loop, p_multisource, p_volume * soundData.Volume, p_mixerGroupName);
				Play(p_cmd);
			}
		}

		private static void Play(PlayCommand p_cmd)
		{
			AudioChannel orCreateChannel = GetOrCreateChannel(p_cmd.ChannelID);
			orCreateChannel.Play(p_cmd);
		}

		public static bool IsMute(int p_channelID)
		{
			AudioChannel channel = GetChannel(p_channelID);
			return channel != null && channel.IsMute;
		}

		public static void Mute(int p_channelID)
		{
			AudioChannel orCreateChannel = GetOrCreateChannel(p_channelID);
			orCreateChannel.Mute();
		}

		public static void UnMute(int p_channelID)
		{
			AudioChannel orCreateChannel = GetOrCreateChannel(p_channelID);
			orCreateChannel.UnMute();
		}

		public static void Pause(bool p_pause, int p_channelID)
		{
			AudioChannel channel = GetChannel(p_channelID);
			if (channel != null)
			{
				channel.Pause(p_pause);
			}
		}

		public static void Stop(int p_channelID, float p_stopTime = 0f)
		{
			AudioChannel channel = GetChannel(p_channelID);
			if (channel != null)
			{
				channel.Stop(p_stopTime);
			}
		}

		public static void Stop(int p_channelID, string p_sound, float p_stopTime = 0f)
		{
			AudioChannel channel = GetChannel(p_channelID);
			if (channel != null)
			{
				channel.Stop(p_sound, p_stopTime);
			}
		}

		public static void SetVolume(float p_volume, int p_channelID)
		{
			AudioChannel orCreateChannel = GetOrCreateChannel(p_channelID);
			orCreateChannel.MasterVolume = p_volume;
		}

		private static bool IsMusicChanel(int p_value)
		{
			return _MusicChanels.Contains(p_value);
		}

		private static AudioChannel GetChannel(int p_channelID)
		{
			AudioChannel value = null;
			_Chanels.TryGetValue(p_channelID, out value);
			return value;
		}

		private static AudioChannel GetOrCreateChannel(int p_channelID)
		{
			AudioChannel value = null;
			if (!_Chanels.TryGetValue(p_channelID, out value))
			{
				value = new AudioChannel(p_channelID, IsMusicChanel(p_channelID));
				_Chanels.Add(p_channelID, value);
			}
			return value;
		}

		public static AudioMixerGroup GetAudioMixerGroup(string p_name)
		{
			if (_MixerGroups == null)
			{
				return null;
			}
			int i = 0;
			for (int num = _MixerGroups.Length; i < num; i++)
			{
				if (_MixerGroups[i].name == p_name)
				{
					return _MixerGroups[i];
				}
			}
			return null;
		}
	}
}
