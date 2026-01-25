using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalMove : PrototypeInterval
	{
		protected List<Vector3f> _Points = new List<Vector3f>();

		public List<Vector3f> Points
		{
			get
			{
				return _Points;
			}
		}

		public IntervalMove()
		{
			_Type = IntervalType.Move;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			List<Vector3f> list = ParsePoints(p_node);
			switch (XmlUtils.ParseString(p_node.Attributes["Type"], "Bezier"))
			{
			case "Bezier":
				TransformationUtils.CalcBezierPoints(list, _Frames, ref _Points);
				break;
			case "Sin":
			{
				string quartersEnum = string.Empty;
				XmlElement xmlElement = p_node["Quarters"];
				if (xmlElement != null)
				{
					quartersEnum = XmlUtils.ParseString(xmlElement.Attributes["Value"], string.Empty);
				}
				TransformationUtils.CalcSinusPoints(list, _Frames, quartersEnum, ref _Points);
				break;
			}
			}
			for (int num = _Points.Count - 1; num > 0; num--)
			{
				_Points[num].Subtract(_Points[num - 1]);
			}
			_Points.RemoveAt(0);
		}

		protected List<Vector3f> ParsePoints(XmlNode p_node)
		{
			List<Vector3f> list = new List<Vector3f>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name == "Point")
				{
					Vector3f vector3f = Vector3f.Create(childNode);
					if (vector3f != null)
					{
						list.Add(vector3f);
					}
				}
			}
			return list;
		}

		public override bool Iteration(TransformInterface Runner)
		{
			if (!Runner.IsEnabled)
			{
				Reset();
				return false;
			}
			Runner.TransformMove(_Points[_CurrentFrame]);
			_CurrentFrame++;
			if (_CurrentFrame >= _Points.Count)
			{
				Reset();
				return false;
			}
			return true;
		}

		public bool InerationFake(ref Vector3f p_point, ref Vector3f p_velocity, bool p_isInit)
		{
			p_velocity.Set(_Points[_CurrentFrameFake].X, _Points[_CurrentFrameFake].Y, 0f);
			p_point.Add(_Points[_CurrentFrameFake]);
			_CurrentFrameFake++;
			if (_CurrentFrameFake >= _Points.Count)
			{
				_CurrentFrameFake = 0;
				return false;
			}
			return true;
		}
	}
}
