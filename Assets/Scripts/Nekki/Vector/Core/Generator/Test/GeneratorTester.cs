using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.GUI.Common;

namespace Nekki.Vector.Core.Generator.Test
{
	public static class GeneratorTester
	{
		private class RoomDataByTest
		{
			public int Count;

			public int MaxGeneratorTime = int.MinValue;

			public int MinGeneratorTime = int.MaxValue;

			private float TotalGeneratorTime;

			public int AverageGeneratorTime
			{
				get
				{
					if (Count == 0)
					{
						return 0;
					}
					return (int)(TotalGeneratorTime / (float)Count);
				}
			}

			public void AddTime(float p_time)
			{
				if ((float)MinGeneratorTime > p_time)
				{
					MinGeneratorTime = (int)p_time;
				}
				if ((float)MaxGeneratorTime < p_time)
				{
					MaxGeneratorTime = (int)p_time;
				}
				TotalGeneratorTime += p_time;
			}
		}

		private static Dictionary<int, Dictionary<string, int>> _RoomByPositionCount;

		private static Dictionary<string, int> _Sample;

		private static Dictionary<string, RoomDataByTest> _TotalRooms;

		private static LocationGenerator _Generator;

		private static double _IterationStartTime;

		private static bool _IsActive;

		public static bool IsActive
		{
			get
			{
				return _IsActive;
			}
		}

		public static bool IsIterationExpired
		{
			get
			{
				return DebugUtils.GetMS() - _IterationStartTime > 60000.0;
			}
		}

