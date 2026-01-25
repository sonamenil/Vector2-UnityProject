using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class LoadingCircle : MonoBehaviour
	{
		[SerializeField]
		private int _SegmentsCount = 12;

		[SerializeField]
		private float _Timeout = 0.1f;

		private float _CurrentTime;

		private Vector3 _Step;

		public bool IsPlaying
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		public void Play()
		{
			_CurrentTime = _Timeout;
			_Step = new Vector3(0f, 0f, -360f / (float)_SegmentsCount);
			base.transform.localEulerAngles = Vector3.zero;
			base.gameObject.SetActive(true);
		}

		public void Stop()
		{
			base.gameObject.SetActive(false);
		}

		private void Awake()
		{
			Stop();
		}

		private void Update()
		{
			if (_CurrentTime > 1E-06f)
			{
				_CurrentTime -= Time.deltaTime;
				return;
			}
			_CurrentTime = _Timeout;
			base.transform.localEulerAngles += _Step;
		}
	}
}
