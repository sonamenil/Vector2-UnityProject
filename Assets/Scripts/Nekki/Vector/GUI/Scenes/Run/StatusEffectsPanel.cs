using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class StatusEffectsPanel : MonoBehaviour
	{
		private class StatusEffectData
		{
			public StatusEffect Effect;

			public Sequence FadeSequence;

			public string EffectName;
		}

		private const float _FadeTime = 1f;

		[SerializeField]
		private GameObject _StatusEffectPrefab;

		private List<StatusEffectData> _Statuses = new List<StatusEffectData>();

		private void Start()
		{
			RunMainController.OnPause += OnPause;
		}

		private void OnDestroy()
		{
			RunMainController.OnPause -= OnPause;
		}

		private void OnPause(bool p_state)
		{
			for (int i = 0; i < _Statuses.Count; i++)
			{
				if (_Statuses[i].FadeSequence != null)
				{
					if (p_state)
					{
						_Statuses[i].FadeSequence.Pause();
					}
					else
					{
						_Statuses[i].FadeSequence.Play();
					}
				}
			}
		}

		public void Render()
		{
			for (int i = 0; i < _Statuses.Count; i++)
			{
				if (_Statuses[i].FadeSequence == null)
				{
					if (!_Statuses[i].Effect.IsOver)
					{
						_Statuses[i].Effect.Render();
					}
					else if (!_Statuses[i].Effect.IsPermantent)
					{
						CreateFadeSequence(_Statuses[i]);
					}
				}
			}
		}

		public void AddStatusEffect(string p_name, string p_image, string p_color, int p_delayFrames, int p_quantity, string p_actionLength, string p_renewType, string p_stackType, string p_counterName, int p_counterValue)
		{
			StatusEffect statusEffect = null;
			StatusEffect.FillSpriteType p_fillSpriteType = StatusEffect.FillSpriteType.None;
			if (p_actionLength == "Timed")
			{
				p_fillSpriteType = StatusEffect.FillSpriteType.Normal;
			}
			else if (p_actionLength == "TimedReversed")
			{
				p_fillSpriteType = StatusEffect.FillSpriteType.Reversed;
			}
			bool p_isPermantent = p_actionLength == "Persistent" || p_actionLength == "TimedReversed";
			if (ConteinsStatusEffect(p_name) && p_renewType != "Queue")
			{
				switch (p_renewType)
				{
				case "Refresh":
				{
					StatusEffectData statusEffectData2 = GetStatusEffectData(p_name);
					statusEffect = statusEffectData2.Effect;
					statusEffect.Init(p_image, p_color, (!(p_actionLength == "Persistent")) ? (p_delayFrames + statusEffect.Frames) : (-1), p_fillSpriteType, p_isPermantent);
					statusEffect.SetQuantity(p_quantity, p_stackType);
					statusEffect.ResetFillAmount();
					statusEffect.CounterName = p_counterName;
					statusEffect.CounterValue = p_counterValue;
					statusEffect.ResetCounterValueOnNextFrame = p_actionLength == "OneTime";
					statusEffect.ResetCounterValueOnLastFrame = p_actionLength == "TimedReversed";
					if (statusEffectData2.FadeSequence != null)
					{
						statusEffectData2.FadeSequence.Kill();
						statusEffectData2.FadeSequence = null;
					}
					statusEffect.Pulse();
					return;
				}
				case "AddTime":
				{
					StatusEffectData statusEffectData = GetStatusEffectData(p_name);
					statusEffect = statusEffectData.Effect;
					statusEffect.Init(p_image, p_color, p_delayFrames + statusEffect.DelayFrames, p_fillSpriteType, p_isPermantent);
					statusEffect.SetQuantity(p_quantity, p_stackType);
					statusEffect.CounterName = p_counterName;
					statusEffect.CounterValue = p_counterValue;
					statusEffect.ResetCounterValueOnNextFrame = p_actionLength == "OneTime";
					statusEffect.ResetCounterValueOnLastFrame = p_actionLength == "TimedReversed";
					if (statusEffectData.FadeSequence != null)
					{
						statusEffectData.FadeSequence.Kill();
						statusEffectData.FadeSequence = null;
					}
					return;
				}
				}
			}
			if (statusEffect == null)
			{
				statusEffect = Object.Instantiate(_StatusEffectPrefab).GetComponent<StatusEffect>();
				statusEffect.transform.SetParent(base.transform, false);
				statusEffect.Init(p_image, p_color, (!(p_actionLength == "Persistent")) ? p_delayFrames : (-1), p_fillSpriteType, p_isPermantent);
				statusEffect.SetQuantity(p_quantity);
				statusEffect.gameObject.SetActive(false);
				statusEffect.gameObject.SetActive(true);
				statusEffect.CounterName = p_counterName;
				statusEffect.CounterValue = p_counterValue;
				statusEffect.ResetCounterValueOnNextFrame = p_actionLength == "OneTime";
				statusEffect.ResetCounterValueOnLastFrame = p_actionLength == "TimedReversed";
			}
			_Statuses.Add(new StatusEffectData
			{
				EffectName = p_name,
				Effect = statusEffect,
				FadeSequence = null
			});
		}

		public void KillStatusEffect(string p_name)
		{
			StatusEffectData statusEffectData = GetStatusEffectData(p_name);
			if (statusEffectData != null)
			{
				CreateFadeSequence(statusEffectData);
			}
		}

		public int CounterAppendValue(string p_name)
		{
			int num = 0;
			for (int i = 0; i < _Statuses.Count; i++)
			{
				if (_Statuses[i].Effect.CounterName == p_name)
				{
					num += _Statuses[i].Effect.CounterValue * _Statuses[i].Effect.Quantity;
				}
			}
			return num;
		}

		private void CreateFadeSequence(StatusEffectData p_effectData)
		{
			if (p_effectData.FadeSequence == null)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.Append(p_effectData.Effect.GroupCanvas.DOFade(0f, 1f));
				sequence.AppendCallback(delegate
				{
					Object.Destroy(p_effectData.Effect.gameObject);
					DeleteStatusEffectData(p_effectData.Effect);
				});
				sequence.Append(p_effectData.Effect.ElementLayout.DOMinSize(default(Vector2), 0.3f));
				sequence.OnKill(delegate
				{
					p_effectData.Effect.GroupCanvas.alpha = 1f;
				});
				sequence.Play();
				p_effectData.FadeSequence = sequence;
			}
		}

		private void DeleteStatusEffectData(StatusEffect p_effect)
		{
			for (int i = 0; i < _Statuses.Count; i++)
			{
				if (_Statuses[i].Effect == p_effect)
				{
					_Statuses.RemoveAt(i);
					break;
				}
			}
		}

		private bool ConteinsStatusEffect(string p_name)
		{
			for (int i = 0; i < _Statuses.Count; i++)
			{
				if (_Statuses[i].EffectName == p_name)
				{
					return true;
				}
			}
			return false;
		}

		private StatusEffectData GetStatusEffectData(string p_name)
		{
			for (int i = 0; i < _Statuses.Count; i++)
			{
				if (_Statuses[i].EffectName == p_name)
				{
					return _Statuses[i];
				}
			}
			return null;
		}
	}
}
