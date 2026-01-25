namespace Nekki.Vector.Core.Variables
{
	public abstract class Variable
	{
		protected string _Name;

		protected IVariableParent _Parent;

		protected VariableType _Type;

		public static bool AllowCreateVariableObscured { get; set; }

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public virtual IVariableParent Parent
		{
			set
			{
				_Parent = value;
			}
		}

		public virtual VariableType Type
		{
			get
			{
				return _Type;
			}
		}

		public bool IsIntVar
		{
			get
			{
				return _Type == VariableType.Int;
			}
		}

		public bool IsFloatVar
		{
			get
			{
				return _Type == VariableType.Float;
			}
		}

		public bool IsStringVar
		{
			get
			{
				return _Type == VariableType.String;
			}
		}

		public bool IsItemVar
		{
			get
			{
				return _Type == VariableType.Item;
			}
		}

		public bool IsFunctionVar
		{
			get
			{
				return _Type == VariableType.Function;
			}
		}

		public bool IsExpressionVar
		{
			get
			{
				return _Type == VariableType.Expression;
			}
		}

		public bool IsLocalizationString
		{
			get
			{
				return _Type == VariableType.LocalizationString;
			}
		}

		public bool IsObscuredInt
		{
			get
			{
				return _Type == VariableType.ObscuredInt;
			}
		}

		public bool IsObscuredFloat
		{
			get
			{
				return _Type == VariableType.ObscuredFloat;
			}
		}

		public bool IsObscuredString
		{
			get
			{
				return _Type == VariableType.ObscuredString;
			}
		}

		public bool IsObscured
		{
			get
			{
				switch (_Type)
				{
				case VariableType.ObscuredInt:
				case VariableType.ObscuredFloat:
				case VariableType.ObscuredString:
					return true;
				default:
					return false;
				}
			}
		}

		public virtual int ValueInt
		{
			get
			{
				return 0;
			}
		}

		public virtual float ValueFloat
		{
			get
			{
				return 0f;
			}
		}

		public virtual string ValueString
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string DebugStringValue
		{
			get
			{
				switch (_Type)
				{
				case VariableType.Int:
					return ValueInt.ToString();
				case VariableType.Float:
					return ValueFloat.ToString();
				case VariableType.String:
					return ValueString;
				default:
					return "Unknown value";
				}
			}
		}

		public virtual string ValueForSave
		{
			get
			{
				return DebugStringValue;
			}
		}

		protected Variable()
		{
		}

		protected Variable(string p_name)
		{
			_Type = VariableType.String;
			_Name = p_name;
			_Parent = null;
		}

		protected Variable(string p_name, IVariableParent p_parent)
		{
			_Type = VariableType.String;
			_Name = p_name;
			_Parent = p_parent;
		}

		public static Variable CreateVariable(string p_value, string p_name = "", IVariableParent p_parent = null)
		{
			Variable variable = null;
			VariableType variableType = GetTypeByNameAndValue(p_name, p_value);
			if (AllowCreateVariableObscured)
			{
				switch (variableType)
				{
				case VariableType.Int:
					variableType = VariableType.ObscuredInt;
					break;
				case VariableType.Float:
					variableType = VariableType.ObscuredFloat;
					break;
				case VariableType.String:
					variableType = VariableType.ObscuredString;
					break;
				}
			}
			switch (variableType)
			{
			case VariableType.Int:
				variable = new VariableValue(int.Parse(p_value), p_name, p_parent);
				break;
			case VariableType.Float:
				variable = new VariableValue(float.Parse(p_value), p_name, p_parent);
				break;
			case VariableType.String:
				variable = new VariableValue(p_value, p_name, p_parent);
				break;
			case VariableType.Function:
				variable = VariableFunction.Create(p_value, p_name, p_parent);
				break;
			case VariableType.Expression:
				variable = new VariableExpression(p_value, p_name, p_parent);
				break;
			case VariableType.Item:
				variable = new VariableItem(p_name, p_parent);
				break;
			case VariableType.LocalizationString:
				variable = new VariableLocalizationString(p_value, p_name, p_parent);
				break;
			case VariableType.ObscuredInt:
				variable = new VariableObscured(int.Parse(p_value), p_name, p_parent);
				break;
			case VariableType.ObscuredFloat:
				variable = new VariableObscured(float.Parse(p_value), p_name, p_parent);
				break;
			case VariableType.ObscuredString:
				variable = new VariableObscured(p_value, p_name, p_parent);
				break;
			}
			if (variable == null)
			{
				DebugUtils.Dialog("Error create var by string = \"" + p_value + "\"", true);
			}
			return variable;
		}

		public virtual void SetValue(int p_value)
		{
		}

		public virtual void SetValue(float p_value)
		{
		}

		public virtual void SetValue(string p_value)
		{
		}

		public virtual void AppendValue(int p_value)
		{
		}

		public virtual void AppendValue(float p_value)
		{
		}

		public virtual void AppendValue(string p_value)
		{
		}

		public bool IsEqual(Variable p_value)
		{
			if (ValueInt == p_value.ValueInt)
			{
				return true;
			}
			return false;
		}

		public bool IsGreater(Variable p_value)
		{
			if (ValueInt > p_value.ValueInt)
			{
				return true;
			}
			return false;
		}

		public bool IsLess(Variable p_value)
		{
			if (ValueInt < p_value.ValueInt)
			{
				return true;
			}
			return false;
		}

		public bool IsLessEqual(Variable p_value)
		{
			if (ValueInt <= p_value.ValueInt)
			{
				return true;
			}
			return false;
		}

		public bool IsGreaterEqual(Variable p_value)
		{
			if (ValueInt >= p_value.ValueInt)
			{
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return _Name + " Type: " + _Type.ToString() + " Value: " + DebugStringValue;
		}

		public static VariableType GetTypeByNameAndValue(string p_name, string p_value)
		{
			if (p_name != null && p_name.Length != 0 && p_name[0] == '#')
			{
				return VariableType.Item;
			}
			if (p_value.Length == 0)
			{
				return VariableType.String;
			}
			if (p_value[0] == '^' && p_value[p_value.Length - 1] == '^')
			{
				return VariableType.LocalizationString;
			}
			if (p_value[0] >= '+' && p_value[0] <= '9' && p_value[0] != '/')
			{
				if (p_value.IndexOf('.') == -1)
				{
					return VariableType.Int;
				}
				return VariableType.Float;
			}
			if (p_value[0] == '?')
			{
				return VariableType.Function;
			}
			if (p_value[0] == '{' && p_value[p_value.Length - 1] == '}')
			{
				return VariableType.Expression;
			}
			if (p_value[0] == '<' && p_value[p_value.Length - 1] == '>')
			{
				return VariableType.Function;
			}
			return VariableType.String;
		}

		public virtual void Log(string name)
		{
			VectorLog.RunLog((!string.IsNullOrEmpty(Name)) ? string.Format("{0}: {1}: {2}", name, Name, ValueString) : string.Format("{0}: {1}", name, ValueString));
		}
	}
}
