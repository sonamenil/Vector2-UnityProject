using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GlobalLoad
{
	public class File
	{
		public string Name { get; private set; }

		public string Text { get; private set; }

		public byte[] Bytes { get; private set; }

		public File(string name, string text, byte[] bytes)
		{
			Bytes = bytes;
			Text = text;
			Name = name;
		}
	}

	private static string GetLocal(GlobalPaths.Path path, string file = "")
	{
		string text = ((!string.IsNullOrEmpty(file)) ? string.Format("{0}/{1}", path.Local, file) : path.Local);
		return (!text.Contains(".")) ? text : text.Split('.')[0];
	}

	public static string GetGlobal(GlobalPaths.Path path, string file = "")
	{
		return (!string.IsNullOrEmpty(file)) ? string.Format("{0}/{1}", path.Global, file) : path.Global;
	}

	private static string GetBundle(GlobalPaths.Path path, string file = "")
	{
		return (!string.IsNullOrEmpty(file)) ? string.Format("{0}/{1}", path.Bundle, (!file.Contains(".")) ? file : file.Split('.')[0]) : ((!path.Bundle.Contains(".")) ? path.Bundle : path.Bundle.Split('.')[0]);
	}

	public static GameObject GetObject(string path, string file = "")
	{
		return GetObject(GlobalPaths.CreatePath(path), file);
	}

	public static GameObject GetObject(GlobalPaths.Path path, string file = "")
	{
		string bundle = GetBundle(path, file);
		if (NekkiAssetImporter.Contains(bundle))
		{
			return NekkiAssetImporter.GetObject(bundle);
		}
		string local = GetLocal(path, file);
		GameObject gameObject = Resources.Load<GameObject>(local);
		if ((bool)gameObject)
		{
			return gameObject;
		}
		return null;
	}

	public static string GetText(string path, string file = "")
	{
		return GetText(GlobalPaths.CreatePath(path), file);
	}

	public static string GetText(GlobalPaths.Path path, string file = "")
	{
		string bundle = GetBundle(path, file);
		if (NekkiAssetImporter.Contains(bundle))
		{
			return NekkiAssetImporter.GetText(bundle);
		}
		string local = GetLocal(path, file);
		string path2 = SelectExtension(GetGlobal(path, file), ".txt", ".yaml", ".lua", ".lua", ".xml", ".cfg");
		if (System.IO.File.Exists(path2))
		{
			return System.IO.File.ReadAllText(path2);
		}
		TextAsset textAsset = Resources.Load<TextAsset>(local);
		if ((bool)textAsset)
		{
			return textAsset.text;
		}
		return string.Empty;
	}

	public static byte[] GetBytes(string path, string file = "")
	{
		return GetBytes(GlobalPaths.CreatePath(path), file);
	}

	public static byte[] GetBytes(GlobalPaths.Path path, string file = "")
	{
		string bundle = GetBundle(path, file);
		if (NekkiAssetImporter.Contains(bundle))
		{
			return NekkiAssetImporter.GetBytes(bundle);
		}
		string local = GetLocal(path, file);
		string global = GetGlobal(path, file);
		if (System.IO.File.Exists(global))
		{
			return System.IO.File.ReadAllBytes(global);
		}
		TextAsset textAsset = Resources.Load<TextAsset>(local);
		if ((bool)textAsset)
		{
			return textAsset.bytes;
		}
		return new byte[0];
	}

	public static Texture2D GetTexture(string path, string file = "")
	{
		return GetTexture(GlobalPaths.CreatePath(path), file);
	}

	public static Texture2D GetTexture(GlobalPaths.Path path, string file = "")
	{
		string text = GetBundle(path, string.Empty) + "/" + file;
		if (NekkiAssetImporter.Contains(text))
		{
			return NekkiAssetImporter.GetTexture(text);
		}
		string local = GetLocal(path, file);
		string text2 = (SystemProperties.IsAndroid ? SelectExtension(GetGlobal(path, file), ".png", ".jpg", ".bmp") : ((!SystemProperties.IsIos) ? SelectExtension(GetGlobal(path, file), ".tiff", ".tga", ".dtx", ".dx1", ".dx3", ".dx5", ".png", ".jpg", ".bmp") : SelectExtension(GetGlobal(path, file), ".pvrtc", ".pvrtc2", ".png", ".jpg", ".bmp")));
		if (!string.IsNullOrEmpty(text2))
		{
			WWW wWW = new WWW(string.Format("file:///{0}", text2));
			while (!wWW.isDone && string.IsNullOrEmpty(wWW.error))
			{
			}
			if (string.IsNullOrEmpty(wWW.error))
			{
				Texture2D texture2D = new Texture2D(1, 1);
				wWW.LoadImageIntoTexture(texture2D);
				return texture2D;
			}
			AdvLog.LogError(wWW.error);
		}
		return Resources.Load<Texture2D>(local);
	}

	public static AudioClip GetAudioClip(string path, string file = "")
	{
		return GetAudioClip(GlobalPaths.CreatePath(path), file);
	}

	public static AudioClip GetAudioClip(GlobalPaths.Path path, string file = "")
	{
		string bundle = GetBundle(path, file);
		if (NekkiAssetImporter.Contains(bundle))
		{
			return NekkiAssetImporter.GetAudioClip(bundle);
		}
		string local = GetLocal(path, file);
		string text = SelectExtension(GetGlobal(path, file), ".mp3", ".wav", ".ogg");
		if (!string.IsNullOrEmpty(text))
		{
			WWW wWW = new WWW(string.Format("file:///{0}", text));
			while (!wWW.isDone && string.IsNullOrEmpty(wWW.error))
			{
			}
			if (string.IsNullOrEmpty(wWW.error))
			{
				Texture2D tex = new Texture2D(1, 1);
				wWW.LoadImageIntoTexture(tex);
				return wWW.GetAudioClip();
			}
			AdvLog.LogError(wWW.error);
		}
		return Resources.Load<AudioClip>(local);
	}

	public static List<File> GetFiles(string path)
	{
		return GetFiles(GlobalPaths.CreatePath(path));
	}

	public static List<File> GetFiles(GlobalPaths.Path path)
	{
		string local = GetLocal(path, string.Empty);
		string global = GetGlobal(path, string.Empty);
		if (!string.IsNullOrEmpty(global) && Directory.Exists(global))
		{
			List<File> list = new List<File>();
			string[] files = Directory.GetFiles(global);
			for (int i = 0; i < files.Length; i++)
			{
				string text = new FileInfo(files[i]).Name;
				if (!text.Contains(".meta"))
				{
					if (text.Contains("."))
					{
						text = text.Split('.')[0];
					}
					list.Add(new File(text, System.IO.File.ReadAllText(files[i]), System.IO.File.ReadAllBytes(files[i])));
				}
			}
			return list;
		}
		List<File> list2 = new List<File>();
		TextAsset[] array = Resources.LoadAll<TextAsset>(local);
		for (int j = 0; j < array.Length; j++)
		{
			list2.Add(new File(array[j].name, array[j].text, array[j].bytes));
		}
		return list2;
	}

	public static List<string> GetFileNames(GlobalPaths.Path path)
	{
		string local = GetLocal(path, string.Empty);
		string global = GetGlobal(path, string.Empty);
		if (!string.IsNullOrEmpty(global) && Directory.Exists(global))
		{
			List<string> list = new List<string>();
			string[] files = Directory.GetFiles(global);
			for (int i = 0; i < files.Length; i++)
			{
				string name = new FileInfo(files[i]).Name;
				if (!name.Contains(".meta"))
				{
					list.Add(name);
				}
			}
			return list;
		}
		return null;
	}

	private static string SelectExtension(string path, params string[] extensions)
	{
		List<string> list = new List<string>(extensions);
		string arg = path;
		if (!string.IsNullOrEmpty(path) && path.Contains("."))
		{
			string[] array = path.Split('.');
			string text = array[array.Length - 1];
			if (!text.StartsWith("app"))
			{
				list.Add(string.Format(".{0}", text));
				arg = path.Replace(list[list.Count - 1], string.Empty);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			string text2 = string.Format("{0}{1}", arg, (!list[i].Contains(".")) ? string.Format(".{0}", list[i]) : list[i]);
			if (System.IO.File.Exists(text2))
			{
				return text2;
			}
		}
		return string.Empty;
	}
}
