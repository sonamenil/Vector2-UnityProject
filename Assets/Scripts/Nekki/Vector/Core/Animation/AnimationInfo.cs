using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Frame;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationInfo
	{
		public const int Jumps = 1;

		public const int Slides = 2;

		protected bool _IsTrick;

		private bool _IsPart;

		private string _Name;

		private bool _Mirror;

		private int _Type;

		private string _FileName;

		private Provider _Frames;

		private int _FirstFrame;

		private int _EndFrame;

		private int _MidFrames;

		private string _PivotNode;

		private int _Priority;

		private Vector3f _Velocity = new Vector3f(0f, 0f, 0f);

		private Vector3f _Gravity = new Vector3f(0f, 0f, 0f);

		private bool _Binding;

		private float _DeltaDetectorV;

		private float _DeltaDetectorH;

		private float _AutoPositionDetectorV;

		private float _AutoPositionDetectorH;

		private float _LandingPositionDetectorH;

		private float _LandingPositionDetectorV;

		private int _PlatformAnticipationFrames;

		public List<AnimationInterval> Intervals = new List<AnimationInterval>();

		private static List<AnimationInterval> _TempIntervalList = new List<AnimationInterval>();

		public bool IsTrick
		{
			get
			{
				return _IsTrick;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public bool Mirror
		{
			get
			{
				return _Mirror;
			}
		}

		public int Type
		{
			get
			{
				return _Type;
			}
		}

		public string FileName
		{
			get
			{
				return _FileName;
			}
		}

		public int FirstFrame
		{
			get
			{
				return _FirstFrame;
			}
		}

		public int EndFrame
		{
			get
			{
				if (_EndFrame < _Frames.Length)
				{
					return _EndFrame;
				}
				return _Frames.Length;
			}
		}

		public int MidFrames
		{
			get
			{
				return _MidFrames;
			}
		}

		public string PivotNode
		{
			get
			{
				return _PivotNode;
			}
		}

		public int Priority
		{
			get
			{
				return _Priority;
			}
		}

		public Vector3f Velocity
		{
			get
			{
				return _Velocity;
			}
		}

		public Vector3f Gravity
		{
			get
			{
				return _Gravity;
			}
		}

		public bool Binding
		{
			get
			{
				return _Binding;
			}
		}

		public float DeltaDetectorV
		{
			get
			{
				return _DeltaDetectorV;
			}
		}

		public float DeltaDetectorH
		{
			get
			{
				return _DeltaDetectorH;
			}
		}

		public float AutoPositionDetectorV
		{
			get
			{
				return _AutoPositionDetectorV;
			}
		}

		public float AutoPositionDetectorH
		{
			get
			{
				return _AutoPositionDetectorH;
			}
		}

		public float LandingPositionDetectorH
		{
			get
			{
				return _LandingPositionDetectorH;
			}
		}

		public float LandingPositionDetectorV
		{
			get
			{
				return _LandingPositionDetectorV;
			}
		}

		public int PlatformAnticipationFrames
		{
			get
			{
				return _PlatformAnticipationFrames;
			}
		}

		public AnimationInfo(XmlNode p_node)
		{
			_IsPart = XmlUtils.ParseBool(p_node.Attributes["Part"]);
			_Name = p_node.Name;
			_FileName = XmlUtils.ParseString(p_node.Attributes["FileName"]);
			_FirstFrame = XmlUtils.ParseInt(p_node.Attributes["FirstFrame"]);
			_MidFrames = XmlUtils.ParseInt(p_node.Attributes["MidFrames"], 2);
			_EndFrame = XmlUtils.ParseInt(p_node.Attributes["EndFrame"]);
			_PivotNode = p_node.Attributes["PivotNode"].Value;
			_Type = XmlUtils.ParseInt(p_node.Attributes["Type"], 1);
			_Priority = XmlUtils.ParseInt(p_node.Attributes["Priority"], 1);
			_Binding = XmlUtils.ParseBool(p_node.Attributes["Binding"]);
			_DeltaDetectorH = XmlUtils.ParseFloat(p_node.Attributes["DeltaDetectorH"]);
			_DeltaDetectorV = XmlUtils.ParseFloat(p_node.Attributes["DeltaDetectorV"]);
			_Mirror = XmlUtils.ParseBool(p_node.Attributes["Mirror"], true);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["VelocityX"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["VelocityY"]);
			_Velocity = new Vector3f(p_x, p_y, 0f);
			_Gravity = new Vector3f(0f, XmlUtils.ParseFloat(p_node.Attributes["Gravity"]), 0f);
			_AutoPositionDetectorH = XmlUtils.ParseFloat(p_node.Attributes["AutoPositionDetectorH"], -1f);
			_AutoPositionDetectorV = XmlUtils.ParseFloat(p_node.Attributes["AutoPositionDetectorV"], -1f);
			_LandingPositionDetectorH = XmlUtils.ParseFloat(p_node.Attributes["LandingPositionDetectorH"], -1f);
			_LandingPositionDetectorV = XmlUtils.ParseFloat(p_node.Attributes["LandingPositionDetectorV"], -1f);
			_PlatformAnticipationFrames = (int)XmlUtils.ParseFloat(p_node.Attributes["PlatformAnticipationFrames"]);
		}

		public void Load(string p_path)
		{
			_FileName = p_path + "/" + _FileName;
			if (!_IsPart && !_IsTrick)
			{
				LoadBinary(true);
			}
		}

		public virtual void LoadBinary(bool p_useCache)
		{
			_Frames = AnimationBinaryParser.ParseFile(_FileName, p_useCache);
		}

		public virtual void UnloadBinary()
		{
			_Frames = null;
		}

		public List<AnimationInterval> Interval(int p_frame)
		{
			_TempIntervalList.Clear();
			for (int i = 0; i < Intervals.Count; i++)
			{
				AnimationInterval animationInterval = Intervals[i];
				if (p_frame >= animationInterval.BeginFrame && p_frame <= animationInterval.EndFrame)
				{
					_TempIntervalList.Add(animationInterval);
				}
			}
			return _TempIntervalList;
		}

		public void CloneFrames(int p_start, int p_end, KeyFrames p_frames)
		{
			int p_to = ((p_end <= 0 || p_end > _Frames.Length - 1) ? (_Frames.Length - 1) : p_end);
			p_frames.SetFrames(p_start, p_to, _Frames.Data);
		}
	}
}
