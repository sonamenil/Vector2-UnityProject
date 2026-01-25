using System.Collections;
using UnityEngine;

public class SphereBounce : MonoBehaviour
{
	public float force;

	public float torque;

	public float interval;

	private Rigidbody thisRigidbody;

	private Mesh thisMesh;

	private Color[] startColor;

	private float[] startTime;

	private float[] duration;

	private Color targetColor;

	private bool needToLerp;

	private WaitForSeconds delay;

	public void Start()
	{
		thisRigidbody = GetComponent<Rigidbody>();
		thisMesh = GetComponent<MeshFilter>().mesh;
		startColor = new Color[thisMesh.vertexCount];
		startTime = new float[thisMesh.vertexCount];
		duration = new float[thisMesh.vertexCount];
		delay = new WaitForSeconds(interval + Random.value * 4f);
		targetColor = Color.white;
		needToLerp = false;
		Vector3[] vertices = thisMesh.vertices;
		Color[] array = new Color[vertices.Length];
		for (int i = 0; i < vertices.Length; i++)
		{
			array[i] = targetColor;
		}
		thisMesh.colors = array;
		StartCoroutine(applyForceAndTorque());
	}

	public void Update()
	{
		lerpColors();
	}

	public void OnCollisionEnter(Collision collision)
	{
		Material sharedMaterial = collision.gameObject.GetComponent<Renderer>().sharedMaterial;
		bool flag = true;
		if (sharedMaterial.HasProperty("_GlowColor"))
		{
			targetColor = sharedMaterial.GetColor("_GlowColor");
			flag = targetColor == Color.black;
		}
		if (flag)
		{
			targetColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
		}
		for (int i = 0; i < startTime.Length; i++)
		{
			startColor[i] = thisMesh.colors[i];
			startTime[i] = Time.time;
			duration[i] = Random.Range(0.35f, 1f);
		}
		needToLerp = true;
	}

	private void lerpColors()
	{
		if (!needToLerp)
		{
			return;
		}
		bool flag = false;
		Vector3[] vertices = thisMesh.vertices;
		Color[] array = new Color[vertices.Length];
		for (int i = 0; i < vertices.Length; i++)
		{
			array[i] = Color.Lerp(startColor[i], targetColor, (Time.time - startTime[i]) / duration[i]);
			if (array[i] != targetColor && !flag)
			{
				flag = true;
			}
		}
		thisMesh.colors = array;
		needToLerp = flag;
	}

	private IEnumerator applyForceAndTorque()
	{
		while (true)
		{
			thisRigidbody.AddForce(force * Random.value * (float)((!((double)Random.value > 0.5)) ? 1 : (-1)), force * Random.value * (float)((!((double)Random.value > 0.5)) ? 1 : (-1)) / 4f, 0f, ForceMode.Force);
			thisRigidbody.AddTorque(torque * Random.value * (float)((!((double)Random.value > 0.5)) ? 1 : (-1)), torque * Random.value, torque * Random.value * (float)((!((double)Random.value > 0.5)) ? 1 : (-1)), ForceMode.VelocityChange);
			yield return delay;
		}
	}
}
