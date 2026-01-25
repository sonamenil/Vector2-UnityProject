using System.Xml;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Camera : TriggerRunnerAction
	{
		private Variable _FollowVar;

		private Variable _ZoomVar;

		private Variable _SmoothnessVar;

		private Variable _FramesVar;

		private Variable _IsStopVar;

		private Variable _OffsetXVar;

		private Variable _OffsetYVar;

		private TRA_Camera(TRA_Camera p_copyAction)
			: base(p_copyAction)
		{
			_FollowVar = p_copyAction._FollowVar;
			_ZoomVar = p_copyAction._ZoomVar;
			_SmoothnessVar = p_copyAction._SmoothnessVar;
			_FramesVar = p_copyAction._FramesVar;
			_IsStopVar = p_copyAction._IsStopVar;
		}

		public TRA_Camera(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			XmlAttribute xmlAttribute = p_node.Attributes["Zoom"];
			XmlAttribute xmlAttribute2 = p_node.Attributes["Smoothness"];
			XmlAttribute xmlAttribute3 = p_node.Attributes["Frames"];
			XmlAttribute xmlAttribute4 = p_node.Attributes["Follow"];
			XmlAttribute xmlAttribute5 = p_node.Attributes["Stop"];
			XmlAttribute xmlAttribute6 = p_node.Attributes["OffsetX"];
			XmlAttribute xmlAttribute7 = p_node.Attributes["OffsetY"];
			if (xmlAttribute != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ZoomVar, xmlAttribute.Value);
			}
			if (xmlAttribute2 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _SmoothnessVar, xmlAttribute2.Value);
			}
			if (xmlAttribute3 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FramesVar, xmlAttribute3.Value);
			}
			if (xmlAttribute4 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FollowVar, xmlAttribute4.Value);
			}
			if (xmlAttribute5 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _IsStopVar, xmlAttribute5.Value);
			}
			if (xmlAttribute6 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _OffsetXVar, xmlAttribute6.Value);
			}
			if (xmlAttribute7 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _OffsetYVar, xmlAttribute7.Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			if (_FollowVar != null)
			{
				string valueString = _FollowVar.ValueString;
				ModelHuman model = GetModel(valueString);
				if (model != null)
				{
					Nekki.Vector.Core.Camera.Camera.Current.CameraNode = model.ModelObject.CameraNode;
				}
				else
				{
					Nekki.Vector.Core.Camera.Camera.Current.Stop();
				}
			}
			if (_SmoothnessVar != null)
			{
				switch (_SmoothnessVar.Type)
				{
				case VariableType.Int:
					Nekki.Vector.Core.Camera.Camera.FluencyCurrent = _SmoothnessVar.ValueInt;
					break;
				case VariableType.Float:
					Nekki.Vector.Core.Camera.Camera.FluencyCurrent = _SmoothnessVar.ValueFloat;
					break;
				}
			}
			if (_ZoomVar != null)
			{
				float currentZoom = Nekki.Vector.Core.Camera.Camera.CurrentZoom;
				switch (_ZoomVar.Type)
				{
				case VariableType.Int:
					Nekki.Vector.Core.Camera.Camera.Current.Zooming((float)_ZoomVar.ValueInt * currentZoom);
					break;
				case VariableType.Float:
					Nekki.Vector.Core.Camera.Camera.Current.Zooming(_ZoomVar.ValueFloat * currentZoom);
					break;
				}
			}
			if (_IsStopVar != null)
			{
				Nekki.Vector.Core.Camera.Camera.Current.Stop();
			}
			if (_OffsetXVar != null)
			{
				Nekki.Vector.Core.Camera.Camera.SetHorizotalValue(_OffsetXVar.ValueFloat);
			}
			if (_OffsetYVar != null)
			{
				Nekki.Vector.Core.Camera.Camera.SetVerticalValue(_OffsetYVar.ValueFloat);
			}
			if (_FramesVar != null)
			{
				Nekki.Vector.Core.Camera.Camera.SetFramesCount(_FramesVar.ValueInt);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Camera(this);
		}

		public override string ToString()
		{
			string text = "Camera:";
			if (_FollowVar != null)
			{
				text = text + " Follow: " + _FollowVar.DebugStringValue + "|";
			}
			if (_SmoothnessVar != null)
			{
				switch (_SmoothnessVar.Type)
				{
				case VariableType.Int:
					Nekki.Vector.Core.Camera.Camera.FluencyCurrent = _SmoothnessVar.ValueInt;
					break;
				case VariableType.Float:
					Nekki.Vector.Core.Camera.Camera.FluencyCurrent = _SmoothnessVar.ValueFloat;
					break;
				}
				text = text + " Smoothness: " + _SmoothnessVar.DebugStringValue;
			}
			if (_ZoomVar != null)
			{
				text = text + " Zoom: " + _ZoomVar.DebugStringValue + "|";
			}
			if (_IsStopVar != null)
			{
				text += "Stop: 1";
			}
			return text;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Camera");
			VectorLog.Tab(1);
			VectorLog.RunLog("Follow", _FollowVar);
			VectorLog.RunLog("Zoom", _ZoomVar);
			VectorLog.RunLog("Smoothness", _SmoothnessVar);
			VectorLog.RunLog("Frame", _FramesVar);
			VectorLog.RunLog("IsStop", _IsStopVar);
			VectorLog.RunLog("OffetX", _OffsetXVar);
			VectorLog.RunLog("OffsetY", _OffsetYVar);
			VectorLog.Untab(1);
		}
	}
}
