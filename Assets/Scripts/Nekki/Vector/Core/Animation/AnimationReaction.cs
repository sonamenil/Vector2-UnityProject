using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Detector;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationReaction
	{
		private static List<AnimationReaction> _Reactions = new List<AnimationReaction>();

		private string _Name;

		private int _FirstFrame;

		private int _Priority;

		private List<int> _Sides = new List<int>();

		private AnimationDeltas _DeltasH;

		private AnimationDeltas _DeltasV;

		private AnimationDeltas _DeltasC;

		private int _CornerClingV = -1;

		private int _CornerClingH = -1;

		private string _PivotNode;

		private bool _SafeHorizontal;

		private bool _SafeVertical;

		private bool _Mirror;

		private bool _Reverse;

		private float _InsideH;

		private float _InsideV;

		private List<int> _SlopeH = new List<int>();

		private List<int> _SlopeV = new List<int>();

		private string _AreaName;

		private int _AreaNameHash;

		private List<string> _NodesWI = new List<string>();

		private bool _OnEndTrigger;

		private AnimationInfo _Info;

		public static string NameDeath = "Death";

		public static List<AnimationReaction> Reactionss
		{
			get
			{
				return _Reactions;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public int FirstFrame
		{
			get
			{
				return _FirstFrame;
			}
			set
			{
				_FirstFrame = value;
			}
		}

		public int Priority
		{
			get
			{
				return _Priority;
			}
			set
			{
				_Priority = value;
			}
		}

		public List<int> Sides
		{
			set
			{
				_Sides = value;
			}
		}

		public AnimationDeltas DeltasH
		{
			get
			{
				return _DeltasH;
			}
		}

		public AnimationDeltas DeltasV
		{
			get
			{
				return _DeltasV;
			}
		}

		public AnimationDeltas DeltasC
		{
			get
			{
				return _DeltasC;
			}
		}

		public int CornerClingV
		{
			get
			{
				return _CornerClingV;
			}
			set
			{
				_CornerClingV = value;
			}
		}

		public int CornerClingH
		{
			get
			{
				return _CornerClingH;
			}
			set
			{
				_CornerClingH = value;
			}
		}

		public string PivotNode
		{
			get
			{
				return _PivotNode;
			}
			set
			{
				_PivotNode = value;
			}
		}

		public bool SafeHorizontal
		{
			get
			{
				return _SafeHorizontal;
			}
			set
			{
				_SafeHorizontal = value;
			}
		}

		public bool SafeVertical
		{
			get
			{
				return _SafeVertical;
			}
			set
			{
				_SafeVertical = value;
			}
		}

		public bool Mirror
		{
			get
			{
				return _Mirror;
			}
		}

		public bool Reverse
		{
			get
			{
				return _Reverse;
			}
			set
			{
				_Reverse = value;
			}
		}

		public float InsideH
		{
			get
			{
				return _InsideH;
			}
			set
			{
				_InsideH = value;
			}
		}

		public float InsideV
		{
			get
			{
				return _InsideV;
			}
			set
			{
				_InsideV = value;
			}
		}

		public string AreaName
		{
			get
			{
				return _AreaName;
			}
		}

		public List<string> NodesWI
		{
			get
			{
				return _NodesWI;
			}
			set
			{
				_NodesWI = value;
			}
		}

		public bool OnEndTrigger
		{
			get
			{
				return _OnEndTrigger;
			}
		}

		public bool IsAnimationArrest
		{
			get
			{
				if (Animations.Animation.ContainsKey(_Name) && Animations.Animation[_Name].Type == 5)
				{
					return true;
				}
				return false;
			}
		}

		public AnimationInfo Info
		{
			get
			{
				if (_Info == null)
				{
					_Info = Animations.Animation[_Name];
				}
				return _Info;
			}
		}

		public AnimationReaction()
		{
		}

		public AnimationReaction(XmlNode p_node)
		{
			if (p_node == null || p_node.Attributes == null)
			{
				throw new Exception();
			}
			_Name = p_node.Name;
			_FirstFrame = XmlUtils.ParseInt(p_node.Attributes["FirstFrame"]);
			_Priority = XmlUtils.ParseInt(p_node.Attributes["Priority"]);
			_CornerClingV = XmlUtils.ParseInt(p_node.Attributes["CornerClingV"], -1);
			_CornerClingH = XmlUtils.ParseInt(p_node.Attributes["CornerClingH"], -1);
			_SafeHorizontal = XmlUtils.ParseBool(p_node.Attributes["SafeH"]);
			_SafeVertical = XmlUtils.ParseBool(p_node.Attributes["SafeV"]);
			_Reverse = XmlUtils.ParseBool(p_node.Attributes["Reverse"]);
			_Mirror = XmlUtils.ParseBool(p_node.Attributes["Mirror"]);
			_OnEndTrigger = XmlUtils.ParseBool(p_node.Attributes["OnEndTrigger"]);
			_PivotNode = XmlUtils.ParseString(p_node.Attributes["PivotNode"]);
			if (p_node.Attributes["AreaName"] != null)
			{
				_AreaName = p_node.Attributes["AreaName"].Value;
				_AreaNameHash = _AreaName.GetHashCode();
			}
			if (p_node.Attributes["Sides"] != null)
			{
				string[] array = p_node.Attributes["Sides"].Value.Split('|');
				foreach (string s in array)
				{
					_Sides.Add(int.Parse(s));
				}
			}
			if (p_node.Attributes["SlopeH"] != null)
			{
				string[] array2 = p_node.Attributes["SlopeH"].Value.Split('|');
				foreach (string s2 in array2)
				{
					_SlopeH.Add(int.Parse(s2));
				}
			}
			if (p_node.Attributes["SlopeV"] != null)
			{
				string[] array3 = p_node.Attributes["SlopeV"].Value.Split('|');
				foreach (string s3 in array3)
				{
					_SlopeV.Add(int.Parse(s3));
				}
			}
			if (p_node.Attributes["NodesWI"] != null)
			{
				_NodesWI = new List<string>(p_node.Attributes["NodesWI"].Value.Split('|'));
			}
			_InsideH = XmlUtils.ParseFloat(p_node.Attributes["InsideH"], float.NaN);
			_InsideV = XmlUtils.ParseFloat(p_node.Attributes["InsideV"], float.NaN);
			_DeltasH = new AnimationDeltas(p_node, AnimationDeltaType.Horizontal);
			_DeltasV = new AnimationDeltas(p_node, AnimationDeltaType.Vertical);
			_DeltasC = new AnimationDeltas(p_node, AnimationDeltaType.Collision);
			_Reactions.Add(this);
			_Info = null;
		}

		private AnimationReaction IndexOf(int p_index)
		{
			return (p_index >= _Reactions.Count) ? null : _Reactions[p_index];
		}

		public bool CheckNameHash(int p_hash)
		{
			return _AreaNameHash == p_hash;
		}

		public bool IsSide(int p_value, int p_sign)
		{
			if (_Sides.Count == 0 || p_value == -1)
			{
				return true;
			}
			for (int i = 0; i < _Sides.Count; i++)
			{
				int num = _Sides[i];
				if (p_sign == -1)
				{
					switch (num)
					{
					case 1:
						num = 3;
						break;
					case 3:
						num = 1;
						break;
					}
				}
				if (num == p_value)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSlope(AnimationDeltaData p_deltaH, AnimationDeltaData p_deltaV, int p_sign)
		{
			return IsDeltaSlope(p_deltaH, _SlopeH, p_sign) && IsDeltaSlope(p_deltaV, _SlopeV, p_sign);
		}

		public bool IsDeltaSlope(AnimationDeltaData p_delta, List<int> p_slopes, int p_sign)
		{
			if (p_delta == null || p_slopes == null || p_slopes.Count == 0)
			{
				return true;
			}
			int type = p_delta.Platform.Type;
			foreach (int p_slope in p_slopes)
			{
				int num;
				if (p_sign == 1)
				{
					num = p_slope;
				}
				else
				{
					switch (p_slope)
					{
					case 1:
						num = 2;
						break;
					case 2:
						num = 1;
						break;
					default:
						num = p_slope;
						break;
					}
				}
				if (num == type)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInside(DetectorLine p_detectorH, DetectorLine p_detectorV)
		{
			return Inside(_InsideH, p_detectorH) && Inside(_InsideV, p_detectorV);
		}

		public bool Inside(float p_inside, DetectorLine p_detector)
		{
			if (float.IsNaN(p_inside))
			{
				return true;
			}
			if (p_detector.Platform == null)
			{
				return p_inside == 0f;
			}
			return p_inside == 1f;
		}

		public bool IsDeltaCheck(AnimationDeltaData p_deltaH, AnimationDeltaData p_deltaV, AnimationDeltaData p_deltaС, int p_sign, Vector3f p_velocity)
		{
			bool flag = _DeltasH.IsCheck(p_deltaH, p_sign, p_velocity);
			bool flag2 = _DeltasV.IsCheck(p_deltaV, p_sign, p_velocity);
			bool flag3 = _DeltasC.IsCheck(p_deltaС, p_sign, p_velocity);
			return flag && flag2 && flag3;
		}
	}
}
