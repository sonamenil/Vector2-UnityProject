using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class SystemProperties
{
	public class DeviceInfoForcibly
	{
		public string isTablet;

		public string resolutionGUI;

		public string resolutionLocation;

		public string qualityCondition;

		public bool Empty
		{
			get
			{
				return string.IsNullOrEmpty(isTablet) && string.IsNullOrEmpty(resolutionGUI) && string.IsNullOrEmpty(resolutionLocation) && string.IsNullOrEmpty(qualityCondition);
			}
		}
	}

	private static readonly List<QualityCondition> QualityConditions = new List<QualityCondition>();

	public static VersionContainer currentBuildVersion;

	public static bool IsMobilePlatform
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.BlackBerryPlayer || Application.platform == RuntimePlatform.TizenPlayer || Application.platform == RuntimePlatform.WP8Player)
			{
				return true;
			}
			return false;
		}
	}

	public static bool IsIos
	{
		get
		{
			return Application.platform == RuntimePlatform.IPhonePlayer;
		}
	}

	public static bool IsAndroid
	{
		get
		{
			return Application.platform == RuntimePlatform.Android;
		}
	}

	public static bool IsTizen
	{
		get
		{
			return Application.platform == RuntimePlatform.TizenPlayer;
		}
	}

	public static bool IsWinwosPhone8
	{
		get
		{
			return Application.platform == RuntimePlatform.WP8Player;
		}
	}

	public static float AllocatedMemory
	{
		get
		{
			return (float)UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory() / 1024f;
		}
	}

	public static float SystemMemory
	{
		get
		{
			return (float)SystemInfo.systemMemorySize / 1024f;
		}
	}

	public static float GraphicsMemory
	{
		get
		{
			return SystemInfo.graphicsMemorySize;
		}
	}

	public static float FreeMemory
	{
		get
		{
			return SystemMemory - AllocatedMemory;
		}
	}

	public static int CoreCount
	{
		get
		{
			return SystemInfo.processorCount;
		}
	}

	public static bool IsTablet { get; private set; }

	public static bool IsTabletDebug
	{
		get
		{
			return (float)Screen.width / (float)Screen.height < 1.66f;
		}
	}

	public static bool IsHighResolution { get; private set; }

	public static bool IsLowResolution
	{
		get
		{
			return !IsHighResolution;
		}
		set
		{
			IsHighResolution = !value;
		}
	}

	public static string DeviceID
	{
		get
		{
			return SystemInfo.deviceName;
		}
	}

	private void initResolution(string devicesXML)
	{
		initResolutionInfo(devicesXML);
	}

	private static Stream StreamFromString(string s)
	{
		MemoryStream memoryStream = new MemoryStream();
		StreamWriter streamWriter = new StreamWriter(memoryStream);
		streamWriter.Write(s);
		streamWriter.Flush();
		memoryStream.Position = 0L;
		return memoryStream;
	}

	private static void initResolutionInfo(string devicesXML)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(StreamFromString(devicesXML));
		XmlElement xmlElement = xmlDocument["Root"];
		if (xmlElement == null)
		{
			return;
		}
		XmlElement xmlElement2 = xmlElement["Config"];
		float num = Screen.height;
		if (xmlElement2 != null)
		{
			for (int i = 0; i < xmlElement2.ChildNodes.Count; i++)
			{
				if (xmlElement2.ChildNodes[i].NodeType != XmlNodeType.Comment && xmlElement2.ChildNodes[i].Attributes != null)
				{
					float num2 = float.Parse(xmlElement2.ChildNodes[i].Attributes["Value"].InnerText);
					string innerText = xmlElement2.ChildNodes[i].Attributes["ConditionType"].InnerText;
					if ((innerText.Equals("Equal") && Math.Abs(num - num2) < 0.0001f) || (innerText.Equals("LessThan") && num < num2) || (innerText.Equals("GreaterThan") && num > num2))
					{
						IsTablet = !string.IsNullOrEmpty(xmlElement2.Attributes["Tablet"].Value) && int.Parse(xmlElement2.Attributes["Tablet"].Value) > 0;
						IsHighResolution = !string.IsNullOrEmpty(xmlElement2.Attributes["Resolution"].Value) && !xmlElement2.Attributes["Resolution"].Value.Equals("LOW");
					}
				}
			}
		}
		XmlElement xmlElement3 = xmlElement["Devices"];
		if (xmlElement3 != null)
		{
			DeviceInfoForcibly deviceInfoForcibly = new DeviceInfoForcibly();
			for (int j = 0; j < xmlElement3.ChildNodes.Count; j++)
			{
				if (xmlElement3.ChildNodes[j].Attributes == null)
				{
					continue;
				}
				if (int.Parse(xmlElement3.ChildNodes[j].Attributes["Forcibly"].Value) > 0)
				{
					if (xmlElement3.ChildNodes[j].Attributes["Tablet"] != null)
					{
						deviceInfoForcibly.isTablet = xmlElement3.ChildNodes[j].Attributes["Tablet"].Value;
					}
					if (xmlElement3.ChildNodes[j].Attributes["Resolution"] != null)
					{
						deviceInfoForcibly.resolutionGUI = xmlElement3.ChildNodes[j].Attributes["Resolution"].Value;
					}
					if (xmlElement3.ChildNodes[j].Attributes["LocationResolution"] != null)
					{
						deviceInfoForcibly.resolutionLocation = xmlElement3.ChildNodes[j].Attributes["LocationResolution"].Value;
					}
					if (xmlElement3.ChildNodes[j].Attributes["QualityCondition"] != null)
					{
						deviceInfoForcibly.qualityCondition = xmlElement3.ChildNodes[j].Attributes["QualityCondition"].Value;
					}
				}
				if (DeviceID.Equals(xmlElement3.ChildNodes[j].Attributes["Name"].Value))
				{
					IsTablet = xmlElement3.ChildNodes[j].Attributes["Tablet"] != null && int.Parse(xmlElement3.ChildNodes[j].Attributes["Tablet"].Value) > 0;
					if (xmlElement3.ChildNodes[j].Attributes["QualityCondition"] != null)
					{
						IsHighResolution = xmlElement3.ChildNodes[j].Attributes["QualityCondition"].Value.Equals("HIGH");
					}
				}
			}
			if (!deviceInfoForcibly.Empty)
			{
				IsTablet = !string.IsNullOrEmpty(deviceInfoForcibly.isTablet) && int.Parse(deviceInfoForcibly.isTablet) > 0;
				IsHighResolution = !string.IsNullOrEmpty(deviceInfoForcibly.qualityCondition) && deviceInfoForcibly.qualityCondition.Equals("HIGH");
			}
		}
		ParseQualityConditions(xmlElement["QualityConditions"]);
	}

	private static void ParseQualityConditions(XmlNode node)
	{
		QualityConditions.Clear();
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			QualityConditions.Add(new QualityCondition(node.ChildNodes[i]));
		}
	}

	internal static void Init(VersionContainer BuildVersion)
	{
		currentBuildVersion = BuildVersion;
		string text = GlobalLoad.GetText(GlobalPaths.GetPath("DevicesQuality"), string.Empty);
		initResolutionInfo(text);
	}
}
