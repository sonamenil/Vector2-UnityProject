using System;
using System.Collections.Generic;
using Nekki.Vector.Core.CameraEffects;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Scripts;
using UnityEngine;

namespace Nekki.Vector.Core.Camera
{
	public class Camera : CameraEffectHandler
	{
		private UnityEngine.Camera _UnityCamera = UnityEngine.Camera.main;

		private float _Zoom = 1f;

		private float _LastZoom;

		private Vector3 _EHPrevPosition = Vector3.zero;

		private Vector3 _EHOffset = Vector3.zero;

		private Vector3 _Position = Vector3.zero;

		private ModelNode _PositionNode = new ModelNode(new Vector3f(0f, 0f, 0f));

		private CameraNode _CameraNode;

		private bool _IsRender;

		private bool _IgnoreZoom;

		private static Camera _Current;

		private bool _IsZooming;

		private int _Frame;

		private float _Time = 30f;

		private float _LocalZoom;

		private bool _IsStopped;

		public static float MinZoom = 0.1f;

		public static float MaxZoom = 1.3f;

		public static float CurrentZoom = 0.5f;

		public static float SpeedZoom = 0.5f;

		public static float Fluency = 2f;

		public static float FluencyCurrent = 2f;

		public static float BaseMagicNumber = 200f;

		public static float HorizonNumber = 300f;

		public static float StickingSpeed = 30f;

		public static float VerticalNumber;

		private static float _HorizontalDeltaValue;

		private static int _FramesCount = 1;

		private static float _VerticalDeltaValue;

		private static int _LocalFramesCount;

		private static float DefaultFollowSpeed = 0.15f;

		private static float CurrentFollowSpeed = 0.15f;

		private static float MaxFollowSpeed = 0.5f;

		private static bool UseCustomFollowSpeed;

		private List<TriggerCameraDetectorWidescreen> _WidescreenCameraDetectors = new List<TriggerCameraDetectorWidescreen>();

		public float Zoom
		{
			get
			{
				return _Zoom;
			}
		}

		public Vector3 Position
		{
			get
			{
				return _Position;
			}
		}

		public CameraNode CameraNode
		{
			get
			{
				return _CameraNode;
			}
			set
			{
				_CameraNode = value;
			}
		}

		public bool IsRender
		{
			get
			{
				return _IsRender;
			}
		}

		public bool IgnoreZoom
		{
			get
			{
				return _IgnoreZoom;
			}
			set
			{
				_IgnoreZoom = value;
			}
		}

		public bool IgnoreEventHorizon { get; set; }

		public static Camera Current
		{
			get
			{
				return _Current;
			}
		}

		public static bool IsCurrentExists
		{
			get
			{
				return _Current != null;
			}
		}

		public List<TriggerCameraDetectorWidescreen> WidescreenCameraDetectors
		{
			get
			{
				return _WidescreenCameraDetectors;
			}
		}

		public float FrameScale
		{
			get
			{
				float num = (float)(2 * _Frame) / _Time;
				num *= num;
				float num2 = (_Zoom - _LastZoom) / 2f;
				if ((float)_Frame < _Time / 2f)
				{
					return _LastZoom + num2 * num;
				}
				return _LastZoom + num2 * ((float)(8 * _Frame) / _Time - 2f - num);
			}
		}

		public Rectangle Viewport
		{
			get
			{
				Vector3 vector = _UnityCamera.ViewportToWorldPoint(new Vector3(0f, 1f, _UnityCamera.nearClipPlane));
				Vector3 vector2 = _UnityCamera.ViewportToWorldPoint(new Vector3(1f, 0f, _UnityCamera.nearClipPlane));
				return new Rectangle(vector.x, vector.y, Mathf.Abs(vector2.x - vector.x), Mathf.Abs(vector.y - vector2.y));
			}
		}

		private int CameraSign
		{
			get
			{
				return RunMainController.Player.Sign;
			}
		}

		public event Action OnRender;

		private Camera()
		{
			IgnoreEventHorizon = false;
			_PositionNode = new ModelNode(new Vector3f(0f, 0f, 0f));
		}

		public static void Create()
		{
			_Current = new Camera();
			_Current.Init();
		}

		public static void Clear()
		{
			_Current = null;
		}

		public void Init()
		{
			VerticalNumber = 0f;
			_PositionNode.Attenuation = 0f;
			HorizonNumber = 300f;
			_FramesCount = 0;
			_Current = this;
			_IsRender = true;
			this.OnRender = delegate
			{
			};
		}

