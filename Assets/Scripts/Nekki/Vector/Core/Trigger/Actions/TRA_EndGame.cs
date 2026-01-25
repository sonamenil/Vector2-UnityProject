using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_EndGame : TriggerRunnerAction
	{
		private Variable _TimeVar;

		private Variable _ResultVar;

		private Variable _ModelVar;

		private TRA_EndGame(TRA_EndGame p_copyAction)
			: base(p_copyAction)
		{
			_ResultVar = p_copyAction._ResultVar;
			_TimeVar = p_copyAction._TimeVar;
			_ModelVar = p_copyAction._ModelVar;
		}

		public TRA_EndGame(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string value = p_node.Attributes["Result"].Value;
			string p_nameOrValue = "80";
			if (p_node.Attributes["Frames"] != null)
			{
				p_nameOrValue = p_node.Attributes["Frames"].Value;
			}
			string value2 = p_node.Attributes["Model"].Value;
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ResultVar, value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _TimeVar, p_nameOrValue);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelVar, value2);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			string valueString = _ModelVar.ValueString;
			switch (_ResultVar.ValueString)
			{
			case "Win":
				RunMainController.Win(GetModel(valueString), (float)_TimeVar.ValueInt / 60f);
				break;
			case "Loss":
				RunMainController.CheckLoss(ModelHuman.ModelState.Loss, GetModel(valueString), (float)_TimeVar.ValueInt / 60f);
				break;
			case "Death":
				RunMainController.CheckLoss(ModelHuman.ModelState.DeadlyDamage, GetModel(valueString), (float)_TimeVar.ValueInt / 60f);
				break;
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_EndGame(this);
		}

		public override string ToString()
		{
			return "EndGame Model=" + _ModelVar.DebugStringValue + " Result=" + _ResultVar.DebugStringValue + " Frames" + _TimeVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: EndGame");
			VectorLog.Tab(1);
			VectorLog.RunLog("Time", _TimeVar);
			VectorLog.RunLog("Result", _ResultVar);
			VectorLog.RunLog("Model", _ModelVar);
			VectorLog.Untab(1);
		}
	}
}
