using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Nekki.Vector.Core.User
{
	public class UserProperty
	{
		public class UserAttribyte
		{
			private enum UserAttribytType
			{
				UAT_INT = 0,
				UAT_FLOAT = 1,
				UAT_STRING = 2
			}

			private string _Name;

			private ObscuredInt _ValueInt;

			private ObscuredFloat _ValueFloat;

			private ObscuredString _ValueString;

			private UserAttribytType _Type;

			public string Name
			{
				get
				{
					return _Name;
				}
			}

			public ObscuredInt ValueInt
			{
				get
				{
					return _ValueInt;
				}
			}

			public ObscuredFloat ValueFloat
			{
				get
				{
					return _ValueFloat;
				}
			}

			public ObscuredString ValueString
			{
				get
				{
					return _ValueString;
				}
			}

			public UserAttribyte(string p_name, string p_value)
			{
				_Name = p_name;
				switch (GetTypeByValue(p_value))
				{
				case UserAttribytType.UAT_INT:
					SetValue(int.Parse(p_value));
					break;
				case UserAttribytType.UAT_FLOAT:
					SetValue(float.Parse(p_value));
					break;
				case UserAttribytType.UAT_STRING:
					SetValue(p_value);
					break;
				}
			}

			private UserAttribyte(UserAttribyte p_copy)
			{
				_Name = p_copy._Name;
				_ValueInt = p_copy._ValueInt;
				_ValueFloat = p_copy._ValueFloat;
				_ValueString = p_copy._ValueString;
			}

			private static UserAttribytType GetTypeByValue(string p_value)
			{
				if (p_value.Length == 0)
				{
					return UserAttribytType.UAT_STRING;
				}
				if (IsAllCharsNumber(p_value))
				{
					if (p_value.IndexOf('.') == -1)
					{
						return UserAttribytType.UAT_INT;
					}
					return UserAttribytType.UAT_FLOAT;
				}
				return UserAttribytType.UAT_STRING;
			}

			private static bool IsAllCharsNumber(string p_string)
			{
				for (int i = 0; i < p_string.Length; i++)
				{
					if (p_string[i] < '+' || p_string[i] > '9')
					{
						return false;
					}
				}
				return true;
			}

			public UserAttribyte Copy()
			{
				return new UserAttribyte(this);
			}

			public void SetValue(int p_value)
			{
				_Type = UserAttribytType.UAT_INT;
				_ValueInt = p_value;
				_ValueFloat = p_value;
				_ValueString = p_value.ToString();
			}

			public void SetValue(float p_value)
			{
				_Type = UserAttribytType.UAT_FLOAT;
				_ValueFloat = p_value;
				_ValueInt = (int)p_value;
				_ValueString = p_value.ToString();
			}

			public void SetValue(string p_value)
			{
				_Type = UserAttribytType.UAT_STRING;
				_ValueString = p_value;
				_ValueInt = p_value.GetHashCode();
			}

			public void SetUntypedValue(string p_value)
			{
				switch (GetTypeByValue(p_value))
				{
				case UserAttribytType.UAT_INT:
					SetValue(int.Parse(p_value));
					break;
				case UserAttribytType.UAT_FLOAT:
					SetValue(float.Parse(p_value));
					break;
				case UserAttribytType.UAT_STRING:
					SetValue(p_value);
					break;
				}
			}

			public void AppendValue(int p_value)
			{
				_Type = UserAttribytType.UAT_INT;
				_ValueInt = (int)_ValueInt + p_value;
				_ValueFloat = (int)_ValueInt;
				_ValueString = _ValueInt.ToString();
			}

			public void AppendValue(float p_value)
			{
				_Type = UserAttribytType.UAT_FLOAT;
				_ValueFloat = (float)_ValueFloat + p_value;
				_ValueInt = (int)(float)_ValueFloat;
				_ValueString = _ValueFloat.ToString();
			}

			public void AppendValue(string p_value)
			{
				_Type = UserAttribytType.UAT_INT;
				_ValueString = string.Concat(_ValueString, p_value);
			}

			public void SaveToXml(XmlElement p_node)
			{
				p_node.SetAttribute(_Name, _ValueString);
			}
		}

		private string _Name;

		private Dictionary<string, UserAttribyte> _Attribytes = new Dictionary<string, UserAttribyte>();

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public UserProperty(string p_name)
		{
			_Name = p_name;
		}

		private UserProperty(UserProperty p_copy)
		{
			_Name = p_copy._Name;
			foreach (KeyValuePair<string, UserAttribyte> attribyte in p_copy._Attribytes)
			{
				_Attribytes.Add(attribyte.Key, attribyte.Value.Copy());
			}
		}

		public UserProperty(XmlNode p_node)
		{
			_Name = p_node.Name;
			foreach (XmlAttribute attribute in p_node.Attributes)
			{
				UserAttribyte userAttribyte = new UserAttribyte(attribute.Name, attribute.Value);
				_Attribytes.Add(userAttribyte.Name, userAttribyte);
			}
		}

		public UserProperty Copy()
		{
			return new UserProperty(this);
		}

		public void SaveToXml(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement(_Name);
			p_node.AppendChild(xmlElement);
			foreach (UserAttribyte value in _Attribytes.Values)
			{
				value.SaveToXml(xmlElement);
			}
		}

		public void AddAttribute(string p_name, string p_value)
		{
			UserAttribyte value = new UserAttribyte(p_name, p_value);
			_Attribytes.Add(p_name, value);
		}

		public void AddAttributeOrSetValue(string p_name, string p_value)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				UserAttribyte value = new UserAttribyte(p_name, p_value);
				_Attribytes.Add(p_name, value);
			}
			else
			{
				_Attribytes[p_name].SetValue(p_value);
			}
		}

		public ObscuredInt ValueInt(string p_name)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: Get Int Value In Property Name:" + _Name + " By Name:" + p_name, true);
				return -1;
			}
			return _Attribytes[p_name].ValueInt;
		}

		public ObscuredFloat ValueFloat(string p_name)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: Get float Value In Property Name:" + _Name + " By Name:" + p_name, true);
				return -1f;
			}
			return _Attribytes[p_name].ValueFloat;
		}

		public ObscuredString ValueString(string p_name)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: Get string Value In Property Name:" + _Name + " By Name:" + p_name, true);
				return string.Empty;
			}
			return _Attribytes[p_name].ValueString;
		}

		public void SetValue(string p_name, int p_value)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: SetValue int In Property Name:" + _Name + " By Name:" + p_name, true);
			}
			else
			{
				_Attribytes[p_name].SetValue(p_value);
			}
		}

		public void SetValue(string p_name, float p_value)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: SetValue float In Property Name:" + _Name + " By Name:" + p_name, true);
			}
			else
			{
				_Attribytes[p_name].SetValue(p_value);
			}
		}

		public void SetValue(string p_name, string p_value)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: SetValue string In Property Name:" + _Name + " By Name:" + p_name, true);
			}
			else
			{
				_Attribytes[p_name].SetValue(p_value);
			}
		}

		public void AppendValue(string p_name, int p_value)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: AppendValue int In Property Name:" + _Name + " By Name:" + p_name, true);
			}
			else
			{
				_Attribytes[p_name].AppendValue(p_value);
			}
		}

		public void AppendValue(string p_name, float p_value)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: AppendValue float In Property Name:" + _Name + " By Name:" + p_name, true);
			}
			else
			{
				_Attribytes[p_name].AppendValue(p_value);
			}
		}

		public void AppendValue(string p_name, string p_value)
		{
			if (!_Attribytes.ContainsKey(p_name))
			{
				DebugUtils.Dialog("Error: AppendValue string In Property Name:" + _Name + " By Name:" + p_name, true);
			}
			else
			{
				_Attribytes[p_name].AppendValue(p_value);
			}
		}
	}
}
