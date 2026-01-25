using UnityEngine;

namespace BlendModes
{
	public class DemoRotator : MonoBehaviour
	{
		public float Speed;

		public Vector3 Axis = new Vector3(1f, 1f, 1f);

		private void Update()
		{
			base.transform.Rotate(Axis, Speed * Time.deltaTime);
		}
	}
}
