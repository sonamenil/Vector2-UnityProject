using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class Pick
	{
		public enum PickResult
		{
			Ok = 0,
			Fail = 1,
			Cancel = 2,
			Postpone = 3,
			ShowDialog = 4,
			RunPreset = 5
		}

		private string _Name;

		private Preset _Parent;

		private bool _AllowDoubles;

		private bool _NoShuffle;

		private bool _CancelGeneration;

		private bool _PostponeGeneration;

		private bool _BreakSelect;

		private SelectData _SelectData;

		private Variable _Weight;

		private Variable _Set;

		private Variable _ItemsCount;

		private List<Pick> _Picks;

		private List<Mapping> _Groups;

		private List<Mapping> _GroupReplace;

		private CounterActions _CounterActions;

		private StringActions _StringActions;

		private ItemActions _ItemActions;

		private TimerActions _TimerActions;

		private List<CounterCondition> _CounterConditions;

		private List<CounterCondition> _ConditionContent;

		private RunPresetData _RunPresetData;

		private PresetDialogData _DialogData;

		private PostProcessData _PostProcessData;

		private List<MusicContent> _MusicContent;

		private Pick(Mapping p_node, Preset p_parent, bool isRoot = false)
		{
			_Name = p_node.key;
			_Parent = p_parent;
			_Set = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Set"), "-1"), string.Empty);
			_ItemsCount = (isRoot ? Variable.CreateVariable("1", string.Empty) : Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("ItemsCount"), "1"), string.Empty));
			_AllowDoubles = YamlUtils.GetBoolValue(p_node.GetText("AllowDoubles"));
			_Weight = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Weight"), "1"), string.Empty);
			_NoShuffle = YamlUtils.GetBoolValue(p_node.GetText("NoShuffle"));
			_CancelGeneration = YamlUtils.GetBoolValue(p_node.GetText("CancelGeneration"));
			_PostponeGeneration = YamlUtils.GetBoolValue(p_node.GetText("PostponeGeneration"));
			_BreakSelect = YamlUtils.GetBoolValue(p_node.GetText("BreakSelect"));
			_CounterConditions = CounterCondition.CreateListConditions(p_node.GetSequence("Conditions"), "ST_Default");
			_ConditionContent = CounterCondition.CreateListConditions(p_node.GetSequence("ConditionContent"), "ST_Default");
			_CounterActions = CounterActions.Create(p_node.GetMapping("CounterActions"), "ST_Default");
			_StringActions = StringActions.Create(p_node.GetMapping("StringActions"));
			_ItemActions = ItemActions.Create(p_node.GetMapping("ItemActions"));
			_TimerActions = TimerActions.Create(p_node.GetMapping("TimerActions"));
			_PostProcessData = PostProcessData.Create(p_node.GetMapping("PostProcessData"));
			_MusicContent = MusicContent.CreateList(p_node.GetSequence("MusicContent"));
			_SelectData = SelectData.Create(p_node.GetMapping("Select"));
			_RunPresetData = RunPresetData.Create(p_node.GetSequence("RunPreset"), p_parent);
			_DialogData = PresetDialogData.Create(p_node.GetMapping("ShowDialog"), p_parent);
			Mapping mapping = p_node.GetMapping("BoxContent");
			Mapping mapping2 = p_node.GetMapping("GroupContent");
			Mapping mapping3 = p_node.GetMapping("ReplaceGroup");
			if (mapping != null)
			{
				_Picks = new List<Pick>();
				foreach (Mapping item3 in mapping)
				{
					Pick pick = Create(item3, p_parent);
					if (pick != null)
					{
						_Picks.Add(pick);
					}
				}
			}
			if (mapping2 != null)
			{
				_Groups = new List<Mapping>();
				foreach (Mapping item4 in mapping2)
				{
					_Groups.Add(item4);
				}
			}
			if (mapping3 == null)
			{
				return;
			}
			_GroupReplace = new List<Mapping>();
			foreach (Mapping item5 in mapping3)
			{
				_GroupReplace.Add(item5);
			}
		}

		public static Pick Create(Mapping p_node, Preset p_parent, bool isRoot = false)
		{
			if (p_node.key == "CounterActions")
			{
				return null;
			}
			return new Pick(p_node, p_parent, isRoot);
		}

		public bool CheckCounterCondition()
		{
			return CounterCondition.CheckCounterConditionList(_CounterConditions);
		}

		private void ActivateActionsOnEnter()
		{
			if (_CounterActions != null)
			{
				_CounterActions.ActivateOnEnter();
			}
			if (_StringActions != null)
			{
				_StringActions.ActivateOnEnter();
			}
			if (_ItemActions != null)
			{
				_ItemActions.ActivateOnEnter();
			}
			if (_TimerActions != null)
			{
				_TimerActions.ActivateOnEnter();
			}
		}

		private void ActivateActionsOnExit()
		{
			if (_CounterActions != null)
			{
				_CounterActions.ActivateOnExit();
			}
			if (_StringActions != null)
			{
				_StringActions.ActivateOnExit();
			}
			if (_ItemActions != null)
			{
				_ItemActions.ActivateOnExit();
			}
			if (_TimerActions != null)
			{
				_TimerActions.ActivateOnExit();
			}
		}

		public PickResult GetCounterConditions(List<CounterCondition> p_conditions)
		{
			Func<Pick, PickResult> p_workMethod = (Pick p_pick) => p_pick.GetCounterConditions(p_conditions);
			System.Action p_processingActions = delegate
			{
				if (_ConditionContent != null)
				{
					for (int i = 0; i < _ConditionContent.Count; i++)
					{
						p_conditions.Add(_ConditionContent[i].Copy());
					}
				}
			};
			return Activate(p_workMethod, p_processingActions);
		}

		public PickResult SetDefaults(Item p_item)
		{
			Func<Pick, PickResult> p_workMethod = (Pick p_pick) => p_pick.SetDefaults(p_item);
			System.Action p_processingActions = delegate
			{
				if (_Groups != null)
				{
					for (int i = 0; i < _Groups.Count; i++)
					{
						p_item.CreateGroupAttibute(_Groups[i]);
					}
				}
				if (_GroupReplace != null)
				{
					for (int j = 0; j < _GroupReplace.Count; j++)
					{
						p_item.ReplaceGroupAttribute(_GroupReplace[j]);
					}
				}
			};
			return Activate(p_workMethod, p_processingActions);
		}

		public PickResult PostProcess(ref ReplacementData p_replacement, ref Dictionary<string, Variable> p_objectParams)
		{
			ReplacementData localReplacement = p_replacement;
			Dictionary<string, Variable> localObjectParams = p_objectParams;
			Func<Pick, PickResult> p_workMethod = (Pick p_pick) => p_pick.PostProcess(ref localReplacement, ref localObjectParams);
			System.Action p_processingActions = delegate
			{
				GetPostProcessData(ref localReplacement, ref localObjectParams);
			};
			PickResult pickResult = Activate(p_workMethod, p_processingActions);
			if (pickResult != PickResult.Cancel)
			{
				p_replacement = localReplacement;
				p_objectParams = localObjectParams;
			}
			return pickResult;
		}

		public PickResult GetMusicContent(List<MusicContent> p_music)
		{
			Func<Pick, PickResult> p_workMethod = (Pick p_pick) => p_pick.GetMusicContent(p_music);
			System.Action p_processingActions = delegate
			{
				if (_MusicContent != null)
				{
					p_music.AddRange(_MusicContent);
				}
			};
			return Activate(p_workMethod, p_processingActions);
		}

		private PickResult Activate(Func<Pick, PickResult> p_workMethod, System.Action p_processingActions)
		{
			PickResult pickResult = PickResult.Ok;
			int i = 0;
			for (int valueInt = _ItemsCount.ValueInt; i < valueInt; i++)
			{
				pickResult = ((_SelectData != null) ? ActivateSelect(p_workMethod, p_processingActions) : ActivateInternal(p_workMethod, p_processingActions));
				if (pickResult == PickResult.Cancel)
				{
					return pickResult;
				}
			}
			return pickResult;
		}

		private PickResult ActivateSelect(Func<Pick, PickResult> p_workMethod, System.Action p_processingActions)
		{
			SelectData.BreakFlag = false;
			PickResult pickResult = PickResult.Ok;
			if (_SelectData.Type == SelectType.Balance || _SelectData.Type == SelectType.Cards)
			{
				Mapping mapping = ((_SelectData.Type != 0) ? CardsManager.Current.GetCardMapping(_SelectData.YamlPath) : BalanceManager.Current.GetBalanceMapping(_SelectData.YamlPath));
				List<Nekki.Yaml.Node> list;
				if (_SelectData.Random)
				{
					list = new List<Nekki.Yaml.Node>();
					list.AddRange(mapping.nodesInside);
					if (_SelectData.Weight != null)
					{
						MainRandom.ShuffleList(list, GetYamlNodesWeights(list));
					}
					else
					{
						MainRandom.ShuffleList(list);
					}
				}
				else
				{
					list = mapping.nodesInside;
				}
				{
					foreach (Nekki.Yaml.Node item in list)
					{
						StringBuffer.AddString("SelectKey", item.key);
						pickResult = ActivateInternal(p_workMethod, p_processingActions);
						StringBuffer.AddString("SelectKey", string.Empty);
						if (pickResult == PickResult.Cancel || SelectData.BreakFlag)
						{
							SelectData.BreakFlag = false;
							return pickResult;
						}
					}
					return pickResult;
				}
			}
			if (_SelectData.Type == SelectType.ItemGroups)
			{
				UserItem itemByName = DataLocal.Current.GetItemByName(_SelectData.Root.ValueString);
				if (itemByName == null)
				{
					DebugUtils.Dialog("In Preset:" + _Name + " with ItemGroups Select item == null", false);
					return pickResult;
				}
				List<ItemGroupAttributes> list2;
				if (_SelectData.Random)
				{
					list2 = new List<ItemGroupAttributes>();
					list2.AddRange(itemByName.GetIterableAttributes());
					if (_SelectData.Weight != null)
					{
						MainRandom.ShuffleList(list2, GetGroupsWeights(list2));
					}
					else
					{
						MainRandom.ShuffleList(list2);
					}
				}
				else
				{
					list2 = itemByName.GetIterableAttributes();
				}
				{
					foreach (ItemGroupAttributes item2 in list2)
					{
						StringBuffer.AddString("SelectKey", item2.GroupName);
						pickResult = ActivateInternal(p_workMethod, p_processingActions);
						StringBuffer.AddString("SelectKey", string.Empty);
						if (pickResult == PickResult.Cancel || SelectData.BreakFlag)
						{
							SelectData.BreakFlag = false;
							return pickResult;
						}
					}
					return pickResult;
				}
			}
			if (_SelectData.Type == SelectType.Items)
			{
				List<UserItem> list3 = new List<UserItem>();
				string valueString = _SelectData.Root.ValueString;
				if (valueString == "Stash")
				{
					list3.AddRange(DataLocal.Current.Stash);
				}
				else if (valueString == "Equipped")
				{
					list3.AddRange(DataLocal.Current.Equipped);
				}
				else
				{
					list3.AddRange(DataLocal.Current.AllItems);
				}
				if (_SelectData.Random)
				{
					if (_SelectData.Weight != null)
					{
						MainRandom.ShuffleList(list3, GetItemsWeights(list3));
					}
					else
					{
						MainRandom.ShuffleList(list3);
					}
				}
				{
					foreach (UserItem item3 in list3)
					{
						StringBuffer.AddString("SelectKey", item3.Name);
						pickResult = ActivateInternal(p_workMethod, p_processingActions);
						StringBuffer.AddString("SelectKey", string.Empty);
						if (pickResult == PickResult.Cancel || SelectData.BreakFlag)
						{
							SelectData.BreakFlag = false;
							return pickResult;
						}
					}
					return pickResult;
				}
			}
			if (_SelectData.Type == SelectType.Range)
			{
				List<int> list4 = new List<int>();
				for (int i = 1; i <= _SelectData.RangeValue.ValueInt; i++)
				{
					list4.Add(i);
				}
				if (_SelectData.Random)
				{
					MainRandom.ShuffleList(list4);
				}
				{
					foreach (int item4 in list4)
					{
						StringBuffer.AddString("SelectKey", item4.ToString());
						pickResult = ActivateInternal(p_workMethod, p_processingActions);
						StringBuffer.AddString("SelectKey", string.Empty);
						if (pickResult == PickResult.Cancel || SelectData.BreakFlag)
						{
							SelectData.BreakFlag = false;
							return pickResult;
						}
					}
					return pickResult;
				}
			}
			List<string> strings = StringBuffer.GetStrings(_SelectData.Root.ToString());
			if (_SelectData.Random)
			{
				MainRandom.ShuffleList(strings);
			}
			foreach (string item5 in strings)
			{
				StringBuffer.AddString("SelectKey", item5);
				pickResult = ActivateInternal(p_workMethod, p_processingActions);
				StringBuffer.AddString("SelectKey", string.Empty);
				if (pickResult == PickResult.Cancel || SelectData.BreakFlag)
				{
					SelectData.BreakFlag = false;
					return pickResult;
				}
			}
			return pickResult;
		}

		private PickResult ActivateInternal(Func<Pick, PickResult> p_workMethod, System.Action p_processingActions)
		{
			if (!CheckCounterCondition())
			{
				return PickResult.Ok;
			}
			if (_CancelGeneration)
			{
				return PickResult.Cancel;
			}
			if (_PostponeGeneration)
			{
				ActivateActionsOnEnter();
				ActivateActionsOnExit();
				return PickResult.Postpone;
			}
			SelectData.BreakFlag = _BreakSelect;
			ActivateActionsOnEnter();
			if (_RunPresetData != null)
			{
				_RunPresetData.Activate();
				ActivateActionsOnExit();
				return PickResult.RunPreset;
			}
			if (_DialogData != null)
			{
				_DialogData.Show();
				_Parent.ActiveDilaog = _DialogData;
				ActivateActionsOnExit();
				return PickResult.ShowDialog;
			}
			p_processingActions();
			if (_Picks == null)
			{
				ActivateActionsOnExit();
				return PickResult.Ok;
			}
			if (_Set.ValueInt == -1 && _AllowDoubles)
			{
				DebugUtils.Dialog("In Preset:" + _Name + " with AllowDuplicates Set=-1", false);
				ActivateActionsOnExit();
				return PickResult.Fail;
			}
			if (_SelectData != null && _AllowDoubles)
			{
				DebugUtils.Dialog("In Preset:" + _Name + " with AllowDuplicates Select", false);
				ActivateActionsOnExit();
				return PickResult.Fail;
			}
			PickResult pickResult = PickResult.Ok;
			if (_Set.ValueInt == -1)
			{
				for (int i = 0; i < _Picks.Count; i++)
				{
					if (_Picks[i].CheckCounterCondition())
					{
						pickResult = p_workMethod(_Picks[i]);
						if (pickResult != 0)
						{
							break;
						}
					}
				}
				ActivateActionsOnExit();
				return pickResult;
			}
			List<Pick> list = new List<Pick>();
			for (int j = 0; j < _Picks.Count; j++)
			{
				if (_Picks[j].CheckCounterCondition())
				{
					list.Add(_Picks[j]);
				}
			}
			if (list.Count == 0)
			{
				DebugUtils.Dialog("In Preset:" + _Name + " result.Count == 0", false);
				DataLocal.Current.Save(true);
				return PickResult.Fail;
			}
			if (_AllowDoubles)
			{
				if (_NoShuffle)
				{
					int count = list.Count;
					for (int k = 0; k < _Set.ValueInt; k++)
					{
						pickResult = p_workMethod(list[k % count]);
						if (pickResult != 0)
						{
							break;
						}
					}
				}
				else
				{
					for (int l = 0; l < _Set.ValueInt; l++)
					{
						Pick randomPick = GetRandomPick(list);
						pickResult = p_workMethod(randomPick);
						if (pickResult != 0)
						{
							break;
						}
					}
				}
			}
			else
			{
				if (list.Count < _Set.ValueInt)
				{
					DebugUtils.Dialog("In Preset:" + _Name + " result.Count < _Set", false);
					return PickResult.Fail;
				}
				if (_NoShuffle)
				{
					for (int m = 0; m < _Set.ValueInt; m++)
					{
						pickResult = p_workMethod(list[m]);
						if (pickResult != 0)
						{
							break;
						}
					}
				}
				else
				{
					for (int n = 0; n < _Set.ValueInt; n++)
					{
						Pick randomPick2 = GetRandomPick(list);
						pickResult = p_workMethod(randomPick2);
						if (pickResult != 0)
						{
							break;
						}
						list.Remove(randomPick2);
					}
				}
			}
			ActivateActionsOnExit();
			return pickResult;
		}

		private void GetPostProcessData(ref ReplacementData p_replacement, ref Dictionary<string, Variable> p_objectParams)
		{
			if (_PostProcessData != null)
			{
				p_replacement = _PostProcessData.Replacement ?? p_replacement;
				p_objectParams = _PostProcessData.ObjectParams ?? p_objectParams;
			}
		}

		private Pick GetRandomPick(List<Pick> p_value)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < p_value.Count; i++)
			{
				num += p_value[i]._Weight.ValueInt;
			}
			int num3 = (int)MainRandom.Current.randomInt(0u, (uint)num);
			for (int j = 0; j < p_value.Count; j++)
			{
				num2 += p_value[j]._Weight.ValueInt;
				if (num2 > num3)
				{
					return p_value[j];
				}
			}
			return p_value[0];
		}

		private List<uint> GetYamlNodesWeights(List<Nekki.Yaml.Node> nodes)
		{
			List<uint> list = new List<uint>();
			foreach (Nekki.Yaml.Node node in nodes)
			{
				AddWeightItem(node.key, list);
			}
			return list;
		}

		private List<uint> GetGroupsWeights(List<ItemGroupAttributes> p_groups)
		{
			List<uint> list = new List<uint>();
			foreach (ItemGroupAttributes p_group in p_groups)
			{
				AddWeightItem(p_group.GroupName, list);
			}
			return list;
		}

		private List<uint> GetItemsWeights(List<UserItem> p_items)
		{
			List<uint> list = new List<uint>();
			foreach (UserItem p_item in p_items)
			{
				AddWeightItem(p_item.Name, list);
			}
			return list;
		}

		private void AddWeightItem(string name, List<uint> weights)
		{
			StringBuffer.AddString("SelectKey", name);
			weights.Add((uint)_SelectData.Weight.ValueInt);
			StringBuffer.AddString("SelectKey", string.Empty);
		}
	}
}
