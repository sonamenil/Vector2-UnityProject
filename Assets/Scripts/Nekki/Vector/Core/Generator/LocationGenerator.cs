using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator.Test;

namespace Nekki.Vector.Core.Generator
{
	public class LocationGenerator
	{
		public static int MaxAttemptCount = 5;

		private static int _GeneratedRoomId;

		private List<RoomData> _Rooms = new List<RoomData>();

		private List<RoomData> _SelectedRooms = new List<RoomData>();

		private List<string> _ObjectsFiles = new List<string>();

		public List<string> ObjectsFile
		{
			get
			{
				return _ObjectsFiles;
			}
		}

		public LocationGenerator(string p_file, NekkiRandom p_generator)
		{
			_GeneratedRoomId = 0;
			Parse(p_file);
		}

		public static string GetGeneratedRoomId()
		{
			string result = _GeneratedRoomId.ToString();
			_GeneratedRoomId++;
			return result;
		}

		private void Parse(string p_file)
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(p_file, string.Empty);
			foreach (XmlNode childNode in xmlDocument["Root"].ChildNodes)
			{
				if (!(childNode.Name != "Room"))
				{
					RoomData item = new RoomData(childNode);
					_Rooms.Add(item);
				}
			}
			ParseIncludes(xmlDocument["Root"]["Includes"]);
		}

		private void ParseIncludes(XmlNode p_node)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name != "Library")
				{
					break;
				}
				_ObjectsFiles.Add(childNode.Attributes["Filename"].Value);
			}
		}

		public Room GetRoom(bool p_testMode = false)
		{
			Room randomRoom = GetRandomRoom();
			CounterController.Current.AddCountersToSelectedGeneratorLabels(randomRoom);
			if (p_testMode)
			{
				return randomRoom;
			}
			if (randomRoom == null)
			{
				DebugUtils.Dialog("Null room(bad generator conditions?)", true);
			}
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.Rooms, randomRoom.File);
			if (xmlDocument["Root"]["Track"]["Properties"] != null)
			{
				randomRoom.CounterActions = CounterActions.Create(xmlDocument["Root"]["Track"]["Properties"]["CounterActions"], "ST_Default");
			}
			XmlNode newChild = xmlDocument["Root"]["Track"]["Content"];
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//Selection[@Choice]");
			foreach (XmlNode item in xmlNodeList)
			{
				string text = XmlUtils.ParseString(item.Attributes["Parent"], string.Empty);
				if (text.Length > 0)
				{
					text += "/";
				}
				item.Attributes["Choice"].Value = randomRoom.UniqueName + "_" + text + item.Attributes["Choice"].Value;
			}
			XmlElement xmlElement = xmlDocument.CreateElement("Object");
			xmlElement.SetAttribute("Name", randomRoom.Name);
			xmlElement.SetAttribute("X", "0");
			xmlElement.SetAttribute("Y", "0");
			xmlElement.SetAttribute("Factor", "0");
			xmlElement.AppendChild(newChild);
			XmlNode xmlNode2 = xmlDocument.SelectSingleNode("Root/Track");
			xmlNode2.InsertBefore(xmlElement, xmlNode2.FirstChild);
			randomRoom.TmpNode = xmlDocument["Root"]["Track"]["Object"];
			return randomRoom;
		}

		private RoomData GetRoom(string p_name)
		{
			foreach (RoomData room in _Rooms)
			{
				if (room.Name == p_name)
				{
					return room;
				}
			}
			return null;
		}

		private Room GetRandomRoom()
		{
			return GetRandomRoom(_Rooms);
		}

		private Room GetRandomRoom(List<RoomData> p_rooms, int p_iteration = 0)
		{
			if (p_rooms.Count == 0 || (GeneratorTester.IsActive && GeneratorTester.IsIterationExpired))
			{
				return null;
			}
			CounterController.Current.CounterGenerationAttempt = p_iteration + 1;
			if (p_iteration == MaxAttemptCount)
			{
				DebugUtils.LogToConsole("The generator room fail on " + _SelectedRooms.Count + " room");
				VectorLog.GeneratorLog("FAIL !! ROOM NUMBER " + _SelectedRooms.Count);
				return null;
			}
			RoomConditionList conditionList = ZoneResource<RoomConditions>.Current.GetConditionList();
			if (Settings.WriteGeneratorLogs)
			{
				VectorLog.GeneratorLog(" ");
				VectorLog.GeneratorLog(conditionList);
			}
			Room randomRoom = GetRandomRoom(p_rooms, conditionList, out RoomData roomdata);
			if (randomRoom == null && (int)CounterController.Current.CounterEnableRoomReuse != 0 && p_rooms != _SelectedRooms)
			{
				if (Settings.WriteGeneratorLogs)
				{
					VectorLog.GeneratorLog(" ");
					VectorLog.GeneratorLog("The generator could not find room in list!");
					VectorLog.GeneratorLog("Try to generate from selected rooms list.");
				}
				DebugUtils.LogToConsole("The generator could not find room in list! Try to generate from selected rooms list.");
				randomRoom = GetRandomRoom(_SelectedRooms, p_iteration);
			}
			if (randomRoom == null)
			{
				if (Settings.WriteGeneratorLogs)
				{
					VectorLog.GeneratorLog(" ");
					VectorLog.GeneratorLog("The generator could not find room in list!");
					VectorLog.GeneratorLog("Try to generate againe");
				}
				DebugUtils.LogToConsole($"The generator could not find {roomdata.Name} in list! Try to generate againe.");
				return GetRandomRoom(p_rooms, p_iteration + 1);
			}
			return randomRoom;
		}

		private Room GetRandomRoom(List<RoomData> p_rooms, RoomConditionList p_conditions, out RoomData roomdata)
		{
			MainRandom.ShuffleList(p_rooms);
			Room room = null;
			roomdata = null;
			for (int i = 0; i < p_rooms.Count; i++)
			{
				RoomData roomData = p_rooms[i];
				roomdata = roomData;
				if (!roomData.Check() || !p_conditions.CheckRanges(roomData.Ranges))
				{
					continue;
				}
				room = roomData.CheckConditions(p_conditions);
				if (room == null)
				{
					continue;
				}
				if (p_rooms != _SelectedRooms)
				{
					p_rooms.Remove(roomData);
					if (!roomData.IsIncludeInPlayCommand)
					{
						roomData.ResetChoiceToIndefined();
						_SelectedRooms.Add(roomData);
					}
				}
				else
				{
					roomData.ResetChoiceToIndefined();
				}
				break;
			}
			return room;
		}
	}
}
