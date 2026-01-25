using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class StringActions
	{
		private struct StringAction
		{
			public bool IsMulti;

			public Variable Name;

			public Variable Value;
		}

		public const string NodeName = "StringActions";

		private List<StringAction> _OnEnter;

		private List<StringAction> _OnExit;

		private StringActions(Mapping p_node)
		{
			Sequence sequence = p_node.GetSequence("OnEnter");
			Sequence sequence2 = p_node.GetSequence("OnExit");
			if (sequence != null)
			{
				_OnEnter = new List<StringAction>();
				foreach (Mapping item3 in sequence)
				{
					StringAction item = new StringAction
					{
						Name = Variable.CreateVariable(YamlUtils.GetStringValue(item3.GetText("Name"), null), string.Empty),
						Value = Variable.CreateVariable(YamlUtils.GetStringValue(item3.GetText("Value"), null), string.Empty),
						IsMulti = YamlUtils.GetBoolValue(item3.GetText("Collect"))
					};
					if (item.Name != null && item.Value != null)
					{
						_OnEnter.Add(item);
					}
				}
			}
			if (sequence2 == null)
			{
				return;
			}
			_OnExit = new List<StringAction>();
			foreach (Mapping item4 in sequence2)
			{
				StringAction item2 = new StringAction
				{
					Name = Variable.CreateVariable(YamlUtils.GetStringValue(item4.GetText("Name"), null), string.Empty),
					Value = Variable.CreateVariable(YamlUtils.GetStringValue(item4.GetText("Value"), null), string.Empty),
					IsMulti = YamlUtils.GetBoolValue(item4.GetText("Collect"))
				};
				if (item2.Name != null && item2.Value != null)
				{
					_OnExit.Add(item2);
				}
			}
		}

		private StringActions(XmlNode p_node)
		{
			XmlNode xmlNode = p_node["OnEnter"];
			XmlNode xmlNode2 = p_node["OnExit"];
			if (xmlNode != null)
			{
				_OnEnter = new List<StringAction>();
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					StringAction item = new StringAction
					{
						Name = Variable.CreateVariable(XmlUtils.ParseString(childNode.Attributes["Name"]), string.Empty),
						Value = Variable.CreateVariable(XmlUtils.ParseString(childNode.Attributes["Value"]), string.Empty),
						IsMulti = XmlUtils.ParseBool(childNode.Attributes["Collect"])
					};
					if (item.Name != null && item.Value != null)
					{
						_OnEnter.Add(item);
					}
				}
			}
			if (xmlNode2 == null)
			{
				return;
			}
			_OnExit = new List<StringAction>();
			foreach (XmlNode childNode2 in xmlNode2.ChildNodes)
			{
				StringAction item2 = new StringAction
				{
					Name = Variable.CreateVariable(XmlUtils.ParseString(childNode2.Attributes["Name"]), string.Empty),
					Value = Variable.CreateVariable(XmlUtils.ParseString(childNode2.Attributes["Value"]), string.Empty),
					IsMulti = XmlUtils.ParseBool(childNode2.Attributes["Collect"])
				};
				if (item2.Name != null && item2.Value != null)
				{
					_OnExit.Add(item2);
				}
			}
		}

		public static StringActions Create(Mapping p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			return new StringActions(p_node);
		}

		public static StringActions Create(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			return new StringActions(p_node);
		}

		public void ActivateOnEnter()
		{
			if (_OnEnter != null)
			{
				for (int i = 0; i < _OnEnter.Count; i++)
				{
					StringBuffer.AddString(_OnEnter[i].Name.ValueString, _OnEnter[i].Value.ValueString, _OnEnter[i].IsMulti);
				}
			}
		}

		public void ActivateOnExit()
		{
			if (_OnExit != null)
			{
				for (int i = 0; i < _OnExit.Count; i++)
				{
					StringBuffer.AddString(_OnExit[i].Name.ValueString, _OnExit[i].Value.ValueString, _OnExit[i].IsMulti);
				}
			}
		}
	}
}
