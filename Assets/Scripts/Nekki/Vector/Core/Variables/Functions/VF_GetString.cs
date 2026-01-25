using Nekki.Vector.Core.GameManagement;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetString : VariableFunction
	{
		private Variable _StringName;

		public override int ValueInt
		{
			get
			{
				int hashCode = StringBuffer.GetString(_StringName.ValueString).GetHashCode();
				int result;
				return (!int.TryParse(StringBuffer.GetString(_StringName.ValueString), out result)) ? hashCode : result;
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
				return StringBuffer.GetString(_StringName.ValueString);
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
				string text = string.Empty;
				if (_IsPointer)
				{
					text = "_";
				}
				string text2 = "?" + text;
				return text2 + "getString[" + _StringName.ValueString + "]";
			}
		}

		protected internal VF_GetString(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			_IsStringFunction = true;
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_name2 = p_value.Substring(num, num2 - num);
			VariableFunction.InitFuncVar(p_parent, ref _StringName, p_name2);
		}

		public override void SetValue(string p_value)
		{
			StringBuffer.AddString(_StringName.ValueString, p_value);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			_StringName.Log("StringName");
			VectorLog.Untab(1);
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _StringName);
		}
	}
}
