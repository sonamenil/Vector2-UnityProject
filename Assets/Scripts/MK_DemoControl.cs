using MKGlowSystem;
using UnityEngine;

public class MK_DemoControl : MonoBehaviour
{
	private MKGlow mkGlow;

	private GUIStyle bStyle;

	private GUIStyle r2bStyle;

	public GUISkin skin;

	private int currentRoom;

	private int currentRoom2Texture;

	private int currentRoom1Object;

	private int currentRoom1Texture;

	private int currentRoom1GlowColor;

	private int currentRoom1GlowTexColor = 2;

	private int currentRoom0GlowColor;

	private int currentRoom1Shader;

	public Shader[] room1Shaders = new Shader[3];

	public Cubemap cm;

	private float room1RimP = 3f;

	private int currentRimColor;

	private float room1GlowIntensity = 0.75f;

	private float room1GlowTextureStrength = 1f;

	private float room1GlowWidth;

	public Texture[] room2Tex = new Texture[2];

	public GameObject[] room2Objects = new GameObject[10];

	public GameObject[] room1Objects = new GameObject[2];

	private void Awake()
	{
		mkGlow = GetComponent<MKGlow>();
		InitGlowSystem();
		skin.horizontalSlider.fixedHeight = 25f;
	}

	private void Update()
	{
		ManageRoom();
	}

	private void InitRoom1()
	{
		room1GlowTextureStrength = 1f;
		room1GlowIntensity = 1f;
		room1GlowWidth = 0f;
		currentRoom1GlowColor = 0;
		currentRoom1Object = 0;
		currentRoom1GlowTexColor = 0;
		currentRoom1Shader = 0;
		room1RimP = 3f;
		currentRimColor = 0;
	}

	private void InitRoom2()
	{
		currentRoom2Texture = 0;
	}

	private void InitGlowSystem()
	{
		mkGlow.BlurIterations = 5;
		mkGlow.BlurOffset = 0.25f;
		mkGlow.Samples = 4;
		mkGlow.GlowIntensity = 0.3f;
		mkGlow.BlurSpread = 0.25f;
		mkGlow.GlowType = MKGlowType.Selective;
		mkGlow.GlowQuality = MKGlowQuality.High;
		currentRoom0GlowColor = 0;
	}

	private void CreateMatrix(int depth, float nativeWidth, float nativeHeight)
	{
		float x = (float)Screen.width / nativeWidth;
		float y = (float)Screen.height / nativeHeight;
		GUI.matrix = Matrix4x4.TRS(new Vector3(0f, 0f, 1f), Quaternion.identity, new Vector3(x, y, depth));
	}

	private void OnGUI()
	{
		if (bStyle == null)
		{
			bStyle = new GUIStyle(GUI.skin.button);
		}
		if (r2bStyle == null)
		{
			r2bStyle = new GUIStyle();
		}
		bStyle.fontSize = 40;
		r2bStyle.fontSize = 40;
		r2bStyle.normal.textColor = Color.white;
		CreateMatrix(1, 1920f, 1080f);
		SwitchRoom();
		if (currentRoom == 1)
		{
			ManageRoom1();
		}
		else if (currentRoom == 2)
		{
			ManageRoom2();
		}
		else
		{
			ManageRoom0();
		}
	}

	private void ManageRoom()
	{
		Quaternion rotation = default(Quaternion);
		if (currentRoom == 1)
		{
			rotation.eulerAngles = new Vector3(10f, 120f, 0f);
		}
		else if (currentRoom == 2)
		{
			rotation.eulerAngles = new Vector3(10f, 240f, 0f);
		}
		else
		{
			rotation.eulerAngles = new Vector3(10f, 0f, 0f);
		}
		base.transform.rotation = rotation;
	}

	private void SwitchRoom()
	{
		if (GUI.Button(new Rect(835f, 950f, 250f, 100f), "Next Room", bStyle))
		{
			currentRoom++;
		}
		if (currentRoom > 2)
		{
			currentRoom = 0;
		}
	}

