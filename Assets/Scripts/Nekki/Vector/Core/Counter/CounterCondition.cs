using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public abstract class CounterCondition
	{
		public const string XmlNodeName = "RequiredCounters";

		public const string YamlNodeName = "Conditions";

		private Variable _Name;

		private bool _UseCurrentRoomNamespace;

		private bool _UseCurrentObjectNamespace;

		private string _Namespace;

		public bool Not { get; private set; }

		public string Name
		{
			get
			{
				return (_Name == null) ? string.Empty : _Name.ValueString;
			}
		}

		protected string Namespace
		{
			get
			{
				if (_UseCurrentRoomNamespace)
				{
					return CounterController.CurrentRoomNamespace;
				}
				if (_UseCurrentObjectNamespace)
				{
					return CounterController.CurrentObjectNamespace;
				}
				return _Namespace;
			}
			set
			{
				_Namespace = value;
				_UseCurrentRoomNamespace = "ST_CurrentRoom" == _Namespace;
				_UseCurrentObjectNamespace = "ST_LocalSpace" == _Namespace;
			}
		}

		protected CounterCondition(XmlNode p_node, string p_namespace)
		{
			_Name = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty), string.Empty);
			Namespace = XmlUtils.ParseString(p_node.Attributes["Namespace"], p_namespace);
			Not = XmlUtils.ParseBool(p_node.Attributes["Not"]);
		}

		protected CounterCondition(Mapping p_node, string p_namespace)
		{
			_Name = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Name"), string.Empty), string.Empty);
			Namespace = YamlUtils.GetStringValue(p_node.GetText("Namespace"), p_namespace);
			Not = YamlUtils.GetBoolValue(p_node.GetText("Not"));
		}

		protected CounterCondition(XmlNode p_node)
		{
			Not = XmlUtils.ParseBool(p_node.Attributes["Not"]);
		}

		protected CounterCondition(Mapping p_node)
		{
			Not = YamlUtils.GetBoolValue(p_node.GetText("Not"));
		}

		protected CounterCondition(CounterCondition p_copy)
		{
			_Namespace = p_copy._Namespace;
			Not = p_copy.Not;
			if (p_copy._Name.IsFunctionVar && !((VariableFunction)p_copy._Name).IsPointer)
			{
				_Name = Variable.CreateVariable(p_copy._Name.ValueString, string.Empty);
			}
			else
			{
				_Name = p_copy._Name;
			}
		}

		public static List<CounterCondition> CreateListConditions(XmlNode p_node, string p_namespace)
		{
			if (p_node == null)
			{
				return null;
			}
			List<CounterCondition> list = new List<CounterCondition>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				CounterCondition counterCondition = Create(childNode, p_namespace);
				if (counterCondition != null)
				{
					list.Add(counterCondition);
				}
			}
			return (list.Count == 0) ? null : list;
		}

		public static List<CounterCondition> CreateListConditions(Sequence p_node, string p_namespace)
		{
			if (p_node == null)
			{
				return null;
			}
			List<CounterCondition> list = new List<CounterCondition>();
			foreach (Mapping item in p_node)
			{
				CounterCondition counterCondition = Create(item, p_namespace);
				if (counterCondition != null)
				{
					list.Add(counterCondition);
				}
			}
			return (list.Count == 0) ? null : list;
		}

		private static CounterCondition Create(XmlNode p_node, string p_namespace)
		{
			if (p_node == null || p_node.Name == "#comment")
			{
				return null;
			}
			switch (p_node.Name)
			{
			case "Range":
				return new CounterConditionRange(p_node, p_namespace);
			case "Equal":
				return new CounterConditionEqual(p_node, p_namespace);
			case "Operator":
				return new CounterConditionOperator(p_node, p_namespace);
			case "GroupExists":
				return new CounterConditionGroupExists(p_node);
			default:
				DebugUtils.Dialog("Unknown CounterCondition node: " + p_node.Name, true);
				return null;
			}
		}

		private static CounterCondition Create(Mapping p_node, string p_namespace)
		{
			if (p_node == null)
			{
				return null;
			}
			Scalar text = p_node.GetText("ConditionType");
			if (text != null)
			{
				switch (text.text)
				{
				case "And":
				case "Or":
					return new CounterConditionOperator(p_node, p_namespace);
				case "LabelRange":
				case "CounterRange":
					return new CounterConditionRange(p_node, p_namespace);
				case "LabelEqual":
				case "CounterEqual":
					return new CounterConditionEqual(p_node, p_namespace);
				case "GroupExists":
					return new CounterConditionGroupExists(p_node);
				case "ValuesEqual":
					return new CounterConditionValuesEqual(p_node);
				case "BalanceElementExists":
					return new CounterConditionBalanceElementExists(p_node);
				}
			}
			DebugUtils.Dialog("Unknown CounterCondition node: " + p_node.ToString(), true);
			return null;
		}

		public static bool CheckCounterConditionList(List<CounterCondition> p_list)
		{
			if (p_list == null)
			{
				return true;
			}
			foreach (CounterCondition item in p_list)
			{
				if (!item.Check())
				{
					return false;
				}
			}
			return true;
		}

		public abstract bool Check();

		public abstract CounterCondition Copy();

		public virtual bool CheckRange(Point p_range)
		{
			return false;
		}

		public virtual bool Check(int p_value)
		{
			return false;
		}

		public virtual bool CheckByMax(int p_value)
		{
			return false;
		}

		public virtual string GetXmlNodeText(string p_tabs)
		{
			return string.Empty;
		}
	}
}
