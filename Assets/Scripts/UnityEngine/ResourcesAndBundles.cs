using Nekki.Vector.Core.AssetBundle;

namespace UnityEngine
{
	public static class ResourcesAndBundles
	{
		public static T Load<T>(string p_path) where T : Object
		{
			T val = BundleManager.LoadAsset<T>(p_path);
			if (val != null)
			{
				return val;
			}
			return Resources.Load<T>(p_path);
		}

		public static T[] LoadAll<T>(string p_path) where T : Object
		{
			T[] array = BundleManager.LoadAssetWithSubAssets<T>(p_path);
			if (array != null)
			{
				return array;
			}
			return Resources.LoadAll<T>(p_path);
		}
	}
}
