using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Generator
{
	public class Choice
	{
		public RoomData _ParentRoom;

		private Variant _ParentVariant;

		private string _Prefix;

		private string _Name;

		private List<Variant> _Variants;

		private List<Variant> _FinalVariant;

		private GeneratorLabelsRange _Ranges;

		public Choice Left;

		public Choice Right;

		private int _IteratorVariants;

		private bool _IsIndefined = true;

		private bool _IsIterable = true;

		private Variant _VariantOnConditions;

		public List<Variant> Variants
		{
			get
			{
				return _Variants;
			}
		}

		public string ChoiceName
		{
			get
			{
				if (_ParentRoom != null)
				{
					return _Prefix + "_" + _Name;
				}
				return _ParentVariant.ChoiceName + "/" + _Name;
			}
		}

		public GeneratorLabelsRange Ranges
		{
			get
			{
				return _Ranges;
			}
		}

		public bool IsRoot
		{
			get
			{
				return _ParentRoom != null;
			}
		}

		public Choice UpperChoice
		{
			get
			{
				if (_ParentVariant == null)
				{
					return null;
				}
				return _ParentVariant.Parent;
			}
		}

		public string Prefix
		{
			get
			{
				return _Prefix;
			}
			set
			{
				_Prefix = value;
				foreach (Variant variant in _Variants)
				{
					variant.Prefix = _Prefix;
				}
			}
		}

		public bool IsIndefined
		{
			get
			{
				return _IsIndefined;
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

		public Variant ParentVariant
		{
			get
			{
				return _ParentVariant;
			}
			set
			{
				_ParentVariant = value;
			}
		}

		public Variant VariantOnConditions
		{
			get
			{
				return _VariantOnConditions;
			}
		}

		private Choice(XmlNode p_node, RoomData p_parentRoom, Variant p_parentVariant)
		{
			_ParentRoom = p_parentRoom;
			_ParentVariant = p_parentVariant;
			_Prefix = string.Empty;
			_Name = p_node.Attributes["Name"].Value;
			_Variants = new List<Variant>();
			_Ranges = new GeneratorLabelsRange();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				_Variants.Add(new Variant(this, childNode));
			}
			if (p_parentRoom != null)
			{
				GetFinalVariants();
				CalcGeneratorLabels();
			}
		}

		public Choice(string name, List<Variant> variants)
		{
			_Name = name;
			_Variants = variants;
		}

		public static List<Choice> Parse(XmlNode p_node, RoomData p_parentRoom, Variant p_parentVariant)
		{
			List<Choice> list = new List<Choice>();
			Choice left = null;
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name == "Choice")
				{
					Choice choice = new Choice(childNode, p_parentRoom, p_parentVariant);
					list.Add(choice);
					choice.Left = left;
					left = choice;
				}
			}
			for (int i = 0; i < list.Count - 1; i++)
			{
				list[i].Right = list[i + 1];
			}
			return list;
		}

		public bool CalcIsIterable()
		{
			_IsIterable = false;
			if (_Variants != null)
			{
				foreach (Variant variant in _Variants)
				{
					_IsIterable = _IsIterable || variant.CalcIsIterable();
				}
			}
			return _IsIterable;
		}

		public void CollectRanges()
		{
			_Ranges.Collect(_Variants);
		}

		public void FinalVariantLog()
		{
			string text = "Choice:" + _Name + " FinalVariant: \n";
			foreach (Variant item in _FinalVariant)
			{
				text = text + "   V:" + item._Name + " Dic:\n";
				foreach (KeyValuePair<string, int> label in item.Labels)
				{
					text = text + "     " + label.ToString() + "\n";
				}
			}
			Debug.Log(text);
		}

		private void GetFinalVariants()
		{
			_FinalVariant = new List<Variant>();
			GetFinalVariant(this);
		}

		private void GetFinalVariant(Choice p_room)
		{
			for (int i = 0; i < _Variants.Count; i++)
			{
				if (_Variants[i].ChildChoice.Count == 0)
				{
					p_room.AddFinalVariant(_Variants[i]);
					continue;
				}
				for (int j = 0; j < _Variants[i].ChildChoice.Count; j++)
				{
					_Variants[i].ChildChoice[j].GetFinalVariant(p_room);
				}
			}
		}

		private void CalcGeneratorLabels()
		{
			for (int i = 0; i < _FinalVariant.Count; i++)
			{
				_FinalVariant[i].InitLabelDictinary();
			}
		}

		public void AddFinalVariant(Variant p_variant)
		{
			_FinalVariant.Add(p_variant);
		}

		public bool CheckAvalibleVariants()
		{
			foreach (Variant variant in _Variants)
			{
				if (variant.Check())
				{
					return true;
				}
			}
			return false;
		}

		public void AddVariant(Dictionary<string, string> p_choices)
		{
			if (_ParentVariant != null)
			{
				_ParentVariant.AddToDictionary(p_choices);
			}
		}

		public void AddLabelsToDictinary(Dictionary<string, int> p_labels)
		{
			if (_ParentVariant != null)
			{
				_ParentVariant.AddLabelsToDictinary(p_labels, this);
			}
		}

		public void Next(RoomConditionList p_conditions, ref bool p_isSwitch)
		{
			_Variants[_IteratorVariants].Next(p_conditions, ref p_isSwitch);
			if (p_isSwitch)
			{
				return;
			}
			while (_IteratorVariants < _Variants.Count - 1)
			{
				if (IncrementIterator(p_conditions))
				{
					_VariantOnConditions = _Variants[_IteratorVariants];
					_Variants[_IteratorVariants].DefineChoice(p_conditions, ref p_isSwitch);
					if (p_isSwitch)
					{
						return;
					}
				}
			}
			p_isSwitch = false;
			ResetIterator(p_conditions);
		}

		public void Define(RoomConditionList p_conditions, ref bool p_result)
		{
			if (_IsIndefined && p_conditions.AddVariant(_Variants[_IteratorVariants]))
			{
				_VariantOnConditions = _Variants[_IteratorVariants];
				_Variants[_IteratorVariants].DefineChoice(p_conditions, ref p_result);
				if (p_result)
				{
					_IsIndefined = false;
					return;
				}
			}
			while (_IteratorVariants < _Variants.Count - 1)
			{
				p_result = IncrementIterator(p_conditions);
				if (p_result)
				{
					_VariantOnConditions = _Variants[_IteratorVariants];
					_Variants[_IteratorVariants].DefineChoice(p_conditions, ref p_result);
					if (p_result)
					{
						break;
					}
				}
			}
			if (!p_result)
			{
				ResetIterator(p_conditions);
			}
			_IsIndefined = !p_result;
		}

		private void RemoveVariantFromConditions(RoomConditionList p_result)
		{
			if (_VariantOnConditions != null)
			{
				p_result.RemoveVariant(_VariantOnConditions);
				_VariantOnConditions = null;
			}
		}

		public void ResetIterator(RoomConditionList p_result)
		{
			_IsIndefined = true;
			RemoveVariantFromConditions(p_result);
			_IteratorVariants = 0;
		}

		public void ResetIterator()
		{
			_IsIndefined = true;
			_VariantOnConditions = null;
			_IteratorVariants = 0;
		}

		public void ResetToIndefined()
		{
			_IsIndefined = true;
			_VariantOnConditions = null;
			_IteratorVariants = 0;
			for (int i = 0; i < _Variants.Count; i++)
			{
				_Variants[i].ResetChoiseIterator();
			}
		}

		public bool IncrementIterator(RoomConditionList p_result)
		{
			RemoveVariantFromConditions(p_result);
			_IteratorVariants++;
			_IsIndefined = false;
			return p_result.AddVariant(_Variants[_IteratorVariants]);
		}

		public Variant GetCurrentVariant()
		{
			return _Variants[_IteratorVariants];
		}

		public void ShuffleContent()
		{
			MainRandom.ShuffleList(_Variants);
			for (int i = 0; i < _Variants.Count; i++)
			{
				_Variants[i].ShuffleContent();
			}
		}

		public override string ToString()
		{
			return "Choice Name: " + _Name + " Undefined: " + ((!_IsIndefined) ? "false" : "true");
		}
	}
}