		public void StartPosition(Vector3f p_position)
		{
			_PositionNode = new ModelNode(p_position);
			_CameraNode = new CameraNode(_PositionNode);
		}

		public void ResetPosition(ModelHuman p_model)
		{
			_CameraNode = p_model.ModelObject.CameraNode;
			_PositionNode.Position(_CameraNode.Start, _CameraNode.Start);
			_IsStopped = false;
			_Position = new Vector3((int)_PositionNode.Start.X, (int)_PositionNode.Start.Y, 1f);
			_EHPrevPosition = Vector2.zero;
		}

		public void MoveToPosition(ModelHuman p_model, float p_time)
		{
			_IsStopped = true;
			UseCustomFollowSpeed = true;
			CurrentFollowSpeed = 1f - (float)Math.Pow(0.01, 1f / (p_time * 60f));
			if (CurrentFollowSpeed > MaxFollowSpeed)
			{
				CurrentFollowSpeed = MaxFollowSpeed;
			}
			Vector3f start = _CameraNode.Start;
			_CameraNode = p_model.ModelObject.CameraNode;
			_PositionNode.Position(_CameraNode.End, start);
			_IsStopped = false;
		}

		public void ResetFollowSpeedToDefault()
		{
			CurrentFollowSpeed = DefaultFollowSpeed;
			UseCustomFollowSpeed = false;
		}

		public void Render()
		{
			if (_IsRender)
			{
				UpdateScale();
				UpdatePosition();
				UpdateEventHorizon();
				UpdateEffects();
				UpdateWidescreenCameraDetectors();
				_UnityCamera.transform.position = _Position;
				this.OnRender();
			}
		}

		private void UpdatePosition()
		{
			_PositionNode.TimeStep(0f);
			Vector3 vector = _CameraNode.End;
			Vector3 vector2 = _CameraNode.Start;
			Vector3 vector3 = _PositionNode.End;
			Vector3 vector4 = _PositionNode.Start;
			vector.z = 0f;
			vector2.z = 0f;
			vector3.z = 0f;
			vector4.z = 0f;
			Vector3 vector5 = vector3 + vector2 - vector - vector4 + (vector2 - vector4) * CurrentFollowSpeed;
			if (!UseCustomFollowSpeed && vector5.magnitude > FluencyCurrent)
			{
				vector5.Normalize();
				vector5 *= FluencyCurrent;
			}
			vector4.x += vector5.x;
			vector4.y += vector5.y;
			vector4.z += vector5.z;
			if (!UseCustomFollowSpeed)
			{
				Vector3 vector6 = vector4 - vector3;
				float num = Mathf.Min(50f, (vector2 - vector4).magnitude);
				if (vector6.magnitude > num)
				{
					vector6 *= num / vector6.magnitude;
					vector4 = vector3 + vector6;
				}
			}
			_PositionNode.PositionStart(vector4);
			_Position = new Vector3((int)_PositionNode.Start.X, (int)_PositionNode.Start.Y, 1f);
		}

		private void UpdateEventHorizon()
		{
			float p_result = 0f;
			if (!GetEventHorizonOffset(Viewport, true, ref p_result))
			{
				_EHPrevPosition = _Position;
				_EHOffset = Vector3.zero;
				return;
			}
			if (_EHPrevPosition != Vector3.zero)
			{
				_Position.x = _EHPrevPosition.x;
			}
			_Position.x += (int)p_result * CameraSign;
			if (_LocalFramesCount < _FramesCount && _FramesCount != 0)
			{
				_LocalFramesCount++;
				HorizonNumber -= _HorizontalDeltaValue / (float)_FramesCount;
				VerticalNumber += _VerticalDeltaValue / (float)_FramesCount;
			}
			else
			{
				_HorizontalDeltaValue = 0f;
				_VerticalDeltaValue = 0f;
			}
			_Position.y += VerticalNumber;
			_EHOffset = _Position - _EHPrevPosition;
			_EHPrevPosition = _Position;
		}

		private void UpdateEffects()
		{
			UpdateDarkness();
		}

		private void UpdateDarkness()
		{
			DarknessEffect darknessEffect = GetDarknessEffect();
			if (!(darknessEffect == null) && darknessEffect.enabled && _CameraNode != null && _CameraNode.Node != null)
			{
				darknessEffect.Center = CalculateLightTarget(darknessEffect.Center, _UnityCamera.WorldToScreenPoint(new Vector3(_CameraNode.End.X, _CameraNode.End.Y, _CameraNode.End.Z)));
			}
		}

