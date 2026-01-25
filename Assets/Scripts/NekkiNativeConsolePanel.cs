using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NekkiNativeConsolePanel : MonoBehaviour
{
	public delegate void OnConsoleActiveDelegate(bool state);

	[SerializeField]
	private RectTransform _Window;

	[SerializeField]
	private Text _Text;

	[SerializeField]
	private InputField _Input;

	[SerializeField]
	private RectTransform _PanelRectTransform;

	[SerializeField]
	private RectTransform _ContentRectTransform;

	[SerializeField]
	private ScrollRect _ScrollRect;

	[SerializeField]
	private Button _NextCommandButton;

	[SerializeField]
	private Button _PrevCommandButton;

	private static NekkiNativeConsolePanel _Panel = null;

	private string _Content = string.Empty;

	protected bool _IsInitialized;

	private static readonly List<string> _OutputList = new List<string>();

	private static readonly List<string> _CommandList = new List<string>();

	private int _LastCommandIndex = -1;

	public static NekkiNativeConsolePanel Instance
	{
		get
		{
			return _Panel;
		}
	}

	private bool IsActive
	{
		get
		{
			return _Window.gameObject.activeSelf;
		}
	}

	public static bool panelExists
	{
		get
		{
			return _Panel != null;
		}
	}

	public static event OnConsoleActiveDelegate OnConsoleActive;

	private void OnDestroy()
	{
		_Panel = null;
	}

	public static void Log(string p_string)
	{
		if (_Panel != null)
		{
			_Panel.AddText(p_string);
		}
		else
		{
			_OutputList.Add(p_string);
		}
	}

	public static void Clear()
	{
		if (_Panel != null)
		{
			_Panel._Text.text = string.Empty;
		}
	}

	public void OnSubmit()
	{
		if (!IsActive)
		{
			return;
		}
		string text = _Input.text;
		string text2 = NekkiConsole.ExecuteCommand(text);
		if (!string.IsNullOrEmpty(text))
		{
			if (!_CommandList.Contains(text))
			{
				_CommandList.Add(text);
			}
			AddText(string.Format("<b>{0}</b>", text));
			string[] array = text2.Split('\n');
			foreach (string text3 in array)
			{
				AddText(text3);
			}
			_Input.text = string.Empty;
			_Input.ActivateInputField();
		}
	}

	public void OnChange()
	{
		_Input.text = _Input.text.Replace("`", string.Empty);
		_Content = _Input.text;
	}

	public void OnButton()
	{
		Vector3 localPosition = _PanelRectTransform.localPosition;
		if (!IsActive)
		{
			Activate(true);
			_PanelRectTransform.localPosition = localPosition - new Vector3(0f, _Window.rect.height, 0f);
			_Input.ActivateInputField();
		}
		else
		{
			Activate(false);
			_PanelRectTransform.localPosition = localPosition + new Vector3(0f, _Window.rect.height, 0f);
			_Input.DeactivateInputField();
		}
	}

	protected void Activate(bool isActive)
	{
		_Window.gameObject.SetActive(isActive);
		if (_IsInitialized && NekkiNativeConsolePanel.OnConsoleActive != null)
		{
			NekkiNativeConsolePanel.OnConsoleActive(isActive);
		}
	}

	private void AddText(string text)
	{
		Text text2 = _Text;
		text2.text = text2.text + text + "\n";
		_ContentRectTransform.sizeDelta = new Vector2(0f, _Text.preferredHeight);
		_ScrollRect.verticalNormalizedPosition = 0f;
		_OutputList.Add(text);
	}

	private void Start()
	{
		_Panel = this;
		_Window.gameObject.SetActive(false);
		foreach (string output in _OutputList)
		{
			Text text = _Text;
			text.text = text.text + output + "\n";
		}
		_ContentRectTransform.sizeDelta = new Vector2(0f, _Text.preferredHeight);
		_IsInitialized = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			OnButton();
		}
		if (IsActive && _CommandList.Count > 0)
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

	public void SelectNextCommand()
	{
		_LastCommandIndex++;
		_LastCommandIndex = ((_LastCommandIndex < _CommandList.Count) ? _LastCommandIndex : (_LastCommandIndex = 0));
		_Input.text = _CommandList[_LastCommandIndex];
		_Input.caretPosition = _Input.text.Length;
	}

	public void SelectPrevCommand()
	{
		_LastCommandIndex--;
		_LastCommandIndex = ((_LastCommandIndex >= 0) ? _LastCommandIndex : (_LastCommandIndex = _CommandList.Count - 1));
		_Input.text = _CommandList[_LastCommandIndex];
		_Input.caretPosition = _Input.text.Length;
	}
}
