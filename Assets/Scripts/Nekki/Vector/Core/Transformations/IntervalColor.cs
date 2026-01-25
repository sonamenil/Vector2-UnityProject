using System.Xml;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalColor : PrototypeInterval
	{
		private Color _StartColor;

		private Color _EndColor;

		private Color _DeltaColor;

		private bool isFirstIteration;

		public IntervalColor()
		{
			_Type = IntervalType.Color;
			isFirstIteration = true;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			ParseColor(p_node.Attributes["ColorStart"], ref _StartColor, new Color(-1f, -1f, -1f, -1f));
			ParseColor(p_node.Attributes["ColorFinish"], ref _EndColor, new Color(1f, 1f, 1f, 1f));
		}

		private static void ParseColor(XmlAttribute p_attr, ref Color p_color, Color p_defColor)
		{
			if (p_attr == null || p_attr.Value.Length == 0)
			{
				p_color = p_defColor;
			}
			else
			{
				p_color = ColorUtils.FromHex(p_attr.Value);
			}
		}

		public void CalcDelta(TransformInterface p_runner)
		{
			VisualRunner visualRunner = p_runner as VisualRunner;
			ObjectRunner objectRunner = p_runner as ObjectRunner;
			if (_StartColor.r < 0f)
			{
				if (visualRunner != null)
				{
					_StartColor = visualRunner.Color;
				}
				if (objectRunner != null)
				{
					_StartColor = objectRunner.Color;
				}
			}
			else
			{
				if (visualRunner != null)
				{
					visualRunner.Color = _StartColor;
				}
				if (objectRunner != null)
				{
					objectRunner.Color = _StartColor;
				}
			}
			_DeltaColor = _EndColor - _StartColor;
			_DeltaColor /= (float)_Frames;
		}

		public override bool Iteration(TransformInterface p_runner)
		{
			if (!p_runner.IsEnabled)
			{
				Reset();
				return false;
			}
			if (isFirstIteration)
			{
				isFirstIteration = false;
				CalcDelta(p_runner);
			}
			_CurrentFrame++;
			if (_CurrentFrame >= _Frames)
			{
				p_runner.TransformColorEnd(_EndColor);
				Reset();
				return false;
			}
			p_runner.TransformColor(_DeltaColor);
			return true;
		}

		public override void Reset()
		{
			base.Reset();
			isFirstIteration = true;
		}
	}
}
