using System;
using UnityEngine;

public static class DeviceInformation
{
	public static string DeviceType
	{
		get
		{
			return Platform + ((!IsTablet) ? "_phone" : "_pad");
		}
	}

	public static string Platform
	{
		get
		{
			if (IsEmulator)
			{
				return "ios";
			}
			if (IsAndroid)
			{
				return "and";
			}
			if (IsiOS)
			{
				return "ios";
			}
			return "unk";
		}
	}

	public static string OS
	{
		get
		{
			if (IsAndroid)
			{
				string operatingSystem = SystemInfo.operatingSystem;
				int num = operatingSystem.IndexOf("(");
				if (num == -1)
				{
					return SystemInfo.operatingSystem;
				}
				return operatingSystem.Substring(0, num - 1);
			}
			return SystemInfo.operatingSystem;
		}
	}

	public static string GetDeviceUniqueID
	{
		get
		{
			string getID = DeviceUniqueID.GetID;
			if (getID != null && getID != string.Empty)
			{
				return getID;
			}
			return SystemInfo.deviceUniqueIdentifier;
		}
	}

	public static string GetServerUniqueID
	{
		get
		{
			return string.Format("{0}_{1}", Platform, GetDeviceUniqueID);
		}
	}

	public static string GUID
	{
		get
		{
			if (IsiOS)
			{
				return GetServerUniqueID;
			}
			if (IsAndroid)
			{
				if (!PlayerPrefs.HasKey("AndroidGUID"))
				{
					string value = Guid.NewGuid().ToString();
					PlayerPrefs.SetString("AndroidGUID", value);
				}
				return PlayerPrefs.GetString("AndroidGUID");
			}
			return null;
		}
	}

	public static string DeviceToken
	{
		get
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
	}

	public static string CoreCount
	{
		get
		{
			return SystemInfo.processorCount.ToString();
		}
	}

	public static string RamSize
	{
		get
		{
			return SystemInfo.systemMemorySize.ToString();
		}
	}

	public static string DeviceModel
	{
		get
		{
			return SystemInfo.deviceModel;
		}
	}

	public static string ScreenWidth
	{
		get
		{
			return Screen.width.ToString();
		}
	}

	public static string ScreenHeight
	{
		get
		{
			return Screen.height.ToString();
		}
	}

	public static bool IsAndroid
	{
		get
		{
			return Application.platform == RuntimePlatform.Android;
		}
	}

	public static bool IsiOS
	{
		get
		{
			return Application.platform == RuntimePlatform.IPhonePlayer;
		}
	}

	public static bool IsMobile
	{
		get
		{
			return IsiOS || IsAndroid;
		}
	}

	public static bool IsLocalbuild
	{
		get
		{
			return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer;
		}
	}

	public static bool IsEmulator
	{
		get
		{
			return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer;
		}
	}

	public static bool IsiOS512mbDevice
	{
		get
		{
			if (!IsiOS)
			{
				return false;
			}
			string deviceModel = DeviceModel;
			return deviceModel == "iPhone4,1" || deviceModel.Contains("iPad2,") || deviceModel == "iPod5,1";
		}
	}

	public static bool CheckScreenHeight
	{
		get
		{
			return Screen.height > 768;
		}
	}

	public static bool IsTablet
	{
		get
		{
			return CheckScreenHeight;
		}
	}
}
