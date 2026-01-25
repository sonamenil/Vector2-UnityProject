using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class MissionItemUI : MonoBehaviour
	{
		[SerializeField]
		private Image _MainBg;

		[SerializeField]
		private LabelAlias _Description;

		[SerializeField]
		[Header("Rewards")]
		private GameObject _MissionStarPrefab;

		[SerializeField]
		private HorizontalLayoutGroup _RewardLayout;

		[SerializeField]
		private GameObject _MissionPointsRewardGroup;

		[SerializeField]
		private LabelAlias _PointsAmount;

		[SerializeField]
		private CanvasGroup _MissionPointsRewardGroupCanvas;

		[SerializeField]
		[Header("RightBlock")]
		private Image _RightBlock;

		[SerializeField]
		private CanvasGroup _RightBlockCanvas;

		[SerializeField]
		private GameObject _CompleteIcon;

		[SerializeField]
		private GameObject _IncompleteIcon;

		[Header("Recommendations")]
		[SerializeField]
		private GameObject _RecommendationsBlock;

		[SerializeField]
		private GridLayoutGroup _CardsLayout;

		[SerializeField]
		private GameObject _CardPrefab;

		[SerializeField]
		private BaseCardUISettings _CardSettings = new BaseCardUISettings();

		private MissionItem _Item;

		private List<StarIcon> _Stars = new List<StarIcon>();

		private List<BaseCardUI> _Cards = new List<BaseCardUI>();

		private static Color _RunMainBgColor = ColorUtils.FromHex("0A0E13D4");

		private static Color _RunRightBgColor = ColorUtils.FromHex("000000FF");

		private int _UnownedCards;

		public void Init(MissionItem p_item, bool p_endfloorView = false)
		{
			Reset();
			SetData(p_item, p_endfloorView);
		}

		private void Reset()
		{
			_Item = null;
			DestroyCards();
			DestroyRewards();
		}

		private void SetData(MissionItem p_item, bool p_endfloorView)
		{
			_Item = p_item;
			_Description.SetAlias(_Item.Description);
			SetRewards((p_item.IsCompleted || !Manager.IsRun) && !p_endfloorView);
			_RightBlockCanvas.alpha = ((!p_endfloorView) ? 1f : 0f);
			if (Manager.IsRun)
			{
				SetCompletionIcons();
			}
			else
			{
				SetRecommendations();
			}
			if (Manager.IsRun)
			{
				_MainBg.color = _RunMainBgColor;
				_RightBlock.color = _RunRightBgColor;
			}
		}

		private void SetRewards(bool isCompleted)
		{
			int i = 0;
			for (int difficulty = _Item.Difficulty; i < difficulty; i++)
			{
				StarIcon component = Object.Instantiate(_MissionStarPrefab).GetComponent<StarIcon>();
				component.transform.SetParent(_RewardLayout.transform, false);
				component.Init(0);
				_Stars.Add(component);
				if (isCompleted)
				{
					component.SetCompleteStarIcon(1f, true);
				}
			}
			_MissionPointsRewardGroupCanvas.alpha = ((!isCompleted) ? 0f : 1f);
			_PointsAmount.Text = ((!isCompleted) ? "0" : _Item.RewardAmount.ToString());
		}

		private void SetCompletionIcons()
		{
			_RecommendationsBlock.SetActive(false);
			_CompleteIcon.SetActive(_Item.IsCompleted);
			_IncompleteIcon.SetActive(!_Item.IsCompleted);
		}

		private void SetRecommendations()
		{
			List<CardsGroupAttribute> cards = _Item.Cards;
			if (cards.Count == 0)
			{
				_RecommendationsBlock.gameObject.SetActive(false);
				_CardsLayout.gameObject.SetActive(false);
				return;
			}
			int i = 0;
			for (int count = cards.Count; i < count; i++)
			{
				BaseCardUI component = Object.Instantiate(_CardPrefab).GetComponent<BaseCardUI>();
				component.transform.SetParent(_CardsLayout.transform, false);
				component.Init(_CardSettings, OnCardTap, cards[i]);
				if (count == 3 && i + 1 == 3)
				{
					component.GetComponent<RectTransform>().pivot = new Vector2(2f, 0.5f);
				}
				if (component.Card.IsShowAsOwned)
				{
					_UnownedCards++;
				}
				_Cards.Add(component);
			}
		}

		private void DestroyCards()
		{
			for (int i = 0; i < _Cards.Count; i++)
			{
				Object.Destroy(_Cards[i].gameObject);
			}
			_Cards.Clear();
			_UnownedCards = 0;
		}

		private void DestroyRewards()
		{
			for (int i = 0; i < _Stars.Count; i++)
			{
				Object.Destroy(_Stars[i].gameObject);
			}
			_Stars.Clear();
		}

		private void ClearStatisticsCounters()
		{
			CounterController.Current.CounterMissionDifficultyRemove();
			CounterController.Current.CounterMissionUnownedCardsRemove();
		}

		private void CreateStatisticsCounters()
		{
			CounterController.Current.CounterMissionDifficulty = _Item.Difficulty;
			CounterController.Current.CounterMissionUnownedCards = _UnownedCards;
		}

		private void OnCardTap(BaseCardUI p_card)
		{
			ClearStatisticsCounters();
			CreateStatisticsCounters();
			Tooltip.UISettings uISettings = new Tooltip.UISettings();
			uISettings.Text = p_card.Card.CardText;
			uISettings.Title = p_card.Card.CardVisualName;
			uISettings.Parent = p_card.GetComponent<RectTransform>();
			uISettings.Shift = new Vector2((float)p_card.CardSize * -0.55f, (float)p_card.CardSize * -0.55f);
			uISettings.Pivot = Tooltip.Pivot.TopRight;
			uISettings.OpenAnimation = Tooltip.AnimationType.Fade;
			uISettings.CloseAnimation = Tooltip.AnimationType.Fade;
			uISettings.BlockTouches = false;
			if (!p_card.Card.IsShowAsOwned)
			{
				uISettings.OnDismiss = p_card.PlayNoCardAnimationOnTooltipClose;
				if (!Manager.IsRun)
				{
					uISettings.ButtonData = new DialogButtonData(DialogCallbacks.MissionTooltip_CloseAndGoToPaymentBoosterpacks, "^GUI.Buttons.BuyUpgradeKits^", ButtonUI.Type.Green);
				}
				uISettings.Title = "^GUI.Labels.NoCard^";
			}
			DialogNotificationManager.ShowTooltip(uISettings);
		}

		public void CompleteAnimation(float p_starDuration, float p_fadeinDuration, float p_pointsDuration)
		{
			Sequence sequence = DOTween.Sequence();
			for (int i = 0; i < _Stars.Count; i++)
			{
				int starIndex = i;
				_Stars[starIndex].FillDuration = p_starDuration / (float)_Stars.Count;
				sequence.AppendCallback(delegate
				{
					_Stars[starIndex].SetCompleteStarIcon(1f);
					AudioManager.PlaySound("star");
				});
				sequence.AppendInterval(p_starDuration / (float)_Stars.Count);
			}
			sequence.Append(_MissionPointsRewardGroupCanvas.DOFade(1f, p_fadeinDuration));
			sequence.Join(_RightBlockCanvas.DOFade(1f, p_fadeinDuration));
			sequence.Append(DOTween.To(() => 0, delegate(int x)
			{
				_PointsAmount.text = x.ToString();
			}, _Item.RewardAmount, p_pointsDuration));
			sequence.Play();
		}

		public void NotCompleteAnimation(float p_duration)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				AudioManager.PlaySound("red_button");
			});
			sequence.Join(_RightBlockCanvas.DOFade(1f, p_duration));
			sequence.Play();
		}
	}
}
