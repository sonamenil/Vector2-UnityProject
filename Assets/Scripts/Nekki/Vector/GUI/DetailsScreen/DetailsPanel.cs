using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.MainScene;
using UnityEngine;

namespace Nekki.Vector.GUI.DetailsScreen
{
	public class DetailsPanel : UIModule
	{
		[SerializeField]
		private StarterPackUIController _StarterpackUI;

		[SerializeField]
		private GadgetUIPanel _GadgetPanel;

		[SerializeField]
		private InfoPanel _InfoPanel;

		protected override void Init()
		{
			base.Init();
			_InfoPanel.Init();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			StarterPackItem selectedStarterPack = StarterPacksManager.SelectedStarterPack;
			_StarterpackUI.UserSelectStarterPack(selectedStarterPack);
			_GadgetPanel.Init(selectedStarterPack.Gadgets, OnGadgetTap, false);
			_InfoPanel.Reset();
			_InfoPanel.ShowStarterpackInfo(selectedStarterPack);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
		}

		private void OnGadgetTap(GadgetUI p_gadget, bool p_instant)
		{
			if (p_gadget != null)
			{
				_InfoPanel.ShowGadgetInfo(p_gadget.Gadget);
			}
			else
			{
				_InfoPanel.ShowCurrentStarterpackInfo();
			}
		}

		public void OnStarterpackTap()
		{
			AudioManager.PlaySound("select_button");
			_GadgetPanel.ClearSelection();
			_InfoPanel.ShowCurrentStarterpackInfo();
		}
	}
}
