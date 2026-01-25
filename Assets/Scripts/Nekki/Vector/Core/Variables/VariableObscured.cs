using CodeStage.AntiCheat.ObscuredTypes;

namespace Nekki.Vector.Core.Variables
{
	public class VariableObscured : Variable
	{
		private ObscuredInt _ValueInt;

		private ObscuredFloat _ValueFloat;

		private ObscuredString _StrValue;

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
				if (_Type != VariableType.ObscuredString)
				{
					switch (_Type)
					{
					case VariableType.ObscuredInt:
						return _ValueInt.ToString();
					case VariableType.ObscuredFloat:
						return _ValueFloat.ToString();
					}
				}
				return _StrValue;
			}
		}

		public override string DebugStringValue
		{
			get
			{
				switch (_Type)
				{
				case VariableType.ObscuredInt:
					return ValueInt.ToString();
				case VariableType.ObscuredFloat:
					return ValueFloat.ToString();
				case VariableType.ObscuredString:
					return ValueString;
				default:
					return "Unknown value";
				}
			}
		}

		protected internal VariableObscured(string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(string.Empty);
		}

		protected internal VariableObscured(int p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(p_value);
		}

		protected internal VariableObscured(float p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(p_value);
		}

		protected internal VariableObscured(string p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			SetValue(p_value);
		}

		public override void SetValue(int p_value)
		{
			_Type = VariableType.ObscuredInt;
			_ValueInt = p_value;
			_ValueFloat = p_value;
			_StrValue = p_value.ToString();
		}

		public override void SetValue(float p_value)
		{
			_Type = VariableType.ObscuredFloat;
			_ValueInt = (int)p_value;
			_ValueFloat = p_value;
			_StrValue = p_value.ToString();
		}

		public override void SetValue(string p_value)
		{
			_Type = VariableType.ObscuredString;
			_ValueInt = p_value.GetHashCode();
			_ValueFloat = (int)_ValueInt;
			_StrValue = p_value;
		}

		public override void AppendValue(int p_value)
		{
			_ValueInt = (int)_ValueInt + p_value;
			_ValueFloat = (int)_ValueInt;
			_StrValue = _ValueInt.ToString();
		}

		public override void AppendValue(float p_value)
		{
			_ValueFloat = (float)_ValueFloat + p_value;
			_ValueInt = (int)(float)_ValueFloat;
			_StrValue = _ValueFloat.ToString();
		}

		public override void AppendValue(string p_value)
		{
			_StrValue = string.Concat(_StrValue, p_value);
			_ValueInt = _StrValue.GetHashCode();
			_ValueFloat = (int)_ValueInt;
		}

		public void ChangeCryptoKey()
		{
			_ValueInt.RandomizeCryptoKey();
			_ValueFloat.RandomizeCryptoKey();
			_StrValue.RandomizeCryptoKey();
		}
	}
}
