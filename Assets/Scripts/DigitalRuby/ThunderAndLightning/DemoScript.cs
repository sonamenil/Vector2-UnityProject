using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScript : MonoBehaviour
	{
		private enum RotationAxes
		{
			MouseXAndY = 0,
			MouseX = 1,
			MouseY = 2
		}

		private const float fastCloudSpeed = 50f;

		public ThunderAndLightningScript ThunderAndLightningScript;

		public LightningBoltScript LightningBoltScript;

		public ParticleSystem CloudParticleSystem;

		public float MoveSpeed = 250f;

		private float deltaTime;

		private float fpsIncrement;

		private string fpsText;

		private RotationAxes axes;

		private float sensitivityX = 15f;

		private float sensitivityY = 15f;

		private float minimumX = -360f;

		private float maximumX = 360f;

		private float minimumY = -60f;

		private float maximumY = 60f;

		private float rotationX;

		private float rotationY;

		private Quaternion originalRotation;

		private void UpdateThunder()
		{
			if (ThunderAndLightningScript != null)
			{
				if (Input.GetKeyDown(KeyCode.Alpha1))
				{
					ThunderAndLightningScript.CallNormalLightning();
				}
				else if (Input.GetKeyDown(KeyCode.Alpha2))
				{
					ThunderAndLightningScript.CallIntenseLightning();
				}
				else if (Input.GetKeyDown(KeyCode.Alpha3) && CloudParticleSystem != null)
				{
					CloudParticleSystem.playbackSpeed = ((CloudParticleSystem.playbackSpeed != 1f) ? 1f : 50f);
				}
			}
		}

		private void UpdateMovement()
		{
			float num = MoveSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.W))
			{
				Camera.main.transform.Translate(0f, 0f, num);
			}
			if (Input.GetKey(KeyCode.S))
			{
				Camera.main.transform.Translate(0f, 0f, 0f - num);
			}
			if (Input.GetKey(KeyCode.A))
			{
				Camera.main.transform.Translate(0f - num, 0f, 0f);
			}
			if (Input.GetKey(KeyCode.D))
			{
				Camera.main.transform.Translate(num, 0f, 0f);
			}
		}

		private void UpdateMouseLook()
		{
			if (axes == RotationAxes.MouseXAndY)
			{
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationX = ClampAngle(rotationX, minimumX, maximumX);
				rotationY = ClampAngle(rotationY, minimumY, maximumY);
				Quaternion quaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
				Quaternion quaternion2 = Quaternion.AngleAxis(rotationY, -Vector3.right);
				base.transform.localRotation = originalRotation * quaternion * quaternion2;
			}
			else if (axes == RotationAxes.MouseX)
			{
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationX = ClampAngle(rotationX, minimumX, maximumX);
				Quaternion quaternion3 = Quaternion.AngleAxis(rotationX, Vector3.up);
				base.transform.localRotation = originalRotation * quaternion3;
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = ClampAngle(rotationY, minimumY, maximumY);
				Quaternion quaternion4 = Quaternion.AngleAxis(0f - rotationY, Vector3.right);
				base.transform.localRotation = originalRotation * quaternion4;
			}
		}

		private void UpdateQuality()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				QualitySettings.SetQualityLevel(0);
			}
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				QualitySettings.SetQualityLevel(1);
			}
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				QualitySettings.SetQualityLevel(2);
			}
			else if (Input.GetKeyDown(KeyCode.F4))
			{
				QualitySettings.SetQualityLevel(3);
			}
			else if (Input.GetKeyDown(KeyCode.F5))
			{
				QualitySettings.SetQualityLevel(4);
			}
			else if (Input.GetKeyDown(KeyCode.F6))
			{
				QualitySettings.SetQualityLevel(5);
			}
		}

		private void UpdateOther()
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.LoadLevel(0);
			}
		}

		private void OnGUI()
		{
			int width = Screen.width;
			int height = Screen.height;
			GUIStyle gUIStyle = new GUIStyle();
			Rect position = new Rect(10f, height - 50, width, height * 2 / 100);
			gUIStyle.alignment = TextAnchor.UpperLeft;
			gUIStyle.fontSize = height * 2 / 50;
			gUIStyle.normal.textColor = Color.white;
			if ((fpsIncrement += Time.deltaTime) > 1f)
			{
				fpsIncrement -= 1f;
				float num = deltaTime * 1000f;
				float num2 = 1f / deltaTime;
				fpsText = string.Format("{0:0.0} ms ({1:0.} fps)", num, num2);
			}
			GUI.Label(position, fpsText, gUIStyle);
		}

		private void Update()
		{
			UpdateThunder();
			UpdateMovement();
			UpdateMouseLook();
			UpdateQuality();
			UpdateOther();
		}

		private void Start()
		{
			originalRotation = base.transform.localRotation;
			if (CloudParticleSystem != null)
			{
				CloudParticleSystem.playbackSpeed = 50f;
			}
		}

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}
