using System.Xml;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Transformation : TriggerRunnerAction
	{
		private Variable _NameVar;

		private Variable _StopVar;

		private Variable _ObjectsToTransform;

		private int _Frame;

		private int _CurrentFrame;

		public override int Frames
		{
			get
			{
				return _ParentLoop.ParentTrigger.ParentElements.Parent.GetTransformationFrame(_NameVar.ValueString);
			}
		}

		private TRA_Transformation(TRA_Transformation p_copyAction)
			: base(p_copyAction)
		{
			_NameVar = p_copyAction._NameVar;
			_StopVar = p_copyAction._StopVar;
			_CurrentFrame = p_copyAction._CurrentFrame;
			_Frame = p_copyAction._Frame;
			_ObjectsToTransform = p_copyAction._ObjectsToTransform;
		}

		public TRA_Transformation(XmlNode p_node, TriggerRunnerLoop p_parent, bool p_wait)
			: base(p_parent)
		{
			_CurrentFrame = -1;
			_Frame = 0;
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _NameVar, p_node.Attributes["Name"].Value);
			if (p_node.Attributes["Stop"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _StopVar, p_node.Attributes["Stop"].Value);
			}
			if (p_node.Attributes["ObjectsToTransform"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ObjectsToTransform, p_node.Attributes["ObjectsToTransform"].Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			if (_StopVar != null)
			{
				base.Activate(ref p_isRunNext);
				p_isRunNext = true;
				_ParentLoop.ParentTrigger.ParentElements.Parent.StopTranformation(_NameVar.ValueString);
				return;
			}
			if (_CurrentFrame == -1)
			{
				if (_ObjectsToTransform != null)
				{
					_Frame = _ParentLoop.ParentTrigger.ParentElements.Parent.RunTranformation(_NameVar.ValueString, _ObjectsToTransform.ValueInt) + 1;
				}
				else
				{
					_Frame = _ParentLoop.ParentTrigger.ParentElements.Parent.RunTranformation(_NameVar.ValueString) + 1;
				}
				_CurrentFrame = 0;
			}
			if (_CurrentFrame <= _Frame)
			{
				_CurrentFrame++;
				p_isRunNext = false;
			}
			if (_CurrentFrame > _Frame)
			{
				_CurrentFrame = -1;
				p_isRunNext = true;
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
				if (_ParentLoop.ParentTrigger.UnityObject != null)
				{
					VectorLog.RunLog("TriggerPosition: " + string.Format("({0}, {1})", _ParentLoop.ParentTrigger.Position.x, _ParentLoop.ParentTrigger.Position.y));
				}
				else
				{
					VectorLog.RunLog("ParentTrigger.UnityObject == null for trigger: " + _ParentLoop.ParentTrigger.Name);
					DebugUtils.Log("ParentTrigger.UnityObject == null for trigger: " + _ParentLoop.ParentTrigger.Name);
				}
				VectorLog.RunLog("LastSeenOnFrame: " + (Scene.FrameCount - _Frame));
				VectorLog.RunLog("EXECUTE:");
				VectorLog.Tab(1);
				VectorLog.RunLog(_ParentLoop);
				VectorLog.Tab(1);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Transformation(this);
		}

		public override string ToString()
		{
			return "Transformation Name=" + _NameVar.DebugStringValue + ((_StopVar == null) ? string.Empty : " Stop:1") + " Frames=" + _Frame + " CurrentFrame=" + _CurrentFrame;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Transformation");
			VectorLog.Tab(1);
			VectorLog.RunLog("Name", _NameVar);
			VectorLog.RunLog("Stop", _StopVar);
			VectorLog.Untab(1);
		}
	}
}
