using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Social
{
	public class Avatars : MonoBehaviour
	{
		private class Request
		{
			public UserInfo Info;

			public Action<string, Texture> OnDone;
		}

		private static Avatars _instance;

		private static readonly Dictionary<string, Texture> _avatars = new Dictionary<string, Texture>();

		private static readonly Dictionary<string, Request> _shedule = new Dictionary<string, Request>();

		private static string _current;

		private bool _inProcess;

		private static void Init()
		{
			if (!_instance)
			{
				_instance = new GameObject("_avatar").AddComponent<Avatars>();
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
			}
		}

		public static void GetAvatar(UserInfo info, Action<string, Texture> onDone)
		{
			Init();
			if (_avatars.ContainsKey(info.UserID))
			{
				onDone(info.UserID, _avatars[info.UserID]);
				return;
			}
			_shedule.Add(info.UserID, new Request
			{
				Info = info,
				OnDone = onDone
			});
		}

		private void Update()
		{
			if (!_inProcess && _shedule.Count > 0)
			{
				StartCoroutine(Load());
			}
		}

		private IEnumerator Load()
		{
			_inProcess = true;
			Request r = null;
			foreach (KeyValuePair<string, Request> item in _shedule)
			{
				r = item.Value;
			}
			if (r != null)
			{
				WWW www = new WWW(r.Info.PhotoURL);
				yield return www;
				if (string.IsNullOrEmpty(www.error))
				{
					if (_avatars.ContainsKey(r.Info.UserID))
					{
						_avatars[r.Info.UserID] = www.texture;
					}
					else
					{
						_avatars.Add(r.Info.UserID, www.texture);
					}
					r.OnDone(r.Info.UserID, _avatars[r.Info.UserID]);
				}
				_shedule.Remove(r.Info.UserID);
			}
			_inProcess = false;
		}
	}
}
