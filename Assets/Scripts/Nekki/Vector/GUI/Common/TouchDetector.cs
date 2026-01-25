using Nekki.Vector.GUI.InputControllers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	[RequireComponent(typeof(Image))]
	public class TouchDetector : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
	{
		public float MinimumSlideDistance = 30f;

		public SlideEvent OnSlideEvent = new SlideEvent();

		public SlideEvent OnDragEvent = new SlideEvent();

		public TapEvent OnTapEvent = new TapEvent();

		private Image _Detector;

		private Vector2 _From;

		private Vector2 _To;

		public Image Detector
		{
			get
			{
				if (_Detector == null)
				{
					_Detector = GetComponent<Image>();
				}
				return _Detector;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return Detector.raycastTarget;
			}
			set
			{
				Detector.raycastTarget = value;
			}
		}

		private void Awake()
		{
			IsEnabled = true;
		}

		public void OnBeginDrag(PointerEventData p_eventData)
		{
			_From = p_eventData.position;
		}

		public void OnDrag(PointerEventData p_eventData)
		{
			_To = p_eventData.position;
			OnDragEvent.Invoke(0, _To, _To);
		}

		public void OnEndDrag(PointerEventData p_eventData)
		{
			_To = p_eventData.position;
			if (Vector2.Distance(_From, _To) > MinimumSlideDistance)
			{
				OnSlideEvent.Invoke(0, _From, _To);
			}
			else
			{
				OnTapEvent.Invoke(0, _To);
			}
		}
	}
}
