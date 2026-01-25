using System.IO;
using UnityEngine;

namespace Nekki.Vector.Core.AssetBundle
{
	public static class AssetBundleExtension
	{
		public static string SimplifyAssetName(string p_assetName)
		{
			return Path.GetFileNameWithoutExtension(p_assetName.ToLower());
		}

		public static string[] GetAllSimplifiedAssetNames(this UnityEngine.AssetBundle p_bundle)
		{
			string[] allAssetNames = p_bundle.GetAllAssetNames();
			string[] array = new string[allAssetNames.Length];
			int i = 0;
			for (int num = allAssetNames.Length; i < num; i++)
			{
				array[i] = SimplifyAssetName(allAssetNames[i]);
			}
			return array;
		}
	}
}
