using System.Collections.Generic;
using System.IO;
using Nekki.Vector.Core.DataValidation;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core
{
	public static class UpdateUtils
	{
		private const string _PatchExtension = ".diff";

		private static Dictionary<string, string> _HashesUpdate = new Dictionary<string, string>();

		private static string _WorkFolder;

		public static bool ProcessUpdateFromPackage(byte[] p_data, int p_version = -1)
		{
			_WorkFolder = VectorPaths.TempExternal;
			VectorPaths.ClearTempFolder();
			if (!CompressUtils.DecompressToDirectory(p_data, _WorkFolder))
			{
				return false;
			}
			if (!CheckUpdate())
			{
				VectorPaths.ClearTempFolder();
				return false;
			}
			ProcessUpdate();
			ResourcesValidator.UpdateValidationHashes(_HashesUpdate, p_version);
			_HashesUpdate.Clear();
			VectorPaths.ClearTempFolder();
			UserUpdater.TryUpdateUserByDownloadContent();
			return true;
		}

		public static bool ProcessUpdateFromFolder(string p_updateDir)
		{
			_WorkFolder = p_updateDir;
			if (!CheckUpdate())
			{
				return false;
			}
			ProcessUpdate();
			ResourcesValidator.UpdateValidationHashes(_HashesUpdate);
			_HashesUpdate.Clear();
			UserUpdater.TryUpdateUserByDownloadContent();
			return true;
		}

		private static void ProcessUpdate()
		{
			DebugUtils.Log("[UpdateUtils]: begin process update:");
			_HashesUpdate.Clear();
			DirectoryInfo directoryInfo = new DirectoryInfo(_WorkFolder);
			FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
			if (files != null && files.Length > 0)
			{
				int startIndex = directoryInfo.GetFullName().Length + 1;
				FileInfo[] array = files;
				foreach (FileInfo fileInfo in array)
				{
					string text = fileInfo.GetFullName().Substring(startIndex);
					if (fileInfo.Extension == ".diff")
					{
						ProcessPatch(text);
						UpdateHash(text.Replace(".diff", string.Empty));
					}
					else
					{
						ProcessFile(text);
						UpdateHash(text, true);
					}
				}
			}
			DebugUtils.Log("[UpdateUtils]: end process update!");
		}

		private static bool CheckUpdate()
		{
			if (!Directory.Exists(_WorkFolder))
			{
				DebugUtils.Log(string.Format("[UpdateUtils]: Directory - '{0}' don't exist, no patch to open!", _WorkFolder));
				return false;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(_WorkFolder);
			FileInfo[] files = directoryInfo.GetFiles("*.diff", SearchOption.AllDirectories);
			if (files != null && files.Length > 0)
			{
				int startIndex = directoryInfo.GetFullName().Length + 1;
				FileInfo[] array = files;
				foreach (FileInfo p_file in array)
				{
					string text = p_file.GetFullName().Substring(startIndex).Replace(".diff", string.Empty);
					if (!IsFileToPatchExists(text))
					{
						DebugUtils.LogError("[UpdateUtils]: can't find file to patch - " + text);
						return false;
					}
				}
			}
			return true;
		}

		private static bool IsFileToPatchExists(string p_filename)
		{
			if (VectorPaths.UsingResources)
			{
				return FileUtils.FileExistsInResources(VectorPaths.GameData + "/" + p_filename) || File.Exists(VectorPaths.ExternalCachedGameData + "/" + p_filename);
			}
			return File.Exists(VectorPaths.GameData + "/" + p_filename) || File.Exists(VectorPaths.ExternalCachedGameData + "/" + p_filename);
		}

		private static void ProcessPatch(string p_patchFilename)
		{
			string text = p_patchFilename.Replace(".diff", string.Empty);
			string text2 = VectorPaths.ExternalCachedGameData + "/" + text;
			string text3 = ((!File.Exists(text2)) ? ResourceManager.GetText(VectorPaths.GameData + "/" + text) : ResourceManager.GetTextFromExternal(text2));
			if (string.IsNullOrEmpty(text3))
			{
				DebugUtils.LogError("[UpdateUtils]: file to patch is null or empty - " + text);
				return;
			}
			PatchUtility.ApplyPatchToText(_WorkFolder + "/" + p_patchFilename, text3, text2);
			DebugUtils.Log("[UpdateUtils]: \tprocess patch - " + text);
		}

		private static void ProcessFile(string p_filename)
		{
			CopyFile(_WorkFolder + "/" + p_filename, VectorPaths.ExternalCachedGameData + "/" + p_filename);
			DebugUtils.Log("[UpdateUtils]: \tprocess file - " + p_filename);
		}

		private static void CopyFile(string p_sourceFile, string p_targetFile)
		{
			FileUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(p_targetFile));
			File.Copy(p_sourceFile, p_targetFile, true);
		}

		private static void UpdateHash(string p_filename, bool p_isNew = false)
		{
			if (p_isNew || ResourcesValidator.IsFileSupportValidation(p_filename))
			{
				string value = MD5Utils.MD5HashFile(VectorPaths.ExternalCachedGameData + "/" + p_filename, ResourcesValidator.Salt);
				_HashesUpdate.Add(p_filename, value);
			}
		}
	}
}
