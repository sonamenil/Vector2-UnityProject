using Nekki.Vector.Core;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Utilites;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class SwarmPointer : MonoBehaviour
	{
		private const uint _MinDeactivationDelay = 50u;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private ResolutionImage _Bg;

		[SerializeField]
		private ResolutionImage _Icon;

		private Swarm _Swarm;

		private uint _LastActivationFrame;

		public bool IsActive
		{
			get
			{
				return _Content.gameObject.activeSelf;
			}
			set
			{
				_Content.gameObject.SetActive(value);
			}
		}

		public void Init(Swarm p_swarm)
		{
			_Swarm = p_swarm;
			IsActive = false;
			base.gameObject.name = string.Format("SwarmPointer: {0}", p_swarm.Name);
		}

		public void Refresh(Rect p_viewport)
		{
			bool flag = _Swarm.IsActive && _Swarm.IsEnabled && !p_viewport.Contains(_Swarm.Position) && !p_viewport.Overlaps(_Swarm.BoundingBox);
			if (!IsActive && flag)
			{
				_LastActivationFrame = Scene.FrameCount;
			}
			if (!flag && IsActive && Scene.FrameCount - _LastActivationFrame < 50)
			{
				return;
			}
			IsActive = flag;
			if (IsActive)
			{
				Vector2 p_result = Vector2.zero;
				if (MathUtils.LineRectIntersection(_Swarm.Position, p_viewport.center, p_viewport, ref p_result))
				{
					_Content.pivot = GetPivot(p_result, p_viewport);
					_Content.localPosition = GetPosition(p_result, p_viewport);
					SetContentFlip(p_result, p_viewport);
				}
			}
		}

		private Vector2 GetPosition(Vector2 p_pointerWorldPos, Rect p_viewPort)
		{
			Vector2 result = Scene<RunScene>.Current.Canvas.WorldToCanvasPosition(p_pointerWorldPos);
			result.x = Scene<RunScene>.Current.Canvas.WorldToCanvasPosition((!(p_pointerWorldPos.x < p_viewPort.center.x)) ? p_viewPort.max : p_viewPort.min).x - _Content.pivot.x * _Content.sizeDelta.x;
			return result;
		}

		private Vector2 GetPivot(Vector2 p_pointerWorldPos, Rect p_viewPort)
		{
			Vector2 result = new Vector2((!(p_pointerWorldPos.x < p_viewPort.center.x)) ? 1f : 0f, 1f - Mathf.Clamp(p_pointerWorldPos.y - p_viewPort.yMin, 0f, p_viewPort.height) / p_viewPort.height);
			return result;
		}

		private void SetContentFlip(Vector2 p_pointerWorldPos, Rect p_viewPort)
		{
			Vector3 localScale = new Vector3((!(p_pointerWorldPos.x < p_viewPort.center.x)) ? (-1f) : 1f, 1f, 1f);
			_Content.localScale = localScale;
			_Icon.transform.localScale = localScale;
		}
	}
}
