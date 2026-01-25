using System.Collections.Generic;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core.Runners
{
	public class SensorRunner : Runner
	{
		private int _AI;

		private List<TriggerRunner> _ActiveTriggers = new List<TriggerRunner>();

		public int AI
		{
			get
			{
				return _AI;
			}
		}

		public SensorRunner(float p_X, float p_Y, int p_AI, Element p_elements)
			: base(p_X, p_Y, p_elements)
		{
			_AI = p_AI;
			_TypeClass = TypeRunner.Sensor;
		}

		public override bool Render()
		{
			return true;
		}

		public void CheckTrigger(TriggerRunner p_trigger)
		{
			if (!p_trigger.IsEnabled)
			{
				return;
			}
			bool isActive = GetIsActive(p_trigger);
			if (CheckAI(p_trigger))
			{
				if (GetInside(p_trigger))
				{
					if (!isActive)
					{
						Activate(p_trigger);
					}
				}
				else if (isActive)
				{
					DeActivate(p_trigger);
				}
			}
			else if (isActive)
			{
				DeActivate(p_trigger, false);
			}
		}

		private void Activate(TriggerRunner p_trigger)
		{
			p_trigger.CheckEvent(new TRE_Enter(), null);
			if (!p_trigger.IsDiagonal)
			{
				_ActiveTriggers.Add(p_trigger);
			}
		}

		private void DeActivate(TriggerRunner p_triger, bool p_sendEvent = true)
		{
			if (p_sendEvent)
			{
				TRE_Exit p_event = new TRE_Exit();
				p_triger.CheckEvent(p_event, null);
			}
			_ActiveTriggers.Remove(p_triger);
		}

		private bool GetIsActive(TriggerRunner p_triger)
		{
			return _ActiveTriggers.Contains(p_triger);
		}

		private bool CheckAI(TriggerRunner p_triger)
		{
			return _AI == p_triger.AIVar.ValueInt || p_triger.AIVar.ValueInt == -1;
		}

		public bool GetInside(TriggerRunner p_trigger)
		{
			return p_trigger.Rectangle.Contains(base.Position);
		}
	}
}
