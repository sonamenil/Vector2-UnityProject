using Nekki.Social;

public class Social
{
	private static ISocialNetwork _network;

	private static SocialWrapper _wrap;

	public static UserInfo CurrentUser
	{
		get
		{
			return SocialWrapper.CurrentUser;
		}
	}

	public static ISocialNetwork Network
	{
		get
		{
			return _network;
		}
	}

	public static void Init(Callbacks callbacks)
	{
		_wrap = SocialWrapper.Init(callbacks, OnSocialInit);
	}

	private static void OnSocialInit(SocialNetworks networks)
	{
		if (networks != 0 && networks == SocialNetworks.VKontakte)
		{
			_network = new VK();
			_network.Init(_wrap);
		}
	}
}
