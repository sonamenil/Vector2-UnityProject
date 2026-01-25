using System.Collections.Generic;
using System.Text;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_MinMax : VariableFunction
	{
		private List<Variable> _Values;

		private bool _IsMax;

		public override int ValueInt
		{
			get
			{
				return (!_IsMax) ? GetMin() : GetMax();
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

		public override string ValueForSave
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append((!_IsPointer) ? "?" : "?_");
				stringBuilder.Append((!_IsMax) ? "min[" : "max[");
				for (int i = 0; i < _Values.Count; i++)
				{
					stringBuilder.Append(_Values[i].ValueForSave);
					if (i != _Values.Count - 1)
					{
						stringBuilder.Append(",");
					}
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
		}

		protected internal VF_MinMax(string p_value, string p_name, IVariableParent p_parent, bool p_isMax)
			: base(p_value, p_name, p_parent)
		{
			_IsMax = p_isMax;
			_Values = new List<Variable>();
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			Variable p_var = null;
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			for (int i = 0; i < funcArgs.Count; i++)
			{
				VariableFunction.InitFuncVar(_Parent, ref p_var, funcArgs[i]);
				_Values.Add(p_var);
			}
		}

		private int GetMax()
		{
			int num = int.MinValue;
			for (int i = 0; i < _Values.Count; i++)
			{
				int valueInt = _Values[i].ValueInt;
				if (valueInt > num)
				{
					num = valueInt;
				}
			}
			return num;
		}

		private int GetMin()
		{
			int num = int.MaxValue;
			for (int i = 0; i < _Values.Count; i++)
			{
				int valueInt = _Values[i].ValueInt;
				if (valueInt < num)
				{
					num = valueInt;
				}
			}
			return num;
		}
	}
}
