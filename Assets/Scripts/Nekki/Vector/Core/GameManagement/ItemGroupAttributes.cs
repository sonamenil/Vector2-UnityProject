using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public class ItemGroupAttributes
	{
		public static readonly string NoIterableAttribute = "NoIterable";

		public static readonly string CardGroupInGadgetAttribute = "CardName";

		private Dictionary<string, Variable> _Attributes = new Dictionary<string, Variable>();

		private string _GroupName;

		public Item _ParentItem;

		public string GroupName
		{
			get
			{
				return _GroupName;
			}
		}

		public Item ParentItem
		{
			get
			{
				return _ParentItem;
			}
			set
			{
				_ParentItem = value;
				foreach (Variable value2 in _Attributes.Values)
				{
					value2.Parent = _ParentItem;
				}
			}
		}

		public bool IsNoIterable
		{
			get
			{
				int p_value = 0;
				TryGetIntValue(NoIterableAttribute, ref p_value);
				return p_value > 0;
			}
		}

		public ItemGroupAttributes(XmlNode p_node)
		{
			_GroupName = XmlUtils.ParseString(p_node.Attributes["Name"]);
			Variable.AllowCreateVariableObscured = true;
			foreach (XmlAttribute attribute in p_node.Attributes)
			{
				if (!(attribute.Name == "Name"))
				{
					_Attributes.Add(attribute.Name, Variable.CreateVariable(attribute.Value, attribute.Name));
				}
			}
			Variable.AllowCreateVariableObscured = false;
		}

		private ItemGroupAttributes(ItemGroupAttributes p_value)
		{
			_GroupName = p_value._GroupName;
			Variable.AllowCreateVariableObscured = true;
			foreach (KeyValuePair<string, Variable> attribute in p_value._Attributes)
			{
				_Attributes.Add(attribute.Key, Variable.CreateVariable(attribute.Value.ValueForSave, attribute.Key));
			}
			Variable.AllowCreateVariableObscured = false;
		}

		public ItemGroupAttributes(string p_name)
		{
			_GroupName = p_name;
		}

		public void ParseFromYaml(Mapping p_node, Item p_item)
		{
			Variable.AllowCreateVariableObscured = true;
			foreach (Nekki.Yaml.Node item in p_node.nodesInside)
			{
				AddAttribute(item.key, Variable.CreateVariable(item.value.ToString(), item.key, p_item));
			}
			Variable.AllowCreateVariableObscured = false;
		}

		public void AddAttribute(string p_name, Variable p_value)
		{
			if (_Attributes.ContainsKey(p_name))
			{
				Debug.Log("-->>" + p_name);
			}
			switch (p_value.Type)
			{
			case VariableType.Function:
			{
				VariableFunction variableFunction = p_value as VariableFunction;
				if (variableFunction.IsPointer)
				{
					variableFunction.SimplifyArguments();
					_Attributes.Add(p_name, p_value);
				}
				else
				{
					_Attributes.Add(p_name, Variable.CreateVariable(variableFunction.ValueString, p_name));
				}
				break;
			}
			case VariableType.Expression:
			{
				VariableExpression variableExpression = p_value as VariableExpression;
				_Attributes.Add(p_name, Variable.CreateVariable(variableExpression.ValueFloat.ToString(), p_name));
				break;
			}
			default:
				_Attributes.Add(p_name, p_value);
				break;
			}
		}

		public void AppendByYaml(Mapping p_node)
		{
			Variable.AllowCreateVariableObscured = true;
			foreach (Nekki.Yaml.Node item in p_node.nodesInside)
			{
				if (_Attributes.ContainsKey(item.key))
				{
					ReplaceValueByName(item.key, item.value.ToString());
				}
				else
				{
					AddAttribute(item.key, Variable.CreateVariable(item.value.ToString(), item.key, _ParentItem));
				}
			}
			Variable.AllowCreateVariableObscured = false;
		}

		public void AppendByAttribute(ItemGroupAttributes p_attribute, bool p_replace = true)
		{
			Variable.AllowCreateVariableObscured = true;
			foreach (KeyValuePair<string, Variable> attribute in p_attribute._Attributes)
			{
				if (_Attributes.ContainsKey(attribute.Key))
				{
					if (p_replace)
					{
						ReplaceValueByName(attribute.Key, attribute.Value.ValueString);
					}
					else
					{
						AppendValueByName(attribute.Key, attribute.Value);
					}
				}
				else
				{
					AddAttribute(attribute.Key, attribute.Value);
				}
			}
			Variable.AllowCreateVariableObscured = false;
		}

		public ItemGroupAttributes CreateCopy()
		{
			return new ItemGroupAttributes(this);
		}

		private void AppendValueByName(string p_name, Variable p_value)
		{
			Variable variable = _Attributes[p_name];
			switch (variable.Type)
			{
			case VariableType.Int:
			case VariableType.ObscuredInt:
				variable.AppendValue(p_value.ValueInt);
				break;
			case VariableType.Float:
			case VariableType.ObscuredFloat:
				variable.AppendValue(p_value.ValueFloat);
				break;
			case VariableType.String:
			case VariableType.ObscuredString:
				variable.AppendValue(p_value.ValueString);
				break;
			default:
				DebugUtils.Dialog("Can not append attributes: " + _GroupName + "Variable type: " + variable.Type, true);
				break;
			}
		}

		private void ReplaceValueByName(string p_name, string p_value)
		{
			Variable variable = _Attributes[p_name];
			switch (Variable.GetTypeByNameAndValue(p_name, p_value))
			{
			case VariableType.Int:
				variable.SetValue(int.Parse(p_value));
				break;
			case VariableType.Float:
				variable.SetValue(float.Parse(p_value));
				break;
			case VariableType.String:
				variable.SetValue(p_value);
				break;
			case VariableType.Function:
			{
				VariableFunction variableFunction = Variable.CreateVariable(p_value, p_name, _ParentItem) as VariableFunction;
				if (variableFunction.IsPointer)
				{
					variableFunction.SimplifyArguments();
					_Attributes[p_name] = variableFunction;
				}
				else if (variableFunction.IsStringFunction)
				{
					_Attributes[p_name].SetValue(variableFunction.ValueString);
				}
				else
				{
					_Attributes[p_name].SetValue(variableFunction.ValueInt);
				}
				break;
			}
			case VariableType.Expression:
			{
				VariableExpression variableExpression = Variable.CreateVariable(p_value, p_name) as VariableExpression;
				variableExpression.Parent = _ParentItem;
				_Attributes[p_name].SetValue(variableExpression.ValueFloat);
				break;
			}
			case VariableType.Item:
				break;
			case VariableType.LocalizationString:
			{
				VariableLocalizationString variableLocalizationString = Variable.CreateVariable(p_value, p_name) as VariableLocalizationString;
				_Attributes[p_name].SetValue(variableLocalizationString.ValueString);
				break;
			}
			}
		}

		public void ReplaceGroup(Mapping attributesMapping)
		{
			_Attributes.Clear();
			foreach (Nekki.Yaml.Node item in attributesMapping.nodesInside)
			{
				Variable variable = Variable.CreateVariable(item.value.ToString(), string.Empty);
				if (item.key == "NewName")
				{
					_GroupName = variable.ValueString;
				}
				else
				{
					AddAttribute(item.key, variable);
				}
			}
		}

		public void SaveToXml(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Group");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Name", _GroupName);
			foreach (KeyValuePair<string, Variable> attribute in _Attributes)
			{
				xmlElement.SetAttribute(attribute.Key, attribute.Value.ValueForSave);
			}
		}

		public void Trace()
		{
		}

		public bool HasValue(string p_name)
		{
			return _Attributes.ContainsKey(p_name);
		}

		public void RemoveAttribute(string p_name)
		{
			if (_Attributes.ContainsKey(p_name))
			{
				_Attributes.Remove(p_name);
			}
		}

		public bool TryGetIntValue(string p_name, ref int p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			p_value = _Attributes[p_name].ValueInt;
			return true;
		}

		public bool TryGetFloatValue(string p_name, ref float p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			p_value = _Attributes[p_name].ValueFloat;
			return true;
		}

		public bool TryGetStrValue(string p_name, ref string p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			p_value = _Attributes[p_name].ValueString;
			return true;
		}

		public bool TrySetValue(string p_name, int p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			_Attributes[p_name].SetValue(p_value);
			return true;
		}

		public bool TrySetValue(string p_name, float p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			_Attributes[p_name].SetValue(p_value);
			return true;
		}

		public bool TrySetValue(string p_name, string p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			_Attributes[p_name].SetValue(p_value);
			return true;
		}

		public bool TryAppendValue(string p_name, int p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			_Attributes[p_name].AppendValue(p_value);
			return true;
		}

		public bool TryAppendValue(string p_name, float p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			_Attributes[p_name].AppendValue(p_value);
			return true;
		}

		public bool TryAppendValue(string p_name, string p_value)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			_Attributes[p_name].AppendValue(p_value);
			return true;
		}

		public bool TryChangeVarByName(string p_name, Variable p_var)
		{
			if (!_Attributes.ContainsKey(p_name))
			{
				return false;
			}
			_Attributes[p_name] = p_var;
			return true;
		}

		public string GetStringValue(string p_name)
		{
			return _Attributes[p_name].ValueString;
		}

		public void ChangeCryptoKeyOnVars()
		{
			foreach (Variable value in _Attributes.Values)
			{
				if (value.IsObscured)
				{
					((VariableObscured)value).ChangeCryptoKey();
				}
			}
		}
	}
}
