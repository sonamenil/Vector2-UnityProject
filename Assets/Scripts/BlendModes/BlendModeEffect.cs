using UnityEngine;
using UnityEngine.UI;

namespace BlendModes
{
	[AddComponentMenu("Effects/Blend Mode")]
	[ExecuteInEditMode]
	public class BlendModeEffect : MonoBehaviour
	{
		[SerializeField]
		private BlendMode _blendMode;

		[SerializeField]
		private RenderMode _renderMode;

		[SerializeField]
		private Texture2D _texture;

		[SerializeField]
		private Color _tintColor = Color.white;

		public BlendMode BlendMode
		{
			get
			{
				return _blendMode;
			}
			set
			{
				SetBlendMode(value, RenderMode);
			}
		}

		public RenderMode RenderMode
		{
			get
			{
				return _renderMode;
			}
			set
			{
				SetBlendMode(BlendMode, value);
			}
		}

		public Texture2D Texture
		{
			get
			{
				return _texture;
			}
			set
			{
				if ((bool)Material && (ObjectType == ObjectType.MeshDefault || ObjectType == ObjectType.ParticleDefault))
				{
					Material.mainTexture = value;
				}
				_texture = value;
			}
		}

		public Color TintColor
		{
			get
			{
				return _tintColor;
			}
			set
			{
				if ((bool)Material && (ObjectType == ObjectType.MeshDefault || ObjectType == ObjectType.ParticleDefault))
				{
					Material.color = value;
				}
				_tintColor = value;
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				if ((bool)GetComponent<Image>())
				{
					return ObjectType.UIDefault;
				}
				if ((bool)GetComponent<Text>())
				{
					return ObjectType.UIDefaultFont;
				}
				if ((bool)GetComponent<SpriteRenderer>())
				{
					return ObjectType.SpriteDefault;
				}
				if ((bool)GetComponent<MeshRenderer>())
				{
					return ObjectType.MeshDefault;
				}
				if ((bool)GetComponent<ParticleSystem>())
				{
					return ObjectType.ParticleDefault;
				}
				return ObjectType.Unknown;
			}
		}

		public Material Material
		{
			get
			{
				switch (ObjectType)
				{
				case ObjectType.UIDefault:
					return GetComponent<Image>().material;
				case ObjectType.UIDefaultFont:
					return GetComponent<Text>().material;
				case ObjectType.SpriteDefault:
					return GetComponent<SpriteRenderer>().sharedMaterial;
				case ObjectType.MeshDefault:
					return GetComponent<MeshRenderer>().sharedMaterial;
				case ObjectType.ParticleDefault:
					return GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial;
				default:
					return null;
				}
			}
			set
			{
				switch (ObjectType)
				{
				case ObjectType.UIDefault:
					GetComponent<Image>().material = value;
					break;
				case ObjectType.UIDefaultFont:
					GetComponent<Text>().material = value;
					break;
				case ObjectType.SpriteDefault:
					GetComponent<SpriteRenderer>().sharedMaterial = value;
					break;
				case ObjectType.MeshDefault:
					GetComponent<MeshRenderer>().sharedMaterial = value;
					break;
				case ObjectType.ParticleDefault:
					GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial = value;
					break;
				}
			}
		}

		public void SetBlendMode(BlendMode blendMode, RenderMode renderMode = RenderMode.Grab)
		{
			if (ObjectType != 0)
			{
				Material = BlendMaterials.GetMaterial(ObjectType, renderMode, blendMode);
				Texture = Texture;
				TintColor = TintColor;
				_blendMode = blendMode;
				_renderMode = renderMode;
			}
		}

		public void OnEnable()
		{
			if ((bool)Material && (bool)Material.mainTexture)
			{
				Texture = (Texture2D)Material.mainTexture;
			}
			SetBlendMode(BlendMode, RenderMode);
		}

		public void OnDisable()
		{
			Texture2D texture = Texture;
			Material = BlendMaterials.GetMaterial(ObjectType, RenderMode.Grab, BlendMode.Normal);
			if ((bool)Material && (bool)texture)
			{
				Material.mainTexture = texture;
			}
		}
	}
}
