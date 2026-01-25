using Nekki.Vector.Core.Game;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class DebugPanel : UIModule
	{
		[SerializeField]
		private RunStats _RunInfo;

		[SerializeField]
		private ScrollRect _DebugScrollView;

		[SerializeField]
		private Text _DebugScrollText;

		public bool IsEnabled
		{
			get
			{
				return _RunInfo.gameObject.activeSelf;
			}
			set
			{
				_RunInfo.gameObject.SetActive(value);
			}
		}

		protected override void Init()
		{
			base.Init();
			_RunInfo.Init();
			DebugDataView.Init(_DebugScrollView, _DebugScrollText);
			UpdateVisiblity();
		}

		protected override void Free()
		{
			base.Free();
			_RunInfo.Free();
			DebugDataView.Reset();
		}

		public void UpdateVisiblity()
		{
			IsEnabled = Settings.Visual.ShowDebugGUI;
		}

		private void UpdateOptionsUI()
		{
			OptionsDialog current = OptionsDialog.Current;
			if (current != null)
			{
				current.UpdateDebugUI();
			}
		}

		public void OnRestartButtonClicked()
		{
			Scene<RunScene>.Current.OnButtonRestart(false);
		}

		public void OnPauseButtonClicked()
		{
			Scene<RunScene>.Current.OnButtonPause();
		}

		public void SwitchEnabled()
		{
			Settings.Visual.ShowDebugGUI = !Settings.Visual.ShowDebugGUI;
			Settings.Save();
			UpdateVisiblity();
			UpdateOptionsUI();
		}

		public void OnKeyDown(KeyCode p_code)
		{
			switch (p_code)
			{
			case KeyCode.I:
				SwitchEnabled();
				break;
			case KeyCode.H:
				_RunInfo.LabelChoicesData.gameObject.SetActive(!_RunInfo.LabelChoicesData.gameObject.activeSelf);
				break;
			case KeyCode.J:
				_RunInfo.LabelChoicesData.gameObject.SetActive(!_RunInfo.LabelChoicesData.gameObject.activeSelf);
				Settings.Visual.ShowChoices = _RunInfo.LabelChoicesData.gameObject.activeSelf;
				Settings.Save();
				break;
			}
		}
	}
}
