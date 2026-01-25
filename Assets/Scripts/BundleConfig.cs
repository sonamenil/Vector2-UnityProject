using System;
using System.Collections.Generic;
using System.Xml;

public class BundleConfig
{
	public class Line
	{
		public string URL;

		public string Name;

		public VersionContainer MinVersion;

		public string BinarryURL
		{
			get
			{
				return InsertExt(URL, "bin");
			}
		}

		public string ConfigURL
		{
			get
			{
				return InsertExt(URL, "config");
			}
		}

		private static string InsertExt(string url, string ext)
		{
			if (url.Contains("?"))
			{
				string[] array = url.Split('?');
				return string.Format("{0}.{2}?{1}", array[0], array[1], ext);
			}
			return string.Format("{0}.{1}", url, ext);
		}
	}

	public class Local
	{
		private static Local _instance;

		private readonly List<Line> _lines = new List<Line>();

		private XmlDocument _doc;

		public VersionContainer DataVersion { get; private set; }

		public List<Line> Lines
		{
			get
			{
				return _lines;
			}
		}

		public static Local GetLocal
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Local(GlobalLoad.GetText(GlobalPaths.GetPath("ConfigPath"), string.Empty));
				}
				return _instance;
			}
		}

		private Local()
		{
			InitEmpty();
		}

		public Local(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				InitEmpty();
				return;
			}
			_doc = new XmlDocument();
			_doc.LoadXml(content);
			try
			{
				DataVersion = new VersionContainer(_doc["Config"].Attributes["DataVersion"].Value);
			}
			catch (Exception)
			{
				DataVersion = VersionContainer.Zero;
			}
			XmlNodeList bundleParams = GetBundleParams(_doc, true);
			if (bundleParams == null)
			{
				return;
			}
			for (int i = 0; i < bundleParams.Count; i++)
			{
				if (bundleParams[i].Attributes != null)
				{
					_lines.Add(new Line
					{
						Name = bundleParams[i].Attributes["Name"].Value,
						URL = bundleParams[i].Attributes["Url"].Value,
						MinVersion = new VersionContainer(bundleParams[i].Attributes["MinVersion"].Value)
					});
				}
			}
		}

		private void InitEmpty()
		{
			DataVersion = VersionContainer.Zero;
			_doc = new XmlDocument();
			XmlElement newChild = _doc.CreateElement("Config");
			_doc.AppendChild(newChild);
			XmlElement newChild2 = _doc.CreateElement("Bundles");
			_doc["Config"].AppendChild(newChild2);
		}

		public bool Contains(Line line)
		{
			return Contains(line.Name, line.URL);
		}

		public bool Contains(string name, string url)
		{
			for (int i = 0; i < _lines.Count; i++)
			{
				if (_lines[i].Name == name && _lines[i].URL == Unity3dAsZip(url))
				{
					return true;
				}
			}
			return false;
		}

		public void Update(Line line)
		{
			Update(line.Name, line.URL, line.MinVersion);
		}

		private bool ContainsRow(string name)
		{
			for (int i = 0; i < _lines.Count; i++)
			{
				if (_lines[i].Name == name)
				{
					return true;
				}
			}
			return false;
		}

		public void Update(string name, string url, VersionContainer minVersion)
		{
			if (ContainsRow(name))
			{
				XmlNodeList bundleParams = GetBundleParams(_doc, true);
				if (bundleParams == null)
				{
					return;
				}
				for (int i = 0; i < bundleParams.Count; i++)
				{
					if (bundleParams[i].Attributes != null && bundleParams[i].Attributes["Name"].Value.Equals(name))
					{
						bundleParams[i].Attributes["Url"].Value = Unity3dAsZip(url);
						Dump();
						break;
					}
				}
				return;
			}
			XmlElement xmlElement = _doc.CreateElement("Bundle");
			xmlElement.SetAttribute("Name", name);
			xmlElement.SetAttribute("Url", Unity3dAsZip(url));
			xmlElement.SetAttribute("MinVersion", minVersion.ToString());
			if (_doc["Config"] == null)
			{
				XmlElement newChild = _doc.CreateElement("Config");
				_doc.AppendChild(newChild);
			}
			if (_doc["Config"]["Bundles"] == null)
			{
				XmlElement newChild2 = _doc.CreateElement("Bundles");
				_doc["Config"].AppendChild(newChild2);
			}
			_doc["Config"]["Bundles"].AppendChild(xmlElement);
			if (minVersion > DataVersion)
			{
				DataVersion = minVersion;
				_doc["Config"].SetAttribute("DataVersion", DataVersion.ToString(true));
			}
			Dump();
		}

		private void Dump()
		{
			string global = GlobalLoad.GetGlobal(GlobalPaths.GetPath("ConfigPath"), string.Empty);
			_doc.Save(global);
		}
	}

	public class Remote
	{
		private readonly List<Line> _lines = new List<Line>();

		public List<Line> Lines
		{
			get
			{
				return _lines;
			}
		}

		public Remote(string content)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(content);
			XmlNodeList bundleParams = GetBundleParams(xmlDocument, false);
			if (bundleParams == null)
			{
				return;
			}
			for (int i = 0; i < bundleParams.Count; i++)
			{
				if (bundleParams[i].Attributes == null)
				{
					continue;
				}
				string value = bundleParams[i].Attributes["Name"].Value;
				string value2 = bundleParams[i].Attributes["Url"].Value;
				VersionContainer versionContainer = new VersionContainer(bundleParams[i].Attributes["MinVersion"].Value);
				if (SystemProperties.currentBuildVersion.ForNextVersions(versionContainer))
				{
					continue;
				}
				Line line = new Line
				{
					Name = value,
					URL = value2,
					MinVersion = versionContainer
				};
				bool flag = false;
				for (int j = 0; j < _lines.Count; j++)
				{
					if (_lines[j].Name.Equals(line.Name))
					{
						flag = true;
						if (_lines[j].MinVersion < line.MinVersion)
						{
							_lines[j] = line;
						}
						break;
					}
				}
				if (!flag)
				{
					_lines.Add(line);
				}
			}
		}
	}

	public static Queue<Line> Copmare(string remoteConfig)
	{
		Remote remote = new Remote(remoteConfig);
		Local getLocal = Local.GetLocal;
		Queue<Line> queue = new Queue<Line>();
		for (int i = 0; i < remote.Lines.Count; i++)
		{
			if (!getLocal.Contains(remote.Lines[i]))
			{
				queue.Enqueue(remote.Lines[i]);
			}
		}
		return queue;
	}

	private static XmlNodeList GetBundleParams(XmlDocument bundleDoc, bool local)
	{
		if (local)
		{
			return bundleDoc.SelectNodes("Config/Bundles/Bundle");
		}
		return bundleDoc.SelectNodes("Config/Bundles/Android/Bundle");
	}

	private static string Unity3dAsZip(string path)
	{
		if (path.Contains(".unity3d"))
		{
			path = path.Replace(".unity3d", ".zip");
		}
		return path;
	}
}
