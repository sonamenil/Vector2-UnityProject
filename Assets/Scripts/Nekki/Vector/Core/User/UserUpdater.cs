using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Nekki.Vector.Core.DataValidation;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.Core.User
{
	public static class UserUpdater
	{
		public const string DownloadFileUpdater = "user_update.yaml";

		private static string DownloadFileUpdaterPath
		{
			get
			{
				return VectorPaths.ExternalCachedGameData + "/user_update.yaml";
			}
		}

		public static void TryUpdateUserByDownloadContent()
		{
			string downloadFileUpdaterPath = DownloadFileUpdaterPath;
			if (File.Exists(downloadFileUpdaterPath))
			{
				RunRegularExpression(downloadFileUpdaterPath, YamlUtils.OpenYamlType.ForcedExternal);
				DataLocal.Reload();
				Run(downloadFileUpdaterPath, YamlUtils.OpenYamlType.ForcedExternal);
				File.Delete(downloadFileUpdaterPath);
			}
		}

		public static void Run(int p_version, int p_currVer)
		{
			for (int i = p_version; i < p_currVer; i++)
			{
				string p_filename = string.Format("{0}/from_{1}_to_{2}_update.yaml", VectorPaths.UserUpdate, i, i + 1);
				Run(p_filename, YamlUtils.OpenYamlType.Normal);
			}
			DataLocal.Current.Save(true);
		}

		public static void RunRegularExpression(int p_version, int p_currVer)
		{
			for (int i = p_version; i < p_currVer; i++)
			{
				string p_filename = string.Format("{0}/from_{1}_to_{2}_update.yaml", VectorPaths.UserUpdate, i, i + 1);
				RunRegularExpression(p_filename, YamlUtils.OpenYamlType.Normal);
			}
		}

		private static void Run(string p_filename, YamlUtils.OpenYamlType p_type)
		{
			MainRandom.SetSeed(-1);
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(p_filename, string.Empty, p_type);
			Nekki.Yaml.Node node = yamlDocumentNekki.GetRoot(0).GetNode("UserVersion");
			if (node != null)
			{
				int version = int.Parse(yamlDocumentNekki.GetRoot(0).GetNode("UserVersion").value.ToString());
				DataLocal.Current.Version = version;
			}
			foreach (Nekki.Yaml.Node item in yamlDocumentNekki.GetRoot(0))
			{
				if (item.IsMapping())
				{
					switch (item.key.Split('_')[0])
					{
					case "CountersUpdater":
						UpdateCounters(Preset.Create((Mapping)item));
						break;
					case "ItemsUpdater":
						UpdateItems(Preset.Create((Mapping)item));
						break;
					case "NewItems":
						NewItems((Mapping)item);
						break;
					default:
						DebugUtils.Dialog("unknown preset " + item.key + " on " + p_filename, true);
						break;
					case "ReplaceRegularExpression":
						break;
					}
				}
			}
		}

		private static void RunRegularExpression(string p_filename, YamlUtils.OpenYamlType p_type)
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(p_filename, string.Empty, p_type);
			foreach (Nekki.Yaml.Node item in yamlDocumentNekki.GetRoot(0))
			{
				if (item.IsMapping() && item.key == "ReplaceRegularExpression")
				{
					UserRegex((Mapping)item);
				}
			}
			XmlDocument p_file = XmlUtils.OpenXMLDocument(DataLocal.FilePath, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
			UserDataValidator.UpdateFileHash(p_file, DataLocal.FilePath);
		}

		private static void UpdateCounters(Preset p_preset)
		{
			p_preset.RunPreset();
		}

		private static void UpdateItems(Preset p_preset)
		{
			for (int num = DataLocal.Current.AllItems.Count - 1; num >= 0; num--)
			{
				UserItem userItem = DataLocal.Current.AllItems.ElementAt(num);
				StringBuffer.AddString("Upd_ItemName", userItem.Name);
				p_preset.RunPreset();
				StringBuffer.AddString("Upd_ItemName", string.Empty);
			}
		}

		private static void NewItems(Mapping p_presetNodes)
		{
			foreach (Mapping p_presetNode in p_presetNodes)
			{
				Preset preset = Preset.Create(p_presetNode);
				if (preset != null)
				{
					int i = 0;
					for (int valueInt = preset.ItemsCount.ValueInt; i < valueInt; i++)
					{
						preset.RunPreset();
					}
				}
			}
		}

		private static void UserRegex(Mapping p_presetNodes)
		{
			string text = string.Empty;
			FileStream fileStream = null;
			try
			{
				using (fileStream = new FileStream(DataLocal.FilePath, FileMode.Open))
				{
					byte[] array = new byte[fileStream.Length];
					if (fileStream.CanRead)
					{
						fileStream.Read(array, 0, array.Length);
					}
					else
					{
						Debug.Log("Reading user Error");
					}
					text = Encoding.UTF8.GetString(array);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Reading user Error " + ex.ToString());
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			foreach (Mapping item in p_presetNodes.GetMapping("BoxContent"))
			{
				text = Regex.Replace(text, item.GetText("Regular").text, item.GetText("Replace").text);
			}
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			try
			{
				using (fileStream = new FileStream(DataLocal.FilePath, FileMode.Truncate))
				{
					if (fileStream.CanWrite)
					{
						fileStream.Write(bytes, 0, bytes.Length);
					}
					else
					{
						Debug.Log("Writing user Error");
					}
				}
			}
			catch (Exception ex2)
			{
				Debug.Log("Reading user Error:" + ex2.ToString());
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
		}
	}
}
