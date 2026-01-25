using UnityEngine;

namespace Nekki.Vector.Core.Audio.Internal
{
	public class AudioUnit : MonoBehaviour
	{
		private PlayCommand _Cmd;

		private AudioSource _Source;

		private AudioChannel _Parent;

		private float _StopTime;

		private float _CurStopTime;

		private bool _IsPaused;

		public bool IsPaused
		{
			get
			{
				return _IsPaused;
			}
		}

		public float BaseVolume
		{
			get
			{
				return _Parent.MasterVolume * _Cmd.Volume;
			}
		}

		public bool IsMute
		{
			get
			{
				return _Source.mute;
			}
			set
			{
				_Source.mute = value;
				UpdateDebug(false);
			}
		}

		public bool Idle
		{
			get
			{
				return !_Source.isPlaying && !_IsPaused && !IsMute && !ApplicationController.IsPaused;
			}
		}

		public void Init(AudioChannel p_parent, PlayCommand p_cmd, AudioClip p_clip)
		{
			_Parent = p_parent;
			_Cmd = p_cmd;
			_IsPaused = false;
			base.gameObject.SetActive(true);
			_Source.outputAudioMixerGroup = AudioManager.GetAudioMixerGroup(_Cmd.MixerGroupName);
			_Source.clip = p_clip;
			_Source.loop = _Cmd.Loop;
			_Source.volume = BaseVolume;
			_Source.mute = _Parent.IsMute;
			_Source.Play();
			UpdateDebug(false);
		}

		public void Pause()
		{
			if (!(_Source == null) && !_IsPaused)
			{
				_Source.Pause();
				_IsPaused = true;
				UpdateDebug(false);
			}
		}

		public void UnPause()
		{
			if (!(_Source == null) && _IsPaused)
			{
				_Source.UnPause();
				_IsPaused = false;
				UpdateDebug(false);
			}
		}

		public void Stop(float p_stopTime = 0f)
		{
			if (!Idle)
			{
				if (p_stopTime > 0f)
				{
					_StopTime = (_CurStopTime = p_stopTime);
					return;
				}
				_Source.Stop();
				_Source.clip = null;
				_StopTime = (_CurStopTime = 0f);
				_IsPaused = false;
			}
		}

		private void Awake()
		{
			_Source = base.gameObject.AddComponent<AudioSource>();
			_Source.spatialBlend = 0f;
			Reset();
		}

		private void Update()
		{
			if (_IsPaused)
			{
				return;
			}
			if (Idle)
			{
				Reset();
			}
			else if ((double)_StopTime > 1E-06)
			{
				if ((double)_CurStopTime > 1E-06)
				{
					_CurStopTime -= Time.deltaTime;
					_Source.volume = Mathf.Lerp(BaseVolume, 0f, 1f - _CurStopTime / _StopTime);
				}
				else
				{
					Stop(0f);
				}
			}
			else
			{
				_Source.volume = BaseVolume;
			}
		}

		private void UpdateDebug(bool p_idle = false)
		{
			base.gameObject.name = ((!p_idle) ? string.Format("Channel_{0}{1}: Sound={2}, Volume={3}, Mute={4} Loop={5}", _Parent.ID, (!_IsPaused) ? string.Empty : "[P]", _Cmd.Sound, BaseVolume, _Source.mute ? 1 : 0, _Source.loop ? 1 : 0) : "Idle");
		}

		private void Reset()
		{
			_Source.Stop();
			_Source.clip = null;
			_StopTime = (_CurStopTime = 0f);
			_IsPaused = false;
			if (_Parent != null)
			{
				_Parent.FreeUnit(this);
			}
			UpdateDebug(true);
			base.gameObject.SetActive(false);
		}
	}
}
