using System.Collections.Generic;
using Nekki.Vector.Core.Quest;
using UnityEngine;

namespace Nekki.Vector.GUI.Journal
{
	public class JournalPanel : UIModule
	{
		[SerializeField]
		private QuestInfo _QuestInfo;

		[SerializeField]
		private QuestScroller _Scroller;

		private string _SelectedQuest;

		public string SelectedQuest
		{
			get
			{
				return _SelectedQuest;
			}
			set
			{
				_SelectedQuest = value;
			}
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			Refresh();
		}

		public void Refresh()
		{
			List<Quest> list = new List<Quest>();
			for (int i = 0; i < QuestManager.Current.Quests.Count; i++)
			{
				if (!QuestManager.Current.Quests[i].IsSleepState && !QuestManager.Current.Quests[i].IsCompleteState)
				{
					list.Add(QuestManager.Current.Quests[i]);
				}
			}
			for (int num = QuestManager.Current.Quests.Count - 1; num >= 0; num--)
			{
				if (!QuestManager.Current.Quests[num].IsSleepState && QuestManager.Current.Quests[num].IsCompleteState)
				{
					list.Add(QuestManager.Current.Quests[num]);
				}
			}
			_Scroller.Init(list, OnQuestSelected, SelectedQuest);
		}

		public void OnQuestSelected(Quest p_quest)
		{
			_QuestInfo.Init(p_quest);
		}
	}
}
