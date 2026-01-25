using UnityEngine;
using UnityEngine.UI;

public class Rotator : MonoBehaviour
{
	public Button _PlayButton;

	public Graphic _Target;

	public float _Delta;

	private void Start()
	{
	}

	private void Update()
	{
		if (_PlayButton.interactable)
		{
			float deltaTime = Time.deltaTime;
			float zAngle = Mathf.Lerp(0f, _Delta, deltaTime);
			_Target.rectTransform.Rotate(0f, 0f, zAngle, Space.Self);
		}
	}
}
