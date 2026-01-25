using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class LockDescriptionController : MonoBehaviour
	{
		private const string _LockDescriptionAlias = "^GUI.Labels.LockedStarterPack^";

		[SerializeField]
		private LabelAlias _LockDescriptionText;

		public void UserSelectStarterPack(StarterPackItem p_item)
		{
			if (p_item == null || p_item.IsBlock)
			{
				base.gameObject.SetActive(true);
				if (p_item != null)
				{
					_LockDescriptionText.SetAlias(p_item.LockText);
				}
				else
				{
					_LockDescriptionText.SetAlias("^GUI.Labels.LockedStarterPack^");
				}
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
