using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.ArchiveCategory
{
	public class CategoryButtonsPanel : MonoBehaviour
	{
		[SerializeField]
		private CategoryButton _HeadButton;

		[SerializeField]
		private CategoryButton _TorsoButton;

		[SerializeField]
		private CategoryButton _HandsButton;

		[SerializeField]
		private CategoryButton _LegsButton;

		[SerializeField]
		private CategoryButton _BeltButton;

		[SerializeField]
		private CategoryButton _StuntsButton;

		[SerializeField]
		private CategoryButton _StoryButton;

		[SerializeField]
		private CategoryButton _NotesButton;

		[SerializeField]
		private CategoryButton _CreditsButton;

		public void Init()
		{
			InitButtons();
		}

		private void InitButtons()
		{
			InitButton(_HeadButton, SlotItem.Slot.Head);
			InitButton(_TorsoButton, SlotItem.Slot.Torso);
			InitButton(_HandsButton, SlotItem.Slot.Hands);
			InitButton(_LegsButton, SlotItem.Slot.Legs);
			InitButton(_BeltButton, SlotItem.Slot.Belt);
			InitButton(_StuntsButton, SlotItem.Slot.Stunts);
			InitButton(_StoryButton, SlotItem.Slot.StoryItems);
			InitButton(_NotesButton, SlotItem.Slot.Notes);
			InitButton(_CreditsButton);
		}

		private void InitButton(CategoryButton p_button, SlotItem.Slot p_slotType = SlotItem.Slot.NotSlot)
		{
			p_button.Init(p_slotType, OnCategoryButtonClicked);
		}

		private void OnCategoryButtonClicked(CategoryButton p_categoryButton)
		{
			AudioManager.PlaySound("blue_button");
			if (p_categoryButton.SlotType != SlotItem.Slot.NotSlot)
			{
				Manager.OpenArchive(p_categoryButton.SlotType);
			}
			else
			{
				Manager.OpenCredits();
			}
		}
	}
}
