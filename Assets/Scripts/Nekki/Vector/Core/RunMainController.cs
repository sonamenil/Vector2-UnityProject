using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Advertising;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.PassiveEffects;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core
{
	public static class RunMainController
	{
		public delegate void EndDelegate(float Time);

		public delegate void OnPauseDelegat(bool p_pause);

		public delegate void OnSimuliteTimeDelegat(float p_time);

		public static string RoomProperties;

		public static int CurrentFloor;

		private static Scene _Scene;

		private static bool _IsWin;

		private static bool _IsDeath;

		private static bool _CanPause = true;

		private static bool _IsPaused;

		private static bool _IsDebugPaused;

		private static bool _IsRunNow;

		private static double _RunStartTime;

		private static double _RunEndTime;

		public static Scene Scene
		{
			get
			{
				return _Scene;
			}
		}

		public static Location Location
		{
			get
			{
				return _Scene.Location;
			}
		}

		public static List<ModelHuman> Models
		{
			get
			{
				return _Scene.Models;
			}
		}

		public static ModelHuman Player
		{
			get
			{
				return _Scene.Player;
			}
		}

		public static bool IsWin
		{
			get
			{
				return _IsWin;
			}
		}

		public static bool IsDeath
		{
			get
			{
				return _IsDeath;
			}
		}

		public static bool CanPause
		{
			get
			{
				return _CanPause;
			}
			set
			{
				_CanPause = value;
			}
		}

		public static bool IsPaused
		{
			get
			{
				return _IsPaused;
			}
		}

		public static bool IsDebugPaused
		{
			get
			{
				return _IsDebugPaused;
			}
			set
			{
				_IsDebugPaused = value;
			}
		}

		public static bool IsRunNow
		{
			get
			{
				return _IsRunNow;
			}
			set
			{
				_IsRunNow = value;
			}
		}

		public static double RunStartTime
		{
			get
			{
				return _RunStartTime;
			}
		}

		public static double RunEndTime
		{
			get
			{
				return _RunEndTime;
			}
		}

		public static event EndDelegate OnWin;

		public static event EndDelegate OnLoss;

		public static event EndDelegate OnWaitingResurrection;

		public static event EndDelegate OnDeath;

		public static event EndDelegate OnMurder;

		public static event OnPauseDelegat OnPause;

		public static event OnSimuliteTimeDelegat OnSimulate;

		public static void IsPause(bool p_value, bool p_dontStopMusic = false)
		{
			if (_CanPause)
			{
				IsPauseForced(p_value, p_dontStopMusic);
			}
		}

		public static void IsPauseForced(bool p_value, bool p_dontStopMusic = false)
		{
			_IsPaused = p_value;
			_IsDebugPaused = false;
			if (RunMainController.OnPause != null)
			{
				RunMainController.OnPause(_IsPaused);
			}
			if (AudioManager.StopOnPause && !p_dontStopMusic)
			{
				AudioManager.PauseMusic(_IsPaused);
				AudioManager.PauseCutscene(_IsPaused);
				AudioManager.PauseAmbient(_IsPaused);
			}
		}

		public static void RunStart()
		{
			DebugUtils.Log("RunStart");
			DataLocal.Current.Save(true);
			DataLocal.Current.SaveLocalBackup();
			DataLocal.UserDontSave = true;
			VectorADSystem.LoadInterstitialAd();
			StarterPacksManager.ActivateSelectedStarterPack();
			GeneratorHelper.PrepareFloor(StarterPacksManager.SelectedStarterPackCurrentFloor, -1);
			ControllerSaveMe.ResetAttemp();
			RankManager.ResetRank();
			RankManager.ResetExp();
		}

		public static void FloorStart()
		{
			DebugUtils.Log("FloorStart");
			if (!Demo.IsPlaying)
			{
				CounterController current = CounterController.Current;
				current.CounterFloor = (int)current.CounterFloor + 1;
			}
			if (!DataLocal.UserDontSave)
			{
				DataLocal.Current.Save(true);
				DataLocal.UserDontSave = true;
			}
			StartLogFile();
			_Scene = new Scene();
			_Scene.Init();
			_IsRunNow = true;
			DebugUtils.SceneLoadStopTimer();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Floor_start);
			Resources.UnloadUnusedAssets();
			GC.Collect();
			NewGame();
			DataLocal.Current.OnStartFloor();
			RankManager.RankOnFloorStart = RankManager.Rank;
			EndFloorManager.SetOnFloorStartValues();
			ControllerPassiveEffects.EventFloorStart();
		}

		public static void FloorEnd()
		{
			DebugUtils.Log("FloorEnd");
			if (!_IsDeath)
			{
				if (Settings.AutosaveDemo && !Demo.IsPlaying)
				{
					Demo.Autosave();
				}
				ControllerPassiveEffects.EventFloorEnd();
				TRE_EndGame.ActivateThisEvent();
				CounterByFloorManager.Current.SetCountersEndFloor();
				EndFloorManager.PrepareData();
				GameRestorer.SaveBackup();
				DataLocal.UserDontSave = false;
				DataLocal.Current.Save(true);
			}
		}

		public static void RunEnd()
		{
			if (_IsRunNow)
			{
				DebugUtils.Log("RunEnd");
				_IsRunNow = false;
				ControllerTutorial.Reset();
				GameRestorer.RemoveBackup();
				GameRestorer.RemoveOnLaunch = false;
				DataLocal.UserDontSave = false;
				DataLocal.Current.OnEndFloor();
				DataLocal.Current.OnEndRun();
				DataLocal.Current.OnTerminal();
				DataLocal.Current.ClearTemporaryItems();
				DataLocal.Current.Save(true);
				MainRandom.ResetRandom();
			}
		}

		private static void NewGame()
		{
			_IsDeath = false;
			_IsWin = false;
			_CanPause = true;
			_IsPaused = false;
			_RunStartTime = DebugUtils.GetMS();
		}

		public static void ClearScene()
		{
			if (_Scene != null)
			{
				_Scene.End();
			}
			_Scene = null;
			Nekki.Vector.Core.Camera.Camera.Clear();
			GC.Collect();
		}

		public static void CheckLoss(ModelHuman.ModelState p_lossType, ModelHuman p_modelHuman, float p_time)
		{
			if (p_lossType == ModelHuman.ModelState.DeadlyDamage)
			{
				p_modelHuman.OnDeath();
				Nekki.Vector.Core.Camera.Camera.Current.Stop();
				AudioManager.PauseMusic(true);
				TRE_OnDeath.ActivateThisEvent();
				if (RunMainController.OnWaitingResurrection != null)
				{
					RunMainController.OnWaitingResurrection(p_time);
				}
			}
			else
			{
				Loss(p_lossType, p_modelHuman, p_time);
			}
		}

		public static void Loss(ModelHuman.ModelState p_lossType, ModelHuman p_modelHuman, float p_time)
		{
			if (_IsWin || _IsDeath)
			{
				return;
			}
			_RunEndTime = DebugUtils.GetMS();
			p_modelHuman.State = p_lossType;
			_CanPause = false;
			_IsDeath = true;
			FloorEnd();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Run_end);
			RunEnd();
			Nekki.Vector.Core.Camera.Camera.Current.Stop();
			switch (p_lossType)
			{
			case ModelHuman.ModelState.Death:
				if (RunMainController.OnDeath != null)
				{
					RunMainController.OnDeath(p_time);
				}
				break;
			case ModelHuman.ModelState.Loss:
				if (RunMainController.OnLoss != null)
				{
					RunMainController.OnLoss(p_time);
				}
				break;
			case ModelHuman.ModelState.Murder:
				if (RunMainController.OnMurder != null)
				{
					RunMainController.OnMurder(p_time);
				}
				break;
			}
			CounterController.Current.CounterFloor = 0;
		}

		public static void Win(ModelHuman p_modelHuman, float p_time)
		{
			_IsWin = true;
			_CanPause = false;
			_RunEndTime = DebugUtils.GetMS();
			FloorEnd();
			p_modelHuman.State = ModelHuman.ModelState.Win;
			if (RunMainController.OnWin != null)
			{
				RunMainController.OnWin(p_time);
			}
		}

		public static void SetDefaultDeathState()
		{
			_IsDeath = false;
		}

		public static void Render()
		{
			if (!_IsPaused && _Scene != null)
			{
				DataLocal.Current.OnStartFrame();
				_Scene.Render();
			}
		}

		private static void StartLogFile()
		{
			VectorLog.Init();
		}

		public static void SimulateTime(float p_time)
		{
			if (RunMainController.OnSimulate != null)
			{
				RunMainController.OnSimulate(p_time);
			}
		}

		public static void SimulateWinEvent()
		{
			foreach (ModelHuman model in Models)
			{
				if (!model.IsBot)
				{
					Win(model, 1f);
					DataLocal.Current.Save(true);
					break;
				}
			}
		}

		public static void SimulateLossEvent(ModelHuman.ModelState p_lossType)
		{
			foreach (ModelHuman model in Models)
			{
				if (!model.IsBot)
				{
					CheckLoss(p_lossType, model, 1f);
					break;
				}
			}
		}
	}
}