	private void ManageRoom0()
	{
		InitRoom1();
		InitRoom2();
		GUI.skin = skin;
		if (mkGlow.GlowType == MKGlowType.Selective)
		{
			if (GUI.Button(new Rect(25f, 900f, 400f, 50f), "Switch Glowmode", bStyle))
			{
				mkGlow.GlowType = MKGlowType.Fullscreen;
			}
		}
		else if (GUI.Button(new Rect(25f, 900f, 400f, 50f), "Switch Glowmode", bStyle))
		{
			mkGlow.GlowType = MKGlowType.Selective;
		}
		if (GUI.Button(new Rect(25f, 800f, 400f, 50f), "Switch Glowquality", bStyle))
		{
			if (mkGlow.GlowQuality == MKGlowQuality.High)
			{
				mkGlow.GlowQuality = MKGlowQuality.Low;
			}
			else
			{
				mkGlow.GlowQuality = MKGlowQuality.High;
			}
		}
		if (mkGlow.GlowQuality == MKGlowQuality.High)
		{
			GUI.Label(new Rect(1500f, 450f, 410f, 50f), "Glowquality: High", r2bStyle);
		}
		else
		{
			GUI.Label(new Rect(1500f, 450f, 410f, 50f), "Glowquality: Low", r2bStyle);
		}
		if (mkGlow.GlowType == MKGlowType.Selective)
		{
			GUI.Label(new Rect(1500f, 500f, 410f, 50f), "GlowMode: Selective", r2bStyle);
		}
		else
		{
			GUI.Label(new Rect(1500f, 500f, 410f, 50f), "GlowMode: Fullscreen", r2bStyle);
		}
		if (mkGlow.GlowType == MKGlowType.Fullscreen)
		{
			if (GUI.Button(new Rect(1500f, 400f, 400f, 50f), "Switch Color", bStyle))
			{
				currentRoom0GlowColor++;
				if (currentRoom0GlowColor > 4)
				{
					currentRoom0GlowColor = 0;
				}
			}
			Color[] array = new Color[5]
			{
				Color.white,
				Color.red,
				Color.cyan,
				Color.yellow,
				Color.green
			};
			array[currentRoom0GlowColor].a = 0f;
			mkGlow.FullScreenGlowTint = array[currentRoom0GlowColor];
		}
		mkGlow.BlurSpread = GUI.HorizontalSlider(new Rect(1500f, 650f, 400f, 100f), mkGlow.BlurSpread, 0.2f, 1f);
		GUI.Label(new Rect(1500f, 600f, 300f, 50f), "Blur Spread", r2bStyle);
		mkGlow.BlurSpread = GUI.HorizontalSlider(new Rect(1500f, 650f, 400f, 100f), mkGlow.BlurSpread, 0.2f, 1f);
		GUI.Label(new Rect(1500f, 700f, 300f, 50f), "Blur Offset", r2bStyle);
		mkGlow.BlurOffset = GUI.HorizontalSlider(new Rect(1500f, 750f, 400f, 100f), mkGlow.BlurOffset, 0f, 1f);
		GUI.Label(new Rect(1500f, 800f, 300f, 50f), "Samples", r2bStyle);
		mkGlow.Samples = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(1500f, 850f, 400f, 100f), mkGlow.Samples, 1f, 16f));
		GUI.Label(new Rect(1500f, 900f, 300f, 50f), "Blur Iterations", r2bStyle);
		mkGlow.BlurIterations = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(1500f, 950f, 400f, 100f), mkGlow.BlurIterations, 0f, 11f));
		GUI.Label(new Rect(1500f, 1000f, 300f, 50f), "Glow Intensity", r2bStyle);
		mkGlow.GlowIntensity = GUI.HorizontalSlider(new Rect(1500f, 1050f, 400f, 100f), mkGlow.GlowIntensity, 0f, 1f);
	}

	private void ManageRoom1()
	{
		InitRoom2();
		InitGlowSystem();
		Color[] array = new Color[5]
		{
			Color.white,
			Color.red,
			Color.cyan,
			Color.yellow,
			Color.green
		};
		if (GUI.Button(new Rect(150f, 860f, 400f, 50f), "Switch Shader", bStyle))
		{
			currentRoom1Shader++;
			if (currentRoom1Shader > 2)
			{
				currentRoom1Shader = 0;
			}
		}
		if (GUI.Button(new Rect(150f, 935f, 400f, 50f), "Switch Texture", bStyle))
		{
			currentRoom1Texture++;
			if (currentRoom1Texture > 1)
			{
				currentRoom1Texture = 0;
			}
		}
		if (GUI.Button(new Rect(150f, 1000f, 400f, 50f), "Switch Object", bStyle))
		{
			currentRoom1Object++;
			if (currentRoom1Object > 1)
			{
				currentRoom1Object = 0;
			}
		}
		if (currentRoom1Object == 0)
		{
			room1Objects[0].SetActive(true);
			room1Objects[1].SetActive(false);
		}
		else
		{
			room1Objects[0].SetActive(false);
			room1Objects[1].SetActive(true);
		}
		GUI.skin = skin;
		if (currentRoom1Shader == 1)
		{
			GUI.Label(new Rect(1500f, 500f, 400f, 50f), "Rim", r2bStyle);
			room1RimP = GUI.HorizontalSlider(new Rect(1500f, 550f, 400f, 100f), room1RimP, 0.5f, 6f);
			if (GUI.Button(new Rect(1400f, 900f, 500f, 50f), "Switch Rim Color", bStyle))
			{
				currentRimColor++;
				if (currentRimColor > 4)
				{
					currentRimColor = 0;
				}
			}
		}
		GUI.Label(new Rect(1500f, 600f, 400f, 50f), "Glow Power", r2bStyle);
		room1GlowIntensity = GUI.HorizontalSlider(new Rect(1500f, 650f, 400f, 100f), room1GlowIntensity, 0f, 2.5f);
		GUI.Label(new Rect(1500f, 700f, 400f, 50f), "Glow Texture Strength", r2bStyle);
		room1GlowTextureStrength = GUI.HorizontalSlider(new Rect(1500f, 750f, 400f, 100f), room1GlowTextureStrength, 0f, 10f);
		GUI.Label(new Rect(1500f, 800f, 400f, 50f), "Glow Width", r2bStyle);
		room1GlowWidth = GUI.HorizontalSlider(new Rect(1500f, 850f, 400f, 100f), room1GlowWidth, 0f, 0.075f);
		if (GUI.Button(new Rect(1400f, 960f, 500f, 50f), "Switch Glow Color", bStyle))
		{
			currentRoom1GlowColor++;
			if (currentRoom1GlowColor > 4)
			{
				currentRoom1GlowColor = 0;
			}
		}
		if (GUI.Button(new Rect(1400f, 1025f, 500f, 50f), "Switch Glow Texture Color", bStyle))
		{
			currentRoom1GlowTexColor++;
			if (currentRoom1GlowTexColor > 4)
			{
				currentRoom1GlowTexColor = 0;
			}
		}
		room1Objects[currentRoom1Object].GetComponent<Renderer>().material.shader = room1Shaders[currentRoom1Shader];
		if (currentRoom1Shader == 2)
		{
			room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetTexture("_ToonShade", cm);
		}
		else if (currentRoom1Shader == 1)
		{
			room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetFloat("_RimPower", room1RimP);
			room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetColor("_RimColor", array[currentRimColor]);
		}
		room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetColor("_MKGlowColor", array[currentRoom1GlowColor]);
		room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", array[currentRoom1GlowTexColor]);
		room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetTexture("_MKGlowTex", room2Tex[currentRoom1Texture]);
		room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetFloat("_MKGlowPower", room1GlowIntensity);
		room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetFloat("_MKGlowTexStrength", room1GlowTextureStrength);
		room1Objects[currentRoom1Object].GetComponent<Renderer>().material.SetFloat("_MKGlowOffSet", room1GlowWidth);
		string[] array2 = new string[3] { "Diffuse", "Rim", "Toon" };
		GUI.Label(new Rect(900f, 900f, 300f, 100f), array2[currentRoom1Shader], r2bStyle);
	}

	private void ManageRoom2()
	{
		InitRoom1();
		InitGlowSystem();
		GUI.Label(new Rect(150f, 800f, 500f, 50f), "DiffuseRim", r2bStyle);
		GUI.Label(new Rect(150f, 850f, 500f, 50f), "Diffuse", r2bStyle);
		GUI.Label(new Rect(450f, 800f, 500f, 50f), "DiffuseRim", r2bStyle);
		GUI.Label(new Rect(450f, 850f, 500f, 50f), "Diffuse", r2bStyle);
		GUI.Label(new Rect(800f, 800f, 500f, 50f), "ToonBasic", r2bStyle);
		GUI.Label(new Rect(800f, 850f, 500f, 50f), "Diffuse", r2bStyle);
		GUI.Label(new Rect(1150f, 800f, 500f, 50f), "Unlit", r2bStyle);
		GUI.Label(new Rect(1150f, 850f, 500f, 50f), "Diffuse", r2bStyle);
		GUI.Label(new Rect(1500f, 800f, 500f, 50f), "Transparent", r2bStyle);
		GUI.Label(new Rect(1500f, 850f, 500f, 50f), "Diffuse", r2bStyle);
		GUI.Label(new Rect(100f, 950f, 400f, 50f), "Some Variations ", r2bStyle);
		GUI.Label(new Rect(100f, 1000f, 500f, 50f), "Every Sphere has the same Textures", r2bStyle);
		if (GUI.Button(new Rect(1500f, 950f, 400f, 50f), "Switch Texture", bStyle))
		{
			currentRoom2Texture++;
			if (currentRoom2Texture > 1)
			{
				currentRoom2Texture = 0;
			}
			for (int i = 0; i < 10; i++)
			{
				room2Objects[i].GetComponent<Renderer>().material.SetTexture("_MKGlowTex", room2Tex[currentRoom2Texture]);
			}
		}
	}
}
