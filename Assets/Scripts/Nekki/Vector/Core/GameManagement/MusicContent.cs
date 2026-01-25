using System.Collections.Generic;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class MusicContent
	{
		public const string NodeName = "MusicContent";

		public string TrackName { get; private set; }

		public string AmbientName { get; private set; }

		public float Volume { get; private set; }

		private MusicContent(Mapping p_node)
		{
			TrackName = YamlUtils.GetStringValue(p_node.GetText("TrackName"), string.Empty);
			AmbientName = YamlUtils.GetStringValue(p_node.GetText("AmbientName"), string.Empty);
			Volume = YamlUtils.GetFloatValue(p_node.GetText("Volume"), 1f);
		}

		public static List<MusicContent> CreateList(Sequence p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			List<MusicContent> list = new List<MusicContent>();
			foreach (Mapping item2 in p_node)
			{
				MusicContent item = new MusicContent(item2);
				list.Add(item);
			}
			return (list.Count == 0) ? null : list;
		}
	}
}
