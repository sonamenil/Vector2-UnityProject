using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class TerminalItemsPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject _TabPrefab;

		[SerializeField]
		private List<TerminalItemTab> _Tabs = new List<TerminalItemTab>();

		private TerminalPanel _Parent;

		private TerminalItemTab _SelectedTab;

		public TerminalItemTab SelectedTab
		{
			get
			{
				return _SelectedTab;
			}
		}

		public void Init(List<TerminalItemGroupAttribute> p_items, TerminalPanel p_parent)
		{
			_Parent = p_parent;
			foreach (TerminalItemGroupAttribute p_item in p_items)
			{
				CreateTab(p_item, OnTabTap);
			}
			foreach (TerminalItemTab tab in _Tabs)
			{
				if (tab.Item.IsExpired)
				{
					tab.TwinkleOff();
				}
			}
			_SelectedTab = _Tabs[0];
			_SelectedTab.MoveRight(0f);
			_Parent.SetTerminalItemTab(_SelectedTab);
		}

		public void RerollAll()
		{
			foreach (TerminalItemTab tab in _Tabs)
			{
				tab.Reset();
				TerminalItemsManager.Reroll(tab.Item);
				tab.RefreshReward();
			}
		}

		private void CreateTab(TerminalItemGroupAttribute p_item, Action<TerminalItemTab> p_onTabTap)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_TabPrefab);
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.SetAsLastSibling();
			TerminalItemTab component = gameObject.GetComponent<TerminalItemTab>();
			component.Init(p_item, p_onTabTap);
			_Tabs.Add(component);
		}

		private void OnTabTap(TerminalItemTab p_tab)
		{
			if (!(p_tab == _SelectedTab))
			{
				AudioManager.PlaySound("select_button");
				p_tab.MoveRight(0.2f);
				_SelectedTab.MoveLeft(0.2f);
				_SelectedTab = p_tab;
				_Parent.SetTerminalItemTab(p_tab);
			}
		}
	}
}
