using System.Collections.Generic;
using BlendModes;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nekki.Vector.Core.Scripts.Primitive
{
	public class Sprite : MonoBehaviour
	{
		private Color _Color = new Color(1f, 1f, 1f, 1f);

		private Texture2D _Texture;

		private Rect _FrameRect = new Rect(0f, 0f, 1f, 1f);

		private float _Width = 1f;

		private float _Height = 1f;

		private static Shader _Shader = Shader.Find("Sprites/Legacy/Default");

		private static Shader _ShaderMultiply = Shader.Find("Sprites/Legacy/Multiply");

		private static Material _SharedMaterial = new Material(_Shader);

		private static Dictionary<Texture, Material> _Materials = new Dictionary<Texture, Material>();

		private static Dictionary<Texture, Material> _MaterialsWithMultiply = new Dictionary<Texture, Material>();

		private Material _Material;

		private MeshRenderer _MeshRender;

		public Mesh _Mesh;

		private bool _HasBlendModeEffect;

		private BlendModes.BlendMode _BlendMode;

		private BlendModes.RenderMode _BlendRenderMode = BlendModes.RenderMode.UnifiedGrab;

		private BlendModeEffect _BlendModeEffect;

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
				if (_Mesh != null)
				{
					int num = _Mesh.vertices.Length;
					Color[] array = new Color[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = _Color;
					}
					_Mesh.colors = array;
				}
				if (_BlendModeEffect != null)
				{
					_BlendModeEffect.TintColor = _Color;
				}
			}
		}

		public Texture2D Texture
		{
			get
			{
				return _Texture;
			}
			set
			{
				_Texture = value;
				if (_Material == null && _MeshRender != null)
				{
					_MeshRender.sharedMaterial = GetOrCreateMaterial(_HasBlendModeEffect && _BlendMode == BlendModes.BlendMode.Multiply, _Texture);
				}
				if (_Material != null)
				{
					_Material.mainTexture = _Texture;
				}
			}
		}

		public Rect FrameRect
		{
			set
			{
				_FrameRect = value;
				SetRectToMesh();
			}
		}

		private static Material GetOrCreateMaterial(bool p_isUseMultiply, Texture p_texture)
		{
			Dictionary<Texture, Material> dictionary = ((!p_isUseMultiply) ? _Materials : _MaterialsWithMultiply);
			Material material = null;
			if (dictionary.ContainsKey(p_texture))
			{
				material = dictionary[p_texture];
			}
			else
			{
				material = new Material((!p_isUseMultiply) ? _Shader : _ShaderMultiply);
				material.mainTexture = p_texture;
				dictionary.Add(p_texture, material);
			}
			return material;
		}

		public void SetWidthHeight(float p_width, float p_height)
		{
			_Width = p_width;
			_Height = p_height;
			UpdateWH();
		}

		public void SetSortingLayerAndOrder(string p_sortingLayerName, int p_order)
		{
			_MeshRender.sortingLayerName = p_sortingLayerName;
			_MeshRender.sortingOrder = p_order;
		}

		public void SetBlendMode(BlendModes.BlendMode mode, BlendModes.RenderMode render)
		{
			_HasBlendModeEffect = true;
			_BlendMode = mode;
			_BlendRenderMode = render;
			if (_BlendMode != BlendModes.BlendMode.Multiply && _BlendModeEffect != null)
			{
				_BlendModeEffect.SetBlendMode(mode, render);
			}
		}

		public void Start()
		{
			if (_Mesh == null)
			{
				_Mesh = new Mesh();
				UpdateWH();
				SetRectToMesh();
				_Mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };
				_Mesh.colors = new Color[4] { _Color, _Color, _Color, _Color };
			}
			base.gameObject.AddComponent<MeshFilter>().mesh = _Mesh;
			_MeshRender = base.gameObject.AddComponent<MeshRenderer>();
			_MeshRender.shadowCastingMode = ShadowCastingMode.Off;
			_MeshRender.receiveShadows = false;
			_MeshRender.lightProbeUsage = LightProbeUsage.Off;
			_MeshRender.reflectionProbeUsage = ReflectionProbeUsage.Off;
			if (_Texture != null)
			{
				Texture = _Texture;
			}
			else
			{
				_MeshRender.sharedMaterial = _SharedMaterial;
			}
			if (_HasBlendModeEffect && _BlendMode != BlendModes.BlendMode.Multiply)
			{
				_BlendModeEffect = base.gameObject.AddComponent<BlendModeEffect>();
				_BlendModeEffect.SetBlendMode(_BlendMode, _BlendRenderMode);
				_BlendModeEffect.TintColor = _Color;
			}
		}

		private void OnDestroy()
		{
			if (_Texture != null)
			{
				if (_HasBlendModeEffect && _BlendMode == BlendModes.BlendMode.Multiply)
				{
					if (_MaterialsWithMultiply.ContainsKey(_Texture))
					{
						_MaterialsWithMultiply.Remove(_Texture);
					}
				}
				else if (_Materials.ContainsKey(_Texture))
				{
					_Materials.Remove(_Texture);
				}
			}
			_Texture = null;
			_Material = null;
			if (_HasBlendModeEffect && _BlendMode != BlendModes.BlendMode.Multiply)
			{
				Object.Destroy(_BlendModeEffect.Material);
				_BlendModeEffect.Material = null;
				if (VectorPaths.UsingResources)
				{
					Resources.UnloadAsset(_BlendModeEffect.Texture);
					_BlendModeEffect.Texture = null;
				}
				else
				{
					Object.Destroy(_BlendModeEffect.Texture);
					_BlendModeEffect.Texture = null;
				}
			}
			_BlendModeEffect = null;
		}

		private void SetRectToMesh()
		{
			if (!(_Mesh == null))
			{
				Vector2[] uv = new Vector2[4]
				{
					new Vector2(_FrameRect.xMin, 1f - _FrameRect.yMax),
					new Vector2(_FrameRect.xMax, 1f - _FrameRect.yMax),
					new Vector2(_FrameRect.xMin, 1f - _FrameRect.yMin),
					new Vector2(_FrameRect.xMax, 1f - _FrameRect.yMin)
				};
				_Mesh.uv = uv;
			}
		}

		private void UpdateWH()
		{
			if (!(_Mesh == null))
			{
				_Mesh.vertices = new Vector3[4]
				{
					new Vector3(0f, _Height, 0f),
					new Vector3(_Width, _Height, 0f),
					new Vector3(0f, 0f, 0f),
					new Vector3(_Width, 0f, 0f)
				};
				_Mesh.RecalculateBounds();
			}
		}
	}
}
