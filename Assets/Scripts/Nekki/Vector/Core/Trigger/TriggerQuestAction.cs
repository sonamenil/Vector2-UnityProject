using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger.Actions;

namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerQuestAction
	{
		protected TriggerQuestLoop _Parent;

		protected TriggerQuestAction(TriggerQuestLoop p_parent)
		{
			_Parent = p_parent;
		}

		public static void Parse(XmlNode p_node, TriggerQuestLoop p_parent, List<TriggerQuestAction> p_result)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				TriggerQuestAction triggerQuestAction = Create(childNode, p_parent);
				if (triggerQuestAction != null)
				{
					p_result.Add(triggerQuestAction);
				}
			}
		}

		private static TriggerQuestAction Create(XmlNode p_node, TriggerQuestLoop p_parent)
		{
			switch (p_node.Name)
			{
			case "SetCounter":
				return new TQA_SetCounter(p_node, p_parent);
			case "IncrementCounter":
				return new TQA_IncrementCounter(p_node, p_parent);
			case "ClearCounter":
				return new TQA_ClearCounter(p_node, p_parent);
			case "AddItem":
				return new TQA_AddItem(p_node, p_parent);
			case "QuestComplete":
				return new TQA_QuestComplete(p_parent);
			case "QuestStart":
				return new TQA_QuestStart(p_parent);
			case "Dialog":
				return new TQA_Dialog(p_node, p_parent);
			case "StartDialog":
				return new TQA_StartDialog(p_node, p_parent);
			case "CompleteDialog":
				return new TQA_CompleteDialog(p_node, p_parent);
			case "Notification":
				return new TQA_Notification(p_node, p_parent);
			case "SetString":
				return new TQA_SetString(p_node, p_parent);
			case "ExecuteCall":
				return new TQA_ExecuteCall(p_node, p_parent);
			case "Statistics":
				return new TQA_Statistics(p_node, p_parent);
			case "RemoveItemGroup":
				return new TQA_RemoveItemGroup(p_node, p_parent);
			case "Chapter":
				return new TQA_Chapter(p_node, p_parent);
			case "HideChapter":
				return new TQA_HideChapter(p_parent);
			case "SetStarDialog":
				return new TQA_SetStarDialog(p_parent);
			case "TutorialSequence":
				return new TQA_TutorialSequence(p_node, p_parent);
			case "Delay":
				return new TQA_Delay(p_node, p_parent);
			case "SetVariable":
				return new TQA_SetVariable(p_node, p_parent);
			case "AddBundleRequest":
				return new TQA_AddBundleRequest(p_node, p_parent);
			case "WaitForBundleRequestsDone":
				return new TQA_WaitForBundleRequestsDone(p_node, p_parent);
			default:
				DebugUtils.Dialog("Unknown action: " + p_node.Name, true);
				return null;
			}
		}

		public abstract void Activate(ref bool p_runNext);
	}
}
