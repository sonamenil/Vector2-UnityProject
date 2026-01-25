using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneLoader : ExtentionBehaviour
{
	public const int OnSceneLoadDone = 0;

	public const int OnConfigLoadDone = 1;

	public const string Server = "http://127.0.0.1";

	public const string DataFolderName = "Export";

	private static SceneLoader _instance;

	private static readonly Dictionary<string, AssetBundle> CachedScenes = new Dictionary<string, AssetBundle>();

	private static readonly Dictionary<string, SceneConfig> CachedConfigs = new Dictionary<string, SceneConfig>();

	private static bool _inited;

	public static SceneLoader Instance
	{
		get
		{
			if (!_instance)
			{
				SceneLoader sceneLoader = Object.FindObjectOfType<SceneLoader>();
				if (!sceneLoader)
				{
					_instance = new GameObject("_sceneLoader").AddComponent<SceneLoader>();
					Object.DontDestroyOnLoad(_instance.gameObject);
				}
				else
				{
					_instance = sceneLoader;
				}
			}
			return _instance;
		}
	}

	public static string Datapath { get; private set; }

	private static void Init()
	{
		string arg = ((!SystemProperties.IsMobilePlatform) ? Application.dataPath : Application.persistentDataPath);
		Datapath = string.Format("{0}/{1}", arg, "Export");
		if (!Directory.Exists(Datapath))
		{
			Directory.CreateDirectory(Datapath);
		}
	}

	internal void Start()
	{
		if (_inited)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Application.targetFrameRate = 60;
		_inited = true;
		Init();
		_instance = this;
		Object.DontDestroyOnLoad(_instance.gameObject);
	}

	public static bool HasInstance()
	{
		return _instance != null;
	}

	private static string GetPathForCurrentPlatform()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
			return "standaloneMacOSX";
		case RuntimePlatform.OSXPlayer:
			return "standaloneMacOSX";
		case RuntimePlatform.WindowsPlayer:
			return "standaloneWindows";
		case RuntimePlatform.WindowsEditor:
			return "standaloneWindows";
		case RuntimePlatform.IPhonePlayer:
			return "ios";
		case RuntimePlatform.Android:
			return "android";
		default:
			return "standaloneWindows";
		}
	}

	public static void GetScene(string scene)
	{
		if (CachedScenes.ContainsKey(scene))
		{
			Instance.callEvent(0, Object.Instantiate(CachedScenes[scene].mainAsset));
		}
		else if (File.Exists(string.Format("{0}/{1}/SceneRoot_{2}.ab", Datapath, GetPathForCurrentPlatform(), scene)))
		{
			Instance.StartCoroutine(LoadSceneRoutine(scene));
		}
		else
		{
			Instance.StartCoroutine(WebLoadScene(scene));
		}
		if (CachedConfigs.ContainsKey(scene))
		{
			Instance.callEvent(1, CachedConfigs[scene]);
		}
		else if (File.Exists(string.Format("{0}/{1}/SceneRoot_{2}_config.ab", Datapath, GetPathForCurrentPlatform(), scene)))
		{
			Instance.StartCoroutine(LoadConfigRoutine(scene));
		}
		else
		{
			Instance.StartCoroutine(WebLoadConfig(scene));
		}
	}

	private static IEnumerator LoadConfigRoutine(string scene)
	{
		string path = string.Format("file:///{0}/{1}/SceneRoot_{2}_config.ab", Datapath.Replace("\\", "/"), GetPathForCurrentPlatform(), scene);
		AdvLog.Log(path);
		WWW w = new WWW(path);
		yield return w;
		if (string.IsNullOrEmpty(w.error))
		{
			if (!CachedConfigs.ContainsKey(scene))
			{
				CachedConfigs.Add(scene, ((GameObject)w.assetBundle.mainAsset).GetComponent<SceneConfig>());
			}
			else
			{
				CachedConfigs[scene] = ((GameObject)w.assetBundle.mainAsset).GetComponent<SceneConfig>();
			}
			Instance.callEvent(1, CachedConfigs[scene]);
		}
		else
		{
			Instance.LogError(string.Format("cant load scene config {0}: {1}", scene, w.error));
		}
	}

	private static IEnumerator LoadSceneRoutine(string scene)
	{
		string path = string.Format("file:///{0}/{1}/SceneRoot_{2}.ab", Datapath.Replace("\\", "/"), GetPathForCurrentPlatform(), scene);
		AdvLog.Log(path);
		WWW w = new WWW(path);
		yield return w;
		if (string.IsNullOrEmpty(w.error))
		{
			if (!CachedScenes.ContainsKey(scene))
			{
				CachedScenes.Add(scene, w.assetBundle);
			}
			else
			{
				CachedScenes[scene] = w.assetBundle;
			}
			Instance.callEvent(0, Object.Instantiate(CachedScenes[scene].mainAsset));
		}
		else
		{
			Instance.LogError(string.Format("cant load scene {0}: {1}", scene, w.error));
		}
	}

	private static IEnumerator WebLoadScene(string scene)
	{
		string path = string.Format("{0}/{1}/SceneRoot_{2}.ab", "http://127.0.0.1", GetPathForCurrentPlatform(), scene);
		WWW w = new WWW(path);
		yield return w;
		if (string.IsNullOrEmpty(w.error))
		{
			File.WriteAllBytes(string.Format("{0}/{1}/SceneRoot_{2}.ab", Datapath, GetPathForCurrentPlatform(), scene), w.bytes);
			if (!CachedScenes.ContainsKey(scene))
			{
				CachedScenes.Add(scene, w.assetBundle);
			}
			else
			{
				CachedScenes[scene] = w.assetBundle;
			}
			Instance.callEvent(0, Object.Instantiate(CachedScenes[scene].mainAsset));
		}
		else
		{
			GameObject sceneObj = GlobalLoad.GetObject(string.Format("Export/SceneRoot_{0}", scene), string.Empty);
			if ((bool)sceneObj)
			{
				Instance.callEvent(0, Object.Instantiate(sceneObj));
			}
			else
			{
				Instance.LogError(string.Format("cant load scene {0}: {1}", scene, w.error));
			}
		}
	}

	private static IEnumerator WebLoadConfig(string scene)
	{
		string path = string.Format("{0}/{1}/SceneRoot_{2}_config.ab", "http://127.0.0.1", GetPathForCurrentPlatform(), scene);
		WWW w = new WWW(path);
		yield return w;
		if (string.IsNullOrEmpty(w.error))
		{
			File.WriteAllBytes(string.Format("{0}/{1}/SceneRoot_{2}_config.ab", Datapath, GetPathForCurrentPlatform(), scene), w.bytes);
			if (!CachedScenes.ContainsKey(scene))
			{
				CachedScenes.Add(scene, w.assetBundle);
			}
			else
			{
				CachedScenes[scene] = w.assetBundle;
			}
			Instance.callEvent(0, Object.Instantiate(CachedScenes[scene].mainAsset));
		}
		else
		{
			GameObject sceneConf = GlobalLoad.GetObject(string.Format("Export/SceneRoot_{0}_config", scene), string.Empty);
			if ((bool)sceneConf)
			{
				Instance.callEvent(0, Object.Instantiate(sceneConf));
			}
			else
			{
				Instance.LogWarning(string.Format("cant load scene config {0}: {1}", scene, w.error));
			}
		}
	}
}
