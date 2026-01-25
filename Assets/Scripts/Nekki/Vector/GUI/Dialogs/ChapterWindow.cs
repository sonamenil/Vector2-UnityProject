using System;
using System.Collections;
using DG.Tweening;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.MainScene;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class ChapterWindow : BaseDialog
	{
		public enum HideBy
		{
			Time = 0,
			Click = 1,
			Manually = 2
		}

		public enum ActionAfterHide
		{
			Nothing = 0,
			UnpauseGame = 1,
			GoToEquip = 2,
			GoToCredits = 3
		}

		private const string _SoundAlias = "chapter";

		private HideBy _HideBy = HideBy.Click;

		private ActionAfterHide _ActionAfterHide;

		private ChapterWindowSettings _Settings;

		private Action[] _ImageActions;

		[SerializeField]
		private GameObject _TextGroup;

		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _Content;

		[SerializeField]
		private ResolutionImage _Image;

		private Sequence _FadeInSequence;

		private Sequence _FadeOutSequence;

		private bool _CanClick;

		private static ChapterWindow _Current;

		public void Init(string title, string content, HideBy hideBy, ActionAfterHide actionAfterHide, ChapterWindowSettings settings, Action additionalAction = null)
		{
			_Current = this;
			_HideBy = hideBy;
			_ActionAfterHide = actionAfterHide;
			Action action = ActionEnumToSystemAction(actionAfterHide);
			_Title.SetAlias(title);
			_Content.SetAlias(content);
			_ImageActions = new Action[1]
			{
				delegate
				{
					if (action != null)
					{
						action();
					}
					if (additionalAction != null)
					{
						additionalAction();
					}
				}
			};
			_Settings = settings;
			SetTweenerValues();
			LockGame();
			AudioManager.PauseMusic(true);
			if (_HideBy == HideBy.Time)
			{
				CoroutineManager.Current.StartRoutine(HideAfterDelay(_Settings.TitleDelayFloat + _Settings.ContentDelayFloat + _Settings.HideDelayFloat));
				AudioManager.PlaySound("chapter");
			}
		}

		public void Init(string title, string content, string hideBy, string actionAfterHide, ChapterWindowSettings settings, Action additionalAction = null)
		{
			Init(title, content, HideByStringToEnum(hideBy), ActionStringToEnum(actionAfterHide), settings, additionalAction);
		}

		private void SetTweenerValues()
		{
			_Image.Alpha = 0f;
			_FadeInSequence = DOTween.Sequence();
			_FadeInSequence.AppendCallback(delegate
			{
				_TextGroup.SetActive(true);
			});
			_FadeInSequence.Append(_Title.DOFade(0f, 0f));
			_FadeInSequence.Append(_Content.DOFade(0f, 0f));
			_FadeInSequence.Append(_Image.DOFade(1f, _Settings.DarknessFadeInFloat));
			_FadeInSequence.Append(_Title.DOFade(1f, _Settings.TitleFadeInFloat));
			_FadeInSequence.Append(_Content.DOFade(1f, _Settings.ContentFadeInFloat));
			_FadeInSequence.OnComplete(delegate
			{
				_CanClick = true;
				if (_HideBy == HideBy.Manually)
				{
					DialogNotificationManager.ShowNextInQueue();
				}
			});
			_FadeInSequence.Play();
		}

		private void HideLabels()
		{
			_FadeOutSequence = DOTween.Sequence();
			_FadeOutSequence.Append(_Content.DOFade(0f, _Settings.ContentFadeOutFloat));
			_FadeOutSequence.Append(_Title.DOFade(0f, _Settings.TitleFadeOutFloat));
			if (Manager.Scene == SceneKind.Run)
			{
				_FadeOutSequence.OnComplete(delegate
				{
					_ImageActions[0]();
					Manager.SceneKeyboardControllerEnabled = true;
					if (_ActionAfterHide != ActionAfterHide.GoToEquip && _ActionAfterHide != ActionAfterHide.GoToCredits)
					{
						AudioManager.PauseMusic(false);
					}
				});
			}
			else
			{
				_FadeOutSequence.Append(_Image.DOFade(0f, _Settings.DarknessFadeOutFloat));
				_FadeOutSequence.OnComplete(delegate
				{
					_ImageActions[0]();
					Dismiss();
					Manager.SceneKeyboardControllerEnabled = true;
					AudioManager.PauseMusic(false);
				});
			}
			_FadeOutSequence.Play();
		}

		private static ActionAfterHide ActionStringToEnum(string actionAfterHide)
		{
			switch (actionAfterHide)
			{
			case "Nothing":
				return ActionAfterHide.Nothing;
			case "UnpauseGame":
				return ActionAfterHide.UnpauseGame;
			case "GoToEquip":
				return ActionAfterHide.GoToEquip;
			case "GoToCredits":
				return ActionAfterHide.GoToCredits;
			default:
				return ActionAfterHide.Nothing;
			}
		}

		private static Action ActionEnumToSystemAction(ActionAfterHide actionAfterHide)
		{
			switch (actionAfterHide)
			{
			case ActionAfterHide.Nothing:
				return null;
			case ActionAfterHide.UnpauseGame:
				return UnpauseGameAction;
			case ActionAfterHide.GoToEquip:
				return GoToEquipAction;
			case ActionAfterHide.GoToCredits:
				return GoToCreditsAction;
			default:
				return null;
			}
		}

		private IEnumerator HideAfterDelay(float p_delay)
		{
			yield return new WaitForSeconds(p_delay);
			HideContent();
		}

		private static void UnpauseGameAction()
		{
			RunMainController.IsPause(false, true);
		}

		private static void GoToEquipAction()
		{
			if (Manager.Scene == SceneKind.Run)
			{
				RunMainController.RunEnd();
			}
			Manager.Load(SceneKind.Main);
		}

		private static void GoToCreditsAction()
		{
			if (Manager.Scene == SceneKind.Run)
			{
				RunMainController.RunEnd();
			}
			Manager.NeedOpenCredits = true;
			Manager.Load(SceneKind.Main);
		}

		public static bool NeedToHideManually(string hideBy)
		{
			return NeedToHideManually(HideByStringToEnum(hideBy));
		}

		public static bool NeedToHideManually(HideBy hideBy)
		{
			return hideBy == HideBy.Manually;
		}

		public static void HideContent()
		{
			if (_Current != null)
			{
				BottomPanel module = UIModule.GetModule<BottomPanel>();
				if (module != null)
				{
					module.UpdateBannerButton();
				}
				ZoneManager.LoadSettings();
				if (Manager.IsEquip)
				{
					Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.Refresh();
					DialogCanvasController.Current.ReinitiateBlur();
				}
				_Current.HideLabels();
				_Current = null;
			}
		}

		private static HideBy HideByStringToEnum(string hideBy)
		{
			switch (hideBy)
			{
			case "Time":
				return HideBy.Time;
			case "Click":
				return HideBy.Click;
			case "Manually":
				return HideBy.Manually;
			default:
				return HideBy.Click;
			}
		}

		private void LockGame()
		{
			if (Manager.Scene == SceneKind.Run)
			{
				RunMainController.Player.ControllerControl.Enable = false;
				RunMainController.CanPause = true;
				RunMainController.IsPause(true, true);
				Manager.SceneKeyboardControllerEnabled = false;
			}
		}

		private IEnumerator ShowLabels(float titleDelay, float contentDealy)
		{
			yield return new WaitForSeconds(titleDelay);
			_Title.gameObject.SetActive(true);
			yield return new WaitForSeconds(contentDealy);
			_Content.gameObject.SetActive(true);
		}

		public void OnClick()
		{
			if (_CanClick && _HideBy == HideBy.Click)
			{
				HideContent();
			}
		}

		public void OnKeyDown(KeyCode p_code)
		{
			if (p_code == KeyCode.Escape)
			{
				OnClick();
			}
		}
	}
}
