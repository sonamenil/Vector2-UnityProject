using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class MD5Utils
{
	public static string MD5HashFile(string p_fileName, string p_salt = null)
	{
		try
		{
			byte[] p_bytes = File.ReadAllBytes(p_fileName);
			return MD5HashBytes(p_bytes, p_salt);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return string.Empty;
	}

	public static string MD5HashString(string p_str, string p_salt = null)
	{
		using (MD5 mD = MD5.Create())
		{
			byte[] p_binaryArray = mD.ComputeHash(StringToByteArray(p_str));
			string text = ByteArrayToString(p_binaryArray);
			if (p_salt == null)
			{
				return text;
			}
			byte[] p_binaryArray2 = mD.ComputeHash(StringToByteArray(text + p_salt));
			return ByteArrayToString(p_binaryArray2);
		}
	}

	public static string MD5HashBytes(byte[] p_bytes, string p_salt = null)
	{
		using (MD5 mD = MD5.Create())
		{
			byte[] p_binaryArray = mD.ComputeHash(p_bytes);
			string text = ByteArrayToString(p_binaryArray);
			if (p_salt == null)
			{
				return text;
			}
			byte[] p_binaryArray2 = mD.ComputeHash(StringToByteArray(text + p_salt));
			return ByteArrayToString(p_binaryArray2);
		}
	}

	public static bool CheckFileHash(string p_fileName, string p_hash, string p_salt = null)
	{
		return MD5HashFile(p_fileName, p_salt) == p_hash;
	}

	public static bool CheckStringHash(string p_str, string p_hash, string p_salt = null)
	{
		return MD5HashString(p_str, p_salt) == p_hash;
	}

	public static bool CheckBytesHash(byte[] p_bytes, string p_hash, string p_salt = null)
	{
		return MD5HashBytes(p_bytes, p_salt) == p_hash;
	}

	public static string ByteArrayToString(byte[] p_binaryArray)
	{
		string text = BitConverter.ToString(p_binaryArray);
		return text.Replace("-", string.Empty);
	}

	public static byte[] StringToByteArray(string p_str)
	{
		return Encoding.UTF8.GetBytes(p_str);
	}
}
