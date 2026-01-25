using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class GadgetInfoPanel : MonoBehaviour
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private bool _ChargesOnlyCurrent;

		[SerializeField]
		private Text _ChargesCount;

		[SerializeField]
		private ResolutionImage _EffectIcon;

		[SerializeField]
		private LabelAlias _EffectText;

		[SerializeField]
		private CardsPanel _CardsPanel;

		[SerializeField]
		private CanvasGroup _Content;

		private GadgetItem _Gadget;

		private Action<BaseCardUI> _OnCardTap;

		public GadgetItem Gadget
		{
			get
			{
				return _Gadget;
			}
		}

		public void Init(GadgetItem p_gadget, Action<BaseCardUI> p_onCardTap)
		{
			_Gadget = p_gadget;
			_OnCardTap = p_onCardTap;
			_Title.SetAlias(_Gadget.CurrItem.VisualName);
			_ChargesCount.text = ((!_ChargesOnlyCurrent) ? string.Format("{0}/{1}", _Gadget.CurrentCharges, _Gadget.TotalCharges) : _Gadget.CurrentCharges.ToString());
			_EffectIcon.SpriteName = _Gadget.EffectIcon;
			List<CardsGroupAttribute> cardsWithEmpty = _Gadget.CardsWithEmpty;
			if (_EffectText != null)
			{
				_EffectText.SetAlias(p_gadget.Description);
			}
			if (_CardsPanel != null)
			{
				_CardsPanel.Init(cardsWithEmpty, OnCardTap);
			}
		}

		public void OnCardTap(BaseCardUI p_card)
		{
			if (_OnCardTap != null)
			{
				_OnCardTap(p_card);
			}
		}

		public Tweener ChangeAlpha(float p_alpha, float p_duration, Ease p_ease)
		{
			return _Content.DOFade(p_alpha, p_duration).SetEase(p_ease);
		}

		public void HideInstantly()
		{
			_Content.alpha = 0f;
			base.gameObject.SetActive(false);
		}
	}
}
