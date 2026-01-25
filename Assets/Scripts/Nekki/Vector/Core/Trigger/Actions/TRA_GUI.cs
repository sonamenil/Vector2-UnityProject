using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Run;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_GUI : TriggerRunnerAction
	{
		private Variable _Action;

		private TRA_GUI(TRA_GUI p_copyAction)
			: base(p_copyAction)
		{
			_Action = p_copyAction._Action;
		}

		public TRA_GUI(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Action, p_node.Attributes["Action"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			switch (_Action.ValueString)
			{
			case "ShowFounds":
			{
				HudPanel module = UIModule.GetModule<HudPanel>();
				if (module != null)
				{
					module.FundsPanel.ShowAllFunds();
				}
				break;
			}
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_GUI(this);
		}

		public override string ToString()
		{
			return "TRA_GUI Action=" + _Action.ValueString;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: GUI");
			VectorLog.Tab(1);
			VectorLog.RunLog("Action", _Action);
			VectorLog.Untab(1);
		}
	}
}
