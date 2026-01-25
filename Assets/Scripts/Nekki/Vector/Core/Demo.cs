using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.InputControllers;
using UnityEngine;

namespace Nekki.Vector.Core
{
	public static class Demo
	{
		public enum StateType
		{
			Idle = 0,
			Recording = 1,
			Playing = 2
		}

		private static readonly Dictionary<uint, KeyVariables> _KeyVariablesByFrames = new Dictionary<uint, KeyVariables>();

		private static int _EndFrame;

		private static int _CurrentFloor;

		private static StateType _CurrentState = StateType.Idle;

		public static bool IsPlaying
		{
			get
			{
				return _CurrentState == StateType.Playing;
			}
		}

		public static void StartRecording()
		{
			_CurrentState = StateType.Recording;
			_CurrentFloor = CounterController.Current.CounterFloor;
			_KeyVariablesByFrames.Clear();
		}

		public static void ResetStatus()
		{
			if (_CurrentState == StateType.Playing)
			{
				DataLocal.LoadDemoBackUp();
			}
			_CurrentState = StateType.Idle;
			_KeyVariablesByFrames.Clear();
		}

		public static void Simulate(Scene scene, uint frame)
		{
			if (_CurrentState == StateType.Playing && _KeyVariablesByFrames.ContainsKey(frame))
			{
				scene.UserModel.ControllerControl.SetKeyVariable(_KeyVariablesByFrames[frame]);
			}
			if (_EndFrame == frame)
			{
				RunMainController.IsPause(true);
				RunMainController.IsDebugPaused = RunMainController.IsPaused;
			}
		}

		public static void Record(KeyVariables keyVariables, uint frame)
		{
			if (_CurrentState == StateType.Recording)
			{
				_KeyVariablesByFrames[frame] = keyVariables;
			}
		}

		public static void Play(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				_CurrentState = StateType.Playing;
				Load(fileInfo.GetFullName());
				Manager.Load(SceneKind.Run);
				KeyboardController.SetEnabledAll(true);
				Debug.Log("Playing demo: " + path);
			}
			else
			{
				Debug.LogWarning("Failed to load demo: " + path);
			}
		}

