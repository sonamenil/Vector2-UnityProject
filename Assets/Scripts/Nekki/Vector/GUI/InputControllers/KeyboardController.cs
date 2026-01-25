using UnityEngine;

namespace Nekki.Vector.GUI.InputControllers
{
	public class KeyboardController : MonoBehaviour
	{
		private const int _KeyCodeCount = 430;

		public KeyEvent OnKeyDown = new KeyEvent();

		public KeyEvent OnKeyUp = new KeyEvent();

		public bool this[KeyCode p_code]
		{
			get
			{
				return Input.GetKey(p_code);
			}
		}

		public static void SetEnabledAll(bool p_value)
		{
			KeyboardController[] array = FindObjectsByType<KeyboardController>(FindObjectsSortMode.None);
			KeyboardController[] array2 = array;
			foreach (KeyboardController keyboardController in array2)
			{
				keyboardController.enabled = p_value;
			}
		}

		private void Update()
		{
			HandleKeyboard();
		}

		private void HandleKeyboard()
		{
			for (int i = 0; i < 430; i++)
			{
				KeyCode keyCode = (KeyCode)i;
				if (Input.GetKeyDown(keyCode))
				{
					OnKeyDown.Invoke(keyCode);
				}
				if (Input.GetKeyUp(keyCode))
				{
					OnKeyUp.Invoke(keyCode);
				}
			}
		}
	}
}
