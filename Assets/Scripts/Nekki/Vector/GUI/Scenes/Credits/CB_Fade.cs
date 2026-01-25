using DG.Tweening;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public class CB_Fade : CreditsBlock
	{
		private const float _ShowDuration = 1.2f;

		private const float _DelayDuration = 5f;

		private const float _HideDuration = 1.2f;

		public CB_Fade(Mapping p_node)
			: base(p_node)
		{
		}

		public override void GenerateContentUI(Transform p_parent, DG.Tweening.Sequence p_sequence)
		{
			GameObject go = CreateGO<CB_Fade>(p_parent);
			go.name = "CreditsBlock: Fade";
			CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
			p_sequence.AppendCallback(delegate
			{
				go.SetActive(true);
			});
			p_sequence.Append(canvasGroup.DOFade(1f, 1.2f).SetEase(Ease.Linear));
			p_sequence.AppendInterval(5f);
			p_sequence.Append(canvasGroup.DOFade(0f, 1.2f).SetEase(Ease.Linear));
			p_sequence.AppendCallback(delegate
			{
				go.SetActive(false);
				canvasGroup.alpha = 0f;
			});
		}
	}
}
