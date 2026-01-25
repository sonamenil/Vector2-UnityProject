using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts
{
	public class TextureAnimator : MonoBehaviour
	{
		public class spritePoint
		{
			public int x;

			public int y;

			public float w;

			public float h;

			public spritePoint()
			{
			}

			public spritePoint(XmlNode p_node)
			{
				x = int.Parse(p_node.Attributes["x"].Value);
				y = int.Parse(p_node.Attributes["y"].Value);
				w = float.Parse(p_node.Attributes["w"].Value);
				h = float.Parse(p_node.Attributes["h"].Value);
			}
		}

		public struct FrameData
		{
			public Vector2 offset;

			public Vector2 scale;
		}

		public float Width;

		public float Height;

		public int TotalFrames;

		private int currentFrame;

		private string _FileName;

		private string _Path;

		private float times = 1f;

		private float _fps = 10f;

		private int _Iterations = -1;

		private bool autoRun;

		private List<FrameData> _framesData = new List<FrameData>();

		private Color _Color = new Color(1f, 1f, 1f, 1f);

		private Texture2D _Texture;

		public float FPS
		{
			set
			{
				_fps = value;
			}
		}

		public int Iterations
		{
			set
			{
				_Iterations = value;
			}
		}

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
				if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null)
				{
					GetComponent<Renderer>().material.color = _Color;
				}
			}
		}

		public void init(string p_fileName, string p_path, float p_widh, float p_heigh, Texture2D texture = null)
		{
			Width = p_widh;
			Height = p_heigh;
			_FileName = p_fileName;
			_Path = p_path;
			_Texture = texture;
			InitMaterial();
			InitAnimation();
		}

		public void runAuto()
		{
			autoRun = true;
		}

		public void Stop()
		{
			autoRun = false;
		}

		private void InitMaterial()
		{
			Shader shader = Shader.Find("Nekki/Unlit/Transparent Colored");
			Material material = new Material(shader);
			material.color = _Color;
			Material material2 = material;
			string p_fileName = _Path + "/" + _FileName + ".png";
			if (_Texture == null)
			{
				_Texture = ResourceManager.GetTexture(p_fileName);
				_Texture.wrapMode = TextureWrapMode.Clamp;
				_Texture.filterMode = FilterMode.Point;
			}
			Mesh mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh;
			GetComponent<Renderer>().material = material2;
			Vector3[] vertices = new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(0f, 1f, 0f),
				new Vector3(1f, 1f, 0f)
			};
			int[] triangles = new int[6] { 0, 1, 2, 2, 1, 3 };
			Vector3[] array = new Vector3[4]
			{
				Vector3.forward,
				Vector3.forward,
				Vector3.forward,
				Vector3.forward
			};
			Vector2[] uv = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uv;
			GetComponent<Renderer>().material.mainTexture = _Texture;
		}

		private void InitAnimation()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(_Path, _FileName + ".xml");
			XmlNode xmlNode = xmlDocument.GetElementsByTagName("TextureAtlas").Item(0);
			XmlNodeList childNodes = xmlNode.ChildNodes;
			Vector2 vector = new Vector2(float.Parse(xmlNode.Attributes["width"].Value), float.Parse(xmlNode.Attributes["height"].Value));
			Dictionary<string, XmlNode> dictionary = new Dictionary<string, XmlNode>();
			foreach (XmlNode item2 in childNodes)
			{
				dictionary.Add(item2.Attributes["n"].Value, item2);
			}
			int count = childNodes.Count;
			for (int i = 1; i <= count; i++)
			{
				XmlNode p_node = dictionary[_FileName + "_" + i + ".png"];
				spritePoint spritePoint = new spritePoint(p_node);
				FrameData frameData = default(FrameData);
				frameData.offset = new Vector2((float)spritePoint.x / (vector.x / 100f) / 100f, (vector.y - (float)spritePoint.y - spritePoint.h) / (vector.y / 100f) / 100f);
				frameData.scale = new Vector2(spritePoint.w / (vector.x / 100f) / 100f, spritePoint.h / (vector.y / 100f) / 100f);
				FrameData item = frameData;
				_framesData.Add(item);
			}
			TotalFrames = _framesData.Count;
			currentFrame = 0;
		}

		public void SetSpriteFrame(int p_index)
		{
			if (p_index < TotalFrames)
			{
				FrameData frameData = _framesData[p_index];
				GetComponent<Renderer>().material.SetTextureOffset("_MainTex", frameData.offset);
				GetComponent<Renderer>().material.SetTextureScale("_MainTex", frameData.scale);
			}
		}

		public void SetSpriteAnimation()
		{
			SetSpriteFrame(currentFrame);
			currentFrame++;
			if (currentFrame < TotalFrames)
			{
				return;
			}
			currentFrame = 0;
			if (_Iterations != -1)
			{
				_Iterations--;
				if (_Iterations <= 0)
				{
					autoRun = false;
				}
			}
		}

		private void FixedUpdate()
		{
			if (autoRun)
			{
				if (times >= 1f / _fps)
				{
					SetSpriteAnimation();
					times = 0f;
				}
				else
				{
					times += Time.deltaTime;
				}
			}
		}

		private void OnDestroy()
		{
			_Texture = null;
		}
	}
}
