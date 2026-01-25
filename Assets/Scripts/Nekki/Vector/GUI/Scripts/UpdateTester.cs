using System.Collections;
using Nekki.Vector.Core;
using UnityEngine;

namespace Nekki.Vector.GUI.Scripts
{
	public class UpdateTester : MonoBehaviour
	{
		[SerializeField]
		public float UpdateInterval = 0.1f;

		private uint _UpdateCount;

		private uint _FixedUpdateCount;

		private uint _UpdateCountRow;

		private uint _FixedUpdateCountRow;

		private bool _IsLastCallUpdate;

		private void Awake()
		{
			_UpdateCount = (_FixedUpdateCount = 0u);
			_UpdateCountRow = (_FixedUpdateCountRow = 0u);
			_IsLastCallUpdate = false;
			StartCoroutine(CalculateStats());
		}

		private IEnumerator CalculateStats()
		{
			while (true)
			{
				yield return new WaitForSeconds(UpdateInterval);
				int delta = (int)(_FixedUpdateCount - _UpdateCount);
				float ratio = (float)_FixedUpdateCount / (float)_UpdateCount;
				VectorLog.RunLog(string.Format("STATS: update: {0}({1}), fixed_update: {2}({3}), fu-u: {4}, fu/u: {5}", _UpdateCount, _UpdateCountRow, _FixedUpdateCount, _FixedUpdateCountRow, delta, ratio));
			}
		}

		private void Update()
		{
			_UpdateCount++;
			if (_IsLastCallUpdate)
			{
				_UpdateCountRow++;
			}
			_IsLastCallUpdate = true;
		}

		private void FixedUpdate()
		{
			_FixedUpdateCount++;
			if (!_IsLastCallUpdate)
			{
				_FixedUpdateCountRow++;
			}
			_IsLastCallUpdate = false;
		}
	}
}
