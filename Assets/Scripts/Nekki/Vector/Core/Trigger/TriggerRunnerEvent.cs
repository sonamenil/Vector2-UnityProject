using System.Xml;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerRunnerEvent : TriggerEvent
	{
		protected TriggerRunnerLoop _Parent;

		public static TriggerRunnerEvent Create(XmlNode p_node, TriggerRunnerLoop p_parent)
		{
			TriggerRunnerEvent triggerRunnerEvent = null;
			if (p_node.LocalName.Equals("Enter"))
			{
				triggerRunnerEvent = new TRE_Enter();
			}
			if (p_node.LocalName.Equals("Exit"))
			{
				triggerRunnerEvent = new TRE_Exit();
			}
			if (p_node.LocalName.Equals("Timeout"))
			{
				triggerRunnerEvent = new TRE_Timeout();
			}
			if (p_node.LocalName.Equals("KeyPressed"))
			{
				triggerRunnerEvent = new TRE_Key(string.Empty);
			}
			if (p_node.LocalName.Equals("Activate"))
			{
				triggerRunnerEvent = new TRE_Activate(string.Empty);
			}
			if (p_node.LocalName.Equals("Line"))
			{
				triggerRunnerEvent = new TRE_Line(p_parent, p_node);
			}
			if (p_node.LocalName.Equals("Collision"))
			{
				triggerRunnerEvent = new TRE_Collision();
			}
			if (p_node.LocalName.Equals("OnShow"))
			{
				triggerRunnerEvent = new TRE_OnShow(p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("OnHide"))
			{
				triggerRunnerEvent = new TRE_OnHide(p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("OnShowWidescreen"))
			{
				triggerRunnerEvent = new TRE_OnShowWidescreen(p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("OnHideWidescreen"))
			{
				triggerRunnerEvent = new TRE_OnHideWidescreen(p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("ValueChange"))
			{
				triggerRunnerEvent = new TRE_ChangeVar(p_node);
			}
			if (p_node.LocalName.Equals("OnStartGame"))
			{
				triggerRunnerEvent = new TRE_StartGame(p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("OnGlobalTimer"))
			{
				triggerRunnerEvent = new TRE_GlobalTimerTimeout(p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("SwarmArrival"))
			{
				triggerRunnerEvent = new TRE_SwarmArrival(p_parent.ParentTrigger, string.Empty);
			}
			if (p_node.LocalName.Equals("SwarmDeparture"))
			{
				triggerRunnerEvent = new TRE_SwarmDeparture(p_parent.ParentTrigger, string.Empty);
			}
			if (p_node.LocalName.Equals("SwarmDec"))
			{
				triggerRunnerEvent = new TRE_SwarmDec(p_parent.ParentTrigger, string.Empty);
			}
			if (p_node.LocalName.Equals("EndGame"))
			{
				triggerRunnerEvent = new TRE_EndGame(p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("ActivateNearPlayer"))
			{
				triggerRunnerEvent = new TRE_ActivateNearPlayer(string.Empty, p_parent.ParentTrigger);
			}
			if (p_node.LocalName.Equals("OnDeath"))
			{
				triggerRunnerEvent = new TRE_OnDeath(p_parent.ParentTrigger);
			}
			if (triggerRunnerEvent == null)
			{
				DebugUtils.Dialog("No Event type =" + p_node.Name, true);
				return null;
			}
			triggerRunnerEvent._Parent = p_parent;
			return triggerRunnerEvent;
		}

		public bool IsCollision()
		{
			return _Type == EventType.TRE_COLISION;
		}

		public bool IsTimeOutOrActivateOrOnStartGame()
		{
			if (_Type == EventType.TRE_ACTIVATE || _Type == EventType.TRE_TIMEOUT || _Type == EventType.TRE_ON_START_GAME || _Type == EventType.TRE_GLOBAL_TIMEOUT || _Type == EventType.TRE_ACTIVATE_NEAR_PLAYER)
			{
				return true;
			}
			return false;
		}
	}
}
