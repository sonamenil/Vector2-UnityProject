using System.IO;

public static class FileInfoExtension
{
	public static string GetDirectoryName(this FileInfo p_file)
	{
		return p_file.DirectoryName.Replace("\\", "/");
	}

	public static string GetFullName(this FileInfo p_file)
	{
		return p_file.FullName.Replace("\\", "/");
	}
}
