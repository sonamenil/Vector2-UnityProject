using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class StarScroller : MonoBehaviour
	{
		[SerializeField]
		private GameObject _PlatePrefab;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private PlateScroller _Scroller;

		private List<StarReward> _StarRewards;

		private List<StarRewardTab> _Plates = new List<StarRewardTab>();

		private StarRewardTab _LastSelectedPlate;

		public List<StarRewardTab> Plates
		{
			get
			{
				return _Plates;
			}
		}

		private void Awake()
		{
			PlateScroller scroller = _Scroller;
			scroller.OnStop = (Action<int>)Delegate.Combine(scroller.OnStop, new Action<int>(PlateStop));
			PlateScroller scroller2 = _Scroller;
			scroller2.OnMove = (Action<int, float>)Delegate.Combine(scroller2.OnMove, new Action<int, float>(PlateMove));
		}

		public void Init(List<StarReward> p_rewards)
		{
			_StarRewards = p_rewards;
			_Scroller.verticalScrollbar.value = 1f;
			_Plates.Clear();
			_Content.DetachChildren();
			for (int i = 0; i < _StarRewards.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_PlatePrefab);
				gameObject.transform.SetParent(_Content, false);
				StarRewardTab component = gameObject.GetComponent<StarRewardTab>();
				component.Init(_StarRewards[i], OnPlateTap);
				_Plates.Add(component);
			}
			_Plates[0].Alpha = 1f;
			_Scroller.verticalScrollbar.value = 1f;
			_Scroller.SetPlateToCenter(0);
		}

		public void OnPlateTap(StarRewardTab p_plate)
		{
			AudioManager.PlaySound("select_button");
			int num = _StarRewards.IndexOf(p_plate.StarReward);
			if (_LastSelectedPlate == null)
			{
				_Scroller.SetPlateToCenter(num, true, num);
			}
			else
			{
				_Scroller.SetPlateToCenter(num, true, _StarRewards.IndexOf(_LastSelectedPlate.StarReward));
			}
		}

		public void PlateMove(int p_index, float p_percent)
		{
			if (StarsManager.IsShowSequence)
			{
				return;
			}
			for (int i = 0; i < _StarRewards.Count; i++)
			{
				float num = (float)(p_index - i + 1) - p_percent;
				if (i != p_index)
				{
					_Plates[i].Alpha = 0.6f;
				}
				else
				{
					_Plates[i].Alpha = 1f;
				}
			}
		}

		public void PlateStop(int p_index)
		{
			if (_LastSelectedPlate != null && _LastSelectedPlate != _Plates[p_index])
			{
				_LastSelectedPlate.Alpha = 0.6f;
			}
			_LastSelectedPlate = _Plates[p_index];
			_LastSelectedPlate.Alpha = 1f;
		}

		public void Skip()
		{
			foreach (StarRewardTab plate in _Plates)
			{
				plate.Alpha = 0.6f;
			}
			_Scroller.verticalScrollbar.value = ((_Plates.Count <= 1) ? 1f : 0f);
			_Scroller.SetPlateToCenter(_Plates.Count);
			_Plates[_Plates.Count - 1].Alpha = 1f;
		}

		public void TabChangeSequence(Sequence p_seq, int p_index, float p_duration, int stars_value)
		{
			p_seq.AppendCallback(delegate
			{
				_Plates[p_index].MakeTabVisible();
			});
			int next = ((p_index != 0) ? (p_index - 1) : p_index);
			if (p_index != 0)
			{
				p_seq.AppendCallback(delegate
				{
					_Scroller.SetPlateToCenter(p_index, true, next);
					AudioManager.PlaySound("starrewardtab");
				});
			}
			p_seq.AppendCallback(delegate
			{
				_Plates[p_index].FillStarsSeq(p_duration, stars_value);
			});
		}
	}
}
