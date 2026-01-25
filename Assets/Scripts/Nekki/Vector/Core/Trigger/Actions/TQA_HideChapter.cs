using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_HideChapter : TriggerQuestAction
	{
		public TQA_HideChapter(TriggerQuestLoop p_parent)
			: base(p_parent)
		{
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			ChapterWindow.HideContent();
		}

		public override string ToString()
		{
			return "TQA_HideChapter";
		}
	}
}
