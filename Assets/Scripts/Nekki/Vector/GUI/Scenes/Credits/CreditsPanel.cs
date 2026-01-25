using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public class CreditsPanel : UIModule
	{
		[SerializeField]
		private CreditsController _CreditsController;

		protected override void Init()
		{
			base.Init();
			_CreditsController.Init();
		}

		protected override void Free()
		{
			base.Free();
			_CreditsController.Free();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			Refresh();
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			_CreditsController.Clear();
		}

		public void Refresh()
		{
			_CreditsController.Clear();
			_CreditsController.GenerateContentUI(CreditsParser.GetCreditsContent());
		}

		public void PlayAnimation()
		{
			_CreditsController.PlayAnimation();
		}
	}
}
