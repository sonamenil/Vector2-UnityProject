using System.IO;
using System.Xml;
using Nekki.Vector.Core.Game;

namespace Nekki.Vector.Core.DataValidation
{
	public static class UserDataValidator
	{
		private const string _HashExtension = ".hash";

		private const string _StaticSalt = "LYvpGHdSwqxwl618wqO+Qchj|r*QXg7o_KNmKaD^QI2vyPwt-h3H8*uJ";

		private static bool _IsValid = true;

		public static bool IsValid
		{
			get
			{
				return _IsValid;
			}
		}

		public static string Salt
		{
			get
			{
				return "LYvpGHdSwqxwl618wqO+Qchj|r*QXg7o_KNmKaD^QI2vyPwt-h3H8*uJ" + DeviceInformation.GetDeviceUniqueID;
			}
		}

		public static bool CheckFileHash(XmlDocument p_file, string p_path)
		{
			if (!ValidatorSettings.IsEnabled && Settings.UserValidationOn)
			{
				return true;
			}
			string text = ReadHash(p_path + ".hash");
			if (string.IsNullOrEmpty(text))
			{
				_IsValid = false;
				DebugUtils.LogError("[UserDataValidator]: file is missing - " + Path.GetFileName(p_path) + " !");
				return IsValid;
			}
			_IsValid = MD5Utils.CheckStringHash(p_file.OuterXml, text, Salt);
			if (!_IsValid)
			{
				DebugUtils.LogError("[UserDataValidator]: incorrect file hash - " + Path.GetFileName(p_path) + " !");
			}
			return _IsValid;
		}

		private static string ReadHash(string p_path)
		{
			if (!File.Exists(p_path))
			{
				return null;
			}
			return File.ReadAllText(p_path);
		}

		public static void UpdateFileHash(string p_file)
		{
			if (ValidatorSettings.IsEnabled)
			{
				string p_hash = MD5Utils.MD5HashFile(p_file, Salt);
				WriteHash(p_file + ".hash", p_hash);
			}
		}

		public static void UpdateFileHash(XmlDocument p_file, string p_path)
		{
			if (ValidatorSettings.IsEnabled)
			{
				string p_hash = MD5Utils.MD5HashString(p_file.OuterXml, Salt);
				WriteHash(p_path + ".hash", p_hash);
			}
		}

		private static void WriteHash(string p_path, string p_hash)
		{
			File.WriteAllText(p_path, p_hash);
		}

		public static void DeleteFileHash(string p_path)
		{
			if (ValidatorSettings.IsEnabled)
			{
				p_path += ".hash";
				if (File.Exists(p_path))
				{
					File.Delete(p_path);
				}
			}
		}

		public static void CopyFileHash(string p_from, string p_to)
		{
			if (ValidatorSettings.IsEnabled && File.Exists(p_from + ".hash"))
			{
				File.Copy(p_from + ".hash", p_to + ".hash", true);
			}
		}
	}
}
