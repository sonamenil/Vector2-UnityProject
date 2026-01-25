using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public class BaseDialog : MonoBehaviour
	{
		public enum Type
		{
			None = 0,
			EndFloorDialog = 1
		}

		private static int _OpenedDialogsCount;

		private List<Image> _AllButtons;

		public static int OpenedDialogsCount
		{
			set
			{
				_OpenedDialogsCount = value;
			}
		}

		protected List<Image> AllButtons
		{
			get
			{
				if (_AllButtons == null)
				{
					_AllButtons = new List<Image>();
					Image[] componentsInChildren = GetComponentsInChildren<Image>(true);
					Image[] array = componentsInChildren;
					foreach (Image image in array)
					{
						if (image.raycastTarget)
						{
							_AllButtons.Add(image);
						}
					}
				}
				return _AllButtons;
			}
		}

		public void Show(bool p_moveToFront)
		{
			if (p_moveToFront)
			{
				base.transform.SetAsLastSibling();
			}
			Show();
		}

		public virtual void Show()
		{
			base.gameObject.SetActive(true);
			_OpenedDialogsCount++;
			Manager.SceneKeyboardControllerEnabled = false;
		}

		public virtual void Dismiss()
		{
			Manager.SceneKeyboardControllerEnabled = true;
			base.gameObject.SetActive(false);
			_OpenedDialogsCount--;
			DialogNotificationManager.DialogsQueue.ShowNext();
			if (!DialogNotificationManager.DialogsQueue.IsProcessing && _OpenedDialogsCount == 0)
			{
				DialogCanvasController.Current.UnBlockTouches();
				DialogCanvasController.Current.TurnOffBlurEffect();
			}
		}

		protected virtual void OnDestroy()
		{
			Manager.SceneKeyboardControllerEnabled = true;
		}

		public void SetButtonsEnabled(bool p_enabled)
		{
			foreach (Image allButton in AllButtons)
			{
				allButton.raycastTarget = base.enabled;
			}
		}

		public List<Button> GetButtonsByName(string p_name)
		{
			List<Button> list = new List<Button>();
			Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (p_name == null || componentsInChildren[i].name == p_name)
				{
					list.Add(componentsInChildren[i]);
				}
			}
			return list;
		}
	}
}
