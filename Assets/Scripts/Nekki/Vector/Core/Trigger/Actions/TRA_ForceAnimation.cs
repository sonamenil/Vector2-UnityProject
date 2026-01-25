using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_ForceAnimation : TriggerRunnerAction
	{
		private Variable _FramesVar;

		private Variable _NameVar;

		private Variable _ModelNameVar;

		private Variable _DirectionVar;

		private TRA_ForceAnimation(TRA_ForceAnimation p_copyAction)
			: base(p_copyAction)
		{
			_FramesVar = p_copyAction._FramesVar;
			_NameVar = p_copyAction._NameVar;
			_ModelNameVar = p_copyAction._ModelNameVar;
			_DirectionVar = p_copyAction._DirectionVar;
		}

		public TRA_ForceAnimation(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FramesVar, XmlUtils.ParseString(p_node.Attributes["Frame"], "-1"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _NameVar, XmlUtils.ParseString(p_node.Attributes["Name"]));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelNameVar, XmlUtils.ParseString(p_node.Attributes["Model"]));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _DirectionVar, XmlUtils.ParseString(p_node.Attributes["Reversed"], "0"));
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			string valueString = _ModelNameVar.ValueString;
			ModelHuman model = GetModel(valueString);
			if (model != null)
			{
				model.PlayAnimation(_NameVar.ValueString, _DirectionVar.ValueInt == 1, _FramesVar.ValueInt);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_ForceAnimation(this);
		}

		public override string ToString()
		{
			return "ForceAnimation Model=" + _ModelNameVar.DebugStringValue + " Name=" + _NameVar.DebugStringValue + " Frame=" + _FramesVar.DebugStringValue + " Revers=" + _DirectionVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: ForceAnimation");
			VectorLog.Tab(1);
			VectorLog.RunLog("Frames", _FramesVar);
			VectorLog.RunLog("Name", _NameVar);
			VectorLog.RunLog("ModelName", _ModelNameVar);
			VectorLog.RunLog("Direction", _DirectionVar);
			VectorLog.Untab(1);
		}
	}
}
