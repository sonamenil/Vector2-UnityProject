using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_ModelExecute : TriggerRunnerAction
	{
		private Variable _AnimVar;

		private Variable _FrameVar;

		private TRA_ModelExecute(TRA_ModelExecute p_copyAction)
			: base(p_copyAction)
		{
			_AnimVar = p_copyAction._AnimVar;
			_FrameVar = p_copyAction._FrameVar;
		}

		public TRA_ModelExecute(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _AnimVar, p_node.Attributes["AnimName"].Value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FrameVar, p_node.Attributes["AnimFrame"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_ModelExecute(this);
		}

		public override string ToString()
		{
			return "ModelExecute AnimName" + _AnimVar.DebugStringValue + " AnimFrame" + _FrameVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: ModelExecute");
			VectorLog.Tab(1);
			VectorLog.RunLog("Anim", _AnimVar);
			VectorLog.RunLog("Frame", _FrameVar);
			VectorLog.Untab(1);
		}
	}
}
