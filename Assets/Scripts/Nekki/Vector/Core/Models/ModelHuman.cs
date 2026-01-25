using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Animation.Events;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Detector;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Result;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Models
{
	public class ModelHuman : Model
	{
		public enum ModelState
		{
			Common = 0,
			Death = 1,
			DeadlyDamage = 2,
			Murder = 3,
			Loss = 4,
			Win = 5
		}

		private const string StartAnimation = "JumpOff";

		public const int MainModelNodesCount = 46;

		private ControllerControl _ControllerControl;

		private ControllerModelEffect _ControllerModelEffect;

		private ControllerAnimation _ControllerAnimation;

		private ControllerTriggers _ControllerTrigger;

		private Data _UserData;

		private bool _IsDeath;

		private AnimationReaction _DelayReaction;

		private float _TimeOut;

		private float _LiveTime;

		private bool _IsKill;

		public Vector3f _VelocityQuadCurrent = new Vector3f(0f, 0f, 0f);

		private SpawnRunner _Respawn;

		public Rectangle _CollisionBox = new Rectangle(0f, 0f, 0f, 0f);

		private Rectangle _Rectangle = new Rectangle(0f, 0f, 0f, 0f);

		private ModelState _State;

		public ControllerControl ControllerControl
		{
			get
			{
				return _ControllerControl;
			}
		}

		public ControllerModelEffect ControllerModelEffect
		{
			get
			{
				return _ControllerModelEffect;
			}
		}

		public ControllerAnimation ControllerAnimation
		{
			get
			{
				return _ControllerAnimation;
			}
		}

		public ControllerTriggers ControllerTrigger
		{
			get
			{
				return _ControllerTrigger;
			}
		}

		public Data UserData
		{
			get
			{
				return _UserData;
			}
		}

		public bool IsDeath
		{
			get
			{
				return _IsDeath;
			}
			set
			{
				_IsDeath = value;
			}
		}

		public AnimationReaction DelayReaction
		{
			get
			{
				return _DelayReaction;
			}
			set
			{
				_DelayReaction = value;
			}
		}

		public Vector3f VelocityQuadCurrent
		{
			get
			{
				return _VelocityQuadCurrent;
			}
		}

		public Vector3f VelocityQuads
		{
			get
			{
				return _VelocityQuadCurrent;
			}
			set
			{
				_VelocityQuadCurrent.X = value.X;
				_VelocityQuadCurrent.Y = value.Y;
			}
		}

		public Rectangle CollisionBox
		{
			get
			{
				return _CollisionBox;
			}
		}

		public bool IsPlay
		{
			get
			{
				return _ControllerAnimation != null && _ControllerAnimation.IsPlay;
			}
		}

		public int Sign
		{
			get
			{
				return (_ControllerAnimation == null) ? 1 : ((_ControllerAnimation.Sign == 1) ? 1 : (-1));
			}
		}

		public string AnimationName
		{
			get
			{
				return _ControllerAnimation.Name;
			}
		}

		public int CurrentFrame
		{
			get
			{
				return _ControllerAnimation.CurrentFrame;
			}
		}

		public bool IsPlayer
		{
			get
			{
				return _UserData.IsPlayer;
			}
		}

		public bool SafeInterval
		{
			get
			{
				return _ControllerAnimation != null && _ControllerAnimation.SafeInterval;
			}
		}

		public bool LockInterval
		{
			get
			{
				return _ControllerAnimation != null && _ControllerAnimation.LockInterval;
			}
		}

		public string ModelName
		{
			get
			{
				return _UserData.Name;
			}
		}

		public int AI
		{
			get
			{
				return _UserData.AI;
			}
		}

		public string BirthSpawn
		{
			get
			{
				return _UserData.BirthSpawn;
			}
		}

		public bool IsBot
		{
			get
			{
				return _UserData.IsBot;
			}
		}

		public int PlatformAnticipationFrames
		{
			get
			{
				return (_ControllerAnimation != null) ? _ControllerAnimation.Animation.PlatformAnticipationFrames : 0;
			}
		}

		public override Rectangle Rectangle
		{
			get
			{
				return _Rectangle;
			}
		}

		public bool IsActionInterval
		{
			get
			{
				return _ControllerAnimation.IsActionInterval;
			}
		}

		public AnimationInfo Animation
		{
			get
			{
				return _ControllerAnimation.Animation;
			}
		}

		public ModelState State
		{
			set
			{
				_State = value;
			}
		}

		public int IntValueOfState
		{
			get
			{
				switch (_State)
				{
				case ModelState.Common:
					return "Common".GetHashCode();
				case ModelState.DeadlyDamage:
					return "DeadlyDamage".GetHashCode();
				case ModelState.Death:
					return "Death".GetHashCode();
				case ModelState.Loss:
					return "Loss".GetHashCode();
				case ModelState.Murder:
					return "Murder".GetHashCode();
				case ModelState.Win:
					return "Win".GetHashCode();
				default:
					return -1;
				}
			}
		}

		public ModelHuman(Data p_data)
			: base(p_data.Skins, ModelType.Human)
		{
			_UserData = p_data;
			_LiveTime = p_data.LiveTime;
			base.Name = p_data.Name;
			Init();
		}

		public override void Init()
		{
			base.Init();
			_ControllerAnimation = new ControllerAnimation(this);
			_ControllerControl = new ControllerControl(this);
			_ControllerTrigger = new ControllerTriggers(this);
			_ControllerModelEffect = new ControllerModelEffect(_ModelObject);
		}

		public AnimationInfo AnimationByName(string p_name)
		{
			return _UserData.Animation(p_name);
		}

		public void OnActiveArea()
		{
			if (IsPlay)
			{
				_ControllerAnimation.CheckArea();
			}
		}

		public void CheckDelayAction(QuadRunner p_trigger)
		{
			if (_DelayReaction != null && _DelayReaction.CheckNameHash(p_trigger.NameHash))
			{
				PlayAnimation(_DelayReaction);
				_DelayReaction = null;
			}
		}

		public override void OnCollisionPlatform(Collision p_collision)
		{
			if (!IsPlay)
			{
				return;
			}
			QuadRunner platform = p_collision.Platform;
			DetectorLine detectorVerticalLine = _ModelObject.DetectorVerticalLine;
			DetectorLine detectorHorizontalLine = _ModelObject.DetectorHorizontalLine;
			if ((detectorVerticalLine.Platform != platform || !detectorVerticalLine.Safe) && (detectorHorizontalLine.Platform != platform || !detectorHorizontalLine.Safe) && !SafeInterval && !_ControllerAnimation.CheckCollision(p_collision, AnimationEventCollision.Type.Quad))
			{
				OnDeath();
				if (!IsBot)
				{
					RunMainController.CheckLoss(ModelState.DeadlyDamage, this, 1f);
				}
			}
		}

		public void PlaySpawn(SpawnRunner p_spawn)
		{
			if (p_spawn != null)
			{
				_ControllerPhysics.NodeReset();
				Position(new Vector3f(p_spawn.Position));
				PlayAnimation(p_spawn.Reaction);
				if (Nekki.Vector.Core.Camera.Camera.Current != null)
				{
					Nekki.Vector.Core.Camera.Camera.Current.Zooming(p_spawn.Zoom);
				}
				base.IsEnabled = true;
			}
		}

		public void Arrest()
		{
		}

		public void Start(SpawnRunner p_spawn)
		{
			PlaySpawn(_Respawn ?? p_spawn);
		}

		public double Distance(ModelHuman p_model1, ModelHuman p_model2, string p_node1 = "COM", string p_node2 = "COM")
		{
			ModelNode modelNode = p_model1.Node(p_node1);
			ModelNode modelNode2 = p_model2.Node(p_node2);
			if (modelNode == null || modelNode2 == null)
			{
				return double.NaN;
			}
			return Vector3f.Distance(modelNode.Start, modelNode2.End);
		}

		public override void OnCollisionModel(Collision p_collision)
		{
			AnimationEventCollision.Type p_type;
			switch (p_collision.Model.Type)
			{
			case ModelType.Primitive:
				p_type = AnimationEventCollision.Type.Primitive;
				break;
			case ModelType.PrimitiveAnimated:
				p_type = AnimationEventCollision.Type.PrimitiveAnimated;
				break;
			default:
				p_type = AnimationEventCollision.Type.Quad;
				break;
			}
			_ControllerAnimation.CheckCollision(p_collision, p_type);
		}

		public void PlayAnimation(AnimationReaction p_reaction)
		{
			if (p_reaction != null)
			{
				if (p_reaction.Name == AnimationReaction.NameDeath)
				{
					OnDeath();
					RunMainController.CheckLoss(ModelState.DeadlyDamage, this, 1f);
				}
				else
				{
					PlayAnimation(p_reaction.Name, p_reaction.Reverse, p_reaction.FirstFrame, p_reaction.PivotNode);
				}
			}
		}

		public void PlayAnimation(string p_name, bool p_reverse = false, int p_firstFrame = -1, string p_pivotNode = null)
		{
			PlayAnimation(AnimationByName(p_name), p_reverse, p_firstFrame, p_pivotNode);
		}

		public void PlayAnimation(AnimationInfo p_info, bool p_reverse, int p_firstFrame, string p_pivotNode)
		{
			if (_ControllerAnimation != null && p_info != null)
			{
				_ControllerPhysics.Stop();
				_ControllerAnimation.Stop();
				_ControllerAnimation.Play(p_info, p_reverse, p_firstFrame, p_pivotNode);
			}
		}

		public void StopAnimation()
		{
			_ControllerAnimation.Stop();
		}

		public AnimationReaction SortAnimation(AnimationDeltaData p_deltaC = null, QuadRunner p_platform = null)
		{
			AnimationDeltaData p_deltaH = DeltaDetector(_ModelObject.DetectorHorizontalLine);
			AnimationDeltaData p_deltaV = DeltaDetector(_ModelObject.DetectorVerticalLine);
			Vector3f vector3f = new Vector3f(0f, 0f, 0f);
			if (p_platform != null)
			{
				vector3f.Add(p_platform.SpeedRunner);
			}
			vector3f = DeltaVelocity(vector3f);
			return AnimationChooser.Choose(p_deltaH, p_deltaV, p_deltaC, _ControllerCollision.ActiveAreas, this, vector3f);
		}

		public Vector3f DeltaVelocity(Vector3f p_value)
		{
			Vector3f vector3f = _ModelObject.Velocity + _VelocityQuadCurrent - p_value;
			vector3f.X *= Sign;
			return vector3f;
		}

		public void DetectorsVelocity()
		{
			float autoPositionDetectorH = _ControllerAnimation.AutoPositionDetectorH;
			float autoPositionDetectorV = _ControllerAnimation.AutoPositionDetectorV;
			if ((double)autoPositionDetectorH != -1.0 || (double)autoPositionDetectorV != -1.0)
			{
				Vector3f vector3f = _ModelObject.Velocity + _VelocityQuadCurrent;
				Vector3f start = _ModelObject.CenterOfMassNode.Start;
				if (autoPositionDetectorV >= 0f)
				{
					_ModelObject.DetectorVerticalNode.Start.X = start.X + 4f * vector3f.X + (float)Sign * autoPositionDetectorV;
					_ModelObject.DetectorVerticalNode.Start.Y = start.Y + 4f * vector3f.Y;
				}
				if (!(autoPositionDetectorH < 0f))
				{
					Vector3f start2 = _ModelObject.ToeRight.Start;
					Vector3f start3 = _ModelObject.ToeLeft.Start;
					_ModelObject.DetectorHorizontalNode.Start.X = ((Sign <= -1) ? (Math.Min(start2.X, start3.X) + 4f * vector3f.X) : (Math.Max(start2.X, start3.X) + 4f * vector3f.X));
				}
			}
		}

		public AnimationDeltaData DeltaDetector(DetectorLine p_detector)
		{
			if (p_detector == null || p_detector.Platform == null)
			{
				return null;
			}
			return new AnimationDeltaData(p_detector.Platform, p_detector.Position, Sign);
		}

		public override void Render(List<QuadRunner> p_platforms = null)
		{
			if (_UserData.StartTime > 0f)
			{
				float num = _UserData.StartTime * 60f * RunMainController.Scene.SlowModeValue;
				if (_TimeOut < num)
				{
					if (base.IsEnabled)
					{
						base.IsEnabled = false;
					}
					_TimeOut += 1f;
				}
				else if (_TimeOut == num)
				{
					if (!base.IsEnabled)
					{
						base.IsEnabled = true;
					}
					_TimeOut += 1f;
				}
			}
			if (!base.IsEnabled)
			{
				return;
			}
			_ControllerPhysics.Render();
			_ControllerTrigger.Render();
			_ControllerModelEffect.Render();
			if (IsPlay)
			{
				_ControllerControl.Render();
				_ControllerAnimation.Render();
				DetectorsVelocity();
			}
			_ModelObject.RenderMacroNode();
			_ModelObject.RenderDetector();
			_ModelObject.Render();
			SetCollisionBox();
			SetRectangle();
			if (_IsKill)
			{
				_LiveTime -= 1f;
				if (_LiveTime == 0f)
				{
					base.IsEnabled = false;
				}
			}
		}

		public void SetCollisionBox()
		{
			float num = -2.1474836E+09f;
			float num2 = -2.1474836E+09f;
			float num3 = 2.1474836E+09f;
			float num4 = 2.1474836E+09f;
			for (int i = 0; i < _ModelObject.CollisibleNodes.Count; i++)
			{
				Vector3f start = _ModelObject.CollisibleNodes[i].Start;
				float num7;
				if (start.X <= num)
				{
					float num6 = (start.X = num);
					num7 = num6;
				}
				else
				{
					num7 = num;
				}
				num = num7;
				float num9;
				if (start.Y <= num2)
				{
					float num6 = (start.Y = num2);
					num9 = num6;
				}
				else
				{
					num9 = num2;
				}
				num2 = num9;
				float num11;
				if (start.X >= num3)
				{
					float num6 = (start.X = num3);
					num11 = num6;
				}
				else
				{
					num11 = num3;
				}
				num3 = num11;
				float num13;
				if (start.Y >= num4)
				{
					float num6 = (start.Y = num4);
					num13 = num6;
				}
				else
				{
					num13 = num4;
				}
				num4 = num13;
			}
			_CollisionBox.Origin.X = num;
			_CollisionBox.Origin.Y = num2;
			_CollisionBox.Size.Width = num3 - num;
			_CollisionBox.Size.Height = num4 - num2;
		}

		private void SetRectangle()
		{
			_Rectangle.Set(_ControllerAnimation.BoundingBox);
			_Rectangle.Origin.X += _ModelObject.CenterOfMassNode.Start.X;
			_Rectangle.Origin.Y += _ModelObject.CenterOfMassNode.Start.Y;
		}

		public override void OnDeath()
		{
			_DeathPosition = Position("NPivot", true);
			StartPhysics();
		}

		public override void StartPhysics()
		{
			_ControllerAnimation.Clear();
			_ControllerPhysics.Start();
		}

		public new void Reset()
		{
			_ControllerPhysics.Stop();
			_ControllerAnimation.Clear();
			_ControllerCollision.Reset();
			_ControllerControl.Reset();
			_ControllerTrigger.Reset();
			_ControllerModelEffect.Reset();
			base.Reset();
			_State = ModelState.Common;
			_TimeOut = 0f;
			_LiveTime = _UserData.LiveTime;
			_DelayReaction = null;
			_IsKill = false;
			_IsDeath = false;
			base.IsEnabled = false;
			_VelocityQuadCurrent.Reset();
		}

		public void ResurrectAt(SpawnRunner p_spawn)
		{
			Reset();
			PlaySpawn(p_spawn);
			base.IsEnabled = false;
		}

		public void PlayBurnLaserReversed()
		{
			_ControllerModelEffect.RunEffect("BurnLaserReversed", null);
		}

		public override void End()
		{
			base.End();
			_ControllerModelEffect.End();
		}
	}
}
