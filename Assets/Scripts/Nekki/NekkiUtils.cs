using System.Globalization;
using UnityEngine;

namespace Nekki
{
	public class NekkiUtils
	{
		public enum DEVICE_TYPES
		{
			NONE = 0,
			UNKNOWN = 1,
			PHONE = 2,
			TABLET = 3,
			CONSOLE = 4,
			DESKTOP = 5
		}

		private static DEVICE_TYPES deviceType;

		public static Vector2 GetVector2FromString(string str, char splitSimbol)
		{
			str = str.Trim();
			string[] array = str.Split(splitSimbol);
			return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
		}

		public static Vector3 GetVector3FromString(string str, char splitSimbol)
		{
			str = str.Trim();
			string[] array = str.Split(splitSimbol);
			return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
		}

		public static Vector4 GetVector4FromString(string str, char splitSimbol)
		{
			str = str.Trim();
			string[] array = str.Split(splitSimbol);
			return new Vector4(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}

		public static Matrix4x4 GetMatrix4x4FromStringByColumns(string str, char splitSimbol)
		{
			str = str.Trim();
			string[] array = str.Split(splitSimbol);
			Matrix4x4 result = default(Matrix4x4);
			short num = 0;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					result[j, i] = float.Parse(array[num]);
					num++;
				}
			}
			return result;
		}

		public static Matrix4x4 GetMatrix4x4FromStringByRows(string str, char splitSimbol)
		{
			str = str.Trim();
			string[] array = str.Split(splitSimbol);
			Matrix4x4 result = default(Matrix4x4);
			short num = 0;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					result[i, j] = float.Parse(array[num]);
					num++;
				}
			}
			return result;
		}

		public static string SubStringBySimbolCount(string str, char splitSimbol, int startPosition, int count, out int lastPosition)
		{
			str = str.Trim();
			int num = 0;
			int num2 = startPosition;
			while (num < count && num2 < str.Length)
			{
				if (str[num2] == splitSimbol)
				{
					num++;
				}
				num2++;
			}
			lastPosition = num2;
			if (num == count)
			{
				return str.Substring(startPosition, num2 - startPosition);
			}
			return str.Substring(startPosition);
		}

		public static Quaternion GetQuaternionFromMatrix(Matrix4x4 matrix)
		{
			return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
		}

		public static GameObject Instantiate(string prefabPath)
		{
			return Object.Instantiate(GlobalLoad.GetObject(prefabPath, string.Empty));
		}

		public static T Instantiate<T>(string prefabPath) where T : Component
		{
			return Instantiate(prefabPath).GetComponent<T>();
		}

		public static string ColorToHex(Color32 color)
		{
			return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		}

		public static Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, byte.MaxValue);
		}

		public static Color IntToColor(int color)
		{
			Color result = default(Color);
			result.r = (float)((color & 0xFF0000) >> 16) / 255f;
			result.g = (float)((color & 0xFF00) >> 8) / 255f;
			result.b = (float)(color & 0xFF) / 255f;
			result.a = 1f;
			return result;
		}

		public static int ColorToInt(Color color)
		{
			return ((int)(color.r * 255f) << 16) + ((int)(color.g * 255f) << 8) + (int)(color.b * 255f);
		}

		public static void SetDeviceType()
		{
			if (SystemInfo.deviceType != DeviceType.Handheld)
			{
				switch (SystemInfo.deviceType)
				{
				case DeviceType.Unknown:
					deviceType = DEVICE_TYPES.UNKNOWN;
					break;
				case DeviceType.Console:
					deviceType = DEVICE_TYPES.CONSOLE;
					break;
				case DeviceType.Desktop:
					deviceType = DEVICE_TYPES.DESKTOP;
					break;
				case DeviceType.Handheld:
					break;
				}
				return;
			}
			float num = ((Screen.width <= Screen.height) ? ((float)Screen.height) : ((float)Screen.width));
			if (num < 800f)
			{
				deviceType = DEVICE_TYPES.PHONE;
			}
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			{
				float f = (float)Screen.width / Screen.dpi;
				float f2 = (float)Screen.height / Screen.dpi;
				float num2 = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
				if (num2 >= 6.5f)
				{
					deviceType = DEVICE_TYPES.TABLET;
				}
			}
			deviceType = DEVICE_TYPES.PHONE;
		}

		public static DEVICE_TYPES GetDeviceType()
		{
			if (deviceType == DEVICE_TYPES.NONE)
			{
				SetDeviceType();
			}
			return deviceType;
		}

		public static bool IsTablet()
		{
			return GetDeviceType() == DEVICE_TYPES.TABLET;
		}

		public static bool IsPhone()
		{
			return GetDeviceType() == DEVICE_TYPES.PHONE;
		}

		public static string GetAliasForCurrentPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
				return "standaloneMacOSX";
			case RuntimePlatform.OSXPlayer:
				return "standaloneMacOSX";
			case RuntimePlatform.WindowsPlayer:
				return "standaloneWindows";
			case RuntimePlatform.WindowsEditor:
				return "standaloneWindows";
			case RuntimePlatform.IPhonePlayer:
				return "ios";
			case RuntimePlatform.Android:
				return "android";
			default:
				return string.Empty;
			}
		}
	}
}
