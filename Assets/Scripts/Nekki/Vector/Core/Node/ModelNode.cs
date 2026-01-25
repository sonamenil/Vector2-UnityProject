using Nekki.Vector.Core.Runners;
using UnityEngine;

namespace Nekki.Vector.Core.Node
{
	public class ModelNode : Vector3fLine
	{
		private string _Name;

		private int _Id;

		private bool _IsFixed;

		private int _Radius;

		private bool _IsPhysics;

		private float _Weight;

		private float _Attenuation;

		private string _Type;

		private MacroNode _MacroNode;

		private bool _IsCollisible;

		private bool _IsDetector;

		private int _BothIndex;

		private string _BothName;

		private QuadRunner _Data;

		private Vector3f _DefaultPosition;

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
				string[] array = _Name.Split('_');
				_BothName = _Name.Split('_')[0];
				if (array.Length == 2)
				{
					_BothIndex = int.Parse(array[1]);
				}
				else
				{
					_BothIndex = 0;
				}
			}
		}

		public int Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		public bool IsFixed
		{
			get
			{
				return _IsFixed;
			}
			set
			{
				_IsFixed = value;
			}
		}

		public bool IsNodeFixed
		{
			get
			{
				return !_IsFixed && IsType;
			}
		}

		public int Radius
		{
			get
			{
				return _Radius;
			}
			set
			{
				_Radius = value;
			}
		}

		public bool IsPhysics
		{
			get
			{
				return _IsPhysics;
			}
			set
			{
				_IsPhysics = value;
			}
		}

		public bool IsNodePhysics
		{
			get
			{
				return _IsPhysics && IsType;
			}
		}

		public float Weight
		{
			get
			{
				return _Weight;
			}
			set
			{
				_Weight = value;
			}
		}

		public float Attenuation
		{
			get
			{
				return _Attenuation;
			}
			set
			{
				_Attenuation = value;
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

		public bool IsType
		{
			get
			{
				return _Type == "Node";
			}
		}

		public MacroNode MacroNode
		{
			get
			{
				return _MacroNode;
			}
			set
			{
				_MacroNode = value;
			}
		}

		public bool IsCollisible
		{
			get
			{
				return _IsCollisible;
			}
			set
			{
				_IsCollisible = value;
			}
		}

		public bool IsDetector
		{
			get
			{
				return _IsDetector;
			}
			set
			{
				_IsDetector = value;
			}
		}

		public int BothIndex
		{
			get
			{
				return _BothIndex;
			}
		}

		public string BothName
		{
			get
			{
				return _BothName;
			}
		}

		public QuadRunner Data
		{
			get
			{
				return _Data;
			}
			set
			{
				_Data = value;
			}
		}

		public ModelNode(Vector3f p_position, MacroNode p_macroNode = null)
			: base(p_position, p_position)
		{
			_DefaultPosition = new Vector3f(p_position);
			_MacroNode = p_macroNode;
			_Data = null;
		}

		public void Reset()
		{
			_Start.Set(_DefaultPosition);
			_End.Set(_DefaultPosition);
			_Data = null;
		}

		public void Position(Vector3f p_end, Vector3f p_start)
		{
			PositionEnd(p_end.X, p_end.Y, p_end.Z);
			PositionStart(p_start.X, p_start.Y, p_start.Z);
		}

		public void PositionStart(Vector3f p_vector)
		{
			_Start.X = p_vector.X;
			_Start.Y = p_vector.Y;
			_Start.Z = p_vector.Z;
		}

		public void PositionStart(Vector3 p_vector)
		{
			_Start.X = p_vector.x;
			_Start.Y = p_vector.y;
			_Start.Z = p_vector.z;
		}

		public void PositionStart(float p_x, float p_y, float p_z)
		{
			_Start.X = p_x;
			_Start.Y = p_y;
			_Start.Z = p_z;
		}

		public void PositionEnd(Vector3f p_vector)
		{
			_End.X = p_vector.X;
			_End.Y = p_vector.Y;
			_End.Z = p_vector.Z;
		}

		public void PositionEnd(float p_x, float p_y, float p_z)
		{
			_End.X = p_x;
			_End.Y = p_y;
			_End.Z = p_z;
		}

		public void EndAssignStart()
		{
			_End.X = _Start.X;
			_End.Y = _Start.Y;
			_End.Z = _Start.Z;
		}

		public void MacroNodeCompute()
		{
			if (_MacroNode != null)
			{
				_Start.Reset();
				_End.Reset();
				for (int i = 0; i < MacroNode.ChildNode.Count; i++)
				{
					_Start.Add(_MacroNode.ChildNode[i].Start, _MacroNode.LCC[i]);
                    _End.Add(_MacroNode.ChildNode[i].End, _MacroNode.LCC[i]);
                }
            }
		}

		public void TimeStep(float p_gravity)
		{
			Vector3 p_vector = new Vector3(_Start.X - _End.X, _Start.Y - _End.Y, _Start.Z - _End.Z);
			if (_IsPhysics)
			{
				p_vector *= 1f - _Attenuation;
			}
			p_vector.x += _Start.X;
			p_vector.y += _Start.Y;
			p_vector.z += _Start.Z;
			p_vector.y += p_gravity;
			EndAssignStart();
			PositionStart(p_vector);
		}
	}
}
