using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.DataValidation;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;

namespace Nekki.Vector.Core.GameManagement
{
	public static class GameRestorer
	{
		private const string _OldStoreStateFilename = "store_state.xml";

		private const string _OldStoreStateBackupFilename = "store_state.backup";

		private const string _GameRestoreFilename = "game_restore.xml";

		private const string _GameRestoreBackupFilename = "game_restore.backup";

		private static int _LastBackupFloor = -1;

		private static bool _Active;

		private static SceneKind _RestoreScene = SceneKind.None;

		private static string _RestoreTerminalItemName;

		private static bool _RemoveOnLaunch;

		public static string GameRestoreFilePath
		{
			get
			{
				return VectorPaths.StorageUserData + "/game_restore.xml";
			}
		}

		public static string GameRestoreBackupFilePath
		{
			get
			{
				return VectorPaths.StorageUserData + "/game_restore.backup";
			}
		}

		public static bool Active
		{
			get
			{
				return _Active;
			}
		}

		public static SceneKind RestoreScene
		{
			get
			{
				return _RestoreScene;
			}
		}

		public static bool IsRestoreShopAvailable
		{
			get
			{
				return _RestoreScene == SceneKind.Shop;
			}
		}

		public static bool IsRestoreTerminalAvailable
		{
			get
			{
				return _RestoreTerminalItemName != null;
			}
		}

		public static bool IsRestoreBoosterpacksAvailable
		{
			get
			{
				return BoosterpackItemsManager.BasketItems.Count > 0;
			}
		}

		public static bool RemoveOnLaunch
		{
			set
			{
				if (value == _RemoveOnLaunch)
				{
					return;
				}
				_RemoveOnLaunch = value;
				if (!FileUtils.FileExists(GameRestoreFilePath))
				{
					return;
				}
				XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentAndCheckHash(VectorPaths.StorageUserData, "game_restore.xml", XmlUtils.OpenXmlType.ForcedExternal);
				if (xmlDocument == null || !UserDataValidator.IsValid)
				{
					xmlDocument = XmlUtils.OpenXMLDocumentAndCheckHash(VectorPaths.StorageUserData, "game_restore.backup", XmlUtils.OpenXmlType.ForcedExternal);
					if (xmlDocument == null || !UserDataValidator.IsValid)
					{
						RemoveBackup();
						return;
					}
				}
				if (_RemoveOnLaunch)
				{
					if (xmlDocument["Backup"].Attributes["RemoveOnLaunch"] != null)
					{
						xmlDocument["Backup"].Attributes["RemoveOnLaunch"].Value = "1";
					}
					else
					{
						XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("RemoveOnLaunch");
						xmlAttribute.Value = "1";
						xmlDocument["Backup"].Attributes.SetNamedItem(xmlAttribute);
					}
				}
				else if (xmlDocument["Backup"].Attributes["RemoveOnLaunch"] != null)
				{
					xmlDocument["Backup"].RemoveAttribute("RemoveOnLaunch");
				}
				XmlUtils.SaveDocumentAndUpdateHash(xmlDocument, GameRestoreFilePath);
			}
		}

		public static void TryRestore()
		{
			TryRemoveOldRestoreFile();
			if (!CheckRestore())
			{
				RunMainController.IsRunNow = true;
				RunMainController.RunEnd();
				_Active = false;
				return;
			}
			_Active = true;
			if (!RestoreFromFile())
			{
				_Active = false;
				return;
			}
			MainRandom.SetSeed(-1);
			RunMainController.RoomProperties = GeneratorHelper.MainRoomPropertiesPath;
			string eventLabel = string.Format("Restore_{0}", CounterController.Current.CounterFloor.ToString());
		}

		private static void TryRemoveOldRestoreFile()
		{
			bool flag = false;
			if (FileUtils.FileExists(VectorPaths.StorageUserData + "/store_state.xml"))
			{
				FileUtils.DeleteFileAndHash(VectorPaths.StorageUserData + "/store_state.xml");
				flag = true;
			}
			if (FileUtils.FileExists(VectorPaths.StorageUserData + "/store_state.backup"))
			{
				FileUtils.DeleteFileAndHash(VectorPaths.StorageUserData + "/store_state.backup");
				flag = true;
			}
			if (flag && (int)CounterController.Current.CounterAdBlock == 1)
			{
				DataLocal.Reset();
			}
		}

		public static void RemoveBackup()
		{
			_Active = false;
			_LastBackupFloor = -1;
			_RestoreScene = SceneKind.None;
			FileUtils.DeleteFileAndHash(GameRestoreFilePath);
			FileUtils.DeleteFileAndHash(GameRestoreBackupFilePath);
		}

