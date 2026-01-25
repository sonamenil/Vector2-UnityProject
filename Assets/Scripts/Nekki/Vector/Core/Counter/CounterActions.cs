using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterActions
	{
		private abstract class CounterAction
		{
			private class AssignCounter : CounterAction
			{
				public AssignCounter(Variable p_name, Variable p_namespace, Variable p_value)
					: base(p_name, p_namespace, p_value)
				{
				}

				public override void Activate()
				{
					CounterController.Current.CreateCounterOrSetValue(_Name.ValueString, _Value.ValueInt, base.Namespace);
				}
			}

			private class ClearCounter : CounterAction
			{
				public ClearCounter(Variable p_name, Variable p_namespace)
					: base(p_name, p_namespace, null)
				{
				}

				public override void Activate()
				{
					if (_Name == null)
					{
						CounterController.Current.ClearCounterNamespace(base.Namespace);
					}
					else
					{
						CounterController.Current.RemoveUserCounter(_Name.ValueString, base.Namespace);
					}
				}
			}

			private class IncrementCounter : CounterAction
			{
				public IncrementCounter(Variable p_name, Variable p_namespace, Variable p_value)
					: base(p_name, p_namespace, p_value)
				{
				}

				public override void Activate()
				{
					CounterController.Current.IncrementUserCounter(_Name.ValueString, _Value.ValueInt, base.Namespace);
				}
			}

			protected Variable _Name;

			protected Variable _Value;

			private Variable _Namespace;

			private bool _UseCurrentRoomNamespace;

			private bool _UseCurrentObjectNamespace;

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
					return _Namespace.ValueString;
				}
			}

			protected CounterAction(Variable p_name, Variable p_namespace, Variable p_value)
			{
				_Name = p_name;
				_Namespace = p_namespace;
				_Value = p_value;
				_UseCurrentRoomNamespace = "ST_CurrentRoom" == _Namespace.ValueString;
				_UseCurrentObjectNamespace = "ST_LocalSpace" == _Namespace.ValueString;
			}

			public static CounterAction Create(XmlNode p_node, string p_namespace)
			{
				if (p_node == null)
				{
					return null;
				}
				Variable p_name = ((p_node.Attributes["Name"] == null) ? null : Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Name"]), string.Empty));
				Variable p_namespace2 = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Namespace"], p_namespace), string.Empty);
				Variable p_value = ((p_node.Attributes["Value"] == null) ? null : Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Value"]), string.Empty));
				CounterAction result = null;
				switch (p_node.Name)
				{
				case "Assign":
					result = new AssignCounter(p_name, p_namespace2, p_value);
					break;
				case "Clear":
					result = new ClearCounter(p_name, p_namespace2);
					break;
				case "IncrementBy":
					result = new IncrementCounter(p_name, p_namespace2, p_value);
					break;
				}
				return result;
			}

			public static CounterAction Create(Mapping p_node, string p_namespace)
			{
				if (p_node == null)
				{
					return null;
				}
				Variable p_name = ((p_node.GetText("Name") == null) ? null : Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Name"), string.Empty), string.Empty));
				Variable p_namespace2 = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Namespace"), p_namespace), string.Empty);
				Variable p_value = ((p_node.GetText("Value") == null) ? null : Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Value"), string.Empty), string.Empty));
				CounterAction result = null;
				switch (p_node.GetText("Type").text)
				{
				case "Assign":
					result = new AssignCounter(p_name, p_namespace2, p_value);
					break;
				case "Clear":
					result = new ClearCounter(p_name, p_namespace2);
					break;
				case "IncrementBy":
					result = new IncrementCounter(p_name, p_namespace2, p_value);
					break;
				}
				return result;
			}

			public abstract void Activate();
		}

		public const string NodeName = "CounterActions";

		private List<CounterAction> _OnEnter;

		private List<CounterAction> _OnExit;

		private CounterActions()
		{
		}

		public static CounterActions Create(XmlNode p_node, string p_namespace)
		{
			if (p_node == null)
			{
				return null;
			}
			CounterActions counterActions = new CounterActions();
			XmlNode xmlNode = p_node["OnEnter"];
			XmlNode xmlNode2 = p_node["OnExit"];
			if (xmlNode != null)
			{
				counterActions._OnEnter = new List<CounterAction>();
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					CounterAction counterAction = CounterAction.Create(childNode, p_namespace);
					if (counterAction != null)
					{
						counterActions._OnEnter.Add(counterAction);
					}
				}
			}
			if (xmlNode2 != null)
			{
				counterActions._OnExit = new List<CounterAction>();
				foreach (XmlNode childNode2 in xmlNode2.ChildNodes)
				{
					CounterAction counterAction2 = CounterAction.Create(childNode2, p_namespace);
					if (counterAction2 != null)
					{
						counterActions._OnExit.Add(counterAction2);
					}
				}
			}
			return counterActions;
		}

		public static CounterActions Create(Mapping p_node, string p_namespace)
		{
			if (p_node == null)
			{
				return null;
			}
			CounterActions counterActions = new CounterActions();
			Sequence sequence = p_node.GetSequence("OnEnter");
			Sequence sequence2 = p_node.GetSequence("OnExit");
			if (sequence != null)
			{
				counterActions._OnEnter = new List<CounterAction>();
				foreach (Mapping item in sequence)
				{
					CounterAction counterAction = CounterAction.Create(item, p_namespace);
					if (counterAction != null)
					{
						counterActions._OnEnter.Add(counterAction);
					}
				}
			}
			if (sequence2 != null)
			{
				counterActions._OnExit = new List<CounterAction>();
				foreach (Mapping item2 in sequence2)
				{
					CounterAction counterAction2 = CounterAction.Create(item2, p_namespace);
					if (counterAction2 != null)
					{
						counterActions._OnExit.Add(counterAction2);
					}
				}
			}
			return counterActions;
		}

		public void ActivateOnEnter()
		{
			ActivateList(_OnEnter);
		}

		public void ActivateOnExit()
		{
			ActivateList(_OnExit);
		}

		private void ActivateList(List<CounterAction> p_list)
		{
			if (p_list == null)
			{
				return;
			}
			foreach (CounterAction item in p_list)
			{
				item.Activate();
			}
		}
	}
}
