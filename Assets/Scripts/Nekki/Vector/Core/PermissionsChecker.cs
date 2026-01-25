using System;
using Nekki.Vector.Core.Localization;

namespace Nekki.Vector.Core
{
	public static class PermissionsChecker
	{
		public class DialogInfo
		{
			private string _Title;

			private string _Text;

			private string _ButtonOk;

			private string _ButtonCancel;

			private string _ButtonSettings;

			public string Title
			{
				get
				{
					return LocalizationManager.GetPhrase(_Title);
				}
				set
				{
					_Title = value;
				}
			}

			public string Text
			{
				get
				{
					return LocalizationManager.GetPhrase(_Text);
				}
				set
				{
					_Text = value;
				}
			}

			public string ButtonOk
			{
				get
				{
					return LocalizationManager.GetPhrase(_ButtonOk);
				}
				set
				{
					_ButtonOk = value;
				}
			}

			public string ButtonCancel
			{
				get
				{
					return LocalizationManager.GetPhrase(_ButtonCancel);
				}
				set
				{
					_ButtonCancel = value;
				}
			}

			public string ButtonSettings
			{
				get
				{
					return LocalizationManager.GetPhrase(_ButtonSettings);
				}
				set
				{
					_ButtonSettings = value;
				}
			}

			public DialogInfo(string p_title = "Permishen.Title", string p_text = "Permishen.TextPhoneStateAds", string p_buttonOk = "GUI.Buttons.Ok", string p_buttonCancel = "GUI.Buttons.Cancel", string p_buttonSettings = "GUI.Labels.Options.Settings")
			{
				Title = p_title;
				Text = p_text;
				ButtonOk = p_buttonOk;
				ButtonCancel = p_buttonCancel;
				ButtonSettings = p_buttonSettings;
			}
		}

		public const string PERMISHEN_READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";

		private static Action<string> _OnGranded;

		private static Action<string> _OnDenied;

		private static Action<string> _OnUserSkip;

		public static bool CheckPermission(string p_permission, Action<string> p_onGranded, Action<string> p_onDenied, Action<string> p_userSkip)
		{
			return CheckPermission(p_permission, new DialogInfo("Permishen.Title", "Permishen.TextPhoneStateAds", "GUI.Buttons.Ok", "GUI.Buttons.Cancel", "GUI.Labels.Options.Settings"), p_onGranded, p_onDenied, p_userSkip);
		}

		public static bool CheckPermission(string p_permission, DialogInfo p_dialogInfo, Action<string> p_onGranded, Action<string> p_onDenied, Action<string> p_userSkip)
		{
			if (!PermissionsManager.Current.CheckPermission(p_permission))
			{
				AddPermissionActions(p_onGranded, p_onDenied, p_userSkip);
				PermissionsManager.Current.RequestPermission(p_permission, p_dialogInfo.Title, p_dialogInfo.Text, p_dialogInfo.ButtonOk, p_dialogInfo.ButtonCancel, p_dialogInfo.ButtonSettings);
				return false;
			}
			return true;
		}

		public static bool ShowExplanationWithOpenSettings(string p_permission, Action<string> p_onGranded, Action<string> p_onDenied, Action<string> p_userSkip)
		{
			return ShowExplanationWithOpenSettings(p_permission, new DialogInfo("Permishen.Title", "Permishen.TextPhoneStateAds", "GUI.Buttons.Ok", "GUI.Buttons.Cancel", "GUI.Labels.Options.Settings"), p_onGranded, p_onDenied, p_userSkip);
		}

		public static bool ShowExplanationWithOpenSettings(string p_permission, DialogInfo p_dialogInfo, Action<string> p_onGranded, Action<string> p_onDenied, Action<string> p_userSkip)
		{
			if (!PermissionsManager.Current.CheckPermission(p_permission) && !PermissionsManager.Current.IsShouldShowRequestPermissionRationale(p_permission))
			{
				AddPermissionActions(p_onGranded, p_onDenied, p_userSkip);
				PermissionsManager.Current.ShowExplanationWithOpenSettings(p_permission, p_dialogInfo.Title, p_dialogInfo.Text, p_dialogInfo.ButtonSettings);
				return false;
			}
			return true;
		}

		private static void Granded(string p_permishen)
		{
			if (_OnGranded != null)
			{
				_OnGranded(p_permishen);
			}
			RemovePermissionActions();
		}

		private static void Denied(string p_permishen)
		{
			if (_OnDenied != null)
			{
				_OnDenied(p_permishen);
			}
			RemovePermissionActions();
		}

		private static void UserSkip(string p_permishen)
		{
			if (_OnUserSkip != null)
			{
				_OnUserSkip(p_permishen);
			}
			RemovePermissionActions();
		}

		private static void AddPermissionActions(Action<string> p_onGranded, Action<string> p_onDenied, Action<string> p_userSkip)
		{
			_OnGranded = p_onGranded;
			_OnDenied = p_onDenied;
			_OnUserSkip = p_userSkip;
			PermissionsManager current = PermissionsManager.Current;
			current.OnGrantedCallBack = (Action<string>)Delegate.Combine(current.OnGrantedCallBack, new Action<string>(Granded));
			PermissionsManager current2 = PermissionsManager.Current;
			current2.OnDeniedCallBack = (Action<string>)Delegate.Combine(current2.OnDeniedCallBack, new Action<string>(Denied));
			PermissionsManager current3 = PermissionsManager.Current;
			current3.OnUserSkipCallBack = (Action<string>)Delegate.Combine(current3.OnUserSkipCallBack, new Action<string>(UserSkip));
		}

		private static void RemovePermissionActions()
		{
			PermissionsManager current = PermissionsManager.Current;
			current.OnGrantedCallBack = (Action<string>)Delegate.Remove(current.OnGrantedCallBack, new Action<string>(Granded));
			PermissionsManager current2 = PermissionsManager.Current;
			current2.OnDeniedCallBack = (Action<string>)Delegate.Remove(current2.OnDeniedCallBack, new Action<string>(Denied));
			PermissionsManager current3 = PermissionsManager.Current;
			current3.OnUserSkipCallBack = (Action<string>)Delegate.Remove(current3.OnUserSkipCallBack, new Action<string>(UserSkip));
			_OnGranded = null;
			_OnDenied = null;
			_OnUserSkip = null;
		}
	}
}
