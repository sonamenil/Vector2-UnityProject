using Nekki.Vector.Core;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class RunFPSMeter : UIModule
	{
		[SerializeField]
		private float _UpdateInterval = 0.2f;

		[SerializeField]
		private float _StartTimeGap = 1f;

		[SerializeField]
		private Text _Label;

		private float _UpdateTimeout;

		private int _StartFramesCount;

		private int _LastFramesCount;

		private float _StartTime;

		private float _LastTime;

		public static float FPS { get; private set; }

		public static float FPS_Average { get; private set; }

		public static float FPS_Min { get; private set; }

		public static float FPS_Max { get; private set; }

		public static string FPS_MinRoom { get; private set; }

		public static string FPS_MaxRoom { get; private set; }

		private string CurrentRoomName
		{
			get
			{
				Room currentRoom = RunMainController.Location.CurrentRoom;
				return (currentRoom == null) ? "Outside any room" : currentRoom.UniqueName;
			}
		}

		protected override void Init()
		{
			base.Init();
			FPS_Min = float.MaxValue;
			FPS_Max = float.MinValue;
			string fPS_MinRoom = (FPS_MaxRoom = string.Empty);
			FPS_MinRoom = fPS_MinRoom;
			_StartFramesCount = Time.frameCount;
			_StartTime = Time.realtimeSinceStartup;
			SetTime(_StartTimeGap);
			UpdateVisiblity();
		}

		protected override void Free()
		{
			base.Free();
		}

		private void Update()
		{
			if (RunMainController.Scene != null)
			{
				_UpdateTimeout -= Time.deltaTime;
				if (_UpdateTimeout <= 1E-06f)
				{
					CalculateFps();
				}
			}
		}

		private void SetTime(float p_timeout)
		{
			_UpdateTimeout = p_timeout;
			_LastFramesCount = Time.frameCount;
			_LastTime = Time.realtimeSinceStartup;
		}

		private void CalculateFps()
		{
			int num = Time.frameCount - _LastFramesCount;
			float num2 = Time.realtimeSinceStartup - _LastTime;
			SetTime(_UpdateInterval);
			FPS = (float)num / num2;
			FPS_Average = (float)(Time.frameCount - _StartFramesCount) / (Time.realtimeSinceStartup - _StartTime);
			if (FPS < FPS_Min)
			{
				FPS_Min = FPS;
				FPS_MinRoom = CurrentRoomName;
			}
			if (FPS > FPS_Max)
			{
				FPS_Max = FPS;
				FPS_MaxRoom = CurrentRoomName;
			}
			if (Settings.Visual.ShowAverageFPS)
			{
				_Label.text = string.Format("FPS: {0:F1}\nFPS Average: {1:F1}\nFPS Min: {2:F1}\nFPS Max: {3:F1}", FPS, FPS_Average, FPS_Min, FPS_Max);
			}
			else
			{
				_Label.text = string.Format("FPS: {0:F1}", FPS);
			}
		}

		public void TogleSettings()
		{
			Settings.Visual.ShowFPS = !Settings.Visual.ShowFPS;
			Settings.Save();
			UpdateVisiblity();
			UpdateOptionsUI();
		}

		public void UpdateVisiblity()
		{
			_Label.enabled = !Settings.IsReleaseBuild && Settings.Visual.ShowFPS;
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
