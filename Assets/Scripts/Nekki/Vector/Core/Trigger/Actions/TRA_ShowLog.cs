using System.Xml;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_ShowLog : TriggerRunnerAction
	{
		private TRA_ShowLog(TRA_ShowLog p_copyAction)
			: base(p_copyAction)
		{
		}

		public TRA_ShowLog(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_ShowLog(this);
		}

		private void GetLogString(ref string result)
		{
		}

		public override string ToString()
		{
			return "ShowLog";
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: ShowLog");
		}
	}
}
