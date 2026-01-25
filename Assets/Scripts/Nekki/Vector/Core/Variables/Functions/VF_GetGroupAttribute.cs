using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetGroupAttribute : VariableFunction
	{
		private Variable _GroupName;

		private Variable _AtributeName;

		public override int ValueInt
		{
			get
			{
				return (_Parent as Item).GetIntValueAttribute(_AtributeName.ValueString, _GroupName.ValueString);
			}
		}

		public override float ValueFloat
		{
			get
			{
				return (_Parent as Item).GetFloatValueAttribute(_AtributeName.ValueString, _GroupName.ValueString);
			}
		}

		public override string ValueString
		{
			get
			{
				return (_Parent as Item).GetStrValueAttribute(_AtributeName.ValueString, _GroupName.ValueString);
			}
		}

		protected internal VF_GetGroupAttribute(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			VariableFunction.InitFuncVar(p_parent, ref _GroupName, funcArgs[0]);
			VariableFunction.InitFuncVar(p_parent, ref _AtributeName, funcArgs[1]);
		}
	}
}
