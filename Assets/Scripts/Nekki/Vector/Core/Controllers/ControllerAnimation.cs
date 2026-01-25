using System;
using System.Collections.Generic;
using System.Text;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Animation.Events;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Detector;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.PassiveEffects;
using Nekki.Vector.Core.Result;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerAnimation
	{
		private Model _Model;

		private ModelObject _ModelObject;

		private AnimationInfo _Animation;

		private AnimationInfo _AnimationOld;

		private KeyFrames _Frames = new KeyFrames();

		private BufferFrames _Buffer = new BufferFrames();

		private List<Key> _BlockedKeys = new List<Key>();

		private int _FirstFrame;

		private int _EndFrame;

		private int _CurrentNode;

		private int _PointFrame;

		private int _AnimationFrame;

		private int _OldFrame;

		private List<Vector3f> _VelocityOld = new List<Vector3f>();

		private List<Vector3f> _TargetPointOld = new List<Vector3f>();

		private string _PivotNode;

		private int _Sign = 1;

		private string _Name;

		private bool _IsPlay;

		private bool _IsMirror;

		private bool _Reverse;

		private bool _IsNewFrame;

		private List<string> _NodesNewPosition;

		private Vector3f _SpeedTweenOld = new Vector3f(0f, 0f, 0f);

		private QuadRunner _VelocityQuad;

		private Vector3f speed = new Vector3f(0f, 0f, 0f);

		public AnimationInfo Animation
		{
			get
			{
				return _Animation;
			}
			set
			{
				_Animation = value;
			}
		}

		public int Sign
		{
			get
			{
				return _Sign;
			}
			set
			{
				_Sign = value;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public bool IsPlay
		{
			get
			{
				return _IsPlay;
			}
		}

		public bool Reverse
		{
			get
			{
				return _Reverse;
			}
		}

		public bool IsNewFrame
		{
			get
			{
				return _IsNewFrame;
			}
			set
			{
				_IsNewFrame = value;
			}
		}

		public List<AnimationInterval> Intervals
		{
			get
			{
				return _Animation.Interval(CurrentFrame);
			}
		}

		public bool IsBuffer
		{
			get
			{
				return _Buffer != null && !_Buffer.IsBufferEmpty;
			}
		}

		public int CurrentFrame
		{
			get
			{
				if (_AnimationFrame < 3)
				{
					return _FirstFrame;
				}
				return _FirstFrame + _AnimationFrame - 2;
			}
		}

		public bool SafeInterval
		{
			get
			{
				AnimationInterval animationInterval = null;
				for (int i = 0; i < Intervals.Count; i++)
				{
					animationInterval = Intervals[i];
					if (animationInterval.IsSafe)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool LockInterval
		{
			get
			{
				AnimationInterval animationInterval = null;
				for (int i = 0; i < Intervals.Count; i++)
				{
					animationInterval = Intervals[i];
					if (animationInterval.IsLock)
					{
						return true;
					}
				}
				return false;
			}
		}

		public float AutoPositionDetectorH
		{
			get
			{
				return _Animation.AutoPositionDetectorH;
			}
		}

		public float AutoPositionDetectorV
		{
			get
			{
				return _Animation.AutoPositionDetectorV;
			}
		}

		public float LandingPositionDetectorH
		{
			get
			{
				return _Animation.LandingPositionDetectorH;
			}
		}

		public float LandingPositionDetectorV
		{
			get
			{
				return _Animation.LandingPositionDetectorV;
			}
		}

		public Rectangle BoundingBox
		{
			get
			{
				if (_Animation == null)
				{
					return _ModelObject.Rectangle;
				}
				AnimationInterval animationInterval = null;
				for (int i = 0; i < Intervals.Count; i++)
				{
					animationInterval = Intervals[i];
					if (!(animationInterval.BoundingBoxLeft == null))
					{
						return (_Sign != 1) ? animationInterval.BoundingBoxLeft : animationInterval.BoundingBoxRight;
					}
				}
				return _ModelObject.Rectangle;
			}
		}

		public bool IsActionInterval
		{
			get
			{
				AnimationInterval animationInterval = null;
				for (int i = 0; i < Intervals.Count; i++)
				{
					animationInterval = Intervals[i];
					if (animationInterval.IsAction)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsDelayReaction
		{
			get
			{
				return _Model is ModelHuman && ((ModelHuman)_Model).DelayReaction != null;
			}
		}

		public Point PlatformBound
		{
			get
			{
				AnimationInterval animationInterval = null;
				for (int i = 0; i < Intervals.Count; i++)
				{
					animationInterval = Intervals[i];
					if (animationInterval.NoPlatformBound.X != 0f || animationInterval.NoPlatformBound.Y != 0f)
					{
						return animationInterval.NoPlatformBound;
					}
				}
				return null;
			}
		}

		public ControllerAnimation(Model p_model)
		{
			_Model = p_model;
			_ModelObject = p_model.ModelObject;
		}

		public void Init(AnimationInfo p_info, bool p_reverse, int p_firstFrame, string p_pivotNode)
		{
			_Buffer.Reset();
			_Frames.Reset();
			_Animation = p_info;
			_Name = p_info.Name;
			_FirstFrame = ((p_firstFrame >= 0) ? p_firstFrame : p_info.FirstFrame);
			_Reverse = p_reverse;
			if (_Reverse)
			{
				_Sign *= -1;
			}
			SetInterruptFrames();
			_EndFrame = p_info.EndFrame;
			_Animation.CloneFrames(_FirstFrame, _EndFrame, _Frames);
			if (p_pivotNode == null)
			{
				_PivotNode = p_info.PivotNode;
			}
			else
			{
				_PivotNode = p_pivotNode;
			}
			_CurrentNode = _ModelObject.GetNodeIdByName(_PivotNode);
			ShiftPoint(_Frames);
			VelocityPoint(_Frames);
			MirrorNode("NHeel_1", "NHeel_2");
			SetNodesNewPosition();
			if (_ModelObject.DetectorVerticalLine != null)
			{
				_ModelObject.DetectorVerticalLine.Delta(p_info.DeltaDetectorV);
			}
			if (_ModelObject.DetectorHorizontalLine != null)
			{
				_ModelObject.DetectorHorizontalLine.Delta(p_info.DeltaDetectorH);
			}
			_AnimationOld = _Animation;
			_Model.IsEnabled = true;
			_IsPlay = true;
			_IsNewFrame = false;
			_AnimationFrame = 0;
			_OldFrame = 0;
		}

		public void SetInterruptFrames()
		{
			_Frames.InterruptFramesSeted();
			List<Vector3f> frame = _Frames.GetFrame(0);
			List<Vector3f> frame2 = _Frames.GetFrame(1);
			ModelNode modelNode = null;
			float num = (float)(_PointFrame + 1) * 0.5f;
			for (int i = 0; i < 46; i++)
			{
				modelNode = _ModelObject.NodesAll[i];
				float num2 = (modelNode.Start.X - modelNode.End.X) * num;
				float num3 = (modelNode.Start.Y - modelNode.End.Y) * num;
				float num4 = (modelNode.Start.Z - modelNode.End.Z) * num;
				frame[i].Set(modelNode.Start.X - num2, modelNode.Start.Y - num3, modelNode.Start.Z - num4);
				frame2[i].Set(modelNode.Start.X + num2, modelNode.Start.Y + num3, modelNode.Start.Z + num4);
			}
		}

		public void Render()
		{
			if (!_IsPlay || !_Model.IsEnabled)
			{
				return;
			}
			CheckEvent();
			if (!IsBuffer && _Frames.SizeMinusActiveFrame == 2)
			{
				_IsPlay = false;
				return;
			}
			if (!IsBuffer)
			{
				SetBufferFrame();
			}
			if (IsBuffer)
			{
				DrawFrame();
			}
		}

		public void SetNodesNewPosition()
		{
			if (_NodesNewPosition == null)
			{
				return;
			}
			ModelNode modelNode = null;
			for (int i = 0; i < _NodesNewPosition.Count; i++)
			{
				modelNode = _ModelObject.GetNode(_NodesNewPosition[i]);
				if (modelNode != null)
				{
					Vector3f p_vector = _Frames.GetFrame(2)[modelNode.Id];
					_Frames.GetFrame(0)[modelNode.Id].Set(p_vector);
					_Frames.GetFrame(1)[modelNode.Id].Set(p_vector);
				}
			}
			_NodesNewPosition = null;
		}

		public void Clear()
		{
			_AnimationOld = null;
			_Animation = null;
			_Sign = 1;
			_VelocityQuad = null;
			_SpeedTweenOld.Reset();
			Stop();
		}

		public void Play(AnimationInfo p_info, bool p_reverse, int p_firstFrame, string p_pivotNode)
		{
			if (p_info != null)
			{
				ControllerPassiveEffects.EventAnimationStart(p_info.Name);
				Init(p_info, p_reverse, p_firstFrame, p_pivotNode);
			}
		}

		public void PlayReaction(AnimationReaction p_reaction, AnimationEventType p_type, string p_data)
		{
			if (p_reaction == null)
			{
				return;
			}
			ModelHuman modelHuman = (ModelHuman)_Model;
			modelHuman.VelocityQuads.Reset();
			if (p_reaction.Name == "Death")
			{
				_Model.OnDeath();
				RunMainController.CheckLoss(ModelHuman.ModelState.DeadlyDamage, modelHuman, 0f);
				return;
			}
			if (p_reaction.IsAnimationArrest)
			{
				modelHuman.Arrest();
			}
			if (p_reaction.OnEndTrigger && modelHuman != null)
			{
				modelHuman.DelayReaction = p_reaction;
				return;
			}
			if (p_reaction.CornerClingV >= 0)
			{
				CornerPoint(p_reaction.CornerClingV, true);
			}
			if (p_reaction.CornerClingH >= 0)
			{
				CornerPoint(p_reaction.CornerClingH, false);
			}
			if (p_reaction.NodesWI != null && p_reaction.NodesWI.Count > 0)
			{
				_NodesNewPosition = p_reaction.NodesWI;
			}
			_ModelObject.DetectorHorizontalLine.Safe = p_reaction.SafeHorizontal;
			_ModelObject.DetectorVerticalLine.Safe = p_reaction.SafeVertical;
			if (modelHuman != null)
			{
				if (p_type == AnimationEventType.Controller)
				{
					modelHuman.ControllerControl.ClearKey();
				}
				modelHuman.PlayAnimation(p_reaction);
			}
		}

		public void Stop()
		{
			_IsPlay = false;
		}

		public void SetBufferFrame()
		{
			_PointFrame = (int)((float)Math.Max(_Animation.MidFrames, 1) * RunMainController.Scene.SlowModeValue);
			_Buffer.InitBuffer(_PointFrame + 1);
			List<Vector3f> activeFrame = _Frames.GetActiveFrame(0);
			List<Vector3f> activeFrame2 = _Frames.GetActiveFrame(1);
			List<Vector3f> activeFrame3 = _Frames.GetActiveFrame(2);
			Vector3f vector3f = new Vector3f(0f, 0f, 0f);
			Vector3f vector3f2 = new Vector3f(0f, 0f, 0f);
			for (int i = 0; i < activeFrame3.Count; i++)
			{
				if (activeFrame[i] != null && activeFrame2[i] != null && activeFrame3[i] != null)
				{
					Vector3f p_point = activeFrame[i];
					Vector3f vector3f3 = activeFrame2[i];
					Vector3f p_point2 = activeFrame3[i];
					Vector3f.Middle(p_point, vector3f3, vector3f);
					Vector3f.Middle(vector3f3, p_point2, vector3f2);
					for (int j = 0; j <= _PointFrame; j++)
					{
						float num = ((float)j + 1f) / ((float)_PointFrame + 1f);
						float num2 = (1f - num) * (1f - num);
						float num3 = num * num;
						float num4 = 2f * num * (1f - num);
						float p_x = vector3f.X * num2 + vector3f3.X * num4 + vector3f2.X * num3;
						float p_y = vector3f.Y * num2 + vector3f3.Y * num4 + vector3f2.Y * num3;
						float p_z = vector3f.Z * num2 + vector3f3.Z * num4 + vector3f2.Z * num3;
						_Buffer.GetFrame(j)[i].Set(p_x, p_y, p_z);
					}
				}
			}
			_Frames.NextActiveFrame();
		}

		public void DrawFrame()
		{
			List<Vector3f> activeFrame = _Buffer.GetActiveFrame(0);
			_Buffer.NextActiveFrame();
			for (int i = 0; i < activeFrame.Count; i++)
			{
				ModelNode node = _ModelObject.GetNode(i);
				if (node != null)
				{
					node.EndAssignStart();
					Vector3f vector3f = activeFrame[i];
					node.PositionStart(vector3f.X, vector3f.Y, vector3f.Z);
				}
			}
			if (_Buffer.IsBufferEmpty)
			{
				_AnimationFrame++;
				if (CurrentFrame != _OldFrame)
				{
					_OldFrame = CurrentFrame;
					_IsNewFrame = true;
				}
			}
		}

		public void PlaySounds(List<AnimationSound> p_sounds)
		{
			if (p_sounds != null)
			{
				float zoom = Nekki.Vector.Core.Camera.Camera.Current.Zoom;
				Vector3f start = Nekki.Vector.Core.Camera.Camera.Current.CameraNode.Start;
				Vector3f start2 = _Model.Node("COM").Start;
				float val = 1.5f - Math.Abs(start2.X - start.X) / 760f * zoom;
				float val2 = 1.5f - Math.Abs(start2.Y - start.Y) / 760f * zoom;
				float val3 = Math.Min(1f, Math.Min(val, val2));
				float num = Math.Max(0f, val3);
				if (_Model is ModelHuman && (_Model as ModelHuman).IsBot)
				{
					num = ((!(num < 0.5f)) ? (num / 1.1f) : (num * 1.5f));
				}
				for (int i = 0; i < p_sounds.Count; i++)
				{
					p_sounds[i].Play(num);
				}
			}
		}

		public void VelocityQuads()
		{
			if (_VelocityQuad == null)
			{
				return;
			}
			Point platformBound = PlatformBound;
			speed.Set(_VelocityQuad.SpeedRunner);
			if (platformBound == null)
			{
				_SpeedTweenOld.Set(speed);
			}
			else
			{
				if (platformBound.X > 0f)
				{
					speed.X = _SpeedTweenOld.X;
				}
				if (platformBound.Y > 0f)
				{
					speed.Y = _SpeedTweenOld.Y;
				}
			}
			if (speed.X != 0f || speed.Y != 0f)
			{
				VelocityVector(speed, _TargetPointOld);
				VelocityFrames(speed);
				VelocityBuffer(speed);
				VelocityNodes(speed);
				((ModelHuman)_Model).VelocityQuads.Set(speed);
			}
		}

		public void SetVelocityQuads(DetectorEvent p_event, bool p_isSort, QuadRunner p_platform = null)
		{
			if ((p_isSort || IsConditionlessPlatformBound(p_event)) && (p_event == null || p_event.Type == DetectorEvent.DetectorEventType.On))
			{
				QuadRunner quadRunner = ((p_platform == null) ? p_event.Platform : p_platform);
				if (quadRunner.Sticky)
				{
					_VelocityQuad = quadRunner;
				}
			}
		}

		public void VelocityVector(Vector3f p_point, List<Vector3f> p_array)
		{
			for (int i = 0; i < p_array.Count; i++)
			{
				p_array[i].Add(p_point);
			}
		}

		public void VelocityFrames(Vector3f p_point)
		{
			List<Vector3f> list = null;
			for (int i = 0; i < _Frames.Size; i++)
			{
				list = _Frames.GetFrame(i);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Add(p_point);
				}
			}
		}

		public void VelocityBuffer(Vector3f p_point)
		{
			_Buffer.VelocityBuffer(p_point);
		}

		public void VelocityNodes(Vector3f p_point)
		{
			ModelNode modelNode = null;
			for (int i = 0; i < 46; i++)
			{
				modelNode = _ModelObject.NodesAll[i];
				modelNode.Start.Add(p_point);
				modelNode.End.Add(p_point);
			}
		}

		public void VelocityPoint(KeyFrames p_frames)
		{
			if (_Animation.Binding)
			{
				int index = Math.Max(0, _AnimationFrame - 1);
				Vector3f vector3f = new Vector3f(0f, 0f, 0f);
				Vector3f vector3f2 = new Vector3f(0f, 0f, 0f);
				if (IsOldType(2))
				{
					vector3f.Add(_TargetPointOld[index]);
					ReverseVelocity();
					vector3f2.Add(_VelocityOld[index]);
				}
				else
				{
					vector3f.Add(p_frames.GetFrame(2)[_CurrentNode]);
					vector3f2.Add(_Animation.Velocity);
				}
				vector3f2.X *= _Sign;
				_VelocityOld.Clear();
				_TargetPointOld.Clear();
				List<Vector3f> list = null;
				for (int i = 2; i < p_frames.Size; i++)
				{
					list = p_frames.GetFrame(i);
					vector3f.Add(vector3f2);
					Position(vector3f, _CurrentNode, list);
					vector3f2.Add(_Animation.Gravity);
					_TargetPointOld.Add(new Vector3f(vector3f));
					_VelocityOld.Add(new Vector3f(vector3f2));
				}
			}
		}

		public bool IsOldType(int p_type)
		{
			if (_AnimationOld == null)
			{
				return false;
			}
			return _Animation.Type == p_type && _Animation.Type == _AnimationOld.Type;
		}

		public void ReverseVelocity()
		{
			for (int i = 0; i < _VelocityOld.Count; i++)
			{
				_VelocityOld[i].X *= _Sign;
			}
		}

		public void CheckEvent()
		{
			CheckController();
			if (_IsNewFrame)
			{
				VectorLog.RunLogAnimationFrame(CurrentFrame, true);
				CheckFrame();
				CheckEndFrame();
				_IsNewFrame = false;
			}
		}

		public void CheckController()
		{
			if (!_Model.IsEnabled || IsDelayReaction)
			{
				return;
			}
			ModelHuman modelHuman = _Model as ModelHuman;
			if (modelHuman == null)
			{
				return;
			}
			KeyVariables keyVariables = modelHuman.ControllerControl.KeyVariables;
			if (keyVariables == null)
			{
				return;
			}
			if (_BlockedKeys.Contains(keyVariables.Key))
			{
				modelHuman.ControllerControl.ClearKey();
				return;
			}
			AnimationChooser.Reset();
			AnimationInterval animationInterval = null;
			AnimationEventKey animationEventKey = null;
			for (int i = 0; i < Intervals.Count; i++)
			{
				animationInterval = Intervals[i];
				for (int j = 0; j < animationInterval.KeyEvents.Count; j++)
				{
					animationEventKey = animationInterval.KeyEvents[j];
					List<AnimationReaction> validateReactions = modelHuman.ControllerControl.GetValidateReactions(keyVariables, animationEventKey, _Sign);
					AnimationChooser.AddReactions(validateReactions);
				}
			}
			AnimationReaction p_reaction = Sort(null, null);
			PlayReaction(p_reaction, AnimationEventType.Controller, string.Empty);
		}

		public bool CheckCollision(Collision p_collisionResult, AnimationEventCollision.Type p_type)
		{
			AnimationChooser.Reset();
			AnimationInterval animationInterval = null;
			AnimationEventCollision animationEventCollision = null;
			for (int i = 0; i < Intervals.Count; i++)
			{
				animationInterval = Intervals[i];
				for (int j = 0; j < animationInterval.CollisionEvents.Count; j++)
				{
					animationEventCollision = animationInterval.CollisionEvents[j];
					if (animationEventCollision.Reaction != null && (animationEventCollision.Types.Count == 0 || animationEventCollision.Types.Contains(p_type)))
					{
						AnimationChooser.AddReactions(animationEventCollision.Reaction);
					}
				}
			}
			AnimationDeltaData p_delta = ((p_type != AnimationEventCollision.Type.Quad) ? null : new AnimationDeltaData(p_collisionResult.Platform, new Vector3f(p_collisionResult.Point), Sign));
			AnimationReaction animationReaction = Sort(p_delta, p_collisionResult.Platform);
			if (p_type == AnimationEventCollision.Type.Quad)
			{
				SetVelocityQuads(null, animationReaction != null, p_collisionResult.Platform);
			}
			string empty = string.Empty;
			PlayReaction(animationReaction, AnimationEventType.OnCollision, empty);
			return animationReaction != null;
		}

		public void CheckEndFrame()
		{
			if (!_Model.IsEnabled)
			{
				return;
			}
			AnimationChooser.Reset();
			AnimationInterval animationInterval = null;
			for (int i = 0; i < Intervals.Count; i++)
			{
				animationInterval = Intervals[i];
				if (CurrentFrame >= animationInterval.EndFrame)
				{
					for (int j = 0; j < animationInterval.EndEvents.Count; j++)
					{
						AnimationChooser.AddReactions(animationInterval.EndEvents[j].Reaction);
					}
				}
			}
			if (!AnimationChooser.IsEmpty())
			{
				AnimationReaction p_reaction = Sort(null, null);
				PlayReaction(p_reaction, AnimationEventType.OnEnd, string.Empty);
			}
		}

		public AnimationReaction Sort(AnimationDeltaData p_delta = null, QuadRunner p_platform = null)
		{
			if (AnimationChooser.IsEmpty() || _Model.Type != ModelType.Human)
			{
				return null;
			}
			return ((ModelHuman)_Model).SortAnimation(p_delta, p_platform);
		}

		public void CheckFrame()
		{
			if (IsDelayReaction)
			{
				return;
			}
			AnimationChooser.Reset();
			AnimationInterval animationInterval = null;
			AnimationEventFrame animationEventFrame = null;
			for (int i = 0; i < Intervals.Count; i++)
			{
				animationInterval = Intervals[i];
				for (int j = 0; j < animationInterval.FrameEvents.Count; j++)
				{
					animationEventFrame = animationInterval.FrameEvents[j];
					if (animationEventFrame.Frame == CurrentFrame)
					{
						AnimationChooser.AddReactions(animationEventFrame.Reaction);
						PlaySounds(animationEventFrame.Sound);
					}
				}
			}
			AnimationReaction p_reaction = Sort(null, null);
			PlayReaction(p_reaction, AnimationEventType.OnFrame, string.Empty);
		}

		public void Position(Vector3f p_target, int p_id, List<Vector3f> p_array)
		{
			Vector3f p_vector = p_target - p_array[p_id];
			for (int i = 0; i < p_array.Count; i++)
			{
				p_array[i].Add(p_vector);
			}
		}

		public void CheckDetectors(DetectorEvent p_event)
		{
			if (!_Model.IsEnabled || IsDelayReaction)
			{
				return;
			}
			AnimationChooser.Reset();
			AnimationInterval animationInterval = null;
			AnimationEventDetector animationEventDetector = null;
			AnimationReaction animationReaction = null;
			for (int i = 0; i < Intervals.Count; i++)
			{
				animationInterval = Intervals[i];
				List<AnimationEventDetector> list = ((!p_event.IsVertical) ? animationInterval.DetectorHEvents : animationInterval.DetectorVEvents);
				for (int j = 0; j < list.Count; j++)
				{
					animationEventDetector = list[j];
					if (animationEventDetector.Type != 0 && animationEventDetector.Type != p_event.Type)
					{
						continue;
					}
					for (int k = 0; k < animationEventDetector.Reaction.Count; k++)
					{
						animationReaction = animationEventDetector.Reaction[k];
						if (animationReaction.IsSide(p_event.Side, _Sign))
						{
							AnimationChooser.AddReaction(animationReaction);
						}
					}
				}
			}
			AnimationReaction animationReaction2 = Sort(null, p_event.Platform);
			if (animationReaction2 != null)
			{
				ModelHuman modelHuman = (ModelHuman)_Model;
				Vector3f vector3f = _ModelObject.Velocity + modelHuman._VelocityQuadCurrent;
				Vector3f vector3f2 = new Vector3f(0f, 0f, 0f);
				if (LandingPositionDetectorH >= 0f && p_event.IsHorizontal)
				{
					vector3f2.X = LandingPositionDetectorH * (float)Sign - 4f * vector3f.X;
				}
				if (LandingPositionDetectorV >= 0f && p_event.IsVertical)
				{
					vector3f2.Y = LandingPositionDetectorV - 4f * vector3f.Y;
				}
				p_event.DeltaPosition(vector3f2);
			}
			SetVelocityQuads(p_event, animationReaction2 != null);
			AnimationEventType p_type;
			switch (p_event.Type)
			{
			case DetectorEvent.DetectorEventType.On:
				p_type = AnimationEventType.DetectorOn;
				break;
			case DetectorEvent.DetectorEventType.Off:
				p_type = AnimationEventType.DetectorOff;
				break;
			default:
				p_type = AnimationEventType.Controller;
				break;
			}
			string empty = string.Empty;
			PlayReaction(animationReaction2, p_type, empty);
		}

		public void CheckArea()
		{
			if (!_Model.IsEnabled)
			{
				return;
			}
			AnimationChooser.Reset();
			AnimationInterval animationInterval = null;
			for (int i = 0; i < Intervals.Count; i++)
			{
				animationInterval = Intervals[i];
				for (int j = 0; j < animationInterval.AreaEvents.Count; j++)
				{
					AnimationChooser.AddReactions(animationInterval.AreaEvents[j].Reaction);
				}
			}
			AnimationReaction p_reaction = Sort(null, null);
			PlayReaction(p_reaction, AnimationEventType.OnArea, string.Empty);
		}

		public void CornerPoint(int p_cornernum, bool p_isDetectorV)
		{
			DetectorLine detectorLine = ((!p_isDetectorV) ? _ModelObject.DetectorHorizontalLine : _ModelObject.DetectorVerticalLine);
			QuadRunner data = detectorLine.Node.Data;
			if (data != null)
			{
				Vector3f vector3f = new Vector3f(detectorLine.Position);
				vector3f.Z = 0f;
				Vector3f vector3f2 = vector3f;
				Vector3f vector3f3 = data.Corner(_Sign, p_cornernum);
				if (vector3f3 != null)
				{
					detectorLine.Node.Start.Add(vector3f3 - vector3f2);
					detectorLine.Node.EndAssignStart();
				}
			}
		}

		public bool IsConditionlessPlatformBound(DetectorEvent p_event)
		{
			AnimationInterval animationInterval = null;
			for (int i = 0; i < Intervals.Count; i++)
			{
				animationInterval = Intervals[i];
				if ((p_event == null && animationInterval.ConditionlessBoundC) || (p_event != null && p_event.IsVertical && animationInterval.ConditionlessBoundV) || (p_event != null && p_event.IsHorizontal && animationInterval.ConditionlessBoundH))
				{
					return true;
				}
			}
			return false;
		}

		public Vector3f Shift(List<Vector3f> p_frameNodes)
		{
			return p_frameNodes[_CurrentNode] - _Frames.GetFrame(2)[_CurrentNode];
		}

		public void ShiftPoint(KeyFrames p_frames)
		{
			ReverseFrames(p_frames);
			Vector3f p_vector = Shift(_Frames.GetFrame(1));
			List<Vector3f> list = null;
			for (int i = 2; i < p_frames.Size; i++)
			{
				list = p_frames.GetFrame(i);
				ShiftNodesPoint(list, p_vector);
			}
		}

		public void ReverseFrames(KeyFrames p_frames)
		{
			if (_Sign == 1)
			{
				return;
			}
			List<Vector3f> list = null;
			for (int i = 2; i < p_frames.Size; i++)
			{
				list = p_frames.GetFrame(i);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].X *= _Sign;
				}
			}
		}

		public void ShiftNodesPoint(List<Vector3f> p_array, Vector3f p_vector)
		{
			for (int i = 0; i < p_array.Count; i++)
			{
				p_array[i].Add(p_vector);
			}
		}

		public void MirrorNode(string p_name1, string p_name2)
		{
			if (Sign == -1)
			{
				string text = p_name1;
				p_name1 = p_name2;
				p_name2 = text;
			}
			ModelNode node = _ModelObject.GetNode(p_name1);
			ModelNode node2 = _ModelObject.GetNode(p_name2);
			if (node == null || node2 == null)
			{
				return;
			}
			Vector3f start = node.Start;
			Vector3f start2 = node2.Start;
			List<Vector3f> frame = _Frames.GetFrame(2);
			Vector3f vector3f = frame[node.Id];
			Vector3f vector3f2 = frame[node2.Id];
			if (_Animation.Mirror)
			{
				_IsMirror = start.X >= start2.X != vector3f.X >= vector3f2.X;
			}
			if (!_IsMirror)
			{
				return;
			}
			_CurrentNode = _ModelObject.GetNodeIdByName(BothNodeName(_Animation.PivotNode));
			List<int[]> bothNodeList = _ModelObject.BothNodeList;
			List<Vector3f> list = null;
			for (int i = 2; i < _Frames.Size; i++)
			{
				list = _Frames.GetFrame(i);
				for (int j = 0; j < bothNodeList.Count; j++)
				{
					int[] array = bothNodeList[j];
					Vector3f value = list[array[0]];
					list[array[0]] = list[array[1]];
					list[array[1]] = value;
				}
			}
		}

		public string BothNodeName(string p_name)
		{
			StringBuilder stringBuilder = new StringBuilder(p_name);
			switch (stringBuilder[p_name.Length - 1])
			{
			case '1':
				stringBuilder[p_name.Length - 1] = '2';
				break;
			case '2':
				stringBuilder[p_name.Length - 1] = '1';
				break;
			}
			return stringBuilder.ToString();
		}

		public void BlockKey(Key p_key, bool is_block = true)
		{
			if (is_block && !_BlockedKeys.Contains(p_key))
			{
				_BlockedKeys.Add(p_key);
			}
			else if (_BlockedKeys.Contains(p_key))
			{
				_BlockedKeys.Remove(p_key);
			}
		}
	}
}
