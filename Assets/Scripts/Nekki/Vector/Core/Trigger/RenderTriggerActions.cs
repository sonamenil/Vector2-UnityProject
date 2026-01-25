using System.Collections.Generic;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger
{
	public class RenderTriggerActions
	{
		private static RenderTriggerActions _Current;

		private TriggerRunner _Parent;

		private List<List<TriggerRunnerAction>> _DelayActions;

		public static RenderTriggerActions Current
		{
			get
			{
				if (_Current == null)
				{
					_Current = new RenderTriggerActions();
				}
				return _Current;
			}
		}

		public RenderTriggerActions()
		{
			_DelayActions = new List<List<TriggerRunnerAction>>();
		}

		public static void Clear()
		{
			_Current = null;
		}

		public void AddActions(List<TriggerRunnerAction> p_actions)
		{
			int count = p_actions.Count;
			for (int i = 0; i < count; i++)
			{
				bool isRunNext = true;
				p_actions[i].Activate(ref isRunNext);
				if (!isRunNext)
				{
					CopyActionToDelay(p_actions, i, count);
					break;
				}
			}
		}

		public void CopyActionToDelay(List<TriggerRunnerAction> p_actions, int p_from, int size)
		{
			if (p_actions[0].IsParentEnable)
			{
				List<TriggerRunnerAction> list = new List<TriggerRunnerAction>(size - p_from);
				for (int num = size - 1; num >= p_from; num--)
				{
					list.Add(p_actions[num]);
				}
				_DelayActions.Add(list);
			}
		}

		public void Render()
		{
			if (_DelayActions.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _DelayActions.Count; i++)
			{
				List<TriggerRunnerAction> list = _DelayActions[i];
				bool isRunNext = false;
				while (list.Count != 0)
				{
					TriggerRunnerAction triggerRunnerAction = list[list.Count - 1];
					triggerRunnerAction.Activate(ref isRunNext);
					if (!triggerRunnerAction.IsParentEnable)
					{
						list.Clear();
						break;
					}
					if (isRunNext)
					{
						list.Remove(triggerRunnerAction);
						continue;
					}
					break;
				}
			}
			for (int num = _DelayActions.Count - 1; num >= 0; num--)
			{
				if (_DelayActions[num].Count == 0)
				{
					_DelayActions.RemoveAt(num);
				}
			}
		}
	}
}
