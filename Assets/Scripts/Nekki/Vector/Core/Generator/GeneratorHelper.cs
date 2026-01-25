using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using UnityEngine;

namespace Nekki.Vector.Core.Generator
{
	public static class GeneratorHelper
	{
		private const string _PlayRoomPropertiesFile = "play_cmd_room_properties.xml";

		private const string _EditorGenFile = "room_editor_gen.xml";

		private static List<string> _ToLoadRooms = new List<string>();

		private static XmlElement _EditorGenRoot;

		public static string MainRoomPropertiesPath
		{
			get
			{
				return VectorPaths.GeneratorData + "/" + ZoneManager.GetResourceFilePath("RoomProperties");
			}
		}

		public static string PlayRoomPropertiesPath
		{
			get
			{
				return VectorPaths.GeneratorDataExternal + "/play_cmd_room_properties.xml";
			}
		}

		public static string EditorGenPath
		{
			get
			{
				return VectorPaths.GeneratorTemp + "/room_editor_gen.xml";
			}
		}

		public static List<string> ToLoadRooms
		{
			get
			{
				return _ToLoadRooms;
			}
		}

		public static XmlElement EditorGenRoot
		{
			get
			{
				return _EditorGenRoot;
			}
		}

		public static bool IsPlayCommand
		{
			get
			{
				return !IsEditorCommand && (int)CounterController.Current.CounterPlayCommand > 0;
			}
		}

		public static bool IsEditorCommand
		{
			get
			{
				return _EditorGenRoot != null;
			}
		}

		public static void ResetEditiorGen()
		{
			if (_EditorGenRoot != null)
			{
				_EditorGenRoot = null;
			}
		}

		public static void PrepareFloor(int p_floor, int p_seed)
		{
			MainRandom.SetSeed(p_seed);
			PrepareFloorData(p_floor);
		}

		public static void PrepareFloor(int p_floor, uint p_seed)
		{
			MainRandom.SetSeed(p_seed);
			PrepareFloorData(p_floor);
		}

		private static void PrepareFloorData(int p_floor)
		{
			RunMainController.RoomProperties = MainRoomPropertiesPath;
			CounterController.Current.CounterFloor = p_floor;
			CounterController.Current.CounterPlayCommand = 0;
			MakeDemoUserBackup();
			DataLocal.Current.OnStartRun();
		}

		public static void PrepareNextFloor(int p_seed)
		{
			MainRandom.SetSeed(p_seed);
			MakeDemoUserBackup();
		}

		public static void PrepareNextFloor(uint p_seed)
		{
			MainRandom.SetSeed(p_seed);
			MakeDemoUserBackup();
		}

		public static void PrepareNextFloor(int p_floor, uint p_seed)
		{
			RunMainController.RoomProperties = MainRoomPropertiesPath;
			CounterController.Current.CounterFloor = p_floor;
			CounterController.Current.CounterPlayCommand = 0;
			PrepareNextFloor(p_seed);
		}

		public static bool PreparePlayCommand(List<string> p_roomNames, int p_floor, uint p_seed)
		{
			if (!PreparePlayCommandData(p_roomNames, p_floor))
			{
				return false;
			}
			MainRandom.SetSeed(p_seed);
			return true;
		}

		public static bool PreparePlayCommand(List<string> p_roomNames, int p_floor, int p_seed = -1)
		{
			if (!PreparePlayCommandData(p_roomNames, p_floor))
			{
				return false;
			}
			MainRandom.SetSeed(p_seed);
			return true;
		}

