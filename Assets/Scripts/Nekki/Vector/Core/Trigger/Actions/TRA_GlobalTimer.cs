using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_GlobalTimer : TriggerRunnerAction
	{
		private Variable _FramesVar;

		private Variable _Action;

		private TRA_GlobalTimer(TRA_GlobalTimer p_copyAction)
			: base(p_copyAction)
		{
			_FramesVar = p_copyAction._FramesVar;
			_Action = p_copyAction._Action;
		}

		public TRA_GlobalTimer(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string p_nameOrValue = XmlUtils.ParseString(p_node.Attributes["Frames"], "600");
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FramesVar, p_nameOrValue);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Action, p_node.Attributes["Action"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			if (_Action.ValueString.ToLower().Equals("increment"))
			{
				RunMainController.Location.Timer.ChangeFramesCount(_FramesVar.ValueInt);
				if (RunMainController.Location.Timer.FrameCount > 3600)
				{
					DebugUtils.Dialog("Too much time. Timer set 3600 frames automatically!!", false);
					RunMainController.Location.Timer.SetFramesCount(3600);
				}
				RunMainController.Location.Timer.StartCountdown();
			}
			if (_Action.ValueString.ToLower().Equals("pause"))
			{
				RunMainController.Location.Timer.Pause();
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_GlobalTimer(this);
		}
	}
}
