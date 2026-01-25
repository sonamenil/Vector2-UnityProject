using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Detector;
using Nekki.Vector.Core.Result;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class QuadRunner : Runner
	{
		public delegate void OnTransformationDelegate(QuadRunner Quad);

		private const float _Epsilon = 0.01f;

		protected int _NameHash;

		protected float _XQuad;

		protected float _YQuad;

		protected float _WidthQuad;

		protected float _HeightQuad;

		private bool _Sticky;

		private int _Type;

		public bool IsRender;

		private Vector3 _LastPosition;

		private Vector3f _SpeedRunner = new Vector3f(0f, 0f, 0f);

		private List<Edge> _Edges = new List<Edge>();

		protected Vector3f _Point1;

		protected Vector3f _Point2;

		protected Vector3f _Point3;

		protected Vector3f _Point4;

		protected List<Vector3f> _FuturePoints = new List<Vector3f>();

		protected List<Vector3f> _TempPoints = new List<Vector3f>();

		public Rectangle Rectangle = new Rectangle(0f, 0f, 0f, 0f);

		public QuadController Controller;

		public override bool IsEnabled
		{
			get
			{
				return base.IsEnabled;
			}
		}

		public int NameHash
		{
			get
			{
				return _NameHash;
			}
		}

		public float XQuad
		{
			get
			{
				return _XQuad;
			}
		}

		public float YQuad
		{
			get
			{
				return _YQuad;
			}
		}

		public float WidthQuad
		{
			get
			{
				return _WidthQuad;
			}
		}

		public float HeightQuad
		{
			get
			{
				return _HeightQuad;
			}
		}

		public bool Sticky
		{
			get
			{
				return _Sticky;
			}
			set
			{
				_Sticky = value;
			}
		}

		public int Type
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

		public virtual bool IsCollisible
		{
			get
			{
				return true;
			}
		}

		public Vector3f SpeedRunner
		{
			get
			{
				return _SpeedRunner;
			}
		}

		public Vector3f Point1
		{
			get
			{
				return _Point1;
			}
		}

		public Vector3f Point2
		{
			get
			{
				return _Point2;
			}
		}

		public Vector3f Point3
		{
			get
			{
				return _Point3;
			}
		}

		public Vector3f Point4
		{
			get
			{
				return _Point4;
			}
		}

		public event OnTransformationDelegate OnTransformationStart;

		public event OnTransformationDelegate OnTransformationEnd;

		public QuadRunner(float p_x, float p_y, float p_width, float p_height, bool p_Sticky, Element p_elements, string Name = "")
			: base(p_x, p_y, p_elements)
		{
			Type = 0;
			_XQuad = p_x;
			_YQuad = p_y;
			_WidthQuad = p_width;
			_HeightQuad = p_height;
			CreatePoints();
			base.Name = Name;
			_Sticky = p_Sticky;
			if (!string.IsNullOrEmpty(Name))
			{
				_NameHash = Name.GetHashCode();
			}
		}

		private void CreatePoints()
		{
			_Point1 = new Vector3f(_XQuad, _YQuad, 0f);
			_Point2 = new Vector3f(_XQuad + _WidthQuad, _YQuad, 0f);
			_Point3 = new Vector3f(_XQuad + _WidthQuad, _YQuad + _HeightQuad, 0f);
			_Point4 = new Vector3f(_XQuad, _YQuad + _HeightQuad, 0f);
		}

		public override bool Render()
		{
			return true;
		}

		public override void InitRunner()
		{
			base.InitRunner();
			SetProperties();
			_LastPosition = _CachedTransform.position;
		}

		public override void Move(Vector3f Point)
		{
			base.Move(Point);
			UpdatePosition();
		}

		public override void UpdatePosition()
		{
			if (!(_LastPosition == _CachedTransform.position))
			{
				_SpeedRunner.Set(_CachedTransform.position - _LastPosition);
				_LastPosition = _CachedTransform.position;
				SetProperties();
			}
		}

        public override void SetPosition(float x, float y)
        {
            SetProperties();
        }

		public override void TransformResetTween()
		{
			_SpeedRunner.Reset();
			_LastPosition = _CachedTransform.position;
		}

		public Vector3f GetCornerByIndex(int p_index)
		{
			switch (p_index)
			{
			case 0:
				return _Point1;
			case 1:
				return _Point2;
			case 2:
				return _Point3;
			case 3:
				return _Point4;
			default:
				return null;
			}
		}

		public virtual Point GetSize(int p_sign)
		{
			return new Point(_WidthQuad, _HeightQuad);
		}

        public virtual Point GetSize()
        {
            return new Point(_WidthQuad, _HeightQuad);
        }

        public Vector3fLine Friction(Vector3f p_end, Vector3f p_start)
		{
			if (!Hit(p_start))
			{
				return null;
			}
			Vector3f vector3f = new Vector3f(p_start);
			Vector3f vector3f2 = new Vector3f(p_end);
			vector3f.Z = 0f;
			vector3f2.Z = 0f;
			List<Cross> list = CrossByEdge(p_end, p_start);
			int p_side = NearestEdge(vector3f2);
			Vector3f vector3f3 = ((list.Count != 0) ? list[0].Point : Closest(vector3f2, p_side));
			Vector3f vector3f4 = Closest(vector3f, p_side);
			float num = Vector3f.Distance(vector3f4, vector3f3);
			float num2 = Vector3f.Distance(vector3f4, vector3f);
			float num3 = num2 * 0.2f;
			return (!(num3 < num)) ? new Vector3fLine(vector3f3, vector3f3) : new Vector3fLine(vector3f4 + (vector3f3 - vector3f4).Normalize().Multiply(num3), vector3f3);
		}

		public int NearestEdge(Vector3f p_vector)
		{
			float num = 0f;
			int result = 0;
			for (int i = 0; i < _Edges.Count; i++)
			{
				float num2 = Vector3f.Distance(Closest(p_vector, i), p_vector);
				if (i == 0 || num > num2)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		public static bool IsCrossIndex(List<Cross> p_crossList1, List<Cross> p_crossList2)
		{
			foreach (Cross item in p_crossList1)
			{
				foreach (Cross item2 in p_crossList2)
				{
					if (item.Index == item2.Index)
					{
						return true;
					}
				}
			}
			return false;
		}

		public Affiliation Affiliation(DetectorLine p_detector)
		{
			Vector3fLine start = p_detector.Start;
			Vector3fLine end = p_detector.End;
			end.Start.Add(_SpeedRunner);
			end.End.Add(_SpeedRunner);
			List<Cross> list = CrossByEdge(start.End, end.End);
			List<Cross> list2 = CrossByEdge(start.Start, end.Start);
			List<Cross> list3 = CrossByEdge(start.Start, start.End);
			bool flag = Hit(start.Start, true);
			bool flag2 = Hit(start.End, true);
			int num = 0;
			if (list.Count > 0 && list2.Count > 0)
			{
				num = 1;
			}
			else if (list.Count > 0)
			{
				num = 2;
			}
			else if (list2.Count > 0)
			{
				num = 3;
			}
			else if (list3.Count > 0)
			{
				num = 4;
			}
			else if (flag)
			{
				num = 5;
			}
			else if (flag2)
			{
				num = 6;
			}
			object result;
			if (num > 0)
			{
				Affiliation affiliation = new Affiliation();
				affiliation.CrossList1 = list;
				affiliation.CrossList2 = list2;
				affiliation.CrossList3 = list3;
				affiliation.Type = num;
				affiliation.Hits = flag || flag2;
				result = affiliation;
			}
			else
			{
				result = null;
			}
			return (Affiliation)result;
		}

		public static Cross MinCross(List<Cross> p_crossList, Vector3f p_point)
		{
			double num = double.MaxValue;
			Cross result = null;
			foreach (Cross p_cross in p_crossList)
			{
				double num2 = Vector3f.Distance(p_cross.Point, p_point);
				if (!(num2 > num))
				{
					num = num2;
					result = p_cross;
				}
			}
			return result;
		}

		public static int Side(DetectorLine p_detector, int p_sign)
		{
			switch (p_detector.Type)
			{
			case DetectorLine.DetectorType.Vertical:
				return (p_sign != 1) ? 1 : 3;
			case DetectorLine.DetectorType.Horizontal:
				return 0;
			default:
				return -1;
			}
		}

		public static int SideForType5Or6(DetectorLine p_detector, Rectangle p_rect, int p_sign, int p_type)
		{
			switch (p_detector.Type)
			{
			case DetectorLine.DetectorType.Vertical:
			{
				Vector3f vector3f = null;
				vector3f = ((p_type != 5) ? p_detector.Start.End : p_detector.Start.Start);
				if (vector3f.X >= p_rect.MinX - 0.01f && vector3f.X <= p_rect.MinX + 0.01f)
				{
					return 3;
				}
				if (vector3f.X >= p_rect.MaxX - 0.01f && vector3f.X <= p_rect.MaxX + 0.01f)
				{
					return 1;
				}
				if (vector3f.Y >= p_rect.MinY - 0.01f && vector3f.Y <= p_rect.MinY + 0.01f)
				{
					return 0;
				}
				if (vector3f.Y >= p_rect.MaxY - 0.01f && vector3f.Y <= p_rect.MaxY + 0.01f)
				{
					return 2;
				}
				return (p_sign != 1) ? 1 : 3;
			}
			case DetectorLine.DetectorType.Horizontal:
				return 0;
			default:
				return -1;
			}
		}

		public int Side(DetectorLine p_detector, Affiliation p_affiliationResult, int p_sign)
		{
			Vector3fLine start = p_detector.Start;
			Vector3fLine end = p_detector.End;
			Vector3fLine perpendicular = p_detector.Perpendicular;
			List<Cross> list = null;
			Cross cross = null;
			switch (p_affiliationResult.Type)
			{
			case 1:
				return (!IsCrossIndex(p_affiliationResult.CrossList1, p_affiliationResult.CrossList2)) ? Side(p_detector, p_sign) : MinCross(p_affiliationResult.CrossList1, end.Start).Index;
			case 2:
				list = CrossByEdge(start.End, perpendicular.End);
				cross = ((list.Count != 0) ? MinCross(list, end.End) : MinCross(p_affiliationResult.CrossList1, end.End));
				return cross.Index;
			case 3:
				list = CrossByEdge(start.Start, perpendicular.Start);
				cross = ((list.Count != 0) ? MinCross(list, end.Start) : MinCross(p_affiliationResult.CrossList2, end.Start));
				return cross.Index;
			case 4:
				return Side(p_detector, p_sign);
			case 5:
				return SideForType5Or6(p_detector, Rectangle, p_sign, 5);
			case 6:
				return SideForType5Or6(p_detector, Rectangle, p_sign, 6);
			default:
				return -1;
			}
		}

		public List<Cross> CrossByEdge(Vector3f p_point1, Vector3f p_point2)
		{
			List<Cross> list = new List<Cross>();
			for (int i = 0; i < _Edges.Count; i++)
			{
				Vector3f vector3f = Vector3f.Cross(p_point1, p_point2, _Edges[i].Point1, _Edges[i].Point2);
				if (vector3f != null)
				{
					list.Add(new Cross(vector3f, i));
				}
			}
			return list;
		}

		public Vector3f DeltaEdge(Vector3f p_vector, int p_side)
		{
			if (p_side == -1)
			{
				return new Vector3f(0f, 0f, 0f);
			}
			Vector3f vector3f = new Vector3f(p_vector);
			if (_TempPoints.Count > 0)
			{
				vector3f.Add(_Point1 - _TempPoints[0]);
			}
			return Closest(vector3f, p_side) - vector3f;
		}

		public Vector3f Closest(Vector3f p_vector, int p_side)
		{
			Vector3f point = _Edges[p_side].Point1;
			Vector3f p_lineDirection = _Edges[p_side].Point2 - point;
			return Vector3f.Closest(p_vector, point, p_lineDirection);
		}

		public bool IsPointToLine(Vector3f p_vector, int p_side)
		{
			Vector3f vector3f = p_vector - _Edges[p_side].Point1;
			Vector3f vector3f2 = p_vector - _Edges[p_side].Point2;
			return vector3f * vector3f2 < 0f;
		}

		public double Distance(Vector3f p_point, int p_side)
		{
			return Math.Abs(Distance(p_point, _Edges[p_side].Point1, _Edges[p_side].Point2));
		}

		public double Distance(Vector3f p_point, Vector3f p_vector1, Vector3f p_vector2)
		{
			Vector3f vector3f = new Vector3f(p_vector2.Y - p_vector1.Y, p_vector1.X - p_vector2.X, 0f);
			return vector3f.Normalize() * (p_point - p_vector1);
		}

		public Vector3f CornerByIndex(int p_index)
		{
			switch (p_index)
			{
			case 0:
				return _Point1;
			case 1:
				return _Point2;
			case 2:
				return _Point3;
			case 3:
				return _Point4;
			default:
				return null;
			}
		}

		public Vector3f Corner(int p_sign, int p_cornernum)
		{
			if (p_sign > 0)
			{
				switch (p_cornernum)
				{
				case 0:
					return (_TempPoints.Count <= 0) ? _Point1 : _TempPoints[0];
				case 1:
					return (_TempPoints.Count <= 0) ? _Point2 : _TempPoints[1];
				case 2:
					return (_TempPoints.Count <= 0) ? _Point3 : _TempPoints[2];
				case 3:
					return (_TempPoints.Count <= 0) ? _Point4 : _TempPoints[3];
				}
			}
			else
			{
				switch (p_cornernum)
				{
				case 0:
					return (_TempPoints.Count <= 0) ? _Point2 : _TempPoints[1];
				case 1:
					return (_TempPoints.Count <= 0) ? _Point1 : _TempPoints[0];
				case 2:
					return (_TempPoints.Count <= 0) ? _Point4 : _TempPoints[3];
				case 3:
					return (_TempPoints.Count <= 0) ? _Point3 : _TempPoints[2];
				}
			}
			return null;
		}

		public virtual bool Hit(Vector3f p_point, bool Equality = false)
		{
			return Rectangle.Contains(p_point, 0.01f);
		}

		public void SetProperties()
		{
			SetX();
			SetY();
			CalcPoints();
			SetEdge();
			SetRectangle();
		}

		protected virtual void SetRectangle()
		{
			Rectangle.Set(_XQuad, _YQuad, _WidthQuad, _HeightQuad);
		}

		protected virtual void CalcPoints()
		{
			_Point1.Set(_XQuad, _YQuad, 0f);
			_Point2.Set(_XQuad + _WidthQuad, _YQuad, 0f);
			_Point3.Set(_XQuad + _WidthQuad, _YQuad + _HeightQuad, 0f);
			_Point4.Set(_XQuad, _YQuad + _HeightQuad, 0f);
		}

		public void SetX()
		{
			_XQuad = base.Position.x;
		}

		public void SetY()
		{
			_YQuad = base.Position.y;
		}

		public void SetEdge()
		{
			if (_Edges.Count == 0)
			{
				_Edges.Add(new Edge
				{
					Point1 = _Point1,
					Point2 = _Point2
				});
				_Edges.Add(new Edge
				{
					Point1 = _Point2,
					Point2 = _Point3
				});
				_Edges.Add(new Edge
				{
					Point1 = _Point3,
					Point2 = _Point4
				});
				_Edges.Add(new Edge
				{
					Point1 = _Point4,
					Point2 = _Point1
				});
			}
		}

		public void SetTempPoint(int Frames)
		{
		}

		public override void TransformationStart()
		{
			if (_ActiveTransformation == 0 && this.OnTransformationStart != null)
			{
				this.OnTransformationStart(this);
			}
			base.TransformationStart();
		}

		public override void TransformationEnd()
		{
			base.TransformationEnd();
			if (_ActiveTransformation == 0 && this.OnTransformationEnd != null)
			{
				this.OnTransformationEnd(this);
			}
		}

		public override void TransformResize(float p_w, float p_h)
		{
			base.TransformResize(p_w, p_h);
			_WidthQuad = p_w * _WidthQuad;
			_HeightQuad = p_h * _HeightQuad;
			SetProperties();
		}

		public override void End()
		{
			base.End();
			this.OnTransformationStart = null;
			this.OnTransformationEnd = null;
		}
	}
}