		public static bool SaveBackup()
		{
			SaveBackupToBackup(false);
			_Active = false;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
			XmlElement xmlElement = xmlDocument.CreateElement("Backup");
			xmlDocument.AppendChild(xmlElement);
			if (_RemoveOnLaunch)
			{
				xmlElement.SetAttribute("RemoveOnLaunch", "1");
			}
			bool flag = Manager.CurrentScreen == ScreenType.Boosterpack && BoosterpackItemsManager.IsBoosterpackOpening;
			if (Manager.IsEquip && flag)
			{
				BoosterPackBackup(xmlElement);
			}
			else if (Manager.IsShop || Manager.IsRun)
			{
				StoreBackup(xmlElement);
				if (flag)
				{
					BoosterPackBackup(xmlElement);
				}
			}
			else
			{
				if (!Manager.IsTerminal)
				{
					return false;
				}
				TerminalBackup(xmlElement);
			}
			DataLocal.Current.Save(true);
			XmlUtils.SaveDocumentAndUpdateHash(xmlDocument, GameRestoreFilePath);
			return true;
		}

		public static void SaveForwardBackupToBackup()
		{
			SaveBackupToBackup(true);
		}

		private static void SaveBackupToBackup(bool p_isForvard = false)
		{
			if (FileUtils.FileExists(GameRestoreFilePath))
			{
				int num = CounterController.Current.CounterFloor;
				if (p_isForvard || num == _LastBackupFloor + 1)
				{
					FileUtils.CopyFileAndHash(GameRestoreFilePath, GameRestoreBackupFilePath);
				}
			}
			_LastBackupFloor = CounterController.Current.CounterFloor;
		}

		private static void StoreBackup(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("StoreBackup");
			p_node.AppendChild(xmlElement);
			List<SupplyItem> basketItems = EndFloorManager.BasketItems;
			for (int i = 0; i < basketItems.Count; i++)
			{
				basketItems[i].CurrItem.SaveToXmlNode(xmlElement);
			}
		}

		private static void TerminalBackup(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("TerminalBackup");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("ItemName", TerminalItemsManager.TerminalItemName);
		}

		private static void BoosterPackBackup(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("BoosterpackBackup");
			p_node.AppendChild(xmlElement);
			foreach (BoosterpackItem basketItem in BoosterpackItemsManager.BasketItems)
			{
				basketItem.SaveToXml(xmlElement);
			}
		}

		private static bool CheckRestore()
		{
			return FileUtils.FileExists(GameRestoreFilePath);
		}

		private static bool RestoreFromFile()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentAndCheckHash(VectorPaths.StorageUserData, "game_restore.xml", XmlUtils.OpenXmlType.ForcedExternal);
			if (xmlDocument == null || !UserDataValidator.IsValid)
			{
				xmlDocument = XmlUtils.OpenXMLDocumentAndCheckHash(VectorPaths.StorageUserData, "game_restore.backup", XmlUtils.OpenXmlType.ForcedExternal);
				if (xmlDocument == null || !UserDataValidator.IsValid)
				{
					RemoveBackup();
					return false;
				}
			}
			if (XmlUtils.ParseBool(xmlDocument["Backup"].Attributes["RemoveOnLaunch"]))
			{
				RemoveBackup();
				RestoreStarterPacks();
				RunMainController.IsRunNow = true;
				RunMainController.RunEnd();
				_Active = true;
				_RestoreScene = SceneKind.Terminal;
				return true;
			}
			XmlNode xmlNode = xmlDocument["Backup"]["StoreBackup"];
			XmlNode xmlNode2 = xmlDocument["Backup"]["TerminalBackup"];
			XmlNode xmlNode3 = xmlDocument["Backup"]["BoosterpackBackup"];
			if (xmlNode == null && xmlNode2 == null && xmlNode3 == null)
			{
				return false;
			}
			if (xmlNode3 != null)
			{
				_RestoreScene = SceneKind.Main;
				foreach (XmlNode childNode in xmlNode3.ChildNodes)
				{
					BoosterpackItem item = new BoosterpackItem(childNode);
					BoosterpackItemsManager.BasketItems.Add(item);
				}
			}
			if (xmlNode != null)
			{
				_RestoreScene = SceneKind.Shop;
				RunMainController.IsRunNow = true;
				StarterPacksManager.ResetCurrentStarterPackFromSelection = false;
				EndFloorManager.BasketItems.Clear();
				foreach (XmlNode childNode2 in xmlNode.ChildNodes)
				{
					UserItem userItem = UserItem.CreateByXmlNode(childNode2);
					if (userItem != null)
					{
						EndFloorManager.BasketItems.Add(SupplyItem.Create(userItem));
					}
				}
			}
			if (xmlNode2 != null)
			{
				_RestoreScene = SceneKind.Terminal;
				_RestoreTerminalItemName = XmlUtils.ParseString(xmlNode2.Attributes["ItemName"]);
			}
			DataLocal.Reload();
			DataLocal.Current.SubscribeKeys();
			RestoreStarterPacks();
			return true;
		}

		public static void RestoreTerminalBasketItems()
		{
			if (_RestoreTerminalItemName != null)
			{
				TerminalItemsManager.RestoreTerminalItem(_RestoreTerminalItemName);
				_RestoreTerminalItemName = null;
			}
		}

		private static void RestoreStarterPacks()
		{
			StarterPacksManager.PrepareStarterPacks();
		}
	}
}
