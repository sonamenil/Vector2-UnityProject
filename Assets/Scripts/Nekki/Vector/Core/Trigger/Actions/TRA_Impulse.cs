using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;
using UnityEngine;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Impulse : TriggerRunnerAction
	{
		private Variable _ModelVar;

		private Variable _Impuls;

		private Variable _R;

		private Variable _Absorption;

		public TRA_Impulse(TRA_Impulse p_copyAction)
			: base(p_copyAction)
		{
			_ModelVar = p_copyAction._ModelVar;
		}

		public TRA_Impulse(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelVar, p_node.Attributes["Model"].Value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Impuls, p_node.Attributes["Impulse"].Value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _R, p_node.Attributes["R"].Value);
			if (p_node.Attributes["Absorption"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Absorption, p_node.Attributes["Absorption"].Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			ModelHuman model = GetModel(_ModelVar.ValueString);
			if (model != null)
			{
				if (_Absorption != null)
				{
					model.ResetImpuls(_Absorption.ValueFloat);
				}
				model.Stricke(_Impuls.ValueFloat, _R.ValueFloat, new Vector3(_ParentLoop.ParentTrigger.Rectangle.MidX, _ParentLoop.ParentTrigger.Rectangle.MidY, 0f));
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Impulse(this);
		}

		public override string ToString()
		{
			return "Impuls Model=" + _ModelVar.DebugStringValue + " Impuls=" + _Impuls.ValueFloat + " R=" + _R.ValueFloat;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Impuls");
			VectorLog.Tab(1);
			VectorLog.RunLog("Model", _ModelVar);
			VectorLog.RunLog("Impuls", _Impuls);
			VectorLog.RunLog("R", _R);
			VectorLog.Untab(1);
		}
	}
}
