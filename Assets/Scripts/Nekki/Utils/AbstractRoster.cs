using CodeStage.AntiCheat.ObscuredTypes;

namespace Nekki.Utils
{
	public abstract class AbstractRoster
	{
		protected static string FileNameDef = string.Empty;

		protected static string FileNameSave = string.Empty;

		protected static string FileNameBkp = string.Empty;

		protected int _timeToSave;

		protected int _timeLeftToSave;

		private bool _needSave;

		public virtual ObscuredDouble Money { get; set; }

		public virtual ObscuredDouble Experience { get; set; }

		protected AbstractRoster()
		{
			GlobalTimer.Instnce.addEventListener(0, TimerTick);
		}

		protected void FreeTimer()
		{
			GlobalTimer.Instnce.removeEventListener(0, TimerTick);
		}

		public void Save(bool force = false)
		{
			if (force)
			{
				SaveToFile();
				_needSave = false;
			}
			else
			{
				_needSave = true;
			}
		}

		protected abstract void SaveToFile();

		private void TimerTick(ExtentionBehaviour.CallEventArgs callEventArgs)
		{
			_timeLeftToSave--;
			if (_needSave && _timeLeftToSave <= 0)
			{
				SaveToFile();
				_needSave = false;
				_timeLeftToSave = _timeToSave;
			}
		}
	}
}
