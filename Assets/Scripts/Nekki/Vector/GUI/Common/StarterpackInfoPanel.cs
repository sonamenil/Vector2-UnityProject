using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class StarterpackInfoPanel : MonoBehaviour
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _Description;

		[SerializeField]
		private CanvasGroup _Content;

		private StarterPackItem _Starterpack;

		public void Init(StarterPackItem p_startepack)
		{
			_Starterpack = p_startepack;
			_Title.SetAlias(_Starterpack.VisualName);
			_Description.SetAlias(_Starterpack.Description);
		}

		public Tweener ChangeAlpha(float p_alpha, float p_duration, Ease p_ease)
		{
			return _Content.DOFade(p_alpha, p_duration).SetEase(p_ease);
		}

		public void HideInstantly()
		{
			_Content.alpha = 0f;
			base.gameObject.SetActive(false);
		}
	}
}
