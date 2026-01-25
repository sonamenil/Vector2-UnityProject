using System.Collections.Generic;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Vector.Core.User;
using Prime31;
using UnityEngine;

namespace Nekki.Vector.Core.Payment
{
	public class Payment_Android : PaymentAbstract
	{
		private const int BILLING_RESPONSE_RESULT_OK = 0;

		private const int BILLING_RESPONSE_RESULT_USER_CANCELED = 1;

		private const int BILLING_RESPONSE_RESULT_SERVICE_UNAVAILABLE = 2;

		private const int BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE = 3;

		private const int BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE = 4;

		private const int BILLING_RESPONSE_RESULT_DEVELOPER_ERROR = 5;

		private const int BILLING_RESPONSE_RESULT_ERROR = 6;

		private const int BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED = 7;

		private const int BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED = 8;

		private static readonly ObscuredString _PublicKeyPaid = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtyjsNY1EARUar+vnHRzSUEfv6tRv2CIo0zVwXX9mzIB9Ob3cYN2Tmri7jCD6WBIENomlar+WjM2r08UkauBOuCyTM9nI5bwHuVNF0ZlT473ahSfUCjQdk36ORS0x8yC52cKZlXS2sf2zUmxz8VXKYCJK0RN+WxTGkvSPpF7WINTsFz6WvMdKduAoFnMMI11XT9jUjKRF+L5d39ZKulSFJNI7qt+33ABQiShDg9SZPaJ6sWOFFaxL9VBIllLeuHHv6gfmK2N9TTfFBJ4WbekRo1SYGoHon5jAEO4r3ZR6DKIr2p2BHJpIHTae+Xa0y+02Cif0QqZ7QmBGOvt3xARZcwIDAQAB";

		private static readonly ObscuredString _PublicKeyFree = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnWuKb6VbpSCb5AWmEbcFNYHVlxNbgCxRShLd0SWrhVihoB3LkaHfcFSmA9JK6qtnpjBHDPD/vEJBlYWm/38yEtxqPAiURpY/s2M9wp0A7l53/DCjKg0dJhPKSPykfcqrXJDBzd7ysYXGibUNq95CsXXnwURLRFbhgo96E711vnIY4qxKDnOxWkzKL7KL2GhtrXciS+swqybbG7tvIBK41ddKM8lJ8+BdPQRXHQk47EhPi4ukRBPScBrJ/bOoTcEbQOl4wMOII589ndRLT5FdGvAcMfOSDz5sm6/OkuaSlwwExu1EsJ9sXUos3x/cTDg54e7BID+SudqpI+VKWQqWBQIDAQAB";

		private bool _CanMakePurchases;

		private Product _PurchaseProduct;

		private bool _PurchaseProductFailedOnConsume;

		private bool _RestorePurchasesRequest;

		private static ObscuredString PublicKey
		{
			get
			{
				return (!ApplicationController.IsPaidBundleID) ? _PublicKeyFree : _PublicKeyPaid;
			}
		}

		public override void Init()
		{
			//Debug.Log("[Payment_Android] init!");
			//GoogleIAB.init(PublicKey);
			//GoogleIAB.setAutoVerifySignatures(true);
			//GoogleIAB.enableLogging(true);
			//GoogleIABManager.billingSupportedEvent += CB_billingSupportedEvent;
			//GoogleIABManager.billingNotSupportedEvent += CB_billingNotSupportedEvent;
			//GoogleIABManager.queryInventorySucceededEvent += CB_productListReceivedEvent;
			//GoogleIABManager.queryInventoryFailedEvent += CB_productListRequestFailedEvent;
			//GoogleIABManager.purchaseSucceededEvent += CB_purchaseSuccessfulEvent;
			//GoogleIABManager.purchaseFailedEvent += CB_purchaseFailedEvent;
			//GoogleIABManager.consumePurchaseSucceededEvent += CB_consumePurchaseSuccessfulEvent;
			//GoogleIABManager.consumePurchaseFailedEvent += CB_consumePurchaseFailedEvent;
		}

