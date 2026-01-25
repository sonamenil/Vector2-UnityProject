using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;
using UnityEngine;

namespace Nekki.Vector.Core.Detector
{
	public class DetectorLine
	{
		public enum DetectorType
		{
			Vertical = 0,
			Horizontal = 1
		}

		private GameObject _Layer;

		private ModelNode _Node;

		private DetectorType _Type;

		private Vector3f _Delta = new Vector3f(0f, 0f, 0f);

		private bool _Safe;

		private int _Side;

		private Vector3fLine _End;

		private Vector3fLine _Start;

		private Vector3fLine _Perpendicular;

		public GameObject Layer
		{
			get
			{
				return _Layer;
			}
			set
			{
				_Layer = value;
			}
		}

		public ModelNode Node
		{
			get
			{
				return _Node;
			}
		}

		public QuadRunner Platform
		{
			get
			{
				return _Node.Data;
			}
			set
			{
				_Node.Data = value;
			}
		}

		public string Name
		{
			get
			{
				return _Node.Name;
			}
		}

		public Vector3f Position
		{
			get
			{
				return _Node.Start;
			}
		}

		public DetectorType Type
		{
			get
			{
				return _Type;
			}
		}

		public Vector3f DeltaValue
		{
			get
			{
				return _Delta;
			}
		}

		public bool Safe
		{
			get
			{
				return _Safe;
			}
			set
			{
				_Safe = value;
			}
		}

		public int Side
		{
			get
			{
				return _Side;
			}
			set
			{
				_Side = value;
			}
		}

		public Vector3fLine End
		{
			get
			{
				return _End;
			}
		}

		public Vector3fLine Start
		{
			get
			{
				return _Start;
			}
		}

		public Vector3fLine Perpendicular
		{
			get
			{
				return _Perpendicular;
			}
		}

		public DetectorLine(ModelNode p_node, DetectorType p_type)
		{
			_Node = p_node;
			_Start = new Vector3fLine(p_node.Start, p_node.Start);
			_End = new Vector3fLine(p_node.Start, p_node.Start);
			_Perpendicular = new Vector3fLine(p_node.Start, p_node.Start);
			_Type = p_type;
		}

		public void Reset()
		{
			_Start.Set(_Node.Start, _Node.Start);
			_End.Set(_Node.Start, _Node.Start);
			_Perpendicular.Set(_Node.Start, _Node.Start);
		}

		public double Normal()
		{
			return (_Node.Start - _Node.End) * new Vector3f(0f, 0f, 1f);
		}

		public Vector3f Subtract()
		{
			return _Node.Start - _Node.End;
		}

		public Vector3f Delta()
		{
			switch (_Type)
			{
			case DetectorType.Horizontal:
				return new Vector3f(_Delta.X, 0f, 0f);
			case DetectorType.Vertical:
				return new Vector3f(0f, _Delta.Y, 0f);
			default:
				return new Vector3f(0f, 0f, 0f);
			}
		}

		public void Delta(float p_value)
		{
			switch (_Type)
			{
			case DetectorType.Horizontal:
				_Delta.X = p_value;
				break;
			case DetectorType.Vertical:
				_Delta.Y = p_value;
				break;
			}
		}

		public void Update()
		{
			_End.Set(_Start.Start, _Start.End);
			_Start.Set(_Node.Start, _Node.Start);
			_Start.SetZerroOnZ();
			_Start.Start.Subtract(_Delta);
			_Start.End.Add(_Delta);
			_Perpendicular.Start = Vector3f.Closest(_Start.Start, _End.Start, _End.End - _End.Start);
			_Perpendicular.End = Vector3f.Closest(_Start.End, _End.End, _End.Start - _End.End);
		}

		public void DeltaPosition(Vector3f p_delta)
		{
			_Node.Start.Add(p_delta);
			_Node.EndAssignStart();
		}

		public string GetStringType()
		{
			return (Type != 0) ? "H" : "V";
		}
	}
}
