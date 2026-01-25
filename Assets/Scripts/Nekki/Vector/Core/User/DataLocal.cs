using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Utils;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.DataValidation;
using Nekki.Vector.Core.GameCenter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Payment;
using UnityEngine;

namespace Nekki.Vector.Core.User
{
	public class DataLocal : AbstractRoster
	{
		public delegate void OnMoneyDelegate();

		public delegate void OnItemDelegate(Action Action, UserItem UserItem, int Value);

		private const string _Filename = "user.xml";

		private const string _LocalBackupFilename = "user.backup";

		private const string _DefaultFile = "user_default.xml";

		public static string Money1Name;

		public static string Money2Name;

		public static string Money3Name;

		private static DataLocal _Current;

		private string _Name = "Player";

		private static int _UserID = -1;

		private int _InstallID;

		private int _GamedataVersion;

		private int _Version;

		private bool _IsPaidVersion;

		private UserSettings _UserSettings;

		private readonly HashSet<UserItem> _Stash = new HashSet<UserItem>();

		private readonly HashSet<UserItem> _Equipped = new HashSet<UserItem>();

		private readonly HashSet<UserItem> _AllItems = new HashSet<UserItem>();

		private readonly List<UserItem> _ExpirationByFrame = new List<UserItem>();

		private CounterController _CounterController;

		private readonly Dictionary<string, UserProperty> _UserProperties = new Dictionary<string, UserProperty>();

		private static bool _InitTabu = true;

		private static int _DefaultUserVersion = -1;

		private static DataLocal _BackUpForDemo;

		private static DataLocal _BackUpForLocalBackup;

		private static bool _UserDontSave;

		public ObscuredInt Money1
		{
			get
			{
				return GetMoneyQuantity(Money1Name);
			}
			set
			{
				SetMoneyQuantity(value, Money1Name);
			}
		}

		public ObscuredInt Money2
		{
			get
			{
				return GetMoneyQuantity(Money2Name);
			}
			set
			{
				SetMoneyQuantity(value, Money2Name);
			}
		}

		public ObscuredInt Money3
		{
			get
			{
				return GetMoneyQuantity(Money3Name);
			}
			set
			{
				SetMoneyQuantity(value, Money3Name);
			}
		}

		public static DataLocal Current
		{
			get
			{
				if (_Current == null)
				{
					_Current = new DataLocal();
					_Current.Init();
				}
				return _Current;
			}
		}

		public static bool IsCurrentExists
		{
			get
			{
				return _Current != null;
			}
		}

		public static int UserID
		{
			get
			{
				return _UserID;
			}
			set
			{
				_UserID = value;
			}
		}

		public int InstallID
		{
			get
			{
				return _InstallID;
			}
		}

		public int GamedataVersion
		{
			get
			{
				return _GamedataVersion;
			}
			set
			{
				_GamedataVersion = value;
				Save(true);
			}
		}

		public int Version
		{
			set
			{
				_Version = value;
			}
		}

		public bool IsPaidVersion
		{
			get
			{
				return _IsPaidVersion;
			}
			set
			{
				_IsPaidVersion = value;
			}
		}

		public UserSettings Settings
		{
			get
			{
				return _UserSettings;
			}
		}

		public HashSet<UserItem> Stash
		{
			get
			{
				return _Stash;
			}
		}

		public HashSet<UserItem> Equipped
		{
			get
			{
				return _Equipped;
			}
		}

		public HashSet<UserItem> AllItems
		{
			get
			{
				return _AllItems;
			}
		}

		public CounterController CounterController
		{
			get
			{
				return _CounterController;
			}
		}

		public int StateRun
		{
			get
			{
				return Current.GetOrCreateUserPropertyByName("States").ValueInt("Run");
			}
			set
			{
				Current.GetOrCreateUserPropertyByName("States").SetValue("Run", value);
			}
		}

		public static bool InitTabu
		{
			get
			{
				return _InitTabu;
			}
			set
			{
				_InitTabu = value;
			}
		}

		private static int DefaultUserVersion
		{
			get
			{
				if (_DefaultUserVersion == -1)
				{
					XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.GameData, "user_default.xml");
					_DefaultUserVersion = int.Parse(xmlDocument["User"].Attributes["Version"].Value);
				}
				return _DefaultUserVersion;
			}
		}

		public static string FilePath
		{
			get
			{
				return VectorPaths.StorageUserData + "/user.xml";
			}
		}

