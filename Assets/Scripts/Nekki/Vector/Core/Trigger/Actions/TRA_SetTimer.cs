using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_SetTimer : TriggerRunnerAction
	{
		private Variable _FramesVar;

		private TRA_SetTimer(TRA_SetTimer p_copyAction)
			: base(p_copyAction)
		{
			_FramesVar = p_copyAction._FramesVar;
		}

		public TRA_SetTimer(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FramesVar, p_node.Attributes["Frames"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			_ParentLoop.ParentTrigger.SetTimer(_FramesVar.ValueInt);
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_SetTimer(this);
		}

		public override string ToString()
		{
			return "SetTimer Frames=" + _FramesVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: SetTimer");
			VectorLog.Tab(1);
			VectorLog.RunLog("Frames", _FramesVar);
			VectorLog.Untab(1);
		}
	}
}
