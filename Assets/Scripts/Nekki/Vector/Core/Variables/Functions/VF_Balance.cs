using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_Balance : VariableFunction
	{
		private const int _SearchDepth = 1;

		private List<Variable> _Params = new List<Variable>();

		private int _DefaultableNode = -1;

		public override int ValueInt
		{
			get
			{
				return TryGetString().ValueInt;
			}
		}

		public override float ValueFloat
		{
			get
			{
				return TryGetString().ValueFloat;
			}
		}

		public override string ValueString
		{
			get
			{
				return TryGetString().ValueString;
			}
		}

		public override string DebugStringValue
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
				string text = string.Empty;
				if (_IsPointer)
				{
					text = "_";
				}
				string text2 = "?" + text;
				text2 += "getBalance[";
				for (int i = 0; i < _Params.Count; i++)
				{
					text2 = text2 + _Params[i].ValueString + ",";
				}
				return text2.Remove(text2.Length - 1, 1) + "]";
			}
		}

		protected internal VF_Balance(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			_IsStringFunction = true;
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			for (int i = 0; i < funcArgs.Count; i++)
			{
				if (funcArgs[i][0] == '$')
				{
					_DefaultableNode = int.Parse(funcArgs[i].Substring(1));
					continue;
				}
				Variable p_var = null;
				VariableFunction.InitFuncVar(p_parent, ref p_var, funcArgs[i]);
				_Params.Add(p_var);
			}
		}

		private List<Variable> GetSearchRequest()
		{
			return _Params.GetRange(0, Math.Min(1, _Params.Count - 1));
		}

		private string GetBalanceValue()
		{
			if (ZoneResource<ZoneBalanceManager>.Current.HasBalanceElement(GetSearchRequest(), false, _DefaultableNode))
			{
				return ZoneResource<ZoneBalanceManager>.Current.GetBalance(_Params, _DefaultableNode);
			}
			return BalanceManager.Current.GetBalance(_Params, _DefaultableNode);
		}

		private Variable TryGetString()
		{
			return Variable.CreateVariable(GetBalanceValue(), string.Empty);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			VectorLog.Untab(1);
		}

		public override void SimplifyArguments()
		{
			for (int i = 0; i < _Params.Count; i++)
			{
				if (_Params[i].Type == VariableType.Function)
				{
					VariableFunction variableFunction = _Params[i] as VariableFunction;
					variableFunction.SimplifyArguments();
					if (variableFunction.IsPointer)
					{
						continue;
					}
				}
				_Params[i] = Variable.CreateVariable(_Params[i].ValueString, _Params[i].Name);
			}
		}
	}
}
