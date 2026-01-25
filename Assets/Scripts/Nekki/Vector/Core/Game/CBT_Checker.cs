using Nekki.Vector.Core.Localization;
using SimpleJSON;

namespace Nekki.Vector.Core.Game
{
	public static class CBT_Checker
	{
		public static void Check()
		{
			if (ApplicationController.IsCBTBuild)
			{
				string userEmail = GoogleAccountGeter.getUserEmail();
				if (userEmail == null)
				{
					CBT_AccessResponce(true, "{\"data\":\"ok\", \"access\":0}", null);
				}
				else
				{
					ServerProvider.Instance.CBT_AccessAction(userEmail, CBT_AccessResponce);
				}
			}
		}

		public static void CBT_AccessResponce(bool p_result, string p_data, object p_userData)
		{
			if (p_result)
			{
				JSONNode jSONNode = JSON.Parse(p_data);
				if (jSONNode["data"].Value != "ok")
				{
					string phrase = LocalizationManager.GetPhrase("GUI.Buttons.Ok");
					DialogManager.Instance.SetLabel(phrase, phrase, phrase);
					string phrase2 = LocalizationManager.GetPhrase("GUI.Labels.DialogWindow.Title.Warning");
					string phrase3 = LocalizationManager.GetPhrase("CBT.NoInternet");
					DialogManager.Instance.ShowSubmitDialog(phrase2, phrase3, delegate
					{
						ApplicationController.Quit();
					});
				}
				else if (jSONNode["access"].AsInt != 1)
				{
					GoogleAnalyticsV4.getInstance().LogEvent("Game", "Application", "Access_Fail", 1L);
					string phrase4 = LocalizationManager.GetPhrase("GUI.Buttons.Ok");
					DialogManager.Instance.SetLabel(phrase4, phrase4, phrase4);
					string phrase5 = LocalizationManager.GetPhrase("GUI.Labels.DialogWindow.Title.Warning");
					string phrase6 = LocalizationManager.GetPhrase("CBT.NoAccess");
					DialogManager.Instance.ShowSubmitDialog(phrase5, phrase6, delegate
					{
						ApplicationController.Quit();
					});
				}
			}
			else
			{
				string phrase7 = LocalizationManager.GetPhrase("GUI.Buttons.Ok");
				DialogManager.Instance.SetLabel(phrase7, phrase7, phrase7);
				string phrase8 = LocalizationManager.GetPhrase("GUI.Labels.DialogWindow.Title.Warning");
				string phrase9 = LocalizationManager.GetPhrase("CBT.NoInternet");
				DialogManager.Instance.ShowSubmitDialog(phrase8, phrase9, delegate
				{
					ApplicationController.Quit();
				});
			}
		}
	}
}
