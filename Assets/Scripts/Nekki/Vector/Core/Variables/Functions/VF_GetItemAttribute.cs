using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetItemAttribute : VariableFunction
	{
		private Variable _ItemName;

		private Variable _GroupName;

		private Variable _AtributeName;

		public override int ValueInt
		{
			get
			{
				return GetItem().GetIntValueAttribute(_AtributeName.ValueString, _GroupName.ValueString);
			}
		}

		public override float ValueFloat
		{
			get
			{
				return GetItem().GetFloatValueAttribute(_AtributeName.ValueString, _GroupName.ValueString);
			}
		}

		public override string ValueString
		{
			get
			{
				return GetItem().GetStrValueAttribute(_AtributeName.ValueString, _GroupName.ValueString);
			}
		}

		protected internal VF_GetItemAttribute(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			VariableFunction.InitFuncVar(p_parent, ref _ItemName, funcArgs[0]);
			VariableFunction.InitFuncVar(p_parent, ref _GroupName, funcArgs[1]);
			VariableFunction.InitFuncVar(p_parent, ref _AtributeName, funcArgs[2]);
		}

		private Item GetItem()
		{
			return DataLocal.Current.GetItemByName(_ItemName.ValueString);
		}
	}
}
