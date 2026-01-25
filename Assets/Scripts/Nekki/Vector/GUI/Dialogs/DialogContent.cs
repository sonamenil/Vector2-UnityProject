using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class DialogContent : MonoBehaviour
	{
		private RectTransform _RectTransform;

		protected List<DialogButtonData> _Buttons;

		private BaseUIDialog _Parent;

		private RectTransform GetRectTransform
		{
			get
			{
				if (_RectTransform == null)
				{
					_RectTransform = GetComponent<RectTransform>();
				}
				return _RectTransform;
			}
		}

		public float Height
		{
			get
			{
				return GetRectTransform.sizeDelta.y;
			}
		}

		public float DeltaY
		{
			set
			{
				GetRectTransform.localPosition = new Vector3(0f, value);
			}
		}

		public BaseUIDialog Parent
		{
			get
			{
				return _Parent;
			}
			set
			{
				_Parent = value;
			}
		}

		protected void Init(List<DialogButtonData> p_buttons)
		{
			_Buttons = p_buttons;
			_Parent.InitButtons(_Buttons);
		}

		public virtual void OnButton1Tap()
		{
			_Buttons[0].Activate(_Parent);
		}

		public virtual void OnButton2Tap()
		{
			_Buttons[1].Activate(_Parent);
		}

		public virtual void OnButton3Tap()
		{
			_Buttons[2].Activate(_Parent);
		}

		public virtual void OnButton4Tap()
		{
			_Buttons[3].Activate(_Parent);
		}
	}
}
