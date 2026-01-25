using Nekki.Vector.Core.Game;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class FPSMeter : UIModule
	{
		[SerializeField]
		private float _UpdateInterval = 0.2f;

		[SerializeField]
		private Text _Label;

		[SerializeField]
		private KeyCode _ToggleKey = KeyCode.F;

		private float _UpdateTimeout;

		private int _LastFramesCount;

		private float _LastTime;

		public static float FPS { get; private set; }

		protected override void Init()
		{
			base.Init();
			UpdateVisiblity();
			SetTime();
		}

		protected override void Free()
		{
			base.Free();
		}

		private void Update()
		{
			_UpdateTimeout -= Time.deltaTime;
			if (_UpdateTimeout <= 1E-06f)
			{
				CalculateFps();
			}
			if (Input.GetKeyDown(_ToggleKey))
			{
				TogleSettings();
			}
		}

		private void SetTime()
		{
			_UpdateTimeout = _UpdateInterval;
			_LastFramesCount = Time.frameCount;
			_LastTime = Time.realtimeSinceStartup;
		}

		private void CalculateFps()
		{
			int num = Time.frameCount - _LastFramesCount;
			float num2 = Time.realtimeSinceStartup - _LastTime;
			SetTime();
			FPS = (float)num / num2;
			_Label.text = string.Format("FPS: {0:F1}", FPS);
		}

		private void TogleSettings()
		{
			Settings.Visual.ShowFPS = !Settings.Visual.ShowFPS;
			Settings.Save();
			UpdateVisiblity();
			UpdateOptionsUI();
		}

		public void UpdateVisiblity()
		{
			_Label.enabled = Settings.Visual.ShowFPS;
		}

		private void UpdateOptionsUI()
		{
			OptionsDialog current = OptionsDialog.Current;
			if (current != null)
			{
				current.UpdateDebugUI();
			}
		}
	}
}
