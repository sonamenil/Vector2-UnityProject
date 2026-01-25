using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;

namespace Nekki.Vector.Core.User
{
	public class UserItem : Item
	{
		public delegate void OnChangeDelegate(Action Action, UserItem Item, int Value);

		private Dictionary<string, long> _TemporaryData = new Dictionary<string, long>();

		public bool HaveGroups
		{
			get
			{
				return _Attributes.Count > 0;
			}
		}

		public bool IsSaved
		{
			get
			{
				if (DataLocal.Current.StateRun == 0)
				{
					return true;
				}
				if (_ExpirationType == ItemExpirationType.IET_NONE)
				{
					return true;
				}
				if (_ExpirationType == ItemExpirationType.IET_TERMINAL && _ExpirationCounter <= 1)
				{
					return false;
				}
				if (_ExpirationType == ItemExpirationType.IET_FLOOR || _ExpirationType == ItemExpirationType.IET_FRAMES)
				{
					return false;
				}
				return true;
			}
		}

		public bool IsExpirationByFrame
		{
			get
			{
				return _ExpirationType == ItemExpirationType.IET_FRAMES;
			}
		}

		public bool IsExpirationByRun
		{
			get
			{
				return _ExpirationType == ItemExpirationType.IET_RUN;
			}
		}

		public bool IsExpirationByFloor
		{
			get
			{
				return _ExpirationType == ItemExpirationType.IET_FLOOR;
			}
		}

		public bool IsExpirationByTerminal
		{
			get
			{
				return _ExpirationType == ItemExpirationType.IET_TERMINAL;
			}
		}

