using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Transformations;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class Swarm : ObjectRunner
	{
		private enum State
		{
			Active = 0,
			Disable = 1
		}

		private enum MotionState
		{
			Start = 0,
			Idle = 1,
			Stop = 2
		}

		private class MotionData
		{
			public MotionState State;

			public float AccelerationS;

			public float AccelerationE;

			private int _AccelerationFramesS;

			private int _AccelerationFramesE;

			private float _IdleSpeed;

			public float CurrentSpeed;

			public float IdleSpeed
			{
				get
				{
					return _IdleSpeed;
				}
				set
				{
					_IdleSpeed = value;
					RecalcAccelerations();
				}
			}

			public MotionData(float p_idleSpeed, int p_accelerationS, int p_accelerationE)
			{
				_IdleSpeed = p_idleSpeed;
				_AccelerationFramesS = p_accelerationS;
				_AccelerationFramesE = p_accelerationE;
				CurrentSpeed = 0f;
				State = MotionState.Start;
				RecalcAccelerations();
			}

			private void RecalcAccelerations()
			{
				AccelerationS = _IdleSpeed / (float)_AccelerationFramesS;
				AccelerationE = _IdleSpeed / (float)_AccelerationFramesE;
			}

			public float DistanceStop()
			{
				int num = (int)Math.Round(CurrentSpeed / AccelerationE, MidpointRounding.AwayFromZero);
				float num2 = 0f;
				for (int i = 0; i <= num; i++)
				{
					num2 += (float)i * AccelerationE;
				}
				return num2;
			}

			public void CallcCurrentSpeed()
			{
				switch (State)
				{
				case MotionState.Idle:
					break;
				case MotionState.Start:
					CurrentSpeed += AccelerationS;
					if (CurrentSpeed > _IdleSpeed)
					{
						CurrentSpeed = _IdleSpeed;
						State = MotionState.Idle;
					}
					break;
				case MotionState.Stop:
					CurrentSpeed -= AccelerationE;
					if (CurrentSpeed < 0f)
					{
						CurrentSpeed = 0f;
						State = MotionState.Idle;
					}
					break;
				}
			}

			public void SetParams(WaypointRunner p_waypoint)
			{
				if (!float.IsNaN(p_waypoint.Speed))
				{
					_IdleSpeed = p_waypoint.Speed;
				}
				if (p_waypoint.AccelerationFramesS != -1)
				{
					_AccelerationFramesS = p_waypoint.AccelerationFramesS;
				}
				if (p_waypoint.AccelerationFramesE != -1)
				{
					_AccelerationFramesE = p_waypoint.AccelerationFramesE;
				}
			}
		}

		private const float _ParticleSize = 50f;

		private State _State;

		private List<QuadRunner> _Quads = new List<QuadRunner>();

		private Matrix33 _RotationMatrix = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		private float _Rotation;

		private Matrix33 _ScaleMatrix = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		private Vector3f _Scale = new Vector3f(1f, 1f, 0f);

		private Matrix33 _TranslateMatrix = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		private Vector3f _Translation = new Vector3f(0f, 0f, 0f);

		private Matrix33 _CurrMatrix = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		private Matrix33 _DestMatrix = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		private List<TransformInterface> _Particles;

		private List<Vector3f> _ParticleCoords;

		private List<Runner> _Elements;

		private int _Sign = 1;

		private ControllerSwarm _Controller;

		private MotionData _MotionData;

		private WaypointRunner _Destination;

		private int _Delay;

		private int _SpeedDelta;

		private string _WayPointKey = string.Empty;

		private string _NextWayPointKey = string.Empty;

		private bool _HasDepartured;

		public bool IsActive
		{
			get
			{
				return _State == State.Active;
			}
			set
			{
				_State = ((!value) ? State.Disable : State.Active);
				if (value)
				{
					_Controller.TurnOn();
				}
			}
		}

		public List<QuadRunner> Quads
		{
			get
			{
				return _Quads;
			}
		}

		public float Rotation
		{
			get
			{
				return _Rotation;
			}
			set
			{
				_Rotation = value - 360f * Mathf.Floor(value / 360f);
				RefreshSRT();
			}
		}

		public Vector3f Scale
		{
			get
			{
				return _Scale;
			}
			set
			{
				_Scale = value;
				RefreshSRT();
			}
		}

		public Vector3f Translation
		{
			get
			{
				return _Translation;
			}
			set
			{
				_Translation = value;
				RefreshSRT();
			}
		}

		public Matrix33 CurrMatrix
		{
			get
			{
				return _CurrMatrix;
			}
			set
			{
				_CurrMatrix = value;
			}
		}

		public Matrix33 DestMatrix
		{
			get
			{
				return _DestMatrix;
			}
			set
			{
				_DestMatrix = value;
			}
		}

		public List<TransformInterface> Particles
		{
			get
			{
				return _Particles;
			}
		}

		public List<Vector3f> ParticleCoords
		{
			get
			{
				return _ParticleCoords;
			}
			set
			{
				_ParticleCoords = value;
			}
		}

		public Vector3f DefaultMoveCoords { get; set; }

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

		public Rect BoundingBox
		{
			get
			{
				if (_Particles == null)
				{
					return new Rect(base.Position.x, base.Position.y, 0f, 0f);
				}
				Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
				int i = 0;
				for (int count = _Particles.Count; i < count; i++)
				{
					if (_Particles[i].Position.x < vector.x)
					{
						vector.x = _Particles[i].Position.x;
					}
					else if (_Particles[i].Position.x > vector2.x)
					{
						vector2.x = _Particles[i].Position.x;
					}
					if (_Particles[i].Position.y < vector.y)
					{
						vector.y = _Particles[i].Position.y;
					}
					else if (_Particles[i].Position.y > vector2.y)
					{
						vector2.y = _Particles[i].Position.y;
					}
				}
				float num = 25f;
				vector.x -= num;
				vector.y -= num;
				vector2.x += num;
				vector2.y += num;
				return new Rect(vector, vector2 - vector);
			}
		}

		public bool HasDepartured
		{
			get
			{
				return _HasDepartured;
			}
			set
			{
				_HasDepartured = value;
			}
		}

		public Swarm(ControllerSwarm p_controller)
		{
			_State = State.Disable;
			_Controller = p_controller;
			DefaultMoveCoords = new Vector3f(0f, 0f, 0f);
		}

		public override void Parse(XmlNode p_node, Dictionary<string, string> p_choices)
		{
			base.Parse(p_node, p_choices);
			ParseMotion(p_node);
		}

		public void InitializeParticles(string p_staticObject = "SwarmBody")
		{
			if (_Particles != null)
			{
				return;
			}
			_Particles = new List<TransformInterface>();
			_ParticleCoords = new List<Vector3f>();
			int i = 0;
			for (int count = base.Childs.Count; i < count; i++)
			{
				if (base.Childs[i].Name != p_staticObject)
				{
					_Particles.Add(base.Childs[i]);
					_ParticleCoords.Add(base.Childs[i].LocalPosition);
				}
			}
			i = 0;
			for (int count = base.Element.Elements.Count; i < count; i++)
			{
				_Particles.Add(base.Element.Elements[i]);
				_ParticleCoords.Add(base.Element.Elements[i].LocalPosition);
			}
		}

		private void ParseMotion(XmlNode p_node)
		{
			if (p_node["Properties"] != null && p_node["Properties"]["Static"] != null && p_node["Properties"]["Static"]["Motion"] != null)
			{
				XmlNode xmlNode = p_node["Properties"]["Static"]["Motion"];
				float p_idleSpeed = XmlUtils.ParseFloat(xmlNode.Attributes["Speed"]);
				int p_accelerationS = XmlUtils.ParseInt(xmlNode.Attributes["StartAccFrames"]);
				int p_accelerationE = XmlUtils.ParseInt(xmlNode.Attributes["StopAccFrames"]);
				_MotionData = new MotionData(p_idleSpeed, p_accelerationS, p_accelerationE);
			}
		}

		public override void Init()
		{
			base.Init();
			ObjectRunner.CollectQuads(this, _Quads);
			_Elements = new List<Runner>();
			CollectElements(this);
		}

		public void SetDefPos(float p_x, float p_y)
		{
			InitializeParticles("SwarmBody");
			Matrix33 inverseMatrix = _CurrMatrix.GetInverseMatrix();
			Vector3f vector3f = new Vector3f(0f, 0f, 0f);
			int i = 0;
			for (int count = _Particles.Count; i < count; i++)
			{
				_Particles[i].SetPosition(p_x, p_y);
				vector3f.Set(p_x, p_y, 0f);
				vector3f *= inverseMatrix;
				_ParticleCoords[i].Set(vector3f.X, vector3f.Y, 1f);
			}
		}

		public void RunTo(WaypointRunner p_waypoint, int p_delay, int p_speedDelta)
		{
			if (_Destination != null)
			{
				_MotionData.SetParams(_Destination);
			}
			_NextWayPointKey = p_waypoint.WayPointKey;
			_MotionData.IdleSpeed -= _SpeedDelta;
			_Delay = p_delay;
			_SpeedDelta = p_speedDelta;
			HasDepartured = false;
			_MotionData.IdleSpeed += _SpeedDelta;
			_Destination = p_waypoint;
			_MotionData.State = MotionState.Start;
		}

		public void Stop()
		{
			_Destination = null;
			_State = State.Disable;
		}

		public void Render()
		{
			if (_Destination == null)
			{
				return;
			}
			if (_Delay > 0)
			{
				_Delay--;
				return;
			}
			if (!_HasDepartured)
			{
				TRE_SwarmDeparture.ActivateThisEvent(_WayPointKey);
				_HasDepartured = true;
				_WayPointKey = _NextWayPointKey;
			}
			CalcSpeed();
		}

		private void CalcSpeed()
		{
			_MotionData.CallcCurrentSpeed();
			MotionState state = _MotionData.State;
			float distanceToDestination = GetDistanceToDestination();
			if (distanceToDestination <= _MotionData.DistanceStop())
			{
				_MotionData.State = MotionState.Stop;
				if (state != MotionState.Stop)
				{
					TRE_SwarmDec.ActivateThisEvent(_NextWayPointKey);
				}
			}
			float currentSpeed = _MotionData.CurrentSpeed;
			if (currentSpeed == 0f || distanceToDestination == 0f)
			{
				_Destination.SwarmArrive(this);
				return;
			}
			float num = (_Destination.Position.x - base.Position.x) / distanceToDestination;
			float num2 = (_Destination.Position.y - base.Position.y) / distanceToDestination;
			float p_x = currentSpeed * num;
			float p_y = currentSpeed * num2;
			MoveLocalPosition(new Vector3f(p_x, p_y, 0f));
		}

		private float GetDistanceToDestination()
		{
			return Mathf.Sqrt(Mathf.Pow(base.Position.x - _Destination.Position.x, 2f) + Mathf.Pow(base.Position.y - _Destination.Position.y, 2f));
		}

		private void RefreshSRT()
		{
			RefreshRotate();
			RefreshScale();
			RefreshTranslate();
			_CurrMatrix = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
			_CurrMatrix = _RotationMatrix * _ScaleMatrix * _TranslateMatrix;
			int i = 0;
			for (int count = Particles.Count; i < count; i++)
			{
				_ParticleCoords[i].Z = 1f;
				Vector3f p_delta = _ParticleCoords[i] * _CurrMatrix - (Vector3f)_Particles[i].LocalPosition;
				Particles[i].TransformMove(p_delta);
				Particles[i].SetDeltaMove();
			}
		}

		private void RefreshScale()
		{
			_ScaleMatrix.Set(_Scale.X, 0f, 0f, 0f, _Scale.Y, 0f, 0f, 0f, 1f);
		}

		private void RefreshRotate()
		{
			float f = _Rotation / 360f * 2f * (float)Math.PI;
			_RotationMatrix.Set(Mathf.Cos(f), Mathf.Sin(f), 0f, 0f - Mathf.Sin(f), Mathf.Cos(f), 0f, 0f, 0f, 1f);
		}

		private void RefreshTranslate()
		{
			_TranslateMatrix.Set(1f, 0f, 0f, 0f, 1f, 0f, _Translation.X, _Translation.Y, 1f);
		}

		private void CollectElements(ObjectRunner p_object)
		{
			_Elements.AddRange(p_object.Element.Elements);
			foreach (ObjectRunner child in p_object.Childs)
			{
				CollectElements(child);
			}
		}

		protected override void OnDisabled()
		{
			base.OnDisabled();
			InitializeParticles("SwarmBody");
			int i = 0;
			for (int count = _Particles.Count; i < count; i++)
			{
				_Particles[i].UnityObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			_Sign = 1;
			Stop();
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
