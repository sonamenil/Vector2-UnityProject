using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.PassiveEffects
{
	public class TriggerPassiveEffect : IVariableParent
	{
		private string _Name;

		private ControllerPassiveEffects _Parent;

		private Dictionary<string, Variable> _Vars = new Dictionary<string, Variable>();

		private Variable _PassiveOccur;

		private List<TriggerPassiveEffectLoop> _Loops = new List<TriggerPassiveEffectLoop>();

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public ControllerPassiveEffects Parent
		{
			get
			{
				return _Parent;
			}
		}

		private TriggerPassiveEffect(XmlNode p_node, Mapping p_vars, ControllerPassiveEffects p_parent)
		{
			_Name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			_Parent = p_parent;
			_Vars.Add("_$PassiveOccur", Variable.CreateVariable("1", "$PassiveOccur"));
			ParseVars(p_vars);
			ParseLoops(p_node["Content"]);
		}

		public static TriggerPassiveEffect Create(XmlNode p_node, Mapping p_vars, ControllerPassiveEffects p_parent)
		{
			return new TriggerPassiveEffect(p_node, p_vars, p_parent);
		}

		private void ParseVars(Mapping p_vars)
		{
			if (p_vars == null)
			{
				return;
			}
			foreach (Nekki.Yaml.Node p_var in p_vars)
			{
				string key = p_var.key;
				string p_value = p_var.value.ToString();
				Variable value = Variable.CreateVariable(p_value, key);
				_Vars.Add("_" + key, value);
			}
		}

		private void ParseLoops(XmlNode p_node)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				_Loops.Add(TriggerPassiveEffectLoop.Create(childNode, this));
			}
		}

		public void IncrementPassiveOccurVar(int p_value)
		{
			_PassiveOccur.SetValue(_PassiveOccur.ValueInt + p_value);
		}

		public Variable GetVariable(string p_name)
		{
			switch (p_name)
			{
			case "_$ActionID":
				return _Parent.ActivateID;
			case "_$AnimationName":
				return _Parent.AnomationName;
			default:
				if (!_Vars.ContainsKey(p_name))
				{
					DebugUtils.Dialog("No Var Name = " + p_name + " in trigger " + Name, true);
					return null;
				}
				return _Vars[p_name];
			}
		}

		public Variable GetOrCreateVar(string p_nameOrValue)
		{
			if (p_nameOrValue.Length > 1 && p_nameOrValue[0] == '_')
			{
				return GetVariable(p_nameOrValue);
			}
			return Variable.CreateVariable(p_nameOrValue, null, this);
		}

		public void ChangeVarByName(string p_name, Variable p_var)
		{
			_Vars[p_name] = p_var;
		}
	}
}
