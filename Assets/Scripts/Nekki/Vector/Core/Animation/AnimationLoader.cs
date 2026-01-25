using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationLoader
	{
		private static AnimationLoader _Current = new AnimationLoader();

		private string _BinaryPath;

		private string _XmlPath;

		private AnimationInfo _Info;

		public static AnimationLoader Current
		{
			get
			{
				return _Current;
			}
		}

		public AnimationInfo Info
		{
			get
			{
				return _Info;
			}
		}

		public void Init()
		{
			_XmlPath = VectorPaths.RunDataLibs + "/moves_new.xml";
			_BinaryPath = VectorPaths.AnimationBinary;
			ParseAnimations();
		}

		public void ReloadAnimations()
		{
			AnimationGroup.ClearGroups();
			Animations.Animation.Clear();
			ParseAnimations();
		}

		public void ParseGroups(XmlNode Node)
		{
			foreach (XmlNode childNode in Node.ChildNodes)
			{
				List<AnimationReaction> list = new List<AnimationReaction>();
				foreach (XmlNode childNode2 in childNode.ChildNodes)
				{
					list.Add(new AnimationReaction(childNode2));
				}
				AnimationGroup animationGroup = new AnimationGroup();
				animationGroup.Reactions = list;
				animationGroup.Name = childNode.Attributes["Name"].Value;
				AnimationGroup.Add(animationGroup);
			}
		}

		public void ParseAnimations()
		{
			Dictionary<string, XmlNode> dictionary = new Dictionary<string, XmlNode>();
			XmlNode xmlNode = XmlUtils.OpenXMLDocument(_XmlPath, string.Empty)["root"];
			foreach (XmlNode childNode in xmlNode["EventGroups"].ChildNodes)
			{
				dictionary[childNode.Attributes["Name"].Value] = childNode;
			}
			ParseGroups(xmlNode["ReactionGroups"]);
			XmlNode xmlNode3 = xmlNode["Moves"];
			foreach (XmlNode childNode2 in xmlNode3.ChildNodes)
			{
				AnimationInfo animationInfo = null;
				animationInfo = ((childNode2.Attributes["Trick"] != null && XmlUtils.ParseInt(childNode2.Attributes["Trick"]) != 0) ? new AnimationTrickInfo(childNode2) : new AnimationInfo(childNode2));
				animationInfo.Load(_BinaryPath);
				foreach (XmlNode childNode3 in childNode2.ChildNodes)
				{
					if (childNode3.Name != "Interval")
					{
						continue;
					}
					AnimationInterval animationInterval = null;
					if (childNode3.Attributes["Groups"] == null)
					{
						animationInterval = new AnimationInterval(childNode3);
					}
					else
					{
						List<XmlNode> list = new List<XmlNode>();
						string[] array = childNode3.Attributes["Groups"].Value.Split('|');
						foreach (string key in array)
						{
							list.Add(dictionary[key]);
						}
						animationInterval = new AnimationInterval(childNode3, list);
					}
					animationInfo.Intervals.Add(animationInterval);
				}
				Animations.Animation[animationInfo.Name] = animationInfo;
			}
			AnimationBinaryParser.ClearCachedBinary();
		}

		public static AnimationReaction ParseReaction(List<string> p_array)
		{
			if (p_array == null || p_array.Count == 0)
			{
				return null;
			}
			AnimationReaction animationReaction = new AnimationReaction();
			animationReaction.Name = p_array[0];
			animationReaction.FirstFrame = int.Parse(p_array[1]);
			AnimationReaction animationReaction2 = animationReaction;
			if (p_array.Count == 2)
			{
				animationReaction2.Reverse = false;
			}
			else
			{
				animationReaction2.Reverse = int.Parse(p_array[2]) > 0;
			}
			return animationReaction2;
		}
	}
}
