using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSize : PrototypeInterval
	{
		private const float _Eps = 1E-05f;

		protected List<Vector3f> _Points = new List<Vector3f>();

		public List<Vector3f> Points
		{
			get
			{
				return _Points;
			}
		}

		public IntervalSize()
		{
			_Type = IntervalType.Size;
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
			ReplaceZeroByEps(_Points[_Points.Count - 1]);
			for (int num = _Points.Count - 1; num > 1; num--)
			{
				ReplaceZeroByEps(_Points[num - 1]);
				_Points[num].Set(_Points[num].X / _Points[num - 1].X, _Points[num].Y / _Points[num - 1].Y, 0f);
			}
			_Points.RemoveAt(0);
		}

		protected void ReplaceZeroByEps(Vector3f p_point)
		{
			if (p_point.X == 0f)
			{
				p_point.X = 1E-05f;
			}
			if (p_point.Y == 0f)
			{
				p_point.Y = 1E-05f;
			}
		}

		protected List<Vector3f> ParsePoints(XmlNode p_node)
		{
			List<Vector3f> list = new List<Vector3f>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name == "Point")
				{
					float p_x = float.Parse(childNode.Attributes["W"].Value);
					float p_y = float.Parse(childNode.Attributes["H"].Value);
					list.Add(new Vector3f(p_x, p_y, 0f));
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
			Runner.TransformResize(_Points[_CurrentFrame].X, _Points[_CurrentFrame].Y);
			_CurrentFrame++;
			if (_CurrentFrame >= _Points.Count)
			{
				Reset();
				return false;
			}
			return true;
		}
	}
}
