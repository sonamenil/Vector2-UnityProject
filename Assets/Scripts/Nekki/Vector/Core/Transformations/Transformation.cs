using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class Transformation
	{
		private List<PrototypeInterval> _Intervals = new List<PrototypeInterval>();

		private int _CurrentInterval;

		private int _Frames;

		private TransformInterface _Parent;

		private string _Name;

		private bool _NotChangePosition;

		public int Frames
		{
			get
			{
				return _Frames;
			}
		}

		public TransformInterface Parent
		{
			get
			{
				return _Parent;
			}
			set
			{
				_Parent = value;
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

		public bool IsChangePosition
		{
			get
			{
				return !_NotChangePosition;
			}
		}

		private Transformation()
		{
			_CurrentInterval = 0;
			_Frames = 0;
			_NotChangePosition = true;
		}

		public static Transformation Create(XmlNode p_node)
		{
			if (p_node == null || p_node.Name != "Transformation")
			{
				return null;
			}
			Transformation transformation = new Transformation();
			transformation.Parse(p_node);
			if (transformation._Intervals.Count == 0)
			{
				transformation = null;
				return null;
			}
			return transformation;
		}

		private void Parse(XmlNode p_node)
		{
			_Name = p_node.Attributes["Name"].Value;
			PrototypeInterval prototypeInterval = null;
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				prototypeInterval = PrototypeInterval.Create(childNode);
				if (prototypeInterval != null)
				{
					_Intervals.Add(prototypeInterval);
					_Frames += prototypeInterval.Frames;
					if (_NotChangePosition && prototypeInterval.IsChangePosition)
					{
						_NotChangePosition = false;
					}
				}
			}
		}

		public bool Iteration()
		{
			if (!_Intervals[_CurrentInterval].Iteration(_Parent))
			{
				_CurrentInterval++;
			}
			if (_CurrentInterval >= _Intervals.Count)
			{
				return false;
			}
			return true;
		}

		public int Run()
		{
			TransformationManager current = TransformationManager.Current;
			current.Add(this);
			return _Frames;
		}

		public void Reset()
		{
			_CurrentInterval = 0;
			foreach (PrototypeInterval interval in _Intervals)
			{
				interval.Reset();
			}
		}
	}
}
