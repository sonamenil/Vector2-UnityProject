using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.DetailsScreen
{
	public class InfoPanel : MonoBehaviour
	{
		public enum Mode
		{
			None = 0,
			Starterpack = 1,
			Gadget = 2,
			Card = 3
		}

		[SerializeField]
		private StarterpackInfoPanel _StarterpackInfo;

		[SerializeField]
		private GadgetInfoPanel _GadgetInfo;

		[SerializeField]
		private CardInfoPanel _CardInfo;

		[SerializeField]
		private LabelAlias _TapTheCardsLabel;

		private Mode _CurrentMode;

		private Sequence _Sequence;

		public bool IsCurrentModeStarterpack
		{
			get
			{
				return _CurrentMode == Mode.Starterpack;
			}
		}

		public bool IsCurrentModeGadget
		{
			get
			{
				return _CurrentMode == Mode.Gadget;
			}
		}

		public bool IsCurrentModeCard
		{
			get
			{
				return _CurrentMode == Mode.Card;
			}
		}

		public void Init()
		{
			_CardInfo.Init();
		}

		public void Reset()
		{
			_StarterpackInfo.HideInstantly();
			_GadgetInfo.HideInstantly();
			_CardInfo.HideInstantly(560f);
			_CurrentMode = Mode.None;
		}

		public void ShowCurrentStarterpackInfo()
		{
			ShowStarterpackInfo(StarterPacksManager.SelectedStarterPack);
		}

		public void ShowStarterpackInfo(StarterPackItem p_starterpack)
		{
			if (IsCurrentModeStarterpack)
			{
				_StarterpackInfo.Init(p_starterpack);
				return;
			}
			_CurrentMode = Mode.Starterpack;
			ResetTween();
			_Sequence = HideAll();
			_Sequence.AppendCallback(delegate
			{
				_StarterpackInfo.gameObject.SetActive(true);
				_StarterpackInfo.Init(p_starterpack);
			});
			_Sequence.Append(_StarterpackInfo.ChangeAlpha(1f, 0.3f, Ease.OutQuad));
			_Sequence.Play();
		}

		public void ShowGadgetInfo(GadgetItem p_gadget)
		{
			if (IsCurrentModeGadget)
			{
				_GadgetInfo.Init(p_gadget, OnCardTap);
				_TapTheCardsLabel.gameObject.SetActive(p_gadget.Cards.Count > 0);
				return;
			}
			_CurrentMode = Mode.Gadget;
			ResetTween();
			_Sequence = HideAll();
			_Sequence.AppendCallback(delegate
			{
				_GadgetInfo.gameObject.SetActive(true);
				_GadgetInfo.Init(p_gadget, OnCardTap);
				_TapTheCardsLabel.gameObject.SetActive(p_gadget.Cards.Count > 0);
			});
			_Sequence.Append(_GadgetInfo.ChangeAlpha(1f, 0.3f, Ease.OutQuad));
			_Sequence.Play();
		}

		public void ShowCardInfo(CardsGroupAttribute p_card)
		{
			if (IsCurrentModeCard)
			{
				_CardInfo.SelectCard(p_card);
				return;
			}
			_CurrentMode = Mode.Card;
			ResetTween();
			_Sequence = HideAll();
			_Sequence.AppendCallback(delegate
			{
				_CardInfo.gameObject.SetActive(true);
				_CardInfo.SelectCard(p_card);
			});
			_Sequence.Append(_CardInfo.MoveContentY(0f, 0.3f, Ease.OutQuad));
			_Sequence.AppendCallback(delegate
			{
				_CardInfo.ChangeBackButtonAlpha(1f, 0.3f);
			});
			_Sequence.Play();
		}

		private void ResetTween()
		{
			if (_Sequence != null)
			{
				_Sequence.Complete(true);
				_Sequence = null;
			}
		}

		private Sequence HideAll()
		{
			Sequence sequence = DOTween.Sequence();
			if (_StarterpackInfo.gameObject.activeSelf)
			{
				sequence.Join(_StarterpackInfo.ChangeAlpha(0f, 0.5f, Ease.InQuad));
				sequence.AppendCallback(delegate
				{
					_StarterpackInfo.gameObject.SetActive(false);
				});
			}
			if (_GadgetInfo.gameObject.activeSelf)
			{
				sequence.Join(_GadgetInfo.ChangeAlpha(0f, 0.5f, Ease.InQuad));
				sequence.AppendCallback(delegate
				{
					_GadgetInfo.gameObject.SetActive(false);
				});
			}
			if (_CardInfo.gameObject.activeSelf)
			{
				sequence.AppendCallback(delegate
				{
					_CardInfo.ChangeBackButtonAlpha(0f, 0.3f);
				});
				sequence.Join(_CardInfo.MoveContentY(560f, 0.5f, Ease.InQuad));
				sequence.AppendCallback(delegate
				{
					_CardInfo.gameObject.SetActive(false);
				});
			}
			return sequence;
		}

		private void OnCardTap(BaseCardUI p_card)
		{
			if (p_card.Card != null)
			{
				ShowCardInfo(p_card.Card);
			}
		}

		public void OnBackTap()
		{
			ShowGadgetInfo(_GadgetInfo.Gadget);
		}
	}
}
