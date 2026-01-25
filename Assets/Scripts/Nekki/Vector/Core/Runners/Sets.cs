using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Grid;
using Nekki.Vector.Core.Runners.Animation;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class Sets
	{
		private List<Room> _RoomsOnTrack = new List<Room>();

		private Dictionary<string, Dictionary<string, ObjectReference>> _ObjectsNodesByFiles;

		private Dictionary<string, string> _ChoisesDictionary = new Dictionary<string, string>();

		private List<Runner> _Elements = new List<Runner>();

		private List<Data> _UserData = new List<Data>();

		private List<ObjectRunner> _Objects = new List<ObjectRunner>();

		public List<ObjectRunner> MoveToRootObjects = new List<ObjectRunner>();

		private Nekki.Vector.Core.Grid.Grid _Grid = new Nekki.Vector.Core.Grid.Grid();

		public List<ObjectRunner> RoomObjects = new List<ObjectRunner>();

		public List<VisualRunner> Visuals = new List<VisualRunner>();

		public List<PlatformRunner> Platforms = new List<PlatformRunner>();

		public List<TriggerRunner> Triggers = new List<TriggerRunner>();

		public List<AreaRunner> Areas = new List<AreaRunner>();

		public List<Nekki.Vector.Core.Runners.Animation.Animation> Animations = new List<Nekki.Vector.Core.Runners.Animation.Animation>();

		public List<ParticleRunner> Particles = new List<ParticleRunner>();

		public List<TrapezoidRunner> Trapezoids = new List<TrapezoidRunner>();

		public List<CameraRunner> Cameras = new List<CameraRunner>();

		public List<SpawnRunner> Spawns = new List<SpawnRunner>();

		public List<QuadRunner> Quads = new List<QuadRunner>();

		public List<QuadRunner> QuadsAll = new List<QuadRunner>();

		public List<SensorRunner> Sensors = new List<SensorRunner>();

		private GateRunner _LastGate;

		private GameObject _DebugLayer = new GameObject("Debug data");

		public List<Room> RoomsOnTrack
		{
			get
			{
				return _RoomsOnTrack;
			}
		}

		public List<Runner> Elements
		{
			get
			{
				return _Elements;
			}
		}

		public List<Data> UserData
		{
			get
			{
				return _UserData;
			}
		}

		public List<ObjectRunner> Objects
		{
			get
			{
				return _Objects;
			}
		}

		public Nekki.Vector.Core.Grid.Grid Grid
		{
			get
			{
				return _Grid;
			}
		}

		public GateRunner LastGate
		{
			get
			{
				return _LastGate;
			}
		}

		public GameObject DebugLayer
		{
			get
			{
				return _DebugLayer;
			}
		}

		public bool DebugLayerVisible
		{
			set
			{
				_DebugLayer.SetActive(value);
			}
		}

		public Sets()
		{
			VisualRunner.Counter = 0;
			UnityModelRunner.Counter = 0;
			_DebugLayer.SetActive(false);
		}

		public void Init()
		{
			ParseLibrary();
			AddModeles(1);
		}

		public void RemoveTemplaryData()
		{
			_ObjectsNodesByFiles.Clear();
			_ObjectsNodesByFiles = null;
		}

		private void ParseLibrary()
		{
			_ObjectsNodesByFiles = new Dictionary<string, Dictionary<string, ObjectReference>>();
			foreach (string item in RunMainController.Location.Generator.ObjectsFile)
			{
				XmlNode xmlNode = XmlUtils.OpenXMLDocument(VectorPaths.RunDataLibs, item);
				if (xmlNode == null || xmlNode["Root"] == null || xmlNode["Root"]["Objects"] == null)
				{
					Debug.Log("Empty node from file \"" + item + "\"!");
					continue;
				}
				XmlNode xmlNode2 = xmlNode["Root"]["Objects"];
				Dictionary<string, ObjectReference> dictionary = new Dictionary<string, ObjectReference>();
				foreach (XmlNode item2 in xmlNode2)
				{
					if (!(item2.Name != "Object"))
					{
						dictionary.Add(item2.Attributes["Name"].Value, new ObjectReference(item2));
					}
				}
				_ObjectsNodesByFiles.Add(item, dictionary);
			}
		}

		public void PregenerateRoom(int p_count, LocationGenerator p_generator)
		{
			List<ObjectRunner> list = new List<ObjectRunner>();
			PlaceholderManager placeholderManager = new PlaceholderManager();
			int num = CounterController.Current.CounterRoomNumberReversed;
			int num2 = CounterController.Current.CounterRoomNumber;
			DebugUtils.StartTimer("Generation");
			Room[] array = new Room[p_count];
			for (int i = 0; i < p_count; i++)
			{
				array[i] = p_generator.GetRoom(false);
				CounterController current = CounterController.Current;
				current.CounterRoomNumberReversed = (int)current.CounterRoomNumberReversed - 1;
				CounterController current2 = CounterController.Current;
				current2.CounterRoomNumber = (int)current2.CounterRoomNumber + 1;
			}
			CounterController.Current.CounterFloorGenerationTime = (int)DebugUtils.StopTimerWithMessage("Generator", "Generation");
			CounterController.Current.CounterRoomNumberReversed = num;
			CounterController.Current.CounterRoomNumber = num2;
			for (int j = 0; j < p_count; j++)
			{
				Room room = array[j];
				AddChoiceByRoom(room);
				_RoomsOnTrack.Add(room);
				ObjectRunner objectRunner = (room.Object = new ObjectRunner());
				objectRunner.Parse(room.TmpNode, _ChoisesDictionary);
				room.TmpNode = null;
				room.CollectGates();
				room.CurrentIn = room.Ins[0];
				room.CurrentOut = room.Outs[0];
				objectRunner.UpdatePosition(new Vector3f(((_LastGate == null) ? default(Vector3) : _LastGate.Position) - room.CurrentIn.Position));
				_LastGate = room.CurrentOut;
				list.Add(objectRunner);
				GetAllPlaceholders(objectRunner, placeholderManager);
				CounterController current3 = CounterController.Current;
				current3.CounterRoomNumberReversed = (int)current3.CounterRoomNumberReversed - 1;
				CounterController current4 = CounterController.Current;
				current4.CounterRoomNumber = (int)current4.CounterRoomNumber + 1;
			}
			DebugUtils.StartTimer("Postprocess");
			placeholderManager.Postprocess(_ChoisesDictionary);
			CounterController.Current.CounterFloorPostprocessTime = (int)DebugUtils.TimerElapsed("Postprocess");
			foreach (ObjectRunner item in list)
			{
				_Objects.Add(item);
				RoomObjects.Add(item);
				item.Init();
				GetElements(item);
			}
			for (int k = 0; k < MoveToRootObjects.Count; k++)
			{
				MoveToRootObjects[k].MoveToRoot();
			}
		}

		public void AddRoomByXMLNode(Room p_room)
		{
			new Exception("Not use this function!");
		}

		private void GetAllPlaceholders(ObjectRunner objectRunner, PlaceholderManager p_placeholderManager)
		{
			p_placeholderManager.AppendPlaceholders(objectRunner.Element.Placeholders);
			objectRunner.Element.Placeholders.Clear();
			foreach (ObjectRunner child in objectRunner.Childs)
			{
				GetAllPlaceholders(child, p_placeholderManager);
			}
		}

		private void AddChoiceByRoom(Room p_room)
		{
			p_room.AddChoices(_ChoisesDictionary);
		}

		public void ChangeOut(string p_name, int p_x)
		{
			Room room = GetRoom(p_x);
			Point point = room.SwitchOutAndGetDelta(p_name);
			if (point.X != 0f || point.Y != 0f)
			{
				int num = _RoomsOnTrack.LastIndexOf(room);
				for (int i = num + 1; i < RoomObjects.Count; i++)
				{
					_Grid.RemoveQuadByObject(RoomObjects[i]);
					RoomObjects[i].MoveLocalPosition(new Vector3f(point.X, point.Y, 0f));
					_Grid.AddQuadByObject(RoomObjects[i]);
				}
			}
		}

		private void AddModeles(int p_count)
		{
			for (int i = 0; i < p_count; i++)
			{
				Data data = new Data();
				data.Name = ((i != 0) ? "Hunter" : "Player");
				data.BirthSpawn = "DefaultSpawn";
				data.Skins = ((i != 0) ? new List<string>("hunter".Split('|')) : new List<string>());
				data.IsPlayer = i == 0;
				data.AI = i;
				if (data.Skins.Count == 0)
				{
					data.Skins.Add("1");
				}
				data.Init();
				_UserData.Add(data);
			}
		}

		public static ObjectReference ObjectNode(string p_name, string p_fileName)
		{
			if (RunMainController.Location.Sets != null && RunMainController.Location.Sets._ObjectsNodesByFiles != null && RunMainController.Location.Sets._ObjectsNodesByFiles.ContainsKey(p_fileName) && RunMainController.Location.Sets._ObjectsNodesByFiles[p_fileName].ContainsKey(p_name))
			{
				return RunMainController.Location.Sets._ObjectsNodesByFiles[p_fileName][p_name];
			}
			DebugUtils.Dialog("Error: Not found object Name:" + p_name + " in File:" + p_fileName, true);
			return null;
		}

		public void InitRooms()
		{
			for (int i = 2; i < _RoomsOnTrack.Count; i++)
			{
				_RoomsOnTrack[i].Object.IsEnableUnityGO = false;
			}
		}

		private void GetElements(ObjectRunner p_objects)
		{
			_Elements.AddRange(p_objects.Element.Elements);
			Add(p_objects.Element);
			foreach (ObjectRunner child in p_objects.Childs)
			{
				GetElements(child);
			}
		}

		private void Add(Element p_element)
		{
			Visuals.AddRange(p_element.Visuals);
			Platforms.AddRange(p_element.Platforms);
			Triggers.AddRange(p_element.Triggers);
			Areas.AddRange(p_element.Areas);
			Animations.AddRange(p_element.Animations);
			Particles.AddRange(p_element.Particles);
			Trapezoids.AddRange(p_element.Trapezoids);
			Spawns.AddRange(p_element.Spawns);
			Cameras.AddRange(p_element.Cameras);
			Sensors.AddRange(p_element.Sensors);
			QuadsAll.AddRange(p_element.QuadsAll);
			foreach (PlatformRunner platform in p_element.Platforms)
			{
				Quads.Add(platform);
			}
			foreach (TrapezoidRunner trapezoid in p_element.Trapezoids)
			{
				Quads.Add(trapezoid);
			}
		}

		public string GetRoomNameByX(int p_x)
		{
			Room room = GetRoom(p_x);
			if (room != null)
			{
				return room.Name;
			}
			return "Player is outside all rooms (p_x = " + p_x + ")";
		}

		public string GetRoomUniqueNameByX(int p_x)
		{
			Room room = GetRoom(p_x);
			if (room != null)
			{
				return room.UniqueName;
			}
			return "Player is outside all rooms (p_x = " + p_x + ")";
		}

		public Room GetRoom(int p_x)
		{
			int roomIndex = GetRoomIndex(p_x);
			if (roomIndex == -1)
			{
				return null;
			}
			return _RoomsOnTrack[roomIndex];
		}

		public Room GetNextRoom(int p_x)
		{
			int roomIndex = GetRoomIndex(p_x);
			if (roomIndex == -1 || roomIndex + 1 >= _RoomsOnTrack.Count)
			{
				return null;
			}
			return _RoomsOnTrack[roomIndex + 1];
		}

		public int GetRoomIndex(int p_x)
		{
			Room room = null;
			int i = 0;
			for (int count = _RoomsOnTrack.Count; i < count; i++)
			{
				room = _RoomsOnTrack[i];
				if (room.CurrentIn.Position.x < (float)p_x && room.CurrentOut.Position.x > (float)p_x)
				{
					return i;
				}
			}
			return -1;
		}

		public string GetChoisesDebugInfo(string roomName)
		{
			if (string.IsNullOrEmpty(roomName))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in _ChoisesDictionary)
			{
				if (item.Key.Contains(roomName))
				{
					string arg = item.Key.Replace(roomName, string.Empty).Replace("_", string.Empty);
					stringBuilder.AppendFormat("<color=red>{0}:</color> {1}\n", arg, item.Value);
				}
			}
			return stringBuilder.ToString().Trim('\n');
		}

		public void End()
		{
			for (int i = 0; i < _Elements.Count; i++)
			{
				_Elements[i].End();
			}
		}
	}
}
