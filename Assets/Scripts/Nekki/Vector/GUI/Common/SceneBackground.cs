using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class SceneBackground : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _Center;

		[SerializeField]
		private ResolutionImage _Left;

		[SerializeField]
		private ResolutionImage _Right;

		[SerializeField]
		private string _AtlasName;

		public string AtlasName
		{
			get
			{
				return _AtlasName;
			}
			set
			{
				if (!(_AtlasName == value))
				{
					_AtlasName = value;
					Refresh();
				}
			}
		}

		public void Refresh()
		{
			_Center.SpriteName = _AtlasName + ".c";
			_Left.SpriteName = _AtlasName + ".l";
			_Right.SpriteName = _AtlasName + ".r";
		}
	}
}
