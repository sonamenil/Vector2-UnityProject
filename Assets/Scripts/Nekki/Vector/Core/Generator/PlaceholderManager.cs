using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Generator
{
	public class PlaceholderManager
	{
		private List<Placeholder> _DummyPlaceholders = new List<Placeholder>();

		private List<Placeholder> _UnSortedPlaceholders = new List<Placeholder>();

		private Dictionary<Placeholder.SortTypeEnum, List<Placeholder>> _SortedPlaceholders = new Dictionary<Placeholder.SortTypeEnum, List<Placeholder>>();

		private Queue<Placeholder> _PostponeQueue = new Queue<Placeholder>();

		public void AppendPlaceholders(List<Placeholder> p_placeholders)
		{
			for (int i = 0; i < p_placeholders.Count; i++)
			{
				if (p_placeholders[i].IsDummy)
				{
					_DummyPlaceholders.Add(p_placeholders[i]);
				}
				else if (p_placeholders[i].IsSorted)
				{
					AppendSortedPlaceholder(p_placeholders[i]);
				}
				else
				{
					_UnSortedPlaceholders.Add(p_placeholders[i]);
				}
			}
		}

		private void AppendSortedPlaceholder(Placeholder p_placeholder)
		{
			switch (p_placeholder.SortType)
			{
			case Placeholder.SortTypeEnum.ByX:
				AppendSortedByX(p_placeholder);
				break;
			case Placeholder.SortTypeEnum.ByXRevers:
				AppendSortedByXReverse(p_placeholder);
				break;
			}
		}

		private void AppendSortedByX(Placeholder p_placeholder)
		{
			List<Placeholder> listPlaceholdersBySortType = GetListPlaceholdersBySortType(Placeholder.SortTypeEnum.ByX);
			if (listPlaceholdersBySortType.Count == 0)
			{
				listPlaceholdersBySortType.Add(p_placeholder);
				return;
			}
			for (int i = 0; i < listPlaceholdersBySortType.Count; i++)
			{
				if (p_placeholder.Position.x < listPlaceholdersBySortType[i].Position.x)
				{
					listPlaceholdersBySortType.Insert(i, p_placeholder);
					return;
				}
			}
			listPlaceholdersBySortType.Add(p_placeholder);
		}

		private void AppendSortedByXReverse(Placeholder p_placeholder)
		{
			List<Placeholder> listPlaceholdersBySortType = GetListPlaceholdersBySortType(Placeholder.SortTypeEnum.ByXRevers);
			if (listPlaceholdersBySortType.Count == 0)
			{
				listPlaceholdersBySortType.Add(p_placeholder);
				return;
			}
			for (int i = 0; i < listPlaceholdersBySortType.Count; i++)
			{
				if (p_placeholder.Position.x > listPlaceholdersBySortType[i].Position.x)
				{
					listPlaceholdersBySortType.Insert(i, p_placeholder);
					return;
				}
			}
			listPlaceholdersBySortType.Add(p_placeholder);
		}

		private List<Placeholder> GetListPlaceholdersBySortType(Placeholder.SortTypeEnum p_type)
		{
			List<Placeholder> list;
			if (_SortedPlaceholders.ContainsKey(p_type))
			{
				list = _SortedPlaceholders[p_type];
			}
			else
			{
				list = new List<Placeholder>();
				_SortedPlaceholders.Add(p_type, list);
			}
			return list;
		}

		public void Postprocess(Dictionary<string, string> p_ChoisesDictionary)
		{
			CounterController.Current.ClearCounterNamespace("ST_Postprocess");
			PostprocessDummy(p_ChoisesDictionary);
			PostprocessSorted(p_ChoisesDictionary);
			PostprocessUnSorted(p_ChoisesDictionary);
			if (_PostponeQueue.Count != 0)
			{
				PostrocessPostpone(p_ChoisesDictionary, _PostponeQueue);
			}
		}

		private void PostprocessDummy(Dictionary<string, string> p_ChoisesDictionary)
		{
			for (int i = 0; i < _DummyPlaceholders.Count; i++)
			{
				_DummyPlaceholders[i].PostProcessDummy();
			}
			_DummyPlaceholders.Clear();
		}

		private void PostprocessSorted(Dictionary<string, string> p_ChoisesDictionary)
		{
			foreach (List<Placeholder> value in _SortedPlaceholders.Values)
			{
				for (int i = 0; i < value.Count; i++)
				{
					bool p_postpone = false;
					value[i].PostProcess(p_ChoisesDictionary, ref p_postpone);
					if (p_postpone)
					{
						_PostponeQueue.Enqueue(value[i]);
					}
				}
				value.Clear();
			}
		}

		private void PostprocessUnSorted(Dictionary<string, string> p_ChoisesDictionary)
		{
			MainRandom.ShuffleList(_UnSortedPlaceholders);
			for (int i = 0; i < _UnSortedPlaceholders.Count; i++)
			{
				bool p_postpone = false;
				_UnSortedPlaceholders[i].PostProcess(p_ChoisesDictionary, ref p_postpone);
				if (p_postpone)
				{
					_PostponeQueue.Enqueue(_UnSortedPlaceholders[i]);
				}
			}
			_UnSortedPlaceholders.Clear();
		}

		private void PostrocessPostpone(Dictionary<string, string> p_ChoisesDictionary, Queue<Placeholder> p_postponeQueue)
		{
			Placeholder placeholder = null;
			while (p_postponeQueue.Count != 0)
			{
				bool p_postpone = false;
				placeholder = p_postponeQueue.Dequeue();
				placeholder.PostProcess(p_ChoisesDictionary, ref p_postpone);
				if (p_postpone)
				{
					p_postponeQueue.Enqueue(placeholder);
				}
			}
		}
	}
}
