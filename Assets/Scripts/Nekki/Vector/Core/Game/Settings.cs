using System.Collections.Generic;
using System.IO;
using System.Xml;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Grid;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI;
using UnityEngine;

namespace Nekki.Vector.Core.Game
{
	public static class Settings
	{
		public class Visual
		{
			public static bool DrawMusicLabel;

			public static bool ShowDebugGUI;

			public static bool ShowFPS;

			public static bool ShowAverageFPS;

			public static bool ShowChoices;

			public bool Visible = false;

			public Color Border = Color.black;

			public Color Background = Color.black;

			public static Visual Platform = new Visual();

			public static Visual Trigger = new Visual();

			public static Visual Area = new Visual();

			public static Visual Spawn = new Visual();

			public static Visual Camera = new Visual();

			public static Visual Bonuse = new Visual();

			public static Visual Image = new Visual();

			public static Visual Model = new Visual();

			public void Parse(XmlNode p_node)
			{
				if (p_node != null)
				{
					if (p_node.Attributes["Color"] != null)
					{
						Background = ColorUtils.FromHex(p_node.Attributes["Color"].Value);
					}
					if (p_node.Attributes["OutlineColor"] != null)
					{
						Border = ColorUtils.FromHex(p_node.Attributes["OutlineColor"].Value);
					}
					if (p_node.Attributes["Visible"] != null)
					{
						Visible = int.Parse(p_node.Attributes["Visible"].Value) > 0;
					}
				}
			}
		}

		public class GadgetSlot
		{
			public string UnityName { get; private set; }

			public string Name { get; private set; }

			public string Image { get; private set; }

			public int LevelGUIOrder { get; private set; }

			public GadgetSlot(string unityName, string name, string image, int levelGUIOrder)
			{
				UnityName = unityName;
				Name = name;
				Image = image;
				LevelGUIOrder = levelGUIOrder;
			}
		}

		public static class RunGUI
		{
			public static bool ShowCurrency;

			public static bool ShowKeys;

			public static bool ShowGadgets;

			public static bool ShowPause;
		}

		public const string SettingsFile = "settings.xml";

		public const string DefaultSettingsFile = "settings_default.xml";

		private static bool _IsInited = false;

		public static bool IsReleaseBuild;

		public static bool IsServerOn;

		public static readonly Dictionary<string, GadgetSlot> GadgetSlots = new Dictionary<string, GadgetSlot>(5);

		public static string MainRoomPropertyFile;

		public static bool PromtExitOnDeath;

		public static bool WriteGeneratorLogs;

		public static bool WriteRunLogs;

		public static bool PlayInBackground;

		public static bool ReloadMovesOnRestart;

		public static bool AutosaveDemo;

		public static int AutosaveDemoLimit;

		public static int PlayCommandFloor;

		public static bool SaveMeGodMode;

		public static bool UserValidationOn;

		public static bool IsAssetBundleOn;

		private static List<string> _TemporaryCounterNamespaces = new List<string>();

		public static int Version { get; private set; }

		public static void Reset()
		{
			if (File.Exists(VectorPaths.SettingsExternal + "/settings.xml"))
			{
				File.Delete(VectorPaths.SettingsExternal + "/settings.xml");
			}
			_IsInited = false;
			Init();
		}

		private static XmlDocument OpenDocument(ref bool p_fromResources)
		{
			if (!File.Exists(VectorPaths.SettingsExternal + "/settings.xml"))
			{
				p_fromResources = true;
				return XmlUtils.OpenXMLDocument(VectorPaths.Settings, "settings_default.xml");
			}
			p_fromResources = false;
			return XmlUtils.OpenXMLDocument(VectorPaths.SettingsExternal, "settings.xml", XmlUtils.OpenXmlType.ForcedExternal);
		}

