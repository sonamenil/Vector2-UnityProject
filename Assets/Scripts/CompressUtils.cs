using System;
using System.IO;
using System.Text;
using Unity.IO.Compression;
using UnityEngine;

public static class CompressUtils
{
	public static void CompressDirectory(string p_inDir, string p_outGz, params string[] p_filters)
	{
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(p_inDir);
			FileInfo[] array = ((p_filters == null || p_filters.Length <= 0) ? directoryInfo.GetFiles("*.*", SearchOption.AllDirectories) : directoryInfo.GetFilesByExtensions(SearchOption.AllDirectories, p_filters));
			int length = directoryInfo.GetFullName().Length;
			using (FileStream stream = new FileStream(p_outGz, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (GZipStream p_gzip = new GZipStream(stream, CompressionMode.Compress))
				{
					FileInfo[] array2 = array;
					foreach (FileInfo p_file in array2)
					{
						CompressDirFile(directoryInfo.GetFullName(), p_file.GetFullName().Substring(length), p_gzip);
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void CompressDirFile(string p_dir, string p_relativePath, GZipStream p_gzip)
	{
		char[] array = p_relativePath.ToCharArray();
		p_gzip.Write(BitConverter.GetBytes(array.Length), 0, 4);
		char[] array2 = array;
		foreach (char value in array2)
		{
			p_gzip.Write(BitConverter.GetBytes(value), 0, 2);
		}
		byte[] array3 = File.ReadAllBytes(Path.Combine(p_dir, p_relativePath));
		p_gzip.Write(BitConverter.GetBytes(array3.Length), 0, 4);
		p_gzip.Write(array3, 0, array3.Length);
	}

	public static void DecompressToDirectory(string p_gzip, string p_outDir)
	{
		try
		{
			using (FileStream stream = new FileStream(p_gzip, FileMode.Open, FileAccess.Read, FileShare.None))
			{
				using (GZipStream p_gzip2 = new GZipStream(stream, CompressionMode.Decompress, true))
				{
					while (DecompressDirFile(p_outDir, p_gzip2))
					{
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static bool DecompressToDirectory(byte[] p_bytes, string p_outDir)
	{
		try
		{
			using (MemoryStream stream = new MemoryStream(p_bytes))
			{
				using (GZipStream p_gzip = new GZipStream(stream, CompressionMode.Decompress, true))
				{
					while (DecompressDirFile(p_outDir, p_gzip))
					{
					}
				}
			}
			return true;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
	}

	private static bool DecompressDirFile(string p_dir, GZipStream p_gzip)
	{
		byte[] array = new byte[4];
		int num = p_gzip.Read(array, 0, 4);
		if (num < 4)
		{
			return false;
		}
		int num2 = BitConverter.ToInt32(array, 0);
		array = new byte[2];
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < num2; i++)
		{
			p_gzip.Read(array, 0, 2);
			char value = BitConverter.ToChar(array, 0);
			stringBuilder.Append(value);
		}
		string path = stringBuilder.ToString();
		array = new byte[4];
		p_gzip.Read(array, 0, 4);
		int num3 = BitConverter.ToInt32(array, 0);
		array = new byte[num3];
		p_gzip.Read(array, 0, array.Length);
		string path2 = Path.Combine(p_dir, path);
		string directoryName = Path.GetDirectoryName(path2);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		using (FileStream fileStream = new FileStream(path2, FileMode.Create, FileAccess.Write, FileShare.None))
		{
			fileStream.Write(array, 0, num3);
		}
		return true;
	}

	public static void CompressFile(string p_in, string p_out)
	{
		try
		{
			byte[] bytes = CompressData(File.ReadAllBytes(p_in));
			File.WriteAllBytes(p_out, bytes);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void DecompressFile(string p_in, string p_out)
	{
		try
		{
			byte[] bytes = DecompressData(File.ReadAllBytes(p_in));
			File.WriteAllBytes(p_out, bytes);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static byte[] CompressData(byte[] p_data)
	{
		try
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gZipStream.Write(p_data, 0, p_data.Length);
				}
				return memoryStream.ToArray();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public static byte[] DecompressData(byte[] p_data)
	{
		try
		{
			using (GZipStream gZipStream = new GZipStream(new MemoryStream(p_data), CompressionMode.Decompress))
			{
				byte[] array = new byte[4096];
				using (MemoryStream memoryStream = new MemoryStream())
				{
					int num = 0;
					do
					{
						num = gZipStream.Read(array, 0, 4096);
						if (num > 0)
						{
							memoryStream.Write(array, 0, num);
						}
					}
					while (num > 0);
					return memoryStream.ToArray();
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}
}