		public static string LocalBackupPath
		{
			get
			{
				return VectorPaths.StorageUserData + "/user.backup";
			}
		}

		public static bool UserDontSave
		{
			get
			{
				return _UserDontSave;
			}
			set
			{
				if (_UserDontSave != value)
				{
					_UserDontSave = value;
					if (_BackUpForLocalBackup != null)
					{
						_BackUpForLocalBackup.FreeStatic();
					}
					if (_UserDontSave)
					{
						_BackUpForLocalBackup = new DataLocal(_Current);
					}
					else
					{
						_BackUpForLocalBackup = null;
					}
				}
			}
		}

		public event OnMoneyDelegate OnMoney;

		public event OnItemDelegate OnItem;

		private DataLocal()
		{
			_CounterController = new CounterController();
			_UserSettings = new UserSettings();
			LocalizationManager.OnLanguageChanged += OnLanguageChanged;
			ResolutionManager.OnResolutionChanged += OnResolutionChanged;
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
		}

		private DataLocal(DataLocal p_copy)
		{
			_Name = p_copy._Name;
			_UserSettings = p_copy._UserSettings;
			_CounterController = p_copy._CounterController.Copy();
			_Version = p_copy._Version;
			_GamedataVersion = p_copy._GamedataVersion;
			_InstallID = p_copy._InstallID;
			_IsPaidVersion = p_copy._IsPaidVersion;
			LocalizationManager.OnLanguageChanged += OnLanguageChanged;
			ResolutionManager.OnResolutionChanged += OnResolutionChanged;
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
			foreach (KeyValuePair<string, UserProperty> userProperty in p_copy._UserProperties)
			{
				_UserProperties.Add(userProperty.Key, userProperty.Value.Copy());
			}
			foreach (UserItem item in p_copy._Stash)
			{
				if (item.IsForceSave)
				{
					AddToStash(item);
				}
				else
				{
					AddToStash(item.Copy());
				}
			}
			foreach (UserItem item2 in p_copy._Equipped)
			{
				AddToEquipped(item2.Copy());
			}
		}

		public void FreeStatic()
		{
			LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
			ResolutionManager.OnResolutionChanged -= OnResolutionChanged;
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
			this.OnItem = null;
			this.OnMoney = null;
			foreach (UserItem allItem in _AllItems)
			{
				allItem.OnChange -= OnItemChange;
			}
			FreeTimer();
			Clear();
		}

		~DataLocal()
		{
		}

		private void OnLanguageChanged(SystemLanguage p_language)
		{
			if (_UserSettings.CurrentLanguage != p_language)
			{
				_UserSettings.CurrentLanguage = p_language;
				Save(false);
                SaveForcedToFile();
            }
        }

        private void OnResolutionChanged(Resolution resolution)
        {
			var currentRes = _UserSettings.CurrentResolution;
			if (currentRes.width != resolution.width || currentRes.height != resolution.height || currentRes.refreshRateRatio.value != resolution.refreshRateRatio.value)
            {
                _UserSettings.CurrentResolution = resolution;
                Save(false);
				SaveForcedToFile();
            }
        }

        private void OnAuthenticate(bool p_authed)
		{
			Settings.GameCenterOn = p_authed;
			SaveForcedToFile();
		}

		public void AddToStash(UserItem p_item)
		{
			if (p_item == null || _AllItems.Contains(p_item))
			{
				return;
			}
			foreach (UserItem allItem in _AllItems)
			{
				if (allItem.Name == p_item.Name)
				{
					allItem.AppendUserItem(p_item);
					return;
				}
			}
			if (p_item.IsForceSave && _BackUpForLocalBackup != null && _BackUpForLocalBackup != this)
			{
				_BackUpForLocalBackup.AddToStash(p_item);
			}
			_AllItems.Add(p_item);
			_Stash.Add(p_item);
			p_item.OnChange += OnItemChange;
			if (this.OnItem != null)
			{
				this.OnItem(Action.Add, p_item, 0);
			}
		}

		public void MoveToStash(UserItem p_item)
		{
			if (_Equipped.Contains(p_item))
			{
				_Stash.Add(p_item);
				_Equipped.Remove(p_item);
				if (p_item.IsExpirationByFrame)
				{
					_ExpirationByFrame.Remove(p_item);
				}
			}
		}

