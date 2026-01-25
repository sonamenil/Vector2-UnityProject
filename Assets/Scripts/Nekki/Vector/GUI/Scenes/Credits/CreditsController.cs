using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public class CreditsController : MonoBehaviour
	{
		private const float _DelayBetweenBlocksDuration = 0.5f;

		private const float _StopMusicDuration = 2f;

		private const float _EndDelayDuration = 2.2f;

		[SerializeField]
		private GameObject _CreditsBlockFadePrefab;

		[SerializeField]
		private GameObject _CreditsBlockScrollPrefab;

		[SerializeField]
		private GameObject _CreditsContentTitlePrefab;

		[SerializeField]
		private GameObject _CreditsContentDataPrefab;

		[SerializeField]
		private GameObject _CreditsContentSpacePrefab;

		[SerializeField]
		private RectTransform _ContentRoot;

		private Dictionary<Type, GameObject> _PrefabsByType = new Dictionary<Type, GameObject>();

		private Sequence _CreditsSequence;

		private static CreditsController _Current;

		public static CreditsController Current
		{
			get
			{
				return _Current;
			}
		}

		public GameObject GetPrefabByType(Type p_type)
		{
			GameObject value = null;
			_PrefabsByType.TryGetValue(p_type, out value);
			return value;
		}

		public void Init()
		{
			_Current = this;
			InitPrefabsByTypeDictionary();
		}

		public void Free()
		{
			_Current = null;
		}

		private void InitPrefabsByTypeDictionary()
		{
			_PrefabsByType.Add(typeof(CB_Fade), _CreditsBlockFadePrefab);
			_PrefabsByType.Add(typeof(CB_Scroll), _CreditsBlockScrollPrefab);
			_PrefabsByType.Add(typeof(CC_Title), _CreditsContentTitlePrefab);
			_PrefabsByType.Add(typeof(CC_Data), _CreditsContentDataPrefab);
			_PrefabsByType.Add(typeof(CC_Space), _CreditsContentSpacePrefab);
		}

		public void GenerateContentUI(List<CreditsBlock> p_creditsContent)
		{
			if (p_creditsContent == null)
			{
				return;
			}
			_CreditsSequence = DOTween.Sequence();
			_CreditsSequence.AppendCallback(delegate
			{
				AudioManager.PlayRandomCreditsMusic();
			});
			foreach (CreditsBlock item in p_creditsContent)
			{
				item.GenerateContentUI(_ContentRoot, _CreditsSequence);
				_CreditsSequence.AppendInterval(0.5f);
			}
			_CreditsSequence.AppendCallback(delegate
			{
				AudioManager.StopMusic(2f);
			});
			_CreditsSequence.AppendInterval(2.2f);
			_CreditsSequence.SetLoops(-1, LoopType.Restart);
			_CreditsSequence.Pause();
		}

		public void PlayAnimation()
		{
			if (_CreditsSequence != null)
			{
				_CreditsSequence.Play();
			}
		}

		public void Clear()
		{
			if (_CreditsSequence == null)
			{
				return;
			}
			_CreditsSequence.Kill(true);
			_CreditsSequence = null;
			foreach (Transform item in _ContentRoot)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
	}
}
