namespace Nekki.Vector.Core.Variables
{
	public class VariableValue : Variable
	{
		private int _ValueInt;

		private float _ValueFloat;

		private string _ValueString;

		public override int ValueInt
		{
			get
			{
				return _ValueInt;
			}
		}

		public override float ValueFloat
		{
			get
			{
				return _ValueFloat;
			}
		}

		public override string ValueString
		{
			get
			{
				if (_Type != VariableType.String)
				{
					switch (_Type)
					{
					case VariableType.Int:
						return _ValueInt.ToString();
					case VariableType.Float:
						return _ValueFloat.ToString();
					}
				}
				return _ValueString;
			}
		}

		protected internal VariableValue(string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(string.Empty);
		}

		protected internal VariableValue(int p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(p_value);
		}

		protected internal VariableValue(float p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(p_value);
		}

		protected internal VariableValue(string p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(p_value);
		}

		public override void SetValue(int p_value)
		{
			_Type = VariableType.Int;
			_ValueInt = p_value;
			_ValueFloat = p_value;
			_ValueString = p_value.ToString();
		}

		public override void SetValue(float p_value)
		{
			_Type = VariableType.Float;
			_ValueInt = (int)p_value;
			_ValueFloat = p_value;
			_ValueString = p_value.ToString();
		}

		public override void SetValue(string p_value)
		{
			_Type = VariableType.String;
			_ValueInt = p_value.GetHashCode();
			_ValueFloat = _ValueInt;
			_ValueString = p_value;
		}

		public override void AppendValue(int p_value)
		{
			_ValueInt += p_value;
			_ValueFloat = _ValueInt;
		}

		public override void AppendValue(float p_value)
		{
			_ValueFloat += p_value;
			_ValueInt = (int)_ValueFloat;
		}

		public override void AppendValue(string p_value)
		{
			_ValueString += p_value;
			_ValueInt = _ValueString.GetHashCode();
			_ValueFloat = _ValueInt;
		}
	}
}
