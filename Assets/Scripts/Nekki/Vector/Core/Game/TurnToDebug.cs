using UnityEngine;

namespace Nekki.Vector.Core.Game
{
	public static class TurnToDebug
	{
		private static readonly int[] _UnlockTouchesCount = new int[10] { 1, 2, 3, 4, 3, 3, 4, 2, 2, 3 };

		private static int _CurrentPosition = 0;

		public static void NewTouchesCount(int p_count)
		{
			if (_UnlockTouchesCount[_CurrentPosition] == p_count)
			{
				_CurrentPosition++;
				if (_UnlockTouchesCount.Length == _CurrentPosition)
				{
					Unlock();
				}
			}
			else
			{
				Reset();
			}
		}

		private static void Reset()
		{
			_CurrentPosition = 0;
		}

		private static void Unlock()
		{
			Reset();
			Debug.Log("you did it");
			Settings.IsReleaseBuild = !Settings.IsReleaseBuild;
			Settings.Save();
			Application.Quit();
		}
	}
}
