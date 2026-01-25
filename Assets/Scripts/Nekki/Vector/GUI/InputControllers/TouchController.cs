using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI.InputControllers
{
	public class TouchController : MonoBehaviour
	{
		private const float _MinimumDistance = 30f;

		public SlideEvent OnSlide = new SlideEvent();

		public SlideEvent OnDrag = new SlideEvent();

		public TapEvent OnTap = new TapEvent();

		private Dictionary<int, Vector2> _Touches = new Dictionary<int, Vector2>();

		public static void SetEnabledAll(bool p_value)
		{
			TouchController[] array = Object.FindObjectsOfType<TouchController>();
			TouchController[] array2 = array;
			foreach (TouchController touchController in array2)
			{
				touchController.enabled = p_value;
			}
		}

		private void Update()
		{
			HandleTouchControl();
			HandleMouseControl();
		}

		private void HandleMouseControl()
		{
            Vector2 currentPos = Input.mousePosition;

            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    _Touches[i] = currentPos;
                }

                if (Input.GetMouseButton(i) && _Touches.ContainsKey(i))
                {
                    Vector2 startPos = _Touches[i];
                    float distance = Vector2.Distance(startPos, currentPos);

                    if (distance > 30f)
                    {
                        OnSlide.Invoke(i, startPos, currentPos);
                        _Touches[i] = currentPos;
                    }
                }

                if (Input.GetMouseButtonUp(i))
                {
                    _Touches.Remove(i);
                }
            }
        }

		private void HandleTouchControl()
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				int fingerId = Input.touches[i].fingerId;
				Vector2 position = Input.touches[i].position;
				switch (Input.touches[i].phase)
				{
				case TouchPhase.Began:
					if (!_Touches.ContainsKey(fingerId))
					{
						_Touches.Add(fingerId, position);
					}
					else
					{
						_Touches[fingerId] = position;
					}
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					if (_Touches.ContainsKey(fingerId))
					{
						Vector2 vector = _Touches[fingerId];
						if (Vector2.Distance(vector, position) > 30f)
						{
							OnSlide.Invoke(fingerId, vector, position);
							_Touches.Remove(fingerId);
						}
					}
					break;
				}
			}
		}

		public static Direction GetDirection(Vector2 p_from, Vector2 p_to)
		{
			float num = p_to.x - p_from.x;
			float num2 = p_to.y - p_from.y;
			if (num2 > 0f && Mathf.Abs(num) < Mathf.Abs(num2))
			{
				return Direction.Up;
			}
			if (num < 0f && Mathf.Abs(num) > Mathf.Abs(num2))
			{
				return Direction.Left;
			}
			if (num > 0f && Mathf.Abs(num) > Mathf.Abs(num2))
			{
				return Direction.Right;
			}
			if (num2 < 0f && Mathf.Abs(num) < Mathf.Abs(num2))
			{
				return Direction.Bottom;
			}
			return Direction.None;
		}
	}
}
