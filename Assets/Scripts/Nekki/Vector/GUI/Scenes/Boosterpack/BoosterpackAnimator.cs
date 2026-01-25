using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Boosterpack
{
	public class BoosterpackAnimator : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _Main;

		[SerializeField]
		private ResolutionImage _MainOpen;

		[SerializeField]
		private ResolutionImage _MainLight;

		[SerializeField]
		private ResolutionImage _MainRightLight;

		[SerializeField]
		private ResolutionImage _MainLeftLight;

		[SerializeField]
		private Image _BigLightspot;

		private Sequence _Shake;

		private Sequence _MainSeq;

		private Sequence _LightSeq;

		public void ToClose()
		{
			_MainOpen.gameObject.SetActive(false);
			_MainLight.gameObject.SetActive(false);
			_MainRightLight.gameObject.SetActive(false);
			_MainLeftLight.gameObject.SetActive(false);
			_BigLightspot.gameObject.SetActive(false);
			_Main.transform.Rotate(default(Vector3));
		}

		public void ToOpen()
		{
			_MainOpen.gameObject.SetActive(true);
			_MainLight.gameObject.SetActive(false);
			_MainRightLight.gameObject.SetActive(false);
			_MainLeftLight.gameObject.SetActive(false);
			_BigLightspot.gameObject.SetActive(false);
			_Main.transform.Rotate(default(Vector3));
		}

		public void RunOpenAnim(BoosterPackLines p_lines, Action p_OnEnd)
		{
			float num = 0.3f;
			float interval = 0.6f;
			_MainRightLight.gameObject.SetActive(true);
			_MainLeftLight.gameObject.SetActive(true);
			_BigLightspot.gameObject.SetActive(true);
			_MainLight.gameObject.SetActive(true);
			_MainRightLight.Alpha = 0f;
			_MainLeftLight.Alpha = 0f;
			_MainLight.Alpha = 0f;
			SetBigLightspotAlpha(0f);
			_MainSeq = DOTween.Sequence();
			_MainSeq.Append(_MainLeftLight.DOFade(1f, num));
			_MainSeq.Join(_MainRightLight.DOFade(1f, num));
			_MainSeq.Join(_MainLight.DOFade(1f, num));
			_MainSeq.AppendCallback(StartLightSeq);
			_MainSeq.AppendInterval(interval);
			_MainSeq.AppendCallback(StopLightSeq);
			_MainSeq.Append(_MainLeftLight.DOFade(0f, num / 2f));
			_MainSeq.Join(_MainRightLight.DOFade(0f, num / 2f));
			_MainSeq.Join(_MainLight.DOFade(0f, num / 2f));
			_MainSeq.Join(DOTween.To(GetBigLightspotAlpha, SetBigLightspotAlpha, 0.45f, 0.1f));
			_MainSeq.AppendCallback(delegate
			{
				p_lines.Play();
				_MainOpen.gameObject.SetActive(true);
			});
			_MainSeq.Join(DOTween.To(GetBigLightspotAlpha, SetBigLightspotAlpha, 0f, 0.1f));
			_MainSeq.AppendCallback(delegate
			{
				_Shake.Kill();
				p_OnEnd();
			});
			_MainSeq.Play();
			_Shake = DOTween.Sequence();
			_Shake.Append(_Main.DOLocalRotate(new Vector3(0f, 0f, -0.5f), 0.05f));
			_Shake.Append(_Main.DOLocalRotate(new Vector3(0f, 0f, 0.5f), 0.05f));
			_Shake.SetLoops(-1);
			_Shake.Play();
		}

		private void StartLightSeq()
		{
			_MainLight.gameObject.SetActive(true);
			_LightSeq = DOTween.Sequence();
			_LightSeq.Append(_MainLeftLight.DOFade(1f, 0.05f));
			_LightSeq.Join(_MainRightLight.DOFade(1f, 0.05f));
			_LightSeq.Append(_MainLeftLight.DOFade(0.7f, 0.05f));
			_LightSeq.Join(_MainRightLight.DOFade(0.7f, 0.05f));
			_LightSeq.SetLoops(-1);
			_LightSeq.Play();
		}

		private void StopLightSeq()
		{
			_LightSeq.Kill();
		}

		private float GetBigLightspotAlpha()
		{
			return _BigLightspot.material.GetFloat("_Intensive");
		}

		private void SetBigLightspotAlpha(float p_value)
		{
			_BigLightspot.material.SetFloat("_Intensive", p_value);
		}
	}
}
