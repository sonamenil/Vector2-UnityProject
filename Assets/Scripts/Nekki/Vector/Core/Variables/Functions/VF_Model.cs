using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_Model : VariableFunction
	{
		private delegate int ModelTFGeter();

		private VF_ModelNode _NodeFunc;

		private Variable paramVar;

		private ModelTFGeter _Geter;

		public override int ValueInt
		{
			get
			{
				if (_NodeFunc != null)
				{
					return _NodeFunc.GetValue(GetModel());
				}
				return _Geter();
			}
		}

		public override float ValueFloat
		{
			get
			{
				return ValueInt;
			}
		}

		public override string ValueString
		{
			get
			{
				return ValueInt.ToString();
			}
		}

		public override string DebugStringValue
		{
			get
			{
				ModelHuman model = GetModel();
				if (model == null)
				{
					return "Model=null";
				}
				return ValueInt.ToString();
			}
		}

		protected internal VF_Model(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			_NodeFunc = null;
			_Geter = null;
			string[] array = p_value.Split('.');
			int num = array[0].IndexOf("[") + 1;
			int num2 = array[0].LastIndexOf("]");
			string p_name2 = p_value.Substring(num, num2 - num);
			VariableFunction.InitFuncVar(p_parent, ref paramVar, p_name2);
			string text = array[1];
			switch (text)
			{
			case "AI":
				_Geter = GetAI;
				break;
			case "localPositionX":
				_Geter = GetLocalPositionX;
				break;
			case "localPositionY":
				_Geter = GetLocalPositionY;
				break;
			case "worldPositionX":
				_Geter = GetWorldPositionX;
				break;
			case "worldPositionY":
				_Geter = GetWorldPositionY;
				break;
			case "direction":
				_Geter = GetDirection;
				break;
			case "isControlled":
				_Geter = GetIsControlled;
				break;
			case "isCameraFollow":
				_Geter = GetIsCameraFollow;
				break;
			case "animationName":
				_Geter = GetAnimationName;
				break;
			case "animationFrame":
				_Geter = GetAnimationFrame;
				break;
			case "condition":
				_Geter = GetCondition;
				break;
			default:
				if (text.IndexOf("getNode") != -1)
				{
					_NodeFunc = new VF_ModelNode(text, array[2], p_parent);
				}
				break;
			}
			if (_Geter == null && _NodeFunc == null)
			{
				DebugUtils.Dialog("TF_Func init error by geterName = " + text, true);
			}
		}

		public ModelHuman GetModel()
		{
			ModelHuman modelByName = RunMainController.Location.GetModelByName(paramVar.ValueString);
			if (modelByName == null)
			{
			}
			return modelByName;
		}

		private int GetAI()
		{
			return GetModel().AI;
		}

		private int GetLocalPositionX()
		{
			ModelNode modelNode = GetModel().Node("COM");
			return (int)(modelNode.Start.X - (_Parent as TriggerRunner).XQuad);
		}

		private int GetLocalPositionY()
		{
			ModelNode modelNode = GetModel().Node("COM");
			return (int)(modelNode.Start.Y - (_Parent as TriggerRunner).YQuad);
		}

		private int GetWorldPositionX()
		{
			ModelNode modelNode = GetModel().Node("COM");
			return (int)modelNode.Start.X;
		}

		private int GetWorldPositionY()
		{
			ModelNode modelNode = GetModel().Node("COM");
			return (int)modelNode.Start.Y;
		}

		private int GetDirection()
		{
			ModelNode modelNode = GetModel().Node("COM");
			return (modelNode.Start.Subtract(modelNode.End).X > 0f) ? 1 : (-1);
		}

		private int GetCondition()
		{
			return GetModel().IntValueOfState;
		}

		private int GetAnimationName()
		{
			return GetModel().ControllerAnimation.Name.GetHashCode();
		}

		private int GetAnimationFrame()
		{
			return GetModel().ControllerAnimation.CurrentFrame;
		}

		private int GetIsControlled()
		{
			return GetModel().ControllerControl.Enable ? 1 : 0;
		}

		private int GetIsCameraFollow()
		{
			return Nekki.Vector.Core.Camera.Camera.Current.CameraNode.Equals(GetModel().ModelObject.CameraNode) ? 1 : 0;
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			paramVar.Log("Model");
			if (_NodeFunc != null)
			{
				VectorLog.Tab(1);
				_NodeFunc.Log("NodeFunc");
				VectorLog.Untab(1);
			}
			VectorLog.Untab(1);
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref paramVar);
		}
	}
}
