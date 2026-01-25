using System;
using System.Collections.Generic;
using System.IO;
using Nekki.Yaml;
using UnityEngine;

public static class GlobalPaths
{
	public struct Path
	{
		public string Local;

		public string Global;

		public string Bundle;

		public string Raw;

		public bool HasBundle;

		public bool HasLocal;

		public bool HasGlobal;

		public static Path Empty = new Path(string.Empty, string.Empty, string.Empty, string.Empty);

		public bool isEmpty
		{
			get
			{
				return !HasLocal && !HasGlobal && !HasBundle;
			}
		}

		public Path(string local, string global, string bundle, string raw)
		{
			if (string.IsNullOrEmpty(local))
			{
				HasLocal = false;
				Local = string.Empty;
			}
			else
			{
				HasLocal = true;
				Local = local.Trim('/').Trim('\\');
			}
			if (string.IsNullOrEmpty(global))
			{
				HasGlobal = false;
				Global = string.Empty;
			}
			else
			{
				HasGlobal = true;
				Global = global.Trim('/').Trim('\\');
			}
			if (string.IsNullOrEmpty(bundle))
			{
				HasBundle = false;
				Bundle = string.Empty;
			}
			else
			{
				HasBundle = true;
				Bundle = bundle.Trim('/').Trim('\\');
			}
			Raw = raw.Trim('/').Trim('\\');
		}

		public Path AddFolder(string folder)
		{
			if (HasLocal)
			{
				Local = Local + "/" + folder;
			}
			if (HasGlobal)
			{
				Global = Global + "/" + folder;
			}
			if (HasBundle)
			{
				Bundle = Bundle + "/" + folder;
			}
			return this;
		}

		public static explicit operator string(Path p)
		{
			return p.Global;
		}
	}

	private const string PathToResourcesFolder = "gamedata/Resources";

	private const string PathToBunlesFolder = "gamedata/Bundles";

	private static readonly Dictionary<string, string> Pathes;

	public static YamlDocumentNekki ConfigYaml { get; private set; }

	public static string ResourcesPath
	{
		get
		{
			return string.Format("{0}/{1}", Application.dataPath, "gamedata/Resources");
		}
	}

	public static string ExternalPath
	{
		get
		{
			if (SystemProperties.IsMobilePlatform)
			{
				return string.Format("{0}/{1}", Application.persistentDataPath, "gamedata/Resources");
			}
			return string.Format("{0}/{1}", Application.dataPath.Replace("Assets", string.Empty).TrimEnd('/'), "gamedata/Resources");
		}
	}

	public static string BundlesPath
	{
		get
		{
			if (SystemProperties.IsMobilePlatform)
			{
				return string.Format("{0}/{1}", Application.persistentDataPath, "gamedata/Bundles");
			}
			return string.Format("{0}/{1}", Application.dataPath.Replace("Assets", string.Empty).TrimEnd('/'), "gamedata/Bundles");
		}
	}

	public static string CurrentResolution
	{
		get
		{
			return (!SystemProperties.IsHighResolution) ? "768" : "1536";
		}
	}

	public static string AssetServer
	{
		get
		{
			return GetLocalPath("AssetServer");
		}
	}

	static GlobalPaths()
	{
		Pathes = new Dictionary<string, string>();
		Pathes = new Dictionary<string, string>();
	}

	public static string GetActualPathToData()
	{
		if (Application.isMobilePlatform)
		{
			return Application.persistentDataPath;
		}
		if (Application.isEditor)
		{
			return Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "gamedata";
		}
		return Application.dataPath + System.IO.Path.DirectorySeparatorChar + "gamedata";
	}

	public static string GetGlobalPath(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			return Pathes[name].Replace("EXTERNAL_PATH", ExternalPath).Replace("CURRENT_RESOLUTION", CurrentResolution).TrimEnd('/')
				.Trim('\\');
		}
		AdvLog.LogError(string.Format("path not found: {0}", name));
		return string.Empty;
	}

	public static string GetLocalPath(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			return Pathes[name].Replace("EXTERNAL_PATH", ExternalPath).Replace("CURRENT_RESOLUTION", CurrentResolution).Trim('/')
				.Trim('\\');
		}
		AdvLog.LogError(string.Format("path not found: {0}", name));
		return string.Empty;
	}

	public static string GetRawPath(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			return Pathes[name].Trim('/').Trim('\\');
		}
		AdvLog.LogError(string.Format("path not found: {0}", name));
		return string.Empty;
	}

	public static Path GetPath(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			string text = Pathes[name];
			return new Path(text.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", string.Empty), text.Replace("EXTERNAL_PATH", ExternalPath).Replace("CURRENT_RESOLUTION", CurrentResolution), text.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", CurrentResolution), text);
		}
		AdvLog.LogError(string.Format("path not found: {0}", name));
		return Path.Empty;
	}

	public static Path CreatePath(string pathUrl)
	{
		pathUrl = "EXTERNAL_PATH/" + pathUrl;
		return new Path(pathUrl.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", string.Empty), pathUrl.Replace("EXTERNAL_PATH", ExternalPath).Replace("CURRENT_RESOLUTION", CurrentResolution), pathUrl.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", CurrentResolution), pathUrl);
	}
}
