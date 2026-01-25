using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Audio.Internal;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using UnityEngine;
using UnityEngine.Audio;

namespace Nekki.Vector.Core.Audio
{
	public class AudioManager
	{
		public const float DefaultSmoothStopTime = 0.4f;

		private const int _MusicChannel = 0;

		private const int _SoundsChannel = 1;

		private const int _CutsceneChannel = 2;

		private const int _AmbientChannel = 3;

		private const int _EffectsFirstChannel = 4;

		private const int _EffectsLastChannel = 12;

		private static bool _StopOnPause;

		private static bool _StopOnDeath;

		private static bool _IsAmbientEnabled = true;

		private static List<string> _EquipSounds = new List<string>();

		private static List<string> _ShopSounds = new List<string>();

		private static List<string> _RunSounds = new List<string>();

		private static List<string> _CreditsSounds = new List<string>();

		private static Dictionary<string, string> _SoundPaths = new Dictionary<string, string>();

		private static string _LastMusic = string.Empty;

		public static bool StopOnPause
		{
			get
			{
				return _StopOnPause;
			}
			set
			{
				_StopOnPause = value;
			}
		}

		public static bool StopOnDeath
		{
			get
			{
				return _StopOnDeath;
			}
			set
			{
				_StopOnDeath = value;
			}
		}

		public static bool IsAmbientEnabled
		{
			get
			{
				return _IsAmbientEnabled;
			}
		}

		public static float MusicVolume
		{
			get
			{
				return DataLocal.Current.Settings.VolumeMusic;
			}
			set
			{
				DataLocal.Current.Settings.VolumeMusic = value;
				DataLocal.Current.Save(false);
				Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(value, 0);
				Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(Mathf.Max(value, DataLocal.Current.Settings.VolumeSound), 2);
				UpdateAmbientVolume();
			}
		}

		public static float SoundVolume
		{
			get
			{
				return DataLocal.Current.Settings.VolumeSound;
			}
			set
			{
				DataLocal.Current.Settings.VolumeSound = value;
				DataLocal.Current.Save(false);
				Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(value, 1);
				Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(Mathf.Max(DataLocal.Current.Settings.VolumeMusic, value), 2);
				UpdateAmbientVolume();
				AudioManager.OnSoundVolumeChanged(value);
			}
		}

		public static bool IsMusicPlaying
		{
			get
			{
				return Nekki.Vector.Core.Audio.Internal.AudioManager.IsPlaying(0);
			}
		}

		public static bool IsAmbientPlaying
		{
			get
			{
				return Nekki.Vector.Core.Audio.Internal.AudioManager.IsPlaying(3);
			}
		}

		private static string RandomEquipSceneTrack
		{
			get
			{
				return _EquipSounds[UnityEngine.Random.Range(0, _EquipSounds.Count)];
			}
		}

		private static string RandomShopSceneTrack
		{
			get
			{
				return _ShopSounds[UnityEngine.Random.Range(0, _ShopSounds.Count)];
			}
		}

		private static string RandomRunSceneTrack
		{
			get
			{
				return _RunSounds[UnityEngine.Random.Range(0, _RunSounds.Count)];
			}
		}

		private static string RandomCreditsTrack
		{
			get
			{
				return _CreditsSounds[UnityEngine.Random.Range(0, _CreditsSounds.Count)];
			}
		}

		public static event Action<string> OnMusicStart;

		public static event Action<float> OnSoundVolumeChanged;

		public static event Action<bool> OnSoundMute;

		static AudioManager()
		{
			AudioManager.OnMusicStart = delegate
			{
			};
			AudioManager.OnSoundVolumeChanged = delegate
			{
			};
			AudioManager.OnSoundMute = delegate
			{
			};
		}

		public static void Init()
		{
			AudioMixerGroup[] p_mixerGroups = Resources.LoadAll<AudioMixerGroup>("MainMixer");
			Nekki.Vector.Core.Audio.Internal.AudioManager.Init(new int[3] { 0, 2, 3 }, new int[1] { 1 }, p_mixerGroups);
			ParseSoundtracks();
			ParseMusic();
			ParseSound();
			RestoreAudioSettings();
		}

