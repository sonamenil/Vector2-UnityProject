using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationDeltas
	{
		private AnimationDeltaType _Type;

		private AnimationDeltaName _Name;

		private List<AnimationDelta> _Deltas = new List<AnimationDelta>();

		public AnimationDeltas(XmlNode p_mainNode, AnimationDeltaType p_type)
		{
			_Type = p_type;
			foreach (XmlNode childNode in p_mainNode.ChildNodes)
			{
				if (_Type == GetDeltaTypeByString(childNode.Attributes["Type"].Value))
				{
					Point p_value = new Point(XmlUtils.ParseFloat(childNode.Attributes["Min"], float.NaN), XmlUtils.ParseFloat(childNode.Attributes["Max"], float.NaN));
					int p_corner = XmlUtils.ParseInt(childNode.Attributes["Corner"], -1);
					_Deltas.Add(new AnimationDelta(GetDeltaNameByString(childNode.Name), p_type, p_value, p_corner));
				}
			}
		}

		public static AnimationDeltaName GetDeltaNameByString(string p_name)
		{
			switch (p_name)
			{
			case "Width":
				return AnimationDeltaName.Width;
			case "Height":
				return AnimationDeltaName.Height;
			case "DeltaX":
				return AnimationDeltaName.DeltaX;
			case "DeltaY":
				return AnimationDeltaName.DeltaY;
			case "VelosityX":
				return AnimationDeltaName.VelocityX;
			case "VelosityY":
				return AnimationDeltaName.VelocityY;
			default:
				return AnimationDeltaName.Max;
			}
		}

		public static AnimationDeltaType GetDeltaTypeByString(string p_type)
		{
			switch (p_type)
			{
			case "H":
				return AnimationDeltaType.Horizontal;
			case "V":
				return AnimationDeltaType.Vertical;
			case "C":
				return AnimationDeltaType.Collision;
			default:
				return AnimationDeltaType.Max;
			}
		}

		public bool IsVelosityCheck(Vector3f p_subSpeed)
		{
			List<AnimationDelta> deltasByName = GetDeltasByName(AnimationDeltaName.VelocityX);
			foreach (AnimationDelta item in deltasByName)
			{
				if (!item.IsInterval(p_subSpeed.X))
				{
					return false;
				}
			}
			deltasByName = GetDeltasByName(AnimationDeltaName.VelocityY);
			foreach (AnimationDelta item2 in deltasByName)
			{
				if (!item2.IsInterval(p_subSpeed.X))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsCheck(AnimationDeltaData p_deltaSort, int p_sign, Vector3f p_velocity)
		{
			if (_Deltas.Count == 0)
			{
				return true;
			}
			if (p_deltaSort == null && _Deltas.Count > 0)
			{
				return false;
			}
			foreach (AnimationDelta delta in _Deltas)
			{
				if (!IsDeltaName(delta, p_deltaSort, p_sign, p_velocity))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsDeltaName(AnimationDelta p_delta, AnimationDeltaData p_deltaData, int p_sign, Vector3f p_velocity)
		{
			int corner = p_delta.GetCorner(p_sign);
			float deltaValue = p_deltaData.GetDeltaValue(p_delta.Name, corner, p_sign, p_velocity);
			return p_delta.IsInterval(deltaValue);
		}

		public bool IsAllNan()
		{
			return _Deltas.Count == 0;
		}

		public AnimationDelta GetDeltaByName(AnimationDeltaName p_name)
		{
			foreach (AnimationDelta delta in _Deltas)
			{
				if (delta.Name == p_name)
				{
					return delta;
				}
			}
			return null;
		}

		public List<AnimationDelta> GetDeltasByName(AnimationDeltaName p_name)
		{
			List<AnimationDelta> list = new List<AnimationDelta>();
			foreach (AnimationDelta delta in _Deltas)
			{
				if (delta.Name == p_name)
				{
					list.Add(delta);
				}
			}
			return list;
		}
	}
}
