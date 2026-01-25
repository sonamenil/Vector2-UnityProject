using System.Collections.Generic;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationGroup
	{
		private static List<AnimationGroup> _Groups = new List<AnimationGroup>();

		private string _Name;

		private List<AnimationReaction> _Reactions;

		public string Name
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

		public List<AnimationReaction> Reactions
		{
			get
			{
				return _Reactions;
			}
			set
			{
				_Reactions = value;
			}
		}

		public static void Add(AnimationGroup Group)
		{
			_Groups.Add(Group);
		}

		public static AnimationGroup GetGroup(string Name)
		{
			foreach (AnimationGroup group in _Groups)
			{
				if (group.Name == Name)
				{
					return group;
				}
			}
			return null;
		}

		public static List<AnimationReaction> GetReactions(string Name)
		{
			AnimationGroup group = GetGroup(Name);
			return (group != null) ? group.Reactions : null;
		}

		public static void ClearGroups()
		{
			_Groups.Clear();
		}
	}
}
