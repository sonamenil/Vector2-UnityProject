using System.Collections.Generic;
using Nekki.Vector.Core;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.MainScene;
using Nekki.Vector.GUI.Scenes;

namespace Nekki.Vector.GUI.Dialogs
{
	public static class DialogCallbacks
	{
		public enum DoYouLikeGameAnswer
		{
			Yes = 0,
			No = 1
		}

		public enum SetStarAnswer
		{
			Ok = 0,
			DontAskAgain = 1,
			Later = 2
		}

		public enum SupportAnswer
		{
			Yes = 0,
			No = 1
		}

		private const int _NextRateUsShowStep = 15;

		private static readonly List<string> _InitialBundleRequestsList = new List<string> { "zone2" };

		public static void DoYouLikeGame_ShowSetStar(BaseDialog p_dialog)
		{
			CounterController.Current.CounterDoYouLikeGameAnswerPt1 = 0;
			CounterController.Current.CounterShowDoYouLikeGameDialog = 0;
			DialogNotificationManager.ShowSetStarDialog(500);
			p_dialog.Dismiss();
		}

		public static void DoYouLikeGame_ShowSupport(BaseDialog p_dialog)
		{
			CounterController.Current.CounterDoYouLikeGameAnswerPt1 = 1;
			CounterController.Current.CounterShowDoYouLikeGameDialog = 0;
			CounterController.Current.CounterShowSetStarDialog = 0;
			DialogNotificationManager.ShowSupportDialog(500);
			p_dialog.Dismiss();
		}

		public static void SetStar_Ok(BaseDialog p_dialog)
		{
			CounterController.Current.CounterDoYouLikeGameAnswerPt2 = 0;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Do_you_like_game);
			ApplicationController.OpenURL(UrlManager.ApplicationUrl);
			CounterController.Current.CounterShowSetStarDialog = 0;
			p_dialog.Dismiss();
		}

		public static void SetStar_DontAskAgain(BaseDialog p_dialog)
		{
			CounterController.Current.CounterDoYouLikeGameAnswerPt2 = 1;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Do_you_like_game);
			CounterController.Current.CounterShowSetStarDialog = 0;
			p_dialog.Dismiss();
		}

		public static void SetStar_Later(BaseDialog p_dialog)
		{
			CounterController.Current.CounterDoYouLikeGameAnswerPt2 = 2;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Do_you_like_game);
			CounterController.Current.CounterShowSetStarDialog = (int)CounterController.Current.CounterRunCounter + 15;
			p_dialog.Dismiss();
		}

		public static void Support_Yes(BaseDialog p_dialog)
		{
			CounterController.Current.CounterDoYouLikeGameAnswerPt2 = 0;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Do_you_like_game);
			ApplicationController.OpenURL(UrlManager.SupportUrl);
			p_dialog.Dismiss();
		}

		public static void Support_No(BaseDialog p_dialog)
		{
			CounterController.Current.CounterDoYouLikeGameAnswerPt2 = 1;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Do_you_like_game);
			p_dialog.Dismiss();
		}

		public static void NotEnoughtCurrency_ShowPayment(BaseDialog p_dialog)
		{
			DialogNotificationManager.ShowPaymentDialog("Currency");
			p_dialog.Dismiss();
		}

		public static void Quit_Ok(BaseDialog p_dialog)
		{
			DialogNotificationManager.StopQueue();
			p_dialog.Dismiss();
			ApplicationController.Quit();
		}

		public static void Misc_Close(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
		}

		public static void Misc_CloseAndEnableParentDialog(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			if (DialogNotificationManager.ParentDialog != null)
			{
				DialogCanvasController.Current.TurnOnBlurEffect();
				DialogCanvasController.Current.BlockNotDialogTouches();
				DialogNotificationManager.ParentDialog.SetButtonsEnabled(true);
			}
		}

		public static void MissionTooltip_CloseAndGoToPaymentBoosterpacks(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			DialogCanvasController.Current.CloseDialog<MissionsDialog>();
			DialogNotificationManager.ShowPaymentDialog("Boosterpacks");
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Open_Payments_From_Mission_Card);
		}

		public static void CardsLevelUp_Run(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.Play();
		}

		public static void CardsLevelUp_Archive(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			int slotsWithLevelUpCards = DataLocalHelper.SlotsWithLevelUpCards;
			if (slotsWithLevelUpCards > 1)
			{
				Manager.OpenArchiveCategory();
			}
			else if (slotsWithLevelUpCards == 1)
			{
				Manager.OpenArchive(DataLocalHelper.GetFirstSlotWithLevelUpCards);
			}
		}

		public static void NoQuestProgress_Ok(BaseDialog p_dialog)
		{
			CounterController.Current.CounterShowNoQuestProgressDialog = -1;
			DataLocal.Current.Save(true);
			DialogNotificationManager.ShowPaymentDialog("Boosterpacks");
			p_dialog.Dismiss();
		}

		public static void NoQuestProgress_Cancel(BaseDialog p_dialog)
		{
			CounterController.Current.CounterShowNoQuestProgressDialog = -1;
			DataLocal.Current.Save(true);
			p_dialog.Dismiss();
		}

		public static void BoosterpackCanOpened_ShowBoosterpack(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			Manager.OpenBoosterpack();
		}

		public static void BoosterpackCanOpened_RunAnyway(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			if (DataLocalHelper.HasLevelUpCards)
			{
				DialogNotificationManager.ShowCardsCanLevelUpDialog(0);
			}
			else
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.Play();
			}
		}

		public static void InitialBundleRequest_Yes(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			foreach (string initialBundleRequests in _InitialBundleRequestsList)
			{
				BundleManager.CreateBundleRequestWithCheckingUpdate(initialBundleRequests, 0, false);
			}
			CounterController.Current.CounterInitialBundleRequest = 0;
			DataLocal.Current.Save(true);
			Scene<GameLoaderScene>.Current.CheckBundleRequests();
		}

		public static void InitialBundleRequest_No(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			CounterController.Current.CounterInitialBundleRequest = 0;
			DataLocal.Current.Save(true);
			Scene<GameLoaderScene>.Current.CheckBundleRequests();
		}

		public static void BundleRequest_Yes(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			DialogNotificationManager.ShowBundleDownloadDialog(BundleManager.RequestsTotalContentLengthInMb);
		}

		public static void BundleRequest_No(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			BundleDownloadDialogContent.SetBundleRequestsDone();
		}

		public static void BundleRequest_DontAskAgain(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			BundleManager.ResetRequests();
			BundleDownloadDialogContent.SetBundleRequestsDone();
		}

		public static void BundleDownloadRetry_Yes(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			if (DialogNotificationManager.ParentDialog != null)
			{
				DialogCanvasController.Current.TurnOnBlurEffect();
				DialogCanvasController.Current.BlockNotDialogTouches();
				DialogNotificationManager.ParentDialog.SetButtonsEnabled(true);
			}
			BundleDownloadDialogContent.Current.RunBundleRequest();
		}

		public static void BundleDownloadRetry_No(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			if (DialogNotificationManager.ParentDialog != null)
			{
				DialogCanvasController.Current.TurnOnBlurEffect();
				DialogCanvasController.Current.BlockNotDialogTouches();
				DialogNotificationManager.ParentDialog.SetButtonsEnabled(true);
			}
			BundleDownloadDialogContent.Current.Close();
		}

		public static void ValidationFail_Ok(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			ApplicationController.Quit();
		}
	}
}
