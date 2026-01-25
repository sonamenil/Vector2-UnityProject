using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Scripts;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class FloorButtonsController : MonoBehaviour
	{
		private const string _NextPageId = "_NextPage_";

		private const string _PrevPageId = "_PrevPage_";

		[SerializeField]
		private List<FloorButton> _Buttons;

		private FloorButton _LastButtonTap;

		private SelectFloor _Parent;

		private List<List<string>> _Pages = new List<List<string>>();

		private int _CurrentPage;

		public Vector3 Position
		{
			get
			{
				return base.transform.localPosition;
			}
		}

		public void CheckPosition()
		{
			UIAspectRatioFitter component = GetComponent<UIAspectRatioFitter>();
			if (component != null)
			{
				component.Calculate();
			}
		}

		public void Init(SelectFloor p_parent, List<string> p_starterPacks)
		{
			_Parent = p_parent;
			_LastButtonTap = null;
			_Pages.Clear();
			if (p_starterPacks.Count <= 4)
			{
				List<string> list = new List<string>();
				foreach (string p_starterPack in p_starterPacks)
				{
					list.Add(p_starterPack);
				}
				_Pages.Add(list);
				_CurrentPage = 0;
			}
			else
			{
				int count = p_starterPacks.Count;
				List<string> list = new List<string>();
				list.Add(p_starterPacks[0]);
				list.Add(p_starterPacks[1]);
				list.Add(p_starterPacks[2]);
				list.Add("_NextPage_");
				_Pages.Add(list);
				int i = 1;
				for (int num = count - 4; i < num; i++)
				{
					list = new List<string>();
					list.Add("_PrevPage_");
					list.Add(p_starterPacks[i * 2 + 1 - i]);
					list.Add(p_starterPacks[i * 2 + 2 - i]);
					list.Add("_NextPage_");
					_Pages.Add(list);
				}
				list = new List<string>();
				list.Add("_PrevPage_");
				list.Add(p_starterPacks[count - 3]);
				list.Add(p_starterPacks[count - 2]);
				list.Add(p_starterPacks[count - 1]);
				_Pages.Add(list);
				_CurrentPage = GetStarterPackContainingLastPageIndex(StarterPacksManager.SelectedStarterPack);
			}
			UpdateButtons();
		}

		public void SelectNextPage()
		{
			_CurrentPage++;
			if (_CurrentPage >= _Pages.Count)
			{
				_CurrentPage = 0;
			}
			Scene<MainScene>.Current.SwitchPage(UpdateButtons);
		}

		public void SelectPrevPage()
		{
			_CurrentPage--;
			if (_CurrentPage < 0)
			{
				_CurrentPage = _Pages.Count - 1;
			}
			Scene<MainScene>.Current.SwitchPage(UpdateButtons);
		}

		public void UpdateButtons()
		{
			StarterPackItem selectedStarterPack = StarterPacksManager.SelectedStarterPack;
			bool flag = false;
			List<string> list = _Pages[_CurrentPage];
			int i = 0;
			for (int count = _Buttons.Count; i < count; i++)
			{
				_Buttons[i].Init(this);
				if (list.Count > i && list[i] == "_NextPage_")
				{
					_Buttons[i].SetNextPageMode();
				}
				else if (list.Count > i && list[i] == "_PrevPage_")
				{
					_Buttons[i].SetPrevPageMode();
				}
				else
				{
					_Buttons[i].StarterPackItem = StarterPacksManager.GetActiveStarterPackByName(list[i]);
				}
				if (selectedStarterPack != null && selectedStarterPack == _Buttons[i].StarterPackItem)
				{
					FloorButtonClicked(_Buttons[i]);
					flag = true;
				}
			}
			if (selectedStarterPack == null || !flag)
			{
				FloorButtonClicked(GetBestAvailableStarterPackButton());
			}
		}

		public bool FloorButtonClicked(FloorButton p_button)
		{
			bool result = false;
			if (_LastButtonTap == p_button)
			{
				return result;
			}
			_LastButtonTap = p_button;
			ResetSelection();
			p_button.SetShift();
			if (p_button.IsBlock)
			{
				p_button.SetBlockColor();
			}
			else
			{
				p_button.SetSelectedColor();
			}
			_Parent.UpdateLine();
			if (p_button.Mode == FloorButton.ButtonMode.NextPage)
			{
				result = true;
				SelectNextPage();
			}
			else if (p_button.Mode == FloorButton.ButtonMode.PrevPage)
			{
				result = true;
				SelectPrevPage();
			}
			else
			{
				result = _Parent.UserSelectStarterPack(p_button);
			}
			return result;
		}

		private void ResetSelection()
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				_Buttons[i].ResetShift();
				if (_Buttons[i].IsBlock)
				{
					_Buttons[i].SetBlockColor();
				}
				else
				{
					_Buttons[i].SetNormalColor();
				}
			}
		}

		private int GetStarterPackContainingLastPageIndex(StarterPackItem p_starterPack)
		{
			if (p_starterPack == null)
			{
				return 0;
			}
			for (int num = _Pages.Count - 1; num >= 0; num--)
			{
				if (_Pages[num].Contains(p_starterPack.Name))
				{
					return num;
				}
			}
			return 0;
		}

		private FloorButton GetBestAvailableStarterPackButton()
		{
			FloorButton result = null;
			for (int num = _Buttons.Count - 1; num >= 0; num--)
			{
				if (_Buttons[num].Mode == FloorButton.ButtonMode.StarterPack)
				{
					result = _Buttons[num];
					if (!_Buttons[num].IsBlock)
					{
						return _Buttons[num];
					}
				}
			}
			return result;
		}
	}
}