		public void SubscribeKeys()
		{
			foreach (UserItem item in _Equipped)
			{
				if (item.ContainsGroup("Key"))
				{
					item.OnChange += OnItemChange;
				}
			}
		}

		private static UserItem GetItemByName(string p_name, HashSet<UserItem> _Set)
		{
			foreach (UserItem item in _Set)
			{
				if (item.Name == p_name)
				{
					return item;
				}
			}
			return null;
		}

		public UserItem GetItemByName(string p_name)
		{
			return GetItemByName(p_name, _AllItems);
		}

		public UserItem GetItemByNameFromStash(string p_name)
		{
			return GetItemByName(p_name, _Stash);
		}

		public UserItem GetItemByNameFromEquipped(string p_name)
		{
			return GetItemByName(p_name, _Equipped);
		}

		public void Remove(UserItem Item)
		{
			if (_AllItems.Contains(Item))
			{
				OnItemChange(Action.Remove, Item, 0);
				_AllItems.Remove(Item);
				if (_Equipped.Contains(Item))
				{
					_Equipped.Remove(Item);
				}
				if (_Stash.Contains(Item))
				{
					_Stash.Remove(Item);
				}
				Item.OnChange -= OnItemChange;
				Save(true);
			}
		}

		public void AddToEquipped(UserItem p_item)
		{
			if (p_item == null || _AllItems.Contains(p_item))
			{
				return;
			}
			foreach (UserItem allItem in _AllItems)
			{
				if (allItem.Name == p_item.Name)
				{
					allItem.AppendUserItem(p_item);
					return;
				}
			}
			if (p_item.IsForceSave && _BackUpForLocalBackup != null && _BackUpForLocalBackup != this)
			{
				_BackUpForLocalBackup.AddToEquipped(p_item);
			}
			_AllItems.Add(p_item);
			_Equipped.Add(p_item);
			if (p_item.IsExpirationByFrame)
			{
				_ExpirationByFrame.Add(p_item);
			}
			p_item.OnChange += OnItemChange;
			if (this.OnItem != null)
			{
				this.OnItem(Action.Add, p_item, 0);
			}
		}

		public void MoveToEquipped(UserItem p_item)
		{
			if (_Stash.Contains(p_item))
			{
				_Equipped.Add(p_item);
				_Stash.Remove(p_item);
				if (p_item.IsExpirationByFrame)
				{
					_ExpirationByFrame.Add(p_item);
				}
			}
		}

		public void UserBuyItem(UserItem p_item, bool p_toEqupped)
		{
			p_item.ExtraActionsOnBuy();
			if (p_toEqupped)
			{
				AddToEquipped(p_item);
			}
			else
			{
				AddToStash(p_item);
			}
		}

		public UserProperty GetUserPropertyByName(string p_name)
		{
			if (!_UserProperties.ContainsKey(p_name))
			{
				return null;
			}
			return _UserProperties[p_name];
		}

		public UserProperty GetOrCreateUserPropertyByName(string p_name)
		{
			if (!_UserProperties.ContainsKey(p_name))
			{
				UserProperty userProperty = new UserProperty(p_name);
				_UserProperties.Add(p_name, userProperty);
				return userProperty;
			}
			return _UserProperties[p_name];
		}

		public void Clear()
		{
			_Equipped.Clear();
			_Stash.Clear();
			_AllItems.Clear();
			_CounterController.Clear();
			_UserProperties.Clear();
			_UserSettings = null;
		}

		public void ClearTemporaryItems()
		{
			List<UserItem> list = new List<UserItem>(_AllItems);
			foreach (UserItem item in list)
			{
				if (!item.IsSaved)
				{
					Remove(item);
				}
			}
		}

		public void OnItemChange(Action Action, UserItem Item, int Value)
		{
			if (this.OnItem != null)
			{
				this.OnItem(Action, Item, Value);
			}
		}

		public void OnStartFrame()
		{
			if (_ExpirationByFrame.Count == 0)
			{
				return;
			}
			int num = _ExpirationByFrame.Count - 1;
			for (int num2 = num; num2 >= 0; num2--)
			{
				UserItem userItem = _ExpirationByFrame[num2];
				userItem.ExpirationCounter--;
				if (userItem.ExpirationCounter <= 0)
				{
					Remove(userItem);
					_ExpirationByFrame.Remove(userItem);
				}
			}
		}

