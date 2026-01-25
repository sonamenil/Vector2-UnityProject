using System;
using System.Collections;
using System.Collections.Generic;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI
{
	public class UIModule : MonoBehaviour
	{
		private static List<UIModule> _Modules = new List<UIModule>();

		[SerializeField]
		private int _Order;

		private Canvas _SceneCanvas;

		protected bool _IsInited;

		protected bool _IsReleased;

		public bool IsActive
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		public static event Action<UIModule> OnModuleActivated;

		public static event Action<UIModule> OnModuleDeactivated;

		public static UIModule MountModule(UIModule p_pref, Transform p_parent, bool p_active)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(p_pref.gameObject);
			gameObject.name = p_pref.gameObject.name;
			gameObject.transform.SetParent(p_parent, false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(false);
			UIModule component = gameObject.GetComponent<UIModule>();
			component._SceneCanvas = p_parent.GetComponent<Canvas>();
			if (p_active)
			{
				component.Activate();
			}
			return component;
		}

		public static T GetModule<T>() where T : UIModule
		{
			for (int i = 0; i < _Modules.Count; i++)
			{
				if (_Modules[i] is T)
				{
					return _Modules[i] as T;
				}
			}
			return (T)null;
		}

		public static UIModule GetModuleByName(string p_name)
		{
			for (int i = 0; i < _Modules.Count; i++)
			{
				if (_Modules[i].name == p_name)
				{
					return _Modules[i];
				}
			}
			return null;
		}

		public void Activate()
		{
			base.gameObject.SetActive(true);
			GetComponent<RectTransform>().SetSiblingIndex(_Order);
			if (!_IsInited)
			{
				Init();
			}
			OnActivated();
			CoroutineManager.Current.StartRoutine(SendEvent(UIModule.OnModuleActivated));
		}

		public void DeActivate()
		{
			base.gameObject.SetActive(false);
			OnDeactivated();
			CoroutineManager.Current.StartRoutine(SendEvent(UIModule.OnModuleDeactivated));
		}

		private IEnumerator SendEvent(Action<UIModule> p_event)
		{
			yield return new WaitForEndOfFrame();
			if (p_event != null)
			{
				p_event(this);
			}
		}

		public void MoveToDialogsCanvas()
		{
			Canvas dialogsCanvas = DialogCanvasController.DialogsCanvas;
			if (dialogsCanvas != null)
			{
				base.transform.SetParent(dialogsCanvas.transform, false);
			}
		}

		public void MoveToSceneCanvas()
		{
			base.transform.SetParent(_SceneCanvas.transform, false);
			Activate();
		}

		protected virtual void Init()
		{
			_IsInited = true;
		}

		protected virtual void Free()
		{
			_IsReleased = true;
		}

		protected virtual void OnActivated()
		{
		}

		protected virtual void OnDeactivated()
		{
		}

		public List<Button> GetButtonsByName(string p_name)
		{
			List<Button> list = new List<Button>();
			Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (p_name == null || componentsInChildren[i].name == p_name)
				{
					list.Add(componentsInChildren[i]);
				}
			}
			return list;
		}

		private void Awake()
		{
			_Modules.Add(this);
		}

		private void OnDestroy()
		{
			if (!_IsReleased)
			{
				Free();
			}
			_Modules.Remove(this);
		}
	}
}
