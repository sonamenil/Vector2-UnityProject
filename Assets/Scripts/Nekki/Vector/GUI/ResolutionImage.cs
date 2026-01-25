using Nekki.Vector.Core.User;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI
{
	[AddComponentMenu("UI_Nekki/ResolutionImage")]
	public class ResolutionImage : Image
	{
		private const string _DefaultTexturesPath = "UI/Textures/";

		private const string _DefaultAtlasPath = "UI/Atlases/";

		public const string LowQualitySuffix = "_low";

		[SerializeField]
		private string _TexturePath;

		[SerializeField]
		private string _SpriteName;

		public string TexturePath
		{
			get
			{
				return _TexturePath;
			}
			set
			{
				_TexturePath = value;
			}
		}

		public string SpriteName
		{
			get
			{
				return _SpriteName;
			}
			set
			{
				_SpriteName = value;
				SetSprite();
			}
		}

		public float Alpha
		{
			get
			{
				return color.a;
			}
			set
			{
				Color color = this.color;
				color.a = value;
				this.color = color;
			}
		}

		protected override void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			base.Awake();
			SetSprite();
		}

		private void SetSprite()
		{
			if (DataLocal.IsCurrentExists)
			{
				string[] array = _SpriteName.Split('.');
				if (array.Length == 1)
				{
					SetSingleSprite();
				}
				else
				{
					SetAtlasSprite(array[0]);
				}
			}
		}

		private void SetSingleSprite()
		{
			Sprite sprite = ResourcesAndBundles.Load<Sprite>(_TexturePath + GetSingleSpriteName());
			if (sprite == null)
			{
				sprite = ResourcesAndBundles.Load<Sprite>("UI/Textures/" + GetSingleSpriteName());
			}
			base.sprite = sprite;
		}

		private void SetAtlasSprite(string p_atlasName)
		{
			Sprite spriteFromAtlas = AtlasCache.GetSpriteFromAtlas(_TexturePath + GetAtlasName(p_atlasName), _SpriteName);
			if (spriteFromAtlas == null)
			{
				spriteFromAtlas = AtlasCache.GetSpriteFromAtlas("UI/Atlases/" + GetAtlasName(p_atlasName), _SpriteName);
			}
			base.sprite = spriteFromAtlas;
		}

		private string GetSingleSpriteName()
		{
			if (DataLocal.Current.Settings.UseLowResGraphics)
			{
				return _SpriteName + "_low";
			}
			return _SpriteName;
		}

		private string GetAtlasName(string p_name)
		{
			if (DataLocal.Current.Settings.UseLowResGraphics)
			{
				return p_name + "_low";
			}
			return p_name;
		}

		public override void SetNativeSize()
		{
			base.SetNativeSize();
			if (DataLocal.Current.Settings.UseLowResGraphics)
			{
				RectTransform component = GetComponent<RectTransform>();
				component.sizeDelta = new Vector2(component.sizeDelta.x * 2f, component.sizeDelta.y * 2f);
			}
		}

		protected override void OnDestroy()
		{
			base.sprite = null;
			base.OnDestroy();
		}
	}
}
