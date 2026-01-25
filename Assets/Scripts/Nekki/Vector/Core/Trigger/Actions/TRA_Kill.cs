using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Kill : TriggerRunnerAction
	{
		private Variable _ModelVar;

		public TRA_Kill(TRA_Kill p_copyAction)
			: base(p_copyAction)
		{
			_ModelVar = p_copyAction._ModelVar;
		}

		public TRA_Kill(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelVar, p_node.Attributes["Model"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			ModelHuman model = GetModel(_ModelVar.ValueString);
			if (model != null)
			{
				model.OnDeath();
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Kill(this);
		}

		public override string ToString()
		{
			return "Kill Model=" + _ModelVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Kill");
			VectorLog.Tab(1);
			VectorLog.RunLog("Model", _ModelVar);
			VectorLog.Untab(1);
		}
	}
}
