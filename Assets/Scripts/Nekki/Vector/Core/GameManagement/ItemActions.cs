using System.Collections.Generic;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class ItemActions
	{
		private abstract class ItemAction
		{
			public static ItemAction Create(Mapping p_node)
			{
				if (p_node == null)
				{
					return null;
				}
				ItemAction result = null;
				switch (p_node.GetText("Type").text)
				{
				case "Remove":
					result = new RemoveItem(p_node);
					break;
				case "RemoveGroup":
					result = new RemoveItemGroup(p_node);
					break;
				}
				return result;
			}

			public abstract void Activate();
		}

		private class RemoveItem : ItemAction
		{
			private Variable _ItemName;

			private bool _IsPartOfName;

			public RemoveItem(Mapping p_node)
			{
				_ItemName = ((p_node.GetText("Name") == null) ? null : Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Name"), string.Empty), string.Empty));
				_IsPartOfName = YamlUtils.GetBoolValue(p_node.GetText("IsPartOfName"));
			}

			public override void Activate()
			{
				List<UserItem> list = new List<UserItem>();
				foreach (UserItem allItem in DataLocal.Current.AllItems)
				{
					if (allItem.Name == _ItemName.ValueString || (_IsPartOfName && allItem.Name.Contains(_ItemName.ValueString)))
					{
						list.Add(allItem);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					DataLocal.Current.Remove(list[i]);
				}
			}
		}

		private class RemoveItemGroup : ItemAction
		{
			private Variable _ItemName;

			private Variable _GroupName;

			public RemoveItemGroup(Mapping p_node)
			{
				_ItemName = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Name"), string.Empty), string.Empty);
				_GroupName = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Group"), string.Empty), string.Empty);
			}

			public override void Activate()
			{
				UserItem itemByName = DataLocal.Current.GetItemByName(_ItemName.ValueString);
				if (itemByName != null)
				{
					itemByName.RemoveGroupAttributes(_GroupName.ValueString);
				}
			}
		}

		public const string NodeName = "ItemActions";

		private List<ItemAction> _OnEnter;

		private List<ItemAction> _OnExit;

		public static ItemActions Create(Mapping p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			ItemActions itemActions = new ItemActions();
			Sequence sequence = p_node.GetSequence("OnEnter");
			Sequence sequence2 = p_node.GetSequence("OnExit");
			if (sequence != null)
			{
				itemActions._OnEnter = new List<ItemAction>();
				foreach (Mapping item in sequence)
				{
					ItemAction itemAction = ItemAction.Create(item);
					if (itemAction != null)
					{
						itemActions._OnEnter.Add(itemAction);
					}
				}
			}
			if (sequence2 != null)
			{
				itemActions._OnExit = new List<ItemAction>();
				foreach (Mapping item2 in sequence2)
				{
					ItemAction itemAction2 = ItemAction.Create(item2);
					if (itemAction2 != null)
					{
						itemActions._OnExit.Add(itemAction2);
					}
				}
			}
			return itemActions;
		}

		public void ActivateOnEnter()
		{
			ActivateList(_OnEnter);
		}

		public void ActivateOnExit()
		{
			ActivateList(_OnExit);
		}

		private void ActivateList(List<ItemAction> p_list)
		{
			if (p_list != null)
			{
				for (int i = 0; i < p_list.Count; i++)
				{
					p_list[i].Activate();
				}
			}
		}
	}
}
