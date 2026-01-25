using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Nekki.Vector.GUI
{
	public static class AtlasCache
	{
		private static Dictionary<string, Sprite[]> _CachedAtlases = new Dictionary<string, Sprite[]>();

		public static Sprite[] GetAtlas(string p_atlasPath)
		{
			if (!_CachedAtlases.ContainsKey(p_atlasPath))
			{
				SpriteAtlas atlas = ResourcesAndBundles.Load<SpriteAtlas>(p_atlasPath);
				if (atlas == null)
				{
					return new Sprite[0];
				}
				Sprite[] array = new Sprite[atlas.spriteCount];
				atlas.GetSprites(array);
				if (array != null)
				{
					foreach (Sprite sprite in array)
					{
						sprite.name = sprite.name.Replace("(Clone)", "");
					}
					_CachedAtlases.Add(p_atlasPath, array);
				}
				return array;
				
			}
			return _CachedAtlases[p_atlasPath];
		}

		public static Sprite GetSpriteFromAtlas(string p_atlasPath, string p_spriteName)
		{
			Sprite[] atlas = GetAtlas(p_atlasPath);
			Sprite[] array = atlas;
			foreach (Sprite sprite in array)
			{
				if (sprite.name == p_spriteName)
				{
					return sprite;
				}
			}
			return null;
		}

		public static void Clear()
		{
			_CachedAtlases.Clear();
		}
	}
}
