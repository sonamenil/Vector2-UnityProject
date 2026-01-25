using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger.Conditions;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerRunnerCondition : TriggerCondition
	{
		protected static Dictionary<string, string> newNamesVars;

		protected TriggerRunnerLoop _Parent;

		public TriggerLoop Parent
		{
			get
			{
				return _Parent;
			}
		}

		protected TriggerRunnerCondition(TriggerRunnerLoop p_parent, XmlNode p_node)
		{
			_Parent = p_parent;
			_IsNot = XmlUtils.ParseBool(p_node.Attributes["Not"]);
		}

		public Variable GetOrCreateVar(string p_name)
		{
			if (p_name[0] == '_')
			{
				return GetVariable(p_name);
			}
			return MakeVariable(p_name);
		}

		private Variable MakeVariable(string p_name)
		{
			return Variable.CreateVariable(p_name, string.Empty, _Parent.ParentTrigger);
		}

		protected Variable GetVariable(string p_name)
		{
			string text = p_name.Substring(1, p_name.Length - 1);
			if (newNamesVars.ContainsKey(text))
			{
				text = newNamesVars[text];
			}
			return _Parent.ParentTrigger.GetVariable("_" + text);
		}

		public static TriggerRunnerCondition Create(TriggerRunnerLoop p_parent, XmlNode p_node, Dictionary<string, string> p_newNameVars)
		{
			newNamesVars = p_newNameVars;
			TriggerRunnerCondition triggerRunnerCondition = null;
			string name = p_node.Name;
			switch (name)
			{
			case "Equal":
				return new TRC_Equal(p_parent, p_node);
			case "Greater":
				return new TRC_Greater(p_parent, p_node);
			case "Less":
				return new TRC_Less(p_parent, p_node);
			case "GreaterEqual":
				return new TRC_GreaterEqual(p_parent, p_node);
			case "LessEqual":
				return new TRC_LessEqual(p_parent, p_node);
			case "Operator":
				return new TRC_Operator(p_parent, p_node);
			case "Select":
				return new TCR_Select(p_parent, p_node);
			default:
				if (triggerRunnerCondition == null)
				{
					DebugUtils.Dialog("Unknown condition " + name, true);
				}
				return triggerRunnerCondition;
			}
		}

		public static void Parse(XmlNode p_node, TriggerRunnerLoop p_loop, List<TriggerCondition> p_conditions, string p_prefix = null)
		{
			if (p_node == null)
			{
				return;
			}
			XmlAttribute xmlAttribute = p_node.Attributes["Template"];
			if (xmlAttribute != null)
			{
				XmlNode templateConditionsXML = TemplateModule.getTemplateConditionsXML(xmlAttribute.Value);
				Parse(templateConditionsXML, p_loop, p_conditions);
				return;
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name.Equals("#comment"))
				{
					continue;
				}
				if (childNode.Name.Equals("ConditionBlock"))
				{
					string value = childNode.Attributes["Template"].Value;
					XmlNode templateConditionsXML2 = TemplateModule.getTemplateConditionsXML(value);
					string p_prefix2 = ((childNode.Attributes["Prefix"] == null) ? null : childNode.Attributes["Prefix"].Value);
					Parse(templateConditionsXML2, p_loop, p_conditions, p_prefix2);
					continue;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (p_prefix != null)
				{
					XmlNode xmlNode2 = p_node.ParentNode["Using"];
					foreach (XmlNode childNode2 in xmlNode2.ChildNodes)
					{
						if (childNode2.Attributes["ComplexName"] != null)
						{
							string value2 = childNode2.Attributes["Name"].Value;
							string value3 = p_prefix + value2;
							dictionary[value2] = value3;
						}
					}
				}
				Dictionary<string, string> dictionary2 = newNamesVars;
				TriggerRunnerCondition triggerRunnerCondition = Create(p_loop, childNode, dictionary);
				if (triggerRunnerCondition != null)
				{
					p_conditions.Add(triggerRunnerCondition);
				}
				newNamesVars = dictionary2;
			}
		}

		protected abstract string TypeString();

		public override void Log(bool result)
		{
			VectorLog.Tab(1);
			VectorLog.RunLog("ConditionType: " + TypeString());
			VectorLog.Tab(1);
			VectorLog.RunLog("Not: " + (_IsNot ? 1 : 0));
		}

		public override string ToString()
		{
			return "?";
		}
	}
}
