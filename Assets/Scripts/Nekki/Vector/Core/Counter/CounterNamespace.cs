using System;
using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Vector.Core.Game;

namespace Nekki.Vector.Core.Counter
{
	public class CounterNamespace
	{
		public bool IsTemporary;

		public string Name;

		private bool _ForceSave;

		public Dictionary<string, ObscuredInt> Counters { get; private set; }

		public bool IsForceSave
		{
			get
			{
				return _ForceSave;
			}
		}

		public event Action<string, string> OnCounterChanged;

		protected CounterNamespace(string p_name)
		{
			Name = p_name;
			_ForceSave = false;
			Counters = new Dictionary<string, ObscuredInt>();
			IsTemporary = IsTemporaryCounterNamespaces(p_name);
		}

		protected CounterNamespace(CounterNamespace p_copy)
		{
			Name = p_copy.Name;
			IsTemporary = p_copy.IsTemporary;
			_ForceSave = p_copy._ForceSave;
			Counters = new Dictionary<string, ObscuredInt>(p_copy.Counters);
		}

		protected CounterNamespace(string p_name, XmlNode p_node)
		{
			Name = p_name;
			_ForceSave = XmlUtils.ParseBool(p_node.Attributes["ForceSave"]);
			IsTemporary = IsTemporaryCounterNamespaces(Name);
			Counters = new Dictionary<string, ObscuredInt>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				Counters.Add(childNode.Attributes["Name"].Value, XmlUtils.ParseInt(childNode.Attributes["Value"]));
			}
		}

		public static CounterNamespace Create(string p_name)
		{
			if (p_name == "PassiveEffects")
			{
				return new CounterPassiveEffectNamespace(p_name);
			}
			return new CounterNamespace(p_name);
		}

		public static CounterNamespace Create(XmlNode p_node)
		{
			string value = p_node.Attributes["Name"].Value;
			if (value == "PassiveEffects")
			{
				return new CounterPassiveEffectNamespace(value, p_node);
			}
			return new CounterNamespace(value, p_node);
		}

		public virtual CounterNamespace Copy()
		{
			return new CounterNamespace(this);
		}

		private static bool IsTemporaryCounterNamespaces(string p_name)
		{
			bool flag = p_name.Contains("!Room_");
			bool flag2 = p_name.Contains("_!Object_");
			if (flag && !flag2)
			{
				return Settings.IsTemporaryCounterNamespaces("ST_CurrentRoom");
			}
			if (flag && flag2)
			{
				return Settings.IsTemporaryCounterNamespaces("ST_LocalSpace");
			}
			return Settings.IsTemporaryCounterNamespaces(p_name);
		}

		public bool IsCounterExists(string p_counterName)
		{
			return Counters.ContainsKey(p_counterName);
		}

		public virtual ObscuredInt GetCounter(string p_counterName)
		{
			if (IsCounterExists(p_counterName))
			{
				return Counters[p_counterName];
			}
			return 0;
		}

		public void SetCounter(string p_counterName, ObscuredInt p_value)
		{
			if (IsCounterExists(p_counterName))
			{
				Counters[p_counterName] = p_value;
			}
			else
			{
				Counters.Add(p_counterName, p_value);
			}
			if (this.OnCounterChanged != null)
			{
				this.OnCounterChanged(p_counterName, Name);
			}
		}

		public void IncCounter(string p_counterName, ObscuredInt p_value)
		{
			if (IsCounterExists(p_counterName))
			{
				Dictionary<string, ObscuredInt> counters;
				Dictionary<string, ObscuredInt> dictionary = (counters = Counters);
				string key;
				string key2 = (key = p_counterName);
				ObscuredInt obscuredInt = counters[key];
				dictionary[key2] = (int)obscuredInt + (int)p_value;
			}
			else
			{
				Counters.Add(p_counterName, p_value);
			}
			if (this.OnCounterChanged != null)
			{
				this.OnCounterChanged(p_counterName, Name);
			}
		}

		public void RemoveCounter(string p_counterName)
		{
			if (IsCounterExists(p_counterName))
			{
				Counters.Remove(p_counterName);
			}
		}

		public void Clear()
		{
			Counters.Clear();
		}

		public void SaveToXml(XmlNode p_node)
		{
			if ((Counters.Count == 0 || IsTemporary) && !_ForceSave)
			{
				return;
			}
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Namespace");
			xmlElement.SetAttribute("Name", Name);
			if (_ForceSave)
			{
				xmlElement.SetAttribute("ForceSave", "1");
			}
			bool flag = false;
			foreach (KeyValuePair<string, ObscuredInt> counter in Counters)
			{
				if ((int)counter.Value != 0)
				{
					XmlElement xmlElement2 = p_node.OwnerDocument.CreateElement("Counter");
					xmlElement.AppendChild(xmlElement2);
					xmlElement2.SetAttribute("Name", counter.Key);
					xmlElement2.SetAttribute("Value", counter.Value.ToString());
					flag = true;
				}
			}
			if (flag || _ForceSave)
			{
				p_node.AppendChild(xmlElement);
			}
		}
	}
}