		private static bool PreparePlayCommandData(List<string> p_roomNames, int p_floor)
		{
			try
			{
				if (!UpdatePlayCommandRoomProperties(p_roomNames))
				{
					return false;
				}
				RunMainController.RoomProperties = PlayRoomPropertiesPath;
				CounterController.Current.CounterFloor = p_floor;
				CounterController.Current.CounterPlayCommand = _ToLoadRooms.Count + 2;
				RunMainController.RunEnd();
				MakeDemoUserBackup();
				if (Manager.IsEquip)
				{
					if (!Demo.IsPlaying)
					{
						StarterPacksManager.ActivateSelectedStarterPack();
					}
					DataLocal.Current.OnStartRun();
				}
				return true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return false;
		}

		private static bool UpdatePlayCommandRoomProperties(List<string> p_roomNames)
		{
			string directoryName = Path.GetDirectoryName(PlayRoomPropertiesPath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(MainRoomPropertiesPath, string.Empty);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//Room");
			List<XmlElement> list = new List<XmlElement>();
			_ToLoadRooms.Clear();
			if (xmlNodeList != null)
			{
				foreach (XmlElement item in xmlNodeList)
				{
					string text = XmlUtils.ParseString(item.Attributes["Name"], string.Empty);
					if (p_roomNames.Contains(text.ToLower()))
					{
						_ToLoadRooms.Add(text);
					}
					else if (item.Attributes["IncludeInPlayCommand"] == null)
					{
						list.Add(item);
					}
				}
				foreach (XmlElement item2 in list)
				{
					if (item2.ParentNode != null)
					{
						item2.ParentNode.RemoveChild(item2);
					}
				}
			}
			if (_ToLoadRooms.Count == 0)
			{
				return false;
			}
			xmlDocument.Save(PlayRoomPropertiesPath);
			return true;
		}

		public static bool PrepareEditorCommand(int p_floor, int p_seed = -1)
		{
			ResetEditiorGen();
			if (!PrepareEditorCommandData(p_floor))
			{
				return false;
			}
			MainRandom.SetSeed(p_seed);
			return true;
		}

		public static bool PrepareEditorCommand(XmlElement p_editorGenRoot, int p_floor, uint p_seed)
		{
			_EditorGenRoot = p_editorGenRoot;
			if (!PrepareEditorCommandData(p_floor))
			{
				return false;
			}
			MainRandom.SetSeed(p_seed);
			return true;
		}

		private static bool PrepareEditorCommandData(int p_floor)
		{
			try
			{
				if (!LoadEditorGen())
				{
					return false;
				}
				RunMainController.RoomProperties = EditorGenPath;
				CounterController.Current.CounterFloor = p_floor;
				CounterController.Current.CounterPlayCommand = 3;
				MakeDemoUserBackup();
				DataLocal.Current.OnStartRun();
				return true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return false;
		}

		private static bool LoadEditorGen()
		{
			if (_EditorGenRoot != null)
			{
				if (_EditorGenRoot.Attributes["New"] != null)
				{
					RemoveEditorGenFile();
					return false;
				}
				RecreateEditorGen();
				return true;
			}
			if (!File.Exists(EditorGenPath))
			{
				return false;
			}
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(EditorGenPath, string.Empty);
			_EditorGenRoot = xmlDocument["Root"];
			if (_EditorGenRoot.Attributes["New"] != null)
			{
				RemoveEditorGenFile();
				return false;
			}
			ProcessEditorGen();
			return true;
		}

		private static void RecreateEditorGen()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("Root");
			foreach (XmlNode childNode in _EditorGenRoot.ChildNodes)
			{
				XmlNode newChild = xmlDocument.ImportNode(childNode, true);
				xmlElement.AppendChild(newChild);
			}
			xmlDocument.AppendChild(xmlElement);
			xmlElement.SetAttribute("New", "0");
			if (!Directory.Exists(VectorPaths.GeneratorTemp))
			{
				Directory.CreateDirectory(VectorPaths.GeneratorTemp);
			}
			xmlDocument.Save(EditorGenPath);
			ResetEditiorGen();
		}

		private static void ProcessEditorGen()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(MainRoomPropertiesPath, string.Empty);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//Room");
			foreach (XmlNode item in xmlNodeList)
			{
				if (item.Attributes["IncludeInPlayCommand"] != null)
				{
					XmlNode newChild = _EditorGenRoot.OwnerDocument.ImportNode(item, true);
					_EditorGenRoot.AppendChild(newChild);
				}
			}
			_EditorGenRoot.SetAttribute("New", "0");
			_EditorGenRoot.OwnerDocument.Save(EditorGenPath);
		}

		public static bool IsEditorGenFileNew()
		{
			if (!File.Exists(EditorGenPath))
			{
				return false;
			}
            XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(EditorGenPath, string.Empty);
			return xmlDocument["Root"].Attributes["New"] == null;
		}

		public static void RemoveEditorGenFile()
		{
			ResetEditiorGen();
			FileInfo fileInfo = new FileInfo(EditorGenPath);
			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
		}

		private static void MakeDemoUserBackup()
		{
			if ((!DeviceInformation.IsMobile || !Settings.IsReleaseBuild) && !Demo.IsPlaying)
			{
				DataLocal.SetupDemoBackup();
			}
		}
	}
}
