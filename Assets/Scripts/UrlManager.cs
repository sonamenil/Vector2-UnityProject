using Nekki.Vector.Core;
using Nekki.Vector.Core.GameManagement;

public static class UrlManager
{
	public static string ApplicationUrl
	{
		get
		{
			return string.Format("market://details?id={0}", ApplicationController.BundleId);
		}
	}

	public static string BannerSF2Url
	{
		get
		{
			return GetUrl("BannerSF2");
		}
	}

	public static string BannerSF3Url
	{
		get
		{
			return GetUrl("BannerSF3");
		}
	}

	public static string FAQUrl
	{
		get
		{
			return GetUrl("FAQ");
		}
	}

	public static string OSTUrl
	{
		get
		{
			return GetUrl("OST");
		}
	}

	public static string SupportUrl
	{
		get
		{
			return GetUrl("Support");
		}
	}

	public static string GetUrl(string p_key)
	{
		return BalanceManager.Current.GetBalance("Url", p_key, "Android");
	}
}
