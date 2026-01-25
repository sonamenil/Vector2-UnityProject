using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Generator.Test;

namespace Nekki.Vector.Core.Generator
{
	public class RoomData
	{
		private string _Name;

		private string _File;

		private bool _IsIncludeInPlayCommand;

		private List<CounterCondition> _Conditions;

		private List<Choice> _Choices;

		private List<GeneratorLabel> _GeneratorLabels;

		private GeneratorLabelsRange _GeneratorLabelsRange;

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public string File
		{
			get
			{
				return _File;
			}
		}

		public bool IsIncludeInPlayCommand
		{
			get
			{
				return _IsIncludeInPlayCommand;
			}
		}

		public GeneratorLabelsRange Ranges
		{
			get
			{
				return _GeneratorLabelsRange;
			}
		}

		public List<Choice> Choices
		{
			get
			{
				return _Choices;
			}
		}

		public List<GeneratorLabel> GeneratorLabels
		{
			get
			{
				return _GeneratorLabels;
			}
		}

		public RoomData(XmlNode p_node)
		{
			_Name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			_File = XmlUtils.ParseString(p_node.Attributes["File"]);
			_IsIncludeInPlayCommand = XmlUtils.ParseBool(p_node.Attributes["IncludeInPlayCommand"]);
			_Conditions = CounterCondition.CreateListConditions(p_node["RequiredCounters"], "ST_Default");
			_GeneratorLabels = GeneratorLabel.ParseGeneratorLabel(p_node["GeneratorLabels"]);
			ParseSelection(p_node["Selection"]);
			_GeneratorLabelsRange = new GeneratorLabelsRange();
			_GeneratorLabelsRange.Collect(_GeneratorLabels, true, false);
			_GeneratorLabelsRange.Collect(_Choices, true);
			WriteNotIterableLabels();
		}

		private void ParseSelection(XmlNode p_node)
		{
			if (p_node != null && p_node.ChildNodes.Count != 0)
			{
				_Choices = Choice.Parse(p_node, this, null);
			}
		}

		private void WriteNotIterableLabels()
		{
			if (_Choices == null)
			{
				return;
			}
			foreach (Choice choice in _Choices)
			{
				choice.CalcIsIterable();
			}
		}

		private void DebugCheckNotIterableLabels(List<Choice> chooses, List<string> iterableFlags)
		{
			if (chooses == null)
			{
				return;
			}
			foreach (Choice choose in chooses)
			{
				string item = choose.ChoiceName + " " + choose.IsIterable;
				iterableFlags.Add(item);
				DebugCheckNotIterableLabels(choose.Variants, iterableFlags);
			}
		}

		private void DebugCheckNotIterableLabels(List<Variant> variants, List<string> iterableFlags)
		{
			if (variants == null)
			{
				return;
			}
			foreach (Variant variant in variants)
			{
				string item = variant._Name + " " + variant.IsIterable;
				iterableFlags.Add(item);
				DebugCheckNotIterableLabels(variant.ChildChoice, iterableFlags);
			}
		}

		public void ResetChoiceToIndefined()
		{
			if (_Choices != null)
			{
				for (int i = 0; i < _Choices.Count; i++)
				{
					_Choices[i].ResetToIndefined();
				}
			}
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

		public Room CheckConditions(RoomConditionList p_conditions)
		{
			if (Settings.WriteGeneratorLogs)
			{
				VectorLog.GeneratorLog(string.Empty);
				VectorLog.GeneratorLog("Check room: " + _Name);
				VectorLog.Tab(1);
			}
			if (_Choices == null)
			{
				if (p_conditions.CheckGeneratorLabel(_GeneratorLabels))
				{
					p_conditions.Reset();
					VectorLog.Untab(1);
					return new Room(this, null);
				}
				p_conditions.Reset();
				VectorLog.Untab(1);
				return null;
			}
			p_conditions.SetGeneratorLabels(_GeneratorLabels);
			GetVariants(_Choices, p_conditions);
			p_conditions.Reset();
			VectorLog.Untab(1);
			if (p_conditions.ResultVariantList.Count == 0 || (GeneratorTester.IsActive && GeneratorTester.IsIterationExpired))
			{
				return null;
			}
			return new Room(this, p_conditions.ResultVariantList);
		}

		private static void GetVariants(List<Choice> p_chooses, RoomConditionList p_conditions)
		{
			Variant variant = new Variant("ROOT", p_chooses);
			variant.ShuffleContent();
			bool p_result = true;
			while (true)
			{
				if (GeneratorTester.IsActive && GeneratorTester.IsIterationExpired)
				{
					p_result = false;
					p_conditions.ResultVariantList.Clear();
					break;
				}
				if (variant.ChildChoice[0].IsIndefined)
				{
					variant.DefineChoice(p_conditions, ref p_result);
					if (!p_result)
					{
						p_conditions.ResultVariantList.Clear();
						break;
					}
				}
				if (p_conditions.CheckCondition())
				{
					p_result = true;
					break;
				}
				p_result = true;
				variant.Next(p_conditions, ref p_result);
				if (!p_result)
				{
					p_conditions.ResultVariantList.Clear();
					break;
				}
			}
			if (p_result)
			{
				AddVariantOnConditionToList(variant, p_conditions);
				for (int i = 0; i < p_conditions.ResultVariantList.Count; i++)
				{
					AddVariantOnConditionToList(p_conditions.ResultVariantList[i], p_conditions);
				}
			}
		}

		private static void AddVariantOnConditionToList(Variant parent, RoomConditionList p_conditions)
		{
			foreach (Choice item in parent.ChildChoice)
			{
				if (!item.IsIterable)
				{
					p_conditions.ResultVariantList.Add(item.VariantOnConditions);
					item.ResetIterator();
				}
			}
		}
	}
}
