using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class MissionRewardElementUI : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _Image;

		public void Init(bool p_completed = true)
		{
			_Image.SpriteName = ((!p_completed) ? "common.star_empty" : "common.star_reward");
		}
	}
}
