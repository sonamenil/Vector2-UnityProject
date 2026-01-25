using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_ChangeExit : TriggerRunnerAction
	{
		private Variable _Name;

		private TRA_ChangeExit(TRA_ChangeExit p_copy)
			: base(p_copy)
		{
			_Name = p_copy._Name;
		}

		public TRA_ChangeExit(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Name, p_node.Attributes["Name"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			RunMainController.Location.Sets.ChangeOut(_Name.ValueString, (int)GetModel().Node("COM").Start.X);
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_ChangeExit(this);
		}

		public override string ToString()
		{
			return "ChangeExit Name=" + _Name.ValueString;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: ChangeExit");
			VectorLog.Tab(1);
			VectorLog.RunLog("Name", _Name);
			VectorLog.Untab(1);
		}
	}
}
