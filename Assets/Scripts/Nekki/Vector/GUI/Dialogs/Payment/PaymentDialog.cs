using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs.Payment.Handlers;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs.Payment
{
	public class PaymentDialog : BaseDialog
	{
		public enum ScrollMode
		{
			Left = 0,
			Right = 1,
			Center = 2
		}

		[SerializeField]
		private Image _Bg;

		[SerializeField]
		private GameObject _ProductUIPrefab;

		[SerializeField]
		private ScrollRect _Scroller;

		[SerializeField]
		private HorizontalLayoutGroup _ScrollerLayout;

		[SerializeField]
		private CategoryButtons _CategoryButtons;

		[SerializeField]
		private PaymentHandler _PaymentHandler;

		[SerializeField]
		private AdsHandler _AdsHandler;

		[SerializeField]
		private GameObject _Blocker;

		[SerializeField]
		private LoadingCircle _LoadingCircle;

		private static PaymentDialog _Current;

		private HashSet<string> _AvaliableGroups = new HashSet<string>();

		private List<ProductUI> _ProductsUI = new List<ProductUI>();

		private static float _CanvasWidth = 0f;

		private static float _ProductUIWidth = 0f;

		public static PaymentDialog Current
		{
			get
			{
				return _Current;
			}
		}

		private float ScrollerContentWidth
		{
			get
			{
				int childCount = _Scroller.content.childCount;
				return _ProductUIWidth * (float)childCount + _ScrollerLayout.spacing * (float)(childCount - 1);
			}
		}

		private float ScrollerContentWidthWithPadding
		{
			get
			{
				return ScrollerContentWidth + (float)_ScrollerLayout.padding.right + (float)_ScrollerLayout.padding.left;
			}
		}

		private float ScrollerContentSizeLimit
		{
			get
			{
				return ScrollerContentWidthWithPadding - _Scroller.viewport.rect.width;
			}
		}

		public static event Action OnOpen;

		public static event Action OnClose;

		static PaymentDialog()
		{
			PaymentDialog.OnOpen = delegate
			{
			};
			PaymentDialog.OnClose = delegate
			{
			};
		}

		public void Init(string p_selectedGroup)
		{
			_Current = this;
			_PaymentHandler.Init(this);
			_AdsHandler.Init(this);
			SetAvaliableGroups();
			LoadProducts();
			_CategoryButtons.Init(this, p_selectedGroup);
			Unblock();
			if (Manager.IsRun)
			{
				Color color = _Bg.color;
				color.a = 0.6f;
				_Bg.color = color;
			}
			PaymentDialog.OnOpen();
		}

		public void Close()
		{
			PaymentDialog.OnClose();
			_PaymentHandler.Free();
			_AdsHandler.Free();
			ResetProducts();
			Dismiss();
			_Current = null;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_PaymentHandler.Free();
			_AdsHandler.Free();
			ResetProducts();
			_Current = null;
		}

		private void SetAvaliableGroups()
		{
			_AvaliableGroups.Clear();
			_AvaliableGroups.Add("Currency");
			_AvaliableGroups.Add("Promo");
			_AvaliableGroups.Add("Premium");
			if ((int)CounterController.Current.CounterBoosterpacksBlock == 0)
			{
				_AvaliableGroups.Add("Boosterpacks");
			}
			if (!Manager.IsRun)
			{
				_AvaliableGroups.Add("Ads");
			}
		}

		private void LoadProducts()
		{
			foreach (Product allProduct in ProductManager.Current.GetAllProducts())
			{
				CreateUI(allProduct);
			}
			UpdateLayout();
		}

		private void ResetProducts()
		{
			foreach (ProductUI item in _ProductsUI)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			_ProductsUI.Clear();
		}

		private void CreateUI(Product p_product)
		{
			if (IsGroupAvaliable(p_product.Group) && p_product.CanShow && p_product.IsAvaliable)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_ProductUIPrefab);
				gameObject.transform.SetParent(_Scroller.content, false);
				gameObject.transform.SetAsLastSibling();
				InitSizeConsts(gameObject);
				ProductUI component = gameObject.GetComponent<ProductUI>();
				component.Init(p_product, OnBuyProductTap, OnTimerExpired);
				_ProductsUI.Add(component);
			}
		}

		private void RemoveUI(ProductUI p_productUI)
		{
			if (_ProductsUI.Contains(p_productUI))
			{
				_ProductsUI.Remove(p_productUI);
				UnityEngine.Object.Destroy(p_productUI.gameObject);
			}
		}

		public void RemoveUIByProduct(Product p_product)
		{
			ProductUI productUI = _ProductsUI.Find((ProductUI p_productUI) => p_productUI.Product == p_product);
			if (productUI != null)
			{
				RemoveUI(productUI);
				UpdateLayout();
				_CategoryButtons.UpdateButtonsActive();
			}
		}

		private void InitSizeConsts(GameObject p_productGO)
		{
			if (_CanvasWidth == 0f)
			{
				_CanvasWidth = DialogCanvasController.DialogsCanvas.GetComponent<RectTransform>().sizeDelta.x;
			}
			if (_ProductUIWidth == 0f)
			{
				_ProductUIWidth = p_productGO.GetComponent<RectTransform>().sizeDelta.x;
			}
		}

		private void UpdateLayout()
		{
			float scrollerContentWidth = ScrollerContentWidth;
			int num = Math.Max((int)_ScrollerLayout.spacing, (int)((_CanvasWidth - scrollerContentWidth) * 0.5f));
			_ScrollerLayout.padding.right = num;
			_ScrollerLayout.padding.left = num;
		}

		public void SelectProductsGroup(string p_groupName, bool p_instant = false)
		{
			int num = 0;
			if (p_groupName == "Boosterpacks")
			{
				num = 1;
			}
			ProductUI productUI = null;
			foreach (ProductUI item in _ProductsUI)
			{
				if (item.Group == p_groupName)
				{
					productUI = item;
					if (num == 0)
					{
						break;
					}
					num--;
				}
			}
			if (productUI != null)
			{
				ScrollToItem(productUI, GetScrollModeByGroup(productUI.Group), (!p_instant) ? 0.4f : 0f);
			}
		}

		private ScrollMode GetScrollModeByGroup(string p_group)
		{
			switch (p_group)
			{
			case "Promo":
			case "Premium":
			case "Boosterpacks":
				return ScrollMode.Center;
			default:
				return ScrollMode.Left;
			}
		}

		private void ScrollToItem(ProductUI p_product, ScrollMode p_scrollMode, float p_duration = 0f)
		{
			ScrollToItem(p_product.GetComponent<RectTransform>(), p_scrollMode, p_duration);
		}

		private void ScrollToItem(RectTransform p_item, ScrollMode p_scrollMode, float p_duration = 0f)
		{
			float num = 0f;
			switch (p_scrollMode)
			{
			case ScrollMode.Left:
				num = (0f - (_ProductUIWidth + _ScrollerLayout.spacing)) * (float)p_item.GetSiblingIndex();
				break;
			case ScrollMode.Right:
				num = (0f - (_ProductUIWidth + _ScrollerLayout.spacing)) * (float)p_item.GetSiblingIndex() + _CanvasWidth - _ProductUIWidth - 2f * _ScrollerLayout.spacing;
				break;
			default:
				num = (0f - (_ProductUIWidth + _ScrollerLayout.spacing)) * (float)p_item.GetSiblingIndex() + (_CanvasWidth - _ProductUIWidth) * 0.5f - _ScrollerLayout.spacing;
				break;
			}
			num = Mathf.Clamp(num, 0f - ScrollerContentSizeLimit, 0f);
			_Scroller.velocity = Vector2.zero;
			DOTween.Kill(_Scroller.content);
			if (p_duration == 0f)
			{
				_Scroller.content.localPosition = new Vector2(num, 0f);
			}
			else
			{
				_Scroller.content.DOLocalMoveX(num, p_duration);
			}
		}

		private int IsProductIntersectViewport(ProductUI p_productUI)
		{
			Vector2 rectMinMaxX = GetRectMinMaxX(p_productUI.GetComponent<RectTransform>(), 0.5f, 0.5f);
			Vector2 rectMinMaxX2 = GetRectMinMaxX(_Scroller.viewport, _Scroller.viewport.anchorMin.x, _Scroller.viewport.anchorMax.x);
			if (rectMinMaxX.x < rectMinMaxX2.x && rectMinMaxX.y > rectMinMaxX2.x)
			{
				return -1;
			}
			if (rectMinMaxX.x < rectMinMaxX2.y && rectMinMaxX.y > rectMinMaxX2.y)
			{
				return 1;
			}
			return 0;
		}

		private Vector2 GetRectMinMaxX(RectTransform p_rect, float p_anchorMin, float p_anchorMax)
		{
			Vector2 vector = Vector2.Scale(p_rect.rect.size, p_rect.lossyScale);
			return new Vector2(p_rect.position.x - vector.x * p_anchorMin, p_rect.position.x + vector.x * p_anchorMax);
		}

		private bool IsGroupAvaliable(string p_group)
		{
			return _AvaliableGroups.Contains(p_group);
		}

		public bool IsProductsOfGroupExists(string p_group)
		{
			foreach (ProductUI item in _ProductsUI)
			{
				if (item.Group == p_group)
				{
					return true;
				}
			}
			return false;
		}

		public void ShowNotification(string p_text)
		{
			Notification.Parameters parameters = new Notification.Parameters();
			parameters.HideBy = Notification.HideBy.TimeDontBlockClicks;
			parameters.Image = string.Empty;
			parameters.Orientation = Notification.Orientation.Top;
			parameters.Text = p_text;
			parameters.QueueType = DialogQueueType.Notification;
			DialogNotificationManager.ShowSimpleNotification(parameters);
		}

		public void UpdateProducts()
		{
			foreach (ProductUI item in _ProductsUI)
			{
				item.UpdateProductInfo();
			}
		}

		private void Block()
		{
			_Blocker.gameObject.SetActive(true);
			_LoadingCircle.Play();
		}

		public void Unblock()
		{
			_LoadingCircle.Stop();
			_Blocker.gameObject.SetActive(false);
		}

		private void OnBuyProductTap(ProductUI p_productUI)
		{
			int num = IsProductIntersectViewport(p_productUI);
			if (num < 0)
			{
				ScrollToItem(p_productUI, ScrollMode.Left, 0.2f);
				return;
			}
			if (num > 0)
			{
				ScrollToItem(p_productUI, ScrollMode.Right, 0.2f);
				return;
			}
			Block();
			if (p_productUI.IsAds)
			{
				_AdsHandler.UseProduct(p_productUI.Product);
			}
			else
			{
				_PaymentHandler.UseProduct(p_productUI.Product);
			}
		}

		private void OnTimerExpired(ProductUI p_productUI)
		{
			RemoveUI(p_productUI);
			UpdateLayout();
			_CategoryButtons.UpdateButtonsActive();
		}

		public void OnKeyDown(KeyCode p_code)
		{
			if (p_code == KeyCode.Escape && DeviceInformation.IsAndroid && !_Blocker.gameObject.activeSelf)
			{
				Close();
			}
		}
	}
}
