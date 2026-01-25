using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetCounter : VariableFunction
	{
		private Variable _CounterName;

		private Variable _CounterNamespace;

		public override int ValueInt
		{
			get
			{
				return CounterController.Current.GetUserCounter(_CounterName.ValueString, _CounterNamespace.ValueString);
			}
		}

		public override float ValueFloat
		{
			get
			{
				return (int)CounterController.Current.GetUserCounter(_CounterName.ValueString, _CounterNamespace.ValueString);
			}
		}

		public override string ValueString
		{
			get
			{
				return CounterController.Current.GetUserCounter(_CounterName.ValueString, _CounterNamespace.ValueString).ToString();
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
				string text3 = text2;
				return text3 + "getCounter[" + _CounterName.ValueString + ":" + _CounterNamespace.ValueString + "]";
			}
		}

		protected internal VF_GetCounter(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			bool flag = p_value[0] == '<';
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = ((!flag) ? (p_value.IndexOf("[") + 1) : (p_value.IndexOf("<") + 1));
			string text = p_value.Substring(num, ((!flag) ? p_value.LastIndexOf("]") : p_value.LastIndexOf(">")) - num);
			if (flag)
			{
				string[] array = text.Split('.');
				VariableFunction.InitFuncVar(p_parent, ref _CounterName, (array.Length != 2) ? array[0] : array[1]);
				VariableFunction.InitFuncVar(p_parent, ref _CounterNamespace, (array.Length != 2) ? "ST_Default" : array[0]);
			}
			else
			{
				List<string> funcArgs = VariableFunction.GetFuncArgs(text, ',', new string[2] { "[]", "{}" });
				VariableFunction.InitFuncVar(p_parent, ref _CounterName, (funcArgs.Count != 2) ? funcArgs[0] : funcArgs[1]);
				VariableFunction.InitFuncVar(p_parent, ref _CounterNamespace, (funcArgs.Count != 2) ? "ST_Default" : funcArgs[0]);
			}
		}

		public override void SetValue(int p_value)
		{
			CounterController.Current.CreateCounterOrSetValue(_CounterName.ValueString, p_value, _CounterNamespace.ValueString);
			DataLocal.Current.Save(false);
		}

		public override void SetValue(float p_value)
		{
			SetValue((int)p_value);
		}

		public override void AppendValue(int p_value)
		{
			CounterController.Current.IncrementUserCounter(_CounterName.ValueString, p_value, _CounterNamespace.ValueString);
			DataLocal.Current.Save(false);
		}

		public override void AppendValue(float p_value)
		{
			AppendValue((int)p_value);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			_CounterName.Log("Counter");
			VectorLog.Untab(1);
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _CounterName);
			SimplifyArgument(ref _CounterNamespace);
		}
	}
}
