using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI
{
	public class ComponentsPool<T> where T : MonoBehaviour
	{
		private Transform _Parent;

		private GameObject _Prefab;

		private List<T> _ObjectsInUse = new List<T>();

		private Stack<T> _ObjectsFree = new Stack<T>();

		public GameObject Prefab
		{
			get
			{
				return _Prefab;
			}
		}

		public List<T> ObjectsInUse
		{
			get
			{
				return _ObjectsInUse;
			}
		}

		public ComponentsPool(Transform p_parent, GameObject p_prefab, int p_initialCacheSize = 0)
		{
			_Parent = p_parent;
			_Prefab = p_prefab;
			_Parent.gameObject.SetActive(false);
			if (p_initialCacheSize > 0)
			{
				for (int i = 0; i < p_initialCacheSize; i++)
				{
					GameObject gameObject = Object.Instantiate(_Prefab);
					Register(gameObject.GetComponent<T>(), false);
				}
			}
		}

		public void Register(T p_obj, bool p_inUse = true)
		{
			if (p_inUse)
			{
				_ObjectsInUse.Add(p_obj);
				return;
			}
			p_obj.transform.SetParent(_Parent);
			_ObjectsFree.Push(p_obj);
		}

		public T Get()
		{
			T val = (T)null;
			if (_ObjectsFree.Count > 0)
			{
				val = _ObjectsFree.Pop();
			}
			else
			{
				GameObject gameObject = Object.Instantiate(_Prefab);
				val = gameObject.GetComponent<T>();
			}
			_ObjectsInUse.Add(val);
			return val;
		}

		public void Return(T p_obj)
		{
			p_obj.transform.SetParent(_Parent, false);
			_ObjectsInUse.Remove(p_obj);
			_ObjectsFree.Push(p_obj);
		}

		public void ReturnAll()
		{
			foreach (T item in _ObjectsInUse)
			{
				T current = item;
				current.transform.SetParent(_Parent, false);
				_ObjectsFree.Push(current);
			}
			_ObjectsInUse.Clear();
		}

		public void Clear()
		{
			foreach (T item in _ObjectsInUse)
			{
				T current = item;
				Object.Destroy(current.gameObject);
			}
			_ObjectsInUse.Clear();
			foreach (T item2 in _ObjectsFree)
			{
				T current2 = item2;
				Object.Destroy(current2.gameObject);
			}
			_ObjectsFree.Clear();
		}
	}
}
