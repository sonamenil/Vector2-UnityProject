using System.Collections.Generic;
using System.Text;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_Switch : VariableFunction
	{
		private Variable _Value;

		private Variable _Def;

		private List<Variable> _Cases;

		private List<Variable> _Values;

		public override int ValueInt
		{
			get
			{
				return GetSwitchResult().ValueInt;
			}
		}

		public override float ValueFloat
		{
			get
			{
				return GetSwitchResult().ValueFloat;
			}
		}

		public override string ValueString
		{
			get
			{
				return GetSwitchResult().ValueString;
			}
		}

		public override string DebugStringValue
		{
			get
			{
				return ValueForSave;
			}
		}

		public override string ValueForSave
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("?");
				if (_IsPointer)
				{
					stringBuilder.Append("_");
				}
				stringBuilder.Append("Switch[");
				stringBuilder.Append(_Value.ValueString + ", ");
				int i = 0;
				for (int count = _Cases.Count; i < count; i++)
				{
					stringBuilder.Append(_Cases[i].ValueForSave + ":" + _Values[i].ValueForSave + ", ");
				}
				stringBuilder.Append(_Def.ValueString + "]");
				return stringBuilder.ToString();
			}
		}

		protected internal VF_Switch(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			_Cases = new List<Variable>();
			_Values = new List<Variable>();
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			ParseAgs(p_value2);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			_Value.Log("Value");
			int i = 0;
			for (int count = _Cases.Count; i < count; i++)
			{
				_Cases[i].Log("Case" + i);
				_Values[i].Log("Value" + i);
			}
			_Def.Log("Def");
			VectorLog.Untab(1);
		}

		private void ParseAgs(string p_value)
		{
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value, ',', new string[1] { "[]" });
			if (funcArgs.Count < 2)
			{
				DebugUtils.Dialog(string.Format("VF_Switch not enought args: {0} ", p_value), true);
				return;
			}
			VariableFunction.InitFuncVar(_Parent, ref _Value, funcArgs[0]);
			if (_Value == null)
			{
				DebugUtils.Dialog(string.Format("VF_Switch error in value: {0} ", funcArgs[0]), true);
				return;
			}
			VariableFunction.InitFuncVar(_Parent, ref _Def, funcArgs[funcArgs.Count - 1]);
			if (_Def == null)
			{
				DebugUtils.Dialog(string.Format("VF_Switch error in def: {0} ", funcArgs[funcArgs.Count - 1]), true);
				return;
			}
			Variable p_var = null;
			int i = 1;
			for (int num = funcArgs.Count - 1; i < num; i++)
			{
				List<string> funcArgs2 = VariableFunction.GetFuncArgs(funcArgs[i], ':', new string[1] { "[]" });
				if (funcArgs2.Count >= 2)
				{
					VariableFunction.InitFuncVar(_Parent, ref p_var, funcArgs2[0]);
					_Cases.Add(p_var);
					VariableFunction.InitFuncVar(_Parent, ref p_var, funcArgs2[1]);
					_Values.Add(p_var);
				}
			}
		}

		private Variable GetSwitchResult()
		{
			int valueInt = _Value.ValueInt;
			int i = 0;
			for (int count = _Cases.Count; i < count; i++)
			{
				if (_Cases[i].ValueInt == valueInt)
				{
					return _Values[i];
				}
			}
			return _Def;
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _Value);
			SimplifyArgument(ref _Def);
			for (int i = 0; i < _Cases.Count; i++)
			{
				Variable argument = _Cases[i];
				SimplifyArgument(ref argument);
				_Cases[i] = argument;
			}
			for (int j = 0; j < _Values.Count; j++)
			{
				Variable argument2 = _Values[j];
				SimplifyArgument(ref argument2);
				_Values[j] = argument2;
			}
		}
	}
}
