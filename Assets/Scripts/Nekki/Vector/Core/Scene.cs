using System.Collections.Generic;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.PassiveEffects;

namespace Nekki.Vector.Core
{
	public class Scene
	{
		protected bool _IsReloadProcess;

		protected Location _Location;

		private ControllerPassiveEffects _ControllerPassiveEffects;

		private static uint _FrameCount;

		private float _SlowMode = 1f;

		public bool _IgnoreKeys;

		public Location Location
		{
			get
			{
				return _Location;
			}
		}

		public static uint FrameCount
		{
			get
			{
				return _FrameCount;
			}
		}

		public float SlowModeValue
		{
			get
			{
				return _SlowMode;
			}
		}

		public bool SlowMode
		{
			get
			{
				return _SlowMode == 10f;
			}
			set
			{
				_SlowMode = ((!value) ? 1f : 10f);
			}
		}

		public ModelHuman BotModel
		{
			get
			{
				foreach (ModelHuman model in _Location.Models)
				{
					if (model.IsBot)
					{
						return model;
					}
				}
				return null;
			}
		}

		public ModelHuman UserModel
		{
			get
			{
				foreach (ModelHuman model in _Location.Models)
				{
					if (!model.IsBot)
					{
						return model;
					}
				}
				return null;
			}
		}

		public List<ModelHuman> Models
		{
			get
			{
				return _Location.Models;
			}
		}

		public ModelHuman Player
		{
			get
			{
				return _Location.Player;
			}
		}

		public void Init()
		{
			CreateCamera();
			_Location = new Location(RunMainController.RoomProperties);
			_Location.Init();
			if (!Demo.IsPlaying)
			{
				Demo.StartRecording();
			}
			RunMainController.CurrentFloor = CounterController.Current.CounterFloor;
			_Location.Start();
			ResourcesMap.ResetSpriteAtlasCache();
			_FrameCount = 0u;
			_ControllerPassiveEffects = new ControllerPassiveEffects();
		}

		public void CreateCamera()
		{
			Nekki.Vector.Core.Camera.Camera.Create();
			Nekki.Vector.Core.Camera.Camera.Current.Zooming(Nekki.Vector.Core.Camera.Camera.CurrentZoom, true);
		}

		public void Render()
		{
			_Location.Render();
			Nekki.Vector.Core.Camera.Camera.Current.Render();
			_FrameCount++;
			_ControllerPassiveEffects.Render();
			if (Demo.IsPlaying)
			{
				Demo.Simulate(this, _FrameCount);
			}
		}

		public void KeysVariables(KeyVariables KeyVariables)
		{
			if (!Demo.IsPlaying && UserModel.ControllerControl.SetKeyVariable(KeyVariables))
			{
				Demo.Record(KeyVariables, _FrameCount);
			}
		}

		public void End()
		{
			_ControllerPassiveEffects.End();
			_Location.End();
		}
	}
}
