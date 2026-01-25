using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Runners
{
	public class ObjectReference
	{
		private XmlNode _Node;

		private Dictionary<string, string> _DefaultValues;

		private Dictionary<string, string> _Expressions;

		private static Dictionary<string, string> _TempValues;

		private int _TempHash = -1;

		private Dictionary<int, XmlNode> _CachedNodes = new Dictionary<int, XmlNode>();

		public ObjectReference(XmlNode p_node)
		{
			_Node = p_node;
			if (_Node["Properties"] == null || _Node["Properties"]["Static"] == null || _Node["Properties"]["Static"]["ContentVariable"] == null)
			{
				return;
			}
			_DefaultValues = new Dictionary<string, string>();
			XmlNode xmlNode = _Node["Properties"]["Static"]["ContentVariable"];
			if (xmlNode["Expression"] != null)
			{
				_Expressions = new Dictionary<string, string>();
			}
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				switch (childNode.Name)
				{
				case "Variable":
				case "Constant":
					_DefaultValues.Add(XmlUtils.ParseString(childNode.Attributes["Name"]), XmlUtils.ParseString(childNode.Attributes["Default"]));
					break;
				case "Expression":
					_Expressions.Add(XmlUtils.ParseString(childNode.Attributes["Name"]), XmlUtils.ParseString(childNode.Attributes["Value"]));
					_DefaultValues.Add(XmlUtils.ParseString(childNode.Attributes["Name"]), string.Empty);
					break;
				}
			}
		}

		public XmlNode GetXmlNode(XmlNode p_settings, Dictionary<string, string> p_choices)
		{
			if (_DefaultValues == null)
			{
				return _Node;
			}
			_TempHash = p_settings.InnerXml.GetHashCode();
			if (_CachedNodes.ContainsKey(_TempHash))
			{
				return _CachedNodes[_TempHash];
			}
			Dictionary<string, string> dictionary = _DefaultValues;
			if (p_settings != null && p_settings["Properties"] != null && p_settings["Properties"]["Static"] != null && p_settings["Properties"]["Static"]["OverrideVariable"] != null)
			{
				dictionary = new Dictionary<string, string>(_DefaultValues);
				XmlNode xmlNode = p_settings["Properties"]["Static"]["OverrideVariable"];
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					if (CheckSelections(childNode, p_choices))
					{
						dictionary[XmlUtils.ParseString(childNode.Attributes["Name"])] = XmlUtils.ParseString(childNode.Attributes["Value"]);
					}
				}
			}
			CalcExpression(dictionary);
			return GetXmlNode(dictionary);
		}

		private static bool CheckSelections(XmlNode p_node, Dictionary<string, string> p_choices)
		{
			if (p_node.ChildNodes.Count == 0)
			{
				return true;
			}
			return Element.CheckSelection(p_node, p_choices);
		}

		public XmlNode GetXmlNodeFromPreset(Dictionary<string, Variable> p_settings)
		{
			if (_DefaultValues == null)
			{
				return _Node;
			}
			_TempHash = -1;
			Dictionary<string, string> dictionary = _DefaultValues;
			if (p_settings != null)
			{
				dictionary = new Dictionary<string, string>(_DefaultValues);
				foreach (KeyValuePair<string, Variable> p_setting in p_settings)
				{
					dictionary[p_setting.Key] = p_setting.Value.ValueString;
				}
			}
			CalcExpression(dictionary);
			return GetXmlNode(dictionary);
		}

		private void CalcExpression(Dictionary<string, string> p_values)
		{
			if (_Expressions == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> expression in _Expressions)
			{
				string text = expression.Value;
				foreach (KeyValuePair<string, string> p_value in p_values)
				{
					if (text.Contains("~" + p_value.Key))
					{
						text = text.Replace("~" + p_value.Key, p_value.Value);
					}
				}
				Variable variable = Variable.CreateVariable(text, expression.Key);
				p_values[expression.Key] = variable.ValueString;
				variable = null;
			}
		}

		private XmlNode GetXmlNode(Dictionary<string, string> p_values)
		{
			_TempValues = p_values;
			XmlNode xmlNode = _Node.CloneNode(true);
			RecursiveFind(xmlNode);
			if (_TempHash != -1)
			{
				_CachedNodes.Add(_TempHash, xmlNode);
			}
			return xmlNode;
		}

		private static void RecursiveFind(XmlNode p_node)
		{
			XmlAttributeCollection attributes = p_node.Attributes;
			int count = attributes.Count;
			string text = null;
			string text2 = null;
			for (int i = 0; i < count; i++)
			{
				text = attributes[i].Value;
				if (!string.IsNullOrEmpty(text) && text[0] == '~')
				{
					text2 = text.Substring(1);
					if (_TempValues.ContainsKey(text2))
					{
						attributes[i].Value = _TempValues[text2];
					}
				}
			}
			XmlNodeList childNodes = p_node.ChildNodes;
			int count2 = childNodes.Count;
			for (int j = 0; j < count2; j++)
			{
				RecursiveFind(childNodes[j]);
			}
		}
	}
}
