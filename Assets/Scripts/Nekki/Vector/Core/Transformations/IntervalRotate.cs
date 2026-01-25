using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalRotate : PrototypeInterval
	{
		private List<float> _Angles;

		public IntervalRotate()
		{
			_Type = IntervalType.Rotate;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_Angles = new List<float>();
			float num = XmlUtils.ParseFloat(p_node.Attributes["Angle"]);
			switch (XmlUtils.ParseString(p_node.Attributes["Type"], "Linear"))
			{
			case "Linear":
				CalcLinear(num);
				break;
			case "EaseIn":
				CalcEaseInCubic(num);
				break;
			case "EaseOut":
				CalcEaseOutCubic(num);
				break;
			case "Sin_1QuarterAcc":
				TransformationUtils.CalcSinusAngles(num, _Frames, "1QuarterAcc", ref _Angles);
				break;
			case "Sin_1QuarterDec":
				TransformationUtils.CalcSinusAngles(num, _Frames, "1QuarterDec", ref _Angles);
				break;
			case "Sin_2Quarters":
				TransformationUtils.CalcSinusAngles(num, _Frames, "2Quarters", ref _Angles);
				break;
			case "Sin_2QuartersFastSlowFast":
				TransformationUtils.CalcSinusAngles(num, _Frames, "2QuartersFastSlowFast", ref _Angles);
				break;
			case "Sin_3Quarters":
				TransformationUtils.CalcSinusAngles(num, _Frames, "3Quarters", ref _Angles);
				break;
			}
		}

		private void CalcLinear(float p_angel)
		{
			float item = p_angel / (float)_Frames;
			for (int i = 0; i < _Frames; i++)
			{
				_Angles.Add(item);
			}
		}

		private void CalcEaseInCubic(float p_angel)
		{
			for (int i = 0; i < _Frames; i++)
			{
				float num = (float)i / (float)_Frames;
				float item = p_angel * num * num * num;
				_Angles.Add(item);
			}
			_Angles.Add(p_angel);
			for (int num2 = _Angles.Count - 1; num2 > 0; num2--)
			{
				List<float> angles;
				List<float> list = (angles = _Angles);
				int index;
				int index2 = (index = num2);
				float num3 = angles[index];
				list[index2] = num3 - _Angles[num2 - 1];
			}
			_Angles.RemoveAt(0);
		}

		private void CalcEaseOutCubic(float p_angel)
		{
			for (int i = 0; i < _Frames; i++)
			{
				float num = (float)i / (float)_Frames;
				num -= 1f;
				float item = p_angel * (num * num * num + 1f);
				_Angles.Add(item);
			}
			for (int num2 = _Angles.Count - 1; num2 > 0; num2--)
			{
				List<float> angles;
				List<float> list = (angles = _Angles);
				int index;
				int index2 = (index = num2);
				float num3 = angles[index];
				list[index2] = num3 - _Angles[num2 - 1];
			}
			_Angles.RemoveAt(0);
		}

		public override bool Iteration(TransformInterface p_runner)
		{
			if (!p_runner.IsEnabled)
			{
				Reset();
				return false;
			}
			p_runner.TransformRotateZ(_Angles[_CurrentFrame]);
			_CurrentFrame++;
			if (_CurrentFrame >= _Angles.Count)
			{
				Reset();
				return false;
			}
			return true;
		}
	}
}
