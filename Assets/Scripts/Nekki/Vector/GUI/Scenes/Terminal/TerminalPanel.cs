using DG.Tweening;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Advertising;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class TerminalPanel : UIModule
	{
		[SerializeField]
		private CardInfoPanel _CardInfo;

		[SerializeField]
		private ItemInfoPanel _ItemInfo;

		[SerializeField]
		private TerminalItemsPanel _TerminalItemsPanel;

		protected override void Init()
		{
			base.Init();
			PaymentDialog.OnClose += UpdateButton;
		}

		protected override void Free()
		{
			base.Free();
			PaymentDialog.OnClose -= UpdateButton;
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			_CardInfo.Init();
			_CardInfo.gameObject.SetActive(false);
			_ItemInfo.Init();
			_ItemInfo.gameObject.SetActive(false);
			_TerminalItemsPanel.Init(TerminalItemsManager.BasketItems, this);
		}

		private void UpdateButton()
		{
			_CardInfo.UpdateButton();
			_ItemInfo.UpdateButton();
		}

		public void GenerateItems()
		{
			Scene<TerminalScene>.Current.IsGameRestoreActive = GameRestorer.Active;
			TerminalItemsManager.CreateBasketItems();
			GameRestorer.SaveBackup();
		}

		public void SetTerminalItemTab(TerminalItemTab p_tab)
		{
			_CardInfo.gameObject.SetActive(false);
			_ItemInfo.gameObject.SetActive(false);
			if (p_tab.Item.IsCardReward)
			{
				_CardInfo.SelectCard(p_tab.Item);
				_CardInfo.gameObject.SetActive(true);
			}
			else
			{
				_ItemInfo.Select(p_tab.Item);
				_ItemInfo.gameObject.SetActive(true);
			}
		}

		public void Exit()
		{
			if (VectorADSystem.ShowInterstitialAd())
			{
				ADSystem.Current.OnAdFinished += OnAdFinished;
				AudioManager.PauseMusic(true);
			}
			else
			{
				LoadMain();
			}
		}

		public void OnAdFinished(ADSystem.ADType p_type, ADSystem.ADFinishedStatus p_status)
		{
			AudioManager.PauseMusic(false);
			ADSystem.Current.OnAdFinished -= OnAdFinished;
			LoadMain();
		}

		private void LoadMain()
		{
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Terminal_close);
			RunMainController.SetDefaultDeathState();
			GameRestorer.RemoveBackup();
			Manager.Load(SceneKind.Main);
		}

		public void RerollAllCards()
		{
			_TerminalItemsPanel.RerollAll();
			SetTerminalItemTab(_TerminalItemsPanel.SelectedTab);
		}

		public void OnBuyButtonClick()
		{
			TerminalItemTab tab = _TerminalItemsPanel.SelectedTab;
			if (!DataLocalHelper.CanSpendCurrency(tab.Item.CurrencyType, tab.Item.Price))
			{
				AudioManager.PlaySound("red_button");
				DialogNotificationManager.ShowNotEnoughtCurrencyDialog(tab.Item.CurrencyType);
				return;
			}
			AudioManager.PlaySound("blue_button");
			EventSystem eventSystem = EventSystem.current;
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				if (eventSystem != null)
				{
					eventSystem.enabled = false;
				}
				_CardInfo.Button.Button.interactable = false;
				_ItemInfo.Button.Button.interactable = false;
				_CardInfo.Button.Fade(0.3f);
			});
			tab.GetBuyTween(sequence);
			sequence.AppendCallback(delegate
			{
				AudioManager.PlaySound("buying");
				TerminalItemsManager.Buy(tab.Item);
				tab.TwinkleOff();
				SetTerminalItemTab(tab);
				_CardInfo.TwinkleOff();
			});
			sequence.OnKill(delegate
			{
				if (eventSystem != null)
				{
					eventSystem.enabled = true;
				}
			});
			sequence.Play();
		}
	}
}