		private static void ParseSoundtracks()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.Settings, "soundtracks.xml");
			XmlNode xmlNode = xmlDocument["Root"]["Equip"];
			XmlNode xmlNode2 = xmlDocument["Root"]["Shop"];
			XmlNode xmlNode3 = xmlDocument["Root"]["Run"];
			XmlNode xmlNode4 = xmlDocument["Root"]["Credits"];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				_EquipSounds.Add(XmlUtils.ParseString(childNode.Attributes["Name"], string.Empty));
			}
			foreach (XmlNode childNode2 in xmlNode2.ChildNodes)
			{
				_ShopSounds.Add(XmlUtils.ParseString(childNode2.Attributes["Name"], string.Empty));
			}
			foreach (XmlNode childNode3 in xmlNode3.ChildNodes)
			{
				_RunSounds.Add(XmlUtils.ParseString(childNode3.Attributes["Name"], string.Empty));
			}
			foreach (XmlNode childNode4 in xmlNode4.ChildNodes)
			{
				_CreditsSounds.Add(XmlUtils.ParseString(childNode4.Attributes["Name"], string.Empty));
			}
		}

		private static void ParseSound()
		{
			DebugUtils.Log("ParseSounds");
			List<string> list = new List<string>();
			list.Add("sounds_old.xml");
			list.Add("sounds.xml");
			foreach (string item in list)
			{
				XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.Sounds, item);
				foreach (XmlNode childNode in xmlDocument["Sounds"].ChildNodes)
				{
					string text = XmlUtils.ParseString(childNode.Attributes["Name"], string.Empty);
					string p_path = XmlUtils.ParseString(childNode.Attributes["File"], string.Empty);
					float p_volume = XmlUtils.ParseFloat(childNode.Attributes["Volume"], 1f);
					AddSoundPath(text, p_path);
					Nekki.Vector.Core.Audio.Internal.AudioManager.AddSoundData(new SoundData(text, p_volume));
				}
			}
		}

		private static void ParseMusic()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.Sounds, "music.xml");
			foreach (XmlNode childNode in xmlDocument["Sounds"].ChildNodes)
			{
				string text = XmlUtils.ParseString(childNode.Attributes["Name"], string.Empty);
				string p_path = XmlUtils.ParseString(childNode.Attributes["File"], string.Empty);
				float p_volume = XmlUtils.ParseFloat(childNode.Attributes["Volume"], 1f);
				AddSoundPath(text, p_path);
				Nekki.Vector.Core.Audio.Internal.AudioManager.AddSoundData(new SoundData(text, p_volume));
			}
		}

		public static void RestoreAudioSettings()
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(DataLocal.Current.Settings.VolumeMusic, 0);
			Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(DataLocal.Current.Settings.VolumeSound, 1);
			Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(Mathf.Max(DataLocal.Current.Settings.VolumeMusic, DataLocal.Current.Settings.VolumeSound), 2);
			if (DataLocal.Current.Settings.MuteMusic)
			{
				MuteMusic();
			}
			else
			{
				UnMuteMusic();
			}
			if (DataLocal.Current.Settings.MuteSound)
			{
				MuteSounds();
			}
			else
			{
				UnMuteSounds();
			}
		}

		public static void PlayRandomMenuMusic()
		{
			PlayMusic((!Manager.IsEquip) ? RandomShopSceneTrack : RandomEquipSceneTrack, true);
		}

		public static void PlayRandomRunMusic()
		{
			PlayMusic(RandomRunSceneTrack, true);
		}

		public static void PlaySpecificRunMusic(string p_trackName)
		{
			if (!_RunSounds.Contains(p_trackName))
			{
				Debug.Log("[AudioManager]: Try to play nonexisting music track: " + p_trackName);
			}
			else
			{
				PlayMusic(p_trackName, true);
			}
		}

		public static void PlayRandomCreditsMusic()
		{
			PlayMusic(RandomCreditsTrack, true);
		}

		public static void PlayMusic(string p_name, bool p_loop = false, bool p_multySource = false)
		{
			if (ContainsSoundPath(p_name))
			{
				AudioCache.UnloadAudioClip(_LastMusic);
				_LastMusic = p_name;
				LoadAudioClip(p_name);
				Nekki.Vector.Core.Audio.Internal.AudioManager.Play(0, p_name, p_loop, p_multySource, 1f, "Music");
				UpdateAmbientVolume();
				AudioManager.OnMusicStart(p_name);
			}
		}

		public static void PlaySound(string p_name, float p_volume = 1f, bool p_isLoop = false)
		{
			LoadAudioClip(p_name);
			Nekki.Vector.Core.Audio.Internal.AudioManager.Play(1, p_name, p_isLoop, true, p_volume);
		}

		public static void PlaySoundDuck(string p_name, float p_volume = 1f, bool p_isLoop = false)
		{
			LoadAudioClip(p_name);
			Nekki.Vector.Core.Audio.Internal.AudioManager.Play(1, p_name, p_isLoop, true, p_volume, "SoundsDuck");
		}

		public static void PlayCutscene(string p_name, float p_volume = 1f, bool p_isLoop = false)
		{
			LoadAudioClip(p_name);
			Nekki.Vector.Core.Audio.Internal.AudioManager.Play(2, p_name, p_isLoop, true, p_volume);
		}

		public static void PlayAmbient(string p_name, float p_volume = 1f)
		{
			LoadAudioClip(p_name);
			Nekki.Vector.Core.Audio.Internal.AudioManager.Play(3, p_name, true, true, p_volume);
		}

		public static int PlayEffect(string p_name, float p_volume = 1f, bool p_isLoop = false)
		{
			if (!DataLocal.Current.Settings.MuteSound)
			{
				int freeEffectChannel = GetFreeEffectChannel();
				LoadAudioClip(p_name);
				Nekki.Vector.Core.Audio.Internal.AudioManager.Play(freeEffectChannel, p_name, p_isLoop, true, p_volume);
				return freeEffectChannel;
			}
			return -1;
		}

		public static void StopMusic(float p_stopTime = 0f)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Stop(0, p_stopTime);
		}

		public static void StopSound(float p_stopTime = 0f)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Stop(1, p_stopTime);
		}

		public static void StopSound(string p_soundId, float p_stopTime = 0f)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Stop(1, p_soundId, p_stopTime);
		}

		public static void StopCutscene(float p_stopTime = 0f)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Stop(2, p_stopTime);
		}

		public static void StopAmbient(float p_stopTime = 0f)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Stop(3, p_stopTime);
		}

		public static void PauseMusic(bool p_value)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Pause(p_value, 0);
		}

		public static void PauseCutscene(bool p_value)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Pause(p_value, 2);
		}

		public static void PauseAmbient(bool p_value)
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Pause(p_value, 3);
		}

		public static void MuteMusic()
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Mute(0);
			MuteCutscene();
			UpdateAmbientVolume();
		}

		public static void UnMuteMusic()
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.UnMute(0);
			UnMuteCutscene();
			UpdateAmbientVolume();
		}

		public static void MuteSounds()
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.Stop(1);
			Nekki.Vector.Core.Audio.Internal.AudioManager.Mute(1);
			Nekki.Vector.Core.Audio.Internal.AudioManager.Mute(3);
			MuteCutscene();
			MuteEffects();
			AudioManager.OnSoundMute(true);
		}

		public static void UnMuteSounds()
		{
			Nekki.Vector.Core.Audio.Internal.AudioManager.UnMute(1);
			Nekki.Vector.Core.Audio.Internal.AudioManager.UnMute(3);
			UnMuteCutscene();
			UnMuteEffects();
			AudioManager.OnSoundMute(false);
		}

		private static void MuteCutscene()
		{
			if (DataLocal.Current.Settings.MuteMusic && DataLocal.Current.Settings.MuteSound)
			{
				Nekki.Vector.Core.Audio.Internal.AudioManager.Mute(2);
			}
		}

		private static void UnMuteCutscene()
		{
			if (!DataLocal.Current.Settings.MuteMusic || !DataLocal.Current.Settings.MuteSound)
			{
				Nekki.Vector.Core.Audio.Internal.AudioManager.UnMute(2);
			}
		}

		public static void MuteEffects()
		{
			for (int i = 4; i <= 12; i++)
			{
				Nekki.Vector.Core.Audio.Internal.AudioManager.Stop(i);
				Nekki.Vector.Core.Audio.Internal.AudioManager.Mute(i);
			}
		}

		public static void UnMuteEffects()
		{
			for (int i = 4; i <= 12; i++)
			{
				Nekki.Vector.Core.Audio.Internal.AudioManager.UnMute(i);
			}
		}

		public static void SetEffectVolume(float p_volume, int p_channel)
		{
			if (!DataLocal.Current.Settings.MuteSound)
			{
				Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume(p_volume * SoundVolume, p_channel);
			}
		}

		private static void AddSoundPath(string p_sound, string p_path)
		{
			if (_SoundPaths.ContainsKey(p_sound))
			{
				_SoundPaths[p_sound] = p_path;
			}
			else
			{
				_SoundPaths.Add(p_sound, p_path);
			}
		}

		private static bool ContainsSoundPath(string p_sound)
		{
			return _SoundPaths.ContainsKey(p_sound);
		}

		private static string GetSoundPath(string p_sound)
		{
			string value = string.Empty;
			_SoundPaths.TryGetValue(p_sound, out value);
			return value;
		}

		public static int GetFreeEffectChannel()
		{
			for (int i = 4; i < 12; i++)
			{
				if (!Nekki.Vector.Core.Audio.Internal.AudioManager.IsPlaying(i))
				{
					return i;
				}
			}
			return 12;
		}

		public static AudioClip GetClip(string p_sound)
		{
			LoadAudioClip(p_sound);
			return AudioCache.GetAudioClip(p_sound);
		}

		public static void UpdateAmbientVolume()
		{
			_IsAmbientEnabled = DataLocal.Current.Settings.MuteMusic || MusicVolume == 0f || !IsMusicPlaying;
			Nekki.Vector.Core.Audio.Internal.AudioManager.SetVolume((!_IsAmbientEnabled) ? 0f : SoundVolume, 3);
		}

		private static void LoadAudioClip(string p_sound)
		{
			if (!AudioCache.ContainsAudioClip(p_sound) && ContainsSoundPath(p_sound))
			{
				AudioClip audioClip = ResourceManager.GetAudioClip(VectorPaths.Sounds + "/" + GetSoundPath(p_sound));
				if (audioClip != null)
				{
					AudioCache.AddAudioClip(p_sound, audioClip);
				}
			}
		}
	}
}
