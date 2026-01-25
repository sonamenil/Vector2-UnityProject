using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;
using UIFigures;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class PlayButtonController : MonoBehaviour
	{
		[SerializeField]
		private UICircleBorder _Frame;

		[SerializeField]
		private UICircle _Triangle;

		private FloorButton _ActiveItem;

		public void OnPlayButtonClick()
		{
			if (DataLocal.Current.IsPaidVersion || EnergyManager.CurrentLevel > 0)
			{
				if (BoosterpacksManager.BoosterpackQuantity > 0)
				{
					AudioManager.PlaySound("red_button");
					DialogNotificationManager.ShowBoosterpackCanOpenedDialog(0);
				}
				else if (DataLocalHelper.HasLevelUpCards)
				{
					AudioManager.PlaySound("red_button");
					DialogNotificationManager.ShowCardsCanLevelUpDialog(0);
				}
				else
				{
					AudioManager.PlaySound("blue_button");
					Scene<MainScene>.Current.Play();
				}
			}
			else
			{
				AudioManager.PlaySound("red_button");
				DialogNotificationManager.ShowEnergyDialog(OnEnergyDialogClosedAfterRecharge);
			}
		}

		private void OnEnergyDialogClosedAfterRecharge()
		{
			if (BoosterpacksManager.BoosterpackQuantity > 0)
			{
				DialogNotificationManager.ShowBoosterpackCanOpenedDialog(0);
			}
			else if (DataLocalHelper.HasLevelUpCards)
			{
				DialogNotificationManager.ShowCardsCanLevelUpDialog(0);
			}
			else
			{
				Scene<MainScene>.Current.Play();
			}
		}

		public bool UserSelectStarterPack(FloorButton p_item)
		{
			_ActiveItem = p_item;
			bool flag = false;
			if (_ActiveItem.StarterPackItem == null)
			{
				flag = false;
				base.gameObject.SetActive(false);
				return flag;
			}
			if (_ActiveItem.StarterPackItem.IsBlock)
			{
				flag = false;
				base.gameObject.SetActive(false);
			}
			else
			{
				flag = true;
				base.gameObject.SetActive(true);
			}
			StarterPacksManager.SetSelectedStarterPack(p_item.StarterPackItem);
			return flag;
		}

		private void SetNormalColor()
		{
			_Frame.color = new Color(0.255f, 0.761f, 0.89f);
			_Triangle.color = new Color(0.255f, 0.761f, 0.89f);
		}
	}
}
