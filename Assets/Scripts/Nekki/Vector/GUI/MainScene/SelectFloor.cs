using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class SelectFloor : MonoBehaviour
	{
		[SerializeField]
		private StarterPackLineUI _Line;

		[SerializeField]
		private PlayButtonController _PlayButton;

		[SerializeField]
		private FloorButtonsController _FloorButtons;

		[SerializeField]
		private LockDescriptionController _LockDescription;

		[SerializeField]
		private StarterPackUIController _StarterPackUIController;

		public void Refresh(bool p_reselectStarterPackToBest = true)
		{
			InitFloorButtons(p_reselectStarterPackToBest);
		}

		private void InitFloorButtons(bool p_reselectStarterPackToBest)
		{
			StarterPacksManager.PrepareStarterPacks();
			if (p_reselectStarterPackToBest || StarterPacksManager.SelectedStarterPack == null)
			{
				StarterPacksManager.SelectBestAvaliableStarterPack();
			}
			_FloorButtons.Init(this, StarterPacksManager.ZoneAvailableStarterPacks);
		}

		public bool UserSelectStarterPack(FloorButton p_item)
		{
			bool flag = false;
			SetLine(p_item);
			flag = _PlayButton.UserSelectStarterPack(p_item);
			_StarterPackUIController.UserSelectStarterPack(p_item.StarterPackItem);
			_LockDescription.UserSelectStarterPack(p_item.StarterPackItem);
			return flag;
		}

		private void SetLine(FloorButton p_item)
		{
			_Line.SetLine(p_item);
		}

		public void UpdateLine()
		{
			_Line.UpdateLine();
		}
	}
}
