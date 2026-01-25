using UnityEngine;

namespace Nekki.Vector.Core.Node
{
	public class ModelLine
	{
		private ModelNode _Start;

		private ModelNode _End;

		private Vector3fLine _LineCurrent;

		private Vector3fLine _LinePrevious;

		private string _Name;

		private Color _Color;

		private float _Length;

		private int _SubType;

		private string _Type;

		private float _Mass;

		private float _Margin1;

		private float _Margin2;

		private bool _Collisible;

		private float _Stroke;

		public ModelNode Start
		{
			get
			{
				return _Start;
			}
		}

		public ModelNode End
		{
			get
			{
				return _End;
			}
		}

		public Vector3fLine LineCurrent
		{
			get
			{
				return _LineCurrent;
			}
		}

		public Vector3fLine LinePrevious
		{
			get
			{
				return _LinePrevious;
			}
		}

		public virtual string Name
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

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
			}
		}

		public float Length
		{
			get
			{
				return _Length;
			}
			set
			{
				_Length = value;
			}
		}

		public float RealLength
		{
			get
			{
				return Vector3f.Distance(_Start.Start, _End.Start);
			}
		}

		public float ScaleLength
		{
			get
			{
				return _Length / RealLength;
			}
		}

		public int SubType
		{
			get
			{
				return _SubType;
			}
			set
			{
				_SubType = value;
			}
		}

		public string Type
		{
			get
			{
				return _Type;
			}
			set
			{
				_Type = value;
			}
		}

		public float Mass
		{
			get
			{
				return _Mass;
			}
		}

		public float Margin1
		{
			get
			{
				return _Margin1;
			}
			set
			{
				_Margin1 = value;
			}
		}

		public float Margin2
		{
			get
			{
				return _Margin2;
			}
			set
			{
				_Margin2 = value;
			}
		}

		public bool Collisible
		{
			get
			{
				return _Collisible;
			}
			set
			{
				_Collisible = value;
			}
		}

		public float Stroke
		{
			get
			{
				return _Stroke;
			}
			set
			{
				_Stroke = value;
			}
		}

		public Vector3f Center
		{
			get
			{
				return Vector3f.Middle(_Start.Start, _End.Start);
			}
		}

		public ModelLine(ModelNode p_node1, ModelNode p_node2)
		{
			_LineCurrent = new Vector3fLine(p_node1.Start, p_node2.Start);
			_LinePrevious = new Vector3fLine(p_node1.End, p_node2.End);
			_Start = p_node1;
			_End = p_node2;
			_Mass = 1f / (_Start.Weight + _End.Weight);
		}

		public ModelLine(ModelLine p_modelLine)
		{
			_Name = p_modelLine.Name;
			_Start = p_modelLine.Start;
			_End = p_modelLine.End;
			_Stroke = p_modelLine.Stroke;
			_Color = p_modelLine.Color;
			_Length = p_modelLine.Length;
			_SubType = p_modelLine.SubType;
			_Type = p_modelLine.Type;
			_Margin1 = p_modelLine.Margin1;
			_Margin2 = p_modelLine.Margin2;
		}

		public Vector3f Iterative(Vector3f p_vector)
		{
			Vector3f vector3f = (_Start.Start * _Start.Weight + _End.Start * _End.Weight) * _Mass;
			Vector3f vector3f2 = (p_vector - vector3f) * ScaleLength;
			return vector3f + vector3f2;
		}
	}
}
