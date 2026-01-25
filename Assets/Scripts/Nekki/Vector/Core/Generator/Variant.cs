using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;

namespace Nekki.Vector.Core.Generator
{
	public class Variant
	{
		public string _Name;

		private List<Choice> _Choice;

		public List<CounterCondition> _Conditions;

		private List<GeneratorLabel> _GeneratorLabels;

		public Dictionary<string, int> Labels = new Dictionary<string, int>();

		private Choice _Parent;

		private bool _IsIterable = true;

		public string ChoiceName
		{
			get
			{
				return _Parent.ChoiceName + "." + _Name;
			}
		}

		public bool IsContainsChoice
		{
			get
			{
				return _Choice.Count != 0;
			}
		}

		public List<Choice> ChildChoice
		{
			get
			{
				return _Choice;
			}
		}

		public List<GeneratorLabel> GeneratorLabels
		{
			get
			{
				return _GeneratorLabels;
			}
		}

		public bool isParentChoiceRoot
		{
			get
			{
				return _Parent.IsRoot;
			}
		}

		public Choice Parent
		{
			get
			{
				return _Parent;
			}
		}

		public bool IsIterable
		{
			get
			{
				return _IsIterable;
			}
			set
			{
				_IsIterable = value;
			}
		}

		public string Prefix
		{
			get
			{
				return _Parent.Prefix;
			}
			set
			{
				foreach (Choice item in _Choice)
				{
					item.Prefix = value;
				}
			}
		}

		public Variant(string name, List<Choice> chooses)
		{
			_Name = name;
			_Choice = chooses;
		}

		public Variant(Choice p_parent, XmlNode p_node)
		{
			_Parent = p_parent;
			_Name = p_node.Attributes["Name"].Value;
			_Conditions = CounterCondition.CreateListConditions(p_node["RequiredCounters"], "ST_Default");
			_GeneratorLabels = GeneratorLabel.ParseGeneratorLabel(p_node["GeneratorLabels"]);
			_Choice = Choice.Parse(p_node, null, this);
		}

		public void InitLabelDictinary()
		{
			AddLabelsToDictinary(Labels, null);
		}

		public bool CalcIsIterable()
		{
			_IsIterable = _GeneratorLabels != null && _GeneratorLabels.Count > 0;
			if (_Choice != null && _Choice.Count > 0)
			{
				bool flag = false;
				foreach (Choice item in _Choice)
				{
					flag = flag || item.CalcIsIterable();
				}
				_IsIterable = _IsIterable || flag;
			}
			return _IsIterable;
		}

		public void AddLabelsToDictinary(Dictionary<string, int> p_labels, Choice p_from)
		{
			if (p_from != null && _Choice.Count > 0 && _Choice[0] != p_from)
			{
				return;
			}
			if (_GeneratorLabels != null)
			{
				for (int i = 0; i < _GeneratorLabels.Count; i++)
				{
					if (p_labels.ContainsKey(_GeneratorLabels[i].Name))
					{
						Dictionary<string, int> dictionary;
						Dictionary<string, int> dictionary2 = (dictionary = p_labels);
						string name;
						string key = (name = _GeneratorLabels[i].Name);
						int num = dictionary[name];
						dictionary2[key] = num + _GeneratorLabels[i].Value;
					}
					else
					{
						p_labels.Add(_GeneratorLabels[i].Name, _GeneratorLabels[i].Value);
					}
				}
			}
			_Parent.AddLabelsToDictinary(p_labels);
		}

		public void Next(RoomConditionList p_conditions, ref bool isSwitch)
		{
			if (_Choice.Count == 0 || !_IsIterable)
			{
				isSwitch = false;
				return;
			}
			isSwitch = false;
			for (int i = 0; i < _Choice.Count; i++)
			{
				if (_Choice[i].IsIterable)
				{
					_Choice[i].Next(p_conditions, ref isSwitch);
					if (isSwitch)
					{
						break;
					}
					_Choice[i].ResetIterator(p_conditions);
				}
			}
			if (isSwitch)
			{
				DefineChoice(p_conditions, ref isSwitch);
			}
			else
			{
				isSwitch = false;
			}
		}

		public void DefineChoice(RoomConditionList p_conditions, ref bool p_result)
		{
			if (_Choice.Count == 0)
			{
				p_result = true;
				return;
			}
			bool p_isSwitch = false;
			int num = -1;
			for (int num2 = _Choice.Count - 1; num2 >= 0; num2--)
			{
				if (_Choice[num2].IsIndefined)
				{
					num = num2;
					break;
				}
			}
			if (num == -1)
			{
				p_result = true;
				return;
			}
			for (int num3 = num; num3 >= 0; num3--)
			{
				Choice choice = _Choice[num3];
				if (choice.IsIndefined)
				{
					choice.Define(p_conditions, ref p_isSwitch);
				}
				else
				{
					if (!choice.IsIterable)
					{
						continue;
					}
					choice.Next(p_conditions, ref p_isSwitch);
				}
				if (!p_isSwitch)
				{
					for (num3++; num3 < _Choice.Count && !_Choice[num3].IsIterable; num3++)
					{
					}
					num3++;
					if (num3 > _Choice.Count)
					{
						break;
					}
				}
			}
			p_result = p_isSwitch;
		}

		public bool Check()
		{
			if (_Conditions == null)
			{
				return true;
			}
			foreach (CounterCondition condition in _Conditions)
			{
				if (!condition.Check())
				{
					return false;
				}
			}
			return true;
		}

		public void AddToDictionary(Dictionary<string, string> p_choices)
		{
			if (!p_choices.ContainsKey(_Parent.ChoiceName))
			{
				p_choices.Add(_Parent.ChoiceName, _Name);
			}
			_Parent.AddVariant(p_choices);
		}

		public override string ToString()
		{
			return string.Format("Name: {0} Choice: {1}", ChoiceName, (_Choice != null) ? _Choice.Count.ToString() : "null");
		}

		public void ShuffleContent()
		{
			MainRandom.ShuffleList(_Choice);
			for (int i = 0; i < _Choice.Count; i++)
			{
				_Choice[i].ShuffleContent();
			}
		}

		public void ResetChoiseIterator()
		{
			if (IsContainsChoice)
			{
				for (int i = 0; i < _Choice.Count; i++)
				{
					_Choice[i].ResetIterator();
				}
			}
		}
	}
}
