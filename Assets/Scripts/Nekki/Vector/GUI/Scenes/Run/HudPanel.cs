using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class HudPanel : UIModule
	{
		public enum UnpauseMode
		{
			Instant = 0,
			FadedWithCountdown = 1,
			CountdownOnly = 2
		}

		[SerializeField]
		private GadgetUIPanel _GadgetPanel;

		[SerializeField]
		private FundsPanel _FundsPanel;

		[SerializeField]
		private StatusEffectsPanel _StatusEffectsPanel;

		[SerializeField]
		private SwarmPointersController _SwarmPointersController;

		[SerializeField]
		private GameObject _KeyPrefab;

		[SerializeField]
		private Transform _KeyGrid;

		[SerializeField]
		private Button _PauseButton;

		[SerializeField]
		private Button _SettingsButton;

		[SerializeField]
		private ResolutionImage _PauseImageAlias;

		[SerializeField]
		private Text _CountdownLabel;

		[SerializeField]
		private Image _CountdownBlackBackground;

		[SerializeField]
		private Color _CountdownFadeColor = Color.black;

		[SerializeField]
		private float _CountdownFadeTargetAlpha;

		[SerializeField]
		private float _CountdownFadeDuration = 4f;

		[SerializeField]
		private AnimationCurve _CountdownFadeCurve;

		private bool _CheckKeys = true;

		private UnpauseMode _CurrentUnpauseMode = UnpauseMode.FadedWithCountdown;

		private IEnumerator _CountdownCoroutine;

		private static bool _IsPopupDialogActive;

		private List<KeyUI> _Keys = new List<KeyUI>();

		public FundsPanel FundsPanel
		{
			get
			{
				return _FundsPanel;
			}
		}

		public StatusEffectsPanel PanelStatusEffects
		{
			get
			{
				return _StatusEffectsPanel;
			}
		}

		public UnpauseMode CurrentUnpauseMode
		{
			get
			{
				return _CurrentUnpauseMode;
			}
			set
			{
				_CurrentUnpauseMode = value;
			}
		}

		public static bool IsPopupActive
		{
			get
			{
				return _IsPopupDialogActive;
			}
		}

		protected override void Init()
		{
			base.Init();
			UpdateItems();
			_GadgetPanel.Init(DataLocalHelper.GetUserGadgets(), null, false);
			_SettingsButton.gameObject.SetActive(false);
			DataLocal.Current.OnItem += OnItem;
			MissionsDialog.OnOpen += OnMissionsDialogOpen;
			MissionsDialog.OnClose += OnMissionsDialogClose;
			OptionsDialog.OnOpen += OnOptionsDialogOpen;
			OptionsDialog.OnClose += OnOptionsDialogClose;
			OptionsDialog.OnReturnToGameButton += ReturnToGame;
			SetupCountdownUI();
			UpdateRunGUIVisiblity();
		}

		protected override void Free()
		{
			base.Free();
			DataLocal.Current.OnItem -= OnItem;
			MissionsDialog.OnOpen -= OnMissionsDialogOpen;
			MissionsDialog.OnClose -= OnMissionsDialogClose;
			OptionsDialog.OnOpen -= OnOptionsDialogOpen;
			OptionsDialog.OnClose -= OnOptionsDialogClose;
			OptionsDialog.OnReturnToGameButton -= ReturnToGame;
		}

		public void InitSwarmPointers()
		{
			_SwarmPointersController.Init(RunMainController.Location.ControllerSwarm.Swarms);
		}

		private void Update()
		{
			if (EndFloorManager.ChargesRefreshAllowed)
			{
				_GadgetPanel.RefreshCharges();
			}
			if (_CheckKeys && _Keys.Count > 0)
			{
				foreach (KeyUI key in _Keys)
				{
					key.RefreshQuantity();
				}
			}
			_SwarmPointersController.Refresh();
		}

		private void SetupCountdownUI()
		{
			_CountdownLabel.text = string.Empty;
			_CountdownBlackBackground.gameObject.SetActive(false);
		}

		public void SwitchRunGUIVisiblity()
		{
			bool flag = Settings.RunGUI.ShowCurrency || Settings.RunGUI.ShowGadgets || Settings.RunGUI.ShowKeys || Settings.RunGUI.ShowPause;
			Settings.RunGUI.ShowCurrency = !flag;
			Settings.RunGUI.ShowGadgets = !flag;
			Settings.RunGUI.ShowKeys = !flag;
			Settings.RunGUI.ShowPause = !flag;
			Settings.Save();
			UpdateRunGUIVisiblity();
		}

		private void UpdateRunGUIVisiblity()
		{
			_GadgetPanel.gameObject.SetActive(Settings.RunGUI.ShowGadgets);
			_KeyGrid.gameObject.SetActive(Settings.RunGUI.ShowKeys);
			_PauseButton.gameObject.SetActive(Settings.RunGUI.ShowPause);
		}

		public void StopCountdown()
		{
			if (_CountdownCoroutine != null)
			{
				CoroutineManager.Current.StopRoutine(_CountdownCoroutine);
				StopCoroutine(_CountdownCoroutine);
				_CountdownLabel.text = string.Empty;
				_CountdownBlackBackground.DOKill();
				_CountdownBlackBackground.gameObject.SetActive(false);
			}
		}

		public void HideKeys()
		{
			_CheckKeys = false;
			_KeyGrid.gameObject.SetActive(false);
		}

		public void SetPauseState(bool p_paused, bool p_usePopupDialog)
		{
			if (p_paused == RunMainController.IsPaused)
			{
				return;
			}
			if (p_paused)
			{
				if (ControllerTutorial.IsShow || _CurrentUnpauseMode == UnpauseMode.Instant)
				{
					return;
				}
				RunMainController.IsPause(true);
				StopCountdown();
				_PauseImageAlias.SpriteName = "run.icon_play";
				if (p_usePopupDialog)
				{
					_IsPopupDialogActive = true;
					if (MissionsManager.IsMissionsEnabled)
					{
						DialogNotificationManager.ShowMissionsDialog(true, 0);
					}
					else
					{
						DialogNotificationManager.ShowOptionsDialog(0);
					}
				}
				return;
			}
			_PauseImageAlias.SpriteName = "run.icon_pause";
			if (IsPopupActive)
			{
				if (!p_usePopupDialog)
				{
					if (MissionsDialog.Current != null)
					{
						MissionsDialog.Current.Close();
					}
					if (OptionsDialog.Current != null)
					{
						OptionsDialog.Current.Close();
					}
				}
				_IsPopupDialogActive = false;
				_CountdownCoroutine = UnPause();
				StartCoroutine(_CountdownCoroutine);
			}
			else
			{
				RunMainController.IsPause(false);
			}
		}

		public void ShowCountdown()
		{
			_CurrentUnpauseMode = UnpauseMode.CountdownOnly;
			_CountdownCoroutine = UnPause();
			CoroutineManager.Current.StartRoutine(_CountdownCoroutine);
		}

		private void UpdateItems()
		{
			foreach (UserItem item in DataLocal.Current.Equipped)
			{
				AddItem(item);
			}
		}

		public void OnPauseTap()
		{
			if (!RunMainController.IsPaused)
			{
				SetPauseState(true, true);
			}
			else if (IsPopupActive)
			{
				ReturnToGame();
			}
		}

		public void OnSettingsTap()
		{
			if (OptionsDialog.Current != null)
			{
				ReturnFromOptionsToMissions();
			}
			else
			{
				DialogNotificationManager.ShowOptionsDialogInstant();
			}
		}

		private void OnItem(Action action, UserItem useritem, int value)
		{
			if (DataLocal.Current.Equipped.Contains(useritem))
			{
				switch (action)
				{
				case Action.Add:
					AddItem(useritem);
					break;
				case Action.Remove:
					RemoveItem(useritem);
					break;
				}
			}
		}

		private void OnMissionsDialogOpen()
		{
			MoveToDialogsCanvas();
			_GadgetPanel.gameObject.SetActive(false);
			_StatusEffectsPanel.gameObject.SetActive(false);
			_KeyGrid.gameObject.SetActive(false);
			_SettingsButton.gameObject.SetActive(true);
		}

		private void OnMissionsDialogClose()
		{
		}

		private void OnOptionsDialogOpen()
		{
			if (MissionsManager.IsMissionsEnabled)
			{
				MissionsDialog.Current.gameObject.SetActive(false);
				return;
			}
			MoveToDialogsCanvas();
			_GadgetPanel.gameObject.SetActive(false);
			_StatusEffectsPanel.gameObject.SetActive(false);
			_KeyGrid.gameObject.SetActive(false);
		}

		private void OnOptionsDialogClose()
		{
		}

		private void ReturnToGame()
		{
			if (MissionsDialog.Current != null)
			{
				MissionsDialog.Current.Close();
			}
			if (OptionsDialog.Current != null)
			{
				OptionsDialog.Current.Close();
			}
			MoveToSceneCanvas();
			_GadgetPanel.gameObject.SetActive(true);
			_StatusEffectsPanel.gameObject.SetActive(true);
			_KeyGrid.gameObject.SetActive(true);
			_SettingsButton.gameObject.SetActive(false);
			SetPauseState(false, true);
		}

		private void ReturnFromOptionsToMissions()
		{
			OptionsDialog.Current.Close();
			MissionsDialog.Current.gameObject.SetActive(true);
			DialogCanvasController.Current.TurnOnBlurEffect();
		}

		private void AddItem(UserItem p_item)
		{
			KeyItem keyItem = KeyItem.Create(p_item);
			if (keyItem != null)
			{
				GameObject gameObject = Object.Instantiate(_KeyPrefab);
				gameObject.transform.SetParent(_KeyGrid, false);
				gameObject.transform.SetAsLastSibling();
				KeyUI component = gameObject.GetComponent<KeyUI>();
				component.Init(keyItem);
				_Keys.Add(component);
			}
		}

		private void RemoveItem(UserItem p_item)
		{
			foreach (KeyUI key in _Keys)
			{
				if (key.CurrentItem.CurrItem == p_item)
				{
					_Keys.Remove(key);
					Object.Destroy(key.gameObject);
					break;
				}
			}
		}

		private IEnumerator UnPause()
		{
			switch (_CurrentUnpauseMode)
			{
			case UnpauseMode.Instant:
				_CountdownBlackBackground.DOKill();
				_CountdownBlackBackground.gameObject.SetActive(false);
				if (!IsPopupActive)
				{
					RunMainController.IsPause(false);
				}
				break;
			case UnpauseMode.FadedWithCountdown:
				_CountdownBlackBackground.gameObject.SetActive(true);
				_CountdownBlackBackground.color = _CountdownFadeColor;
				_CountdownBlackBackground.DOFade(_CountdownFadeTargetAlpha, _CountdownFadeDuration).SetEase(_CountdownFadeCurve);
				AudioManager.PlaySound("countdown");
				_CountdownLabel.text = "3";
				yield return new WaitForSeconds(1f);
				_CountdownLabel.text = "2";
				yield return new WaitForSeconds(1f);
				_CountdownLabel.text = "1";
				yield return new WaitForSeconds(1f);
				_CountdownLabel.text = string.Empty;
				if (!IsPopupActive)
				{
					RunMainController.IsPause(false);
				}
				_CountdownBlackBackground.gameObject.SetActive(false);
				break;
			case UnpauseMode.CountdownOnly:
				_CountdownBlackBackground.DOKill();
				_CountdownBlackBackground.gameObject.SetActive(false);
				AudioManager.PlaySound("countdown");
				_CountdownLabel.text = "3";
				yield return new WaitForTime(1f);
				_CountdownLabel.text = "2";
				yield return new WaitForTime(1f);
				_CountdownLabel.text = "1";
				yield return new WaitForTime(1f);
				_CountdownLabel.text = string.Empty;
				if (!IsPopupActive)
				{
					RunMainController.IsPause(false);
				}
				ResetUnpauseModeToDefault();
				break;
			}
		}

		public void ResetUnpauseModeToDefault()
		{
			_CurrentUnpauseMode = UnpauseMode.FadedWithCountdown;
		}

		public Transform FindGadget(UserItem item)
		{
			return _GadgetPanel.GetTransformGadget(item);
		}
	}
}
