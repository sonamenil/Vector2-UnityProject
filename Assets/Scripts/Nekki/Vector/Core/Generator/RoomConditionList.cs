using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;

namespace Nekki.Vector.Core.Generator
{
	public class RoomConditionList
	{
		protected List<CounterCondition> _Conditions;

		protected Dictionary<string, int> _Labels = new Dictionary<string, int>();

		protected Dictionary<string, CounterCondition> _ConditionsDic = new Dictionary<string, CounterCondition>();

		protected List<Variant> _ResultVariantList;

		public List<CounterCondition> Conditions
		{
			get
			{
				return _Conditions;
			}
		}

		public List<Variant> ResultVariantList
		{
			get
			{
				return _ResultVariantList;
			}
		}

		public RoomConditionList(List<CounterCondition> p_conditions)
		{
			SortConditions(p_conditions);
			_ResultVariantList = new List<Variant>();
			CreateDictionary();
		}

		private void SortConditions(List<CounterCondition> p_conditions)
		{
			_Conditions = new List<CounterCondition>();
			for (int i = 0; i < p_conditions.Count; i++)
			{
				if (p_conditions[i] is CounterConditionRange && (p_conditions[i] as CounterConditionRange).MinValue > 0)
				{
					_Conditions.Insert(0, p_conditions[i]);
				}
				else
				{
					_Conditions.Add(p_conditions[i]);
				}
			}
		}

		private void CreateDictionary()
		{
			for (int i = 0; i < _Conditions.Count; i++)
			{
				if (!_Labels.ContainsKey(_Conditions[i].Name))
				{
					_Labels.Add(_Conditions[i].Name, 0);
					_ConditionsDic.Add(_Conditions[i].Name, _Conditions[i]);
				}
			}
		}

		public void SetGeneratorLabels(List<GeneratorLabel> p_labels)
		{
			if (p_labels == null)
			{
				return;
			}
			for (int i = 0; i < p_labels.Count; i++)
			{
				if (_Labels.ContainsKey(p_labels[i].Name))
				{
					Dictionary<string, int> labels;
					Dictionary<string, int> dictionary = (labels = _Labels);
					string name;
					string key = (name = p_labels[i].Name);
					int num = labels[name];
					dictionary[key] = num + p_labels[i].Value;
				}
			}
		}

		public bool CheckGeneratorLabel(List<GeneratorLabel> p_labels)
		{
			if (p_labels == null)
			{
				return true;
			}
			SetGeneratorLabels(p_labels);
			return CheckCondition();
		}

		public void Reset()
		{
			List<string> list = new List<string>(_Labels.Keys);
			foreach (string item in list)
			{
				_Labels[item] = 0;
			}
		}

		public virtual bool CheckCondition()
		{
			for (int i = 0; i < _Conditions.Count; i++)
			{
				CounterCondition counterCondition = _Conditions[i];
				if (!counterCondition.Check(_Labels[counterCondition.Name]))
				{
					if (Settings.WriteGeneratorLogs)
					{
						VectorLog.GeneratorLog("FAIL: " + counterCondition.ToString());
					}
					return false;
				}
			}
			if (Settings.WriteGeneratorLogs)
			{
				VectorLog.GeneratorLog("Ok");
			}
			return true;
		}

		public bool CheckRanges(GeneratorLabelsRange p_range)
		{
			for (int i = 0; i < _Conditions.Count; i++)
			{
				if (!p_range.IsContentRange(_Conditions[i].Name))
				{
					if (!_Conditions[i].CheckRange(Point.ZeroPoint))
					{
						return false;
					}
				}
				else if (!_Conditions[i].CheckRange(p_range.GetRange(_Conditions[i].Name)))
				{
					return false;
				}
			}
			return true;
		}

		public virtual bool AddVariant(Variant p_variant)
		{
			List<GeneratorLabel> generatorLabels = p_variant.GeneratorLabels;
			if (generatorLabels != null)
			{
				for (int i = 0; i < generatorLabels.Count; i++)
				{
					if (!_Labels.ContainsKey(generatorLabels[i].Name))
					{
						continue;
					}
					Dictionary<string, int> labels;
					Dictionary<string, int> dictionary = (labels = _Labels);
					string name;
					string key = (name = generatorLabels[i].Name);
					int num = labels[name];
					dictionary[key] = num + generatorLabels[i].Value;
					if (_ConditionsDic[generatorLabels[i].Name].CheckByMax(_Labels[generatorLabels[i].Name]))
					{
						continue;
					}
					for (int j = 0; j <= i; j++)
					{
						if (_Labels.ContainsKey(generatorLabels[j].Name))
						{
							Dictionary<string, int> labels2;
							Dictionary<string, int> dictionary2 = (labels2 = _Labels);
							string key2 = (name = generatorLabels[j].Name);
							num = labels2[name];
							dictionary2[key2] = num - generatorLabels[j].Value;
						}
					}
					return false;
				}
			}
			if (p_variant.Parent != null && p_variant.Parent.IsIterable)
			{
				_ResultVariantList.Add(p_variant);
			}
			return true;
		}

		public void RemoveVariant(Variant p_variant)
		{
			if (!_ResultVariantList.Remove(p_variant) || p_variant.GeneratorLabels == null)
			{
				return;
			}
			List<GeneratorLabel> generatorLabels = p_variant.GeneratorLabels;
			for (int i = 0; i < generatorLabels.Count; i++)
			{
				if (_Labels.ContainsKey(generatorLabels[i].Name))
				{
					Dictionary<string, int> labels;
					Dictionary<string, int> dictionary = (labels = _Labels);
					string name;
					string key = (name = generatorLabels[i].Name);
					int num = labels[name];
					dictionary[key] = num - generatorLabels[i].Value;
				}
			}
		}

		public override string ToString()
		{
			string text = "Conditions:\n";
			for (int i = 0; i < _Conditions.Count; i++)
			{
				string text2 = text;
				text = text2 + i + " " + _Conditions[i].ToString() + "\n";
			}
			return text;
		}
	}
}
