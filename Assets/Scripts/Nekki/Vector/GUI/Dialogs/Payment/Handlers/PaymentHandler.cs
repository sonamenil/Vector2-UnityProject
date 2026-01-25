using System;
using Nekki.Vector.Core.Payment;

namespace Nekki.Vector.GUI.Dialogs.Payment.Handlers
{
	public class PaymentHandler : BaseHandler
	{
		public override void Init(PaymentDialog p_parent)
		{
			base.Init(p_parent);
			PaymentAbstract current = PaymentController.Current;
			current.OnPurchaseSuccess = (Action<Product>)Delegate.Combine(current.OnPurchaseSuccess, new Action<Product>(OnPurchaseSuccess));
			PaymentAbstract current2 = PaymentController.Current;
			current2.OnPurchaseFailed = (Action<string>)Delegate.Combine(current2.OnPurchaseFailed, new Action<string>(OnPurchaseFailed));
			PaymentAbstract current3 = PaymentController.Current;
			current3.OnPurchaseCanceled = (Action<string>)Delegate.Combine(current3.OnPurchaseCanceled, new Action<string>(OnPurchaseCanceled));
			PaymentAbstract current4 = PaymentController.Current;
			current4.OnCheatPurchase = (Action<string>)Delegate.Combine(current4.OnCheatPurchase, new Action<string>(OnCheatPurchase));
			PaymentAbstract current5 = PaymentController.Current;
			current5.OnTryPaymentLater = (Action<string>)Delegate.Combine(current5.OnTryPaymentLater, new Action<string>(OnTryPaymentLater));
			PaymentAbstract current6 = PaymentController.Current;
			current6.OnRestorePurchasesSuccess = (Action)Delegate.Combine(current6.OnRestorePurchasesSuccess, new Action(OnRestorePurchasesSuccess));
			PaymentAbstract current7 = PaymentController.Current;
			current7.OnRestorePurchasesFailed = (Action<string>)Delegate.Combine(current7.OnRestorePurchasesFailed, new Action<string>(OnRestorePurchasesFailed));
			ProductManager current8 = ProductManager.Current;
			current8.OnProductsUpdate = (Action)Delegate.Combine(current8.OnProductsUpdate, new Action(OnProductsUpdate));
			ProductManager current9 = ProductManager.Current;
			current9.OnProductsUpdateFailed = (Action)Delegate.Combine(current9.OnProductsUpdateFailed, new Action(OnProductsUpdate));
			if (!ProductManager.Current.ProductWasRecived)
			{
				ProductManager.Current.GetProductsData();
			}
			ProductManager.Current.UpdateProductsAvaliable();
			PaymentController.RetryVerification();
		}

		public override void Free()
		{
			base.Free();
			PaymentAbstract current = PaymentController.Current;
			current.OnPurchaseSuccess = (Action<Product>)Delegate.Remove(current.OnPurchaseSuccess, new Action<Product>(OnPurchaseSuccess));
			PaymentAbstract current2 = PaymentController.Current;
			current2.OnPurchaseFailed = (Action<string>)Delegate.Remove(current2.OnPurchaseFailed, new Action<string>(OnPurchaseFailed));
			PaymentAbstract current3 = PaymentController.Current;
			current3.OnPurchaseCanceled = (Action<string>)Delegate.Remove(current3.OnPurchaseCanceled, new Action<string>(OnPurchaseCanceled));
			PaymentAbstract current4 = PaymentController.Current;
			current4.OnCheatPurchase = (Action<string>)Delegate.Remove(current4.OnCheatPurchase, new Action<string>(OnCheatPurchase));
			PaymentAbstract current5 = PaymentController.Current;
			current5.OnTryPaymentLater = (Action<string>)Delegate.Remove(current5.OnTryPaymentLater, new Action<string>(OnTryPaymentLater));
			PaymentAbstract current6 = PaymentController.Current;
			current6.OnRestorePurchasesSuccess = (Action)Delegate.Remove(current6.OnRestorePurchasesSuccess, new Action(OnRestorePurchasesSuccess));
			PaymentAbstract current7 = PaymentController.Current;
			current7.OnRestorePurchasesFailed = (Action<string>)Delegate.Remove(current7.OnRestorePurchasesFailed, new Action<string>(OnRestorePurchasesFailed));
			ProductManager current8 = ProductManager.Current;
			current8.OnProductsUpdate = (Action)Delegate.Remove(current8.OnProductsUpdate, new Action(OnProductsUpdate));
			ProductManager current9 = ProductManager.Current;
			current9.OnProductsUpdateFailed = (Action)Delegate.Remove(current9.OnProductsUpdateFailed, new Action(OnProductsUpdate));
		}

		public override void UseProduct(Product p_product)
		{
			PaymentController.Current.MakePurchase(p_product.Id);
		}

		private void OnPurchaseSuccess(Product p_product)
		{
			_Parent.ShowNotification("^Payment.Events.Success^");
			if (p_product.IsNeedRestart)
			{
				_Parent.ShowNotification("^DialogWindow.Text.PleaseRestart^");
			}
			if (!p_product.IsAvaliable)
			{
				_Parent.RemoveUIByProduct(p_product);
			}
			_Parent.Unblock();
		}

		private void OnPurchaseFailed(string p_error)
		{
			_Parent.ShowNotification("^Payment.Events.Failed^");
			_Parent.Unblock();
		}

		private void OnPurchaseCanceled(string p_error)
		{
			_Parent.ShowNotification("^Payment.Events.Failed^");
			_Parent.Unblock();
		}

		private void OnCheatPurchase(string p_iconName)
		{
			_Parent.ShowNotification("^Payment.Events.Cheater^");
			_Parent.Unblock();
		}

		private void OnTryPaymentLater(string p_iconName)
		{
			_Parent.ShowNotification("^Payment.Events.ServerError^");
			_Parent.Unblock();
		}

		private void OnRestorePurchasesSuccess()
		{
			_Parent.Unblock();
		}

		private void OnRestorePurchasesFailed(string p_error)
		{
			_Parent.ShowNotification("^Payment.Events.Failed^");
			_Parent.Unblock();
		}

		private void OnProductsUpdate()
		{
			_Parent.UpdateProducts();
		}
	}
}
