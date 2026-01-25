using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Audio.Internal
{
	public static class AudioCache
	{
		private static Dictionary<string, AudioClip> _Clips = new Dictionary<string, AudioClip>();

		public static AudioClip GetAudioClip(string p_name)
		{
			AudioClip value = null;
			_Clips.TryGetValue(p_name, out value);
			return value;
		}

		public static bool ContainsAudioClip(string p_name)
		{
			return _Clips.ContainsKey(p_name);
		}

		public static void AddAudioClip(string p_name, AudioClip p_clip)
		{
			if (p_clip == null)
			{
				Debug.LogWarningFormat("[Internal.AudioCache]: add null audio clip - {0}", p_name);
			}
			else if (_Clips.ContainsKey(p_name))
			{
				_Clips[p_name] = p_clip;
			}
			else
			{
				_Clips.Add(p_name, p_clip);
			}
		}

		public static void UnloadAudioClip(string p_name)
		{
			if (_Clips.ContainsKey(p_name))
			{
				_Clips.Remove(p_name);
			}
		}

		public static void Clear()
		{
			_Clips.Clear();
		}
	}
}
