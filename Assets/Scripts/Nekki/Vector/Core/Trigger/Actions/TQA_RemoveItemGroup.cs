using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.MainScene;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_RemoveItemGroup : TriggerQuestAction
	{
		private string _ItemName;

		private string _GroupName;

		public TQA_RemoveItemGroup(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_ItemName = XmlUtils.ParseString(p_node.Attributes["ItemName"]);
			_GroupName = XmlUtils.ParseString(p_node.Attributes["GroupName"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			UserItem itemByName = DataLocal.Current.GetItemByName(_ItemName);
			if (itemByName != null)
			{
				itemByName.RemoveGroupAttributes(_GroupName);
			}
			if (StarterPackItem.IsThis(itemByName) && Manager.IsEquip)
			{
				Scene<MainScene>.Current.RefreshStarterPacksUI();
			}
		}
	}
}
