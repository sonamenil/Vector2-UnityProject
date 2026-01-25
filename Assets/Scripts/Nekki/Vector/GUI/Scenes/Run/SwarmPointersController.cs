using System.Collections.Generic;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Runners;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class SwarmPointersController : MonoBehaviour
	{
		[SerializeField]
		private GameObject _PointerPrefab;

		[SerializeField]
		private RectTransform _Root;

		private List<SwarmPointer> _Pointers = new List<SwarmPointer>();

		public void Init(List<Swarm> p_swarms)
		{
			foreach (Swarm p_swarm in p_swarms)
			{
				CreateSwarmPointer(p_swarm);
			}
		}

		private void CreateSwarmPointer(Swarm p_swarm)
		{
			GameObject gameObject = Object.Instantiate(_PointerPrefab);
			gameObject.transform.SetParent(_Root, false);
			SwarmPointer component = gameObject.GetComponent<SwarmPointer>();
			component.Init(p_swarm);
			_Pointers.Add(component);
		}

		public void Refresh()
		{
			if (_Pointers.Count == 0 || !Nekki.Vector.Core.Camera.Camera.IsCurrentExists)
			{
				return;
			}
			Rect p_viewport = Nekki.Vector.Core.Camera.Camera.Current.Viewport;
			foreach (SwarmPointer pointer in _Pointers)
			{
				pointer.Refresh(p_viewport);
			}
		}
	}
}
