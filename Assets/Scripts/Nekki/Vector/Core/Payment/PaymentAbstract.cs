using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.Payment
{
	public abstract class PaymentAbstract
	{
		public Action<List<Product>> OnProductsListRequestSuccess;

		public Action<string> OnProductsListRequestFalied;

		public Action<Product> OnPurchaseSuccess;

		public Action<string> OnPurchaseCanceled;

		public Action<string> OnPurchaseFailed;

		public Action<string> OnCheatPurchase;

		public Action<string> OnTryPaymentLater;

		public System.Action OnRestorePurchasesSuccess;

		public Action<string> OnRestorePurchasesFailed;

		public System.Action OnRefreshReceiptSuccess;

		public Action<string> OnRefreshReceiptFailed;

		public System.Action OnPaymentQueueUpdatedDownloads;

		private string _LastProductID;

		private long _StartPurchaseTime;

		public abstract void Init();

		public abstract void Free();

		public abstract bool CanMakePurchaces();

		public abstract void GetProductsData(params string[] p_productIds);

		public abstract void RestorePurchases();

		public abstract void RefreshReceipt();

		public virtual void MakePurchase(string p_productId)
		{
			_LastProductID = p_productId;
			ArgsDict argsDict = new ArgsDict();
			argsDict.Add("type", "start");
			argsDict.Add("product", GetCleanProductId(p_productId));
			argsDict.Add("error", string.Empty);
			ArgsDict args = argsDict;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Payment, args);
			GoogleAnalyticsV4.getInstance().LogEvent("Payment", "Start", p_productId, 1L);
			_StartPurchaseTime = TimeManager.UTCTime;
		}

		public void GivePaymentRevard(string p_productIdentifier, PaymentInfo p_payment)
		{
			Product productById = ProductManager.Current.GetProductById(p_productIdentifier);
			if (productById != null)
			{
				productById.Activate();
				productById.UpdateAvaliable();
				DataLocal.Current.Save(true);
				DataLocal.Current.SaveLocalBackup();
				bool flag = TimeManager.UTCTime - _StartPurchaseTime > 3000;
				if (!flag && p_payment != null)
				{
					p_payment.IsCheating = true;
					PaymentController.CheatingCount++;
				}
				ArgsDict argsDict = new ArgsDict();
				argsDict.Add("type", (!flag) ? "complete_chit" : "complete");
				argsDict.Add("product", GetCleanProductId(p_productIdentifier));
				argsDict.Add("error", string.Empty);
				argsDict.Add("p_info", p_payment);
				ArgsDict args = argsDict;
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Payment, args);
				if (flag && p_payment != null)
				{
					ServerProvider.Instance.PurchaseAction(p_payment, productById.PriceInUSD.ToString());
				}
				GoogleAnalyticsV4.getInstance().LogEvent("Payment", "Purchase", p_productIdentifier, 1L);
				if (OnPurchaseSuccess != null)
				{
					OnPurchaseSuccess(productById);
				}
			}
		}

		public void RestorePaymentReward(string p_productId)
		{
			Product productById = ProductManager.Current.GetProductById(p_productId);
			RestorePaymentReward(productById);
		}

		public void RestorePaymentReward(Product p_product)
		{
			if (p_product != null)
			{
				p_product.Activate();
				p_product.UpdateAvaliable();
				if (OnPurchaseSuccess != null)
				{
					OnPurchaseSuccess(p_product);
				}
			}
		}

		public void PaymentFailed(string p_error)
		{
			Debug.Log("[Payment_Abstract] purchaseFailedEvent: " + p_error);
			if (OnPurchaseFailed != null)
			{
				OnPurchaseFailed(p_error);
			}
			ArgsDict argsDict = new ArgsDict();
			argsDict.Add("type", "fail");
			argsDict.Add("product", GetCleanProductId(_LastProductID));
			argsDict.Add("error", p_error);
			ArgsDict args = argsDict;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Payment, args);
			GoogleAnalyticsV4.getInstance().LogEvent("Payment", "PurchaseFailed", _LastProductID + ": " + p_error, 1L);
		}

		public void PaymentTryLater(string p_productIdentifier)
		{
			if (OnTryPaymentLater != null)
			{
				Product productById = ProductManager.Current.GetProductById(p_productIdentifier);
				OnTryPaymentLater(productById.Icon);
			}
			ArgsDict argsDict = new ArgsDict();
			argsDict.Add("type", "try_later");
			argsDict.Add("product", GetCleanProductId(p_productIdentifier));
			argsDict.Add("error", string.Empty);
			ArgsDict args = argsDict;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Payment, args);
			GoogleAnalyticsV4.getInstance().LogEvent("Payment", "TryLater", p_productIdentifier, 1L);
		}

		public void CheatPayment(string p_productIdentifier)
		{
			if (OnCheatPurchase != null)
			{
				Product productById = ProductManager.Current.GetProductById(p_productIdentifier);
				OnCheatPurchase(productById.Icon);
			}
			ArgsDict argsDict = new ArgsDict();
			argsDict.Add("type", "cheat");
			argsDict.Add("product", GetCleanProductId(p_productIdentifier));
			argsDict.Add("error", string.Empty);
			ArgsDict args = argsDict;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Payment, args);
			GoogleAnalyticsV4.getInstance().LogEvent("Payment", "Cheat", p_productIdentifier, 1L);
		}

		public void CancelPayment(string p_error)
		{
			if (OnPurchaseCanceled != null)
			{
				OnPurchaseCanceled(p_error);
			}
			ArgsDict argsDict = new ArgsDict();
			argsDict.Add("type", "cancel");
			argsDict.Add("product", GetCleanProductId(_LastProductID));
			argsDict.Add("error", p_error);
			ArgsDict args = argsDict;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Payment, args);
			GoogleAnalyticsV4.getInstance().LogEvent("Payment", "Cancel", _LastProductID + ": " + p_error, 1L);
		}

		private static string GetCleanProductId(string p_productId)
		{
			return (p_productId == null) ? null : p_productId.ToLower().Replace("_2", string.Empty);
		}
	}
}
