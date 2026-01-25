using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class ProgressBarUI : MonoBehaviour
	{
		private const float pixelsTotal = 175f;

		private const float pixelsForFinishedLevel = 10f;

		private const float pixelsForDelimiter = 8f;

		[SerializeField]
		private ProgressBarSegmentUI _SegmentPrefab;

		[SerializeField]
		private RectTransform _SegmentsAnchor;

		[SerializeField]
		private Text _ValueNumbers;

		[SerializeField]
		private VerticalLayoutGroup _LayoutGroup;

		[SerializeField]
		private ProgressBarSegmentUI _CardProgress;

		[SerializeField]
		private GameObject _BoostTimerRoot;

		[SerializeField]
		private ResolutionImage _BoostTimerCircle;

		[SerializeField]
		private ProgressBarUIParameters _Parameters;

		private int _CurrentLevel;

		private int _MaxLevel;

		private int _BoostedLevel;

		private int _LevelsUp;

		private int _CurrentProgress;

		private int _MaxProgress;

		private int _AnimatedProgress;

		private List<ProgressBarSegmentUI> _Segments = new List<ProgressBarSegmentUI>();

		public ProgressBarUIParameters Parameters
		{
			get
			{
				return _Parameters;
			}
			set
			{
				if (_Parameters != value && _Parameters != null)
				{
					_Parameters = value;
					Refresh();
				}
			}
		}

		public bool ShowAnimation
		{
			get
			{
				return _Parameters.ShowAnimation;
			}
			set
			{
				if (_Parameters.ShowAnimation != value)
				{
					_Parameters.ShowAnimation = value;
					Refresh();
				}
			}
		}

		public void SetParameters(ProgressBarUIParameters p_parameters)
		{
			_Parameters = p_parameters;
			Refresh();
			if (_Parameters.ShowNumbers && _BoostedLevel > 0)
			{
				Debug.Log("StartRoutine ");
				if (base.isActiveAndEnabled)
				{
					StopCoroutine(ObserveBoostTimer());
					StartCoroutine(ObserveBoostTimer());
				}
			}
		}

		public void OnEnable()
		{
			if (_Parameters.ShowNumbers && _BoostedLevel > 0)
			{
				Debug.Log("StartRoutine OnEnable");
				StopCoroutine(ObserveBoostTimer());
				StartCoroutine(ObserveBoostTimer());
			}
		}

		public void Refresh()
		{
			StopAnim();
			SetupUI();
			UpdateProgress();
			UpdateUI();
		}

		private void SetupUI()
		{
			_MaxLevel = _Parameters.Card.CardMaxLevel;
			_MaxProgress = _Parameters.Card.CardDeltaByLevel(Math.Min(_MaxLevel, _CurrentLevel + 1));
			_LayoutGroup.spacing = 8f;
		}

		private void UpdateProgress()
		{
			_BoostedLevel = _Parameters.Card.UserCardBoostLevel;
			_LevelsUp = 1;
			if (_Parameters.ShowProgress)
			{
				_CardProgress.gameObject.SetActive(true);
			}
			else
			{
				_CardProgress.gameObject.SetActive(false);
			}
			if (_Parameters.ShowAnimation && _Parameters.AnimationCardCount < 0)
			{
				_CurrentProgress = _Parameters.Card.UserCardsSinceLastLevel;
				_CurrentLevel = _Parameters.Card.UserCardTotalLevel;
				if (_CurrentProgress < Math.Abs(_Parameters.AnimationCardCount) && _CurrentLevel > 0)
				{
					_CurrentProgress = _Parameters.Card.GetCardsSinceLastLevelForProgress(_Parameters.Card.UserCardProgress + _Parameters.AnimationCardCount);
					_CurrentLevel--;
					_AnimatedProgress = _Parameters.Card.CardDeltaByLevel(_CurrentLevel + 1);
					_MaxProgress = _Parameters.Card.CardDeltaByLevel(_CurrentLevel + 1);
					_LevelsUp = _Parameters.Card.GetLevelsUp();
				}
				else
				{
					_AnimatedProgress = _CurrentProgress;
					_CurrentProgress += _Parameters.AnimationCardCount;
					_MaxProgress = _Parameters.Card.CardDeltaByLevel(Math.Min(_MaxLevel, _CurrentLevel + 1));
				}
			}
			else
			{
				_CurrentLevel = _Parameters.Card.UserCardTotalLevel;
				_MaxProgress = _Parameters.Card.CardDeltaByLevel(Math.Min(_MaxLevel, _CurrentLevel + 1));
				if (_CurrentLevel != _MaxLevel)
				{
					_CurrentProgress = _Parameters.Card.UserCardsSinceLastLevel;
					_AnimatedProgress = _CurrentProgress + _Parameters.AnimationCardCount;
				}
				else
				{
					_CurrentProgress = _MaxProgress;
					_AnimatedProgress = _MaxProgress;
				}
				_LevelsUp = _Parameters.Card.GetLevelsUpForProgress(_Parameters.Card.UserCardProgress + _Parameters.AnimationCardCount);
			}
		}

		private void UpdateUI()
		{
			SetActive(true);
			UpdateProgressBar();
			UpdateLevels();
			UpdateValueNumbers();
			RefreshBoostTimer();
		}

		private void StopAnim()
		{
			foreach (ProgressBarSegmentUI segment in _Segments)
			{
				segment.StopAnim();
			}
		}

		private void UpdateValueNumbers()
		{
			_ValueNumbers.enabled = _Parameters.ShowNumbers;
			if (_Parameters.ShowNumbers)
			{
				_ValueNumbers.text = string.Format("{0}/{1}", _Parameters.Card.UserCardProgress, _Parameters.Card.UserCardsToNextLevelFromZero);
			}
		}

		private void UpdateProgressBar()
		{
			ProgressBarSegmentUIParameters progressBarSegmentUIParameters = new ProgressBarSegmentUIParameters();
			progressBarSegmentUIParameters.BackgroundColor = _Parameters.BackgroundColor;
			progressBarSegmentUIParameters.ProgressColor = _Parameters.ProgressColor;
			progressBarSegmentUIParameters.LineWidth = _Parameters.ProgressWidth;
			progressBarSegmentUIParameters.FillType = ProgressBarSegmentFillType.UseProgress;
			progressBarSegmentUIParameters.ShowAnimation = _Parameters.ShowAnimation;
			progressBarSegmentUIParameters.MaxProgress = _MaxProgress;
			progressBarSegmentUIParameters.AnimatedProgress = Math.Min(_AnimatedProgress, _MaxProgress);
			progressBarSegmentUIParameters.LineHeight = 175;
			_CardProgress.SetParametersAndProgress(progressBarSegmentUIParameters, _CurrentProgress);
		}

		private void UpdateLevels()
		{
			if (_Parameters.ShowProgress)
			{
				_SegmentsAnchor.localPosition.Set(20f, _SegmentsAnchor.localPosition.y, _SegmentsAnchor.localPosition.z);
				_SegmentsAnchor.DOLocalMoveX(20f, 0f);
			}
			else
			{
				_SegmentsAnchor.localPosition.Set(0f, _SegmentsAnchor.localPosition.y, _SegmentsAnchor.localPosition.z);
				_SegmentsAnchor.DOLocalMoveX(0f, 0f);
			}
			if (_SegmentsAnchor.childCount != 0)
			{
				List<GameObject> list = new List<GameObject>();
				foreach (Transform item in _SegmentsAnchor)
				{
					list.Add(item.gameObject);
				}
				list.ForEach(delegate(GameObject child)
				{
					UnityEngine.Object.Destroy(child);
				});
			}
			int num = _BoostedLevel;
			int num2 = 0;
			if (_Parameters.ShowAnimation && _Parameters.Card.UserCardIsLevelUp)
			{
				num2 = _LevelsUp;
			}
			int num3 = Math.Min(_MaxLevel, _CurrentLevel + num2);
			for (int i = 0; i < num3; i++)
			{
				ProgressBarSegmentUI progressBarSegmentUI = UnityEngine.Object.Instantiate(_SegmentPrefab);
				progressBarSegmentUI.transform.SetParent(_SegmentsAnchor, false);
				ProgressBarSegmentUIParameters progressBarSegmentUIParameters = new ProgressBarSegmentUIParameters();
				progressBarSegmentUIParameters.BackgroundColor = new Color(0f, 0f, 0f, 0f);
				progressBarSegmentUIParameters.ProgressColor = _Parameters.ProgressColor;
				progressBarSegmentUIParameters.LineWidth = _Parameters.LevelWidth;
				if (num > 0)
				{
					progressBarSegmentUIParameters.FillType = ProgressBarSegmentFillType.Boosted;
					num--;
					progressBarSegmentUIParameters.MaxProgress = 1;
					progressBarSegmentUIParameters.LineHeight = 10;
					progressBarSegmentUIParameters.BoostGlowSpriteName = GetBoostGlowName();
				}
				else if (num2 > 0)
				{
					progressBarSegmentUIParameters.FillType = ProgressBarSegmentFillType.Filled;
					num2--;
					progressBarSegmentUIParameters.MaxProgress = 1;
					progressBarSegmentUIParameters.LineHeight = 10;
					progressBarSegmentUIParameters.ShowAnimation = true;
				}
				else if (num3 > i)
				{
					progressBarSegmentUIParameters.FillType = ProgressBarSegmentFillType.Filled;
					progressBarSegmentUIParameters.MaxProgress = 1;
					progressBarSegmentUIParameters.LineHeight = 10;
				}
				progressBarSegmentUI.SetParametersAndProgress(progressBarSegmentUIParameters, 0);
			}
		}

		private IEnumerator ObserveBoostTimer()
		{
			while (_Parameters.Card.UserCardBoostLevel > 0)
			{
				yield return new WaitForSeconds(1f);
				RefreshBoostTimer();
			}
		}

		public void RefreshBoostTimer()
		{
			if (_Parameters.ShowNumbers && _BoostedLevel > 0)
			{
				_BoostTimerRoot.SetActive(true);
				string p_timerName = CardsManager.BoostCardTimerPrefix + _Parameters.Card.CardName;
				float timerValueInSeconds = TimersManager.GetTimerValueInSeconds(p_timerName);
				_BoostTimerCircle.fillAmount = timerValueInSeconds / CardsManager.BoostTime;
			}
			else if (_BoostTimerRoot != null)
			{
				_BoostTimerRoot.SetActive(false);
			}
		}

		private void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}

		private string GetBoostGlowName()
		{
			switch (_Parameters.Card.CardRarity)
			{
			case 1:
				return "cards.boost_indicator_blue";
			case 2:
				return "cards.boost_indicator_orange";
			case 3:
				return "cards.boost_indicator_red";
			default:
				return string.Empty;
			}
		}
	}
}
