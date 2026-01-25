using System;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Boosterpack
{
	public class BoosterpackPanel : UIModule
	{
		[SerializeField]
		private ButtonUI _OpenButton;

		[SerializeField]
		private CanvasGroup _OpenButtonCanvasGroup;

		[SerializeField]
		private BoosterpackItemsPanel _ItemsPanel;

		[SerializeField]
		private BoosterPackLines _BoosterPackLines;

		[SerializeField]
		private BoosterpackAnimator _BoosterpackAnimator;

		private BottomPanel _BottomPanel;

		private Action _OpenButtonCallback;

		protected override void Init()
		{
			base.Init();
			_BottomPanel = UIModule.GetModule<BottomPanel>();
			_ItemsPanel.Init(this);
			if (GameRestorer.IsRestoreBoosterpacksAvailable)
			{
				_ItemsPanel.RestoreItems();
				_OpenButtonCanvasGroup.alpha = 0f;
				_OpenButtonCanvasGroup.interactable = false;
			}
			PaymentDialog.OnClose += OnPaymentClose;
			_OpenButtonCallback = OnOpenButtonNext;
		}

		protected override void Free()
		{
			base.Free();
			PaymentDialog.OnClose -= OnPaymentClose;
		}

		protected override void OnActivated()
		{
			UpdateOpenButton();
			if (GameRestorer.IsRestoreBoosterpacksAvailable)
			{
				_BoosterpackAnimator.ToOpen();
			}
			else if (Manager.PrevScreen == ScreenType.Main)
			{
				_ItemsPanel.Reset();
				_BoosterpackAnimator.ToClose();
			}
			else
			{
				_ItemsPanel.RestoreItemUI();
			}
		}

		public void UpdateOpenButton()
		{
			int boosterpackQuantity = BoosterpacksManager.BoosterpackQuantity;
			_OpenButton.PaidCount.SetAlias(boosterpackQuantity.ToString());
			SetOpenButtonGrey(boosterpackQuantity > 0);
			SetOpenCallback(boosterpackQuantity > 0);
			bool flag = !BoosterpackItemsManager.IsBoosterpackOpening;
			if (_OpenButtonCanvasGroup.interactable && !flag)
			{
				SetOpenButtonVisible(false, 0.3f);
			}
			else if (!_OpenButtonCanvasGroup.interactable && flag)
			{
				SetOpenButtonVisible(true, 0.3f);
			}
		}

		private void SetOpenButtonGrey(bool p_value)
		{
			_OpenButton.SetType(p_value ? ButtonUI.Type.Green : ButtonUI.Type.Grey);
			_OpenButton.ButtonText.SetAlias((!p_value) ? "^GUI.Buttons.BoosterPack.More^" : "^GUI.Buttons.BoosterPack.Open^");
		}

		private void SetOpenCallback(bool p_notEmpty)
		{
			if (p_notEmpty)
			{
				if (BoosterpackItemsManager.IsBoosterpackOpening)
				{
					_OpenButtonCallback = delegate
					{
					};
				}
				else
				{
					_OpenButtonCallback = OnOpenButtonNext;
				}
			}
			else
			{
				_OpenButtonCallback = ShowPaidBoosterpacks;
			}
		}

		private void SetOpenButtonVisible(bool p_visible, float p_duration, Ease p_easeType = Ease.Linear)
		{
			Tweener t = _OpenButtonCanvasGroup.DOFade((!p_visible) ? 0f : 1f, p_duration).SetEase(p_easeType);
			if (p_visible)
			{
				t.OnComplete(delegate
				{
					_OpenButtonCanvasGroup.interactable = true;
				});
			}
			else
			{
				_OpenButtonCanvasGroup.interactable = false;
			}
		}

		private void OnPaymentClose()
		{
			UpdateOpenButton();
		}

		public void OnOpenButtonTap()
		{
			_OpenButtonCallback();
		}

		public void OnOpenButtonNext()
		{
			_OpenButtonCallback = delegate
			{
			};
			_BottomPanel.SetBackButtonActive(false);
			SetOpenButtonVisible(false, 0.3f);
			AudioManager.PlaySound("blue_button");
			_ItemsPanel.MoveItemsToStart(OnEndMoveItemToStart);
		}

		public void ShowPaidBoosterpacks()
		{
			AudioManager.PlaySound("grey_button");
			DialogNotificationManager.ShowPaymentDialog("Boosterpacks");
		}

		private void OnEndMoveItemToStart()
		{
			_BoosterpackAnimator.ToClose();
			AudioManager.PlaySound("boosterpack_animation");
			_BoosterpackAnimator.RunOpenAnim(_BoosterPackLines, OnEndBoosterpackAnim);
		}

		private void OnEndBoosterpackAnim()
		{
			_ItemsPanel.GenerateItems();
			_ItemsPanel.Prepare();
			UpdateOpenButton();
			_BottomPanel.SetBackButtonActive(true);
		}

		public void OnIconTap()
		{
			_OpenButtonCallback();
		}

		public void OnBackButtonTap()
		{
			if (BoosterpackItemsManager.IsBoosterpackOpening)
			{
				AudioManager.PlaySound("boosterpack_item_open");
				_ItemsPanel.OpenAll();
			}
			else
			{
				BottomPanel module = UIModule.GetModule<BottomPanel>();
				module.OpenSceneScreen();
			}
		}
	}
}
