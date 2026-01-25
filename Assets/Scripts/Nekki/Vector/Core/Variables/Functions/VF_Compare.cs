using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_Compare : VariableFunction
	{
		public delegate bool CompareFunc(Variable p_value1, Variable p_value2);

		private Variable _Value1;

		private Variable _Value2;

		private CompareFunc _CompareFunc;

		public override int ValueInt
		{
			get
			{
				return _CompareFunc(_Value1, _Value2) ? 1 : 0;
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
				return ValueString;
			}
		}

		public override string ValueForSave
		{
			get
			{
				string text = ((!_IsPointer) ? "?" : "?_");
				string text2 = text;
				return text2 + _Name + "[" + _Value1.ValueForSave + ":" + _Value2.ValueForSave + "]";
			}
		}

		protected internal VF_Compare(string p_value, string p_name, IVariableParent p_parent, CompareFunc p_compareFunc)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			VariableFunction.InitFuncVar(p_parent, ref _Value1, funcArgs[0]);
			VariableFunction.InitFuncVar(p_parent, ref _Value2, funcArgs[1]);
			_CompareFunc = p_compareFunc;
		}

		public static bool Less(Variable p_value1, Variable p_value2)
		{
			return p_value1.ValueFloat < p_value2.ValueFloat;
		}

		public static bool LessEqual(Variable p_value1, Variable p_value2)
		{
			float valueFloat = p_value1.ValueFloat;
			float valueFloat2 = p_value2.ValueFloat;
			return valueFloat < valueFloat2 || Mathf.Approximately(valueFloat, valueFloat2);
		}

		public static bool Greater(Variable p_value1, Variable p_value2)
		{
			return p_value1.ValueFloat > p_value2.ValueFloat;
		}

		public static bool GreaterEqual(Variable p_value1, Variable p_value2)
		{
			float valueFloat = p_value1.ValueFloat;
			float valueFloat2 = p_value2.ValueFloat;
			return valueFloat > valueFloat2 || Mathf.Approximately(valueFloat, valueFloat2);
		}

		public static bool Equal(Variable p_value1, Variable p_value2)
		{
			return Mathf.Approximately(p_value1.ValueFloat, p_value2.ValueFloat);
		}

		public static bool NotEqual(Variable p_value1, Variable p_value2)
		{
			return !Mathf.Approximately(p_value1.ValueFloat, p_value2.ValueFloat);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			_Value1.Log("Value1");
			_Value2.Log("Value2");
			VectorLog.Untab(1);
		}
	}
}