		public static void Init()
		{
			if (_IsInited)
			{
				return;
			}
			_IsInited = true;
			bool p_fromResources = false;
			XmlDocument xmlDocument = OpenDocument(ref p_fromResources);
			XmlNode xmlNode = xmlDocument["Settings"]["Build"];
			Version = XmlUtils.ParseInt(xmlDocument["Settings"].Attributes["Version"], -1);
			if (Version != 2)
			{
				Reset();
				return;
			}
			IsReleaseBuild = XmlUtils.ParseBool(xmlNode.Attributes["Release"]);
			ParseConfigSection(xmlNode["Config"]);
			ParseDebugSection(xmlNode["Debug"]);
			ParseGadgetSlotsSection(xmlNode["GadgetSlots"]);
			ParseTemporaryCounterNamespaces(xmlNode["TemporaryCounterNamespaces"]);
            IsAssetBundleOn = false;
            if (!IsReleaseBuild && p_fromResources)
			{
				Save();
				Manager.IsSettingsReset = true;
			}
		}

		public static void Save()
		{
			bool p_fromResources = false;
			XmlDocument xmlDocument = OpenDocument(ref p_fromResources);
			XmlNode xmlNode = xmlDocument["Settings"]["Build"];
			xmlNode.Attributes["Release"].Value = ((!IsReleaseBuild) ? "0" : "1");
			xmlNode["Config"]["Server"].Attributes["IsOn"].Value = ((!IsServerOn) ? "0" : "1");
			xmlNode["Config"]["UserValidationOn"].Attributes["Value"].Value = ((!UserValidationOn) ? "0" : "1");
			xmlNode["Config"]["AssetBundle"].Attributes["IsOn"].Value = ((!IsAssetBundleOn) ? "0" : "1");
			SaveDebugSection(xmlNode["Debug"]);
			xmlDocument.Save(VectorPaths.SettingsExternal + "/settings.xml");
		}

		private static void SaveDebugSection(XmlNode p_debugNode)
		{
			XmlNode xmlNode = p_debugNode["Visual"];
			xmlNode["DebugGUI"].Attributes["Visible"].Value = ((!Visual.ShowDebugGUI) ? "0" : "1");
			xmlNode["FPS"].Attributes["Visible"].Value = ((!Visual.ShowFPS) ? "0" : "1");
			xmlNode["AverageFPS"].Attributes["Visible"].Value = ((!Visual.ShowAverageFPS) ? "0" : "1");
			xmlNode["Choices"].Attributes["Visible"].Value = ((!Visual.ShowChoices) ? "0" : "1");
			p_debugNode["PromtExitOnDeath"].Attributes["Value"].Value = ((!PromtExitOnDeath) ? "0" : "1");
			p_debugNode["WriteGeneratorLogs"].Attributes["Value"].Value = ((!WriteGeneratorLogs) ? "0" : "1");
			p_debugNode["WriteRunLogs"].Attributes["Value"].Value = ((!WriteRunLogs) ? "0" : "1");
			p_debugNode["PlayInBackground"].Attributes["Value"].Value = ((!PlayInBackground) ? "0" : "1");
			p_debugNode["ReloadMovesOnRestart"].Attributes["Value"].Value = ((!ReloadMovesOnRestart) ? "0" : "1");
			p_debugNode["AutosaveDemo"].Attributes["Value"].Value = ((!AutosaveDemo) ? "0" : "1");
			p_debugNode["AutosaveDemoLimit"].Attributes["Value"].Value = AutosaveDemoLimit.ToString();
			p_debugNode["PlayCommandFloor"].Attributes["Value"].Value = PlayCommandFloor.ToString();
			p_debugNode["SaveMeGodMode"].Attributes["Value"].Value = ((!SaveMeGodMode) ? "0" : "1");
			p_debugNode["RunGUI"]["ShowCurrency"].Attributes["Value"].Value = ((!RunGUI.ShowCurrency) ? "0" : "1");
			p_debugNode["RunGUI"]["ShowGadgets"].Attributes["Value"].Value = ((!RunGUI.ShowGadgets) ? "0" : "1");
			p_debugNode["RunGUI"]["ShowKeys"].Attributes["Value"].Value = ((!RunGUI.ShowGadgets) ? "0" : "1");
			p_debugNode["RunGUI"]["ShowPause"].Attributes["Value"].Value = ((!RunGUI.ShowPause) ? "0" : "1");
		}

