using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerQuestEvent : TriggerEvent
	{
		public static TriggerQuestEvent Create(XmlNode p_node)
		{
			switch (p_node.Name)
			{
			case "OnScreen":
				return new TQE_OnScreen(p_node.Attributes["Name"].Value);
			case "OnCall":
				return new TQE_OnCall(p_node);
			case "OnBuyItem":
				return new TQE_OnBuyItem(p_node);
			case "OnBuyCard":
				return new TQE_OnBuyCard(p_node);
			default:
				return null;
			}
		}

		public static void Parse(XmlNode p_node, List<TriggerEvent> p_result)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				TriggerQuestEvent triggerQuestEvent = Create(childNode);
				if (triggerQuestEvent != null)
				{
					p_result.Add(triggerQuestEvent);
				}
			}
		}
	}
}
