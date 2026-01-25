using UnityEngine;

namespace Nekki.Vector.GUI.Tutorial
{
	public class IdsHelper : MonoBehaviour
	{
		public string Name;

		public int Index { get; set; }

		public string GetId()
		{
			string result = Name + Index;
			Index++;
			return result;
		}
	}
}
