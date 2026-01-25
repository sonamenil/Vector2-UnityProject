using System;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI.Common.Effects;
using Nekki.Vector.GUI.Scenes.Shop;
using UIFigures;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class BaseCardUI : MonoBehaviour
	{
		private const float _NoCardBGOpacity = 0.4f;

		private const float _DefaultBGOpacity = 0.65f;

		private const string _NoCardsIcon = "cards.questionIcon";

		private const string _PlusIcon = "cards.plus";

		private const float _NoCardAnimationDuration = 0.2f;

		[SerializeField]
		private BaseCardUISettings _UISettings = new BaseCardUISettings();

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private CanvasGroup _CanvasGroup;

		[SerializeField]
		private LayoutElement _LayoutElement;

		[SerializeField]
		private UIPoligon _Background;

		[SerializeField]
		private ResolutionImage _Effect;

		[SerializeField]
		private LabelAlias _NoteId;

		[SerializeField]
		private LabelAlias _StoryCount;

		[SerializeField]
		private UILine _Outline;

		[SerializeField]
		private BaseCardUISlot _Slot;

		[SerializeField]
		private ProgressBarUI _ProgressBar;

		[SerializeField]
		private GameObject _NewAnnounce;

		[SerializeField]
		private GameObject _ForMissionIcon;

		[SerializeField]
		private CanvasGroup _LevelUpEffect;

		[SerializeField]
		private CanvasGroup _NoCardEffect;

		[SerializeField]
		private ParticlesSpawn _ParticlesSpawn;

		[SerializeField]
		private ParticlesSpawn _LvlUpParticlesSpawn;

		private static Color _BlueInactiveColor = ColorUtils.FromHex("5EA4BB");

		private static Color _BlueActiveColor = ColorUtils.FromHex("AEF2FC");

		private static Color _OrangeInactiveColor = ColorUtils.FromHex("DA6C0E");

		private static Color _OrangeActiveColor = ColorUtils.FromHex("ED9B15");

		private static Color _RedInactiveColor = ColorUtils.FromHex("C11527");

		private static Color _RedActiveColor = ColorUtils.FromHex("E6252C");

		private static Color _NoCardColor = ColorUtils.FromHex("7D85937F");

		private static Color _ProgressBarBlueBGColor = ColorUtils.FromHex("3B697E");

		private static Color _ProgressBarBlueLineColor = ColorUtils.FromHex("AEF2FC");

		private static Color _ProgressBarOrangeBGColor = ColorUtils.FromHex("905028");

		private static Color _ProgressBarOrangeLineColor = ColorUtils.FromHex("ED9B15");

		private static Color _ProgressBarRedBGColor = ColorUtils.FromHex("812C40");

		private static Color _ProgressBarRedLineColor = ColorUtils.FromHex("F5363F");

		private static Color _LevelUpColor = ColorUtils.FromHex("35b546");

		private Sequence _LevelUpAnimation;

		private Action<BaseCardUI> _OnTap;

		private CardsGroupAttribute _Card;

		private bool _IsEnabled = true;

		public float TapZoneSize
		{
			get
			{
				return GetComponent<RectTransform>().sizeDelta.x;
			}
		}

		public Action<BaseCardUI> OnTap
		{
			get
			{
				return _OnTap;
			}
			set
			{
				_OnTap = value;
			}
		}

		public CardsGroupAttribute Card
		{
			get
			{
				return _Card;
			}
			set
			{
				_Card = value;
				Refresh();
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				if (_IsEnabled != value)
				{
					_IsEnabled = value;
					SetEnabled();
				}
			}
		}

		public CanvasGroup CanvasGroup
		{
			get
			{
				return _CanvasGroup;
			}
		}

		public float CanvasGroupAlpha
		{
			get
			{
				return _CanvasGroup.alpha;
			}
			set
			{
				_CanvasGroup.alpha = value;
			}
		}

		public LayoutElement LayoutElement
		{
			get
			{
				return _LayoutElement;
			}
		}

		public BaseCardUISettings UISettings
		{
			get
			{
				return _UISettings;
			}
			set
			{
				if (value != null && _UISettings != value)
				{
					_UISettings = value;
					Refresh();
				}
			}
		}

		public int CardSize
		{
			get
			{
				return _UISettings.CardSize;
			}
			set
			{
				if (_UISettings.CardSize != value)
				{
					_UISettings.CardSize = value;
					SetSize();
				}
			}
		}

		public bool NeedShowSlot
		{
			get
			{
				return _UISettings.NeedShowSlot;
			}
			set
			{
				if (_UISettings.NeedShowSlot != value)
				{
					_UISettings.NeedShowSlot = value;
					SetSlot();
				}
			}
		}

		public bool NeedShowCurrentLevelProgress
		{
			get
			{
				return _UISettings.NeedShowCurrentLevelProgress;
			}
			set
			{
				if (_UISettings.NeedShowCurrentLevelProgress != value)
				{
					_UISettings.NeedShowCurrentLevelProgress = value;
					SetProgress();
				}
			}
		}

		public bool NeedShowProgressBar
		{
			get
			{
				return _UISettings.NeedShowProgressBar;
			}
			set
			{
				if (_UISettings.NeedShowProgressBar != value)
				{
					_UISettings.NeedShowProgressBar = value;
					SetProgress();
				}
			}
		}

		public bool NeedShowProgressNumbers
		{
			get
			{
				return _UISettings.NeedShowProgressNumbers;
			}
			set
			{
				if (_UISettings.NeedShowProgressNumbers != value)
				{
					_UISettings.NeedShowProgressNumbers = value;
					SetProgress();
				}
			}
		}

		public bool NeedShowProgressAnimation
		{
			get
			{
				return _UISettings.NeedShowProgressAnimation;
			}
			set
			{
				if (_UISettings.NeedShowProgressAnimation != value)
				{
					_UISettings.NeedShowProgressAnimation = value;
					SetProgress();
				}
			}
		}

		public int AnimationCardCount
		{
			get
			{
				return _UISettings.AnimationCardCount;
			}
			set
			{
				if (_UISettings.AnimationCardCount != value)
				{
					_UISettings.AnimationCardCount = value;
					SetProgress();
				}
			}
		}

		public bool NeedShowNoCardIcon
		{
			get
			{
				return _UISettings.NeedShowNoCardIcon;
			}
			set
			{
				if (_UISettings.NeedShowNoCardIcon != value)
				{
					_UISettings.NeedShowNoCardIcon = value;
					SetEffect();
				}
			}
		}

		public bool NeedShowPlusIcon
		{
			get
			{
				return _UISettings.NeedShowPlusIcon;
			}
			set
			{
				if (_UISettings.NeedShowPlusIcon != value)
				{
					_UISettings.NeedShowPlusIcon = value;
					SetEffect();
				}
			}
		}

		public float NoCardBorderOpacity
		{
			get
			{
				return _UISettings.NoCardBorderOpacity;
			}
			set
			{
				if (_UISettings.NoCardBorderOpacity != value)
				{
					_UISettings.NoCardBorderOpacity = value;
					if (_Card == null)
					{
						SetRarity();
					}
				}
			}
		}

		public int SlotOffset
		{
			get
			{
				return _UISettings.SlotOffset;
			}
			set
			{
				if (_UISettings.SlotOffset != value)
				{
					_UISettings.SlotOffset = value;
					SetSlot();
				}
			}
		}

		public bool NeedAnnounce
		{
			get
			{
				return _UISettings.NeedAnnounce;
			}
			set
			{
				if (_UISettings.NeedAnnounce != value)
				{
					_UISettings.NeedAnnounce = value;
					SetAnnounce();
				}
			}
		}

		public bool NeedShowForMissionIcon
		{
			get
			{
				return _UISettings.NeedShowForMissionIcon;
			}
			set
			{
				if (_UISettings.NeedShowForMissionIcon != value)
				{
					_UISettings.NeedShowForMissionIcon = value;
					SetForMissionIcon();
				}
			}
		}

		public bool NeedShowLevelUpAnimation
		{
			get
			{
				return _UISettings.NeedShowLevelUpAnimation;
			}
			set
			{
				if (_UISettings.NeedShowLevelUpAnimation != value)
				{
					_UISettings.NeedShowLevelUpAnimation = value;
					SetLevelUpAnimation();
				}
			}
		}

		public bool NeedShowNoCardAnimation
		{
			get
			{
				return _UISettings.NeedShowNoCardAnimation;
			}
			set
			{
				if (_UISettings.NeedShowNoCardAnimation != value)
				{
					_UISettings.NeedShowNoCardAnimation = value;
					SetNoCardEffect();
				}
			}
		}

		public bool NeedShowStoryCount
		{
			get
			{
				return _UISettings.NeedShowStoryCount;
			}
			set
			{
				if (_UISettings.NeedShowStoryCount != value)
				{
					_UISettings.NeedShowStoryCount = value;
					SetEffect();
				}
			}
		}

		private Color ActiveColor
		{
			get
			{
				if (_Card == null)
				{
					return _NoCardColor;
				}
				switch (_Card.CardRarity)
				{
				case 1:
					return _BlueActiveColor;
				case 2:
					return _OrangeActiveColor;
				case 3:
					return _RedActiveColor;
				default:
					return _NoCardColor;
				}
			}
		}

		private Color InactiveColor
		{
			get
			{
				if (_Card == null)
				{
					return _NoCardColor;
				}
				switch (_Card.CardRarity)
				{
				case 1:
					return _BlueInactiveColor;
				case 2:
					return _OrangeInactiveColor;
				case 3:
					return _RedInactiveColor;
				default:
					return _NoCardColor;
				}
			}
		}

		private Color ProgressBarLineColor
		{
			get
			{
				if (_Card == null)
				{
					return _NoCardColor;
				}
				switch (_Card.CardRarity)
				{
				case 1:
					return _ProgressBarBlueLineColor;
				case 2:
					return _ProgressBarOrangeLineColor;
				case 3:
					return _ProgressBarRedLineColor;
				default:
					return _NoCardColor;
				}
			}
		}

		private Color ProgressBarBGColor
		{
			get
			{
				if (_Card == null)
				{
					return _NoCardColor;
				}
				switch (_Card.CardRarity)
				{
				case 1:
					return _ProgressBarBlueBGColor;
				case 2:
					return _ProgressBarOrangeBGColor;
				case 3:
					return _ProgressBarRedBGColor;
				default:
					return _NoCardColor;
				}
			}
		}

		public bool IsNew
		{
			get
			{
				return _Card != null && _Card.IsNew;
			}
			set
			{
				if (_Card != null)
				{
					_Card.IsNew = value;
				}
				SetAnnounce();
			}
		}

		public void Init(BaseCardUISettings p_UISettings, Action<BaseCardUI> p_onTap = null, CardsGroupAttribute p_card = null, bool p_isEnabled = true)
		{
			_UISettings = p_UISettings;
			_OnTap = p_onTap;
			_Card = p_card;
			_IsEnabled = p_isEnabled;
			Refresh();
			PlaySpawnAnimation();
		}

		public void Refresh()
		{
			SetSize();
			SetEffect();
			SetRarity();
			SetSlot();
			SetProgress();
			SetAnnounce();
			SetForMissionIcon();
			SetLevelUpAnimation();
			SetNoCardEffect();
			SetEnabled();
		}

		public void RefreshBoost()
		{
			SetProgress();
		}

		private void SetSize()
		{
			float num = (float)CardSize / 175f;
			base.transform.localScale = new Vector3(num, num, 1f);
			if (_LayoutElement != null)
			{
				LayoutElement layoutElement = _LayoutElement;
				float num2 = CardSize;
				_LayoutElement.preferredHeight = num2;
				layoutElement.preferredWidth = num2;
			}
		}

		private void SetEffect()
		{
			bool flag = _Card != null;
			_Effect.enabled = true;
			_NoteId.enabled = false;
			_StoryCount.enabled = false;
			if (flag)
			{
				_Effect.SpriteName = _Card.CardImage;
				if (_Card.CardType == CardType.Notes)
				{
					_NoteId.SetAlias(Card.CardNoteId);
					_NoteId.enabled = true;
				}
				else if (_Card.CardType == CardType.StoryItems && NeedShowStoryCount)
				{
					_StoryCount.SetAlias(Card.CurrentCardProgress.ToString());
					_StoryCount.enabled = true;
				}
			}
			else if (NeedShowPlusIcon)
			{
				_Effect.SpriteName = "cards.plus";
			}
			else if (NeedShowNoCardIcon)
			{
				_Effect.SpriteName = "cards.questionIcon";
			}
			else
			{
				_Effect.enabled = false;
			}
			_Background.color = new Color(_Background.color.r, _Background.color.g, _Background.color.b, (!flag) ? 0.4f : 0.65f);
		}

		private void SetRarity()
		{
			if (!NeedShowNoCardAnimation || _Card == null || _Card.IsShowAsOwned)
			{
				ChangeColorToInactive(0f);
			}
		}

		private void SetSlot()
		{
			if (!NeedShowSlot || _Card == null || !_Card.HasSlotIcon)
			{
				_Slot.gameObject.SetActive(false);
				return;
			}
			_Slot.gameObject.SetActive(true);
			_Slot.Init(_Card.CardSlotImage);
			_Slot.SetContentColor(InactiveColor);
			_Slot.transform.localPosition = new Vector3(136 + SlotOffset, 0f, _Slot.transform.localPosition.z);
		}

		private void SetProgress()
		{
			if (!NeedShowProgressBar || _Card == null || (Card.UserCardTotalLevel == 0 && (Card.UserCardProgress == 0 || Card.UserCardProgress == -AnimationCardCount)))
			{
				_ProgressBar.gameObject.SetActive(false);
				return;
			}
			_ProgressBar.gameObject.SetActive(true);
			ProgressBarUIParameters progressBarUIParameters = new ProgressBarUIParameters();
			progressBarUIParameters.BackgroundColor = ProgressBarBGColor;
			progressBarUIParameters.ProgressColor = ProgressBarLineColor;
			progressBarUIParameters.ShowAnimation = NeedShowProgressAnimation;
			progressBarUIParameters.ShowProgress = NeedShowCurrentLevelProgress;
			progressBarUIParameters.AnimationCardCount = AnimationCardCount;
			progressBarUIParameters.Card = Card;
			progressBarUIParameters.ShowNumbers = NeedShowProgressNumbers;
			progressBarUIParameters.ProgressWidth = 6;
			progressBarUIParameters.LevelWidth = 10;
			_ProgressBar.SetParameters(progressBarUIParameters);
		}

		private void SetAnnounce()
		{
			_NewAnnounce.SetActive(NeedAnnounce && _Card != null && _Card.IsNew);
		}

		private void SetForMissionIcon()
		{
			_ForMissionIcon.SetActive(NeedShowForMissionIcon && _Card != null && _Card.IsNeedForMission && Card.IsAvailableInShop);
		}

		private void SetLevelUpAnimation()
		{
			if ((!NeedShowLevelUpAnimation || (_Card != null && !_Card.IsLevelUp)) && _LevelUpAnimation != null)
			{
				StopLevelUpAnimation();
			}
			else if (NeedShowLevelUpAnimation && _Card != null && _Card.IsLevelUp && _LevelUpAnimation == null)
			{
				PlayLevelUpAnimation();
			}
		}

		private void SetEnabled()
		{
			_CanvasGroup.alpha = ((!_IsEnabled) ? 0.5f : 1f);
		}

		private void SetNoCardEffect()
		{
			if (NeedShowNoCardAnimation && _Card != null && !_Card.IsShowAsOwned)
			{
				_NoCardEffect.alpha = 1f;
				_Effect.color = _NoCardColor;
				_Outline.color = _NoCardColor;
			}
		}

		private void StopLevelUpAnimation()
		{
			if (_LevelUpAnimation != null)
			{
				_LevelUpAnimation.Kill(true);
				_Effect.Alpha = 1f;
				_LevelUpEffect.alpha = 0f;
				ChangeColorToActive(0f);
				_LevelUpAnimation = null;
			}
		}

		private void PlayLevelUpAnimation()
		{
			if (_LevelUpAnimation != null)
			{
				_LevelUpAnimation.Kill(true);
			}
			_LevelUpAnimation = DOTween.Sequence();
			_LevelUpAnimation.Join(_Effect.DOFade(0f, 1.2f)).SetEase(Ease.OutCubic);
			_LevelUpAnimation.Join(_LevelUpEffect.DOFade(1f, 1.2f)).SetEase(Ease.OutCubic);
			_LevelUpAnimation.Join(_Outline.DOColor(_LevelUpColor, 1.2f)).SetEase(Ease.OutCubic);
			_LevelUpAnimation.SetLoops(-1, LoopType.Yoyo);
			_LevelUpAnimation.Play();
		}

		public void PlayLevelUpFinishAnimation()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				_LvlUpParticlesSpawn.Spawn();
			});
			sequence.Join(_Content.DOShakePosition(0.5f, 30f, 20));
			sequence.Play();
		}

		public void PlayNoCardAnimationOnTooltipClose()
		{
			if (NeedShowNoCardAnimation && _Card != null && !_Card.IsShowAsOwned)
			{
				_Effect.DOColor(_NoCardColor, 0.2f);
				_Outline.DOColor(_NoCardColor, 0.2f);
			}
		}

		private void PlaySpawnAnimation()
		{
			if (Card != null && Card.CardName == ShopPanel.EquippedCardName)
			{
				float duration = 0.5f;
				float num = 1.35f;
				RectTransform component = GetComponent<RectTransform>();
				float x = component.localScale.x;
				Color[] effectcolor = new Color[4];
				switch (Card.CardRarity)
				{
				case 1:
					effectcolor = new Color[4]
					{
						ColorUtils.FromHex("54AAFF6D"),
						ColorUtils.FromHex("BFEFFFA7"),
						ColorUtils.FromHex("BFEFFFA7"),
						ColorUtils.FromHex("BFEFFF52")
					};
					break;
				case 2:
					effectcolor = new Color[4]
					{
						ColorUtils.FromHex("FF89546D"),
						ColorUtils.FromHex("FF7638A7"),
						ColorUtils.FromHex("FF7638A7"),
						ColorUtils.FromHex("FF763852")
					};
					break;
				case 3:
					effectcolor = new Color[4]
					{
						ColorUtils.FromHex("FF3E3E6D"),
						ColorUtils.FromHex("FF2E2EA7"),
						ColorUtils.FromHex("FF0D0DA7"),
						ColorUtils.FromHex("FF0D0D52")
					};
					break;
				}
				Sequence sequence = DOTween.Sequence();
				sequence.AppendCallback(delegate
				{
					_ParticlesSpawn.Spawn();
					_ParticlesSpawn.GetComponentInChildren<CardEffect>().ChangeParticleStartColor(effectcolor);
				});
				sequence.Append(component.DOScale(x * num, 1E-05f));
				sequence.Append(component.DOScale(x, duration));
				sequence.Play();
				ShopPanel.EquippedCardName = null;
			}
		}

		public void ChangeColorToActive(float p_duration = 0f)
		{
			if (_Card == null)
			{
				_Effect.color = _NoCardColor;
				_NoteId.color = _NoCardColor;
				_StoryCount.color = _NoCardColor;
				_Outline.color = _NoCardColor;
			}
			else
			{
				UpdateColor(ActiveColor, p_duration);
			}
		}

		public void ChangeColorToInactive(float p_duration = 0f)
		{
			if (_Card == null)
			{
				_Effect.color = new Color(_NoCardColor.r, _NoCardColor.g, _NoCardColor.b, NoCardBorderOpacity);
				_Outline.color = new Color(_NoCardColor.r, _NoCardColor.g, _NoCardColor.b, NoCardBorderOpacity);
				_NoteId.color = _NoCardColor;
				_StoryCount.color = _NoCardColor;
			}
			else
			{
				UpdateColor(InactiveColor, p_duration);
			}
		}

		private void UpdateColor(Color p_color, float p_duration)
		{
			_Slot.SetContentColor(p_color, p_duration);
			if (p_duration > 0f)
			{
				_Effect.DOColor(p_color, p_duration);
				_NoteId.DOColor(p_color, p_duration);
				_StoryCount.DOColor(p_color, p_duration);
				_Outline.DOColor(p_color, p_duration);
			}
			else
			{
				_Effect.color = p_color;
				_NoteId.color = p_color;
				_StoryCount.color = p_color;
				_Outline.color = p_color;
			}
		}

		public void MoveContentTo(float p_x, float p_time = 0f, bool p_canGoLeft = true, bool p_canGoRight = true, float p_proportionRefDistance = 0f)
		{
			if ((p_canGoLeft || _Content.localPosition.x - p_x <= 0f) && (p_canGoRight || _Content.localPosition.x - p_x >= 0f))
			{
				if (p_time == 0f)
				{
					_Content.DOKill();
					_Content.localPosition = new Vector3(p_x, 0f, 0f);
				}
				else if (p_proportionRefDistance != 0f)
				{
					_Content.DOLocalMoveX(p_x, p_time * Mathf.Abs(_Content.localPosition.x - p_x) / p_proportionRefDistance);
				}
				else
				{
					_Content.DOLocalMoveX(p_x, p_time);
				}
			}
		}

		public void OnCardTap()
		{
			if (NeedShowNoCardAnimation && _Card != null && !_Card.IsShowAsOwned)
			{
				ChangeColorToInactive(0.2f);
			}
			if (_IsEnabled && _OnTap != null)
			{
				_OnTap(this);
			}
		}

		public void CardAlphaChange(bool p_alphadown, float p_duration)
		{
			_CanvasGroup.DOFade((!p_alphadown) ? 1f : 0f, p_duration);
		}

		public void CardAppearEndfloor()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_Slot.transform.DOLocalMove(new Vector3(0f, -130f), 0f));
			sequence.Append(_Content.DOScale(1.3f, 0.001f));
			sequence.Append(_CanvasGroup.DOFade(1f, 0.2f));
			sequence.Join(_Content.DOScale(1f, 0.2f));
			sequence.Play();
		}

		public Tweener GetFadeTween(bool p_alphadown, float p_duration, bool fromzero = false)
		{
			return _CanvasGroup.DOFade((!p_alphadown) ? 1f : 0f, p_duration);
		}

		public Tweener GetShiftTween(float delta, float p_duration)
		{
			return _Content.DOLocalMoveX(_Content.localPosition.x + delta, p_duration);
		}

		private void Awake()
		{
			_NoCardEffect.alpha = 0f;
		}
	}
}
