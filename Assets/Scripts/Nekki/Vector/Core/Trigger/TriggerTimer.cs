using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerTimer
	{
		private bool _IsActiv;

		private int _Counter;

		private TriggerRunner _Parent;

		public TriggerTimer(TriggerRunner p_parent)
		{
			_Parent = p_parent;
		}

		public void Start(int p_value)
		{
			RunnerRender.AddRunner(_Parent);
			_Counter = p_value;
			_IsActiv = true;
		}

		public bool Render()
		{
			if (_IsActiv)
			{
				_Counter--;
				if (_Counter <= 0)
				{
					_IsActiv = false;
					TRE_Timeout p_event = new TRE_Timeout();
					ModelHuman modelByName = RunMainController.Location.GetModelByName(_Parent.ModelVar.ValueString);
					_Parent.CheckEvent(p_event, modelByName);
					return !_IsActiv;
				}
				return false;
			}
			return true;
		}
	}
}
