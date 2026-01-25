using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Audio.Internal
{
	public class AudioChannel
	{
		private Dictionary<string, AudioUnit> _ActiveUnits = new Dictionary<string, AudioUnit>();

		public int ID { get; private set; }

		public bool IsMusic { get; private set; }

		public float MasterVolume { get; set; }

		public bool IsMute { get; set; }

		public bool IsPlaying
		{
			get
			{
				return _ActiveUnits.Count > 0;
			}
		}

		public AudioChannel(int p_id, bool p_isMusic)
		{
			ID = p_id;
			IsMusic = p_isMusic;
			MasterVolume = 1f;
			IsMute = false;
		}

		public void Play(PlayCommand p_command)
		{
			if (IsMute && !IsMusic)
			{
				return;
			}
			AudioClip audioClip = AudioCache.GetAudioClip(p_command.Sound);
			if (!(audioClip == null))
			{
				if (!p_command.MultiSource)
				{
					Stop(0f);
				}
				AudioUnit orCreateAudioUnit = GetOrCreateAudioUnit(p_command.Sound);
				if (!(orCreateAudioUnit == null))
				{
					orCreateAudioUnit.Init(this, p_command, audioClip);
				}
			}
		}

		public void Pause(bool p_pause)
		{
			if (p_pause)
			{
				foreach (AudioUnit value in _ActiveUnits.Values)
				{
					value.Pause();
				}
				return;
			}
			foreach (AudioUnit value2 in _ActiveUnits.Values)
			{
				value2.UnPause();
			}
		}

		public void Pause(bool p_pause, string p_sound)
		{
			AudioUnit audioUnit = GetAudioUnit(p_sound);
			if (audioUnit != null)
			{
				if (p_pause)
				{
					audioUnit.Pause();
				}
				else
				{
					audioUnit.UnPause();
				}
			}
		}

		public void FreeUnit(AudioUnit p_audioUnit)
		{
			foreach (KeyValuePair<string, AudioUnit> activeUnit in _ActiveUnits)
			{
				if (activeUnit.Value == p_audioUnit)
				{
					_ActiveUnits.Remove(activeUnit.Key);
					break;
				}
			}
		}

		public void Stop(float p_stopTime = 0f)
		{
			foreach (AudioUnit value in _ActiveUnits.Values)
			{
				value.Stop(p_stopTime);
			}
			_ActiveUnits.Clear();
		}

		public void Stop(string p_sound, float p_stopTime = 0f)
		{
			AudioUnit audioUnit = GetAudioUnit(p_sound);
			if (audioUnit != null)
			{
				audioUnit.Stop(p_stopTime);
				_ActiveUnits.Remove(p_sound);
			}
		}

		public void Mute()
		{
			foreach (AudioUnit value in _ActiveUnits.Values)
			{
				value.IsMute = true;
			}
			IsMute = true;
		}

		public void UnMute()
		{
			IsMute = false;
			foreach (AudioUnit value in _ActiveUnits.Values)
			{
				value.IsMute = false;
			}
		}

		private AudioUnit GetAudioUnit(string p_soundId)
		{
			AudioUnit value = null;
			_ActiveUnits.TryGetValue(p_soundId, out value);
			return value;
		}

		private AudioUnit GetOrCreateAudioUnit(string p_soundId)
		{
			AudioUnit value = null;
			if (!_ActiveUnits.TryGetValue(p_soundId, out value))
			{
				value = AudioUnitPool.GetIdle();
				if (value != null)
				{
					_ActiveUnits.Add(p_soundId, value);
				}
			}
			return value;
		}
	}
}
