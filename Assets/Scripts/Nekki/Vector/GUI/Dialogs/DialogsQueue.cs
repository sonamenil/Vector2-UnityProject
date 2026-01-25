using System;
using System.Collections.Generic;

namespace Nekki.Vector.GUI.Dialogs
{
	public class DialogsQueue
	{
		private bool _IsProcessing;

		private SortedList<int, List<Action>> _Queue;

		public bool IsProcessing
		{
			get
			{
				return _IsProcessing;
			}
			set
			{
				_IsProcessing = value;
			}
		}

		public void ShowNext()
		{
			List<Action> lastList = GetLastList();
			if (lastList == null)
			{
				_IsProcessing = false;
				return;
			}
			_IsProcessing = true;
			Action action = lastList[0];
			lastList.Remove(action);
			action();
		}

		public void AddToQueue(int p_order, Action p_data)
		{
			if (_Queue == null)
			{
				_Queue = new SortedList<int, List<Action>>();
			}
			List<Action> list = null;
			if (_Queue.ContainsKey(p_order))
			{
				list = _Queue[p_order];
			}
			else
			{
				list = new List<Action>();
				_Queue.Add(p_order, list);
			}
			list.Add(p_data);
			if (!_IsProcessing)
			{
				ShowNext();
			}
		}

		public List<Action> GetLastList()
		{
			if (_Queue == null)
			{
				return null;
			}
			List<Action> list = null;
			int key = int.MinValue;
			do
			{
				if (list != null)
				{
					_Queue.Remove(key);
				}
				if (_Queue.Count == 0)
				{
					list = null;
					_Queue = null;
					break;
				}
				key = _Queue.Keys[_Queue.Keys.Count - 1];
				list = _Queue[key];
			}
			while (list.Count == 0);
			return list;
		}
	}
}
