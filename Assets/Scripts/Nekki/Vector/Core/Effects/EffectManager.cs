using UnityEngine;

namespace Nekki.Vector.Core.Effects
{
	public static class EffectManager
	{
		public static ParticleSystem Instantiate(string prefabName, Transform root)
		{
			GameObject gameObject = (GameObject)Resources.Load("Effects/Prefabs/" + prefabName);
			if (gameObject != null)
			{
				ParticleSystem component = Object.Instantiate(gameObject).GetComponent<ParticleSystem>();
				component.transform.SetParent(root, false);
				return component;
			}
			Debug.LogWarning("No such effect prefab as " + prefabName);
			return null;
		}
	}
}
