using System.Collections.Generic;
using UnityEngine;

namespace BlendModes
{
	public static class BlendMaterials
	{
		private static Dictionary<ObjectType, Dictionary<RenderMode, Dictionary<BlendMode, Material>>> cachedMaterials = new Dictionary<ObjectType, Dictionary<RenderMode, Dictionary<BlendMode, Material>>>();

		public static Material GetMaterial(ObjectType objectType, RenderMode renderMode, BlendMode blendMode)
		{
			if (blendMode == BlendMode.Normal)
			{
				switch (objectType)
				{
				case ObjectType.MeshDefault:
				{
					Material material3 = new Material(Shader.Find("Diffuse"));
					material3.hideFlags = HideFlags.HideAndDontSave;
					return material3;
				}
				case ObjectType.SpriteDefault:
				{
					Material material2 = new Material(Shader.Find("Sprites/Default"));
					material2.hideFlags = HideFlags.HideAndDontSave;
					return material2;
				}
				case ObjectType.ParticleDefault:
				{
					Material material = new Material(Shader.Find("Particles/Additive"));
					material.hideFlags = HideFlags.HideAndDontSave;
					return material;
				}
				default:
					return null;
				}
			}
			if (Application.isEditor && renderMode == RenderMode.Framebuffer)
			{
				renderMode = RenderMode.Grab;
			}
			if (objectType != ObjectType.MeshDefault && objectType != ObjectType.ParticleDefault && cachedMaterials.ContainsKey(objectType) && cachedMaterials[objectType].ContainsKey(renderMode) && cachedMaterials[objectType][renderMode].ContainsKey(blendMode))
			{
				return cachedMaterials[objectType][renderMode][blendMode];
			}
			Material material4 = new Material(Resources.Load<Shader>(string.Format("BlendModes/{0}/{1}", objectType, renderMode)));
			material4.hideFlags = HideFlags.HideAndDontSave;
			material4.EnableKeyword("BM" + blendMode);
			if (!cachedMaterials.ContainsKey(objectType))
			{
				cachedMaterials.Add(objectType, new Dictionary<RenderMode, Dictionary<BlendMode, Material>>());
			}
			if (!cachedMaterials[objectType].ContainsKey(renderMode))
			{
				cachedMaterials[objectType].Add(renderMode, new Dictionary<BlendMode, Material>());
			}
			if (!cachedMaterials[objectType][renderMode].ContainsKey(blendMode))
			{
				cachedMaterials[objectType][renderMode].Add(blendMode, material4);
			}
			return material4;
		}
	}
}
