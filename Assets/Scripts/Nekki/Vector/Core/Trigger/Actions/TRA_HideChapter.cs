namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_HideChapter : TriggerRunnerAction
	{
		private TRA_HideChapter(TRA_HideChapter p_copy)
			: base(p_copy)
		{
		}

		public TRA_HideChapter(TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
		}

		public override string ToString()
		{
			return "TQA_HideChapter";
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_HideChapter(this);
		}
	}
}
