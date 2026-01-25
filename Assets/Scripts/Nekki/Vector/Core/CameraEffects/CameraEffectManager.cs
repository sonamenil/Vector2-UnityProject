using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.CameraEffects
{
	public class CameraEffectManager : MonoBehaviour
	{
		private static CameraEffectManager _Instance;

		private readonly Dictionary<CameraEffectType, CameraEffectBase> _Effects = new Dictionary<CameraEffectType, CameraEffectBase>();

		public static void Show(CameraEffectType p_type, int p_frames = -1)
		{
			if ((bool)_Instance && _Instance._Effects.ContainsKey(p_type))
			{
				_Instance._Effects[p_type].Show(p_frames);
			}
		}

		public static void Hide(CameraEffectType p_type)
		{
			if ((bool)_Instance && _Instance._Effects.ContainsKey(p_type))
			{
				_Instance._Effects[p_type].Hide();
			}
		}

		public static void Toggle(CameraEffectType p_type)
		{
			if ((bool)_Instance && _Instance._Effects.ContainsKey(p_type))
			{
				_Instance._Effects[p_type].Toggle();
			}
		}

		public static bool IsEffectActive(CameraEffectType p_type)
		{
			if ((bool)_Instance && _Instance._Effects.ContainsKey(p_type))
			{
				return _Instance._Effects[p_type].enabled;
			}
			return false;
		}

		public static CameraEffectBase GetEffect(CameraEffectType p_type)
		{
			if ((bool)_Instance && _Instance._Effects.ContainsKey(p_type))
			{
				return _Instance._Effects[p_type];
			}
			return null;
		}

		public static void Clear()
		{
			if (_Instance == null)
			{
				return;
			}
			foreach (CameraEffectBase value in _Instance._Effects.Values)
			{
				value.Hide();
			}
		}

		private void Awake()
		{
			if (_Instance == null)
			{
				_Instance = this;
				CameraEffectBase[] components = GetComponents<CameraEffectBase>();
				CameraEffectBase[] array = components;
				foreach (CameraEffectBase cameraEffectBase in array)
				{
					if (!_Effects.ContainsKey(cameraEffectBase.Type))
					{
						_Effects.Add(cameraEffectBase.Type, cameraEffectBase);
					}
					else
					{
						cameraEffectBase.Hide();
					}
				}
			}
			else
			{
				Object.Destroy(this);
			}
		}

		private void OnDestroy()
		{
			if (_Instance == this)
			{
				_Instance = null;
			}
		}
	}
}
