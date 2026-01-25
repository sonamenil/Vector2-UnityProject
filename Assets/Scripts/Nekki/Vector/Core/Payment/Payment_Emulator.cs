using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Payment
{
	public class Payment_Emulator : PaymentAbstract
	{
		private static float _SimulateConnectionTimeout = 2f;

		public override void Init()
		{
			Debug.Log("[Payment_Emulator] init!");
		}

		public override void Free()
		{
			Debug.Log("[Payment_Emulator] free!");
		}

		public override bool CanMakePurchaces()
		{
			return true;
		}

		public override void GetProductsData(params string[] p_productIds)
		{
			CB_productListReceivedEvent();
		}

		public override void MakePurchase(string p_productId)
		{
			base.MakePurchase(p_productId);
			Product productById = ProductManager.Current.GetProductById(p_productId);
			if (productById != null)
			{
				CB_purchaseSuccessfulEvent(productById);
			}
		}

		public override void RestorePurchases()
		{
			CB_restoreTransactionsFinishedEvent();
		}

		public override void RefreshReceipt()
		{
		}

		private void CB_purchaseSuccessfulEvent(Product p_product)
		{
			if (_SimulateConnectionTimeout > 0f)
			{
				CoroutineManager.Current.StartRoutine(SimulateDataRecievingRoutine(delegate
				{
					PurchaseProduct(p_product);
				}));
			}
			else
			{
				PurchaseProduct(p_product);
			}
		}

		private void CB_restoreTransactionsFinishedEvent()
		{
			if (_SimulateConnectionTimeout > 0f)
			{
				CoroutineManager.Current.StartRoutine(SimulateDataRecievingRoutine(RestoreTransactions));
			}
			else
			{
				RestoreTransactions();
			}
		}

		private void CB_productListReceivedEvent()
		{
			if (_SimulateConnectionTimeout > 0f)
			{
				CoroutineManager.Current.StartRoutine(SimulateDataRecievingRoutine(RecieveProductList));
			}
			else
			{
				RecieveProductList();
			}
		}

		private void PurchaseProduct(Product p_product)
		{
			Debug.Log("[Payment_Emulator] purchaseSuccessfulEvent: " + p_product);
			PaymentVerificationManager.Verify(p_product.Id, string.Empty, string.Empty, null, false);
		}

		private void RecieveProductList()
		{
			Debug.Log("[Payment_Emulator] productListReceivedEvent");
			if (OnProductsListRequestSuccess != null)
			{
				OnProductsListRequestSuccess(new List<Product>());
			}
		}

		private void RestoreTransactions()
		{
			Debug.Log("[Payment_Emulator] restoreTransactionsFinishedEvent");
			if (OnRestorePurchasesSuccess != null)
			{
				OnRestorePurchasesSuccess();
			}
		}

		private IEnumerator SimulateDataRecievingRoutine(Action p_action)
		{
			yield return new WaitForSeconds(_SimulateConnectionTimeout);
			p_action();
		}
	}
}
