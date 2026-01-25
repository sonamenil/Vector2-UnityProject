using System.Collections.Generic;
using System.IO;
using System.Text;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.GUI.Common;

namespace Nekki.Vector.Core.Generator.Test
{
	public class RoomConditionListTest : RoomConditionList
	{
		private class ResultGenerator
		{
			public List<CounterCondition> _FailCondition;

			public List<List<Variant>> _Combinations = new List<List<Variant>>();

			public List<List<KeyValuePair<string, int>>> _Labels = new List<List<KeyValuePair<string, int>>>();
		}

		private Dictionary<string, ResultGenerator> _Result = new Dictionary<string, ResultGenerator>();

		private List<CounterCondition> _FailCondition;

		public int Combinations;

		public int Ok;

		public int Fail;

		public bool WriteCombinations = true;

		private Dictionary<string, Dictionary<int, int>> _LabelsLog = new Dictionary<string, Dictionary<int, int>>();

		public RoomConditionListTest(List<CounterCondition> p_conditions)
			: base(p_conditions)
		{
			_FailCondition = new List<CounterCondition>();
		}

		public override bool AddVariant(Variant p_variant)
		{
			List<GeneratorLabel> generatorLabels = p_variant.GeneratorLabels;
			if (generatorLabels != null)
			{
				for (int i = 0; i < generatorLabels.Count; i++)
				{
					if (_Labels.ContainsKey(generatorLabels[i].Name))
					{
						Dictionary<string, int> labels;
						Dictionary<string, int> dictionary = (labels = _Labels);
						string name;
						string key = (name = generatorLabels[i].Name);
						int num = labels[name];
						dictionary[key] = num + generatorLabels[i].Value;
					}
				}
				_ResultVariantList.Add(p_variant);
			}
			return true;
		}

		public bool CheckRanges(GeneratorLabelsRange p_range, StringBuilder p_strings)
		{
			bool result = true;
			for (int i = 0; i < _Conditions.Count; i++)
			{
				if (!p_range.IsContentRange(_Conditions[i].Name))
				{
					if (!_Conditions[i].CheckRange(Point.ZeroPoint))
					{
						CounterConditionRange counterConditionRange = _Conditions[i] as CounterConditionRange;
						string text = string.Format("  <Range Name=\"{0}\" Value=\"[{1} {2}]\" Need=\"{3}\" />", _Conditions[i].Name, 0, 0, counterConditionRange.RangeString);
						p_strings.AppendLine(text);
						ConsoleUI.Log(text);
						result = false;
					}
				}
				else if (!_Conditions[i].CheckRange(p_range.GetRange(_Conditions[i].Name)))
				{
					CounterConditionRange counterConditionRange2 = _Conditions[i] as CounterConditionRange;
					string text2 = string.Format("  <Range Name=\"{0}\" Value=\"[{1}]\" Need=\"{2}\" />", _Conditions[i].Name, p_range.GetRange(_Conditions[i].Name).RangeString, counterConditionRange2.RangeString);
					p_strings.AppendLine(text2);
					ConsoleUI.Log(text2);
					result = false;
				}
			}
			return result;
		}

		public override bool CheckCondition()
		{
			_FailCondition.Clear();
			bool flag = true;
			for (int i = 0; i < _Conditions.Count; i++)
			{
				int p_value = _Labels[_Conditions[i].Name];
				CounterCondition counterCondition = _Conditions[i];
				if (!counterCondition.Check(p_value))
				{
					flag = false;
					_FailCondition.Add(counterCondition);
				}
			}
			AddLabelsToDic(_LabelsLog, _ResultVariantList);
			if (flag)
			{
				Ok++;
			}
			else
			{
				Fail++;
			}
			Combinations++;
			PutFailCombination();
			return false;
		}

		private void PutFailCombination()
		{
			if (_FailCondition.Count == 0)
			{
				return;
			}
			string text = string.Empty;
			List<KeyValuePair<string, int>> list = null;
			if (WriteCombinations)
			{
				list = new List<KeyValuePair<string, int>>();
			}
			for (int i = 0; i < _FailCondition.Count; i++)
			{
				text += _FailCondition[i].Name;
				if (WriteCombinations)
				{
					list.Add(new KeyValuePair<string, int>(_FailCondition[i].Name, _Labels[_FailCondition[i].Name]));
				}
			}
			ResultGenerator resultGenerator = null;
			if (!_Result.ContainsKey(text))
			{
				resultGenerator = new ResultGenerator();
				resultGenerator._FailCondition = new List<CounterCondition>(_FailCondition);
				_Result.Add(text, resultGenerator);
			}
			else
			{
				resultGenerator = _Result[text];
			}
			if (WriteCombinations)
			{
				resultGenerator._Labels.Add(list);
				resultGenerator._Combinations.Add(new List<Variant>(_ResultVariantList));
			}
			else
			{
				resultGenerator._Combinations.Add(null);
			}
		}

