using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class ParticlesSpawn : MonoBehaviour
	{
		public enum SpawnType
		{
			Manual = 0,
			SpawnOnAwake = 1,
			SpawnOnStart = 2
		}

		[SerializeField]
		private List<GameObject> _ParticlesPrefabs = new List<GameObject>();

		[SerializeField]
		private SpawnType _SpawnType;

		private void Awake()
		{
			if (_SpawnType == SpawnType.SpawnOnAwake)
			{
				Spawn();
			}
		}

		private void Start()
		{
			if (_SpawnType == SpawnType.SpawnOnStart)
			{
				Spawn();
			}
		}

		public void Spawn()
		{
			foreach (GameObject particlesPrefab in _ParticlesPrefabs)
			{
				CreateParticle(particlesPrefab);
			}
		}

		private void CreateParticle(GameObject p_prefab)
		{
			GameObject gameObject = Object.Instantiate(p_prefab);
			gameObject.transform.SetParent(base.transform, false);
		}
	}
}
