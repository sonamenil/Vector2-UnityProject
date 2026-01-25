using System;
using System.Collections.Generic;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_Round : VariableFunction
	{
		private Variable _Value;

		private Variable _Pow;

		public override int ValueInt
		{
			get
			{
				return (int)GetRounded();
			}
		}

		public override float ValueFloat
		{
			get
			{
				return GetRounded();
			}
		}

		public override string ValueString
		{
			get
			{
				return GetRounded().ToString();
			}
		}

		protected internal VF_Round(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			if (funcArgs.Count != 2)
			{
				DebugUtils.Dialog("Round need 2 args", true);
			}
			VariableFunction.InitFuncVar(_Parent, ref _Value, funcArgs[0]);
			VariableFunction.InitFuncVar(_Parent, ref _Pow, funcArgs[1]);
		}

		private float GetRounded()
		{
			return (float)Math.Round(_Value.ValueFloat, _Pow.ValueInt);
		}
	}
}
