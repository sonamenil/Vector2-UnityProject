using DG.Tweening;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class ItemRewardUI : MonoBehaviour
	{
		private const int _DefaultSize = 175;

		[SerializeField]
		private CanvasGroup _CanvasGroup;

		[SerializeField]
		private ResolutionImage _Image;

		[SerializeField]
		private int _Size = 175;

		public int Size
		{
			get
			{
				return _Size;
			}
			set
			{
				_Size = value;
				RefreshSize();
			}
		}

		public RectTransform RectTransform
		{
			get
			{
				return GetComponent<RectTransform>();
			}
		}

		public void Init(string p_image)
		{
			ImageResourceFinder.SetImage(_Image, p_image);
		}

		private void RefreshSize()
		{
			RectTransform.sizeDelta = new Vector2(_Size, _Size);
		}

		public void TweenAlpha(bool p_alphadown, float p_duration)
		{
			_CanvasGroup.DOFade((!p_alphadown) ? 1f : 0f, p_duration);
		}
	}
}
