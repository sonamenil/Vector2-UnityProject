using Nekki.Vector.Core;
using Nekki.Vector.Core.ABTest;
using Nekki.Vector.Core.Offer;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using SimpleJSON;

public static class ServerResponseReceiver
{
	public static void UserResponse(bool p_result, string p_data, object p_userData)
	{
		if (p_result)
		{
			JSONNode jSONNode = JSON.Parse(p_data);
			if (jSONNode == null)
			{
				return;
			}
			if (jSONNode["data"] != null && jSONNode["data"].Value == "user")
			{
				JSONNode jSONNode2 = jSONNode["value"];
				int asInt = jSONNode2["user_id"].AsInt;
				bool logger = jSONNode2["should_log"].AsInt == 1;
				string newGroup = string.Empty;
				string hash = string.Empty;
				JSONArray asArray = jSONNode2["ab_group"].AsArray;
				if (asArray != null && asArray.Count > 0)
				{
					newGroup = asArray[0]["group"];
					hash = asArray[0]["hash"];
				}
				DataLocal.UserID = asInt;
				StatisticsCollector.Current.SetLogger(logger);
				ABTestManager.SetABGroup(newGroup, hash);
			}
		}
		Preloader.ServerResponse(true, false, false, false);
	}

	public static void OfferResponse(bool p_result, string p_data, object p_userData)
	{
		if (p_result)
		{
			JSONNode jSONNode = JSON.Parse(p_data);
			if (jSONNode == null)
			{
				return;
			}
			if (jSONNode["data"] != null && jSONNode["data"].Value == "offer")
			{
				OffersManager.CheckServerOffers(jSONNode["value"].AsArray);
			}
		}
		Preloader.ServerResponse(false, true, false, false);
	}
}
