using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class StarRewardsPanel : UIModule
	{
		private const float _TabChangeDuration = 0.6f;

		private const float _TabChangeDelay = 0.3f;

		[SerializeField]
		private Text _StarsCount;

		[SerializeField]
		private StarScroller _Scroller;

		[SerializeField]
		private GameObject _BlockTouch;

		private List<StarReward> _StarRewards;

		private Sequence _RewardsSequence;

		private bool _TappedFirstTime;

		private int _CurrentStars;

		private bool _IsTutorialEndSubscribed;

		public bool TappedFirstTime
		{
			get
			{
				return _TappedFirstTime;
			}
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			StarsManager.Init(Scene<TerminalScene>.Current.IsGameRestoreActive);
			_CurrentStars = CounterController.Current.CounterCurrentMissionStars;
			_StarsCount.text = _CurrentStars.ToString();
			_StarRewards = StarsManager.StarRewards;
			_Scroller.Init(_StarRewards);
			if (StarsManager.IsShowSequence)
			{
				ShowSequence();
			}
			else
			{
				SkipSequence();
			}
		}

		private void ShowSequence()
		{
			if ((int)CounterController.Current.CounterTutorialMissionsStars != 0)
			{
				Nekki.Vector.GUI.Tutorial.Tutorial.OnTutorialEnd += StartSequence;
				_IsTutorialEndSubscribed = true;
			}
			else
			{
				StartSequence();
			}
		}

		private void StartSequence()
		{
			_RewardsSequence = DOTween.Sequence();
			for (int i = 0; i < _StarRewards.Count; i++)
			{
				int num = ((_StarRewards[i].Cost >= _CurrentStars) ? _CurrentStars : _StarRewards[i].Cost);
				_Scroller.TabChangeSequence(_RewardsSequence, i, 0.6f, num);
				_RewardsSequence.Append(DOTween.To(() => int.Parse(_StarsCount.text), delegate(int x)
				{
					_StarsCount.text = x.ToString();
				}, _CurrentStars - num, 0.6f));
				_CurrentStars -= num;
				_RewardsSequence.AppendInterval(0.3f);
			}
			_RewardsSequence.AppendCallback(delegate
			{
				UnBlockTouches(true);
			});
			_RewardsSequence.Play();
		}

		private void UpdateStarsCount(int p_Stars)
		{
			_StarsCount.text = p_Stars.ToString();
		}

		private void UnBlockTouches(bool unblock = true)
		{
			_TappedFirstTime = true;
			_BlockTouch.SetActive(!unblock);
		}

		public void OnSkipTouch()
		{
			Scene<TerminalScene>.Current.ContinueTap();
		}

		public void SkipSequence()
		{
			_RewardsSequence.Complete();
			_Scroller.Skip();
			_CurrentStars = CounterController.Current.CounterCurrentMissionStars;
			SkipFilling();
			_StarsCount.text = _CurrentStars.ToString();
			UnBlockTouches(true);
		}

		private void SkipFilling()
		{
			for (int i = 0; i < _StarRewards.Count; i++)
			{
				int num = ((_StarRewards[i].Cost >= _CurrentStars) ? _CurrentStars : _StarRewards[i].Cost);
				_CurrentStars -= num;
				_Scroller.Plates[i].SkipTabSequences(num);
			}
		}

		protected override void Free()
		{
			base.Free();
			if (_IsTutorialEndSubscribed)
			{
				Nekki.Vector.GUI.Tutorial.Tutorial.OnTutorialEnd -= StartSequence;
			}
		}
	}
}
