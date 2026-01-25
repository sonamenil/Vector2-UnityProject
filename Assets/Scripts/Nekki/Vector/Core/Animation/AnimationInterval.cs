using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Animation.Events;
using Nekki.Vector.Core.Detector;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationInterval
	{
		private int _BeginFrame;

		private int _EndFrame;

		private bool _IsSafe;

		private bool _IsLock;

		private bool _IsAction;

		private List<AnimationEventDetector> _DetectorHEvents;

		private List<AnimationEventDetector> _DetectorVEvents;

		private List<AnimationEventEnd> _EndEvents;

		private List<AnimationEventKey> _KeyEvents;

		private List<AnimationEventCollision> _CollisionEvents;

		private List<AnimationEventArea> _AreaEvents;

		private List<AnimationEventFrame> _FrameEvents;

		private Rectangle _BoundingBoxLeft;

		private Rectangle _BoundingBoxRight;

		private Point _NoPlatformBound = new Point(0f, 0f);

		private bool _ConditionlessBoundH;

		private bool _ConditionlessBoundV;

		private bool _ConditionlessBoundC;

		public int BeginFrame
		{
			get
			{
				return _BeginFrame;
			}
		}

		public int EndFrame
		{
			get
			{
				return _EndFrame;
			}
		}

		public bool IsSafe
		{
			get
			{
				return _IsSafe;
			}
		}

		public bool IsLock
		{
			get
			{
				return _IsLock;
			}
		}

		public bool IsAction
		{
			get
			{
				return _IsAction;
			}
		}

		public List<AnimationEventDetector> DetectorHEvents
		{
			get
			{
				return _DetectorHEvents;
			}
		}

		public List<AnimationEventDetector> DetectorVEvents
		{
			get
			{
				return _DetectorVEvents;
			}
		}

		public List<AnimationEventEnd> EndEvents
		{
			get
			{
				return _EndEvents;
			}
		}

		public List<AnimationEventKey> KeyEvents
		{
			get
			{
				return _KeyEvents;
			}
		}

		public List<AnimationEventCollision> CollisionEvents
		{
			get
			{
				return _CollisionEvents;
			}
		}

		public List<AnimationEventArea> AreaEvents
		{
			get
			{
				return _AreaEvents;
			}
		}

		public List<AnimationEventFrame> FrameEvents
		{
			get
			{
				return _FrameEvents;
			}
		}

		public Rectangle BoundingBoxLeft
		{
			get
			{
				return _BoundingBoxLeft;
			}
		}

		public Rectangle BoundingBoxRight
		{
			get
			{
				return _BoundingBoxRight;
			}
		}

		public Point NoPlatformBound
		{
			get
			{
				return _NoPlatformBound;
			}
		}

		public bool ConditionlessBoundH
		{
			get
			{
				return _ConditionlessBoundH;
			}
		}

		public bool ConditionlessBoundV
		{
			get
			{
				return _ConditionlessBoundV;
			}
		}

		public bool ConditionlessBoundC
		{
			get
			{
				return _ConditionlessBoundC;
			}
		}

		public AnimationInterval(XmlNode p_node)
		{
			PreParse(p_node);
			Parse(p_node);
		}

		public AnimationInterval(XmlNode Node, List<XmlNode> Groups)
			: this(Node)
		{
			foreach (XmlNode Group in Groups)
			{
				Parse(Group);
			}
		}

		private void PreParse(XmlNode p_node)
		{
			_BeginFrame = XmlUtils.ParseInt(p_node.Attributes["Start"]);
			_EndFrame = XmlUtils.ParseInt(p_node.Attributes["End"]);
			_IsSafe = XmlUtils.ParseBool(p_node.Attributes["Safe"]);
			_IsLock = XmlUtils.ParseBool(p_node.Attributes["Lock"]);
			_IsAction = ((p_node.Attributes["Action"] != null) ? true : false);
			_NoPlatformBound.X = XmlUtils.ParseFloat(p_node.Attributes["NoPlatformBoundX"]);
			_NoPlatformBound.Y = XmlUtils.ParseFloat(p_node.Attributes["NoPlatformBoundY"]);
			_ConditionlessBoundH = XmlUtils.ParseBool(p_node.Attributes["ConditionlessPlatformBoundH"], true);
			_ConditionlessBoundV = XmlUtils.ParseBool(p_node.Attributes["ConditionlessPlatformBoundV"], true);
			_ConditionlessBoundC = XmlUtils.ParseBool(p_node.Attributes["ConditionlessPlatformBoundC"], true);
			if (p_node.Attributes["LT"] != null && p_node.Attributes["RB"] != null)
			{
				string[] array = p_node.Attributes["LT"].Value.Split('|');
				string[] array2 = p_node.Attributes["RB"].Value.Split('|');
				float[] array3 = new float[2]
				{
					float.Parse(array[0]),
					float.Parse(array[1])
				};
				float[] array4 = new float[2]
				{
					float.Parse(array2[0]),
					float.Parse(array2[1])
				};
				_BoundingBoxRight = new Rectangle(array3[0], array3[1], array4[0] - array3[0], array4[1] - array3[1]);
				_BoundingBoxLeft = new Rectangle(-1f * array4[0], array3[1], array4[0] - array3[0], array4[1] - array3[1]);
			}
			_DetectorHEvents = new List<AnimationEventDetector>();
			_DetectorVEvents = new List<AnimationEventDetector>();
			_EndEvents = new List<AnimationEventEnd>();
			_KeyEvents = new List<AnimationEventKey>();
			_CollisionEvents = new List<AnimationEventCollision>();
			_FrameEvents = new List<AnimationEventFrame>();
			_AreaEvents = new List<AnimationEventArea>();
		}

		private void Parse(XmlNode p_node)
		{
			foreach (XmlNode item in p_node)
			{
				string p_GroupNames = XmlUtils.ParseString(item.Attributes["Groups"]);
				switch (item.Name)
				{
				case "Keys":
					_KeyEvents.Add(new AnimationEventKey(GetEventParam(item, p_GroupNames), item));
					break;
				case "OnEnd":
					_EndEvents.Add(new AnimationEventEnd(GetEventParam(item, p_GroupNames)));
					break;
				case "DetectorH":
				{
					DetectorEvent.DetectorEventType p_type2 = (DetectorEvent.DetectorEventType)XmlUtils.ParseInt(item.Attributes["Type"]);
					_DetectorHEvents.Add(new AnimationEventDetector(p_type2, GetEventParam(item, p_GroupNames)));
					break;
				}
				case "DetectorV":
				{
					DetectorEvent.DetectorEventType p_type = (DetectorEvent.DetectorEventType)XmlUtils.ParseInt(item.Attributes["Type"]);
					_DetectorVEvents.Add(new AnimationEventDetector(p_type, GetEventParam(item, p_GroupNames)));
					break;
				}
				case "OnCollision":
				{
					List<AnimationEventCollision.Type> list = new List<AnimationEventCollision.Type>();
					if (item.Attributes["Types"] != null)
					{
						List<string> list2 = new List<string>(item.Attributes["Types"].Value.Split('|'));
						foreach (string item2 in list2)
						{
							list.Add((AnimationEventCollision.Type)int.Parse(item2));
						}
					}
					_CollisionEvents.Add(new AnimationEventCollision(list, GetEventParam(item)));
					break;
				}
				case "OnFrame":
					_FrameEvents.Add(new AnimationEventFrame(int.Parse(item.Attributes["Frame"].Value), GetEventParam(item)));
					break;
				case "OnArea":
					_AreaEvents.Add(new AnimationEventArea(GetEventParam(item)));
					break;
				}
			}
		}

		public AnimationEventParam GetEventParam(XmlNode p_node, string p_GroupNames = null)
		{
			List<AnimationReaction> list = new List<AnimationReaction>();
			List<AnimationSound> list2 = new List<AnimationSound>();
			XmlNode xmlNode = p_node["Reactions"];
			if (xmlNode != null)
			{
				foreach (XmlNode item in xmlNode)
				{
					list.Add(new AnimationReaction(item));
				}
			}
			if (p_GroupNames != null)
			{
				string[] array = p_GroupNames.Split('|');
				foreach (string name in array)
				{
					AnimationGroup group = AnimationGroup.GetGroup(name);
					list.AddRange(group.Reactions);
				}
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name == "Sound")
				{
					list2.Add(new AnimationSound(childNode.Attributes["Name"].Value, int.Parse(childNode.Attributes["Type"].Value)));
				}
			}
			if (list2.Count == 0)
			{
				list2 = null;
			}
			if (list.Count == 0)
			{
				list = null;
			}
			return new AnimationEventParam(list, list2);
		}

		public static string TypeToString(AnimationEventType p_type)
		{
			switch (p_type)
			{
			case AnimationEventType.OnEnd:
				return "ON_END";
			case AnimationEventType.OnFrame:
				return "ON_FRAME";
			case AnimationEventType.OnArea:
				return "ON_AREA";
			case AnimationEventType.DetectorOn:
				return "DETECTOR_ON";
			case AnimationEventType.DetectorOff:
				return "DETECTOR_OFF";
			case AnimationEventType.Controller:
				return "CONTROLLER";
			case AnimationEventType.OnCollision:
				return "COLISION";
			default:
				return string.Empty;
			}
		}
	}
}
