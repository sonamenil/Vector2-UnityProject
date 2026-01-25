using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public static class DialogNotificationManager
	{
		private static DialogsQueue _DialogsQueue = new DialogsQueue();

		private static DialogsQueue _NotificationsQueue = new DialogsQueue();

		private static BaseDialog _ParentDialog;

		public static DialogsQueue DialogsQueue
		{
			get
			{
				return _DialogsQueue;
			}
		}

		public static DialogsQueue NotificationsQueue
		{
			get
			{
				return _NotificationsQueue;
			}
		}

		public static BaseDialog ParentDialog
		{
			get
			{
				return _ParentDialog;
			}
		}

		public static void StopQueue()
		{
			_DialogsQueue.IsProcessing = false;
			_NotificationsQueue.IsProcessing = false;
		}

		public static void ShowNextInQueue()
		{
			_DialogsQueue.ShowNext();
		}

		public static void ShowEndFloorDialog(Action p_OnClose, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowEndFloorDialog(p_OnClose);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowInfoDialog(List<DialogButtonData> p_buttons, string p_title, string p_text, int p_Order = 0, bool p_isIgnoringQueue = false)
		{
			if (p_isIgnoringQueue)
			{
				DialogCanvasController.Current.ShowInfoDialog(p_buttons, p_title, p_text);
				return;
			}
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowInfoDialog(p_buttons, p_title, p_text);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		private static void ShowInfoDialogInstant(List<DialogButtonData> p_buttons, string p_title, string p_text, bool p_moveToFront = false)
		{
			DialogCanvasController.Current.ShowInfoDialog(p_buttons, p_title, p_text, p_moveToFront);
		}

		public static void ShowOptionsDialog(int p_Order = 0)
		{
			Action p_data = delegate
			{
				ShowOptionsDialogInstant();
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowOptionsDialogInstant()
		{
			DialogCanvasController.Current.ShowOptionsDialog();
		}

		public static void ShowMissionsDialog(bool p_isWithContinueButton = true, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowMissionsDialog(p_isWithContinueButton);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowNotEnoughtCurrencyDialog(CurrencyType p_currencyType, Action<BaseDialog> p_onCloseAction = null, int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			if (p_currencyType == CurrencyType.Money1 || p_currencyType == CurrencyType.Money2)
			{
				buttons.Add(new DialogButtonData((p_onCloseAction == null) ? new Action<BaseDialog>(DialogCallbacks.Misc_Close) : p_onCloseAction, "^GUI.Buttons.Ok^", ButtonUI.Type.Blue));
			}
			else
			{
				buttons.Add(new DialogButtonData(DialogCallbacks.NotEnoughtCurrency_ShowPayment, "^GUI.Buttons.Store^", ButtonUI.Type.Blue));
				buttons.Add(new DialogButtonData((p_onCloseAction == null) ? new Action<BaseDialog>(DialogCallbacks.Misc_Close) : p_onCloseAction, "^GUI.Buttons.Cancel^", ButtonUI.Type.Red));
			}
			string title = "^GUI.Labels.DialogWindow.Title.Warning^";
			string content = string.Empty;
			switch (p_currencyType)
			{
			case CurrencyType.Money1:
				content = "^GUI.Labels.DialogWindow.Text.NoPoints^";
				break;
			case CurrencyType.Money2:
				content = "^GUI.Labels.DialogWindow.Text.NoMoney^";
				break;
			default:
				content = "^GUI.Labels.DialogWindow.Text.NoChips^";
				break;
			}
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, title, content);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowEnergyDialog(Action p_onCloseAfterRechargeAction = null, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowEnergyDialog(p_onCloseAfterRechargeAction);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowSaleDialog(int p_saleID, bool p_forceShow, int p_Order = 0)
		{
			if (!SaleDialogContent.IsSaleShowed || p_forceShow)
			{
				Action p_data = delegate
				{
					DialogCanvasController.Current.ShowSaleDialog(p_saleID);
				};
				_DialogsQueue.AddToQueue(p_Order, p_data);
			}
		}

		public static void ShowNewsDialog(string imagePath, Action<BaseDialog> imageAction, List<DialogButtonData> buttonsInfo, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowNewsDialog(imagePath, imageAction, buttonsInfo);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowChapterWindow(string title, string text, ChapterWindow.HideBy hideBy, ChapterWindow.ActionAfterHide actionAfterHide, ChapterWindowSettings settings, Action additionalAction = null, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowChapterWindow(title, text, hideBy, actionAfterHide, settings, additionalAction);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowChapterWindow(string title, string text, string hideBy, string actionAfterHide, ChapterWindowSettings settings, Action additionalAction = null, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowChapterWindow(title, text, hideBy, actionAfterHide, settings, additionalAction);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowPromoEffectDialog(int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowPromoEffectDialog();
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowPaymentDialog(string p_selectedGroup = "Promo", int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowPaymentDialog(p_selectedGroup);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowSelectCardDialog(GadgetItem p_item, CardsGroupAttribute p_card, Action<bool> p_answer, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowSelectCardDialog(p_item, p_card, p_answer);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowDoYouLikeGameDialog(int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.DoYouLikeGame_ShowSetStar, "^GUI.Buttons.BigYes^", ButtonUI.Type.Green));
			buttons.Add(new DialogButtonData(DialogCallbacks.DoYouLikeGame_ShowSupport, "^GUI.Buttons.No^", ButtonUI.Type.Red));
			string title = "^GUI.Labels.DialogWindow.Title.HelpImproveGame^";
			string content = "^GUI.Labels.DialogWindow.Text.DoYouLikeGame^";
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, title, content);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowSetStarDialog(int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.SetStar_Ok, "^GUI.Buttons.Ok^", ButtonUI.Type.Green));
			buttons.Add(new DialogButtonData(DialogCallbacks.SetStar_DontAskAgain, "^GUI.Buttons.DontAskAgain^", ButtonUI.Type.Red));
			buttons.Add(new DialogButtonData(DialogCallbacks.SetStar_Later, "^GUI.Buttons.BigLater^", ButtonUI.Type.Blue));
			string title = "^GUI.Labels.DialogWindow.Title.RateGame^";
			string content = "^GUI.Labels.DialogWindow.Text.RateGame^";
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, title, content);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowSupportDialog(int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.Support_Yes, "^GUI.Buttons.Support^", ButtonUI.Type.Green));
			buttons.Add(new DialogButtonData(DialogCallbacks.Support_No, "^GUI.Buttons.Cancel^", ButtonUI.Type.Red));
			string title = "^GUI.Labels.DialogWindow.Title.HelpImproveGame^";
			string content = "^GUI.Labels.DialogWindow.Text.HelpImproveGame^";
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, title, content);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowSaveMeDialog(string p_saveMePrice, Action<BaseDialog> p_okButtonAction, Action<BaseDialog> p_cancelButtonAction, Action<BaseDialog> p_adsButtonAction = null, int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			if (CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.Saveme))
			{
				buttons.Add(new DialogButtonData(p_okButtonAction, CouponsManager.GetCouponButtonText(CouponsManager.CouponType.Saveme), string.Empty, CouponsManager.GetCouponButtonIcon(CouponsManager.CouponType.Saveme), CouponsManager.GetCouponButtonIconColor(CouponsManager.CouponType.Saveme), ButtonUI.Type.Blue));
			}
			else
			{
				buttons.Add(new DialogButtonData(p_okButtonAction, "^GUI.Buttons.Reanimate^", p_saveMePrice, CurrencyInfo.GetCurrencySprite(CurrencyType.Money3), CurrencyInfo.GetCurrencyColor(CurrencyType.Money3), ButtonUI.Type.Blue));
			}
			if (p_adsButtonAction != null)
			{
				buttons.Add(new DialogButtonData(p_adsButtonAction, "^GUI.Buttons.WatchRV^", ButtonUI.Type.Blue));
			}
			buttons.Add(new DialogButtonData(p_cancelButtonAction, "^GUI.Buttons.Cancel^", ButtonUI.Type.Red));
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.SaveMe^", "^GUI.Labels.DialogWindow.Text.SaveMe^");
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowConfirmationDialog(Action<BaseDialog> p_okButtonAction, Action<BaseDialog> p_cancelButtonAction = null, int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(p_okButtonAction, "^GUI.Buttons.Ok^", ButtonUI.Type.Blue));
			buttons.Add(new DialogButtonData((p_cancelButtonAction == null) ? new Action<BaseDialog>(DialogCallbacks.Misc_Close) : p_cancelButtonAction, "^GUI.Buttons.Cancel^", ButtonUI.Type.Red));
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.Confirm^", "^GUI.Labels.DialogWindow.Text.BuyQuestion^");
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowBoostConfirmationDialog(Action<BaseDialog> p_okButtonAction, Action<BaseDialog> p_cancelButtonAction = null, int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(p_okButtonAction, "^GUI.Buttons.Ok^", ButtonUI.Type.Blue));
			buttons[0]._SoundAlias = "boost_button";
			buttons.Add(new DialogButtonData((p_cancelButtonAction == null) ? new Action<BaseDialog>(DialogCallbacks.Misc_Close) : p_cancelButtonAction, "^GUI.Buttons.Cancel^", ButtonUI.Type.Red));
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.Confirm^", "^GUI.Labels.DialogWindow.Text.BoostQuestion^");
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowSimpleNotification(Notification.Parameters p_parameters, Action p_callback = null, int p_order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowSimpleNotification(p_parameters);
			};
			if (p_parameters.QueueType == DialogQueueType.Dialog)
			{
				_DialogsQueue.AddToQueue(p_order, p_data);
			}
			else
			{
				_NotificationsQueue.AddToQueue(p_order, p_data);
			}
		}

		public static void ShowMissionNotification(Notification.Parameters p_parameters, Action p_callback = null, int p_order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowMissionNotification(p_parameters);
			};
			if (p_parameters.QueueType == DialogQueueType.Dialog)
			{
				_DialogsQueue.AddToQueue(p_order, p_data);
			}
			else
			{
				_NotificationsQueue.AddToQueue(p_order, p_data);
			}
		}

		public static void ShowTooltip(Tooltip.UISettings p_uiSettings, int p_order = 0)
		{
			if (Tooltip.CheckTooltipDouplicate(p_uiSettings))
			{
				Action p_data = delegate
				{
					DialogCanvasController.Current.ShowTooltip(p_uiSettings);
				};
				if (p_uiSettings.QueueType == DialogQueueType.Dialog)
				{
					_DialogsQueue.AddToQueue(p_order, p_data);
				}
				else
				{
					_NotificationsQueue.AddToQueue(p_order, p_data);
				}
			}
		}

		public static void ShowQuestStartDialog(string p_title, string p_objectiveText, Quest p_quest, Action p_onClose, int p_Order = 0, bool p_noDetails = false)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowQuestStartDialog(p_title, p_objectiveText, p_quest, p_onClose, p_noDetails);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowQuestCompleteDialog(string p_title, Quest p_quest, Action p_onClose, int p_Order = 0)
		{
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowQuestCompleteDialog(p_title, p_quest, p_onClose);
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowQuestTalkingDialog(string p_title, string p_text, string p_buttonText, string p_image, Action p_onClose, int p_order = 0, bool p_isIgnoringQueue = false)
		{
			if (p_isIgnoringQueue)
			{
				DialogCanvasController.Current.ShowQuestTalkingDialog(p_title, p_text, p_buttonText, p_image, p_onClose);
				return;
			}
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowQuestTalkingDialog(p_title, p_text, p_buttonText, p_image, p_onClose);
			};
			_DialogsQueue.AddToQueue(p_order, p_data);
		}

		public static void ShowQuestTalkingDialog(string p_title, string p_text, string p_image, List<DialogButtonData> p_buttons, int p_order = 0, bool p_isIgnoringQueue = false)
		{
			if (p_isIgnoringQueue)
			{
				DialogCanvasController.Current.ShowQuestTalkingDialog(p_title, p_text, p_image, p_buttons);
				return;
			}
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowQuestTalkingDialog(p_title, p_text, p_image, p_buttons);
			};
			_DialogsQueue.AddToQueue(p_order, p_data);
		}

		public static void ShowInsertUpgradeTalkingDialog(Action p_onClose, int p_order = 0)
		{
			ShowQuestTalkingDialog("^Quests.InsertUpgrade.Dialog2.Title^", "^Quests.InsertUpgrade.Dialog2.Message^", "^Quests.InsertUpgrade.Dialog2.Button^", "AI_portrait", p_onClose, p_order);
		}

		public static void ShowQuitDialog(int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.Quit_Ok, "^GUI.Buttons.BigYes^", ButtonUI.Type.Green));
			buttons.Add(new DialogButtonData(DialogCallbacks.Misc_Close, "^GUI.Buttons.No^", ButtonUI.Type.Red));
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.Confirm^", "^GUI.Labels.DialogWindow.Text.ReallyQuit^");
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowCardsCanLevelUpDialog(int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.CardsLevelUp_Archive, "^GUI.Buttons.ShowArchive^", ButtonUI.Type.Green));
			buttons.Add(new DialogButtonData(DialogCallbacks.CardsLevelUp_Run, "^GUI.Buttons.RunAnyway^", ButtonUI.Type.Red));
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.Warning^", "^GUI.Labels.DialogWindow.Text.CardsNotLeveledUp^");
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowBoosterpackCanOpenedDialog(int p_Order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.BoosterpackCanOpened_ShowBoosterpack, "^GUI.Buttons.ShowBoosterpacks^", ButtonUI.Type.Green));
			if ((int)CounterController.Current.CounterForcedOpenBoosterpacks == 0)
			{
				buttons.Add(new DialogButtonData(DialogCallbacks.BoosterpackCanOpened_RunAnyway, "^GUI.Buttons.RunAnyway^", ButtonUI.Type.Red));
			}
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.Warning^", "^GUI.Labels.DialogWindow.Text.BoosterpackCanOpened^");
			};
			_DialogsQueue.AddToQueue(p_Order, p_data);
		}

		public static void ShowNoQuestProgressDialog(int p_order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.NoQuestProgress_Ok, "^GUI.Buttons.Show^", ButtonUI.Type.Green));
			buttons.Add(new DialogButtonData(DialogCallbacks.NoQuestProgress_Cancel, "^GUI.Buttons.Cancel^", ButtonUI.Type.Red));
			Action p_data = delegate
			{
				CounterController.Current.CounterShowNoQuestProgressDialog = Mathf.FloorToInt((float)(int)CounterController.Current.CounterShowNoQuestProgressDialog * 0.5f);
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.NoQuestProgress^", "^GUI.Labels.DialogWindow.Text.NoQuestProgress^");
			};
			_DialogsQueue.AddToQueue(p_order, p_data);
		}

		public static void ShowInitialBundleRequestDialog(int p_order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.InitialBundleRequest_Yes, "^GUI.Buttons.Yes^", ButtonUI.Type.Green));
			buttons.Add(new DialogButtonData(DialogCallbacks.InitialBundleRequest_No, "^GUI.Buttons.No^", ButtonUI.Type.Red));
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.BundleAvailable^", "^GUI.Labels.DialogWindow.Text.InitialBundleRequest^");
			};
			_DialogsQueue.AddToQueue(p_order, p_data);
		}

		public static void ShowBundleRequestDialog(bool p_isRequiredUpdateAvalible, float p_totalContentLength, int p_order = 0)
		{
			StringBuffer.AddString("TotalContentLength", string.Format("{0} Mb", (float)(int)(p_totalContentLength * 100f) / 100f));
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			if (p_isRequiredUpdateAvalible)
			{
				buttons.Add(new DialogButtonData(DialogCallbacks.BundleRequest_Yes, "^GUI.Buttons.Download^", ButtonUI.Type.Green));
			}
			else
			{
				buttons.Add(new DialogButtonData(DialogCallbacks.BundleRequest_Yes, "^GUI.Buttons.Yes^", ButtonUI.Type.Green));
				buttons.Add(new DialogButtonData(DialogCallbacks.BundleRequest_No, "^GUI.Buttons.No^", ButtonUI.Type.Red));
				buttons.Add(new DialogButtonData(DialogCallbacks.BundleRequest_DontAskAgain, "^GUI.Buttons.DontAskAgain^", ButtonUI.Type.Blue));
			}
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, (!p_isRequiredUpdateAvalible) ? "^GUI.Labels.DialogWindow.Title.BundleAvailable^" : "^GUI.Labels.DialogWindow.Title.BundleIsRequired^", (!p_isRequiredUpdateAvalible) ? "^GUI.Labels.DialogWindow.Text.BundleRequest^" : "^GUI.Labels.DialogWindow.Text.BundleRequestRequired^");
			};
			_DialogsQueue.AddToQueue(p_order, p_data);
		}

		public static void ShowBundleDownloadDialog(float p_totalContentLength, int p_order = 0)
		{
			StringBuffer.AddString("TotalContentLength", string.Format("{0} Mb", (float)(int)(p_totalContentLength * 100f) / 100f));
			Action p_data = delegate
			{
				DialogCanvasController.Current.ShowBundleDownloadDialog();
			};
			_DialogsQueue.AddToQueue(p_order, p_data);
		}

		public static void ShowValidationFailDialog(string p_content, int p_order = 0)
		{
			List<DialogButtonData> buttons = new List<DialogButtonData>();
			buttons.Add(new DialogButtonData(DialogCallbacks.ValidationFail_Ok, "^GUI.Buttons.Ok^", ButtonUI.Type.Green));
			Action p_data = delegate
			{
				ShowInfoDialogInstant(buttons, "^GUI.Labels.DialogWindow.Title.Warning^", p_content);
			};
			_DialogsQueue.AddToQueue(p_order, p_data);
		}

		public static void ShowBundleDownloadRetryDialog(BaseDialog p_parentDialog, bool p_isRequiredUpdateAvalible)
		{
			_ParentDialog = p_parentDialog;
			_ParentDialog.SetButtonsEnabled(false);
			List<DialogButtonData> list = new List<DialogButtonData>();
			if (p_isRequiredUpdateAvalible)
			{
				list.Add(new DialogButtonData(DialogCallbacks.BundleDownloadRetry_Yes, "^GUI.Buttons.Ok^", ButtonUI.Type.Green));
			}
			else
			{
				list.Add(new DialogButtonData(DialogCallbacks.BundleDownloadRetry_Yes, "^GUI.Buttons.Yes^", ButtonUI.Type.Green));
				list.Add(new DialogButtonData(DialogCallbacks.BundleDownloadRetry_No, "^GUI.Buttons.No^", ButtonUI.Type.Red));
			}
			ShowInfoDialogInstant(list, "^GUI.Labels.DialogWindow.Title.ConnectionFailed^", (!p_isRequiredUpdateAvalible) ? "^GUI.Labels.DialogWindow.Text.BundleDownloadRetry^" : "^GUI.Labels.DialogWindow.Text.BundleDownloadRetryRequired^", true);
		}

		public static void ShowQualityChangeDialog(BaseDialog p_parentDialog)
		{
			_ParentDialog = p_parentDialog;
			_ParentDialog.SetButtonsEnabled(false);
			List<DialogButtonData> list = new List<DialogButtonData>();
			list.Add(new DialogButtonData(DialogCallbacks.Misc_CloseAndEnableParentDialog, "^GUI.Buttons.Ok^", ButtonUI.Type.Green));
			ShowInfoDialogInstant(list, "^GUI.Labels.DialogWindow.Title.Warning^", "^GUI.Labels.DialogWindow.Text.Restart^", true);
		}

		public static T PreloadDialog<T>() where T : BaseDialog
		{
			return DialogCanvasController.Current.GetOrCreateDialog<T>();
		}

		public static List<Button> GetDialogButtons(string p_dialogTypeName, string p_buttonName)
		{
			BaseDialog orCreateDialog = DialogCanvasController.Current.GetOrCreateDialog(p_dialogTypeName);
			if (orCreateDialog == null)
			{
				return null;
			}
			return orCreateDialog.GetButtonsByName(p_buttonName);
		}
	}
}
