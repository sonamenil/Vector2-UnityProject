using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core.Runners
{
	public class WaypointRunner : Runner
	{
		private Dictionary<string, int[]> _NextWaypoints;

		private int _AccelerationFramesS = -1;

		private int _AccelerationFramesE = -1;

		private float _Speed = float.NaN;

		private float _SpawnX = float.NaN;

		private float _SpawnY = float.NaN;

		private int _SpawnDelay;

		private string _WayPointKey;

		public int AccelerationFramesS
		{
			get
			{
				return _AccelerationFramesS;
			}
		}

		public int AccelerationFramesE
		{
			get
			{
				return _AccelerationFramesE;
			}
		}

		public float Speed
		{
			get
			{
				return _Speed;
			}
		}

		public float SpawnX
		{
			get
			{
				return _SpawnX;
			}
		}

		public float SpawnY
		{
			get
			{
				return _SpawnY;
			}
		}

		public int SpawnDelay
		{
			get
			{
				return _SpawnDelay;
			}
		}

		public string WayPointKey
		{
			get
			{
				return _WayPointKey;
			}
		}

		public WaypointRunner(float p_x, float p_y, string p_name, XmlNode p_node, Element p_elements)
			: base(p_x, p_y, p_elements)
		{
			_TypeClass = TypeRunner.Waypoint;
			_Name = p_name;
			_SpawnX = XmlUtils.ParseFloat(p_node.Attributes["SpawnX"], float.NaN);
			_SpawnY = XmlUtils.ParseFloat(p_node.Attributes["SpawnY"], float.NaN);
			_SpawnDelay = XmlUtils.ParseInt(p_node.Attributes["SpawnDelay"]);
			_WayPointKey = XmlUtils.ParseString(p_node.Attributes["WaypointKey"], string.Empty);
			XmlNode xmlNode = p_node["Properties"]["Static"];
			ParseNext(xmlNode["Next"]);
			ParseMotion(xmlNode["Motion"]);
		}

		private void ParseNext(XmlNode p_node)
		{
			if (p_node == null)
			{
				return;
			}
			_NextWaypoints = new Dictionary<string, int[]>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name != "Waypoint")
				{
					DebugUtils.Dialog("Error parse Next section Waypoints :" + p_node.Name, false);
					continue;
				}
				_NextWaypoints.Add(childNode.Attributes["Name"].Value, new int[2]
				{
					XmlUtils.ParseInt(childNode.Attributes["Delay"]),
					XmlUtils.ParseInt(childNode.Attributes["SpeedDelta"])
				});
			}
		}

		private void ParseMotion(XmlNode p_node)
		{
			if (p_node != null)
			{
				_AccelerationFramesS = XmlUtils.ParseInt(p_node.Attributes["StartAccFrames"], -1);
				_AccelerationFramesE = XmlUtils.ParseInt(p_node.Attributes["StopAccFrames"], -1);
				_Speed = XmlUtils.ParseFloat(p_node.Attributes["Speed"], float.NaN);
			}
		}

		public void SpawnSwarm(Swarm p_swarm)
		{
			p_swarm.SetPosition(base.Position);
			if (!float.IsNaN(SpawnX) && !float.IsNaN(SpawnY))
			{
				p_swarm.SetDefPos(SpawnX, SpawnY);
			}
			p_swarm.IsActive = true;
			SwarmArrive(p_swarm, SpawnDelay);
		}

		public void SwarmArrive(Swarm p_swarm, int additionalDelay = 0)
		{
			KeyValuePair<WaypointRunner, int[]> next = GetNext(RunMainController.Location.Sets.GetRoom((int)base.Position.x));
			p_swarm.DefaultMoveCoords.Set(SpawnX, SpawnY, 0f);
			TRE_SwarmArrival.ActivateThisEvent(_WayPointKey);
			if (next.Key != null)
			{
				p_swarm.RunTo(next.Key, next.Value[0] + additionalDelay, next.Value[1]);
			}
			else
			{
				p_swarm.Stop();
			}
		}

		private KeyValuePair<WaypointRunner, int[]> GetNext(Room p_room)
		{
			if (_NextWaypoints == null || p_room == null)
			{
				return new KeyValuePair<WaypointRunner, int[]>(null, new int[2]);
			}
			WaypointRunner waypointRunner = null;
			foreach (string key in _NextWaypoints.Keys)
			{
				if (key == "next_room")
				{
					waypointRunner = GetNext(RunMainController.Location.Sets.GetNextRoom((int)base.Position.x), "Start");
					if (waypointRunner != null && waypointRunner.IsEnabled)
					{
						return new KeyValuePair<WaypointRunner, int[]>(waypointRunner, _NextWaypoints[key]);
					}
				}
				waypointRunner = GetNext(p_room, key);
				if (waypointRunner != null && waypointRunner.IsEnabled)
				{
					return new KeyValuePair<WaypointRunner, int[]>(waypointRunner, _NextWaypoints[key]);
				}
			}
			return new KeyValuePair<WaypointRunner, int[]>(null, new int[2]);
		}

		private WaypointRunner GetNext(Room p_room, string p_waypointName)
		{
			if (p_room == null)
			{
				return null;
			}
			return p_room.GetWaypointByName(p_waypointName);
		}

		public override bool Render()
		{
			return true;
		}
	}
}
