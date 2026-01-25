using SimpleJSON;

namespace Nekki.Vector.Core.Payment
{
	public class PaymentVerificationManager
	{
		private const string _Platform_iOS = "iOS";

		private const string _Platform_Android = "Android";

		private const int verification_status_ok = 0;

		private const int verification_status_fail = 1;

		private const int MAX_RETRY_COUNT = 2;

		private static bool _IsActive = true;

		public static bool IsActive
		{
			get
			{
				return _IsActive;
			}
		}

		public static void SetActive(bool p_iOS, bool p_android)
		{
			_IsActive = !DeviceInformation.IsEmulator && ((!DeviceInformation.IsiOS) ? p_android : p_iOS);
		}

		public static void Verify(string p_productIdentifier, string p_paymentID, string p_receipt, string p_signature, bool p_isRestored)
		{
			PaymentInfo paymentInfo = null;
			if (!_IsActive || p_paymentID == null)
			{
				paymentInfo = PaymentController.AddComleteNotVerefied(p_productIdentifier, p_paymentID, p_isRestored);
				PaymentController.Current.GivePaymentRevard(p_productIdentifier, paymentInfo);
			}
			else
			{
				paymentInfo = PaymentController.AddNotVerifiedPayment(p_productIdentifier, p_paymentID, p_receipt, p_signature, p_isRestored);
				SendVerifyRequest(paymentInfo);
			}
		}

		public static void ReVerify(PaymentInfo p_payment)
		{
			if (!_IsActive)
			{
				PaymentController.Current.GivePaymentRevard(p_payment.ProductID, p_payment);
				PaymentController.PaymentCompleteWithoutVerification(p_payment);
			}
			SendVerifyRequest(p_payment);
		}

		public static void ReConfirm(PaymentInfo p_payment)
		{
			SendConfirmedRequest(p_payment);
		}

		private static void SendVerifyRequest(PaymentInfo p_payment)
		{
			if (DeviceInformation.IsiOS)
			{
				ServerProvider.Instance.VerifyPurchaseAction(p_payment, p_payment.Decode64Receipt, null, "iOS", VerifyRequest_Response);
			}
			if (DeviceInformation.IsAndroid)
			{
				ServerProvider.Instance.VerifyPurchaseAction(p_payment, p_payment.Receipt, p_payment.Signature, "Android", VerifyRequest_Response);
			}
		}

		private static void VerifyRequest_Response(bool p_result, string p_data, object p_userData)
		{
			DebugUtils.Log("VerifyRequest_Response");
			DebugUtils.Log((!p_result) ? "Result fail" : "Result ok!");
			DebugUtils.Log(p_data);
			PaymentInfo paymentInfo = (PaymentInfo)p_userData;
			if (!p_result)
			{
				if (paymentInfo.RetryAttempts <= 2)
				{
					paymentInfo.RetryAttempts++;
					SendVerifyRequest(paymentInfo);
				}
				else
				{
					paymentInfo.RetryAttempts = 0;
					PaymentController.Current.PaymentTryLater(paymentInfo.ProductID);
				}
				return;
			}
			JSONNode jSONNode = JSONNode.Parse(p_data);
			switch (jSONNode["status"].AsInt)
			{
			case 0:
				if (PaymentController.IsPaymentComplete(paymentInfo))
				{
					paymentInfo.RetryAttempts = 0;
					paymentInfo.IsCheating = true;
					PaymentController.CheatingCount++;
					PaymentController.Current.CheatPayment(paymentInfo.ProductID);
					break;
				}
				paymentInfo.PaymentDate = jSONNode["data"]["receiptPurchaseDate"].Value;
				PaymentController.Current.GivePaymentRevard(paymentInfo.ProductID, paymentInfo);
				PaymentController.VerifyComplete(paymentInfo);
				if (!paymentInfo.IsRestored)
				{
					paymentInfo.RetryAttempts = 0;
					SendConfirmedRequest(paymentInfo);
				}
				else
				{
					PaymentController.PaymentCompleteWithoutVerification(paymentInfo);
					DebugUtils.Log("Restore payments doesn't need to be confirmed!");
				}
				break;
			case 1:
				paymentInfo.IsCheating = true;
				PaymentController.CheatingCount++;
				PaymentController.VerifyFaled(paymentInfo);
				PaymentController.Current.CheatPayment(paymentInfo.ProductID);
				break;
			}
		}

		private static void SendConfirmedRequest(PaymentInfo p_payment)
		{
			if (DeviceInformation.IsiOS)
			{
				ServerProvider.Instance.ConfirmVerificationAction(p_payment, "iOS", CompletedRequest_Response);
			}
			if (DeviceInformation.IsAndroid)
			{
				ServerProvider.Instance.ConfirmVerificationAction(p_payment, "Android", CompletedRequest_Response);
			}
		}

		private static void CompletedRequest_Response(bool p_result, string p_data, object p_userData)
		{
			DebugUtils.Log("CompletedRequest_Response");
			DebugUtils.Log((!p_result) ? "Result fail" : "Result ok!");
			DebugUtils.Log(p_data);
			PaymentInfo paymentInfo = (PaymentInfo)p_userData;
			if (!p_result)
			{
				if (paymentInfo.RetryAttempts <= 2)
				{
					paymentInfo.RetryAttempts++;
					SendConfirmedRequest(paymentInfo);
				}
				else
				{
					paymentInfo.RetryAttempts = 0;
				}
			}
			else
			{
				PaymentController.PaymentConfirmed(paymentInfo);
			}
		}
	}
}
