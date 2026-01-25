using Nekki.Vector.Core;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Audio.Internal;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class ResourceManager
{
	public static Texture2D GetTexture(string p_fileName, bool p_compressed = false, bool p_filtering = false)
	{
		Texture2D texture2D;
		if (VectorPaths.UsingResources)
		{
			texture2D = GetTextureFromResources(p_fileName);
		}
		else
		{
			texture2D = GetTextureFromExternal(p_fileName);
			if (p_compressed)
			{
				texture2D.Compress(true);
			}
			if (p_filtering)
			{
				texture2D.filterMode = FilterMode.Bilinear;
				texture2D.anisoLevel = 1;
			}
			else
			{
				texture2D.filterMode = FilterMode.Point;
				texture2D.anisoLevel = 0;
			}
		}
		return texture2D;
	}

	public static Texture2D GetTextureFromResources(string p_fileName)
	{
		Texture2D texture2D = BundleManager.LoadAsset<Texture2D>(p_fileName);
		if (texture2D != null)
		{
			return texture2D;
		}
		return GlobalLoad.GetTexture(p_fileName, string.Empty);
	}

	public static Texture2D GetTextureFromExternal(string p_fileName)
	{
		byte[] array;
		using (FileStream fileStream = new FileStream(p_fileName, FileMode.Open, FileAccess.Read))
		{
			array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
		}
		Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
		texture2D.LoadImage(array);
		return texture2D;
	}

	public static AudioClip GetAudioClip(string p_fileName)
	{
		if (VectorPaths.UsingResources)
		{
			return GetAudioClipFromResources(p_fileName);
		}
		return GetAudioClipFromExternal(p_fileName);
	}

	public static AudioClip GetAudioClipFromResources(string p_fileName)
	{
		AudioClip audioClip = BundleManager.LoadAsset<AudioClip>(p_fileName);
		if (audioClip != null)
		{
			return audioClip;
		}
		return GlobalLoad.GetAudioClip(p_fileName, string.Empty);
	}

	public static AudioClip GetAudioClipFromExternal(string p_fileName)
	{
        using (var www = UnityWebRequestMultimedia.GetAudioClip($"file:///{p_fileName}", AudioType.UNKNOWN))
        {
            www.SendWebRequest();
            while (!www.isDone && string.IsNullOrEmpty(www.error)) 
			{ 
			}
            if (!string.IsNullOrEmpty(www.error))
            {
                return null;
            }
            return DownloadHandlerAudioClip.GetContent(www);
        }
    }

	public static byte[] GetBinary(string p_fileName)
	{
		if (VectorPaths.UsingResources)
		{
			return GlobalLoad.GetBytes(p_fileName, string.Empty);
		}
		FileStream fileStream = new FileStream(p_fileName, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, (int)fileStream.Length);
		fileStream.Close();
		return array;
	}

	public static string GetText(string p_path, bool p_forcedExternal = false)
	{
		if (VectorPaths.UsingResources && !p_forcedExternal)
		{
			return GetTextFromResources(p_path);
		}
		return GetTextFromExternal(p_path);
	}

	public static string GetTextFromResources(string p_path)
	{
		p_path = p_path.TrimStart('\\', '/');
		string text = VectorPaths.CombineExternalCachedPath(p_path);
		if (File.Exists(text))
		{
			return GetTextFromExternal(text);
		}
		p_path = RemoveExtension(p_path);
		TextAsset textAsset = Resources.Load<TextAsset>(p_path);
		return (!textAsset) ? string.Empty : textAsset.text;
	}

	public static string GetTextFromExternal(string p_path)
	{
		if (p_path.StartsWith(VectorPaths.GameData))
		{
			string path = p_path.Replace(VectorPaths.GameData, VectorPaths.ExternalCachedGameData);
			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
			if (File.Exists(p_path))
			{
				return File.ReadAllText(p_path);
			}
			return null;
		}
		if (File.Exists(p_path))
		{
			return File.ReadAllText(p_path);
		}
		return null;
	}

	private static string RemoveExtension(string p_path)
	{
		if (Path.HasExtension(p_path))
		{
			return Path.ChangeExtension(p_path, null);
		}
		return p_path;
	}
}
