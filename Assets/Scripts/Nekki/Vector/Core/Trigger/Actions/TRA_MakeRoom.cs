using System.Xml;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_MakeRoom : TriggerRunnerAction
	{
		public TRA_MakeRoom(TRA_MakeRoom p_copyAction)
			: base(p_copyAction)
		{
		}

		public TRA_MakeRoom(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			RunMainController.Location.AddRandomRoom();
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_MakeRoom(this);
		}

		public override string ToString()
		{
			return "MakeRoom";
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: MakeRoom");
		}
	}
}
