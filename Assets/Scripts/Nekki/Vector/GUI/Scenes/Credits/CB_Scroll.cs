using DG.Tweening;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public class CB_Scroll : CreditsBlock
	{
		private const float _MoveTime = 18f;

		public CB_Scroll(Mapping p_node)
			: base(p_node)
		{
		}

		public override void GenerateContentUI(Transform p_parent, DG.Tweening.Sequence p_sequence)
		{
			GameObject go = CreateGO<CB_Scroll>(p_parent);
			go.name = "CreditsBlock: Scroll";
			float beginY = 0f;
			float p_endY = 0f;
			CalcY(go.GetComponent<RectTransform>(), go.transform.parent.GetComponent<RectTransform>(), ref beginY, ref p_endY);
			go.transform.localPosition = new Vector2(0f, beginY);
			p_sequence.AppendCallback(delegate
			{
				go.SetActive(true);
			});
			p_sequence.Append(go.transform.DOLocalMoveY(p_endY, 18f).SetEase(Ease.Linear));
			p_sequence.AppendCallback(delegate
			{
				go.SetActive(false);
				go.transform.localPosition = new Vector2(0f, beginY);
			});
		}

		private void CalcY(RectTransform p_currentTransform, RectTransform p_parentTransform, ref float p_beginY, ref float p_endY)
		{
			p_beginY = (0f - p_parentTransform.rect.height) * 0.5f - p_currentTransform.rect.height * 0.5f;
			p_endY = p_beginY + p_parentTransform.rect.height + p_currentTransform.rect.height;
		}
	}
}
