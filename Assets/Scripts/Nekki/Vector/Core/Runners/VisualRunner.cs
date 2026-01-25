using System;
using System.Xml;
using BlendModes;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Scripts;
using Nekki.Vector.Core.Scripts.Primitive;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class VisualRunner : MatrixSupport
	{
		private Nekki.Vector.Core.Scripts.Primitive.Sprite _Sprite;

		private SpriteRenderer _SpriteRender;

		protected AnimationSprite _AnimationSprite;

		protected CustomAnimationSprite _CustomAnimationSprite;

		private bool _UseSolidMesh;

		private float _ImageWidth;

		private float _ImageHeight;

		private float _FactorX;

		private float _FactorY;

		private Parallax _Paralax;

		private bool _HasBlendModeEffect;

		private BlendMode _BlendMode;

		private Color _Color = new Color(-1f, 1f, 1f, 1f);

		public static int Counter = 0;

		private string _Layer = string.Empty;

		private int _Order;

		private static Shader _ShaderMultiply = Shader.Find("Sprites/Legacy/Multiply");

		private static Shader _ShaderLinearDodge = Shader.Find("Sprites/Legacy/LinearDodge");

		private static Material _SharedMultiplyMaterial = new Material(_ShaderMultiply);

		private static Material _SharedLinearDodgeMaterial = new Material(_ShaderLinearDodge);

		public float ImageWidth
		{
			get
			{
				return _ImageWidth;
			}
		}

		public float ImageHeight
		{
			get
			{
				return _ImageHeight;
			}
		}

		public float FactorX
		{
			get
			{
				return _FactorX;
			}
		}

		public float FactorY
		{
			get
			{
				return _FactorY;
			}
		}

		public override Element ParentElements
		{
			get
			{
				return base.ParentElements;
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
			}
		}

		public VisualRunner(string p_name, float p_x, float p_y, float p_width, float p_height, Element p_elements, XmlNode p_node)
			: base(p_x, p_y, p_elements, p_node)
		{
			base.Name = p_name;
			_TypeClass = TypeRunner.Visual;
			_ImageWidth = p_width;
			_ImageHeight = p_height;
			if (p_node["Properties"] != null && p_node["Properties"]["Static"] != null && p_node["Properties"]["Static"]["StartColor"] != null)
			{
				_Color = ColorUtils.FromHex(XmlUtils.ParseString(p_node["Properties"]["Static"]["StartColor"].Attributes["Color"]));
			}
			_Layer = XmlUtils.ParseString(p_node.Attributes["Layer"], string.Empty);
			_Order = ++Counter;
			float k = LayersController.GetK(XmlUtils.ParseString(p_node.Attributes["Layer"], string.Empty));
			_FactorX = XmlUtils.ParseFloat(p_node.Attributes["Factor"], float.NaN);
			_FactorY = 1f - k * (1f - FactorX);
			if (p_node["Properties"] != null && p_node["Properties"]["Static"] != null && p_node["Properties"]["Static"]["BlendMode"] != null)
			{
				_HasBlendModeEffect = true;
				XmlNode xmlNode = p_node["Properties"]["Static"]["BlendMode"];
				if (xmlNode.Attributes != null && xmlNode.Attributes["Mode"] != null)
				{
					_BlendMode = (BlendMode)(int)Enum.Parse(typeof(BlendMode), xmlNode.Attributes["Mode"].Value);
				}
			}
			_UseSolidMesh = XmlUtils.ParseBool(p_node.Attributes["SolidMesh"]);
		}

		public override void Generate()
		{
			base.Generate();
		}

		protected override void GenerateObject()
		{
			base.GenerateObject();
		}

		public virtual void GenerateContent()
		{
			CreateVisualContent();
			Transform();
			UpdateColor();
			UpdateLayer();
		}

		private void CreateVisualContent()
		{
			if (_TypeClass != TypeRunner.Visual)
			{
				MakeAnimationAndRun();
			}
			else
			{
				if (_Sprite != null)
				{
					return;
				}
				if (Mathf.Abs(_FactorX) > float.Epsilon)
				{
					_Paralax = base.UnityObject.AddComponent<Parallax>();
					_Paralax.SetFactor(_FactorX, _FactorY);
					_Paralax.SetParent(_ParentElements.Parent.UnityObject);
				}
				if (!Settings.Visual.Image.Visible)
				{
					_Color = Color.gray;
				}
				if (_UseSolidMesh)
				{
					_Sprite = base.UnityObject.AddComponent<Nekki.Vector.Core.Scripts.Primitive.Sprite>();
					_Sprite._Mesh = ResourcesMap.GetMesh(_Name);
					if (_Color.r < 0f)
					{
						_Color = Color.black;
					}
				}
				else
				{
					CreateSprite();
				}
			}
		}

		private void CreateSprite()
		{
			_SpriteRender = base.UnityObject.AddComponent<SpriteRenderer>();
			UnityEngine.Sprite sprite = ResourcesMap.GetSprite(_Name);
			if (sprite == null)
			{
				DebugUtils.LogFormat("Pic: {0}", _Name);
				return;
			}
			_SpriteRender.sprite = sprite;
			float width = sprite.rect.width;
			float height = sprite.rect.height;
			if (_SupportUnityObject != null)
			{
				_Transformation[0, 0] = _Transformation[0, 0] / width;
				_Transformation[0, 1] = _Transformation[0, 1] / width;
				_Transformation[1, 0] = _Transformation[1, 0] / height;
				_Transformation[1, 1] = _Transformation[1, 1] / height;
			}
			else
			{
				_CachedTransform.localScale = new Vector3(_CachedTransform.localScale.x / width, _CachedTransform.localScale.y / height, 1f);
			}
			if (_HasBlendModeEffect)
			{
				if (_BlendMode == BlendMode.Multiply)
				{
					_SpriteRender.sharedMaterial = _SharedMultiplyMaterial;
				}
				else if (_BlendMode == BlendMode.LinearDodge)
				{
					_SpriteRender.sharedMaterial = _SharedLinearDodgeMaterial;
				}
			}
		}

		public override bool Render()
		{
			return true;
		}

		protected override void Transform()
		{
			if (!(_SupportUnityObject == null))
			{
				base.Transform();
				if (!(_Sprite == null))
				{
					_Sprite.SetWidthHeight(_SupportUnityObject.transform.localScale.x, _SupportUnityObject.transform.localScale.y);
					_SupportUnityObject.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}

		public override void InitRunner()
		{
			base.InitRunner();
			if (float.IsNaN(_FactorX))
			{
				_FactorX = ParentElements.Parent.FactorX;
			}
			if (float.IsNaN(_FactorY))
			{
				_FactorY = ParentElements.Parent.FactorY;
			}
			BatchingByScaleZFixer.FixForBatching(base.UnityObject.transform);
		}

		protected void MakeAnimationAndRun()
		{
			_SpriteRender = base.UnityObject.AddComponent<SpriteRenderer>();
			if (_TypeClass == TypeRunner.CustomAnimation)
			{
				_CustomAnimationSprite = base.UnityObject.AddComponent<CustomAnimationSprite>();
				_CustomAnimationSprite.Init(_Name, _SpriteRender);
			}
			else
			{
				_AnimationSprite = base.UnityObject.AddComponent<AnimationSprite>();
				_AnimationSprite.Init(_Name, _SpriteRender);
			}
			UnityEngine.Sprite sprite = _SpriteRender.sprite;
			float width = sprite.rect.width;
			float height = sprite.rect.height;
			if (_SupportUnityObject != null)
			{
				_Transformation[0, 0] = _Transformation[0, 0] / width;
				_Transformation[0, 1] = _Transformation[0, 1] / width;
				_Transformation[1, 0] = _Transformation[1, 0] / height;
				_Transformation[1, 1] = _Transformation[1, 1] / height;
			}
			else
			{
				_CachedTransform.localScale = new Vector3(_CachedTransform.localScale.x / width, _CachedTransform.localScale.y / height, 1f);
			}
			if (_HasBlendModeEffect)
			{
				if (_BlendMode == BlendMode.Multiply)
				{
					_SpriteRender.sharedMaterial = _SharedMultiplyMaterial;
				}
				else if (_BlendMode == BlendMode.LinearDodge)
				{
					_SpriteRender.sharedMaterial = _SharedLinearDodgeMaterial;
				}
			}
		}

		public override void Move(Vector3f p_point)
		{
			Shift(p_point);
		}

		public override void TransformColor(Color p_delta)
		{
			_Color += p_delta;
			UpdateColor();
		}

		public override void TransformColorEnd(Color p_color)
		{
			_Color = p_color;
			UpdateColor();
		}

		public override void TransformResize(float p_w, float p_h)
		{
			Transform transform = base.UnityObject.transform;
			transform.localScale = new Vector3(p_w * transform.localScale.x, p_h * transform.localScale.y, 1f);
		}

		public void UpdateColor()
		{
			Color color = ParentElements.Parent.Color;
			if (color.r < 0f && _Color.r < 0f)
			{
				return;
			}
			if (color.r < 0f)
			{
				if (_Sprite != null)
				{
					_Sprite.Color = _Color;
				}
				if (_SpriteRender != null)
				{
					_SpriteRender.color = _Color;
				}
				return;
			}
			if (_Color.r < 0f)
			{
				if (_Sprite != null)
				{
					_Sprite.Color = color;
				}
				if (_SpriteRender != null)
				{
					_SpriteRender.color = color;
				}
				return;
			}
			Color color2 = Color.Lerp(color, _Color, 0.5f);
			color2.a = ParentElements.Parent.Color.a * _Color.a;
			if (_Sprite != null)
			{
				_Sprite.Color = color2;
			}
			if (_SpriteRender != null)
			{
				_SpriteRender.color = color2;
			}
		}

		private void UpdateLayer()
		{
			if (_UseSolidMesh)
			{
				_Sprite.SetSortingLayerAndOrder(_Layer, _Order);
				return;
			}
			_SpriteRender.flipY = true;
			_SpriteRender.sortingLayerName = _Layer;
			_SpriteRender.sortingOrder = _Order;
		}

		public override void End()
		{
			base.End();
			_Sprite = null;
			_SpriteRender = null;
			_UnityObject = null;
			_SupportUnityObject = null;
		}

		public override void TransformLayer(string p_layer)
		{
			_Layer = p_layer;
			UpdateLayer();
		}
	}
}