		public void WriteResult(StreamWriter p_fileStream)
		{
			StringBuilder stringBuilder = new StringBuilder();
			p_fileStream.WriteLine("  <Conditions>");
			for (int i = 0; i < _Conditions.Count; i++)
			{
				p_fileStream.WriteLine(_Conditions[i].GetXmlNodeText("    "));
			}
			p_fileStream.WriteLine("  </Conditions>");
			p_fileStream.Flush();
			foreach (ResultGenerator value in _Result.Values)
			{
				string arg = string.Format("{0} ({1}%)", value._Combinations.Count, (int)((float)value._Combinations.Count / (float)Combinations * 100f));
				p_fileStream.WriteLine(string.Format("  <FailGroup Combinations=\"{0}\" >", arg));
				for (int j = 0; j < value._FailCondition.Count; j++)
				{
					p_fileStream.WriteLine(value._FailCondition[j].GetXmlNodeText("    "));
				}
				if (WriteCombinations)
				{
					p_fileStream.WriteLine("    <Combinations>");
					for (int k = 0; k < value._Combinations.Count; k++)
					{
						stringBuilder.Length = 0;
						stringBuilder.Append("      <Combination Variants=\"");
						List<Variant> list = value._Combinations[k];
						for (int l = 0; l < list.Count; l++)
						{
							stringBuilder.Append(list[l].ChoiceName);
							if (l != list.Count - 1)
							{
								stringBuilder.Append("|");
							}
						}
						stringBuilder.Append("\" Labels=\"");
						List<KeyValuePair<string, int>> list2 = value._Labels[k];
						for (int m = 0; m < list2.Count; m++)
						{
							stringBuilder.Append(list2[m].Key);
							stringBuilder.Append("=");
							stringBuilder.Append(list2[m].Value);
							if (m != list2.Count - 1)
							{
								stringBuilder.Append("|");
							}
						}
						stringBuilder.Append("\" />");
						p_fileStream.WriteLine(stringBuilder.ToString());
						if (k % 100 == 0)
						{
							p_fileStream.Flush();
						}
					}
					p_fileStream.WriteLine("    </Combinations>");
				}
				p_fileStream.WriteLine("  </FailGroup>");
				p_fileStream.Flush();
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			p_fileStream.WriteLine("  <LabelsAll>");
			foreach (KeyValuePair<string, Dictionary<int, int>> item in _LabelsLog)
			{
				stringBuilder2.Length = 0;
				stringBuilder2.AppendFormat("    <Label Name=\"{0}\" ", item.Key);
				foreach (KeyValuePair<int, int> item2 in item.Value)
				{
					stringBuilder2.AppendFormat("{0}=\"{1}\" ", item2.Key, item2.Value);
				}
				stringBuilder2.Append(" />");
				p_fileStream.WriteLine(stringBuilder2.ToString());
			}
			p_fileStream.WriteLine("  </LabelsAll>");
			p_fileStream.Flush();
		}

		private static void AddLabelsToDic(Dictionary<string, Dictionary<int, int>> p_dic, List<Variant> p_variants)
		{
			for (int i = 0; i < p_variants.Count; i++)
			{
				Variant variant = p_variants[i];
				foreach (KeyValuePair<string, int> label in variant.Labels)
				{
					string key = label.Key;
					int value = label.Value;
					Dictionary<int, int> dictionary = null;
					if (p_dic.ContainsKey(key))
					{
						dictionary = p_dic[key];
					}
					else
					{
						dictionary = new Dictionary<int, int>();
						p_dic.Add(key, dictionary);
					}
					if (dictionary.ContainsKey(value))
					{
						Dictionary<int, int> dictionary2;
						Dictionary<int, int> dictionary3 = (dictionary2 = dictionary);
						int key2;
						int key3 = (key2 = value);
						key2 = dictionary2[key2];
						dictionary3[key3] = key2 + 1;
					}
					else
					{
						dictionary.Add(value, 1);
					}
				}
			}
		}
	}
}
