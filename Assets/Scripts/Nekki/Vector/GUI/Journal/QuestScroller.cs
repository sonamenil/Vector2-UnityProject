using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Quest;
using UnityEngine;

namespace Nekki.Vector.GUI.Journal
{
	public class QuestScroller : MonoBehaviour
	{
		[SerializeField]
		private GameObject _PlatePrefab;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private PlateScroller _Scroller;

		private Action<Quest> _OnQuestSelected;

		private List<Quest> _Quests;

		private List<QuestScrollerPlate> _Plates = new List<QuestScrollerPlate>();

		private QuestScrollerPlate _LastSelectedPlate;

		private void Awake()
		{
			PlateScroller scroller = _Scroller;
			scroller.OnStop = (Action<int>)Delegate.Combine(scroller.OnStop, new Action<int>(PlateStop));
			PlateScroller scroller2 = _Scroller;
			scroller2.OnMove = (Action<int, float>)Delegate.Combine(scroller2.OnMove, new Action<int, float>(PlateMove));
		}

		public void Init(List<Quest> p_quests, Action<Quest> p_onQuestSelected, string MoveToQuestName = null)
		{
			_Quests = p_quests;
			_OnQuestSelected = p_onQuestSelected;
			_Plates.Clear();
			_Content.DetachChildren();
			for (int i = 0; i < _Quests.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_PlatePrefab);
				gameObject.transform.SetParent(_Content, false);
				QuestScrollerPlate component = gameObject.GetComponent<QuestScrollerPlate>();
				component.Init(_Quests[i], OnPlateTap);
				_Plates.Add(component);
			}
			if (MoveToQuestName != null)
			{
				MoveToQuestNameOnInit(MoveToQuestName);
			}
			else
			{
				_Scroller.SetPlateToCenter(0);
			}
		}

		public void MoveToQuestNameOnInit(string QuestName)
		{
			for (int i = 0; i < _Quests.Count; i++)
			{
				if (_Quests[i].Name == QuestName)
				{
					_Scroller.SetPlateToCenter(i);
					break;
				}
			}
		}

		public void OnPlateTap(QuestScrollerPlate p_plate)
		{
			AudioManager.PlaySound("select_button");
			int num = _Quests.IndexOf(p_plate.PlateQuest);
			if (_LastSelectedPlate == null)
			{
				_Scroller.SetPlateToCenter(num, true, num);
			}
			else
			{
				_Scroller.SetPlateToCenter(num, true, _Quests.IndexOf(_LastSelectedPlate.PlateQuest));
			}
		}

		public void PlateStop(int p_index)
		{
			if (_LastSelectedPlate != null && _LastSelectedPlate != _Plates[p_index])
			{
				_LastSelectedPlate.ResizeDelta(0f);
				_LastSelectedPlate.SetNormalColorToQuestName();
			}
			_LastSelectedPlate = _Plates[p_index];
			_LastSelectedPlate.ResizeDelta(1f, 0.4f, true, true, true);
			_LastSelectedPlate.SetSelectedColorToQuestName();
			if (_OnQuestSelected != null)
			{
				_OnQuestSelected(_LastSelectedPlate.PlateQuest);
			}
		}

		public void PlateMove(int p_index, float p_percent)
		{
			float p_persect = 1f - Mathf.Min(Mathf.Abs(p_percent), 1f);
			_Plates[p_index].ResizeDelta(p_persect, 0f, false);
			p_percent = 0.5f * (p_percent + 1f);
			int num = ((p_index + 5 < _Plates.Count) ? (p_index + 5) : _Plates.Count);
			int num2 = ((p_index - 5 >= 0) ? (p_index - 5) : 0);
			for (int i = num2; i < p_index; i++)
			{
				float p_pos = (float)(p_index - i + 1) - p_percent;
				_Plates[i].SetPlateAlpha(CalcPlateAlpha(p_pos));
				if (p_index - i + 1 >= 1)
				{
					_Plates[i].SetQuestContentAlpha(CalcQuestNameAlpha(p_pos));
				}
			}
			for (int j = p_index; j < num; j++)
			{
				float p_pos2 = (float)(j - p_index) + p_percent;
				_Plates[j].SetPlateAlpha(CalcPlateAlpha(p_pos2));
				if (j - p_index >= 2)
				{
					_Plates[j].SetQuestContentAlpha(CalcQuestNameAlpha(p_pos2));
				}
				else
				{
					_Plates[j].SetQuestContentAlpha(1f);
				}
			}
		}

		private float CalcPlateAlpha(float p_pos)
		{
			return (float)(-0.0107143 * (double)p_pos * (double)p_pos - 0.0321429 * (double)p_pos + 0.498571);
		}

		private float CalcQuestNameAlpha(float p_pos)
		{
			return 1f - 0.2f * p_pos;
		}
	}
}
