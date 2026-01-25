using UnityEngine;

public class MK_RotateObject : MonoBehaviour
{
	public float speed;

	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, speed, 0f));
	}
}