		private static void ParseConfigSection(XmlNode p_config)
		{
			Model.BoundingBoxSize = XmlUtils.ParseFloat(p_config["Model"].Attributes["BoundingBoxSize"], 300f);
			Model.ModelLayer = XmlUtils.ParseString(p_config["Model"].Attributes["Layer"]);
			XmlAttributeCollection attributes = p_config["Camera"].Attributes;
			Nekki.Vector.Core.Camera.Camera.MinZoom = XmlUtils.ParseFloat(attributes["MinZoom"], 0.1f);
			Nekki.Vector.Core.Camera.Camera.CurrentZoom = XmlUtils.ParseFloat(attributes["CurrZoom"], 0.5f);
			Nekki.Vector.Core.Camera.Camera.MaxZoom = XmlUtils.ParseFloat(attributes["MaxZoom"], 1.3f);
			Nekki.Vector.Core.Camera.Camera.SpeedZoom = XmlUtils.ParseFloat(attributes["MaxSpeed"], 0.5f);
			Nekki.Vector.Core.Camera.Camera.Fluency = XmlUtils.ParseFloat(attributes["Fluency"], 2f);
			Nekki.Vector.Core.Camera.Camera.BaseMagicNumber = XmlUtils.ParseFloat(attributes["BaseMagicNumber"], 250f);
			Nekki.Vector.Core.Camera.Camera.HorizonNumber = XmlUtils.ParseFloat(attributes["HorizonNumber"], 300f);
			Nekki.Vector.Core.Camera.Camera.StickingSpeed = XmlUtils.ParseFloat(attributes["StickingSpeed"], 30f);
			XmlAttributeCollection attributes2 = p_config["Money"].Attributes;
			DataLocal.Money1Name = XmlUtils.ParseString(attributes2["Money1"]);
			DataLocal.Money2Name = XmlUtils.ParseString(attributes2["Money2"]);
			DataLocal.Money3Name = XmlUtils.ParseString(attributes2["Money3"]);
			XmlAttributeCollection attributes3 = p_config["Music"].Attributes;
			AudioManager.StopOnDeath = XmlUtils.ParseBool(attributes3["StopOnDeath"]);
			AudioManager.StopOnPause = XmlUtils.ParseBool(attributes3["StopOnPause"]);
			Nekki.Vector.Core.Grid.Grid.CellWidth = XmlUtils.ParseInt(p_config["Grid"].Attributes["CellWidth"], Nekki.Vector.Core.Grid.Grid.CellWidth);
			Nekki.Vector.Core.Grid.Grid.CellHeight = XmlUtils.ParseInt(p_config["Grid"].Attributes["CellHeight"], Nekki.Vector.Core.Grid.Grid.CellHeight);
			LocationGenerator.MaxAttemptCount = XmlUtils.ParseInt(p_config["Generator"].Attributes["MaxAttemptCount"], LocationGenerator.MaxAttemptCount);
			IsServerOn = IsReleaseBuild || XmlUtils.ParseBool(p_config["Server"].Attributes["IsOn"]);
			UserValidationOn = IsReleaseBuild || p_config["UserValidationOn"] == null || XmlUtils.ParseBool(p_config["UserValidationOn"].Attributes["Value"], true);
			IsAssetBundleOn = IsReleaseBuild || p_config["AssetBundle"] == null || XmlUtils.ParseBool(p_config["AssetBundle"].Attributes["IsOn"], true);
		}

