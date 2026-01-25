using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetUserProperty : VariableFunction
	{
		private Variable _PropertName;

		private Variable _AttributeName;

		public override int ValueInt
		{
			get
			{
				return DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).ValueInt(_AttributeName.ValueString);
			}
		}

		public override float ValueFloat
		{
			get
			{
				return DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).ValueFloat(_AttributeName.ValueString);
			}
		}

		public override string ValueString
		{
			get
			{
				return DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).ValueString(_AttributeName.ValueString);
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
				return text3 + "getUserProperties[" + _PropertName.ValueString + "]." + _AttributeName.ValueString;
			}
		}

		protected internal VF_GetUserProperty(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			string[] array = p_value.Split('.');
			int num = array[0].IndexOf("[") + 1;
			int num2 = array[0].LastIndexOf("]");
			string p_name2 = array[0].Substring(num, num2 - num);
			VariableFunction.InitFuncVar(p_parent, ref _AttributeName, array[1]);
			VariableFunction.InitFuncVar(p_parent, ref _PropertName, p_name2);
		}

		public override void SetValue(int p_value)
		{
			DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).SetValue(_AttributeName.ValueString, p_value);
			DataLocal.Current.Save(true);
		}

		public override void SetValue(float p_value)
		{
			DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).SetValue(_AttributeName.ValueString, p_value);
			DataLocal.Current.Save(true);
		}

		public override void SetValue(string p_value)
		{
			DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).SetValue(_AttributeName.ValueString, p_value);
			DataLocal.Current.Save(true);
		}

		public override void AppendValue(int p_value)
		{
			DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).AppendValue(_AttributeName.ValueString, p_value);
			DataLocal.Current.Save(true);
		}

		public override void AppendValue(float p_value)
		{
			DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).AppendValue(_AttributeName.ValueString, p_value);
			DataLocal.Current.Save(true);
		}

		public override void AppendValue(string p_value)
		{
			DataLocal.Current.GetUserPropertyByName(_PropertName.ValueString).AppendValue(_AttributeName.ValueString, p_value);
			DataLocal.Current.Save(true);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			_PropertName.Log("Property");
			VectorLog.Untab(1);
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _PropertName);
			SimplifyArgument(ref _AttributeName);
		}
	}
}
