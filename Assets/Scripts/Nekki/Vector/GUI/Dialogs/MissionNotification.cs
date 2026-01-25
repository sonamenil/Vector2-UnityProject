using System.Collections.Generic;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public class MissionNotification : Notification
	{
		[SerializeField]
		private HorizontalLayoutGroup _RewardsLayout;

		[SerializeField]
		private GameObject _MissionRewardPrefab;

		[SerializeField]
		private LabelAlias _PointsAmount;

		private List<MissionRewardElementUI> _Rewards = new List<MissionRewardElementUI>();

		public void Init(Parameters p_parameters)
		{
			_Parameters = p_parameters;
			Reset();
			SetRootPosition();
			SetRewards();
		}

		private void Reset()
		{
			foreach (MissionRewardElementUI reward in _Rewards)
			{
				Object.DestroyImmediate(reward.gameObject);
			}
			_Rewards.Clear();
		}

		private void SetRewards()
		{
			int i = 0;
			for (int difficulty = _Parameters.Mission.Difficulty; i < difficulty; i++)
			{
				MissionRewardElementUI component = Object.Instantiate(_MissionRewardPrefab).GetComponent<MissionRewardElementUI>();
				component.transform.SetParent(_RewardsLayout.transform, false);
				component.Init(true);
				_Rewards.Add(component);
			}
			_PointsAmount.Text = _Parameters.Mission.RewardAmount.ToString();
		}
	}
}
