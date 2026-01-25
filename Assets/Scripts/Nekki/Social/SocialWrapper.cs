using System;
using System.Text;
using UnityEngine;

namespace Nekki.Social
{
	public class SocialWrapper : MonoBehaviour
	{
		private string _currentUserID;

		private static SocialWrapper _instance;

		private static Action<SocialNetworks> _onSocialInit;

		private bool _initDone;

		private Callbacks _callbacks;

		internal static UserInfo CurrentUser { get; private set; }

		public bool Initialized
		{
			get
			{
				return _initDone;
			}
		}

		internal static SocialWrapper Init(Callbacks callbacks, Action<SocialNetworks> onSocialInit)
		{
			GameObject gameObject = GameObject.Find("_social");
			if (!gameObject)
			{
				gameObject = new GameObject("_social");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			SocialWrapper component = gameObject.GetComponent<SocialWrapper>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			_instance = gameObject.AddComponent<SocialWrapper>();
			_instance._callbacks = callbacks;
			_onSocialInit = onSocialInit;
			return _instance;
		}

		protected virtual void Start()
		{
			Debug.Log("SocialWrapper Start");
			RequestSocialNetworkInfo();
		}

		private void RequestSocialNetworkInfo()
		{
			Application.ExternalCall("RequestSocialNetworkInfo");
		}

		internal void OnSocialNetworkInfo(string info)
		{
			if (!_initDone && info.Contains("|"))
			{
				_initDone = true;
				string[] array = info.Split('|');
				SocialData(array[0], array[1]);
			}
		}

		protected virtual void SocialData(string network, string userId)
		{
			_currentUserID = userId;
			switch (network)
			{
			case "VK":
				_callbacks.onInit(SocialNetworks.VKontakte, userId);
				_onSocialInit(SocialNetworks.VKontakte);
				break;
			default:
				_callbacks.onInit(SocialNetworks.None, userId);
				_onSocialInit(SocialNetworks.None);
				break;
			}
		}

		internal void RequestUsersInfo(string[] userIds)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < userIds.Length; i++)
			{
				stringBuilder.Append(userIds[i]);
				if (i < userIds.Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
			Application.ExternalCall("RequestUsersInfo", stringBuilder.ToString());
		}

		internal void OnUserInfo(string source)
		{
			UserInfo userInfo = new UserInfo(source);
			if (userInfo.UserID == _currentUserID)
			{
				CurrentUser = userInfo;
			}
			_callbacks.onUserInfo(userInfo);
		}

		internal void RequestFriends()
		{
			Application.ExternalCall("RequestFriends");
		}

		internal void OnInAppFriends(string source)
		{
			_callbacks.onInFriends(UserInfo.GetInfos(source));
		}

		internal void OnOutAppFriends(string source)
		{
			_callbacks.onOutFriends(UserInfo.GetInfos(source));
		}

		internal void Invite()
		{
			Application.ExternalCall("Invite");
		}

		internal void RequestBookmark(bool showBox)
		{
			Application.ExternalCall("RequestBookmark", (!showBox) ? "false" : "true");
		}

		internal void OnBookmark(string state)
		{
			_callbacks.onBookmark(state.Equals("true"));
		}

		internal void RequestCheckGroupMembership()
		{
			Application.ExternalCall("RequestCheckGroupMembership");
		}

		internal void OnCheckGroupMembership(string state)
		{
			_callbacks.onGroupMembership(state.Equals("true"));
		}

		internal void RequestWallPost(string userid, string message, string content)
		{
			Application.ExternalCall("RequestWallPost", userid, message, content);
		}

		internal void OnWallPost(string postId)
		{
			_callbacks.onWallPost(postId);
		}

		internal void Buy(string itemId)
		{
			Application.ExternalCall("Buy", itemId);
		}

		internal void OnBuy(string orderId)
		{
			_callbacks.onSuccessOrder(orderId);
		}

		internal void OnWindowBlur()
		{
			_callbacks.onPause(true);
		}

		internal void OnWindowFocus()
		{
			_callbacks.onPause(false);
		}

		internal void OnSizeChanged(string resolution)
		{
			string[] array = resolution.Split('|');
			int num = int.Parse(array[0]);
			int num2 = int.Parse(array[1]);
			Screen.SetResolution(num, num2, false);
			AdvLog.Log(string.Format("Resolution changed to ({0}x{1})", num, num2));
		}

		private void OnDestroy()
		{
			Delete();
		}

		public static void Delete()
		{
			CurrentUser = null;
			_instance = null;
		}
	}
}
