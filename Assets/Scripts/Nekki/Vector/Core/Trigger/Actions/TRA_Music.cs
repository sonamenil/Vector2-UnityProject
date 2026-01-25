using System.Xml;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Music : TriggerRunnerAction
	{
		private Variable _ActionVar;

		private Variable _TimeVar;

		private Variable _TrackVar;

		private TRA_Music(TRA_Music p_copyAction)
			: base(p_copyAction)
		{
			_ActionVar = p_copyAction._ActionVar;
			_TimeVar = p_copyAction._TimeVar;
			_TrackVar = p_copyAction._TrackVar;
		}

		public TRA_Music(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ActionVar, XmlUtils.ParseString(p_node.Attributes["Action"], "Play"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _TimeVar, XmlUtils.ParseString(p_node.Attributes["Time"], "0.0"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _TrackVar, XmlUtils.ParseString(p_node.Attributes["Track"], string.Empty));
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			switch (_ActionVar.ValueString)
			{
			case "Play":
				AudioManager.PlayMusic(_TrackVar.ValueString);
				break;
			case "Stop":
				AudioManager.StopMusic(_TimeVar.ValueFloat);
				break;
			case "Pause":
				AudioManager.PauseMusic(true);
				break;
			case "Resume":
				AudioManager.PauseMusic(false);
				break;
			}
			AudioManager.UpdateAmbientVolume();
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Music(this);
		}

		public override string ToString()
		{
			return "Music: Action=" + _ActionVar.DebugStringValue + " Time=" + _TimeVar.ValueFloat + " Track=" + _TimeVar.ValueFloat;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Music");
			VectorLog.Tab(1);
			VectorLog.RunLog("Action", _ActionVar);
			VectorLog.RunLog("Time", _TimeVar);
			VectorLog.RunLog("Track", _TrackVar);
			VectorLog.Untab(1);
		}
	}
}
