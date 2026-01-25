using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI
{
	public class ModuleHolder : MonoBehaviour
	{
		[SerializeField]
		private List<UIModule> _MountOnStart = new List<UIModule>();

		[SerializeField]
		private List<UIModule> _MountOnLater = new List<UIModule>();

		[SerializeField]
		private List<UIModule> _MountByRequest = new List<UIModule>();

		public List<UIModule> MountOnStart
		{
			get
			{
				return _MountOnStart;
			}
		}

		public List<UIModule> MountOnLater
		{
			get
			{
				return _MountOnLater;
			}
		}

		protected virtual void BeforeMountModules()
		{
		}

		protected virtual bool InitCondition()
		{
			return true;
		}

		protected virtual void Awake()
		{
			BeforeMountModules();
			MountModules();
		}

		private void MountModules()
		{
			foreach (UIModule item in _MountOnStart)
			{
				UIModule.MountModule(item, base.transform, true);
			}
			foreach (UIModule item2 in _MountOnLater)
			{
				UIModule.MountModule(item2, base.transform, false);
			}
		}

		protected virtual void OnDestroy()
		{
		}

		protected T GetModule<T>() where T : UIModule
		{
			return UIModule.GetModule<T>();
		}

		public T GetOrMountModule<T>() where T : UIModule
		{
			T val = GetModule<T>();
			if (val == null)
			{
				foreach (UIModule item in _MountByRequest)
				{
					if (item is T)
					{
						val = UIModule.MountModule(item, base.transform, true) as T;
						return val;
					}
				}
			}
			return val;
		}

		public UIModule GetModuleByName(string p_name)
		{
			return UIModule.GetModuleByName(p_name);
		}

		public List<UIModule> ActiveModule()
		{
			List<UIModule> list = new List<UIModule>();
			GetActiveModules(_MountOnStart, list);
			GetActiveModules(_MountOnLater, list);
			return list;
		}

		protected static void GetActiveModules(List<UIModule> p_modules, List<UIModule> p_result)
		{
			for (int i = 0; i < p_modules.Count; i++)
			{
				if (p_modules[i].gameObject.activeSelf)
				{
					p_result.Add(p_modules[i]);
				}
			}
		}
	}
}