		private static void ParseDebugSection(XmlNode p_debugNode)
		{
			XmlNode xmlNode = p_debugNode["Visual"];
			Visual.Area.Parse(xmlNode["Areas"]);
			Visual.Platform.Parse(xmlNode["Platforms"]);
			Visual.Trigger.Parse(xmlNode["Triggers"]);
			Visual.Camera.Parse(xmlNode["Cameras"]);
			Visual.Spawn.Parse(xmlNode["Spawns"]);
			Visual.Bonuse.Parse(xmlNode["Bonuses"]);
			Visual.Image.Parse(xmlNode["Images"]);
			Visual.Model.Parse(xmlNode["Model"]);
			Visual.DrawMusicLabel = XmlUtils.ParseBool(xmlNode["SoundtrackName"].Attributes["Visible"], true);
			Visual.ShowDebugGUI = XmlUtils.ParseBool(xmlNode["DebugGUI"].Attributes["Visible"]);
			Visual.ShowFPS = xmlNode["FPS"] == null || XmlUtils.ParseBool(xmlNode["FPS"].Attributes["Visible"]);
			Visual.ShowAverageFPS = XmlUtils.ParseBool(xmlNode["AverageFPS"].Attributes["Visible"]);
			Visual.ShowChoices = XmlUtils.ParseBool(xmlNode["Choices"].Attributes["Visible"], true);
			PromtExitOnDeath = XmlUtils.ParseBool(p_debugNode["PromtExitOnDeath"].Attributes["Value"]);
			WriteGeneratorLogs = XmlUtils.ParseBool(p_debugNode["WriteGeneratorLogs"].Attributes["Value"]);
			WriteRunLogs = XmlUtils.ParseBool(p_debugNode["WriteRunLogs"].Attributes["Value"]);
			PlayInBackground = XmlUtils.ParseBool(p_debugNode["PlayInBackground"].Attributes["Value"]);
			ReloadMovesOnRestart = XmlUtils.ParseBool(p_debugNode["ReloadMovesOnRestart"].Attributes["Value"]);
			AutosaveDemo = XmlUtils.ParseBool(p_debugNode["AutosaveDemo"].Attributes["Value"]);
			AutosaveDemoLimit = XmlUtils.ParseInt(p_debugNode["AutosaveDemoLimit"].Attributes["Value"]);
			PlayCommandFloor = ((p_debugNode["PlayCommandFloor"] != null) ? XmlUtils.ParseInt(p_debugNode["PlayCommandFloor"].Attributes["Value"]) : 0);
			SaveMeGodMode = XmlUtils.ParseBool(p_debugNode["SaveMeGodMode"].Attributes["Value"]);
			XmlNode xmlNode2 = p_debugNode["RunGUI"];
			RunGUI.ShowCurrency = XmlUtils.ParseBool(xmlNode2["ShowCurrency"].Attributes["Value"]);
			RunGUI.ShowGadgets = XmlUtils.ParseBool(xmlNode2["ShowGadgets"].Attributes["Value"]);
			RunGUI.ShowKeys = XmlUtils.ParseBool(xmlNode2["ShowKeys"].Attributes["Value"]);
			RunGUI.ShowPause = XmlUtils.ParseBool(xmlNode2["ShowPause"].Attributes["Value"]);
			WriteRunLogs = false;
			WriteGeneratorLogs = false;
			AutosaveDemo = false;
			if (IsReleaseBuild)
			{
				Visual.DrawMusicLabel = false;
				Visual.ShowFPS = false;
				Visual.ShowAverageFPS = false;
			}
		}

		private static void ParseGadgetSlotsSection(XmlNode p_slotsNode)
		{
			GadgetSlots.Clear();
			foreach (XmlNode childNode in p_slotsNode.ChildNodes)
			{
				string text = XmlUtils.ParseString(childNode.Attributes["UnityName"]);
				string text2 = XmlUtils.ParseString(childNode.Attributes["Name"]);
				string image = XmlUtils.ParseString(childNode.Attributes["Image"], "/ui/items/gadget_slot_X.png".Replace("X", text2.ToLower()));
				int levelGUIOrder = XmlUtils.ParseInt(childNode.Attributes["LevelGUIOrder"]);
				GadgetSlots.Add(text, new GadgetSlot(text, text2, image, levelGUIOrder));
			}
		}

		private static void ParseTemporaryCounterNamespaces(XmlNode p_node)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				_TemporaryCounterNamespaces.Add(childNode.Attributes["Name"].Value);
			}
		}

		public static bool IsTemporaryCounterNamespaces(string p_name)
		{
			return _TemporaryCounterNamespaces.Contains(p_name);
		}
	}
}
