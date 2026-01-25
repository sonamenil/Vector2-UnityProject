using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_IsItemType : VariableFunction
	{
		private Variable _ItemName;

		private Variable _TypeName;

		public override int ValueInt
		{
			get
			{
				return (GetItem().Type.ToString() == _TypeName.ValueString) ? 1 : 0;
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

		protected internal VF_IsItemType(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			VariableFunction.InitFuncVar(p_parent, ref _ItemName, funcArgs[0]);
			VariableFunction.InitFuncVar(p_parent, ref _TypeName, funcArgs[1]);
		}

		private Item GetItem()
		{
			return DataLocal.Current.GetItemByName(_ItemName.ValueString);
		}
	}
}
