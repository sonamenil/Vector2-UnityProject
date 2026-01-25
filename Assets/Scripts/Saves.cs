using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class Saves
{
	public const string DataFolderName = "Saves";

	public static string Datapath { get; private set; }

	static Saves()
	{
		string text = ((!Application.isEditor) ? ((!SystemProperties.IsMobilePlatform) ? Application.dataPath : Application.persistentDataPath) : Application.dataPath.Remove(Application.dataPath.Length - 7, 7));
		Datapath = string.Format("{0}\\{1}", text.Replace("/", "\\"), "Saves");
		if (!Directory.Exists(Datapath))
		{
			Directory.CreateDirectory(Datapath);
		}
	}

	public static void Save(string file, object content)
	{
		File.WriteAllText(string.Format("{0}\\{1}.json", Datapath, file), JsonConvert.SerializeObject(content));
	}

	public static T Load<T>(string file) where T : class
	{
		string path = string.Format("{0}\\{1}.json", Datapath, file);
		if (File.Exists(path))
		{
			string value = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<T>(value);
		}
		return (T)null;
	}
}
