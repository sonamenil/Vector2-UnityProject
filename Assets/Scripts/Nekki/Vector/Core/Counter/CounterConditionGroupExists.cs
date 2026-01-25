using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterConditionGroupExists : CounterCondition
	{
		private Variable _ItemName;

		private Variable _GroupName;

		public CounterConditionGroupExists(XmlNode p_node)
			: base(p_node)
		{
			_ItemName = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Item"], string.Empty), null);
			string text = XmlUtils.ParseString(p_node.Attributes["Group"]);
			if (text != null)
			{
				_GroupName = Variable.CreateVariable(text, null);
			}
		}

		public CounterConditionGroupExists(Mapping p_node)
			: base(p_node)
		{
			_ItemName = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Item"), string.Empty), null);
			string stringValue = YamlUtils.GetStringValue(p_node.GetText("Group"), null);
			if (stringValue != null)
			{
				_GroupName = Variable.CreateVariable(stringValue, null);
			}
		}

		private CounterConditionGroupExists(CounterConditionGroupExists p_copy)
			: base(p_copy)
		{
			_ItemName = p_copy._ItemName;
			_GroupName = p_copy._GroupName;
		}

		public override bool Check()
		{
			Item item = GetItem();
			if (_GroupName == null)
			{
				return (!base.Not) ? (item != null) : (item == null);
			}
			return base.Not ? (item == null || !item.HasGroup(_GroupName.ValueString)) : (item != null && item.HasGroup(_GroupName.ValueString));
		}

		public override CounterCondition Copy()
		{
			return new CounterConditionGroupExists(this);
		}

		public override string ToString()
		{
			return "Condition GroupExists Item:" + _ItemName.ValueString + ((_GroupName != null) ? (" Group:" + _GroupName.ValueString) : string.Empty) + " Not:" + base.Not;
		}

		private Item GetItem()
		{
			return DataLocal.Current.GetItemByName(_ItemName.ValueString);
		}
	}
}
