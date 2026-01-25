using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TPEA_Wait : TriggerPassiveEffectAction
	{
		public const string NodeName = "Wait";

		private Variable _FrameVar;

		private int _CurrentFrame;

		public override int Frames
		{
			get
			{
				return _FrameVar.ValueInt;
			}
		}

		private TPEA_Wait(TPEA_Wait p_copyAction)
			: base(p_copyAction)
		{
			_CurrentFrame = p_copyAction._CurrentFrame;
			_FrameVar = p_copyAction._FrameVar;
		}

		public TPEA_Wait(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
			: base(p_parent)
		{
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _FrameVar, p_node.Attributes["Frames"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			if (_CurrentFrame > _FrameVar.ValueInt)
			{
				p_isRunNext = true;
				_CurrentFrame = 0;
			}
			else
			{
				_CurrentFrame++;
				p_isRunNext = false;
			}
		}

		public override TriggerPassiveEffectAction Copy()
		{
			return new TPEA_Wait(this);
		}

		public override string ToString()
		{
			return "Wait Frames=" + _FrameVar.DebugStringValue + " CurFrame=" + _CurrentFrame;
		}
	}
}
