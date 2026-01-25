using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_ModelNode : VariableFunction
	{
		private delegate int ModelNodeTFGeter(ModelHuman p_model);

		private Variable _ParamVar;

		private ModelNodeTFGeter _Geter;

		protected internal VF_ModelNode(string p_value, string p_geterName, IVariableParent p_parent)
			: base(p_value, string.Empty, p_parent)
		{
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_name = p_value.Substring(num, num2 - num);
			VariableFunction.InitFuncVar(p_parent, ref _ParamVar, p_name);
			switch (p_geterName)
			{
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
			}
		}

		public int GetValue(ModelHuman p_model)
		{
			return _Geter(p_model);
		}

		private int GetLocalPositionX(ModelHuman p_model)
		{
			ModelNode modelNode = p_model.Node(_ParamVar.ValueString);
			return (int)(modelNode.Start.X - (_Parent as TriggerRunner).XQuad);
		}

		private int GetLocalPositionY(ModelHuman p_model)
		{
			ModelNode modelNode = p_model.Node(_ParamVar.ValueString);
			return (int)(modelNode.Start.Y - (_Parent as TriggerRunner).YQuad);
		}

		private int GetWorldPositionX(ModelHuman p_model)
		{
			ModelNode modelNode = p_model.Node(_ParamVar.ValueString);
			return (int)modelNode.Start.X;
		}

		private int GetWorldPositionY(ModelHuman p_model)
		{
			ModelNode modelNode = p_model.Node(_ParamVar.ValueString);
			return (int)modelNode.Start.Y;
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _ParamVar);
		}
	}
}
