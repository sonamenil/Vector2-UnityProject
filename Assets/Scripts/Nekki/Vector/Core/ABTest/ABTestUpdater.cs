using System.IO;
using Nekki.Vector.Core.DataValidation;
using UnityEngine;

namespace Nekki.Vector.Core.ABTest
{
	public static class ABTestUpdater
	{
		private const int _MaxDownloadAttempt = 2;

		private const string _ContentFolder = "/content";

		private const string _ForwardFolder = "/forward";

		private const string _RevertFolder = "/revert";

		public static readonly byte[] Key = new byte[32]
		{
			91, 139, 94, 155, 191, 36, 162, 132, 140, 135,
			0, 118, 91, 102, 91, 225, 230, 183, 22, 239,
			129, 147, 46, 185, 7, 172, 237, 175, 233, 174,
			55, 72
		};

		public static readonly byte[] IV = new byte[16]
		{
			36, 226, 50, 224, 70, 229, 136, 212, 214, 9,
			128, 205, 163, 67, 84, 221
		};

		private static string _Group = string.Empty;

		private static string URL_UpdateContent
		{
			get
			{
				return URLCreator.Make(string.Format("ab_test/{0}.bin", _Group));
			}
		}

		public static string ContentFolder
		{
			get
			{
				return VectorPaths.ABTestExternal + "/content";
			}
		}

		public static void LoadResources(string newGroup, string hash)
		{
			_Group = newGroup;
			LoadUpdateContent(hash);
		}

		public static bool ApplyGroupChanges(string from, string to)
		{
			Debug.Log(string.Format("[ABTestUpdater] ApplyGroup from '{0}' to '{1}'", from, to));
			if (string.IsNullOrEmpty(from))
			{
				return ApplyContent(string.Format("{0}/{1}{2}", ContentFolder, to, "/forward"));
			}
			if (string.IsNullOrEmpty(to))
			{
				return ApplyContent(string.Format("{0}/{1}{2}", ContentFolder, from, "/revert"));
			}
			bool flag = true;
			flag = ApplyContent(string.Format("{0}/{1}{2}", ContentFolder, from, "/revert")) && flag;
			return ApplyContent(string.Format("{0}/{1}{2}", ContentFolder, to, "/forward")) && flag;
		}

		private static void OnLoadFail()
		{
		}

		private static void LoadUpdateContent(string p_hash, int p_downloadAttempt = 0)
		{
			if (p_downloadAttempt >= 2)
			{
				OnLoadFail();
				return;
			}
			InternetUtils.DownloadFileResult downloadFileResult = InternetUtils.DownloadFile(URL_UpdateContent);
			if (downloadFileResult.IsError)
			{
				LoadUpdateContent(p_hash, p_downloadAttempt + 1);
			}
			else if (MD5Utils.CheckBytesHash(downloadFileResult.Data, p_hash, ResourcesValidator.Salt))
			{
				if (!ExtractContent(downloadFileResult.Data))
				{
					LoadUpdateContent(p_hash, p_downloadAttempt + 1);
				}
			}
			else
			{
				LoadUpdateContent(p_hash, p_downloadAttempt + 1);
			}
		}

		private static bool ExtractContent(byte[] p_data)
		{
			string text = string.Format("{0}/{1}", ContentFolder, _Group);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			Debug.Log("ExtractContent path=" + text);
			return CompressUtils.DecompressToDirectory(p_data, text);
		}

		private static bool ApplyContent(string folder)
		{
			bool flag = UpdateUtils.ProcessUpdateFromFolder(folder);
			if (flag && Directory.Exists(folder))
			{
				Directory.Delete(folder, true);
			}
			return flag;
		}

		public static void ClearResources(string group)
		{
			string path = string.Format("{0}/{1}", ContentFolder, group);
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}
	}
}
