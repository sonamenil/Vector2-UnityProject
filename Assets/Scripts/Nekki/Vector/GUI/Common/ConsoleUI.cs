using System;
using System.Collections.Generic;
using System.Text;
using Nekki.Vector.Core.Console;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class ConsoleUI : UIModule
	{
		[SerializeField]
		private RectTransform _Window;

		[SerializeField]
		private Text _Text;

		[SerializeField]
		private InputField _Input;

		[SerializeField]
		private RectTransform _ContentRectTransform;

		[SerializeField]
		private ScrollRect _ScrollRect;

		[SerializeField]
		private Button _NextCommandButton;

		[SerializeField]
		private Button _PrevCommandButton;

		private static ConsoleUI _Current;

		private static StringBuilder _OutputList = new StringBuilder();

		private static List<string> _CommandList = new List<string>();

		private int _LastCommandIndex = -1;

		private RectTransform _CachedRectTransform;

		public static ConsoleUI Current
		{
			get
			{
				return _Current;
			}
		}

		public bool IsWindowActive
		{
			get
			{
				return _Window.gameObject.activeSelf;
			}
		}

		public static event Action<bool> OnConsoleActive;

		protected override void Init()
		{
			base.Init();
			_Current = this;
			_Text.text = _OutputList.ToString();
			_Window.gameObject.SetActive(false);
			_ContentRectTransform.sizeDelta = new Vector2(0f, _Text.preferredHeight);
			_CachedRectTransform = GetComponent<RectTransform>();
		}

		protected override void Free()
		{
			base.Free();
			_Current = null;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				OnToggleConsoleButton();
			}
			if (IsWindowActive && _CommandList.Count > 0)
			{
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					SelectPrevCommand();
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					SelectNextCommand();
				}
			}
		}

		public static void Log(string p_string)
		{
			if (_Current != null)
			{
				_Current.AddText(p_string);
			}
			else
			{
				_OutputList.AppendLine(p_string);
			}
		}

		public static void Clear()
		{
			if (_Current != null)
			{
				_Current.ClearText();
			}
		}

		public void OnSubmit()
		{
			if (!IsWindowActive)
			{
				return;
			}
			string text = _Input.text;
			string text2 = ConsoleDatabase.ExecuteCommand(text);
			if (!string.IsNullOrEmpty(text))
			{
				if (!_CommandList.Contains(text))
				{
					_CommandList.Add(text);
				}
				AddText(string.Format("<b>{0}</b>", text));
				Debug.Log(text2);
				string[] array = text2.Split('\n');
				foreach (string p_text in array)
				{
					AddText(p_text);
				}
				_Input.text = string.Empty;
				_Input.ActivateInputField();
			}
		}

		public void OnChange()
		{
			_Input.text = _Input.text.Replace("`", string.Empty);
		}

		public void OnToggleConsoleButton()
		{
			Vector3 localPosition = _CachedRectTransform.localPosition;
			if (!IsWindowActive)
			{
				Activate(true);
				_CachedRectTransform.localPosition = localPosition - new Vector3(0f, _Window.rect.height, 0f);
				_Input.ActivateInputField();
			}
			else
			{
				Activate(false);
				_CachedRectTransform.localPosition = localPosition + new Vector3(0f, _Window.rect.height, 0f);
				_Input.DeactivateInputField();
			}
		}

		protected void Activate(bool p_isActive)
		{
			_Window.gameObject.SetActive(p_isActive);
			if (_IsInited && ConsoleUI.OnConsoleActive != null)
			{
				ConsoleUI.OnConsoleActive(p_isActive);
			}
		}

		private void AddText(string p_text)
		{
			_OutputList.AppendLine(p_text);
			_Text.text = _OutputList.ToString();
			_ContentRectTransform.sizeDelta = new Vector2(0f, _Text.preferredHeight);
			_ScrollRect.verticalNormalizedPosition = 0f;
		}

		private void ClearText()
		{
			_OutputList.Clear();
			_Text.text = string.Empty;
            _ContentRectTransform.sizeDelta = new Vector2(0f, _Text.preferredHeight);
            _ScrollRect.verticalNormalizedPosition = 0f;
        }

		public void SelectNextCommand()
		{
			if (_CommandList.Count != 0)
			{
				_LastCommandIndex++;
				if (_LastCommandIndex >= _CommandList.Count)
				{
					_LastCommandIndex = 0;
				}
				_Input.text = _CommandList[_LastCommandIndex];
				_Input.caretPosition = _Input.text.Length;
			}
		}

		public void SelectPrevCommand()
		{
			if (_CommandList.Count != 0)
			{
				_LastCommandIndex--;
				if (_LastCommandIndex < 0)
				{
					_LastCommandIndex = _CommandList.Count - 1;
				}
				_Input.text = _CommandList[_LastCommandIndex];
				_Input.caretPosition = _Input.text.Length;
			}
		}
	}
}
