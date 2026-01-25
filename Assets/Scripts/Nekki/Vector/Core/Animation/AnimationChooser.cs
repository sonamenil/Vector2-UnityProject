using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Detector;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Animation
{
	public static class AnimationChooser
	{
		private static List<AnimationReaction> _Reactions = new List<AnimationReaction>();

		private static Random _Random = new Random();

		public static void Reset()
		{
			_Reactions.Clear();
		}

		public static bool IsEmpty()
		{
			return _Reactions.Count == 0;
		}

		public static void AddReaction(AnimationReaction p_reaction)
		{
			_Reactions.Add(p_reaction);
		}

		public static void AddReactions(List<AnimationReaction> p_reactions)
		{
			if (p_reactions != null)
			{
				for (int i = 0; i < p_reactions.Count; i++)
				{
					_Reactions.Add(p_reactions[i]);
				}
			}
		}

		public static AnimationReaction Choose(AnimationDeltaData p_deltaH, AnimationDeltaData p_deltaV, AnimationDeltaData p_deltaC, List<AreaRunner> p_areas, ModelHuman p_model, Vector3f p_velocity)
		{
			if (_Reactions.Count == 0)
			{
				return null;
			}
			SortByInside(p_model);
			SortByArea(p_areas, p_model);
			SortBySlope(p_deltaH, p_deltaV, p_deltaC, p_model);
			SortByDelta(p_deltaH, p_deltaV, p_deltaC, p_model.Sign, p_velocity);
			SortByPriority();
			return Random();
		}

		private static void SortByInside(ModelHuman p_model)
		{
			DetectorLine detectorHorizontalLine = p_model.ModelObject.DetectorHorizontalLine;
			DetectorLine detectorVerticalLine = p_model.ModelObject.DetectorVerticalLine;
			for (int num = _Reactions.Count - 1; num >= 0; num--)
			{
				if (!_Reactions[num].IsInside(detectorHorizontalLine, detectorVerticalLine))
				{
					_Reactions.RemoveAt(num);
				}
			}
		}

		private static void SortByArea(List<AreaRunner> p_areas, ModelHuman p_model)
		{
			for (int num = _Reactions.Count - 1; num >= 0; num--)
			{
				if (_Reactions[num].AreaName != null)
				{
					bool flag = false;
					foreach (AreaRunner p_area in p_areas)
					{
						if (_Reactions[num].CheckNameHash(p_area.NameHash))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						_Reactions.Remove(_Reactions[num]);
					}
				}
			}
		}

		private static void SortBySlope(AnimationDeltaData p_deltaH, AnimationDeltaData p_deltaV, AnimationDeltaData p_deltaС, ModelHuman p_model)
		{
			for (int num = _Reactions.Count - 1; num >= 0; num--)
			{
				if (!_Reactions[num].IsSlope(p_deltaH, p_deltaV, p_model.Sign))
				{
					_Reactions.RemoveAt(num);
				}
			}
		}

		private static void SortByDelta(AnimationDeltaData p_deltaH, AnimationDeltaData p_deltaV, AnimationDeltaData p_deltaС, int p_sign, Vector3f p_velocity)
		{
			for (int num = _Reactions.Count - 1; num >= 0; num--)
			{
				if (!_Reactions[num].IsDeltaCheck(p_deltaH, p_deltaV, p_deltaС, p_sign, p_velocity))
				{
					_Reactions.RemoveAt(num);
				}
			}
		}

		private static void SortByPriority()
		{
			int num = MaxPriority();
			for (int num2 = _Reactions.Count - 1; num2 >= 0; num2--)
			{
				if (_Reactions[num2].Priority != num)
				{
					_Reactions.RemoveAt(num2);
				}
			}
		}

		private static AnimationReaction Random()
		{
			return (_Reactions.Count != 0) ? _Reactions[_Random.Next(0, _Reactions.Count - 1)] : null;
		}

		private static int MaxPriority()
		{
			int num = 0;
			for (int i = 0; i < _Reactions.Count; i++)
			{
				num = Math.Max(_Reactions[i].Priority, num);
			}
			return num;
		}
	}
}
