using Nekki.Vector.Core.Audio;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundClick : MonoBehaviour
{
	[SerializeField]
	private string _SoundAlias;

	private Button.ButtonClickedEvent _OnClickEvents;

	public string SoundAlias
	{
		set
		{
			_SoundAlias = value;
		}
	}

	private void Awake()
	{
		Button component = GetComponent<Button>();
		if (!(component == null))
		{
			_OnClickEvents = component.onClick;
			component.onClick = new Button.ButtonClickedEvent();
			component.onClick.AddListener(OnButtonTap);
		}
	}

	private void OnButtonTap()
	{
		AudioManager.PlaySound(_SoundAlias);
		_OnClickEvents.Invoke();
	}
}
