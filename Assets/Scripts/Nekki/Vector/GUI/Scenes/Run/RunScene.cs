using System;
using System.Collections;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.InputControllers;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class RunScene : Scene<RunScene>
	{
		[SerializeField]
		private Camera _Camera;

		private HudPanel _Hud;

		private RunFPSMeter _FPSMeter;

		private System.Action _EndAction;

		private System.Action _DelayEndAction;

		public override SceneKind SceneId
		{
			get
			{
				return SceneKind.Run;
			}
		}

		protected override void Init()
		{
			base.Init();
			RunMainController.FloorStart();
			_Hud = GetModule<HudPanel>();
			_FPSMeter = GetModule<RunFPSMeter>();
			_Hud.InitSwarmPointers();
			RunMainController.OnWin += OnWin;
			RunMainController.OnLoss += OnGameOver;
			RunMainController.OnWaitingResurrection += OnWaitingResurrection;
			RunMainController.OnDeath += OnGameOver;
			RunMainController.OnMurder += OnGameOver;
			ZoneResource<MusicManager>.Current.PlayMusic();
			ZoneResource<MusicManager>.Current.PlayAmbient();
			QuestManager.Current.CheckEvent(TQE_OnScreen.ScreenRunEvent);
			Manager.ShowDebugOptionsIfNeed();
		}

		protected override void Free()
		{
			StopAllCoroutines();
			RunMainController.OnWin -= OnWin;
			RunMainController.OnLoss -= OnGameOver;
			RunMainController.OnWaitingResurrection -= OnWaitingResurrection;
			RunMainController.OnDeath -= OnGameOver;
			RunMainController.OnMurder -= OnGameOver;
			RunMainController.ClearScene();
			base.Free();
		}

		private void Update()
		{
			if (_IsInited && RunMainController.IsRunNow)
			{
			}
		}

		private void FixedUpdate()
		{
			if (_EndAction != null)
			{
				_EndAction();
				_EndAction = null;
			}
			else if (_IsInited && RunMainController.IsRunNow)
			{
				RunMainController.Render();
			}
		}

		public void OnKeyDown(KeyCode p_code)
		{
			switch (p_code)
			{
				case KeyCode.UpArrow:
					RunMainController.Scene.KeysVariables(new KeyVariables("Up"));
					break;
				case KeyCode.DownArrow:
					RunMainController.Scene.KeysVariables(new KeyVariables("Down"));
					break;
				case KeyCode.LeftArrow:
					RunMainController.Scene.KeysVariables(new KeyVariables("Left"));
					break;
				case KeyCode.RightArrow:
					RunMainController.Scene.KeysVariables(new KeyVariables("Right"));
					break;
				case KeyCode.Escape:
					if (DeviceInformation.IsAndroid || DeviceInformation.IsEmulator)
					{
						if (!RunMainController.IsPaused)
						{
							_Hud.SetPauseState(true, true);
						}
					}
					else
					{
						Manager.Quit();
					}
					break;
				case KeyCode.R:
					if (!Core.Game.Settings.IsReleaseBuild)
					{
						RunScene.Current.OnButtonRestart();
					}
					break;
				case KeyCode.P:
                    if (!Core.Game.Settings.IsReleaseBuild)
                    {
                        RunScene.Current.OnButtonPause();
                    }
                    break;
                default:
					if (!Core.Game.Settings.IsReleaseBuild)
					{
						RunMainController.Scene.KeysVariables(new KeyVariables(p_code.ToString()));
					}
                    break;
            }
		}

		public void OnSlide(int p_index, Vector2 p_from, Vector2 p_to)
		{
			switch (TouchController.GetDirection(p_from, p_to))
			{
			case Direction.Up:
				RunMainController.Scene.KeysVariables(new KeyVariables("Up"));
				break;
			case Direction.Bottom:
				RunMainController.Scene.KeysVariables(new KeyVariables("Down"));
				break;
			case Direction.Left:
				RunMainController.Scene.KeysVariables(new KeyVariables("Left"));
				break;
			case Direction.Right:
				RunMainController.Scene.KeysVariables(new KeyVariables("Right"));
				break;
			}
		}

		public void OnDrag(int p_index, Vector2 p_from, Vector2 p_to)
		{
		}

		public void OnButtonPause()
		{
			if (RunMainController.IsRunNow)
			{
				if (RunMainController.IsPaused)
				{
					_Hud.SetPauseState(false, false);
				}
				else
				{
					RunMainController.IsPauseForced(!RunMainController.IsPaused);
				}
				RunMainController.IsDebugPaused = RunMainController.IsPaused;
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (RunMainController.IsRunNow && !RunMainController.IsWin && !RunMainController.IsPaused)
			{
				_Hud.SetPauseState(true, true);
			}
		}

		public void OnButtonRestart(bool p_resetSeed = false)
		{
			_EndAction = delegate
			{
				if (!Input.GetKeyDown(KeyCode.LeftCommand))
				{
					RunMainController.RunEnd();
				}
				if (Settings.ReloadMovesOnRestart)
				{
					AnimationLoader.Current.ReloadAnimations();
				}
				if (!Settings.IsReleaseBuild)
				{
					DataLocal.LoadDemoBackUp();
				}
				if (p_resetSeed)
				{
					MainRandom.SetSeed(-1);
				}
				else
				{
					MainRandom.ResetWithCurrentSeed();
				}
				CounterByFloorManager.Reset();
				if (!Demo.IsPlaying && LinkEditor.IsFileNew())
				{
					LinkEditor.TryLaunchEditorRoom();
				}
				else
				{
					CounterController.Current.CounterFloor = RunMainController.CurrentFloor - 1;
					Manager.Load(SceneKind.Run);
				}
			};
		}

		public void OnButtonClick()
		{
			_EndAction = delegate
			{
				Exit();
			};
		}

		public void OnWin(float p_time)
		{
			StartCoroutine(ShowMessage(p_time, true));
		}

		public void OnGameOver(float p_time)
		{
			StartCoroutine(ShowMessage(p_time, false));
		}

		public void OnWaitingResurrection(float p_time)
		{
			StartCoroutine(ShowSaveMe(p_time));
		}

		public virtual IEnumerator ShowMessage(float p_time, bool p_IsWin)
		{
			if (!p_IsWin && AudioManager.StopOnDeath)
			{
				AudioManager.StopMusic(0f);
				AudioManager.StopCutscene(0f);
				AudioManager.StopAmbient(0f);
			}
			yield return new WaitForSeconds(p_time);
			if (p_IsWin)
			{
				RunMainController.Player.ControllerControl.Enable = false;
				RunMainController.CanPause = true;
				RunMainController.IsPause(true, true);
				_Hud.HideKeys();
				if ((int)CounterController.Current.СounterInsertUpgradeTutorial != 0)
				{
					DialogNotificationManager.ShowEndFloorDialog(null);
					DialogNotificationManager.ShowInsertUpgradeTalkingDialog(Exit);
				}
				else
				{
					DialogNotificationManager.ShowEndFloorDialog(Exit);
				}
				CounterByFloorManager.Current.SetCountersSupply();
			}
			else if (Settings.PromtExitOnDeath)
			{
				SetDelayEndAction();
				OnButtonPause();
			}
			else
			{
				OnButtonClick();
			}
		}

		public virtual IEnumerator ShowSaveMe(float p_time)
		{
			yield return new WaitForSeconds(p_time);
			RunMainController.Location.ControllerSaveMe.OnLoss(p_time);
		}

		private void SetDelayEndAction()
		{
			_DelayEndAction = delegate
			{
				Exit();
			};
		}

		private void CheckDelayEndAction()
		{
			if (_DelayEndAction != null)
			{
				_DelayEndAction();
				_DelayEndAction = null;
			}
		}

		private void Exit()
		{
			Free();
			RunMainController.IsPause(true, true);
			if (RunMainController.IsWin)
			{
				if (!Demo.IsPlaying && (int)CounterController.Current.CounterPlayCommand == 0)
				{
					Manager.Load(SceneKind.Shop);
					return;
				}
				Manager.Load(SceneKind.Terminal);
			}
			else
			{
				Manager.Load(SceneKind.Terminal);
			}
			Demo.ResetStatus();
			LinkEditor.RemoveTempFile();
		}
	}
}