		public static IEnumerator StartTest(int p_iteration, int p_floor, int p_seed)
		{
			_IsActive = true;
			Create_RoomByPositionCountAndSample();
			XmlDocument doc = new XmlDocument();
			XmlElement rootNode = doc.CreateElement("Root");
			rootNode.SetAttribute("Floor", p_floor.ToString());
			doc.AppendChild(rootNode);
			XmlElement iterationsNode = doc.CreateElement("Iterations");
			float averageTime = 0f;
			for (int i = 0; i < p_iteration; i++)
			{
				XmlElement iteration = doc.CreateElement("Iteration");
				iteration.SetAttribute("Number", i.ToString());
				iterationsNode.AppendChild(iteration);
				int time = Generate(p_floor, iteration, p_seed);
				iteration.SetAttribute("Time", time.ToString());
				averageTime += (float)time;
				yield return null;
				ConsoleUI.Log("Iteration: " + i + ", time: " + time + ((!IsIterationExpired) ? string.Empty : " Expired"));
			}
			averageTime /= (float)p_iteration;
			rootNode.SetAttribute("AverageTime", averageTime.ToString());
			ConsoleUI.Log("Average time: " + (int)averageTime);
			XmlElement roomByPositionNode = doc.CreateElement("RoomByPositions");
			XmlElement totalRoomsNode = doc.CreateElement("Total");
			roomByPositionNode.AppendChild(totalRoomsNode);
			foreach (KeyValuePair<string, RoomDataByTest> item in _TotalRooms.OrderByDescending((KeyValuePair<string, RoomDataByTest> key) => key.Value.Count))
			{
				XmlElement roomNode = doc.CreateElement("Room");
				roomNode.SetAttribute("Name", item.Key);
				roomNode.SetAttribute("Count", item.Value.Count.ToString());
				if (item.Value.Count != 0)
				{
					roomNode.SetAttribute("AverageGenerateTime", item.Value.AverageGeneratorTime.ToString());
					roomNode.SetAttribute("MinGenerateTime", item.Value.MinGeneratorTime.ToString());
					roomNode.SetAttribute("MaxGenerateTime", item.Value.MaxGeneratorTime.ToString());
				}
				totalRoomsNode.AppendChild(roomNode);
			}
			foreach (KeyValuePair<int, Dictionary<string, int>> item2 in _RoomByPositionCount)
			{
				XmlElement position = doc.CreateElement("Position");
				position.SetAttribute("Value", item2.Key.ToString());
				roomByPositionNode.AppendChild(position);
				foreach (KeyValuePair<string, int> item3 in item2.Value.OrderByDescending((KeyValuePair<string, int> key) => key.Value))
				{
					XmlElement roomNode2 = doc.CreateElement("Room");
					roomNode2.SetAttribute("Name", item3.Key);
					roomNode2.SetAttribute("Count", item3.Value.ToString());
					position.AppendChild(roomNode2);
				}
			}
			rootNode.AppendChild(roomByPositionNode);
			rootNode.AppendChild(iterationsNode);
			if (!Directory.Exists(VectorPaths.Logs + "/generator_logs"))
			{
				Directory.CreateDirectory(VectorPaths.Logs + "/generator_logs");
			}
			doc.Save(VectorPaths.Logs + "/generator_logs/" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + "--GeneratorLog.xml");
			Clear();
			ConsoleUI.Log("Test generator complete File saved: \"logs/generator_logs/" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + "--GeneratorLog.xml");
			_IsActive = false;
			yield return null;
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

		private static int Generate(int p_floors, XmlElement p_xmlnode, int p_seed)
		{
			PreparationGenerator(p_floors, p_seed);
			int num = CounterController.Current.CounterRoomNumberReversed;
			_IterationStartTime = DebugUtils.GetMS();
			DebugUtils.StartTimer(string.Empty);
			for (int i = 0; i < num; i++)
			{
				double mS = DebugUtils.GetMS();
				Room room = _Generator.GetRoom(true);
				mS = DebugUtils.GetMS() - mS;
				if (room != null)
				{
					room.SaveToXMLTest(p_xmlnode);
					PutRoomByPlace(room, i, (float)mS);
				}
				else
				{
					XmlElement newChild = p_xmlnode.OwnerDocument.CreateElement("FAIL");
					p_xmlnode.AppendChild(newChild);
					if (IsIterationExpired)
					{
						XmlElement newChild2 = p_xmlnode.OwnerDocument.CreateElement("EXPIRED");
						p_xmlnode.AppendChild(newChild2);
						break;
					}
				}
				CounterController current = CounterController.Current;
				current.CounterRoomNumberReversed = (int)current.CounterRoomNumberReversed - 1;
				CounterController current2 = CounterController.Current;
				current2.CounterRoomNumber = (int)current2.CounterRoomNumber + 1;
			}
			return (int)DebugUtils.TimerElapsed(string.Empty);
		}

		private static void PutRoomByPlace(Room p_room, int p_place, float p_generateTime)
		{
			Dictionary<string, int> dictionary = null;
			if (_RoomByPositionCount.ContainsKey(p_place))
			{
				dictionary = _RoomByPositionCount[p_place];
			}
			else
			{
				dictionary = new Dictionary<string, int>(_Sample);
				_RoomByPositionCount.Add(p_place, dictionary);
			}
			_TotalRooms[p_room.Name].Count++;
			_TotalRooms[p_room.Name].AddTime(p_generateTime);
			Dictionary<string, int> dictionary2;
			Dictionary<string, int> dictionary3 = (dictionary2 = dictionary);
			string name;
			string key = (name = p_room.Name);
			int num = dictionary2[name];
			dictionary3[key] = num + 1;
		}

		private static void Create_RoomByPositionCountAndSample()
		{
			_RoomByPositionCount = new Dictionary<int, Dictionary<string, int>>();
			_Sample = new Dictionary<string, int>();
			_TotalRooms = new Dictionary<string, RoomDataByTest>();
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(GeneratorHelper.MainRoomPropertiesPath, string.Empty);
			foreach (XmlNode childNode in xmlDocument["Root"].ChildNodes)
			{
				if (!(childNode.Name != "Room"))
				{
					_Sample.Add(childNode.Attributes["Name"].Value, 0);
					_TotalRooms.Add(childNode.Attributes["Name"].Value, new RoomDataByTest());
				}
			}
		}

		private static void Clear()
		{
			_Generator = null;
			_RoomByPositionCount = null;
			_Sample = null;
			_TotalRooms = null;
			GC.Collect();
		}
	}
}
