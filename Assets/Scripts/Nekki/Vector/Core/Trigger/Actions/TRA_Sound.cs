using System.Xml;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Sound : TriggerRunnerAction
	{
		private Variable _ActionVar;

		private Variable _ChannelVar;

		private Variable _NameVar;

		private Variable _VolumeVar;

		private Variable _DuckEffectVar;

		private Variable _TimeVar;

		private TRA_Sound(TRA_Sound p_copyAction)
			: base(p_copyAction)
		{
			_ActionVar = p_copyAction._ActionVar;
			_ChannelVar = p_copyAction._ChannelVar;
			_NameVar = p_copyAction._NameVar;
			_VolumeVar = p_copyAction._VolumeVar;
			_DuckEffectVar = p_copyAction._DuckEffectVar;
			_TimeVar = p_copyAction._TimeVar;
		}

		public TRA_Sound(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ActionVar, XmlUtils.ParseString(p_node.Attributes["Action"], "Play"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ChannelVar, XmlUtils.ParseString(p_node.Attributes["Channel"], "Sound"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _NameVar, XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _VolumeVar, XmlUtils.ParseString(p_node.Attributes["Volume"], "1.0"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _DuckEffectVar, XmlUtils.ParseString(p_node.Attributes["DuckEffect"], "0"));
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _TimeVar, XmlUtils.ParseString(p_node.Attributes["Time"], "0.0"));
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			switch (_ActionVar.ValueString)
			{
			case "Play":
				PlayChannel(_ChannelVar.ValueString, _NameVar.ValueString, _VolumeVar.ValueFloat, _DuckEffectVar.ValueInt == 1);
				break;
			case "Stop":
				StopChannel(_ChannelVar.ValueString, _TimeVar.ValueFloat);
				break;
			}
		}

		private void PlayChannel(string p_channel, string p_name, float p_volume, bool p_duckEffect)
		{
			switch (p_channel)
			{
			case "Sound":
				if (p_duckEffect)
				{
					AudioManager.PlaySoundDuck(p_name, p_volume);
				}
				else
				{
					AudioManager.PlaySound(p_name, p_volume);
				}
				break;
			case "Cutscene":
				AudioManager.PlayCutscene(p_name, p_volume);
				break;
			case "Ambient":
				AudioManager.PlayAmbient(p_name, p_volume);
				break;
			}
		}

		private void StopChannel(string p_channel, float p_timeout)
		{
			switch (p_channel)
			{
			case "Sound":
				AudioManager.StopSound(p_timeout);
				break;
			case "Cutscene":
				AudioManager.StopCutscene(p_timeout);
				break;
			case "Ambient":
				AudioManager.StopAmbient(p_timeout);
				break;
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Sound(this);
		}

		public override string ToString()
		{
			return "Sound: Action:" + _ActionVar.DebugStringValue + " Channel:" + _ChannelVar.DebugStringValue + " Name:" + _NameVar.DebugStringValue + " Volume:" + _VolumeVar.ValueFloat + " DuckEffect:" + _DuckEffectVar.ValueInt + " Time:" + _TimeVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Sound");
			VectorLog.Tab(1);
			VectorLog.RunLog("Action", _ActionVar);
			VectorLog.RunLog("Channel", _ChannelVar);
			VectorLog.RunLog("Name", _NameVar);
			VectorLog.RunLog("Volume", _VolumeVar);
			VectorLog.RunLog("DuckEffect", _DuckEffectVar);
			VectorLog.RunLog("Time", _TimeVar);
			VectorLog.Untab(1);
		}
	}
}