		private void UpdateWidescreenCameraDetectors()
		{
			if (_WidescreenCameraDetectors.Count == 0)
			{
				return;
			}
			float num = 1.7777778f / ((float)Screen.width / (float)Screen.height);
			Rectangle viewport = Viewport;
			viewport.Set(viewport.Origin.X - (num - 1f) * viewport.Size.Width, viewport.Origin.Y, viewport.Size.Width * num, viewport.Size.Height);
			float p_result = 0f;
			GetEventHorizonOffset(viewport, false, ref p_result);
			viewport.Origin.X += p_result - _EHOffset.x;
			foreach (TriggerCameraDetectorWidescreen widescreenCameraDetector in _WidescreenCameraDetectors)
			{
				widescreenCameraDetector.Update(viewport);
			}
		}

		private bool GetEventHorizonOffset(Rectangle p_viewport, bool p_limitOffset, ref float p_result)
		{
			if (IgnoreEventHorizon || UseCustomFollowSpeed)
			{
				return false;
			}
			float minX = p_viewport.MinX;
			float maxX = p_viewport.MaxX;
			float num = ((!_IsStopped) ? _CameraNode.Start.X : _PositionNode.Start.X);
			float num2 = -50f;
			if (num > maxX + num2 || num < minX - num2)
			{
				return false;
			}
			float max = maxX - minX;
			float f = _PositionNode.Start.X - _PositionNode.End.X;
			float num3 = Mathf.Abs((CameraSign != 1) ? (num - minX) : (maxX - num));
			float num4 = Mathf.Clamp(HorizonNumber / _LocalZoom, 0f, max);
			p_result = ((!(num3 <= num4)) ? 0f : Mathf.Abs(num4 - num3));
			if (p_limitOffset)
			{
				p_result = Mathf.Min(p_result, Mathf.Abs(Mathf.Abs(f) + HorizonNumber / StickingSpeed * 0.25f));
			}
			return true;
		}

		public static void SetFramesCount(int frames)
		{
			_FramesCount = frames;
			_LocalFramesCount = 0;
		}

		public static void SetHorizotalValue(float value)
		{
			_HorizontalDeltaValue = value;
		}

		public static void SetVerticalValue(float value)
		{
			_VerticalDeltaValue = value;
		}

		public void ResetOffsets()
		{
			_HorizontalDeltaValue = 0f;
			_VerticalDeltaValue = 0f;
			_FramesCount = 0;
			_LocalFramesCount = 0;
			HorizonNumber = 300f;
			VerticalNumber = 0f;
		}

		private void UpdateScale()
		{
			if (_IsZooming)
			{
				_Frame++;
				_LocalZoom = FrameScale;
				SetScaleToLayer(_LocalZoom);
				if ((float)_Frame >= _Time)
				{
					_IsZooming = false;
				}
			}
		}

		public void ResetOrthographicSize()
		{
			_UnityCamera.orthographicSize = BaseMagicNumber * 1f / _LocalZoom;
			UpdatePosition();
		}

		private void SetScaleToLayer(float p_value)
		{
			_UnityCamera.orthographicSize = BaseMagicNumber * 1f / p_value;
		}

		public void Stop()
		{
			Vector3f p_position = _CameraNode.Start + (_CameraNode.Start - _CameraNode.End) * 20f;
			_CameraNode = new CameraNode(new ModelNode(p_position));
			_IsStopped = true;
		}

		public void Zooming(float p_value = 1f, bool p_isStart = false)
		{
			if (p_value != _Zoom && !_IgnoreZoom)
			{
				if (p_value < MinZoom)
				{
					p_value = MinZoom;
				}
				if (p_value > MaxZoom)
				{
					p_value = MaxZoom;
				}
				SetLocalZoom(p_value, p_isStart);
			}
		}

		private void SetLocalZoom(float p_value, bool p_isStart)
		{
			_Frame = 0;
			_LastZoom = _Zoom;
			_Zoom = p_value;
			_LocalZoom = p_value;
			if (p_isStart)
			{
				_LastZoom = _Zoom;
				_LocalZoom = FrameScale;
				SetScaleToLayer(_LocalZoom);
			}
			else
			{
				_IsZooming = true;
			}
		}

		public void ZoomIncrease(float p_value)
		{
			Zooming(_Zoom + p_value);
		}

		public void ZoomReduce(float p_value)
		{
			Zooming(_Zoom - p_value);
		}
	}
}
