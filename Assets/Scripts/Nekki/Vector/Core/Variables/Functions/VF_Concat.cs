using System.Collections.Generic;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_Concat : VariableFunction
	{
		private string _StaticString;

		private List<Variable> _Vars = new List<Variable>();

		public override string ValueString
		{
			get
			{
				if (!_IsPointer)
				{
					return _StaticString;
				}
				return CreateString();
			}
		}

		public override string ValueForSave
		{
			get
			{
				string text = string.Empty;
				if (_IsPointer)
				{
					text = "_";
				}
				string text2 = "?" + text;
				text2 += "concat['";
				text2 += _StaticString;
				text2 += "',";
				for (int i = 0; i < _Vars.Count; i++)
				{
					text2 = text2 + _Vars[i].ValueForSave + ",";
				}
				return text2.Remove(text2.Length - 1, 1) + "]";
			}
		}

		protected internal VF_Concat(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			_IsStringFunction = true;
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string text = p_value.Substring(num, num2 - num);
			int num3 = text.IndexOf("'", text.IndexOf("'") + 1);
			if (num3 == -1)
			{
				num3 = text.IndexOf(",");
				_StaticString = text.Substring(0, num3);
				text = text.Substring(num3 + 1);
			}
			else
			{
				_StaticString = text.Substring(1, num3 - 1);
				text = text.Substring(num3 + 2);
			}
			text = VariableFunction.TrimSpaces(text);
			List<string> funcArgs = VariableFunction.GetFuncArgs(text, ',', new string[2] { "[]", "{}" });
			int num4 = 1;
			for (int i = 0; i < funcArgs.Count; i++)
			{
				if (funcArgs[i][0] == '?')
				{
					VariableFunction variableFunction = Variable.CreateVariable(funcArgs[i], string.Empty, p_parent) as VariableFunction;
					if (variableFunction.IsPointer)
					{
						_IsPointer = true;
						_StaticString = _StaticString.Replace("%" + (i + 1), "%" + num4);
						num4++;
						_Vars.Add(variableFunction);
					}
					else
					{
						_StaticString = _StaticString.Replace("%" + (i + 1), variableFunction.ValueString);
					}
				}
				else
				{
					Variable p_var = null;
					VariableFunction.InitFuncVar(p_parent, ref p_var, funcArgs[i]);
					_IsPointer = true;
					_StaticString = _StaticString.Replace("%" + (i + 1), "%" + num4);
					num4++;
					_Vars.Add(p_var);
				}
			}
		}

		private string CreateString()
		{
			string text = _StaticString;
			for (int i = 0; i < _Vars.Count; i++)
			{
				text = text.Replace("%" + (i + 1), _Vars[i].ValueString);
			}
			return text;
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			for (int i = 0; i < _Vars.Count; i++)
			{
				_Vars[i].Log("Value" + i);
			}
			VectorLog.Untab(1);
		}

		public override void SimplifyArguments()
		{
			for (int i = 0; i < _Vars.Count; i++)
			{
				Variable argument = _Vars[i];
				SimplifyArgument(ref argument);
				_Vars[i] = argument;
			}
		}
	}
}
