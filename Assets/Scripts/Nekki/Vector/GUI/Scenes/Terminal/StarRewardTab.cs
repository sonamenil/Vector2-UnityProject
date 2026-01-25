using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class StarRewardTab : MonoBehaviour
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _Description;

		[SerializeField]
		private GridLayoutGroup _StarsLayout;

		[SerializeField]
		private GameObject _StarPrefab;

		[SerializeField]
		private ResolutionImage _Icon;

		[SerializeField]
		private ResolutionImage _SingleRewardIcon;

		[SerializeField]
		private ResolutionImage _SingleRewardArt;

		[SerializeField]
		private CanvasGroup _Canvas;

		private StarReward _StarReward;

		private Action<StarRewardTab> _OnTap;

		private List<StarIcon> _Stars = new List<StarIcon>();

		public float Alpha
		{
			get
			{
				return _Canvas.alpha;
			}
			set
			{
				_Canvas.alpha = value;
			}
		}

		public StarReward StarReward
		{
			get
			{
				return _StarReward;
			}
		}

		public void Init(StarReward p_reward, Action<StarRewardTab> p_onTap)
		{
			_OnTap = p_onTap;
			_StarReward = p_reward;
			_Description.SetAlias(_StarReward.Description);
			_Title.SetAlias(_StarReward.Title);
			_Icon.SpriteName = _StarReward.IconName;
			_Icon.SetNativeSize();
			SetIconColor(_Icon, p_reward.Rarity);
			SetSingleRewardIconAndArt();
			SetIconColor(_SingleRewardIcon, p_reward.Rarity);
			_Icon.Alpha = 0.35f;
			_SingleRewardIcon.Alpha = 0.35f;
			Alpha = 0.1f;
			InitStars();
		}

		private void InitStars()
		{
			for (int i = 0; i < _StarReward.StarsCount; i++)
			{
				StarIcon component = UnityEngine.Object.Instantiate(_StarPrefab).GetComponent<StarIcon>();
				component.transform.SetParent(_StarsLayout.transform, false);
				component.Init(_StarReward.Rarity);
				_Stars.Add(component);
			}
		}

		public void MakeTabVisible()
		{
			_Canvas.DOFade(1f, 0.4f);
		}

		private void SetIconColor(ResolutionImage p_icon, int p_rarity)
		{
			switch (p_rarity)
			{
			case 2:
				p_icon.color = ColorUtils.FromHex("DA660E");
				break;
			case 3:
				p_icon.color = ColorUtils.FromHex("C11527");
				break;
			}
		}

		private void SetIconOpacity(float p_percent)
		{
			_Icon.Alpha = ((p_percent == 1f) ? 1f : 0.35f);
		}

		private void SetSingleIconOpacity(float p_percent)
		{
			if (_StarReward.IsSingle)
			{
				_SingleRewardIcon.Alpha = ((p_percent == 1f) ? 1f : 0.35f);
			}
		}

		private void SetSingleRewardSkipped(float p_percent)
		{
			if (_StarReward.IsSingle)
			{
				if (p_percent != 1f)
				{
					_SingleRewardIcon.Alpha = 0.35f;
					return;
				}
				_SingleRewardIcon.gameObject.SetActive(false);
				_SingleRewardArt.Alpha = 1f;
			}
		}

		private void SetSingleRewardIconAndArt()
		{
			if (_StarReward.SingleRewardIconName == null)
			{
				_SingleRewardIcon.gameObject.SetActive(false);
				_SingleRewardArt.gameObject.SetActive(false);
				return;
			}
			_SingleRewardIcon.SpriteName = _StarReward.SingleRewardIconName;
			_SingleRewardIcon.gameObject.SetActive(true);
			_SingleRewardArt.SpriteName = _StarReward.SingleRewardArtName;
			_SingleRewardArt.Alpha = 0f;
		}

		public void OnTap()
		{
			if (_OnTap != null)
			{
				_OnTap(this);
			}
		}

		public void FillStarsSeq(float p_duration, int p_stars)
		{
			Sequence sequence = DOTween.Sequence();
			float num = 0f;
			float num2 = 0.7f * p_duration;
			for (int i = 0; i < _Stars.Count; i++)
			{
				int star_index = i;
				int num3 = p_stars - i * _StarReward.RarityModifier;
				float percent = ((num3 >= _StarReward.RarityModifier) ? 1f : ((float)num3 / (float)_StarReward.RarityModifier));
				_Stars[star_index].FillDuration = num2 / (float)_Stars.Count;
				sequence.AppendCallback(delegate
				{
					_Stars[star_index].SetCompleteStarIcon(percent);
				});
				sequence.AppendInterval(num2 / (float)_Stars.Count);
				num = percent;
			}
			SetIconOpacity(num);
			SetSingleIconOpacity(num);
			sequence.AppendCallback(delegate
			{
				SingleRewardArtSequence(p_duration, p_stars);
			});
			sequence.Play();
		}

		public void SingleRewardArtSequence(float p_duration, int p_stars)
		{
			if (StarReward.IsSingle && p_stars >= StarReward.Cost)
			{
				Sequence sequence = DOTween.Sequence();
				RectTransform component = _SingleRewardArt.gameObject.GetComponent<RectTransform>();
				sequence.Append(_SingleRewardIcon.DOFade(0f, p_duration / 8f));
				sequence.Join(_SingleRewardArt.DOFade(1f, p_duration / 8f));
				sequence.Join(component.DOScale(new Vector3(1.05f, 1.05f, 1f), p_duration / 8f));
				sequence.Append(component.DOScale(new Vector3(0.95f, 0.95f, 1f), p_duration / 8f));
				sequence.Play();
			}
		}

		public void SkipTabSequences(int p_stars)
		{
			float num = 0f;
			for (int i = 0; i < _Stars.Count; i++)
			{
				int num2 = p_stars - i * _StarReward.RarityModifier;
				num = ((num2 >= _StarReward.RarityModifier) ? 1f : ((float)num2 / (float)_StarReward.RarityModifier));
				_Stars[i].SetCompleteStarIcon(num, true);
			}
			SetSingleRewardSkipped(num);
			SetIconOpacity(num);
		}
	}
}
