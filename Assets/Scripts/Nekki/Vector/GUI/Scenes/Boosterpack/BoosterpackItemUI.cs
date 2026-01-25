using System;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Boosterpack
{
	public class BoosterpackItemUI : MonoBehaviour
	{
		private const string _CountTextFormat = "+{0}";

		private const int _RewardSize = 200;

		private const float _AnimTime = 0.5f;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private GameObject _BaseCardUIPrefab;

		[SerializeField]
		private GameObject _CouponsRewardUIPrefab;

		[SerializeField]
		private GameObject _CurrencyRewardUIPrefab;

		[SerializeField]
		private GameObject _EffectsHolder;

		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _Description;

		[SerializeField]
		private Button _Button;

		[SerializeField]
		private GameObject _Count;

		[SerializeField]
		private LabelAlias _CountText;

		[SerializeField]
		private BoosterpackEffect _BoosterpackEffect;

		private float _StartPositionX = 1000f;

		private float _MidlePositionX;

		private BoosterpackPanel _Parent;

		private BaseCardUI _CardRewardUI;

		private CouponRewardUI _CouponRewardUI;

		private CurrencyRewardUI _CurrencyRewardUI;

		private Sequence _MoveToMidlePositionSeq;

		private BoosterpackItem _RewardItem;

		public BoosterpackItem RewardItem
		{
			get
			{
				return _RewardItem;
			}
			set
			{
				_RewardItem = value;
				if (_RewardItem != null)
				{
					base.gameObject.SetActive(true);
					if (_RewardItem.IsGiven)
					{
						Open(true);
					}
					else
					{
						_Button.enabled = true;
					}
				}
			}
		}

		public bool IsOpened
		{
			get
			{
				return _RewardItem != null && _RewardItem.IsGiven;
			}
		}

		public void Init(BoosterpackPanel p_parent)
		{
			_Parent = p_parent;
			_CardRewardUI = CreateRewardUI<BaseCardUI>(_BaseCardUIPrefab);
			_CardRewardUI.NeedShowSlot = true;
			_CardRewardUI.NeedShowProgressBar = true;
			_CardRewardUI.NeedShowProgressAnimation = true;
			_CardRewardUI.NeedShowForMissionIcon = Manager.IsShop;
			_CardRewardUI.SlotOffset = 52;
			_CouponRewardUI = CreateRewardUI<CouponRewardUI>(_CouponsRewardUIPrefab);
			_CurrencyRewardUI = CreateRewardUI<CurrencyRewardUI>(_CurrencyRewardUIPrefab);
			_StartPositionX = _Content.rect.width + 150f;
			_MidlePositionX = _Content.rect.width - 315f;
			Reset();
		}

		private T CreateRewardUI<T>(GameObject p_prefab) where T : MonoBehaviour
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(p_prefab);
			gameObject.transform.SetParent(_EffectsHolder.transform, false);
			gameObject.transform.SetAsLastSibling();
			return gameObject.GetComponent<T>();
		}

		public void Open(bool p_instantly = false)
		{
			_Button.enabled = false;
			MoveLeft((!p_instantly) ? 0.5f : 0f, OnOpened);
		}

		private void OnOpened()
		{
			_BoosterpackEffect.Stop();
			_RewardItem.GiveReward();
			_Parent.UpdateOpenButton();
			UpdateUI();
		}

		public void Reset()
		{
			_BoosterpackEffect.Stop();
			_RewardItem = null;
			_Button.enabled = false;
			UpdateUI();
			MoveToStartPosition(0f);
			base.gameObject.SetActive(false);
		}

		public void Prepare()
		{
			MoveToMidlePosition(0.5f, 0.5f);
		}

		public void RetoreUI()
		{
			if (_RewardItem != null && !_RewardItem.IsGiven)
			{
				_BoosterpackEffect.Play();
			}
		}

		private void UpdateUI()
		{
			if (_RewardItem == null)
			{
				_CardRewardUI.gameObject.SetActive(false);
				_CouponRewardUI.gameObject.SetActive(false);
				_CurrencyRewardUI.gameObject.SetActive(false);
				_Title.gameObject.SetActive(false);
				_Description.gameObject.SetActive(false);
				_Count.SetActive(false);
				return;
			}
			if (_RewardItem.IsCard)
			{
				CardsGroupAttribute itemAsCard = _RewardItem.ItemAsCard;
				_CardRewardUI.Card = itemAsCard;
				_CardRewardUI.AnimationCardCount = -_RewardItem.Count;
				_CardRewardUI.gameObject.SetActive(true);
				_Title.SetAlias(itemAsCard.CardVisualName);
				_Description.SetAlias(itemAsCard.CardText);
				_CountText.SetAlias(string.Format("+{0}", _RewardItem.Count));
				if (itemAsCard.UserCardTotalLevel != 0 || itemAsCard.UserCardsSinceLastLevel != 1)
				{
					_Count.SetActive(true);
				}
			}
			else if (_RewardItem.IsCoupon)
			{
				CouponGroupAttribute itemAsCoupon = _RewardItem.ItemAsCoupon;
				_CouponRewardUI.Init(itemAsCoupon);
				_Title.SetAlias(itemAsCoupon.VisualName);
				_Description.SetAlias(itemAsCoupon.Description);
				_CountText.SetAlias(string.Format("+{0}", _RewardItem.Count));
				_Count.SetActive(true);
				_CouponRewardUI.gameObject.SetActive(true);
			}
			else
			{
				CurrencyItem itemAsCurrency = _RewardItem.ItemAsCurrency;
				_CurrencyRewardUI.Init(itemAsCurrency);
				_Title.SetAlias(string.Empty);
				_Description.SetAlias(string.Empty);
				_Count.SetActive(false);
				_CurrencyRewardUI.gameObject.SetActive(true);
			}
			_Title.gameObject.SetActive(true);
			_Description.gameObject.SetActive(true);
		}

		private void MoveLeft(float p_duration, Action p_callback)
		{
			StopPlaySeq();
			if (p_duration > 1E-05f)
			{
				_Content.DOLocalMoveX(0f, p_duration).OnComplete(delegate
				{
					p_callback();
				});
			}
			else
			{
				_Content.localPosition = new Vector2(0f, _Content.localPosition.y);
				p_callback();
			}
		}

		private void MoveToMidlePosition(float p_delay, float p_duration)
		{
			StopPlaySeq();
			_MoveToMidlePositionSeq = DOTween.Sequence();
			_MoveToMidlePositionSeq.AppendInterval(p_delay);
			_MoveToMidlePositionSeq.Append(_Content.DOLocalMoveX(_MidlePositionX, p_duration));
			_MoveToMidlePositionSeq.AppendCallback(delegate
			{
				_BoosterpackEffect.Play();
			});
			_MoveToMidlePositionSeq.OnKill(ResetMoveToMidlePositionSeq);
			_MoveToMidlePositionSeq.Play();
		}

		public void MoveToStartPosition(float p_duration = 0f)
		{
			StopPlaySeq();
			if (p_duration > 1E-05f)
			{
				AudioManager.PlaySound("item_out");
				_Content.DOLocalMoveX(_StartPositionX, p_duration);
			}
			else
			{
				_Content.localPosition = new Vector2(_StartPositionX, _Content.localPosition.y);
			}
		}

		public void OnTap()
		{
			AudioManager.PlaySound("boosterpack_item_open");
			Open(false);
		}

		private void StopPlaySeq()
		{
			if (_MoveToMidlePositionSeq != null)
			{
				_MoveToMidlePositionSeq.Kill();
			}
		}

		private void ResetMoveToMidlePositionSeq()
		{
			_MoveToMidlePositionSeq = null;
		}
	}
}
