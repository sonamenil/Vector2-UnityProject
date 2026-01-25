using System.Xml;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Swarm : TriggerRunnerAction
	{
		private Variable _Type;

		private Variable _SwarmName;

		private Variable _Waypoint;

		private Variable _ActionID;

		private TRA_Swarm(TRA_Swarm p_copyAction)
			: base(p_copyAction)
		{
			_Type = p_copyAction._Type;
			_SwarmName = p_copyAction._SwarmName;
			_Waypoint = p_copyAction._Waypoint;
			_ActionID = p_copyAction._ActionID;
		}

		public TRA_Swarm(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Type, p_node.Attributes["Type"].Value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _SwarmName, p_node.Attributes["SwarmName"].Value);
			if (p_node.Attributes["Waypoint"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Waypoint, p_node.Attributes["Waypoint"].Value);
			}
			if (p_node.Attributes["ActionID"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ActionID, p_node.Attributes["ActionID"].Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			Swarm swarmByName = RunMainController.Scene.Location.ControllerSwarm.GetSwarmByName(_SwarmName.ValueString);
			if (swarmByName != null)
			{
				switch (_Type.ValueString)
				{
				case "Spawn":
					SpawnSwarm(swarmByName);
					break;
				case "Activate":
					ActivateSwarm(swarmByName);
					break;
				case "Stop":
					StopSwarm(swarmByName);
					break;
				}
			}
		}

		private void SpawnSwarm(Swarm p_swarm)
		{
			p_swarm.SetEnabled(true, true);
			WaypointRunner waypointByName = ObjectRunner.GetWaypointByName(_Waypoint.ValueString, _ParentLoop.ParentTrigger.ParentElements.Parent.ParentRoot);
			if (waypointByName == null)
			{
				DebugUtils.Dialog("Action TRA_Swarm No Waypoint Name: " + _Waypoint.ValueString, true);
			}
			else
			{
				waypointByName.SpawnSwarm(p_swarm);
			}
		}

		private void ActivateSwarm(Swarm p_swarm)
		{
			TRA_Activate.ActivEvents(p_swarm, _ActionID.ValueString);
		}

		private void StopSwarm(Swarm p_swarm)
		{
			p_swarm.Stop();
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Swarm(this);
		}

		public override string ToString()
		{
			return "Swarm";
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Swarm");
			VectorLog.Tab(1);
			VectorLog.Untab(1);
		}
	}
}
