using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Nekki.Vector.Core.User;
using UnityEngine;
using UnityEngine.U2D;

namespace Nekki.Vector.Core
{
	public static class ResourcesMap
	{
		private static Dictionary<string, Sprite[]> _AtlasCache = new Dictionary<string, Sprite[]>();

		private static string QualityPrefix
		{
			get
			{
				return (!DataLocal.Current.Settings.UseLowResGraphics) ? string.Empty : "_low";
			}
		}

		public static Mesh GetMesh(string p_name)
		{
			string path = "Meshes/" + p_name;
			return (Mesh)Resources.Load(path, typeof(Mesh));
		}

		public static Sprite GetSprite(string p_name)
		{
			string[] array = p_name.Split('.');
			string text = array[0];
			string qualityPrefix = QualityPrefix;
			if (array.Length == 1)
			{
				return ResourcesAndBundles.Load<Sprite>(string.Format("Run/SingleSprite/{0}{1}", text, QualityPrefix));
			}
			Sprite[] orCreateAtlas = GetOrCreateAtlas(text);
			for (int i = 0; i < orCreateAtlas.Length; i++)
			{
				if (orCreateAtlas[i].name == p_name)
				{
					return orCreateAtlas[i];
				}
			}
			return null;
		}

		public static List<Sprite> GetFramesSequence(string p_name)
		{
			string[] array = p_name.Split('.');
			string p_atlasName = array[0];
			Sprite[] orCreateAtlas = GetOrCreateAtlas(p_atlasName);
			List<Sprite> list = new List<Sprite>();
			for (int i = 0; i < orCreateAtlas.Length; i++)
			{
				if (orCreateAtlas[i].name.IndexOf(p_name) != -1)
				{
					list.Add(orCreateAtlas[i]);
				}
			}
			list.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));
			return list;
		}

		public static List<KeyValuePair<Sprite, int>> GetCustomFramesSequence(string p_name)
		{
			List<KeyValuePair<Sprite, int>> list = new List<KeyValuePair<Sprite, int>>();
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.CustomAnimations + "/" + p_name, string.Empty);
			XmlNode xmlNode = xmlDocument["CustomAnimation"];
			string[] array = xmlNode.Attributes["Atlas"].Value.Split('.');
			string text = array[0];
			Sprite[] orCreateAtlas = GetOrCreateAtlas(text);
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				Sprite key = null;
				for (int i = 0; i < orCreateAtlas.Length; i++)
				{
					if (orCreateAtlas[i].name == text + "." + childNode.Attributes["Name"].Value)
					{
						key = orCreateAtlas[i];
						break;
					}
				}
				KeyValuePair<Sprite, int> item = new KeyValuePair<Sprite, int>(key, Convert.ToInt32(childNode.Attributes["Frames"].Value));
				list.Add(item);
			}
			return list;
		}

		public static void ResetSpriteAtlasCache()
		{
			_AtlasCache.Clear();
		}

		private static Sprite[] GetOrCreateAtlas(string p_atlasName)
		{
			Sprite[] value = null;
			if (!_AtlasCache.TryGetValue(p_atlasName, out value))
			{
				SpriteAtlas atlas = ResourcesAndBundles.Load<SpriteAtlas>(string.Format("Run/Atlases/{0}{1}", p_atlasName, QualityPrefix));
				value = new Sprite[atlas.spriteCount];
				atlas.GetSprites(value);
				foreach (Sprite sprite in value)
				{
					sprite.name = sprite.name.Replace("(Clone)", "");
				}
				_AtlasCache.Add(p_atlasName, value);
			}
			return value;
		}
	}
}