		public void OnStartRun()
		{
			List<UserItem> list = new List<UserItem>();
			foreach (UserItem item in _Equipped)
			{
				if (item.IsExpirationByTerminal)
				{
					item.ExpirationCounter--;
					if (item.ExpirationCounter < 0)
					{
						list.Add(item);
					}
				}
			}
			foreach (UserItem item2 in list)
			{
				Remove(item2);
			}
		}

		public void OnTerminal()
		{
			List<UserItem> list = new List<UserItem>();
			foreach (UserItem item in _Equipped)
			{
				if (item.IsExpirationByTerminal && item.ExpirationCounter == 0)
				{
					list.Add(item);
				}
			}
			foreach (UserItem item2 in list)
			{
				Remove(item2);
			}
		}

		public void OnEndRun()
		{
			List<UserItem> list = new List<UserItem>();
			foreach (UserItem allItem in _AllItems)
			{
				if (allItem.IsExpirationByRun)
				{
					list.Add(allItem);
				}
			}
			foreach (UserItem item in list)
			{
				Remove(item);
			}
		}

		public void OnStartFloor()
		{
			List<UserItem> list = new List<UserItem>();
			foreach (UserItem item in _Equipped)
			{
				if (item.IsExpirationByFloor)
				{
					item.ExpirationCounter--;
					if (item.ExpirationCounter < 0)
					{
						list.Add(item);
					}
				}
			}
			foreach (UserItem item2 in list)
			{
				Remove(item2);
			}
		}

		public void OnEndFloor()
		{
			List<UserItem> list = new List<UserItem>();
			foreach (UserItem item in _Equipped)
			{
				if (item.IsExpirationByFloor && item.ExpirationCounter == 0)
				{
					list.Add(item);
				}
			}
			foreach (UserItem item2 in list)
			{
				Remove(item2);
			}
		}

		public bool IsItemOnEquipped(UserItem p_item)
		{
			return _Equipped.Contains(p_item);
		}

		public void AppendMoneyQuantity(ObscuredInt value, string p_moneyType)
		{
			UserItem orCreateMoneyItem = GetOrCreateMoneyItem(p_moneyType);
			orCreateMoneyItem.SetValue(orCreateMoneyItem.GetIntValueAttribute("Quantity", "Countable") + (int)value, "Quantity", "Countable");
			if (this.OnMoney != null)
			{
				this.OnMoney();
			}
		}

		public void SetMoneyQuantity(ObscuredInt value, string p_moneyType)
		{
			UserItem orCreateMoneyItem = GetOrCreateMoneyItem(p_moneyType);
			orCreateMoneyItem.SetValue(value, "Quantity", "Countable");
			if (this.OnMoney != null)
			{
				this.OnMoney();
			}
		}

		public ObscuredInt GetMoneyQuantity(string p_moneyType)
		{
			UserItem itemByNameFromStash = GetItemByNameFromStash(p_moneyType);
			if (itemByNameFromStash != null)
			{
				return itemByNameFromStash.GetIntValueAttribute("Quantity", "Countable");
			}
			return 0;
		}

		private UserItem GetOrCreateMoneyItem(string p_moneyType)
		{
			UserItem userItem = GetItemByNameFromStash(p_moneyType);
			if (userItem == null)
			{
				Preset presetByName = PresetsManager.GetPresetByName(p_moneyType);
				PresetResult presetResult = presetByName.RunPreset();
				userItem = presetResult.Item;
			}
			return userItem;
		}

		public static void SetupDemoBackup()
		{
			if (_BackUpForDemo != null)
			{
				_BackUpForDemo.FreeStatic();
			}
			_BackUpForDemo = new DataLocal(_Current);
		}

		public static void LoadDemoBackUp()
		{
			if (_Current != null)
			{
				_Current.FreeStatic();
			}
			_Current = new DataLocal(_BackUpForDemo);
		}

		public static void SaveDemoBackUp(XmlElement p_userNode)
		{
			_BackUpForDemo.SaveToUserNode(p_userNode);
		}

		public static void SaveCurrent(XmlElement p_userNode)
		{
			Current.SaveToUserNode(p_userNode);
		}

		public static void Reset()
		{
			_Current = null;
			FileUtils.DeleteFileAndHash(FilePath);
			FileUtils.DeleteFileAndHash(LocalBackupPath);
			GameRestorer.RemoveBackup();
			AchievementsManager.SyncAchievements();
			SetDefaultFile();
		}

