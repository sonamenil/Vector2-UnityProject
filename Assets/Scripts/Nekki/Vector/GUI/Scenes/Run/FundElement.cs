using System;
using System.Collections;
using Nekki.Vector.Core.Localization;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class FundElement : MonoBehaviour
	{
		private enum State
		{
			FadeIn = 1,
			Visible = 2,
			FadeOut = 3,
			NotVisible = 4
		}

		private const float FadeInTime = 0.3f;

		private const float FadeOutTime = 2f;

		private const float VisibilityTime = 2f;

		private const float MoveTime = 0.5f;

		[SerializeField]
		private LabelAlias _AmountLable;

		[SerializeField]
		private CanvasRenderer _ImageRenderer;

		[SerializeField]
		private CanvasRenderer _TextRenderer;

		private bool isFadeIn;

		private bool isFadeOut;

		private bool isVisible;

		private bool isMoving;

		private float _Time;

		private State _State;

		private Coroutine _FadeIn;

		private Coroutine _FadeOut;

		private Coroutine _KeepVisible;

		private Coroutine _MoveCoroutine;

		public int Index { get; set; }

		public bool IsShowing
		{
			get
			{
				return true;
			}
		}

		public event Action<FundElement> Hiden;

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		private void Start()
		{
			_State = State.NotVisible;
			_ImageRenderer.SetAlpha(0f);
			_TextRenderer.SetAlpha(0f);
		}

		public void AddFunds(int amount)
		{
			_AmountLable.SetAlias(amount.ToString());
			switch (_State)
			{
			case State.Visible:
				_Time = 0f;
				break;
			case State.FadeIn:
				break;
			case State.FadeOut:
				_Time = 0.3f - _Time;
				_State = State.FadeIn;
				StopCoroutine(_FadeOut);
				_FadeIn = StartCoroutine(FadeIn());
				break;
			case State.NotVisible:
				_Time = 0f;
				_State = State.FadeIn;
				_FadeIn = StartCoroutine(FadeIn());
				break;
			}
		}

		private void TimerEnd()
		{
			switch (_State)
			{
			case State.Visible:
				_Time = 0f;
				_State = State.FadeOut;
				_FadeOut = StartCoroutine(FadeOut());
				break;
			case State.FadeIn:
				_Time = 0f;
				_State = State.Visible;
				_KeepVisible = StartCoroutine(KeepVisible());
				break;
			case State.FadeOut:
				_State = State.NotVisible;
				if (this.Hiden != null)
				{
					this.Hiden(this);
				}
				break;
			case State.NotVisible:
				break;
			}
		}

		public void SetPosition(Vector3 position)
		{
			if (base.transform != null)
			{
				_MoveCoroutine = StartCoroutine(MoveTo(position));
			}
		}

		private IEnumerator FadeOut()
		{
			while (_Time < 2f)
			{
				_Time += Time.deltaTime;
				float alpha = Mathf.Lerp(1f, 0f, _Time / 2f);
				_ImageRenderer.SetAlpha(alpha);
				_TextRenderer.SetAlpha(alpha);
				yield return null;
			}
			if (_FadeOut != null)
			{
				StopCoroutine(_FadeOut);
			}
			TimerEnd();
		}

		private IEnumerator FadeIn()
		{
			while (_Time < 0.3f)
			{
				_Time += Time.deltaTime;
				float alpha = Mathf.Lerp(0f, 1f, _Time / 0.3f);
				_ImageRenderer.SetAlpha(alpha);
				_TextRenderer.SetAlpha(alpha);
				yield return null;
			}
			if (_FadeIn != null)
			{
				StopCoroutine(_FadeIn);
			}
			TimerEnd();
		}

		private IEnumerator KeepVisible()
		{
			while (_Time < 2f)
			{
				_Time += Time.deltaTime;
				yield return null;
			}
			if (_KeepVisible != null)
			{
				StopCoroutine(_KeepVisible);
			}
			TimerEnd();
		}

		private IEnumerator MoveTo(Vector3 position)
		{
			float i = 0f;
			while (i < 0.5f)
			{
				i += Time.deltaTime;
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, position, i);
				yield return null;
			}
			if (_MoveCoroutine != null)
			{
				StopCoroutine(_MoveCoroutine);
			}
		}

		public void Show(int amount)
		{
			_AmountLable.SetAlias(amount.ToString());
			_ImageRenderer.SetAlpha(1f);
			_TextRenderer.SetAlpha(1f);
			_State = State.Visible;
			if (_KeepVisible != null)
			{
				StopCoroutine(_KeepVisible);
			}
			if (_FadeIn != null)
			{
				StopCoroutine(_FadeIn);
			}
			if (_FadeOut != null)
			{
				StopCoroutine(_FadeOut);
			}
		}

		public void Hide()
		{
			_ImageRenderer.SetAlpha(0f);
			_TextRenderer.SetAlpha(0f);
			if (this.Hiden != null)
			{
				this.Hiden(this);
			}
			_State = State.NotVisible;
		}
	}
}
