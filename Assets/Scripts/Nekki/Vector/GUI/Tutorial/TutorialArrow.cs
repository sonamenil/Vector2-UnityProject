using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TutorialArrow : MonoBehaviour
	{
		public Vector2 DefaultPosition = new Vector2(0f, 0f);

		public Vector2 Position = new Vector2(-200f, 0f);

		public Vector2 Delta = new Vector2(-20f, 0f);

		public float Angle;

		public float Duration = 1f;

		public float Delay;

		private void Awake()
		{
			Reset();
		}

		public void Activate()
		{
			Reset();
			if (Delay > 1E-05f)
			{
				CoroutineManager.Current.StartRoutine(DelayActivate(Delay));
			}
			else
			{
				InternalActivate();
			}
		}

		private IEnumerator DelayActivate(float p_delay)
		{
			yield return new WaitForSeconds(p_delay);
			InternalActivate();
		}

		private void InternalActivate()
		{
			base.transform.position = DefaultPosition;
			base.transform.localPosition = (Vector2)base.transform.localPosition + Position;
			base.transform.localEulerAngles = new Vector3(0f, 0f, Angle);
			Vector2 vector = new Vector2(base.transform.localPosition.x + Delta.x, base.transform.localPosition.y + Delta.y);
			base.transform.DOLocalMove(vector, Duration).SetLoops(-1, LoopType.Yoyo);
			base.gameObject.SetActive(true);
		}

		public void Reset()
		{
			DOTween.Kill(base.transform);
			base.gameObject.SetActive(false);
		}
	}
}
