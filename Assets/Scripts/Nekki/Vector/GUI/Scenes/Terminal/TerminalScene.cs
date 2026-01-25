using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class TerminalScene : Scene<TerminalScene>
	{
		[SerializeField]
		private SceneBackground _Background;

		private TerminalPanel _TerminalPanel;

		private StarRewardsPanel _StarRewardsPanel;

		private bool _IsGameRestoreActive;

		public override SceneKind SceneId
		{
			get
			{
				return SceneKind.Terminal;
			}
		}

		public bool IsGameRestoreActive
		{
			get
			{
				return _IsGameRestoreActive;
			}
			set
			{
				_IsGameRestoreActive = value;
			}
		}

		protected override void Init()
		{
			base.Init();
			_TerminalPanel = GetModule<TerminalPanel>();
			_StarRewardsPanel = GetModule<StarRewardsPanel>();
			_TerminalPanel.GenerateItems();
			if ((int)CounterController.Current.CounterMissionsBlock == 0 && (int)CounterController.Current.CounterTutorialMissionsStars != -1)
			{
				_StarRewardsPanel.Activate();
			}
			else
			{
				_TerminalPanel.Activate();
			}
			RefreshBG();
			AudioManager.PlayRandomMenuMusic();
			DebugUtils.SceneLoadStopTimer();
			QuestManager.Current.CheckEvent(TQE_OnScreen.ScreenTerminalEvent);
		}

		protected override void Free()
		{
			base.Free();
		}

		public void RefreshBG()
		{
			_Background.AtlasName = Manager.ZoneBackground;
		}

		public void Exit()
		{
			StarsManager.GetAllRewards();
			StarsManager.ClearMissionStars();
			_TerminalPanel.Exit();
		}

		public void Regenerate()
		{
			_TerminalPanel.RerollAllCards();
		}

		public void OnKeyDown(KeyCode p_code)
		{
			if (p_code == KeyCode.R)
			{
				if (!Core.Game.Settings.IsReleaseBuild)
				{
					Regenerate();
				}
			}
		}

		public void ContinueTap()
		{
			if (_StarRewardsPanel.IsActive)
			{
				if (_StarRewardsPanel.TappedFirstTime)
				{
					_StarRewardsPanel.DeActivate();
					_TerminalPanel.Activate();
				}
				else
				{
					_StarRewardsPanel.SkipSequence();
				}
			}
			else
			{
				Exit();
			}
		}
	}
}
