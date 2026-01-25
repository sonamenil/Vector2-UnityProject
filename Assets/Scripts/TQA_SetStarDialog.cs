using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.GUI.Dialogs;

public class TQA_SetStarDialog : TriggerQuestAction
{
	public TQA_SetStarDialog(TriggerQuestLoop p_parent)
		: base(p_parent)
	{
	}

	public override void Activate(ref bool p_runNext)
	{
		p_runNext = true;
		if ((int)CounterController.Current.CounterShowDoYouLikeGameDialog > 0)
		{
			DialogNotificationManager.ShowDoYouLikeGameDialog(3);
		}
		else if ((int)CounterController.Current.CounterShowSetStarDialog > 0)
		{
			DialogNotificationManager.ShowSetStarDialog(3);
		}
	}
}