		public override void Free()
		{
			//GoogleIABManager.billingSupportedEvent -= CB_billingSupportedEvent;
			//GoogleIABManager.billingNotSupportedEvent -= CB_billingNotSupportedEvent;
			//GoogleIABManager.queryInventorySucceededEvent -= CB_productListReceivedEvent;
			//GoogleIABManager.queryInventoryFailedEvent -= CB_productListRequestFailedEvent;
			//GoogleIABManager.purchaseSucceededEvent -= CB_purchaseSuccessfulEvent;
			//GoogleIABManager.purchaseFailedEvent -= CB_purchaseFailedEvent;
			//GoogleIABManager.consumePurchaseSucceededEvent -= CB_consumePurchaseSuccessfulEvent;
			//GoogleIABManager.consumePurchaseFailedEvent -= CB_consumePurchaseFailedEvent;
			//GoogleIAB.unbindService();
			//Debug.Log("[Payment_Android] free!");
		}

		public override bool CanMakePurchaces()
		{
			return _CanMakePurchases;
		}

		public override void GetProductsData(params string[] p_productIds)
		{
			//_RestorePurchasesRequest = false;
			//GoogleIAB.queryInventory(p_productIds);
		}

		public override void MakePurchase(string p_productId)
		{
			//base.MakePurchase(p_productId);
			//_PurchaseProduct = ProductManager.Current.GetProductById(p_productId);
			//_PurchaseProductFailedOnConsume = false;
			//GoogleIAB.purchaseProduct(p_productId);
		}

		public override void RestorePurchases()
		{
			//if (!PaymentController.IsRestorePurchasesNeed)
			//{
			//	if (OnRestorePurchasesSuccess != null)
			//	{
			//		OnRestorePurchasesSuccess();
			//	}
			//	return;
			//}
			//string[] array = ProductManager.Current.GetNonConsumableIds().ToArray();
			//StringBuilder stringBuilder = new StringBuilder();
			//stringBuilder.AppendLine("RestorePurchases for Ids:");
			//string[] array2 = array;
			//foreach (string value in array2)
			//{
			//	stringBuilder.AppendLine(value);
			//}
			//Debug.Log(stringBuilder.ToString());
			//_RestorePurchasesRequest = true;
			//GoogleIAB.queryInventory(array);
		}

		public override void RefreshReceipt()
		{
		}

		private void CB_billingSupportedEvent()
		{
			_CanMakePurchases = true;
			Debug.Log("[Payment_Android] billingSupportedEvent");
		}

		private void CB_billingNotSupportedEvent(string p_error)
		{
			_CanMakePurchases = false;
			Debug.Log("[Payment_Android] billingNotSupportedEvent: " + p_error);
		}

