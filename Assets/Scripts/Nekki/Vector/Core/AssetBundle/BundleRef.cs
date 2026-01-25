using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.AssetBundle
{
	public class BundleRef
	{
		private string _BundleId;

		private int _Version;

		private string _ContentHash;

		private bool _IsContentValid;

		private UnityEngine.AssetBundle _UnityBundle;

		public string ContentPath
		{
			get
			{
				return VectorPaths.BundlesExternal + "/" + _BundleId + ".bin";
			}
		}

		public string BundleId
		{
			get
			{
				return _BundleId;
			}
		}

		public int Version
		{
			get
			{
				return _Version;
			}
		}

		public string ContentHash
		{
			get
			{
				return _ContentHash;
			}
		}

		public bool IsContentValid
		{
			get
			{
				return _IsContentValid;
			}
		}

		public BundleRef(XmlNode p_node)
		{
			_BundleId = XmlUtils.ParseString(p_node.Attributes["Id"], string.Empty);
			_Version = XmlUtils.ParseInt(p_node.Attributes["Version"]);
			_ContentHash = XmlUtils.ParseString(p_node.Attributes["ContentHash"]);
			_IsContentValid = File.Exists(ContentPath) && MD5Utils.CheckFileHash(ContentPath, _ContentHash, BundleManager.Salt);
		}

		public BundleRef()
		{
		}

		public void Update(BundleRequest p_request)
		{
			_BundleId = p_request.BundleId;
			_Version = p_request.Version;
			_ContentHash = p_request.ContentHash;
			_IsContentValid = true;
			File.WriteAllBytes(ContentPath, p_request.Content);
		}

		public void Load()
		{
			if (_UnityBundle != null)
			{
				DebugUtils.LogFormat("[BundleRef]: trying to load already loaded bundle [{0}-{1}]!", _BundleId, _Version);
			}
			else
			{
				_UnityBundle = UnityEngine.AssetBundle.LoadFromFile(ContentPath);
			}
		}

		public void Unload(bool p_unloadAllAssets)
		{
			if (_UnityBundle == null)
			{
				DebugUtils.LogFormat("[BundleRef]: trying to unload already unloaded bundle [{0}-{1}]!", _BundleId, _Version);
			}
			else
			{
				_UnityBundle.Unload(p_unloadAllAssets);
				_UnityBundle = null;
			}
		}

		public void RegisterAssets(Dictionary<string, BundleRef> p_assetsDict)
		{
			if (!IsContentValid)
			{
				DebugUtils.LogFormat("[BundleRef]: RegisterAssets - content is invalid for bundle [{0}-{1}]!", _BundleId, _Version);
				return;
			}
			Load();
			string[] allSimplifiedAssetNames = _UnityBundle.GetAllSimplifiedAssetNames();
			string[] array = allSimplifiedAssetNames;
			foreach (string key in array)
			{
				if (p_assetsDict.ContainsKey(key))
				{
					p_assetsDict[key] = this;
				}
				else
				{
					p_assetsDict.Add(key, this);
				}
			}
			Unload(true);
		}

		public T LoadAsset<T>(string p_assetName) where T : Object
		{
			DebugUtils.StartTimer("BundleRef.LoadAsset");
			T result = _UnityBundle.LoadAsset<T>(p_assetName);
			DebugUtils.StopTimerWithMessage("BundleRef.LoadAsset: " + p_assetName, "BundleRef.LoadAsset");
			return result;
		}

		public T[] LoadAssetWithSubAssets<T>(string p_assetName) where T : Object
		{
			DebugUtils.StartTimer("BundleRef.LoadAssetWithSubAssets");
			T[] result = _UnityBundle.LoadAssetWithSubAssets<T>(p_assetName);
			DebugUtils.StopTimerWithMessage("BundleRef.LoadAssetWithSubAssets: " + p_assetName, "BundleRef.LoadAssetWithSubAssets");
			return result;
		}

		public void Save(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Bundle");
			xmlElement.SetAttribute("Id", _BundleId);
			xmlElement.SetAttribute("Version", _Version.ToString());
			if (!string.IsNullOrEmpty(_ContentHash))
			{
				xmlElement.SetAttribute("ContentHash", _ContentHash);
			}
			p_node.AppendChild(xmlElement);
		}

		public void CheckUpdateAndValid()
		{
			int num = BundleUpdater.CheckBundleUpdate(this);
			if (num != 0 || !IsContentValid)
			{
				if (num < 0)
				{
					Free();
				}
				else
				{
					BundleManager.CreateBundleRequest(_BundleId, num, true);
				}
			}
		}

		public void CheckValid()
		{
			if (!IsContentValid)
			{
				BundleManager.CreateBundleRequest(_BundleId, _Version, true);
			}
		}

		public void Free()
		{
			FileUtils.DeleteFile(ContentPath);
			BundleManager.RemoveBundleRef(this);
		}
	}
}
