using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class AESUtils
{
	public static void EncryptBytesToFile(byte[] p_data, byte[] p_key, byte[] p_IV, string p_destination)
	{
		byte[] bytes = EncryptBytes(p_data, p_key, p_IV);
		File.WriteAllBytes(p_destination, bytes);
	}

	public static byte[] DecryptFileToBytes(byte[] p_key, byte[] p_IV, string p_from, bool p_forcedExternal = false)
	{
		try
		{
			byte[] p_data = ((!p_forcedExternal) ? ResourceManager.GetBinary(p_from) : File.ReadAllBytes(p_from));
			return DecryptBytes(p_data, p_key, p_IV);
		}
		catch
		{
			return null;
		}
	}

	public static void DecryptFile(byte[] p_key, byte[] p_IV, string p_from, string p_destination = null, bool p_forcedExternal = false)
	{
		if (p_destination == null)
		{
			p_destination = p_from;
		}
		byte[] p_data = ((!p_forcedExternal) ? ResourceManager.GetBinary(p_from) : File.ReadAllBytes(p_from));
		byte[] bytes = DecryptBytes(p_data, p_key, p_IV);
		File.WriteAllBytes(p_destination, bytes);
	}

	public static void EncryptFile(byte[] p_key, byte[] p_IV, string p_from, string p_destination = null, bool p_forcedExternal = false)
	{
		if (p_destination == null)
		{
			p_destination = p_from;
		}
		byte[] p_data = ((!p_forcedExternal) ? ResourceManager.GetBinary(p_from) : File.ReadAllBytes(p_from));
		byte[] bytes = EncryptBytes(p_data, p_key, p_IV);
		File.WriteAllBytes(p_destination, bytes);
	}

	public static byte[] EncryptBytes(byte[] p_data, byte[] p_key, byte[] p_IV)
	{
		try
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = p_key;
				aes.IV = p_IV;
				ICryptoTransform p_transform = aes.CreateEncryptor(aes.Key, aes.IV);
				return RunCryptography(p_data, p_transform);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return null;
	}

	public static byte[] DecryptBytes(byte[] p_data, byte[] p_key, byte[] p_IV)
	{
		try
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = p_key;
				aes.IV = p_IV;
				ICryptoTransform p_transform = aes.CreateDecryptor(aes.Key, aes.IV);
				return RunCryptography(p_data, p_transform);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return null;
	}

	private static byte[] RunCryptography(byte[] p_data, ICryptoTransform p_transform)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, p_transform, CryptoStreamMode.Write))
			{
				cryptoStream.Write(p_data, 0, p_data.Length);
				cryptoStream.FlushFinalBlock();
				return memoryStream.ToArray();
			}
		}
	}
}
