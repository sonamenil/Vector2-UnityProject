using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NekkiAssetImporter
{
	private static readonly Dictionary<string, string> BundlesContent = new Dictionary<string, string>();

	private static readonly Dictionary<string, AssetBundle> Loaded = new Dictionary<string, AssetBundle>();

	public static void Init(List<NekkiAssetDownloader.BundleData> data)
	{
		if (!Directory.Exists(GlobalPaths.BundlesPath))
		{
			AdvLog.LogWarning(string.Format("<NekkiAssetImporter::Init> {0} was not found", GlobalPaths.BundlesPath));
			return;
		}
		string[] files = Directory.GetFiles(GlobalPaths.BundlesPath, "*.config");
		string[] array = files;
		foreach (string text in array)
		{
			AnalizeConfig(File.ReadAllText(text), text);
		}
	}

	private static void AnalizeConfig(string content, string fileName)
	{
		string[] array = content.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			string text2 = text.Split(':')[1].Replace("Assets/gamedata/Bundles/", string.Empty).Replace("Assets/gamedata/Resources/", string.Empty);
			if (text2.Contains("."))
			{
				text2 = text2.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
			}
			if (!BundlesContent.ContainsKey(text2))
			{
				BundlesContent.Add(text2, fileName.Replace(".config", ".unity3d"));
			}
		}
	}

	public static bool Contains(string path)
	{
		return BundlesContent.ContainsKey(RemoveDots(path));
	}

	private static string RemoveDots(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return string.Empty;
		}
		if (path.Contains("."))
		{
			path = path.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
		}
		return path;
	}

	private static string ExtractPath(string bundle)
	{
		if (string.IsNullOrEmpty(bundle))
		{
			return string.Empty;
		}
		return bundle.Replace("/", "^");
	}

	public static AudioClip GetAudioClip(string bundle)
	{
		bundle = RemoveDots(bundle);
		if (!Contains(bundle))
		{
			return null;
		}
		AssetBundle assetBundle = CheckBundle(bundle);
		return assetBundle ? assetBundle.LoadAsset<AudioClip>(ExtractPath(bundle)) : null;
	}

	public static string GetText(string bundle)
	{
		bundle = RemoveDots(bundle);
		if (!Contains(bundle))
		{
			return null;
		}
		AssetBundle assetBundle = CheckBundle(bundle);
		return assetBundle ? assetBundle.LoadAsset<TextAsset>(ExtractPath(bundle)).text : null;
	}

	public static byte[] GetBytes(string bundle)
	{
		bundle = RemoveDots(bundle);
		if (!Contains(bundle))
		{
			return null;
		}
		AssetBundle assetBundle = CheckBundle(bundle);
		return assetBundle ? assetBundle.LoadAsset<TextAsset>(ExtractPath(bundle)).bytes : null;
	}

	public static Texture2D GetTexture(string bundle)
	{
		bundle = RemoveDots(bundle);
		if (!Contains(bundle))
		{
			return null;
		}
		AssetBundle assetBundle = CheckBundle(bundle);
		return assetBundle ? assetBundle.LoadAsset<Texture2D>(ExtractPath(bundle)) : null;
	}

	public static GameObject GetObject(string bundle)
	{
		bundle = RemoveDots(bundle);
		if (!Contains(bundle))
		{
			return null;
		}
		AssetBundle assetBundle = CheckBundle(bundle);
		GameObject gameObject = assetBundle.LoadAsset<GameObject>(ExtractPath(bundle));
		if (!gameObject)
		{
			gameObject = assetBundle.LoadAsset<GameObject>(ExtractPath(bundle + ".prefab"));
		}
		AdvLog.Log(string.Format("object {1} is alive {0}", (bool)gameObject, bundle));
		return gameObject;
	}

	private static AssetBundle CheckBundle(string path)
	{
		if (!Contains(path))
		{
			return null;
		}
		string text = BundlesContent[path];
		if (!Loaded.ContainsKey(text))
		{
			Loaded.Add(text, AssetBundle.LoadFromFile(text));
		}
		return Loaded[text];
	}
}
