using System;
using UnityEngine;

namespace GlowEffect
{
	public class Demo : MonoBehaviour
	{
		public bool enableGlow = true;

		public GlowEffect.GlowMode glowMode;

		public GlowEffect.BlendMode blendMode;

		public int downsamplePower = 8;

		public float glowStrength = 2f;

		public int blurIterations = 8;

		public float blurSpread = 1.2f;

		public GameObject glowGroup;

		public GameObject alphaGlowGroup;

		public GlowEffect glowEffect;

		private Rect glowControlsVisibleRect;

		private Rect glowControlsNotVisibleRect;

		private bool showGlowControls;

		private float updateInterval = 0.5f;

		private float accum;

		private int frames;

		private float timeleft;

		private float fps;

		public void Start()
		{
			glowControlsVisibleRect = new Rect(0f, 0f, 320f, 500f);
			glowControlsNotVisibleRect = new Rect(0f, 0f, 320f, 100f);
			showGlowControls = true;
			timeleft = updateInterval;
			UpdateGlow();
		}

		public void OnGUI()
		{
			GUILayout.BeginArea((!showGlowControls) ? glowControlsNotVisibleRect : glowControlsVisibleRect);
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Toggle Controls"))
			{
				showGlowControls = !showGlowControls;
			}
			if (GUILayout.Button((Time.timeScale != 0f) ? "Pause" : "Play"))
			{
				Time.timeScale = ((Time.timeScale != 0f) ? 0f : 1f);
			}
			if (GUILayout.Button((Time.timeScale != 0.2f) ? "Slow Motion" : "Normal Speed"))
			{
				Time.timeScale = ((Time.timeScale != 0.2f) ? 0.2f : 1f);
			}
			GUILayout.EndHorizontal();
			if (showGlowControls)
			{
				bool flag = enableGlow;
				GUILayout.BeginHorizontal(GUILayout.Width(200f));
				GUILayout.Label(string.Format("Enable Glow: {0}", enableGlow));
				GUILayout.FlexibleSpace();
				enableGlow = GUILayout.Toggle(enableGlow, string.Empty);
				if (flag != enableGlow)
				{
					UpdateGlow();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(GUILayout.Width(240f));
				GUILayout.Label(string.Format("Glow Mode: {0}", Enum.GetName(typeof(GlowEffect.GlowMode), glowMode)));
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("<-"))
				{
					glowMode = (GlowEffect.GlowMode)((int)(glowMode - 1) % 4);
					if (glowMode < GlowEffect.GlowMode.Glow)
					{
						glowMode = GlowEffect.GlowMode.SimpleAlphaGlow;
					}
					UpdateGlow();
				}
				if (GUILayout.Button("->"))
				{
					glowMode = (GlowEffect.GlowMode)((int)(glowMode + 1) % 4);
					UpdateGlow();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(GUILayout.Width(240f));
				GUILayout.Label(string.Format("Blend Mode: {0}", Enum.GetName(typeof(GlowEffect.BlendMode), blendMode)));
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("<-"))
				{
					blendMode = (GlowEffect.BlendMode)((int)(blendMode - 1) % 4);
					if (blendMode < GlowEffect.BlendMode.Additive)
					{
						blendMode = GlowEffect.BlendMode.Subtract;
					}
					UpdateGlow();
				}
				if (GUILayout.Button("->"))
				{
					blendMode = (GlowEffect.BlendMode)((int)(blendMode + 1) % 4);
					UpdateGlow();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				GUILayout.Label(string.Format("Downsample size: {0}", Mathf.Pow(2f, downsamplePower)));
				GUILayout.FlexibleSpace();
				int num = downsamplePower;
				downsamplePower = Mathf.RoundToInt(GUILayout.HorizontalSlider(downsamplePower, 5f, 9f, GUILayout.Width(125f)));
				if (num != downsamplePower)
				{
					UpdateGlow();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				GUILayout.Label(string.Format("Blur Iterations: {0}", blurIterations));
				GUILayout.FlexibleSpace();
				num = blurIterations;
				blurIterations = Mathf.RoundToInt(GUILayout.HorizontalSlider(blurIterations, 1f, 20f, GUILayout.Width(125f)));
				if (num != blurIterations)
				{
					UpdateGlow();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				GUILayout.Label(string.Format("Blur Spread: {0}", decimal.Round((decimal)blurSpread, 2)));
				GUILayout.FlexibleSpace();
				float num2 = blurSpread;
				blurSpread = GUILayout.HorizontalSlider(blurSpread, 1f, 2.5f, GUILayout.Width(125f));
				if (num2 != blurSpread)
				{
					UpdateGlow();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				GUILayout.Label(string.Format("Glow Strength: {0}", decimal.Round((decimal)glowStrength, 2)));
				GUILayout.Width(50f);
				num2 = glowStrength;
				glowStrength = GUILayout.HorizontalSlider(glowStrength, 0.5f, 10f, GUILayout.Width(125f));
				if (num2 != glowStrength)
				{
					UpdateGlow();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.Label(string.Format("{0:F2} FPS", fps));
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		private void UpdateGlow()
		{
			glowEffect.enabled = enableGlow;
			if (enableGlow)
			{
				ActiveRecursively(glowGroup.transform, (int)glowMode % 2 == 0);
				ActiveRecursively(alphaGlowGroup.transform, (int)glowMode % 2 == 1);
				glowEffect.glowMode = glowMode;
				glowEffect.blendMode = blendMode;
				glowEffect.downsampleSize = (int)Mathf.Pow(2f, downsamplePower);
				glowEffect.blurIterations = blurIterations;
				glowEffect.blurSpread = blurSpread;
				glowEffect.glowStrength = glowStrength;
				glowEffect.enabled = false;
				glowEffect.enabled = true;
			}
		}

		private void ActiveRecursively(Transform obj, bool active)
		{
			foreach (Transform item in obj)
			{
				ActiveRecursively(item, active);
			}
			obj.gameObject.SetActive(active);
		}

		public void Update()
		{
			timeleft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			frames++;
			if ((double)timeleft <= 0.0)
			{
				fps = accum / (float)frames;
				timeleft = updateInterval;
				accum = 0f;
				frames = 0;
			}
		}
	}
}
