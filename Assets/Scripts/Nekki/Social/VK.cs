namespace Nekki.Social
{
	public class VK : ISocialNetwork
	{
		private SocialWrapper _wrap;

		public bool NetworkHasGroups
		{
			get
			{
				return true;
			}
		}

		public bool NetworkHasBookmarks
		{
			get
			{
				return true;
			}
		}

		public SocialNetworks Network
		{
			get
			{
				return SocialNetworks.VKontakte;
			}
		}

		public void Init(SocialWrapper wrapper)
		{
			_wrap = wrapper;
		}

		public void GetUser(string userId)
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
				return;
			}
			_wrap.RequestUsersInfo(new string[1] { userId });
		}

		public void GetUsers(string[] userIds)
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.RequestUsersInfo(userIds);
			}
		}

		public void GetFriends()
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.RequestFriends();
			}
		}

		public void InviteFriends()
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.Invite();
			}
		}

		public void GetBookmarkState()
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.RequestBookmark(false);
			}
		}

		public void ShowBookmarkBox()
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.RequestBookmark(true);
			}
		}

		public void CheckGroupMembership()
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.RequestCheckGroupMembership();
			}
		}

		public void WallPost(string userId, string message, string content)
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.RequestWallPost(userId, message, content);
			}
		}

		public void Buy(int item)
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.Buy(item.ToString());
			}
		}

		public void Buy(string item)
		{
			if (!_wrap.Initialized)
			{
				AdvLog.LogWarning("you must call Social.Init(..) method first");
			}
			else
			{
				_wrap.Buy(item);
			}
		}
	}
}
