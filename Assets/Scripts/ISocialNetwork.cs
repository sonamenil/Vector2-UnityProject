using Nekki.Social;

public interface ISocialNetwork
{
	bool NetworkHasGroups { get; }

	bool NetworkHasBookmarks { get; }

	SocialNetworks Network { get; }

	void Init(SocialWrapper wraper);

	void GetUser(string userId);

	void GetUsers(string[] userIds);

	void GetFriends();

	void InviteFriends();

	void GetBookmarkState();

	void ShowBookmarkBox();

	void CheckGroupMembership();

	void WallPost(string userId, string message, string content);

	void Buy(int item);

	void Buy(string item);
}
