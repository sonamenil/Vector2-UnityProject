using System.Collections.Generic;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Transformations;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Scenes.Run;

namespace Nekki.Vector.Core
{
	public class Location
	{
		private List<ModelHuman> _Models = new List<ModelHuman>();

		private Sets _Sets;

		private LocationGenerator _Generator;

		private ControllerSpawns _ControllerSpawns;

		private ControllerSaveMe _ControllerSaveMe;

		private ControllerSwarm _ControllerSwarm;

		private ControllerRooms _RoomsController;

		private ControllerMissions _ControllerMissions;

		private GlobalTimer _GlobalTimer;

		private List<QuadRunner> _RenderQuad = new List<QuadRunner>();

		private List<TriggerRunner> _TriggersInViewport = new List<TriggerRunner>();

		public List<ModelHuman> Models
		{
			get
			{
				return _Models;
			}
			set
			{
				_Models = value;
			}
		}

		public List<Data> UserData
		{
			get
			{
				return _Sets.UserData;
			}
		}

		public Sets Sets
		{
			get
			{
				return _Sets;
			}
		}

		public List<SpawnRunner> Spawns
		{
			get
			{
				return _Sets.Spawns;
			}
		}

		public List<CameraRunner> Cameras
		{
			get
			{
				return _Sets.Cameras;
			}
		}

		public ModelHuman Player
		{
			get
			{
				for (int i = 0; i < _Models.Count; i++)
				{
					if (_Models[i].IsPlayer)
					{
						return _Models[i];
					}
				}
				return null;
			}
		}

		public LocationGenerator Generator
		{
			get
			{
				return _Generator;
			}
		}

		public ControllerSpawns ControllerSpawns
		{
			get
			{
				return _ControllerSpawns;
			}
		}

		public ControllerSaveMe ControllerSaveMe
		{
			get
			{
				return _ControllerSaveMe;
			}
		}

		public ControllerSwarm ControllerSwarm
		{
			get
			{
				return _ControllerSwarm;
			}
		}

		public Room CurrentRoom
		{
			get
			{
				int p_x = (int)Player.ModelObject.GetNode("COM").Start.X;
				return _Sets.GetRoom(p_x);
			}
		}

		public string CurrentRoomName
		{
			get
			{
				int p_x = (int)Player.ModelObject.GetNode("COM").Start.X;
				return _Sets.GetRoomNameByX(p_x);
			}
		}

		public string CurrentRoomUniqueName
		{
			get
			{
				int p_x = (int)Player.ModelObject.GetNode("COM").Start.X;
				return _Sets.GetRoomUniqueNameByX(p_x);
			}
		}

		public int CurrentRoomIndex
		{
			get
			{
				int p_x = (int)Player.ModelObject.GetNode("COM").Start.X;
				return _Sets.GetRoomIndex(p_x);
			}
		}

		public GlobalTimer Timer
		{
			get
			{
				return _GlobalTimer;
			}
		}

		public List<TriggerRunner> TriggersInViewport
		{
			get
			{
				return _TriggersInViewport;
			}
		}

		public Location(string p_roomPropertyFile)
		{
			if ((int)CounterController.Current.CounterFloor == 0)
			{
				DebugUtils.Log("FLOOR 0!!!!");
			}
			DataLocal.Current.StateRun = 1;
			SetCounters();
			if ((int)CounterController.Current.CounterPlayCommand != 0)
			{
				CounterController.Current.CounterRoomNumberReversed = CounterController.Current.CounterPlayCommand;
			}
			_Generator = new LocationGenerator(p_roomPropertyFile, MainRandom.Current);
			_Sets = new Sets();
			if ((int)CounterController.Current.CounterTimerEnabled == 1)
			{
				_GlobalTimer = new GlobalTimer();
			}
			_ControllerSpawns = new ControllerSpawns();
			_ControllerSaveMe = new ControllerSaveMe();
			_RoomsController = new ControllerRooms(this);
			_ControllerMissions = new ControllerMissions();
		}

		private void SetCounters()
		{
			StringBuffer.Clear();
			CounterController.Current.ClearLocalRoomNamespaces();
			CounterController.Current.ClearCounterNamespace("ST_SelectedGeneratorLabels");
			CounterController.Current.CounterRoomNumber = 0;
			CounterByFloorManager.Current.SetCountersPreProcess();
			ZoneResource<MusicManager>.Current.GenerateMusic();
		}

