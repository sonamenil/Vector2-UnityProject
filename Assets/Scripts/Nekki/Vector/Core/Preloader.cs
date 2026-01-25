using System;
using System.Collections;
using Nekki.Core;
using Nekki.Vector.Core.ABTest;
using Nekki.Vector.Core.Advertising;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Console;
using Nekki.Vector.Core.ContentUpdater;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.DataValidation;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameCenter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.News;
using Nekki.Vector.Core.Notifications;
using Nekki.Vector.Core.Offer;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes;
using UnityEngine;

namespace Nekki.Vector.Core
{
	public class Preloader : GameInit
	{
		private static Preloader _Instance;

		private static bool _IsInited;

		private static bool _UserComplete;

		private static bool _OfferComplete;

		private static bool _ContentFileDownload;

		private static bool _BundlesFileDownload;

		public static Preloader Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new Preloader();
				}
				return _Instance;
			}
		}

		public static bool IsInited
		{
			get
			{
				return _IsInited;
			}
		}

		public override void Init(params System.Action[] actions)
		{
			if (!_IsInited)
			{
				Subscribe(actions);
				base.Initialize(false);
				Application.targetFrameRate = 60;
				PreInit();
				StartServerActions();
			}
		}

		private static void PreInit()
		{
			CUDLRConsole.Init();
			ChitProtector.InitObscuredTypes();
			ApplicationController.Init();
			VectorPaths.Init();
			DataLocalWiper.TryWipe();
			DeviceDetector.Init();
			Settings.Init();
			OffersManager.Init();
		}

		private static void StartServerActions()
		{
			ExternalGamedataValidator.CheckExternalGamedata();
			ResourcesValidator.PrepareValidation();
			if (!InternetUtils.IsInternetAvailable)
			{
				ServerResponse(true, true, true, true);
				return;
			}
			if (Settings.IsServerOn)
			{
				ServerProvider.Instance.UserAction();
				ServerProvider.Instance.OfferAction();
				Nekki.Vector.Core.ContentUpdater.ContentUpdater.LoadConfigFile(0);
			}
			else
			{
				ServerResponse(true, true, true, false);
			}
			BundleUpdater.LoadBundlesListFile(0);
		}

		private static void FinishServerAcions()
		{
			NewsManager.Init();
			Config.Init(true);
			OffersManager.DeactivateEndedOffers();
			LocalizationManager.Init();
			ResolutionManager.Init();
			AudioManager.Init();
			if (BundleUpdater.IsInited)
			{
				BundleManager.CheckBundlesUpdateAndValid();
			}
			else
			{
				BundleManager.CheckBundlesValid();
			}
		}

		public static void ServerResponse(bool p_userComplete, bool p_offerComplete, bool p_contentFileDownload, bool p_bundlesFileDownload)
		{
			if (p_userComplete)
			{
				_UserComplete = true;
			}
			if (p_offerComplete)
			{
				_OfferComplete = true;
			}
			if (p_contentFileDownload)
			{
				_ContentFileDownload = true;
			}
			if (p_bundlesFileDownload)
			{
				_BundlesFileDownload = true;
			}
			if (_UserComplete && _OfferComplete && _ContentFileDownload && _BundlesFileDownload)
			{
				ApplicationController.Current.StartCoroutine(RunUpdate());
			}
		}

		public static IEnumerator RunUpdate()
		{
			int total = 0;
			int iterations = 0;
			if (Nekki.Vector.Core.ContentUpdater.ContentUpdater.IsUpdateAvalible)
			{
				total++;
			}
			if (ABTestManager.IsUpdateAvalible)
			{
				total++;
			}
			if (OffersManager.IsUpdateAvalible)
			{
				total++;
			}
			Scene<GameLoaderScene>.Current.RefreshLoadingLabel(iterations, total);
			yield return new WaitForEndOfFrame();
			if (Nekki.Vector.Core.ContentUpdater.ContentUpdater.IsUpdateAvalible)
			{
				Scene<GameLoaderScene>.Current.RefreshLoadingLabel(iterations++, total);
				Nekki.Vector.Core.ContentUpdater.ContentUpdater.RunUpdate();
			}
			yield return new WaitForEndOfFrame();
			if (ABTestManager.IsUpdateAvalible)
			{
				Scene<GameLoaderScene>.Current.RefreshLoadingLabel(iterations++, total);
				ABTestManager.RunUpdate();
			}
			yield return new WaitForEndOfFrame();
			if (OffersManager.IsUpdateAvalible)
			{
				Scene<GameLoaderScene>.Current.RefreshLoadingLabel(iterations++, total);
				OffersManager.RunUpdate();
			}
			OffersManager.ShowNewsForActivatedOffers();
			yield return new WaitForEndOfFrame();
			Scene<GameLoaderScene>.Current.RefreshLoadingLabel(iterations, total);
			DataLocal.InitTabu = false;
			ResourcesValidator.RunValidation();
			yield return new WaitForEndOfFrame();
			FinishServerAcions();
			Scene<GameLoaderScene>.Current.PreloaderServerActionsDone();
		}

		public void PostInit()
		{
			GameManager.Init();
			LayersController.Init();
			AnimationLoader.Current.Init();
			GameRestorer.TryRestore();
			if (!GameRestorer.Active)
			{
				CounterController.Current.CounterFloor = 0;
			}
			AchievementsManager.Init();
			ConsoleCommands.Init();
			QuestManager.Init();
			PaymentController.Init();
			ProductManager.Init();
			GameCenterController.Init();
			RemoteNotifications.Init();
			VectorADSystem.Init();
			EnergyManager.Init();
			CBT_Checker.Check();
			PaymentController.RestoreStatisticsData();
			if (ResourcesValidator.ApplyValidationResult())
			{
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Session_start);
				_IsInited = true;
				Scene<GameLoaderScene>.Current.PreloaderWorkDone();
				LocalNotificationManager.Current.CancelAllNotifications();
			}
		}
	}
}
