namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_QuestComplete : TriggerQuestAction
	{
		public TQA_QuestComplete(TriggerQuestLoop p_parent)
			: base(p_parent)
		{
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			_Parent.Parent.Parent.QuestCompleted();
		}
	}
}