		public bool IsCardsBought
		{
			get
			{
				UserItem itemByNameFromStash = DataLocal.Current.GetItemByNameFromStash(base.Name);
				if (itemByNameFromStash == null)
				{
					return false;
				}
				if (base.Groups.Count == 0)
				{
					return true;
				}
				foreach (ItemGroupAttributes group in base.Groups)
				{
					if (!itemByNameFromStash.HasGroup(group.GroupName))
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool IsInUser
		{
			get
			{
				return DataLocal.Current.AllItems.Contains(this);
			}
		}

		public bool IsInStash
		{
			get
			{
				return DataLocal.Current.Stash.Contains(this);
			}
		}

		public bool IsInEquipped
		{
			get
			{
				return DataLocal.Current.Equipped.Contains(this);
			}
		}

		public event OnChangeDelegate OnChange;

		private UserItem(XmlNode p_node)
			: base(p_node)
		{
			LoadExpirationFromXml(p_node["ST_Expiration"]);
			ParseGroups(p_node);
			LoadTemporary(p_node);
		}

		private UserItem(string Name)
			: base(Name)
		{
		}

		private UserItem(UserItem p_copy)
			: base(p_copy)
		{
			foreach (KeyValuePair<string, long> temporaryDatum in p_copy._TemporaryData)
			{
				_TemporaryData.Add(temporaryDatum.Key, temporaryDatum.Value);
			}
		}

		public UserItem Copy()
		{
			return new UserItem(this);
		}

		public static UserItem CreateUserItem(string Name)
		{
			return new UserItem(Name);
		}

		public static UserItem CreateByXmlNode(XmlNode p_node)
		{
			UserItem userItem = new UserItem(p_node);
			if (!userItem.IsSaved && !Demo.IsPlaying && !GameRestorer.Active)
			{
				userItem = null;
			}
			return userItem;
		}

		public void AppendUserItem(UserItem p_item, bool p_replace = true)
		{
			foreach (ItemGroupAttributes attribute in p_item._Attributes)
			{
				if (_AttributesDic.ContainsKey(attribute.GroupName))
				{
					_AttributesDic[attribute.GroupName].AppendByAttribute(attribute, p_replace);
				}
				else
				{
					AddGroupAttributes(attribute);
				}
			}
		}

		public void SaveToXmlNode(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Item");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Name", _Name);
			xmlElement.SetAttribute("VisualName", base.VisualName);
			if (_ForceSave)
			{
				xmlElement.SetAttribute("ForceSave", "1");
			}
			if (_Type != 0)
			{
				xmlElement.SetAttribute("ItemType", base.Type.ToString());
			}
			SaveExpiration(xmlElement);
			foreach (ItemGroupAttributes attribute in _Attributes)
			{
				attribute.SaveToXml(xmlElement);
			}
			SaveTemporary(xmlElement);
		}

		private void SaveExpiration(XmlNode p_itemNode)
		{
			if (_ExpirationType != 0)
			{
				XmlElement xmlElement = p_itemNode.OwnerDocument.CreateElement("ST_Expiration");
				p_itemNode.AppendChild(xmlElement);
				xmlElement.SetAttribute("Type", Item.ConvertItemExpirationTypeToString(_ExpirationType));
				xmlElement.SetAttribute("Counter", _ExpirationCounter.ToString());
			}
		}

		private void SaveTemporary(XmlNode p_node)
		{
			if (_TemporaryData.Count == 0)
			{
				return;
			}
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Temporary");
			p_node.AppendChild(xmlElement);
			foreach (KeyValuePair<string, long> temporaryDatum in _TemporaryData)
			{
				xmlElement.SetAttribute(temporaryDatum.Key, temporaryDatum.Value.ToString());
			}
		}

		public void LoadExpirationFromXml(XmlNode p_expirationNode)
		{
			if (p_expirationNode != null)
			{
				_ExpirationType = Item.ParsExpirationTypeByString(p_expirationNode.Attributes["Type"].Value);
				_ExpirationCounter = int.Parse(p_expirationNode.Attributes["Counter"].Value);
			}
		}

		private void LoadTemporary(XmlNode p_node)
		{
			if (p_node["Temporary"] == null)
			{
				return;
			}
			foreach (XmlAttribute attribute in p_node["Temporary"].Attributes)
			{
				_TemporaryData.Add(attribute.Name, XmlUtils.ParseLong(attribute));
			}
		}

		public override void SetValue(int p_value, string p_name, string p_groupName)
		{
			base.SetValue(p_value, p_name, p_groupName);
			if (p_groupName == "Countable" && p_name == "Quantity" && this.OnChange != null)
			{
				this.OnChange(Action.Set, this, p_value);
				if (p_value < 0)
				{
					DataLocal.Current.Remove(this);
				}
			}
		}

		public override void SetOrAddValue(int p_value, string p_name, string p_groupName)
		{
			base.SetOrAddValue(p_value, p_name, p_groupName);
			if (p_groupName == "Countable" && p_name == "Quantity" && this.OnChange != null)
			{
				this.OnChange(Action.Set, this, p_value);
				if (p_value < 0)
				{
					DataLocal.Current.Remove(this);
				}
			}
		}

		public override void AppendValue(int p_value, string p_name, string p_groupName)
		{
			base.AppendValue(p_value, p_name, p_groupName);
			if (p_groupName == "Countable" && p_name == "Quantity" && this.OnChange != null)
			{
				this.OnChange(Action.Change, this, p_value);
				if (GetIntValueAttribute(p_name, p_groupName) <= 0)
				{
					DataLocal.Current.Remove(this);
				}
			}
		}

		public void SetTemplary(string p_name, long p_value)
		{
			if (_TemporaryData.ContainsKey(p_name))
			{
				_TemporaryData[p_name] = p_value;
			}
			else
			{
				_TemporaryData.Add(p_name, p_value);
			}
		}

		public bool GetTemplary(string p_name, ref long p_value)
		{
			if (!_TemporaryData.ContainsKey(p_name))
			{
				return false;
			}
			p_value = _TemporaryData[p_name];
			return true;
		}

		public void RemoveTemplary(string p_name)
		{
			if (_TemporaryData.ContainsKey(p_name))
			{
				_TemporaryData.Remove(p_name);
			}
		}

		public void ExtraActionsOnBuy()
		{
			ItemGroupAttributes attributeByGroupName = GetAttributeByGroupName("ST_ShopSignal");
			if (attributeByGroupName == null)
			{
				return;
			}
			string p_value = string.Empty;
			if (!attributeByGroupName.TryGetStrValue("ST_CounterName", ref p_value))
			{
				return;
			}
			string p_value2 = "Increment";
			attributeByGroupName.TryGetStrValue("ST_Type", ref p_value2);
			string p_value3 = string.Empty;
			attributeByGroupName.TryGetStrValue("ST_Namespace", ref p_value3);
			if (p_value2 == "Assign")
			{
				int p_value4 = 0;
				if (attributeByGroupName.TryGetIntValue("ST_Value", ref p_value4))
				{
					if (!string.IsNullOrEmpty(p_value3))
					{
						CounterController.Current.CreateCounterOrSetValue(p_value, p_value4, p_value3);
					}
					else
					{
						CounterController.Current.CreateCounterOrSetValue(p_value, p_value4);
					}
				}
			}
			else if (p_value2 == "Increment")
			{
				if (!string.IsNullOrEmpty(p_value3))
				{
					CounterController.Current.IncrementUserCounter(p_value, 1, p_value3);
				}
				else
				{
					CounterController.Current.IncrementUserCounter(p_value, 1);
				}
			}
			_AttributesDic.Remove("ST_ShopSignal");
			_Attributes.Remove(attributeByGroupName);
		}

		public void ChangeCryptoKeyOnAttributes()
		{
			for (int i = 0; i < _Attributes.Count; i++)
			{
				_Attributes[i].ChangeCryptoKeyOnVars();
			}
		}
	}
}