		public static void Save(int p_endFrame, string p_file = null)
		{
			Debug.Log("Saving demo...");
			if (p_file == null)
			{
				p_file = DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss");
			}
			string filename = VectorPaths.Demo + "/" + p_file + ".dmo";
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement("Root");
			xmlDocument.AppendChild(xmlNode);
			XmlElement xmlElement = xmlDocument.CreateElement("Generator");
			xmlElement.SetAttribute("Seed", MainRandom.Current.getSeed().ToString());
			xmlElement.SetAttribute("Floor", _CurrentFloor.ToString());
			xmlNode.AppendChild(xmlElement);
			if (GeneratorHelper.IsPlayCommand)
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("Play");
				xmlElement2.SetAttribute("Rooms", GetFloorRooms());
				xmlNode.AppendChild(xmlElement2);
			}
			else if (GeneratorHelper.IsEditorCommand)
			{
				XmlElement xmlElement3 = xmlDocument.CreateElement("EditorGen");
				foreach (XmlNode childNode in GeneratorHelper.EditorGenRoot.ChildNodes)
				{
					XmlNode newChild = xmlDocument.ImportNode(childNode, true);
					xmlElement3.AppendChild(newChild);
				}
				xmlNode.AppendChild(xmlElement3);
			}
			XmlElement xmlElement4 = xmlDocument.CreateElement("Keys");
			xmlNode.AppendChild(xmlElement4);
			foreach (KeyValuePair<uint, KeyVariables> keyVariablesByFrame in _KeyVariablesByFrames)
			{
				WriteInputNode(xmlElement4, keyVariablesByFrame.Key, keyVariablesByFrame.Value);
			}
			XmlElement xmlElement5 = xmlDocument.CreateElement("EndFrame");
			xmlElement5.SetAttribute("Value", p_endFrame.ToString());
			xmlNode.AppendChild(xmlElement5);
			XmlElement xmlElement6 = xmlDocument.CreateElement("User");
			DataLocal.SaveDemoBackUp(xmlElement6);
			xmlNode.AppendChild(xmlElement6);
			if (!Directory.Exists(VectorPaths.Demo))
			{
				Directory.CreateDirectory(VectorPaths.Demo);
			}
			xmlDocument.Save(filename);
		}

		private static string GetFloorRooms()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Room item in RunMainController.Location.Sets.RoomsOnTrack)
			{
				if (!item.IsIncludeInPlayCommand)
				{
					stringBuilder.Append(item.Name + " ");
				}
			}
			return stringBuilder.ToString().TrimEnd(' ');
		}

		public static void Autosave()
		{
		}

		private static void RemoveOldAutosaveDemo()
		{
			if (!Directory.Exists(VectorPaths.Demo))
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(VectorPaths.Demo);
			List<FileInfo> list = (from t in directoryInfo.GetFiles("Autosave_*.dmo", SearchOption.AllDirectories)
				orderby t.CreationTime
				select t).ToList();
			if (list.Count >= Settings.AutosaveDemoLimit)
			{
				for (int num = list.Count - Settings.AutosaveDemoLimit; num >= 0; num--)
				{
					list[0].Delete();
					list.RemoveAt(0);
				}
			}
		}

		private static void WriteInputNode(XmlNode root, uint frame, KeyVariables keyVariables)
		{
			XmlNode xmlNode = root.OwnerDocument.CreateElement("Input");
			root.AppendChild(xmlNode);
			XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("Frames");
			xmlAttribute.Value = frame.ToString();
			xmlNode.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute2 = xmlNode.OwnerDocument.CreateAttribute("Key");
			xmlAttribute2.Value = keyVariables.ToString();
			xmlNode.Attributes.Append(xmlAttribute2);
		}

		private static void Load(string path)
		{
			_KeyVariablesByFrames.Clear();
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(path, string.Empty);
			XmlNode xmlNode = xmlDocument["Root"]["Keys"];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				ReadInputNode(childNode);
			}
			XmlNode p_userNode = xmlDocument["Root"]["User"];
			DataLocal.SetupDemoBackup();
			DataLocal.ReloadFromXmlNode(p_userNode);
			_EndFrame = XmlUtils.ParseInt(xmlDocument["Root"]["EndFrame"].Attributes["Value"], -1);
			if (xmlDocument["Root"]["Play"] != null)
			{
				LoadPlay(xmlDocument);
			}
			else if (xmlDocument["Root"]["EditorGen"] != null)
			{
				LoadEditor(xmlDocument);
			}
			else
			{
				LoadFloor(xmlDocument);
			}
		}

		private static void LoadPlay(XmlDocument p_document)
		{
			XmlNode xmlNode = p_document["Root"]["Generator"];
			XmlNode xmlNode2 = p_document["Root"]["Play"];
			List<string> p_roomNames = XmlUtils.ParseString(xmlNode2.Attributes["Rooms"], string.Empty).Split(' ').ToList();
			int p_floor = XmlUtils.ParseInt(xmlNode.Attributes["Floor"]);
			uint p_seed = XmlUtils.ParseUint(xmlNode.Attributes["Seed"], 0u);
			GeneratorHelper.PreparePlayCommand(p_roomNames, p_floor, p_seed);
		}

		private static void LoadEditor(XmlDocument p_document)
		{
			XmlNode xmlNode = p_document["Root"]["Generator"];
			XmlElement p_editorGenRoot = p_document["Root"]["EditorGen"];
			int p_floor = XmlUtils.ParseInt(xmlNode.Attributes["Floor"]);
			uint p_seed = XmlUtils.ParseUint(xmlNode.Attributes["Seed"], 0u);
			GeneratorHelper.PrepareEditorCommand(p_editorGenRoot, p_floor, p_seed);
		}

		private static void LoadFloor(XmlDocument p_document)
		{
			XmlNode xmlNode = p_document["Root"]["Generator"];
			int num = XmlUtils.ParseInt(xmlNode.Attributes["Floor"]);
			uint p_seed = XmlUtils.ParseUint(xmlNode.Attributes["Seed"], 0u);
			if (num > 1)
			{
				GeneratorHelper.PrepareNextFloor(num, p_seed);
			}
			else
			{
				GeneratorHelper.PrepareFloor(num, p_seed);
			}
		}

		private static void ReadInputNode(XmlNode node)
		{
			uint key = XmlUtils.ParseUint(node.Attributes["Frames"], 0u);
			KeyVariables value = new KeyVariables(XmlUtils.ParseString(node.Attributes["Key"], string.Empty));
			_KeyVariablesByFrames[key] = value;
		}
	}
}
