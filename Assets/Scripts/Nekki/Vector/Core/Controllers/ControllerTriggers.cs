using System.Collections.Generic;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerTriggers
	{
		private List<TriggerRunner> _ActiveTriggers = new List<TriggerRunner>();

		private List<TriggerRunner> _LineTrggers = new List<TriggerRunner>();

		private ModelHuman _Model;

		public ControllerTriggers(ModelHuman p_model)
		{
			_Model = p_model;
		}

		public ModelNode GetNode(string p_trigger)
		{
			return _Model.Node(p_trigger);
		}

		public void Check(TriggerRunner p_triger)
		{
			bool isActive = GetIsActive(p_triger);
			if (CheckAI(p_triger))
			{
				if (GetInside(p_triger))
				{
					if (!isActive)
					{
						Activate(p_triger);
					}
				}
				else if (isActive)
				{
					DeActivate(p_triger);
				}
			}
			else if (isActive)
			{
				DeActivate(p_triger, false);
			}
		}

		public void Render()
		{
			CheckLine();
			CheckRenderEvents();
		}

		private void CheckLine()
		{
			for (int i = 0; i < _LineTrggers.Count; i++)
			{
				for (int j = 0; j < _LineTrggers[i].Lines.Count; j++)
				{
					_LineTrggers[i].Lines[j].Check(_Model);
				}
			}
		}

		private void CheckRenderEvents()
		{
			for (int i = 0; i < _ActiveTriggers.Count; i++)
			{
				_ActiveTriggers[i].CheckRenderEvent(_Model);
			}
		}

		private void Activate(TriggerRunner p_trigger)
		{
			p_trigger.CheckEvent(new TRE_Enter(), _Model);
			p_trigger.CheckEvent(new TRE_Collision(), _Model);
			if (!p_trigger.IsDiagonal)
			{
				_ActiveTriggers.Add(p_trigger);
				if (p_trigger.Lines != null)
				{
					_LineTrggers.Add(p_trigger);
				}
			}
		}

		private void DeActivate(TriggerRunner p_triger, bool p_sendEvent = true)
		{
			if (p_sendEvent)
			{
				TRE_Exit p_event = new TRE_Exit();
				p_triger.CheckEvent(p_event, _Model);
			}
			_ActiveTriggers.Remove(p_triger);
			if (p_triger.Lines != null)
			{
				_LineTrggers.Remove(p_triger);
			}
			p_triger.ResetRenderEvents();
			_Model.CheckDelayAction(p_triger);
		}

		public void RemoveTrigger(TriggerRunner p_trigger)
		{
			if (_ActiveTriggers.Contains(p_trigger))
			{
				_ActiveTriggers.Remove(p_trigger);
				if (p_trigger.Lines != null)
				{
					_LineTrggers.Remove(p_trigger);
				}
				p_trigger.ResetRenderEvents();
			}
		}

		public void SetKeyEvent(string p_key)
		{
			TRE_Key p_event = new TRE_Key(p_key);
			for (int i = 0; i < _ActiveTriggers.Count; i++)
			{
				_ActiveTriggers[i].CheckEvent(p_event, _Model);
			}
		}

		private bool GetIsActive(TriggerRunner p_triger)
		{
			return _ActiveTriggers.Contains(p_triger);
		}

		private bool CheckAI(TriggerRunner p_triger)
		{
			return _Model.AI == p_triger.AIVar.ValueInt || p_triger.AIVar.ValueInt == -1;
		}

		public bool GetInside(TriggerRunner p_trigger)
		{
			switch (p_trigger.CollisionType)
			{
			case TriggerRunner.TriggerColisionType.OneNode:
				return p_trigger.Hit(GetNode(p_trigger.TriggerNodeName));
			case TriggerRunner.TriggerColisionType.MultiNode:
			{
				string text = null;
				for (int i = 0; i < p_trigger.TriggerNodesName.Count; i++)
				{
					text = (p_trigger.CollisionNodeName = p_trigger.TriggerNodesName[i]);
					if (p_trigger.Hit(GetNode(text)))
					{
						return true;
					}
				}
				return false;
			}
			default:
				return false;
			}
		}

		public void Reset()
		{
			_ActiveTriggers.Clear();
			_LineTrggers.Clear();
		}
	}
}
