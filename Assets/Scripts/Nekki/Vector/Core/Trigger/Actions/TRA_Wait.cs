using System.Xml;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Wait : TriggerRunnerAction
	{
		private Variable _FrameVar;

		private int _CurrentFrame;

		public override int Frames
		{
			get
			{
				return _FrameVar.ValueInt;
			}
		}

		private TRA_Wait(TRA_Wait p_copyAction)
			: base(p_copyAction)
		{
			_CurrentFrame = p_copyAction._CurrentFrame;
			_FrameVar = p_copyAction._FrameVar;
		}

		public TRA_Wait(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FrameVar, p_node.Attributes["Frames"].Value);
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
			if (_CurrentFrame == 1)
			{
				base.Activate(ref p_isRunNext);
			}
			else if (Settings.WriteRunLogs && p_isRunNext && !_IsLastAction)
			{
				VectorLog.RunLog("OnTrigger:EXECUTING");
				VectorLog.Tab(1);
				VectorLog.RunLog(_ParentLoop.ParentTrigger);
				VectorLog.Tab(1);
				VectorLog.RunLog("TriggerPosition: " + string.Format("({0}, {1})", _ParentLoop.ParentTrigger.Position.x, _ParentLoop.ParentTrigger.Position.y));
				VectorLog.RunLog("LastSeenOnFrame: " + (Scene.FrameCount - _FrameVar.ValueInt - 1));
				VectorLog.RunLog("EXECUTE:");
				VectorLog.Tab(1);
				VectorLog.RunLog(_ParentLoop);
				VectorLog.Tab(1);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Wait(this);
		}

		public override string ToString()
		{
			return "Wait Frames=" + _FrameVar.DebugStringValue + " CurFrame=" + _CurrentFrame;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Wait");
			VectorLog.Tab(1);
			VectorLog.RunLog("Frame", _FrameVar);
			VectorLog.Untab(1);
		}
	}
}
