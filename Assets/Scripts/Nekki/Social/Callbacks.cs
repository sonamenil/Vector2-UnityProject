using System;
using System.Collections.Generic;

namespace Nekki.Social
{
	public class Callbacks
	{
		public Action<SocialNetworks, string> onInit;

		public Action<UserInfo> onUserInfo;

		public Action<Dictionary<string, UserInfo>> onInFriends;

		public Action<Dictionary<string, UserInfo>> onOutFriends;

		public Action<bool> onBookmark;

		public Action<bool> onGroupMembership;

		public Action<string> onWallPost;

		public Action<string> onSuccessOrder;

		public Action<bool> onPause;
	}
}
