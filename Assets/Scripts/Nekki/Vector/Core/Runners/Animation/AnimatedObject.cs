using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Runners.Animation
{
	public class AnimatedObject
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

		private GameObject _obj = new GameObject
		{
			name = "[AnimatedObject]"
		};

		public TextAsset xmlAtlas;

		private List<FrameData> _frameData = new List<FrameData>();

		private int currentFrame;

		public AnimatedObject()
		{
			_obj.AddComponent<MeshFilter>();
			_obj.AddComponent<MeshRenderer>();
			InitAnimation();
			InitMaterial();
		}

		private void InitMaterial()
		{
			Material material = new Material(Shader.Find("Transparent/Diffuse"));
			float x = 2f;
			float y = 2f;
			string path = Directory.GetCurrentDirectory() + "/Assets/Resources/bird/birdAtlas.png";
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			Texture2D texture2D = new Texture2D(0, 0);
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.filterMode = FilterMode.Point;
			Texture2D texture2D2 = texture2D;
			texture2D2.LoadImage(array);
			Mesh mesh = new Mesh();
			_obj.GetComponent<MeshFilter>().mesh = mesh;
			_obj.GetComponent<Renderer>().material = material;
			Vector3[] vertices = new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(x, 0f, 0f),
				new Vector3(0f, y, 0f),
				new Vector3(x, y, 0f)
			};
			int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
			Vector3[] normals = new Vector3[4]
			{
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward
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
			mesh.normals = normals;
			mesh.uv = uv;
			_obj.GetComponent<Renderer>().material.mainTexture = texture2D2;
		}

		private void InitAnimation()
		{
			string filename = Directory.GetCurrentDirectory() + "/Assets/Resources/bird/bird.xml";
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			XmlNode xmlNode = xmlDocument.GetElementsByTagName("TextureAtlas").Item(0);
			XmlNodeList childNodes = xmlNode.ChildNodes;
			Vector2 vector = new Vector2(float.Parse(xmlNode.Attributes["width"].Value), float.Parse(xmlNode.Attributes["height"].Value));
			foreach (XmlNode item2 in childNodes)
			{
				spritePoint spritePoint = new spritePoint(item2);
				FrameData item = default(FrameData);
				item.offset = new Vector2((float)spritePoint.x / (vector.x / 100f) / 100f, (vector.y - (float)spritePoint.y - spritePoint.h) / (vector.y / 100f) / 100f);
				item.scale = new Vector2(spritePoint.w / (vector.x / 100f) / 100f, spritePoint.h / (vector.y / 100f) / 100f);
				_frameData.Add(item);
			}
		}

		public void SetSpriteAnimation()
		{
			FrameData frameData = _frameData[currentFrame];
			currentFrame++;
			if (currentFrame == _frameData.Count)
			{
				currentFrame = 0;
			}
			_obj.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", frameData.offset);
			_obj.GetComponent<Renderer>().material.SetTextureScale("_MainTex", frameData.scale);
		}
	}
}
