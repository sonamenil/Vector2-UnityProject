using System.Collections.Generic;
using Nekki.Vector.Core.Game;
using UnityEngine;

namespace Nekki.Vector.GUI.Scripts
{
	public class OnlyMobileElements : MonoBehaviour
	{
		public List<GameObject> Elements = new List<GameObject>();

		private void Awake()
		{
			foreach (GameObject element in Elements)
			{
				element.SetActive(!Settings.IsReleaseBuild);
			}
		}
	}
}
