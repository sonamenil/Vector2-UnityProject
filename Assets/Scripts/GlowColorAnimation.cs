using UnityEngine;

public class GlowColorAnimation : MonoBehaviour
{
	public Color colorA;

	public Color colorB;

	public float speed;

	public Material glowMaterial;

	private void Update()
	{
		Color color = Color.Lerp(colorA, colorB, Mathf.PingPong(Time.time * speed, 1f));
		glowMaterial.SetColor("_GlowColorMult", color);
	}
}
