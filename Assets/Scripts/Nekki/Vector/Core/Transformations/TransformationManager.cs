using System.Collections.Generic;

namespace Nekki.Vector.Core.Transformations
{
	public class TransformationManager
	{
		private static TransformationManager _Curent;

		private HashSet<Transformation> _Storage;

		private List<Transformation> _FlushStorage;

		public static TransformationManager Current
		{
			get
			{
				if (_Curent == null)
				{
					_Curent = new TransformationManager();
				}
				return _Curent;
			}
		}

		public TransformationManager()
		{
			_Storage = new HashSet<Transformation>();
			_FlushStorage = new List<Transformation>();
		}

		public static void Clear()
		{
			if (_Curent != null)
			{
				_Curent = null;
			}
		}

		public void Reset()
		{
			foreach (Transformation item in _Storage)
			{
				item.Reset();
			}
			_Storage.Clear();
			_FlushStorage.Clear();
		}

		public void Add(Transformation p_transformation)
		{
			if (!_Storage.Contains(p_transformation))
			{
				if (p_transformation.IsChangePosition)
				{
					p_transformation.Parent.TransformationStart();
				}
				p_transformation.Reset();
				_Storage.Add(p_transformation);
			}
		}

		public void Stop(string p_name)
		{
			foreach (Transformation item in _Storage)
			{
				if (item.Name == p_name)
				{
					Remove(item);
				}
			}
		}

		public void Remove(Transformation p_transformation)
		{
			_FlushStorage.Add(p_transformation);
		}

		public void Update()
		{
			if (_Storage.Count == 0)
			{
				return;
			}
			foreach (Transformation item in _Storage)
			{
				if (!item.Iteration())
				{
					Remove(item);
				}
			}
			foreach (Transformation item2 in _Storage)
			{
				item2.Parent.SetDeltaMove();
			}
		}

		public void RemoveEndedTransformation()
		{
			if (_FlushStorage.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _FlushStorage.Count; i++)
			{
				if (_FlushStorage[i].Parent != null)
				{
					_FlushStorage[i].Parent.TransformResetTween();
					if (_FlushStorage[i].IsChangePosition)
					{
						_FlushStorage[i].Parent.TransformationEnd();
					}
				}
				_Storage.Remove(_FlushStorage[i]);
			}
			_FlushStorage.Clear();
		}

		public void RemoveTransformationByParent(TransformInterface p_parent)
		{
			foreach (Transformation item in _Storage)
			{
				if (item.Parent == p_parent)
				{
					Remove(item);
				}
			}
		}
	}
}
