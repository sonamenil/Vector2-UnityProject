using System.Collections.Generic;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class FundsPanel : MonoBehaviour
	{
		private const int VerticalStartVar = 0;

		private const int VerticalOffset = -100;

		private int _Money1Count;

		private int _Money2Count;

		private int _Money3Count;

		private bool _CanShowMoney1;

		private bool _CanShowMoney2;

		private bool _CanShowMoney3;

		private bool _SaveMeOpened;

		[SerializeField]
		private FundElement _Money1Element;

		[SerializeField]
		private FundElement _Money2Element;

		[SerializeField]
		private FundElement _Money3Element;

		private List<FundElement> _CurentShowed = new List<FundElement>();

		private void SetElementPosition(FundElement element)
		{
			element.transform.localPosition = new Vector3(element.transform.localPosition.x, 0 + element.Index * -100, 0f);
		}

		private void OnHideElement(FundElement element)
		{
			_CurentShowed.Remove(element);
			foreach (FundElement item in _CurentShowed)
			{
				if (item.Index > element.Index)
				{
					item.Index--;
				}
				item.SetPosition(new Vector3(item.transform.localPosition.x, 0 + item.Index * -100, 0f));
			}
		}

		private void UpdateMoney1()
		{
			if (_Money1Count != (int)DataLocal.Current.Money1)
			{
				ShowMoney1();
				_Money1Count = DataLocal.Current.Money1;
			}
		}

		private void ShowMoney1()
		{
			if (!_CurentShowed.Contains(_Money1Element))
			{
				_Money1Element.Index = _CurentShowed.Count;
				_CurentShowed.Add(_Money1Element);
			}
			SetElementPosition(_Money1Element);
			_Money1Element.AddFunds(DataLocal.Current.Money1);
		}

		private void UpdateMoney2()
		{
			if (_Money2Count != (int)DataLocal.Current.Money2)
			{
				ShowMoney2();
				_Money2Count = DataLocal.Current.Money2;
			}
		}

		private void ShowMoney2()
		{
			if (!_CurentShowed.Contains(_Money2Element))
			{
				_Money2Element.Index = _CurentShowed.Count;
				_CurentShowed.Add(_Money2Element);
			}
			SetElementPosition(_Money2Element);
			_Money2Element.AddFunds(DataLocal.Current.Money2);
		}

		private void UpdateMoney3()
		{
			if (_Money3Count != (int)DataLocal.Current.Money3)
			{
				ShowMoney3();
				_Money3Count = DataLocal.Current.Money3;
			}
		}

		private void ShowMoney3()
		{
			if (!_CurentShowed.Contains(_Money3Element))
			{
				_Money3Element.Index = _CurentShowed.Count;
				_CurentShowed.Add(_Money3Element);
			}
			SetElementPosition(_Money3Element);
			_Money3Element.AddFunds(DataLocal.Current.Money3);
		}

		private void Update()
		{
			if (_SaveMeOpened)
			{
				ShowChips();
				return;
			}
			if (_CanShowMoney1)
			{
				UpdateMoney1();
			}
			if (_CanShowMoney2)
			{
				UpdateMoney2();
			}
			if (_CanShowMoney3)
			{
				UpdateMoney3();
			}
		}

		private void Start()
		{
			_Money3Element.Hiden += OnHideElement;
			_Money1Element.Hiden += OnHideElement;
			_Money2Element.Hiden += OnHideElement;
			DataLocal.Current.OnItem += OnItem;
			RunMainController.OnPause += RunMainControllerOnOnPause;
			ControllerSaveMe.Show += ControllerSaveMeOnShow;
			ControllerSaveMe.Close += ControllerSaveMeOnClose;
			_CanShowMoney1 = (int)CounterController.Current.CounterTutorialBasic == 0;
			_CanShowMoney2 = true;
			_CanShowMoney3 = (int)CounterController.Current.CounterTutorialBasic == 0;
		}

		private void RunMainControllerOnOnPause(bool pPause)
		{
			if (!pPause && !_SaveMeOpened)
			{
				ShowAllFunds();
			}
		}

		private void ShowChips()
		{
			if (_Money3Count != (int)DataLocal.Current.Money3)
			{
				_Money3Element.Show(DataLocal.Current.Money3);
				_Money3Count = DataLocal.Current.Money3;
			}
		}

		private void ControllerSaveMeOnClose()
		{
			_CanShowMoney3 = true;
			_SaveMeOpened = false;
			_Money3Element.Hide();
		}

		private void ControllerSaveMeOnShow()
		{
			if (!_CurentShowed.Contains(_Money3Element))
			{
				_Money3Element.Index = _CurentShowed.Count;
				_CurentShowed.Add(_Money3Element);
			}
			SetElementPosition(_Money3Element);
			_Money3Element.Show(DataLocal.Current.Money3);
			_CanShowMoney3 = false;
			_SaveMeOpened = true;
		}

		private void OnItem(Action Action, UserItem userItem, int Value)
		{
			if (userItem.VisualName == DataLocal.Money2Name)
			{
				_CanShowMoney2 = true;
			}
			else if (userItem.VisualName == DataLocal.Money1Name)
			{
				_CanShowMoney1 = true;
			}
			else if (userItem.VisualName == DataLocal.Money3Name)
			{
				_CanShowMoney3 = true;
			}
		}

		private void OnDestroy()
		{
			_Money3Element.Hiden -= OnHideElement;
			_Money1Element.Hiden -= OnHideElement;
			_Money2Element.Hiden -= OnHideElement;
			DataLocal.Current.OnItem -= OnItem;
			ControllerSaveMe.Show -= ControllerSaveMeOnShow;
			ControllerSaveMe.Close -= ControllerSaveMeOnClose;
			RunMainController.OnPause -= RunMainControllerOnOnPause;
		}

		public void ShowAllFunds()
		{
			if (_CanShowMoney1 && (int)DataLocal.Current.Money1 > 0)
			{
				ShowMoney1();
			}
			if (_CanShowMoney2 && (int)DataLocal.Current.Money2 > 0)
			{
				ShowMoney2();
			}
			if (_CanShowMoney3 && (int)DataLocal.Current.Money3 > 0)
			{
				ShowMoney3();
			}
		}
	}
}
