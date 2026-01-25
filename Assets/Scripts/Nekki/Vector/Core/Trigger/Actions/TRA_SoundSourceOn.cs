using System.Xml;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_SoundSourceOn : TriggerRunnerAction
	{
		private Variable _SoundSourceNameVar;

		private Variable _EnableSoundVar;

		private Variable _VolumeFactorVar;

		private Variable _StartSoundFrameVar;

		private Variable _EndSoundFrameVar;

		private TRA_SoundSourceOn(TRA_SoundSourceOn p_copyAction)
			: base(p_copyAction)
		{
			_SoundSourceNameVar = p_copyAction._SoundSourceNameVar;
			_EnableSoundVar = p_copyAction._EnableSoundVar;
			_VolumeFactorVar = p_copyAction._VolumeFactorVar;
			_StartSoundFrameVar = p_copyAction._StartSoundFrameVar;
		}

		public TRA_SoundSourceOn(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _SoundSourceNameVar, p_node.Attributes["Name"].Value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _VolumeFactorVar, XmlUtils.ParseString(p_node.Attributes["VolumeFactor"], "1.0"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _StartSoundFrameVar, XmlUtils.ParseString(p_node.Attributes["StartFrame"], "0"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _EndSoundFrameVar, XmlUtils.ParseString(p_node.Attributes["EndFrame"], int.MaxValue.ToString()));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _EnableSoundVar, XmlUtils.ParseString(p_node.Attributes["Switch"], "On"));
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			foreach (SoundSourceRunner soundSource in _ParentLoop.ParentTrigger.ParentElements.SoundSources)
			{
				if (_SoundSourceNameVar.ValueString == soundSource.Name)
				{
					soundSource.EnableSoundSource(_EnableSoundVar.ValueString == "On", _VolumeFactorVar.ValueFloat, _StartSoundFrameVar.ValueInt, _EndSoundFrameVar.ValueInt);
				}
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_SoundSourceOn(this);
		}

		public override string ToString()
		{
			return "SoundSourceOn Name=" + _SoundSourceNameVar.DebugStringValue + " VolumeFactor=" + _VolumeFactorVar.DebugStringValue + " Frame=" + _SoundSourceNameVar.DebugStringValue + " Switch=" + _EnableSoundVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: SoundSourceOn");
			VectorLog.Tab(1);
			VectorLog.RunLog("SoundSourceName", _SoundSourceNameVar);
			VectorLog.RunLog("EnableSound", _EnableSoundVar);
			VectorLog.RunLog("VolumeFactor", _VolumeFactorVar);
			VectorLog.RunLog("StartSoundFrame", _StartSoundFrameVar);
			VectorLog.Untab(1);
		}
	}
}
