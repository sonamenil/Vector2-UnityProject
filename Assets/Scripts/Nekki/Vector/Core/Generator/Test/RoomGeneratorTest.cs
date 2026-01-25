using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Common;

namespace Nekki.Vector.Core.Generator.Test
{
	public static class RoomGeneratorTest
	{
		public static StreamWriter _FileStream;

		private static LocationGenerator _Generator;

		public static IEnumerator StartTest(string p_roomName, int p_position, int p_floor, bool p_writeCombination)
		{
			ConsoleUI.Log("Start test room: " + p_roomName);
			yield return null;
			GenerateRoomsToPosition(p_position, p_floor);
			ConsoleUI.Log("Generate rooms to position complete " + p_roomName);
			yield return null;
			RoomData room = null;
			XmlNode roomNode = null;
			XmlDocument document = XmlUtils.OpenXMLDocument(GeneratorHelper.MainRoomPropertiesPath, string.Empty);
			foreach (XmlNode node in document["Root"].ChildNodes)
			{
				if (node.Name == "Room" && node.Attributes["Name"].Value.ToLower() == p_roomName)
				{
					roomNode = node;
					break;
				}
			}
			if (roomNode == null)
			{
				ConsoleUI.Log("Room " + p_roomName + " not find");
				yield break;
			}
			RoomConditionListTest conditions = ZoneResource<RoomConditions>.Current.GetConditionListTest();
			conditions.WriteCombinations = p_writeCombination;
			StringBuilder stringBilder = new StringBuilder();
			room = new RoomData(roomNode);
			if (!room.Check() || !conditions.CheckRanges(room.Ranges, stringBilder))
			{
				ConsoleUI.Log(p_position + ". Floor: " + p_floor + " In " + p_roomName + " ranges fail");
				CreateFileStream(p_roomName, 0, 0, 0, p_floor, p_position);
				_FileStream.Write(stringBilder.ToString());
				Clear();
				yield break;
			}
			conditions.SetGeneratorLabels(room.GeneratorLabels);
			room.CheckConditions(conditions);
			ConsoleUI.Log(p_position + ". Floor: " + p_floor + " In " + p_roomName + " " + conditions.Combinations + " combinations OK: " + conditions.Ok + " (" + ((int)((float)conditions.Ok / (float)conditions.Combinations * 100f)).ToString() + "%) Fail: " + conditions.Fail + " (" + ((int)((float)conditions.Fail / (float)conditions.Combinations * 100f)).ToString() + "%)");
			CreateFileStream(p_roomName, conditions.Combinations, conditions.Fail, conditions.Ok, p_floor, p_position);
			conditions.WriteResult(_FileStream);
			Clear();
			yield return null;
		}

		private static void GenerateRoomsToPosition(int p_position, int p_floor)
		{
			PreparationGenerator(p_floor, -1);
			for (int i = 0; i < p_position; i++)
			{
				_Generator.GetRoom(true);
				CounterController current = CounterController.Current;
				current.CounterRoomNumberReversed = (int)current.CounterRoomNumberReversed - 1;
				CounterController current2 = CounterController.Current;
				current2.CounterRoomNumber = (int)current2.CounterRoomNumber + 1;
			}
		}

		private static void PreparationGenerator(int p_floor, int p_seed)
		{
			MainRandom.SetSeed(p_seed);
			CounterController.Current.CounterFloor = p_floor;
			CounterController.Current.CounterPlayCommand = 0;
			CounterController.Current.CounterRoomNumber = 0;
			CounterController.Current.ClearLocalRoomNamespaces();
			CounterController.Current.ClearCounterNamespace("ST_SelectedGeneratorLabels");
			CounterByFloorManager.Current.SetCountersPreProcess();
			_Generator = new LocationGenerator(GeneratorHelper.MainRoomPropertiesPath, MainRandom.Current);
		}

		private static void CreateFileStream(string p_roomName, int p_combination, int p_failed, int p_ok, int p_floor, int p_position)
		{
			string fileName = string.Format("{0}/result_{1}_floor_{2}_pos_{3}.xml", VectorPaths.Logs, p_roomName, p_floor, p_position);
			FileInfo fileInfo = new FileInfo(fileName);
			FileStream fileStream = fileInfo.Create();
			fileStream.Close();
			_FileStream = fileInfo.AppendText();
			if (p_combination != 0)
			{
				string text = p_ok + " (" + ((int)((float)p_ok / (float)p_combination * 100f)).ToString() + "%)";
				string text2 = p_failed + " (" + ((int)((float)p_failed / (float)p_combination * 100f)).ToString() + "%)";
				_FileStream.WriteLine(string.Format("<Room Name=\"{0}\" Combination=\"{1}\" Ok=\"{2}\" Fail=\"{3}\" >", p_roomName, p_combination, text, text2));
			}
			else
			{
				_FileStream.WriteLine(string.Format("<Room Name=\"{0}\" Fail=\"100%\" >", p_roomName));
			}
			_FileStream.Flush();
		}

		private static void Clear()
		{
			_Generator = null;
			_FileStream.WriteLine("</Room>");
			_FileStream.Close();
		}
	}
}
