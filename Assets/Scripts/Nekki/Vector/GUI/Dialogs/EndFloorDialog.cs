using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.MainScene;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public class EndFloorDialog : BaseDialog
	{
		[SerializeField]
		private GadgetUIPanel _GadgetPanel;

		[SerializeField]
		private Text _Rank;

		[SerializeField]
		private RankProgressBarTweener _RankProgressBar;

		[SerializeField]
		private CardsPanel _CardsLayout;

		[SerializeField]
		private Text _Money2Earned;

		[SerializeField]
		private LabelAlias _TapAnywhere;

		[Header("Records")]
		[SerializeField]
		private CanvasGroup _RecordsCanvasGroup;

		[SerializeField]
		private Image _FloorPointsIcon;

		[SerializeField]
		private LabelAlias _FloorTitle;

		[SerializeField]
		private Text _FloorPoints;

		[SerializeField]
		private Image _StuntPointsIcon;

		[SerializeField]
		private LabelAlias _StuntsTitle;

		[SerializeField]
		private Text _StuntsPoints;

		[SerializeField]
		private ResolutionImage _Flash;

		[SerializeField]
		[Header("Missions")]
		private MissionItemUI _MissionTab;

		private int _RankShowed;

		private int _TargetRank;

		private int _TargetPoints;

		private System.Action _OnClose;

		private bool _TappedFirstTime;

		private bool _ScoreSeqStop;

		private float _BarRotateTime = 0.75f;

		private Sequence _MainSequence;

		public void Init(System.Action p_onClose)
		{
			_OnClose = p_onClose;
			_GadgetPanel.Init(EndFloorManager.ItemsBeforeCharge, null);
			_RankShowed = RankManager.RankOnFloorStart;
			_TargetPoints = EndFloorManager.StartMoney1;
			RunMainSequence();
			EndFloorManager.ChargesRefreshAllowed = true;
			LabelAlias floorTitle = _FloorTitle;
			floorTitle.text = floorTitle.text + " " + (((int)CounterController.Current.CounterFloor <= 9) ? "0" : string.Empty) + CounterController.Current.CounterFloor.ToString();
			LabelAlias stuntsTitle = _StuntsTitle;
			string text = stuntsTitle.text;
			stuntsTitle.text = text + " " + CounterController.Current.CounterStuntsExecuted.ToString() + "/" + CounterController.Current.CounterStuntsGenerated.ToString();
			_FloorPoints.text = CounterController.Current.CounterPointsForFloor.ToString();
			_StuntsPoints.text = CounterController.Current.CounterPointsForStunts.ToString();
			_Money2Earned.text = ((int)DataLocal.Current.Money2 - EndFloorManager.StartMoney2).ToString();
			_Flash.gameObject.SetActive(false);
		}

		public void OnDialogTap()
		{
			if (_TappedFirstTime)
			{
				if (_OnClose != null)
				{
					_OnClose();
				}
				Dismiss();
				return;
			}
			_MainSequence.Complete();
			_MissionTab.gameObject.SetActive(false);
			_RecordsCanvasGroup.alpha = 1f;
			ScoreToZero(0.1f);
			OnFinalRank();
			_RankProgressBar.Stop(false);
			_RankProgressBar.SetArc(0f, (float)Math.PI * 2f * RankManager.CurrentPointsPart());
			if (RankManager.Rank > RankManager.RankOnFloorStart)
			{
				_Rank.text = RankManager.Rank.ToString();
				_GadgetPanel.Init(DataLocalHelper.GetUserGadgets(), null);
				for (int i = _RankShowed + 1; i <= RankManager.Rank; i++)
				{
					ShowCards(i);
				}
			}
		}

		private void RunMainSequence()
		{
			_MainSequence = DOTween.Sequence();
			InitiateRank(RankManager.PointsPartAtFloorStart());
			ShowMissions();
			_MainSequence.AppendCallback(delegate
			{
				AudioManager.PlaySound("mission_tab");
			});
			_MainSequence.Append(_RecordsCanvasGroup.DOFade(1f, 0.5f));
			_MainSequence.AppendCallback(delegate
			{
				RecordsAnim();
			});
			_MainSequence.AppendInterval(0.2f);
			_MainSequence.AppendCallback(delegate
			{
				OnFinalRank();
			});
			_MainSequence.Play();
		}

		private void InitiateRank(float p_arc_from)
		{
			Text component = _Rank.GetComponent<Text>();
			component.text = _RankShowed.ToString();
			_RankProgressBar.Stop(false);
			_RankProgressBar.SetArc(0f, (float)Math.PI * 2f * p_arc_from);
		}

		private void ShowMissions()
		{
			CanvasGroup component = _MissionTab.GetComponent<CanvasGroup>();
			component.alpha = 0f;
			if (!MissionsManager.IsMissionsEnabled)
			{
				return;
			}
			foreach (MissionItem prevMissionItem in MissionsManager.PrevMissionItems)
			{
				ShowMissionTab(prevMissionItem, component);
			}
		}

		private void ShowMissionTab(MissionItem p_mission, CanvasGroup p_canvas)
		{
			_MainSequence.AppendCallback(delegate
			{
				_MissionTab.Init(p_mission, true);
			});
			_MainSequence.AppendCallback(delegate
			{
				AudioManager.PlaySound("mission_tab");
			});
			_MainSequence.Append(p_canvas.DOFade(1f, 0.25f));
			_MainSequence.AppendInterval(0.1f);
			if (p_mission.IsCompleted)
			{
				float starFillDuration = 0.15f * (float)p_mission.Difficulty;
				float pointsFadeinDuration = 0.2f;
				float pointsScoreUpDuration = _BarRotateTime;
				_MainSequence.AppendCallback(delegate
				{
					_MissionTab.CompleteAnimation(starFillDuration, pointsFadeinDuration, pointsScoreUpDuration);
				});
				_MainSequence.AppendInterval(starFillDuration + pointsFadeinDuration);
				_MainSequence.AppendCallback(delegate
				{
					_MainSequence.TogglePause();
					ShowPointsFromMission(p_mission.Difficulty - 1, pointsScoreUpDuration);
				});
				_MainSequence.AppendInterval(0.25f);
			}
			else
			{
				_MainSequence.AppendCallback(delegate
				{
					_MissionTab.NotCompleteAnimation(0.25f);
				});
				_MainSequence.AppendInterval(0.75f);
			}
			_MainSequence.Append(p_canvas.DOFade(0f, 0.25f));
		}

		private void ShowPointsFromMission(int difficultyIndex, float p_time)
		{
			int targetPoints = _TargetPoints;
			_TargetPoints = targetPoints + EndFloorManager.PointsFromMissions[difficultyIndex];
			_TargetRank = RankManager.RankFromPoints(_TargetPoints);
			ProgressBarRotate(RankManager.PointsRankCostRatio(_RankShowed, targetPoints), p_time);
		}

		private void RecordsAnim()
		{
			_MainSequence.TogglePause();
			int targetPoints = _TargetPoints;
			_TargetRank = RankManager.Rank;
			_TargetPoints = RankManager.Experience;
			float num = _BarRotateTime * ((float)(_TargetRank - _RankShowed) + 0.5f);
			ScoreToZero(num);
			ProgressBarRotate(RankManager.PointsRankCostRatio(_RankShowed, targetPoints), num);
		}

		private void ProgressBarRotate(float p_arc_from, float p_time)
		{
			AudioManager.PlaySound("circle_loop");
			if (_RankShowed < _TargetRank)
			{
				float speedUpCoef = 1f / Mathf.Pow(_TargetRank - _RankShowed, 1f);
				_RankProgressBar.RotateArc((float)Math.PI * 2f * p_arc_from, (float)Math.PI * 2f, UpdateRank, p_arc_from == 0f, speedUpCoef);
			}
			else
			{
				float num = RankManager.PointsRankCostRatio(_TargetRank, _TargetPoints);
				_RankProgressBar.RotateArc((float)Math.PI * 2f * p_arc_from, (float)Math.PI * 2f * num, _MainSequence.TogglePause, true, 1f);
			}
		}

		private void UpdateRank()
		{
			_RankShowed++;
			_Rank.text = _RankShowed.ToString();
			RunFlash();
			ShowCards(_RankShowed);
			foreach (KeyValuePair<string, int> item in EndFloorManager.ChargesPerLevel[_RankShowed])
			{
				ShowCharges(item);
			}
			ProgressBarRotate(0f, _BarRotateTime);
		}

		private void ShowCharges(KeyValuePair<string, int> item)
		{
			_GadgetPanel.AddSegmentsToGadget(item.Key, item.Value);
		}

		private void ShowCards(int rank = -1)
		{
			SupplyItem supplyItem = ((rank == -1) ? EndFloorManager.GetBasketItemFromMission() : EndFloorManager.GetBasketItemByRank(rank));
			if (supplyItem != null)
			{
				_CardsLayout.AppendCards(supplyItem.Cards);
			}
		}

		private void RunFlash()
		{
			_Flash.gameObject.SetActive(true);
			_Flash.rectTransform.anchoredPosition = new Vector2(50f, _Flash.rectTransform.anchoredPosition.y);
			_Flash.rectTransform.sizeDelta = new Vector2(1500f, 250f);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(0.05f);
			sequence.Append(_Flash.rectTransform.DOSizeDelta(new Vector2(0f, 0f), 0.1f));
			sequence.Join(_Flash.rectTransform.DOAnchorPosX(0f, 0.1f));
			sequence.AppendCallback(delegate
			{
				_Flash.gameObject.SetActive(false);
			});
			sequence.AppendCallback(delegate
			{
				AudioManager.PlaySound("full_circle");
			});
			sequence.Play();
		}

		private void OnFinalRank()
		{
			_TappedFirstTime = true;
			_TapAnywhere.gameObject.SetActive(true);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				_ScoreSeqStop = true;
			});
			sequence.Append(_TapAnywhere.DOFade(0f, 1f));
			sequence.Append(_TapAnywhere.DOFade(1f, 1f));
			sequence.SetAutoKill(false);
			sequence.SetLoops(-1);
			sequence.Play();
		}

		private void ScoreToZero(float p_duration)
		{
			Sequence seq = DOTween.Sequence();
			seq.Append(DOTween.To(() => int.Parse(_FloorPoints.text), delegate(int x)
			{
				_FloorPoints.text = x.ToString();
			}, 0, p_duration));
			seq.Join(DOTween.To(() => int.Parse(_StuntsPoints.text), delegate(int x)
			{
				_StuntsPoints.text = x.ToString();
			}, 0, p_duration));
			seq.OnUpdate(delegate
			{
				ScoreToZeroStop(_ScoreSeqStop, seq);
			});
			seq.Play();
		}

		private void ScoreToZeroStop(bool p_stop, Sequence p_seq)
		{
			if (p_stop)
			{
				p_seq.Kill();
				_FloorPoints.text = "0";
				_StuntsPoints.text = "0";
				_FloorPointsIcon.DOFade(0f, 0.8f);
				_StuntPointsIcon.DOFade(0f, 0.8f);
				_FloorPoints.DOFade(0f, 0.8f);
				_StuntsPoints.DOFade(0f, 0.8f);
			}
		}

		public override void Dismiss()
		{
			base.Dismiss();
			EndFloorManager.ChargesRefreshAllowed = true;
		}
	}
}
