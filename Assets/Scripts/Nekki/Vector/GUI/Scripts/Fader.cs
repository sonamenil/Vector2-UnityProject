using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scripts
{
	public class Fader : MonoBehaviour
	{
		private static Fader _Instance;

		[SerializeField]
		private Image _FadeImage;

		private Vector4 _FadeColor;

		private float _FadeAlpha;

		private Action _Callback;

		private int _FadeDir;

		private float _FadeTime;

		private float _Timeout;

		private bool _IsFading;

		private bool _IsPaused;

		public static Fader Instance
		{
			get
			{
				if (_Instance == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefab/Fader"));
					gameObject.name = "[Fader]";
					_Instance = gameObject.GetComponent<Fader>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
				return _Instance;
			}
		}

		public bool IsFading
		{
			get
			{
				return _IsFading;
			}
		}

		public bool IsPaused
		{
			get
			{
				return _IsPaused;
			}
		}

		public void FadeIn(float p_time)
		{
			FadeIn(p_time, Color.black, null);
		}

		public void FadeIn(float p_time, Color p_color)
		{
			FadeIn(p_time, p_color, null);
		}

		public void FadeIn(float p_time, Action p_action = null)
		{
			FadeIn(p_time, Color.black, p_action);
		}

		public void FadeIn(float p_time, Color p_color, Action p_action)
		{
			if (!_IsFading)
			{
				_FadeColor = new Vector4(p_color.r, p_color.g, p_color.b, p_color.a);
				_FadeAlpha = 0f;
				_Callback = p_action;
				_FadeDir = -(int)(_FadeColor[3] * 255f);
				_FadeTime = p_time;
				_Timeout = _FadeTime;
				_IsPaused = false;
				_IsFading = true;
			}
		}

		public void FadeOut(float p_time, Action p_action = null)
		{
			if (!_IsFading)
			{
				_FadeAlpha = _FadeColor[3];
				_Callback = p_action;
				_FadeDir = -(int)(_FadeColor[3] * 255f);
				_FadeTime = p_time;
				_Timeout = _FadeTime;
				_IsPaused = false;
				_IsFading = true;
			}
		}

		public void Pause()
		{
			if (_IsFading)
			{
				_IsPaused = true;
			}
		}

		public void Resume()
		{
			if (_IsFading && _IsPaused)
			{
				_IsPaused = false;
			}
		}

		private void Awake()
		{
			Color color = _FadeImage.color;
			_FadeImage.color = new Color(color.r, color.g, color.b, 0f);
		}

		private void Update()
		{
			if (_IsFading && !_IsPaused)
			{
				float deltaTime = Time.deltaTime;
				_Timeout -= deltaTime;
				_FadeAlpha += deltaTime / _FadeTime * (float)_FadeDir;
				_FadeImage.color = new Color(_FadeColor.x, _FadeColor.y, _FadeColor.z, _FadeAlpha);
				if (_Timeout < 0f)
				{
					OnFadeFinish();
				}
			}
		}

		private void OnFadeFinish()
		{
			_IsFading = false;
			_IsPaused = false;
			if (_Callback != null)
			{
				_Callback();
			}
		}
	}
}
