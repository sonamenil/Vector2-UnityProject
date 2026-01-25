using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.InputControllers;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class SelectZone : MonoBehaviour
	{
		[SerializeField]
		private GameObject _SelectorPrefab;

		private List<ZoneSelector> _Selectors = new List<ZoneSelector>();

		private ZoneSelector _CurrentSelector;

		public void Refresh()
		{
			ClearSelectors();
			CreateSelectors();
			Zone currentZone = ZoneManager.CurrentZone;
			SetCurrentSelector(_Selectors.Find((ZoneSelector p_selector) => p_selector.Zone == currentZone), false);
		}

		public void OnSlide(int p_index, Vector2 p_from, Vector2 p_to)
		{
			switch (TouchController.GetDirection(p_from, p_to))
			{
			case Direction.Left:
				SelectPrev();
				break;
			case Direction.Right:
				SelectNext();
				break;
			}
		}

		private void ClearSelectors()
		{
			if (_Selectors.Count > 0)
			{
				foreach (ZoneSelector selector in _Selectors)
				{
					Object.DestroyImmediate(selector.gameObject);
				}
				_Selectors.Clear();
			}
			_CurrentSelector = null;
		}

		private void CreateSelectors()
		{
			HashSet<Zone> availableZones = ZoneManager.AvailableZones;
			foreach (Zone item in availableZones)
			{
				CreateZoneSelector(item);
			}
			base.gameObject.SetActive(_Selectors.Count > 1);
		}

		private void CreateZoneSelector(Zone p_zone)
		{
			GameObject gameObject = Object.Instantiate(_SelectorPrefab);
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.SetAsLastSibling();
			gameObject.name = string.Format("Selector_{0}", p_zone);
			ZoneSelector component = gameObject.GetComponent<ZoneSelector>();
			component.Init(p_zone);
			_Selectors.Add(component);
		}

		public void SelectNext()
		{
			int num = _Selectors.IndexOf(_CurrentSelector) + 1;
			if (num >= _Selectors.Count)
			{
				num = 0;
			}
			SetCurrentSelector(_Selectors[num]);
		}

		public void SelectPrev()
		{
			int num = _Selectors.IndexOf(_CurrentSelector) - 1;
			if (num < 0)
			{
				num = _Selectors.Count - 1;
			}
			SetCurrentSelector(_Selectors[num]);
		}

		public void SetCurrentSelector(ZoneSelector p_selector, bool p_manualSelect = true)
		{
			if (!(_CurrentSelector == p_selector))
			{
				if (_CurrentSelector != null)
				{
					_CurrentSelector.Unselect();
				}
				_CurrentSelector = p_selector;
				_CurrentSelector.Select();
				if (p_manualSelect)
				{
					Scene<MainScene>.Current.SwitchZone(_CurrentSelector.Zone);
				}
			}
		}

		public void OnTap()
		{
			SelectNext();
		}
	}
}
