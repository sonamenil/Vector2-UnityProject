using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public class Preset
	{
		private string _Name;

		private Variable _ItemsCount;

		private List<Preset> _PresetBlock;

		private PresetDialogData _ActivDialog;

		private Variable _Target;

		private Variable _Destination;

		private Pick _Pick;

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public Variable ItemsCount
		{
			get
			{
				return _ItemsCount;
			}
		}

		public List<Preset> PresetBlock
		{
			get
			{
				return _PresetBlock;
			}
		}

		public PresetDialogData ActiveDilaog
		{
			get
			{
				return _ActivDialog;
			}
			set
			{
				_ActivDialog = value;
			}
		}

		private Preset(Mapping p_node, List<Preset> p_presetBlock)
		{
			_Name = p_node.key;
			_ItemsCount = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("ItemsCount"), "1"), string.Empty);
			_Pick = Pick.Create(p_node, this, true);
			_PresetBlock = p_presetBlock;
			Nekki.Yaml.Node node = p_node.GetNode("Target");
			if (node != null && node.IsMapping())
			{
				_Target = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetMapping("Target").GetText("Type"), "None"), string.Empty);
				_Destination = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetMapping("Target").GetText("Destination"), "None"), string.Empty);
			}
			else
			{
				_Target = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Target"), "None"), string.Empty);
			}
		}

		public static Preset Create(Mapping p_node, List<Preset> p_presetBlock = null)
		{
			return new Preset(p_node, p_presetBlock);
		}

		public PresetResult RunPreset()
		{
			if (!CheckCounterConditions())
			{
				PresetResult presetResult = new PresetResult();
				presetResult.Result = false;
				return presetResult;
			}
			UserItem userItem = null;
			switch (_Target.ValueString)
			{
			case "None":
			{
				Pick.PickResult pickResult = SetPreset(null);
				switch (pickResult)
				{
				case Pick.PickResult.ShowDialog:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = false;
					presetResult.DialogData = _ActivDialog;
					return presetResult;
				}
				case Pick.PickResult.RunPreset:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = false;
					presetResult.RunPreset = true;
					return presetResult;
				}
				default:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = pickResult == Pick.PickResult.Ok;
					return presetResult;
				}
				}
			}
			case "NewItem":
			{
				userItem = UserItem.CreateUserItem(string.Empty);
				Pick.PickResult pickResult = SetPreset(userItem);
				switch (pickResult)
				{
				case Pick.PickResult.ShowDialog:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = false;
					presetResult.DialogData = _ActivDialog;
					return presetResult;
				}
				case Pick.PickResult.RunPreset:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = false;
					presetResult.RunPreset = true;
					return presetResult;
				}
				default:
				{
					PresetResult.ItemType typeItem = PresetResult.ItemType.NewItem;
					PresetResult presetResult;
					if (pickResult == Pick.PickResult.Ok)
					{
						if (_Destination == null)
						{
							presetResult = new PresetResult();
							presetResult.Result = pickResult == Pick.PickResult.Ok;
							presetResult.Item = userItem;
							presetResult.TypeItem = typeItem;
							return presetResult;
						}
						switch (_Destination.ValueString)
						{
						case "Stash":
							typeItem = PresetResult.ItemType.AddToStashItem;
							DataLocal.Current.AddToStash(userItem);
							break;
						case "Equip":
							typeItem = PresetResult.ItemType.AddToEquipItem;
							DataLocal.Current.AddToEquipped(userItem);
							break;
						case "ShopBasket":
						{
							SupplyItem supplyItem = SupplyItem.Create(userItem);
							if (supplyItem != null)
							{
								typeItem = PresetResult.ItemType.AddToShopBascketItem;
								EndFloorManager.BasketItems.Add(supplyItem);
								break;
							}
							pickResult = Pick.PickResult.Fail;
							typeItem = PresetResult.ItemType.NullItem;
							userItem = null;
							DebugUtils.Dialog("Item not supply: " + userItem.Name, false);
							break;
						}
						}
					}
					presetResult = new PresetResult();
					presetResult.Result = pickResult == Pick.PickResult.Ok;
					presetResult.Item = userItem;
					presetResult.TypeItem = typeItem;
					return presetResult;
				}
				}
			}
			default:
			{
				userItem = DataLocal.Current.GetItemByName(_Target.ValueString);
				if (userItem == null)
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = false;
					return presetResult;
				}
				Pick.PickResult pickResult = SetPreset(userItem);
				switch (pickResult)
				{
				case Pick.PickResult.ShowDialog:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = false;
					presetResult.DialogData = _ActivDialog;
					return presetResult;
				}
				case Pick.PickResult.RunPreset:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = false;
					presetResult.RunPreset = true;
					return presetResult;
				}
				default:
				{
					PresetResult presetResult = new PresetResult();
					presetResult.Result = pickResult == Pick.PickResult.Ok;
					presetResult.Item = userItem;
					presetResult.TypeItem = PresetResult.ItemType.OldItem;
					return presetResult;
				}
				}
			}
			}
		}

		public void Run()
		{
			SetPreset(null);
		}

		private Pick.PickResult SetPreset(Item p_item)
		{
			Pick.PickResult result = _Pick.SetDefaults(p_item);
			if (p_item != null && p_item.Name == string.Empty)
			{
				p_item.Name = _Name + "_" + Random.Range(0, 1000);
			}
			return result;
		}

		public Pick.PickResult PostProcess(ref ReplacementData p_replacement, ref Dictionary<string, Variable> p_objectParams)
		{
			return _Pick.PostProcess(ref p_replacement, ref p_objectParams);
		}

		public void GetCounterConditions(List<CounterCondition> p_condition)
		{
			_Pick.GetCounterConditions(p_condition);
		}

		public void GetMusicContent(List<MusicContent> p_music)
		{
			_Pick.GetMusicContent(p_music);
		}

		public bool CheckCounterConditions()
		{
			return _Pick.CheckCounterCondition();
		}
	}
}
