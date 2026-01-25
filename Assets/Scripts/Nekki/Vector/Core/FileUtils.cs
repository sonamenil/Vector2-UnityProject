using System;
using System.IO;
using System.Linq;
using Nekki.Vector.Core.DataValidation;
using UnityEngine;

namespace Nekki.Vector.Core
{
	public static class FileUtils
	{
		public static readonly byte[] CorruptedFilesKey = new byte[32]
		{
			133, 5, 55, 142, 90, 88, 219, 154, 183, 173,
			42, 194, 97, 197, 86, 166, 57, 44, 206, 162,
			191, 209, 95, 89, 67, 22, 3, 102, 78, 22,
			170, 186
		};

		public static readonly byte[] CorruptedFilesIV = new byte[16]
		{
			146, 111, 80, 32, 53, 107, 130, 252, 20, 104,
			183, 19, 140, 193, 11, 158
		};

		public static bool FileExists(string p_file)
		{
			return File.Exists(p_file);
		}

		public static bool FileExistsInResources(string p_file)
		{
			p_file = RemoveExtension(p_file.TrimStart('/'));
			return Resources.Load(RemoveExtension(p_file)) != null;
		}

		public static void CopyFile(string p_from, string p_to, bool p_owerwrite = true)
		{
			File.Copy(p_from, p_to, p_owerwrite);
		}

		public static void CopyFileAndEncrypt(byte[] p_key, byte[] p_iv, string p_from, string p_to, bool p_forceExternal = false)
		{
			try
			{
				AESUtils.EncryptFile(p_key, p_iv, p_from, p_to, p_forceExternal);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
			}
		}

		public static void CopyFileAndHash(string p_from, string p_to)
		{
			CopyFile(p_from, p_to);
			UserDataValidator.CopyFileHash(p_from, p_to);
		}

		public static void CopyFileAndGenerateHash(string p_from, string p_to)
		{
			CopyFile(p_from, p_to);
			UserDataValidator.UpdateFileHash(p_to);
		}

		public static void DeleteFile(string p_file)
		{
			if (FileExists(p_file))
			{
				File.Delete(p_file);
			}
		}

		public static void DeleteFileAndHash(string p_file)
		{
			if (FileExists(p_file))
			{
				File.Delete(p_file);
				UserDataValidator.DeleteFileHash(p_file);
			}
		}

		public static void CopyFileFromResourcesToExternal(string p_from, string p_to)
		{
			try
			{
				byte[] binary = ResourceManager.GetBinary(p_from);
				File.WriteAllBytes(p_to, binary);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void MoveToCorruptedFileAndHash(string p_file)
		{
			if (FileExists(p_file))
			{
				if (!Directory.Exists(VectorPaths.CorruptedFiles))
				{
					Directory.CreateDirectory(VectorPaths.CorruptedFiles);
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(p_file);
				string extension = Path.GetExtension(p_file);
				int num = Directory.GetFiles(VectorPaths.CorruptedFiles, "user*.xml").Length;
				string p_to = VectorPaths.CorruptedFiles + "/" + fileNameWithoutExtension + "_" + num + extension;
				CopyFileAndEncrypt(CorruptedFilesKey, CorruptedFilesIV, p_file, p_to, true);
				UserDataValidator.CopyFileHash(p_file, p_to);
				File.Delete(p_file);
				UserDataValidator.DeleteFileHash(p_file);
			}
		}

		public static void CopyFilesRecursively(string p_source, string p_target, bool p_overwrite = true)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(p_source);
			DirectoryInfo directoryInfo2 = new DirectoryInfo(p_target);
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo3 in directories)
			{
				CopyFilesRecursively(directoryInfo3.GetFullName(), directoryInfo2.CreateSubdirectory(directoryInfo3.Name).GetFullName());
			}
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.CopyTo(Path.Combine(directoryInfo2.GetFullName(), fileInfo.Name), p_overwrite);
			}
		}

		public static void CopySpecificFilesRecursively(string p_source, string p_target, string[] p_supportedExt, bool p_overwrite = true)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(p_source);
			DirectoryInfo directoryInfo2 = new DirectoryInfo(p_target);
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo3 in directories)
			{
				CopySpecificFilesRecursively(directoryInfo3.GetFullName(), directoryInfo2.CreateSubdirectory(directoryInfo3.Name).GetFullName(), p_supportedExt);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (p_supportedExt.Contains(fileInfo.Extension))
				{
					fileInfo.CopyTo(Path.Combine(directoryInfo2.GetFullName(), fileInfo.Name), p_overwrite);
				}
			}
		}

		public static void CreateDirectoryIfNotExists(string p_path)
		{
			if (!Directory.Exists(p_path))
			{
				Directory.CreateDirectory(p_path);
			}
		}

		public static void SafeDeleteDirectory(string p_path, bool p_recursive = true)
		{
			if (Directory.Exists(p_path))
			{
				Directory.Delete(p_path, p_recursive);
			}
		}

		public static void RecreateDirectory(string p_path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(p_path);
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete(true);
			}
			directoryInfo.Create();
		}

		public static string RemoveExtension(string p_path)
		{
			if (Path.HasExtension(p_path))
			{
				return Path.ChangeExtension(p_path, null);
			}
			return p_path;
		}
	}
}
