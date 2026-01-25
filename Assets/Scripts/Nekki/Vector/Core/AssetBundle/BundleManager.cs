using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.AssetBundle
{
	public static class BundleManager
	{
		public static readonly string Salt;

		public static readonly byte[] Key;

		public static readonly byte[] IV;

		private static List<BundleRef> _Bundles;

		private static List<BundleRequest> _Requests;

		private static Dictionary<string, BundleRef> _BundleByAssetName;

		private static bool _IsBundleByAssetNameDirty;

		public static Action<string> OnBundleDownloadStarted;

		public static Action<string, float> OnBundleDownloadProgress;

		public static Action<string, bool> OnBundleDownloadFinished;

		private static BundleRef _LoadedBundle;

		public static List<BundleRequest> Requests
		{
			get
			{
				return _Requests;
			}
		}

		public static int RequestsCount
		{
			get
			{
				return _Requests.Count;
			}
		}

		public static bool IsUpdateAvailable
		{
			get
			{
				return _Requests.Count > 0;
			}
		}

		public static bool IsRequiredUpdateAvailable
		{
			get
			{
				return _Requests.Find((BundleRequest p_request) => p_request.IsRequired) != null;
			}
		}

		public static long RequestsTotalContentLength
		{
			get
			{
				if (_Requests.Count == 0)
				{
					return 0L;
				}
				UpdateRequestsContentLength();
				long num = 0L;
				foreach (BundleRequest request in _Requests)
				{
					num += request.ContentLength;
				}
				return num;
			}
		}

		public static float RequestsTotalContentLengthInMb
		{
			get
			{
				return (float)RequestsTotalContentLength / 1024f / 1024f;
			}
		}

		static BundleManager()
		{
			Salt = "YNEg6vWZteeA2yH4Vr^7HpZ_7Nn%RQCTj=PuhXtn*N-TncxgXh&TLwMrb@3C";
			Key = new byte[32]
			{
				102, 60, 63, 64, 57, 34, 228, 113, 112, 229,
				101, 58, 94, 62, 10, 140, 216, 57, 45, 208,
				99, 140, 130, 160, 172, 122, 69, 186, 142, 47,
				185, 60
			};
			IV = new byte[16]
			{
				160, 63, 142, 208, 109, 209, 49, 149, 5, 153,
				212, 159, 99, 150, 69, 80
			};
			_Bundles = new List<BundleRef>();
			_Requests = new List<BundleRequest>();
			_BundleByAssetName = new Dictionary<string, BundleRef>();
			OnBundleDownloadStarted = delegate
			{
			};
			OnBundleDownloadProgress = delegate
			{
			};
			OnBundleDownloadFinished = delegate
			{
			};
			ApplicationController.OnAppUpdateCallBack += OnUpdate;
		}

		public static bool IsBundleExists(string p_bundleId)
		{
			return _Bundles.Find((BundleRef p_bundle) => p_bundle.BundleId == p_bundleId) != null;
		}

		public static bool IsRequestExists(string p_bundleId)
		{
			return _Requests.Find((BundleRequest p_request) => p_request.BundleId == p_bundleId) != null;
		}

		private static void OnUpdate()
		{
			if (_IsBundleByAssetNameDirty)
			{
				RegisterAssets();
			}
		}

		public static void Load(XmlNode p_rootNode)
		{
			Reset();
			XmlNode xmlNode = p_rootNode["Bundles"];
			if (xmlNode == null)
			{
				return;
			}
			XmlNode xmlNode2 = xmlNode["Content"];
			if (xmlNode2 != null)
			{
				foreach (XmlNode childNode in xmlNode2.ChildNodes)
				{
					_Bundles.Add(new BundleRef(childNode));
				}
			}
			XmlNode xmlNode3 = xmlNode["Requests"];
			if (xmlNode3 != null)
			{
				foreach (XmlNode childNode2 in xmlNode3.ChildNodes)
				{
					_Requests.Add(BundleRequest.CreateInProgress(childNode2));
				}
			}
			RegisterAssets();
			UpdateRequestsContentLength();
		}

		public static void Save(XmlNode p_rootNode)
		{
			if (_Bundles.Count == 0 && _Requests.Count == 0)
			{
				return;
			}
			XmlElement xmlElement = p_rootNode.OwnerDocument.CreateElement("Bundles");
			p_rootNode.AppendChild(xmlElement);
			if (_Bundles.Count > 0)
			{
				XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement("Content");
				xmlElement.AppendChild(xmlElement2);
				foreach (BundleRef bundle in _Bundles)
				{
					bundle.Save(xmlElement2);
				}
			}
			if (_Requests.Count <= 0)
			{
				return;
			}
			XmlElement xmlElement3 = xmlElement.OwnerDocument.CreateElement("Requests");
			xmlElement.AppendChild(xmlElement3);
			foreach (BundleRequest request in _Requests)
			{
				request.Save(xmlElement3);
			}
		}

		private static void Reset()
		{
			UnloadLoadedBundle(true);
			_Bundles.Clear();
			_BundleByAssetName.Clear();
			_Requests.Clear();
			_IsBundleByAssetNameDirty = false;
		}

		private static void SaveAllData()
		{
			DataLocal.Current.Save(true);
			DataLocal.Current.SaveLocalBackup();
		}

		private static void RegisterAssets()
		{
			_IsBundleByAssetNameDirty = false;
			_BundleByAssetName.Clear();
			foreach (BundleRef bundle in _Bundles)
			{
				bundle.RegisterAssets(_BundleByAssetName);
			}
		}

		public static void RunRequest()
		{
			if (!BundleRequest.IsRequesrsIsRunning && _Requests.Count > 0)
			{
				UnloadLoadedBundle(false);
				_Requests[0].Run();
			}
		}

		public static void CompleteRequest(BundleRequest p_request)
		{
			_Requests.Remove(p_request);
			if (p_request.Content != null)
			{
				AddOrUpdateBundleRef(p_request);
			}
			SaveAllData();
		}

		private static void AddOrUpdateBundleRef(BundleRequest p_request)
		{
			BundleRef bundleRef = _Bundles.Find((BundleRef p_ref) => p_ref.BundleId == p_request.BundleId);
			if (bundleRef == null)
			{
				bundleRef = new BundleRef();
				_Bundles.Add(bundleRef);
			}
			bundleRef.Update(p_request);
			_IsBundleByAssetNameDirty = true;
		}

		public static void RemoveBundleRef(BundleRef p_bundle)
		{
			_Bundles.Remove(p_bundle);
			_IsBundleByAssetNameDirty = true;
			SaveAllData();
		}

		public static bool CreateBundleRequestWithCheckingUpdate(string p_bundleId, int p_version, bool p_isRequired)
		{
			if (IsRequestExists(p_bundleId))
			{
				return false;
			}
			int actualBundleVersion = BundleUpdater.GetActualBundleVersion(p_bundleId, p_version);
			if (actualBundleVersion == -1)
			{
				return false;
			}
			return CreateBundleRequestInternal(p_bundleId, actualBundleVersion, p_isRequired);
		}

		public static bool CreateBundleRequest(string p_bundleId, int p_version, bool p_isRequired)
		{
			if (IsRequestExists(p_bundleId))
			{
				return false;
			}
			return CreateBundleRequestInternal(p_bundleId, p_version, p_isRequired);
		}

		private static bool CreateBundleRequestInternal(string p_bundleId, int p_version, bool p_isRequired)
		{
			BundleRequest item = BundleRequest.CreateInProgress(p_bundleId, p_version, p_isRequired);
			_Requests.Add(item);
			SaveAllData();
			return true;
		}

		public static void ResetRequests()
		{
			_Requests.Clear();
			SaveAllData();
		}

		public static void CheckBundlesUpdateAndValid()
		{
			if (!BundleUpdater.IsInited || _Bundles.Count == 0)
			{
				return;
			}
			DebugUtils.Log("[BundleManager]: CheckBundlesUpdateAndValid");
			List<BundleRef> list = new List<BundleRef>(_Bundles);
			foreach (BundleRef item in list)
			{
				item.CheckUpdateAndValid();
			}
		}

		public static void CheckBundlesValid()
		{
			if (_Bundles.Count == 0)
			{
				return;
			}
			DebugUtils.Log("[BundleManager]: CheckBundlesValid");
			List<BundleRef> list = new List<BundleRef>(_Bundles);
			foreach (BundleRef item in list)
			{
				item.CheckValid();
			}
		}

		public static void UpdateRequestsContentLength()
		{
			if (_Requests.Count == 0)
			{
				return;
			}
			bool flag = false;
			foreach (BundleRequest request in _Requests)
			{
				flag = flag || request.UpdateContentLength();
			}
			if (flag)
			{
				SaveAllData();
			}
		}

		public static T LoadAsset<T>(string p_assetName) where T : UnityEngine.Object
		{
			p_assetName = AssetBundleExtension.SimplifyAssetName(p_assetName);
			BundleRef value = null;
			if (!_BundleByAssetName.TryGetValue(p_assetName, out value))
			{
				return (T)null;
			}
			SwitchBundle(value);
			return value.LoadAsset<T>(p_assetName);
		}

		public static T[] LoadAssetWithSubAssets<T>(string p_assetName) where T : UnityEngine.Object
		{
			p_assetName = AssetBundleExtension.SimplifyAssetName(p_assetName);
			BundleRef value = null;
			if (!_BundleByAssetName.TryGetValue(p_assetName, out value))
			{
				return null;
			}
			SwitchBundle(value);
			return value.LoadAssetWithSubAssets<T>(p_assetName);
		}

		private static void SwitchBundle(BundleRef p_bundle)
		{
			if (p_bundle != null && _LoadedBundle != p_bundle)
			{
				if (_LoadedBundle != null)
				{
					_LoadedBundle.Unload(false);
				}
				DebugUtils.StartMem();
				_LoadedBundle = p_bundle;
				_LoadedBundle.Load();
				DebugUtils.StopMem("SwitchBundle: " + _LoadedBundle.BundleId);
			}
		}

		private static void UnloadLoadedBundle(bool p_unloadAllAssets = false)
		{
			if (_LoadedBundle != null)
			{
				_LoadedBundle.Unload(p_unloadAllAssets);
				_LoadedBundle = null;
			}
		}

		public static void UnloadAllAssets()
		{
			UnloadLoadedBundle(true);
		}
	}
}
