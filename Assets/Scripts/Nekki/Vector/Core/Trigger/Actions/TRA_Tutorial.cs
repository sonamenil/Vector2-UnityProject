using System.Xml;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Tutorial : TriggerRunnerAction
	{
		private Variable _Key;

		private Variable _Lock;

		private Variable _TextMobile;

		private Variable _TextKeyboard;

		private TRA_Tutorial(TRA_Tutorial p_copyAction)
			: base(p_copyAction)
		{
			_Key = p_copyAction._Key;
			_Lock = p_copyAction._Lock;
		}

		public TRA_Tutorial(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			if (p_node.Attributes["Key"] != null)
			{
				string p_nameOrValue = string.Empty;
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Key, p_node.Attributes["Key"].Value);
				if (p_node != null)
				{
					p_nameOrValue = p_node.Attributes["TextMobile"].Value.Replace("(nl)", "\n");
				}
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _TextMobile, p_nameOrValue);
				if (p_node != null)
				{
					p_nameOrValue = p_node.Attributes["TextKeyboard"].Value.Replace("(nl)", "\n");
				}
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _TextKeyboard, p_nameOrValue);
			}
			if (p_node.Attributes["LockGame"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Lock, p_node.Attributes["LockGame"].Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			if (_Key != null)
			{
				ControllerTutorial.ShowTutorial(_Key.ValueString, _TextMobile.ValueString, _TextKeyboard.ValueString, _Lock != null && _Lock.ValueInt != 0);
			}
			else
			{
				ControllerTutorial.LockGame(_Lock != null && _Lock.ValueInt != 0);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Tutorial(this);
		}

		public override string ToString()
		{
			return "Tutorial";
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Tutorial");
		}
	}
}
