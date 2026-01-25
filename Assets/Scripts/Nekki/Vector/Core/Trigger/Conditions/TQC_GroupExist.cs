using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TQC_GroupExist : TriggerQuestCondition
	{
		private string _ItemName;

		private string _GroupName;

		private string _Section;

		public TQC_GroupExist(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_node, p_parent)
		{
			_ItemName = XmlUtils.ParseString(p_node.Attributes["ItemName"]);
			_GroupName = XmlUtils.ParseString(p_node.Attributes["GroupName"]);
			_Section = XmlUtils.ParseString(p_node.Attributes["Section"]);
		}

		public override bool Check()
		{
			Item item = GetItem();
			return (item == null || !item.HasGroup(_GroupName)) ? _IsNot : (!_IsNot);
		}

		private Item GetItem()
		{
			if (_Section == null)
			{
				return DataLocal.Current.GetItemByName(_ItemName);
			}
			switch (_Section)
			{
			case "Equipped":
				return DataLocal.Current.GetItemByNameFromEquipped(_ItemName);
			case "Stash":
				return DataLocal.Current.GetItemByNameFromStash(_ItemName);
			case "All":
				return DataLocal.Current.GetItemByName(_ItemName);
			default:
				return null;
			}
		}
	}
}
