using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public abstract class Item : IVariableParent
	{
		public enum ItemExpirationType
		{
			IET_NONE = 0,
			IET_RUN = 1,
			IET_FLOOR = 2,
			IET_FRAMES = 3,
			IET_TERMINAL = 4
		}

		public enum ItemType
		{
			Unknown = 0,
			Gadget = 1,
			StarterPack = 2,
			CardsContainer = 3,
			StarterPackBuff = 4,
			GadgetEffect = 5,
			GadgetCharger = 6,
			Supply = 7
		}

		public delegate bool ItemGroupAttributeCondition(ItemGroupAttributes p_attr);

		protected string _Name;

		protected Variable _VisualName;

		protected List<ItemGroupAttributes> _Attributes = new List<ItemGroupAttributes>();

		protected Dictionary<string, ItemGroupAttributes> _AttributesDic = new Dictionary<string, ItemGroupAttributes>();

		protected ItemExpirationType _ExpirationType;

		protected int _ExpirationCounter;

		protected bool _ForceSave;

		protected ItemType _Type;

		private Dictionary<string, Variable> _ObjectParams;

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				if (Variable.GetTypeByNameAndValue("Name", value) == VariableType.Function)
				{
					VariableFunction variableFunction = Variable.CreateVariable(value, "Name", this) as VariableFunction;
					_Name = variableFunction.ValueString;
					DataLocal.Current.GetUserPropertyByName("ItemNameIterator").AppendValue("Value", 1);
				}
				else
				{
					_Name = value;
				}
			}
		}

		public ItemType Type
		{
			get
			{
				return _Type;
			}
			set
			{
				_Type = value;
			}
		}

		public string VisualName
		{
			get
			{
				return _VisualName.ValueString;
			}
			set
			{
				switch (Variable.GetTypeByNameAndValue("Name", value))
				{
				case VariableType.Int:
				case VariableType.Float:
				case VariableType.String:
					if (_VisualName.IsFunctionVar)
					{
						_VisualName = Variable.CreateVariable(value, "Name", this);
					}
					else
					{
						_VisualName.SetValue(value);
					}
					break;
				case VariableType.Function:
					SetVariableValue(ref _VisualName, value, "Name");
					break;
				case VariableType.LocalizationString:
					_VisualName = Variable.CreateVariable(value, "Name", this) as VariableLocalizationString;
					break;
				case VariableType.Expression:
				case VariableType.Item:
					break;
				}
			}
		}

		public List<ItemGroupAttributes> Groups
		{
			get
			{
				return _Attributes;
			}
		}

		public ItemExpirationType ExpirationType
		{
			get
			{
				return _ExpirationType;
			}
			set
			{
				_ExpirationType = value;
			}
		}

		public int ExpirationCounter
		{
			get
			{
				return _ExpirationCounter;
			}
			set
			{
				_ExpirationCounter = value;
			}
		}

		public bool IsForceSave
		{
			get
			{
				return _ForceSave;
			}
		}

		public Dictionary<string, Variable> ObjectParams
		{
			get
			{
				return _ObjectParams;
			}
			set
			{
				_ObjectParams = value;
			}
		}

		public Item(string Name)
		{
			_Name = string.Empty;
			_VisualName = Variable.CreateVariable(string.Empty, "VisualName", this);
			_Type = ItemType.Unknown;
			_ExpirationType = ItemExpirationType.IET_NONE;
			_ExpirationCounter = -1;
			_ForceSave = false;
		}

		public Item(XmlNode p_node)
		{
			_Name = XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty);
			_VisualName = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["VisualName"], string.Empty), "VisualName", this);
			_Type = ItemTypeFromString(XmlUtils.ParseString(p_node.Attributes["ItemType"], string.Empty));
			_ForceSave = XmlUtils.ParseBool(p_node.Attributes["ForceSave"]);
			_ExpirationType = ItemExpirationType.IET_NONE;
			_ExpirationCounter = -1;
		}

		public Item(Mapping p_node)
		{
			_Name = p_node.key;
			_VisualName = Variable.CreateVariable(p_node.key, "VisualName", this);
			_ExpirationType = ItemExpirationType.IET_NONE;
			_ForceSave = false;
			_ExpirationCounter = -1;
		}

		protected Item(Item p_copy)
		{
			_Name = p_copy._Name;
			_VisualName = Variable.CreateVariable(p_copy._VisualName.ValueForSave, "VisualName", this);
			_Type = p_copy._Type;
			_ExpirationType = p_copy._ExpirationType;
			_ExpirationCounter = p_copy._ExpirationCounter;
			_ForceSave = p_copy._ForceSave;
			foreach (ItemGroupAttributes attribute in p_copy._Attributes)
			{
				ItemGroupAttributes p_value = attribute.CreateCopy();
				AddGroupAttributes(p_value);
			}
		}

		public static ItemExpirationType ParsExpirationTypeByString(string p_value)
		{
			switch (p_value)
			{
			case "Frames":
				return ItemExpirationType.IET_FRAMES;
			case "Run":
				return ItemExpirationType.IET_RUN;
			case "Floor":
				return ItemExpirationType.IET_FLOOR;
			case "Terminal":
				return ItemExpirationType.IET_TERMINAL;
			default:
				return ItemExpirationType.IET_NONE;
			}
		}

		public static string ConvertItemExpirationTypeToString(ItemExpirationType p_value)
		{
			switch (p_value)
			{
			case ItemExpirationType.IET_NONE:
				return "None";
			case ItemExpirationType.IET_FRAMES:
				return "Frames";
			case ItemExpirationType.IET_RUN:
				return "Run";
			case ItemExpirationType.IET_FLOOR:
				return "Floor";
			case ItemExpirationType.IET_TERMINAL:
				return "Terminal";
			default:
				return "None";
			}
		}

		public bool IsType(ItemType p_type)
		{
			return _Type == p_type;
		}

		public static ItemType ItemTypeFromString(string p_value)
		{
			switch (p_value)
			{
			case "Gadget":
				return ItemType.Gadget;
			case "StarterPack":
				return ItemType.StarterPack;
			case "CardsContainer":
				return ItemType.CardsContainer;
			case "StarterPackBuff":
				return ItemType.StarterPackBuff;
			case "GadgetEffect":
				return ItemType.GadgetEffect;
			case "GadgetCharger":
				return ItemType.GadgetCharger;
			case "Supply":
				return ItemType.Supply;
			default:
				return ItemType.Unknown;
			}
		}

		protected void ParseGroups(XmlNode p_node)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name == "Group")
				{
					ParseGroup(childNode);
				}
			}
		}

		private void ParseGroup(XmlNode p_node)
		{
			ItemGroupAttributes p_value = new ItemGroupAttributes(p_node);
			AddGroupAttributes(p_value);
		}

		public void AddGroupAttributes(ItemGroupAttributes p_value, int p_insertIndex = -1)
		{
			p_value.ParentItem = this;
			if (p_insertIndex == -1)
			{
				_Attributes.Add(p_value);
			}
			else
			{
				_Attributes.Insert(p_insertIndex, p_value);
			}
			_AttributesDic.Add(p_value.GroupName, p_value);
		}

		public void RemoveGroupAttributes(string p_name)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_name);
			if (attributeByGroupName != null)
			{
				_AttributesDic.Remove(p_name);
				_Attributes.Remove(attributeByGroupName);
				attributeByGroupName.ParentItem = null;
			}
		}

		public void RemoveGroupAttributes(ItemGroupAttributes p_attribute)
		{
			if (_Attributes.Contains(p_attribute))
			{
				_AttributesDic.Remove(p_attribute.GroupName);
				_Attributes.Remove(p_attribute);
				p_attribute.ParentItem = null;
			}
		}

		public void ReplaceGroupAttributes(ItemGroupAttributes p_from, ItemGroupAttributes p_to)
		{
			int num = _Attributes.IndexOf(p_from);
			if (num == -1)
			{
				DebugUtils.Dialog("ReplaceGroupAttributes not avalible Group:" + p_from.GroupName + " not in item Name:" + _Name, true);
			}
			RemoveGroupAttributes(p_from);
			AddGroupAttributes(p_to, num);
		}

		public void ReplaceGroupAttribute(Mapping p_value)
		{
			string text = p_value.key;
			VariableType typeByNameAndValue = Variable.GetTypeByNameAndValue(string.Empty, text);
			if (typeByNameAndValue == VariableType.Function)
			{
				Variable variable = Variable.CreateVariable(text, string.Empty);
				text = variable.ValueString;
			}
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(text);
			if (attributeByGroupName != null)
			{
				attributeByGroupName.ReplaceGroup(p_value);
				_AttributesDic.Remove(text);
				_AttributesDic.Add(attributeByGroupName.GroupName, attributeByGroupName);
			}
		}

		public void CreateGroupAttibute(Mapping p_value)
		{
			if (!InitFieldsItem(p_value))
			{
				string text = p_value.key;
				VariableType typeByNameAndValue = Variable.GetTypeByNameAndValue(string.Empty, text);
				if (typeByNameAndValue == VariableType.Function)
				{
					Variable variable = Variable.CreateVariable(text, string.Empty);
					text = variable.ValueString;
				}
				if (_AttributesDic.ContainsKey(text))
				{
					_AttributesDic[text].AppendByYaml(p_value);
					return;
				}
				ItemGroupAttributes itemGroupAttributes = new ItemGroupAttributes(text);
				itemGroupAttributes.ParentItem = this;
				_Attributes.Add(itemGroupAttributes);
				_AttributesDic.Add(itemGroupAttributes.GroupName, itemGroupAttributes);
				itemGroupAttributes.ParseFromYaml(p_value, this);
			}
		}

		private bool InitFieldsItem(Mapping p_value)
		{
			switch (p_value.key)
			{
			case "ST_ItemMainData":
			{
				Name = p_value.GetNode("ST_ItemName").value.ToString();
				VisualName = p_value.GetNode("ST_VisualItemName").value.ToString();
				Nekki.Yaml.Node node = p_value.GetNode("ST_ItemType");
				if (node != null)
				{
					_Type = ItemTypeFromString(node.value.ToString());
				}
				return true;
			}
			case "ST_Expiration":
				ExpirationType = ParsExpirationTypeByString(p_value.GetNode("ST_ExpirationType").value.ToString());
				ExpirationCounter = int.Parse(p_value.GetNode("ST_ExpirationTime").value.ToString());
				return true;
			case "ST_ForceSave":
				_ForceSave = YamlUtils.GetBoolValue(p_value.GetText("Value"));
				return true;
			default:
				return false;
			}
		}

		public ItemGroupAttributes GetAttributeByGroupName(string p_groupName)
		{
			if (_AttributesDic.ContainsKey(p_groupName))
			{
				return _AttributesDic[p_groupName];
			}
			return null;
		}

		public List<ItemGroupAttributes> GetAttributesByCondition(ItemGroupAttributeCondition p_condition)
		{
			List<ItemGroupAttributes> list = new List<ItemGroupAttributes>();
			foreach (ItemGroupAttributes attribute in _Attributes)
			{
				if (p_condition(attribute))
				{
					list.Add(attribute);
				}
			}
			return list;
		}

		public bool HasGroup(string p_groupName)
		{
			return _AttributesDic.ContainsKey(p_groupName);
		}

		public string GetStrValueAttribute(string p_name)
		{
			string p_value = null;
			foreach (ItemGroupAttributes attribute in _Attributes)
			{
				if (attribute.TryGetStrValue(p_name, ref p_value))
				{
					return p_value;
				}
			}
			return p_value;
		}

		public string GetStrValueAttribute(string p_name, string p_groupName, string p_def = null)
		{
			string p_value = p_def;
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName == null)
			{
				DebugUtils.Dialog("ItemGroup Name:" + p_groupName + " not found! In Item Name:" + _Name, false);
				return p_value;
			}
			attributeByGroupName.TryGetStrValue(p_name, ref p_value);
			return p_value;
		}

		public int GetIntValueAttribute(string p_name)
		{
			int p_value = -1;
			foreach (ItemGroupAttributes attribute in _Attributes)
			{
				if (attribute.TryGetIntValue(p_name, ref p_value))
				{
					return p_value;
				}
			}
			return p_value;
		}

		public float GetFloatValueAttribute(string p_name)
		{
			float p_value = -1f;
			foreach (ItemGroupAttributes attribute in _Attributes)
			{
				if (attribute.TryGetFloatValue(p_name, ref p_value))
				{
					return p_value;
				}
			}
			return p_value;
		}

		public float GetFloatValueAttribute(string p_name, string p_groupName, float p_default = -1f)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName == null)
			{
			}
			float p_value = p_default;
			attributeByGroupName.TryGetFloatValue(p_name, ref p_value);
			return p_value;
		}

		public int GetIntValueAttribute(string p_name, string p_groupName, int p_default = -1)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName == null)
			{
			}
			int p_value = p_default;
			attributeByGroupName.TryGetIntValue(p_name, ref p_value);
			return p_value;
		}

		public int GetIntValueForTrigger(string p_name, string p_groupName)
		{
			if (p_name == "Name")
			{
				return _Name.GetHashCode();
			}
			if (p_name == "ST_VisualItemName")
			{
				return VisualName.GetHashCode();
			}
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName == null)
			{
				return -1;
			}
			int p_value = -1;
			if (attributeByGroupName.TryGetIntValue(p_name, ref p_value))
			{
				return p_value;
			}
			float p_value2 = -1f;
			if (attributeByGroupName.TryGetFloatValue(p_name, ref p_value2))
			{
				return (int)p_value2;
			}
			string p_value3 = string.Empty;
			if (attributeByGroupName.TryGetStrValue(p_name, ref p_value3))
			{
				return p_value3.GetHashCode();
			}
			return -1;
		}

		public string GetStringValueForTrigger(string p_name, string p_groupName)
		{
			if (p_name == "Name")
			{
				return _Name;
			}
			if (p_name == "ST_VisualItemName")
			{
				return VisualName;
			}
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName == null)
			{
				return string.Empty;
			}
			string p_value = string.Empty;
			if (attributeByGroupName.TryGetStrValue(p_name, ref p_value))
			{
				return p_value;
			}
			return string.Empty;
		}

		public bool ContainsGroup(string p_name)
		{
			return _AttributesDic.ContainsKey(p_name);
		}

		public bool IsContainsAttributeWithValue(string p_name, int p_value)
		{
			int p_value2 = -1;
			foreach (ItemGroupAttributes attribute in _Attributes)
			{
				if (attribute.TryGetIntValue(p_name, ref p_value2) && p_value2 == p_value)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsContainsAttributeWithValue(string p_name, string p_value)
		{
			string p_value2 = null;
			foreach (ItemGroupAttributes attribute in _Attributes)
			{
				if (attribute.TryGetStrValue(p_name, ref p_value2) && p_value2 == p_value)
				{
					return true;
				}
			}
			return false;
		}

		public virtual void SetValue(int p_value, string p_name, string p_groupName)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null && attributeByGroupName.TrySetValue(p_name, p_value))
			{
			}
		}

		public virtual void SetValue(float p_value, string p_name, string p_groupName)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null && attributeByGroupName.TrySetValue(p_name, p_value))
			{
			}
		}

		public virtual void SetValue(string p_value, string p_name, string p_groupName)
		{
			if (p_name == "Name")
			{
				_Name = p_value;
				return;
			}
			if (p_name == "ST_VisualItemName")
			{
				VisualName = p_value;
			}
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null && attributeByGroupName.TrySetValue(p_name, p_value))
			{
			}
		}

		public virtual void SetOrAddValue(int p_value, string p_name, string p_groupName)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null)
			{
				if (attributeByGroupName.HasValue(p_name))
				{
					attributeByGroupName.TrySetValue(p_name, p_value);
				}
				else
				{
					attributeByGroupName.AddAttribute(p_name, Variable.CreateVariable(p_value.ToString(), string.Empty));
				}
			}
		}

		public virtual void SetOrAddValue(float p_value, string p_name, string p_groupName)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null)
			{
				if (attributeByGroupName.HasValue(p_name))
				{
					attributeByGroupName.TrySetValue(p_name, p_value);
				}
				else
				{
					attributeByGroupName.AddAttribute(p_name, Variable.CreateVariable(p_value.ToString(), string.Empty));
				}
			}
		}

		public virtual void SetOrAddValue(string p_value, string p_name, string p_groupName)
		{
			if (p_name == "Name")
			{
				_Name = p_value;
				return;
			}
			if (p_name == "ST_VisualItemName")
			{
				VisualName = p_value;
			}
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null)
			{
				if (attributeByGroupName.HasValue(p_name))
				{
					attributeByGroupName.TrySetValue(p_name, p_value);
				}
				else
				{
					attributeByGroupName.AddAttribute(p_name, Variable.CreateVariable(p_value, string.Empty));
				}
			}
		}

		public virtual void AppendValue(int p_value, string p_name, string p_groupName)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null && attributeByGroupName.TryAppendValue(p_name, p_value))
			{
			}
		}

		public virtual void AppendValue(float p_value, string p_name, string p_groupName)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null && attributeByGroupName.TryAppendValue(p_name, p_value))
			{
			}
		}

		public virtual void AppendValue(string p_value, string p_name, string p_groupName)
		{
			if (p_name == "Name")
			{
				_Name += p_value;
				return;
			}
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null && attributeByGroupName.TryAppendValue(p_name, p_value))
			{
			}
		}

		public void ChangeVar(Variable p_var, string p_name, string p_groupName)
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName(p_groupName);
			if (attributeByGroupName != null && attributeByGroupName.TryChangeVarByName(p_name, p_var))
			{
			}
		}

		public Variable GetVariable(string p_name)
		{
			return null;
		}

		public List<ItemGroupAttributes> GetIterableAttributes()
		{
			return GetAttributesByCondition((ItemGroupAttributes p_attr) => !p_attr.IsNoIterable);
		}

		private void SetVariableValue(ref Variable variable, string value, string defaultName = "")
		{
			VariableFunction variableFunction = Variable.CreateVariable(value, defaultName, this) as VariableFunction;
			if (variableFunction.IsPointer)
			{
				variableFunction.SimplifyArguments();
				variable = variableFunction;
			}
			else
			{
				variable.SetValue(variableFunction.ValueString);
			}
		}
	}
}