		public void Init()
		{
			_Sets.Init();
			int p_count = CounterController.Current.CounterRoomNumberReversed;
			_Sets.PregenerateRoom(p_count, _Generator);
			_Generator = null;
			_Sets.Grid.InitGrid(_Sets.QuadsAll);
			_ControllerSpawns.InitSpawns(_Sets.Spawns);
			_RoomsController.Init();
			AnimationBinaryParser.ClearCachedBinary();
			AddModeles();
			_ControllerSwarm = new ControllerSwarm(this, 1);
		}

		protected void AddModeles()
		{
			foreach (Data userDatum in UserData)
			{
				ModelHuman modelHuman = new ModelHuman(userDatum);
				_Models.Add(modelHuman);
				modelHuman.Start(Spawn(modelHuman.BirthSpawn));
			}
		}

		public SpawnRunner Spawn(string p_name)
		{
			foreach (SpawnRunner spawn in Spawns)
			{
				if (spawn.Name == p_name)
				{
					return spawn;
				}
			}
			return null;
		}

		public void Start()
		{
			CounterController.Current.ClearCounterNamespace("ST_Run");
			List<CameraRunner> cameras = Cameras;
			Vector3f vector3f = null;
			vector3f = ((cameras.Count <= 0) ? new Vector3f(0f, 0f, 0f) : new Vector3f(cameras[0].Position));
			Nekki.Vector.Core.Camera.Camera.Current.StartPosition(vector3f);
			Nekki.Vector.Core.Camera.Camera.Current.Render();
			TransformationManager.Current.Reset();
			TRE_StartGame.ActivateThisEvent();
			_Sets.RemoveTemplaryData();
			_Sets.InitRooms();
		}

		public void Render()
		{
			RenderTriggerActions.Current.Render();
			TransformationManager.Current.Update();
			_ControllerSwarm.Render();
			RenderModels();
			RenderSensors(_Sets.Sensors);
			RenderSensors(_ControllerSwarm.Sensors);
			RenderElements();
			TransformationManager.Current.RemoveEndedTransformation();
			_RoomsController.Render();
			if (_GlobalTimer != null)
			{
				_GlobalTimer.Update();
			}
		}

		public void RenderModels()
		{
			foreach (ModelHuman model in _Models)
			{
				model.ControllerAnimation.VelocityQuads();
				model.Render(null);
				Collision(model);
			}
		}

		private void RenderSensors(List<SensorRunner> sensors)
		{
			foreach (SensorRunner sensor in sensors)
			{
				if (!sensor.IsEnabled)
				{
					continue;
				}
				foreach (TriggerRunner trigger in _Sets.Triggers)
				{
					sensor.CheckTrigger(trigger);
				}
			}
		}

		public void RenderElements()
		{
			RunnerRender.RenderRunners();
		}

		public void Collision(ModelHuman p_modelHuman)
		{
			_RenderQuad.Clear();
			List<QuadRunner> transformation = _Sets.Grid.Transformation;
			for (int i = 0; i < transformation.Count; i++)
			{
				p_modelHuman.ControllerCollision.UpdateQuad(transformation[i]);
			}
			_RenderQuad = new List<QuadRunner>();
			_Sets.Grid.Collect(p_modelHuman.Rectangle, _RenderQuad);
			_ControllerSwarm.AddQuads(_RenderQuad);
			for (int j = 0; j < _RenderQuad.Count; j++)
			{
				p_modelHuman.ControllerCollision.UpdateQuad(_RenderQuad[j]);
			}
		}

		public void AddRandomRoom()
		{
			_Sets.AddRoomByXMLNode(_Generator.GetRoom(false));
		}

		public ModelHuman GetModelByName(string p_name)
		{
			foreach (ModelHuman model in _Models)
			{
				if (model.ModelName == p_name)
				{
					return model;
				}
			}
			return null;
		}

		public ModelHuman GetUserModel()
		{
			foreach (ModelHuman model in _Models)
			{
				if (model.IsPlayer)
				{
					return model;
				}
			}
			return null;
		}

		public void End()
		{
			TransformationManager.Clear();
			RenderTriggerActions.Clear();
			TRE_StartGame.Reset();
			TRE_EndGame.Reset();
			TRE_SwarmArrival.Reset();
			TRE_SwarmDec.Reset();
			TRE_SwarmDeparture.Reset();
			TRE_OnDeath.Reset();
			TRE_GlobalTimerTimeout.Reset();
			FloatingText.FreeAllActive();
			FloatingText.ResetStaticData();
			RunnerRender.Reset();
			_Sets.End();
			_ControllerSwarm.End();
			for (int i = 0; i < _Models.Count; i++)
			{
				_Models[i].End();
			}
			_ControllerSaveMe.End();
			_ControllerMissions.End();
			AnimationTrickInfo.UnloadTricks();
		}
	}
}
