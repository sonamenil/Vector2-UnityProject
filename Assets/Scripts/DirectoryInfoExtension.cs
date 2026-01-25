using System.Collections.Generic;
using System.IO;

public static class DirectoryInfoExtension
{
	public static FileInfo[] GetFilesByExtensions(this DirectoryInfo p_dir, params string[] p_extensions)
	{
		if (p_extensions == null)
		{
			return p_dir.GetFiles();
		}
		List<FileInfo> list = new List<FileInfo>();
		foreach (string searchPattern in p_extensions)
		{
			list.AddRange(p_dir.GetFiles(searchPattern));
		}
		return list.ToArray();
	}

	public static FileInfo[] GetFilesByExtensions(this DirectoryInfo p_dir, SearchOption p_option, params string[] p_extensions)
	{
		if (p_extensions == null)
		{
			return p_dir.GetFiles();
		}
		List<FileInfo> list = new List<FileInfo>();
		foreach (string searchPattern in p_extensions)
		{
			list.AddRange(p_dir.GetFiles(searchPattern, p_option));
		}
		return list.ToArray();
	}

	public static string GetFullName(this DirectoryInfo p_dir)
	{
		return p_dir.FullName.Replace("\\", "/");
	}
}