		public static void MoveToCorruptedAndReset()
		{
			_Current = null;
			FileUtils.MoveToCorruptedFileAndHash(LocalBackupPath);
			FileUtils.MoveToCorruptedFileAndHash(FilePath);
			GameRestorer.RemoveBackup();
			SetDefaultFile();
		}

		public static void Reload()
		{
			_Current = null;
			_Current = Current;
		}

		public static void ReloadFromXmlNode(XmlNode p_userNode)
		{
			_Current = new DataLocal();
			_Current.LoadFromUserNode(p_userNode);
		}

		private void Init()
		{
			if (_InitTabu)
			{
				DebugUtils.Dialog("DataLocal.Init Tabu!", false);
				return;
			}
			PresetsManager.Init();
			ZoneManager.Init();
			if (FileUtils.FileExists(FilePath))
			{
				LoadFromFile();
			}
			else
			{
				LoadFromFile();
			}
		}

		private static void SetDefaultFile()
		{
			XmlUtils.CopyXmlFromResourcesAndUpdateHash(VectorPaths.GameData + "/user_default.xml", FilePath);
		}

		protected override void SaveToFile()
		{
			if (!_UserDontSave)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
				XmlElement xmlElement = xmlDocument.CreateElement("User");
				xmlDocument.AppendChild(xmlElement);
				SaveToUserNode(xmlElement);
				XmlUtils.SaveDocumentAndUpdateHash(xmlDocument, FilePath);
			}
		}

		public string SaveToString()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
			XmlElement xmlElement = xmlDocument.CreateElement("User");
			xmlDocument.AppendChild(xmlElement);
			SaveToUserNode(xmlElement);
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = true;
			xmlWriterSettings.Indent = true;
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
			{
				xmlDocument.Save(xmlWriter);
			}
			return stringBuilder.ToString();
		}

		public void DumpSave()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
			XmlElement xmlElement = xmlDocument.CreateElement("User");
			xmlDocument.AppendChild(xmlElement);
			SaveToUserNode(xmlElement);
			XmlUtils.SaveDocumentAndUpdateHash(xmlDocument, VectorPaths.StorageUserData + "/user_dump.xml");
		}

		private void SaveToUserNode(XmlElement p_userNode)
		{
			p_userNode.SetAttribute("Name", _Name);
			p_userNode.SetAttribute("ID", _UserID.ToString());
			p_userNode.SetAttribute("InstallID", _InstallID.ToString());
			p_userNode.SetAttribute("Version", _Version.ToString());
			if (_IsPaidVersion)
			{
				p_userNode.SetAttribute("PaidVersion", "1");
			}
			SaveGamedataVersion(p_userNode);
			SaveSettings(p_userNode);
			SaveUserTags(p_userNode);
			SaveUserProperties(p_userNode);
			SaveUserItems(p_userNode);
			SavePayments(p_userNode);
			SaveBundles(p_userNode);
		}

		private void SaveGamedataVersion(XmlNode p_rootNode)
		{
			XmlElement xmlElement = p_rootNode.OwnerDocument.CreateElement("Build");
			p_rootNode.AppendChild(xmlElement);
			xmlElement.SetAttribute("Version", ApplicationController.BuildVersion);
			xmlElement.SetAttribute("GamedataVersion", _GamedataVersion.ToString());
		}

		private void SaveSettings(XmlNode p_rootNode)
		{
			XmlNode xmlNode = p_rootNode.OwnerDocument.CreateElement("Settings");
			p_rootNode.AppendChild(xmlNode);
			XmlElement xmlElement = p_rootNode.OwnerDocument.CreateElement("Audio");
			xmlNode.AppendChild(xmlElement);
			xmlElement.SetAttribute("MuteMusic", (!_UserSettings.MuteMusic) ? "0" : "1");
			xmlElement.SetAttribute("MuteSound", (!_UserSettings.MuteSound) ? "0" : "1");
			xmlElement.SetAttribute("VolumeSound", _UserSettings.VolumeSound.ToString());
			xmlElement.SetAttribute("VolumeMusic", _UserSettings.VolumeMusic.ToString());
			XmlElement xmlElement2 = p_rootNode.OwnerDocument.CreateElement("Localization");
			xmlNode.AppendChild(xmlElement2);
			xmlElement2.SetAttribute("CurrentLanguage", _UserSettings.CurrentLanguage.ToString());
			XmlElement xmlElement3 = p_rootNode.OwnerDocument.CreateElement("Other");
			xmlNode.AppendChild(xmlElement3);
			xmlElement3.SetAttribute("UseLowResGraphics", (!_UserSettings.UseLowResGraphics) ? "0" : "1");
			xmlElement3.SetAttribute("SubtitlesOn", (!_UserSettings.SubtitlesOn) ? "0" : "1");
			xmlElement3.SetAttribute("GameCenterOn", (!_UserSettings.GameCenterOn) ? "0" : "1");
			xmlElement3.SetAttribute("Fullscreen", (!_UserSettings.Fullscreen) ? "0" : "1");
			xmlElement3.SetAttribute("Resolution", _UserSettings.CurrentResolution.ToString());

		}

		private void SaveUserTags(XmlNode p_rootNode)
		{
			_CounterController.SaveToXml(p_rootNode);
		}

		private void SaveUserProperties(XmlNode p_rootNode)
		{
			XmlNode xmlNode = p_rootNode.OwnerDocument.CreateElement("UserProperties");
			p_rootNode.AppendChild(xmlNode);
			foreach (UserProperty value in _UserProperties.Values)
			{
				value.SaveToXml(xmlNode);
			}
		}

		private void SaveUserItems(XmlNode p_rootNode)
		{
			XmlElement xmlElement = p_rootNode.OwnerDocument.CreateElement("Items");
			p_rootNode.AppendChild(xmlElement);
			XmlElement xmlElement2 = p_rootNode.OwnerDocument.CreateElement("Stash");
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = p_rootNode.OwnerDocument.CreateElement("Equipped");
			xmlElement.AppendChild(xmlElement3);
			foreach (UserItem item in _Stash)
			{
				item.SaveToXmlNode(xmlElement2);
			}
			foreach (UserItem item2 in _Equipped)
			{
				item2.SaveToXmlNode(xmlElement3);
			}
		}

		private void SavePayments(XmlNode p_rootNode)
		{
			PaymentController.SavePayments(p_rootNode);
		}

		private void SaveBundles(XmlNode p_rootNode)
		{
			BundleManager.Save(p_rootNode);
		}

		public void SaveForcedToFile()
		{
			if (_UserDontSave)
			{
				_UserDontSave = false;
				_BackUpForLocalBackup.Save(true);
				_UserDontSave = true;
			}
			else
			{
				SaveToFile();
			}
		}

		public void SaveLocalBackup()
		{
			if (_UserDontSave)
			{
				_UserDontSave = false;
				_BackUpForLocalBackup.Save(true);
				_UserDontSave = true;
			}
			FileUtils.CopyFileAndHash(FilePath, LocalBackupPath);
		}

		public void LoadFromFile()
		{
			XmlNode xmlNode = null;
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(FilePath, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
			if (xmlDocument == null || !UserDataValidator.IsValid)
			{
				xmlDocument = XmlUtils.OpenXMLDocumentAndCheckHash(LocalBackupPath, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
				if (xmlDocument == null || !UserDataValidator.IsValid)
				{
					MoveToCorruptedAndReset();
					LoadFromFile();
					return;
				}
			}
			xmlNode = xmlDocument["User"];
			if (xmlNode == null)
			{
				Reset();
				return;
			}
			_Version = XmlUtils.ParseInt(xmlNode.Attributes["Version"]);
			if (_Version < DefaultUserVersion)
			{
				GameRestorer.RemoveBackup();
				UserUpdater.RunRegularExpression(_Version, DefaultUserVersion);
				xmlDocument = XmlUtils.OpenXMLDocument(FilePath, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
				xmlNode = xmlDocument["User"];
			}
			LoadFromUserNode(xmlNode);
			if (_Version < DefaultUserVersion)
			{
				UserUpdater.Run(_Version, DefaultUserVersion);
			}
		}

		private void LoadFromUserNode(XmlNode p_userNode)
		{
			_Name = p_userNode.Attributes["Name"].Value;
			if (_UserID == -1)
			{
				_UserID = XmlUtils.ParseInt(p_userNode.Attributes["ID"], _UserID);
			}
			_InstallID = XmlUtils.ParseInt(p_userNode.Attributes["InstallID"], (int)new RandomGenerator((uint)DateTime.Now.Millisecond).getRandom());
			if (_InstallID == 0)
			{
				_InstallID = (int)new RandomGenerator((uint)DateTime.Now.Millisecond).getRandom();
			}
			_Version = XmlUtils.ParseInt(p_userNode.Attributes["Version"]);
			_IsPaidVersion = true;
			LoadGamedataVersion(p_userNode["Build"]);
			LoadSettings(p_userNode["Settings"]);
			LoadUserCounters(p_userNode["UserCounters"]);
			LoadUserProperties(p_userNode["UserProperties"]);
			LoadUserItems(p_userNode);
			LoadPayments(p_userNode);
			LoadBundles(p_userNode);
			LoadCoupons();
			LoadBoosterPacks();
			LoadTimers();
			LoadZones();
		}

		private void LoadGamedataVersion(XmlNode p_node)
		{
			if (p_node != null)
			{
				string text = XmlUtils.ParseString(p_node.Attributes["Version"]);
				if (text != ApplicationController.BuildVersion)
				{
					_GamedataVersion = ResourcesValidator.GamedataVersion;
				}
				else
				{
					_GamedataVersion = XmlUtils.ParseInt(p_node.Attributes["GamedataVersion"]);
				}
			}
		}

		private void LoadSettings(XmlNode p_settingsNode)
		{
			_UserSettings.VolumeSound = XmlUtils.ParseFloat(p_settingsNode["Audio"].Attributes["VolumeSound"]);
			_UserSettings.VolumeMusic = XmlUtils.ParseFloat(p_settingsNode["Audio"].Attributes["VolumeMusic"]);
			_UserSettings.MuteMusic = XmlUtils.ParseBool(p_settingsNode["Audio"].Attributes["MuteMusic"]);
			_UserSettings.MuteSound = XmlUtils.ParseBool(p_settingsNode["Audio"].Attributes["MuteSound"]);
			_UserSettings.CurrentLanguage = XmlUtils.ParseEnum(p_settingsNode["Localization"].Attributes["CurrentLanguage"], LocalizationManager.DefaultLanguage);
			_UserSettings.UseLowResGraphics = XmlUtils.ParseBool(p_settingsNode["Other"].Attributes["UseLowResGraphics"], DeviceDetector.UseLowResolution);
			_UserSettings.SubtitlesOn = XmlUtils.ParseBool(p_settingsNode["Other"].Attributes["SubtitlesOn"], true);
			_UserSettings.GameCenterOn = XmlUtils.ParseBool(p_settingsNode["Other"].Attributes["GameCenterOn"], true);
            _UserSettings.Fullscreen = XmlUtils.ParseBool(p_settingsNode["Other"].Attributes["Fullscreen"], Screen.fullScreen);
            _UserSettings.CurrentResolution = XmlUtils.ParseResolution(p_settingsNode["Other"].Attributes["Resolution"], ResolutionManager.DefaultResolution);
        }

		private void LoadUserCounters(XmlNode p_rootNode)
		{
			_CounterController.LoadFromXml(p_rootNode);
		}

		private void LoadUserProperties(XmlNode p_propertyNode)
		{
			if (p_propertyNode == null)
			{
				return;
			}
			foreach (XmlNode childNode in p_propertyNode.ChildNodes)
			{
				UserProperty userProperty = new UserProperty(childNode);
				_UserProperties.Add(userProperty.Name, userProperty);
			}
		}

		private void LoadUserItems(XmlNode p_rootNode)
		{
			XmlNode xmlNode = p_rootNode["Items"]["Stash"];
			XmlNode xmlNode2 = p_rootNode["Items"]["Equipped"];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				UserItem userItem = UserItem.CreateByXmlNode(childNode);
				if (userItem != null)
				{
					_Stash.Add(userItem);
					_AllItems.Add(userItem);
				}
			}
			foreach (XmlNode childNode2 in xmlNode2.ChildNodes)
			{
				UserItem userItem2 = UserItem.CreateByXmlNode(childNode2);
				if (userItem2 != null)
				{
					_Equipped.Add(userItem2);
					_AllItems.Add(userItem2);
				}
			}
		}

		private void LoadPayments(XmlNode p_rootNode)
		{
			PaymentController.LoadPayments(p_rootNode);
		}

		private void LoadBundles(XmlNode p_rootNode)
		{
			BundleManager.Load(p_rootNode);
		}

		private void LoadCoupons()
		{
			CouponsManager.Init();
		}

		private void LoadBoosterPacks()
		{
			BoosterpacksManager.Init();
		}

		private void LoadTimers()
		{
			TimersManager.Init();
		}

		private void LoadZones()
		{
			ZoneManager.LoadSettings();
		}
	}
}
