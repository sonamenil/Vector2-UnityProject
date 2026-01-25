using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Nekki.Zip;
using UnityEngine;

public class NekkiAssetDownloader
{
	public class DownloadInfo
	{
		public BundleConfig.Line Line;

		public string ZipFile;

		public string BundleFile;
	}

	public class BundleData
	{
		public TextAsset Config;

		public AssetBundle Bundle;

		public string Name;

		public override string ToString()
		{
			return Name + ": " + Config;
		}
	}

	public delegate void OnLoadDoneEventHanler(List<BundleData> bundles);

	public static List<BundleData> _loaded = new List<BundleData>();

	protected List<AssetBundle> _bundles;

	protected Queue<BundleConfig.Line> _fileUpdated;

	private int _bundlesInProcess;

	protected List<DownloadInfo> _unzipQueue;

	protected static NekkiAssetDownloader _instance;

	protected NekkiHelperWWW _wwwHelper;

	protected string _rootURL = string.Empty;

	protected int _version;

	protected int _sync;

	protected bool _isDownloading;

	protected Dictionary<string, int> _versionInfo;

	private WWW _current;

	private float _progress;

	public static float Progress { get; private set; }

	public static string CurrentExtract { get; private set; }

	public static bool NoneToLoad { get; private set; }

	public static int Sheduled { get; private set; }

	public static int Done { get; private set; }

	public static bool IsExtracting { get; private set; }

	public static string ExtractingState { get; private set; }

	public static NekkiAssetDownloader Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new NekkiAssetDownloader();
			}
			return _instance;
		}
	}

	public bool IsDownloading
	{
		get
		{
			return _isDownloading;
		}
	}

	public static event OnLoadDoneEventHanler LoadDone;

	public NekkiAssetDownloader()
	{
		_bundles = new List<AssetBundle>();
		_fileUpdated = new Queue<BundleConfig.Line>();
		_unzipQueue = new List<DownloadInfo>();
		_wwwHelper = NekkiHelperWWW.Instance;
	}

	public static void Delete()
	{
		_instance = null;
	}

	protected static void OnLoadDone()
	{
		List<BundleData> list = new List<BundleData>();
		NekkiAssetImporter.Init(list);
		OnLoadDoneEventHanler loadDone = NekkiAssetDownloader.LoadDone;
		if (loadDone != null)
		{
			loadDone(list);
		}
	}

	protected static void OnLoadDone(List<BundleData> data)
	{
		NekkiAssetImporter.Init(data);
		OnLoadDoneEventHanler loadDone = NekkiAssetDownloader.LoadDone;
		if (loadDone != null)
		{
			loadDone(data);
		}
	}

	public void Init(string rootURL, Action onDone)
	{
		NekkiAssetDownloader.LoadDone = (OnLoadDoneEventHanler)Delegate.Combine(NekkiAssetDownloader.LoadDone, (OnLoadDoneEventHanler)delegate
		{
			onDone();
		});
		Init(rootURL);
	}

	public void Init(string rootURL, Action<List<BundleData>> onDone)
	{
		NekkiAssetDownloader.LoadDone = (OnLoadDoneEventHanler)Delegate.Combine(NekkiAssetDownloader.LoadDone, (OnLoadDoneEventHanler)delegate(List<BundleData> bundles)
		{
			onDone(bundles);
		});
		Init(rootURL);
	}

	public void Init(string rootURL)
	{
		_rootURL = rootURL;
		_isDownloading = true;
		_wwwHelper.ExecWWW(Timestamp(_rootURL), ProcessConfig);
	}

	public static string Timestamp(string url)
	{
		return string.Format("{0}?time={1}", url, DateTime.Now.Ticks);
	}

	private void ProcessConfig(object url, WWW www = null)
	{
		if (www == null)
		{
			DownloadBundles();
			return;
		}
		_fileUpdated = BundleConfig.Copmare(www.text);
		DownloadBundles();
	}

	private void DownloadBundles()
	{
		if (_fileUpdated.Count == 0)
		{
			NoneToLoad = true;
		}
		_sync = _fileUpdated.Count;
		Sheduled = _sync;
		_bundlesInProcess = _fileUpdated.Count;
		NekkiHelperWWW.Instance.StartCoroutine(LoadBundle());
		NekkiHelperWWW.Instance.StartCoroutine(CheckCurrent());
		if (NoneToLoad)
		{
			OnLoadDone();
		}
	}

	private IEnumerator LoadBundle()
	{
		while (_fileUpdated.Count > 0)
		{
			BundleConfig.Line line = _fileUpdated.Dequeue();
			using (WWW www = new WWW(line.ConfigURL))
			{
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					AdvLog.LogError(www.error + www.url);
				}
				else
				{
					Directory.CreateDirectory(GlobalPaths.BundlesPath);
					string savePath = string.Format("{0}/{1}", GlobalPaths.BundlesPath, Path.GetFileName(line.ConfigURL.Split('?')[0]));
					string confPath = Path.ChangeExtension(savePath, "config");
					File.WriteAllText(confPath, www.text);
				}
			}
			using (WWW www2 = new WWW(line.BinarryURL))
			{
				_current = www2;
				yield return www2;
				Directory.CreateDirectory(GlobalPaths.BundlesPath);
				string savePath2 = string.Format("{0}/{1}", GlobalPaths.BundlesPath, Path.GetFileName(line.BinarryURL.Split('?')[0]));
				File.WriteAllBytes(savePath2, www2.bytes);
				try
				{
					ExtractZipFile(savePath2);
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					AdvLog.LogException(ex2);
				}
				BundleConfig.Local.GetLocal.Update(line);
				File.Delete(savePath2);
				_current = null;
			}
			_bundlesInProcess--;
			Done++;
			if (_bundlesInProcess == 0)
			{
				OnLoadDone(_loaded);
			}
		}
	}

	private IEnumerator CheckCurrent()
	{
		while (_bundlesInProcess > 0)
		{
			yield return new WaitForEndOfFrame();
			Progress = 1f / (float)Sheduled * (float)Done + ((_current != null) ? (1f / (float)Sheduled * _current.progress) : 0f);
		}
		Progress = 1f;
	}

	public void ExtractZipFile(string archiveFilenameIn)
	{
		Compressor.Unzip(archiveFilenameIn, Path.ChangeExtension(archiveFilenameIn, "unity3d"));
	}
}