		//private void CB_productListReceivedEvent(List<GooglePurchase> p_purchasesHistory, List<GoogleSkuInfo> p_products)
		//{
		//	if (_RestorePurchasesRequest)
		//	{
		//		_RestorePurchasesRequest = false;
		//		StringBuilder stringBuilder = new StringBuilder();
		//		stringBuilder.AppendLine("PurchasesHistory:");
		//		foreach (GooglePurchase item in p_purchasesHistory)
		//		{
		//			stringBuilder.AppendLine(item.ToString());
		//		}
		//		Debug.Log(stringBuilder.ToString());
		//		stringBuilder.Length = 0;
		//		stringBuilder.AppendLine("[Payment_Android] restorePurchasesReceivedEvent: ");
		//		int num = 0;
		//		foreach (GooglePurchase item2 in p_purchasesHistory)
		//		{
		//			Product productById = ProductManager.Current.GetProductById(item2.productId);
		//			if (item2.purchaseState == GooglePurchase.GooglePurchaseState.Purchased && !productById.IsConsumable && !PaymentController.IsTransactionInProgress(item2.productId, item2.orderId) && !PaymentController.IsTransactionComplete(item2.productId, item2.orderId))
		//			{
		//				stringBuilder.AppendLine(string.Format("ProductId={0}, PaymentId={1}, Receipt={2}, Signature={3}", item2.productId, item2.orderId, item2.originalJson, (item2.signature == null) ? string.Empty : item2.signature));
		//				PaymentController.AddCompletedVerified(item2.productId, item2.orderId, item2.purchaseTime.ToString(), true, false);
		//				PaymentController.AddMoneySpent(item2.productId);
		//				RestorePaymentReward(productById);
		//				num++;
		//			}
		//		}
		//		if (num > 0)
		//		{
		//			DataLocal.Current.Save(true);
		//			DataLocal.Current.SaveLocalBackup();
		//			Debug.Log("UserSave");
		//		}
		//		Debug.Log(stringBuilder.ToString());
		//		if (OnRestorePurchasesSuccess != null)
		//		{
		//			OnRestorePurchasesSuccess();
		//		}
		//		return;
		//	}
		//	Debug.Log("[Payment_Android] productListReceivedEvent: " + p_products);
		//	if (OnProductsListRequestSuccess == null)
		//	{
		//		return;
		//	}
		//	List<Product> list = new List<Product>();
		//	foreach (GoogleSkuInfo p_product in p_products)
		//	{
		//		list.Add(new Product(p_product.productId, p_product.title, p_product.description, p_product.price, p_product.priceCurrencyCode));
		//	}
		//	OnProductsListRequestSuccess(list);
		//}

		private void CB_productListRequestFailedEvent(string p_error)
		{
			if (_RestorePurchasesRequest)
			{
				_RestorePurchasesRequest = false;
				Debug.Log("[Payment_Android] restorePurchasesFailedEvent: " + p_error);
				if (OnRestorePurchasesFailed != null)
				{
					OnRestorePurchasesFailed(p_error);
				}
			}
			else
			{
				Debug.Log("[Payment_Android] productListRequestFailedEvent: " + p_error);
				if (OnProductsListRequestFalied != null)
				{
					OnProductsListRequestFalied(p_error);
				}
			}
		}

		//private void CB_purchaseSuccessfulEvent(GooglePurchase p_transaction)
		//{
		//	Debug.Log("[Payment_Android] purchaseSuccessfulEvent: " + p_transaction);
		//	PurchaseSucceed(p_transaction);
		//	if (_PurchaseProduct.IsConsumable)
		//	{
		//		GoogleIAB.consumeProduct(p_transaction.productId);
		//	}
		//}

		//private void CB_purchaseFailedEvent(string p_error, int p_response)
		//{
		//	if (p_response == 1 || p_response < 0)
		//	{
		//		CB_purchaseCancelledEvent(p_error);
		//	}
		//	else if (p_response == 7)
		//	{
		//		if (_PurchaseProduct.IsConsumable)
		//		{
		//			_PurchaseProductFailedOnConsume = true;
		//			GoogleIAB.consumeProduct(_PurchaseProduct.Id);
		//		}
		//		else
		//		{
		//			RestorePurchases();
		//		}
		//	}
		//	else
		//	{
		//		PaymentFailed(p_error);
		//	}
		//}

		private void CB_purchaseCancelledEvent(string p_error)
		{
			Debug.Log("[Payment_Android] purchaseCancelledEvent: " + p_error);
			CancelPayment(p_error);
		}

		//private void CB_consumePurchaseSuccessfulEvent(GooglePurchase p_transaction)
		//{
		//	if (_PurchaseProductFailedOnConsume)
		//	{
		//		MakePurchase(_PurchaseProduct.Id);
		//	}
		//}

		private void CB_consumePurchaseFailedEvent(string p_error)
		{
		}

		//private void PurchaseSucceed(GooglePurchase p_transaction)
		//{
		//	PaymentVerificationManager.Verify(p_transaction.productId, p_transaction.orderId, p_transaction.originalJson, p_transaction.signature, false);
		//}
	}
}
