using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Payment
{
	public static class PaymentController
	{
		private static PaymentAbstract _Current = null;

		private static float _MoneySpent = 0f;

		private static int _CheatingCount = 0;

		private static List<PaymentInfo> _PaymentsCompleted = new List<PaymentInfo>();

		private static List<PaymentInfo> _PaymentsInprogress = new List<PaymentInfo>();

		public static PaymentAbstract Current
		{
			get
			{
				return _Current;
			}
		}

		public static bool IsRestorePurchasesNeed
		{
			get
			{
				List<Product> nonConsumableProducts = ProductManager.Current.GetNonConsumableProducts();
				foreach (Product item in nonConsumableProducts)
				{
					if (!IsTransactionComplete(item.Id))
					{
						return true;
					}
				}
				return false;
			}
		}

		public static float MoneySpent
		{
			get
			{
				return _MoneySpent;
			}
		}

		public static int CheatingCount
		{
			get
			{
				return _CheatingCount;
			}
			set
			{
				_CheatingCount = value;
			}
		}

		public static bool IsUserMakePayments
		{
			get
			{
				if (_PaymentsCompleted.Count == 0)
				{
					return false;
				}
				foreach (PaymentInfo item in _PaymentsCompleted)
				{
					if (item.IsVerified)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static void Init()
		{
			_Current = new Payment_Android();
			_Current.Init();
		}

		public static void Free()
		{
			_Current.Free();
		}

		public static bool CanMakePurchaces()
		{
			return _Current != null && _Current.CanMakePurchaces();
		}

		public static void AddMoneySpent(string p_productId)
		{
			Product productById = ProductManager.Current.GetProductById(p_productId);
			if (productById == null)
			{
				DebugUtils.Log("[PaymentControler]: AddMoneySpent for unknown product - " + p_productId);
			}
			else
			{
				_MoneySpent += productById.PriceInUSD;
			}
		}

		public static void RestoreStatisticsData()
		{
			foreach (PaymentInfo item in _PaymentsInprogress)
			{
				if (item.IsCheating)
				{
					CheatingCount++;
				}
			}
			foreach (PaymentInfo item2 in _PaymentsCompleted)
			{
				if (item2.IsCheating)
				{
					CheatingCount++;
				}
				if (item2.IsVerified || item2.IsNotVerified)
				{
					AddMoneySpent(item2.ProductID);
				}
			}
		}

		public static void RetryVerification()
		{
			for (int i = 0; i < _PaymentsInprogress.Count; i++)
			{
				PaymentInfo paymentInfo = _PaymentsInprogress[i];
				if (paymentInfo.IsVerified)
				{
					PaymentVerificationManager.ReConfirm(paymentInfo);
				}
				else
				{
					PaymentVerificationManager.ReVerify(paymentInfo);
				}
			}
		}

		public static PaymentInfo AddNotVerifiedPayment(string p_productID, string p_paymentID, string p_receipt, string p_signature, bool p_isRestored)
		{
			PaymentInfo paymentInfo = PaymentInfo.CreateNotVerified(p_productID, p_paymentID, p_receipt, p_signature);
			paymentInfo.IsRestored = p_isRestored;
			_PaymentsInprogress.Add(paymentInfo);
			SaveAllData();
			return paymentInfo;
		}

		public static PaymentInfo AddComleteNotVerefied(string p_productID, string p_paymentID, bool p_isRestored)
		{
			PaymentInfo paymentInfo = PaymentInfo.CreateCompletedNotVerified(p_productID, p_paymentID);
			paymentInfo.IsRestored = p_isRestored;
			_PaymentsCompleted.Add(paymentInfo);
			AddMoneySpent(p_productID);
			SaveAllData();
			return paymentInfo;
		}

		public static PaymentInfo AddCompletedVerified(string p_productID, string p_paymentID, string p_date, bool p_isRestored, bool p_saveUser = true)
		{
			PaymentInfo paymentInfo = PaymentInfo.CreateCompletedVerified(p_productID, p_paymentID, p_date);
			paymentInfo.IsRestored = p_isRestored;
			_PaymentsCompleted.Add(paymentInfo);
			if (p_saveUser)
			{
				SaveAllData();
			}
			return paymentInfo;
		}

		public static void VerifyFaled(PaymentInfo p_payment)
		{
			_PaymentsInprogress.Remove(p_payment);
			p_payment.VerifyFaled();
			_PaymentsCompleted.Add(p_payment);
			SaveAllData();
		}

		public static void VerifyComplete(PaymentInfo p_payment)
		{
			p_payment.VerifyComplete();
			SaveAllData();
		}

		public static void PaymentCompleteWithoutVerification(PaymentInfo p_payment)
		{
			_PaymentsInprogress.Remove(p_payment);
			_PaymentsCompleted.Add(p_payment);
			SaveAllData();
		}

		public static void PaymentConfirmed(PaymentInfo p_payment)
		{
			_PaymentsInprogress.Remove(p_payment);
			p_payment.ComfirmedComplete();
			_PaymentsCompleted.Add(p_payment);
			AddMoneySpent(p_payment.ProductID);
			SaveAllData();
		}

		public static bool IsPaymentComplete(PaymentInfo p_payment)
		{
			for (int i = 0; i < _PaymentsInprogress.Count; i++)
			{
				if (_PaymentsInprogress[i].PaymentID == p_payment.PaymentID)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsTransactionInProgress(string p_productID, string p_paymentID = null)
		{
			foreach (PaymentInfo item in _PaymentsInprogress)
			{
				if (item.ProductID == p_productID && (p_paymentID == null || item.PaymentID == p_paymentID))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsTransactionComplete(string p_productID, string p_paymentID = null)
		{
			foreach (PaymentInfo item in _PaymentsCompleted)
			{
				if (item.ProductID == p_productID && (p_paymentID == null || item.PaymentID == p_paymentID))
				{
					return true;
				}
			}
			return false;
		}

		private static void SaveAllData()
		{
			DataLocal.Current.Save(true);
			DataLocal.Current.SaveLocalBackup();
		}

		public static void SavePayments(XmlNode p_rootNode)
		{
			XmlNode xmlNode = null;
			if (_PaymentsCompleted.Count != 0)
			{
				xmlNode = p_rootNode.OwnerDocument.CreateElement("Payments");
				p_rootNode.AppendChild(xmlNode);
				XmlNode xmlNode2 = p_rootNode.OwnerDocument.CreateElement("Completed");
				xmlNode.AppendChild(xmlNode2);
				XmlElement xmlElement = null;
				PaymentInfo paymentInfo = null;
				for (int i = 0; i < _PaymentsCompleted.Count; i++)
				{
					paymentInfo = _PaymentsCompleted[i];
					xmlElement = xmlNode.OwnerDocument.CreateElement("Payment");
					xmlElement.SetAttribute("ID", paymentInfo.PaymentID);
					xmlElement.SetAttribute("ProductID", paymentInfo.ProductID);
					xmlElement.SetAttribute("Verified", (!paymentInfo.IsVerified) ? "0" : "1");
					if (paymentInfo.IsCheating)
					{
						xmlElement.SetAttribute("Cheating", "1");
					}
					if (paymentInfo.IsRestored)
					{
						xmlElement.SetAttribute("Restored", "1");
					}
					if (paymentInfo.IsVerified)
					{
						xmlElement.SetAttribute("Date", paymentInfo.PaymentDate);
					}
					else if (paymentInfo.IsVerificationFailed)
					{
						xmlElement.SetAttribute("VerificationFailed", "1");
					}
					xmlNode2.AppendChild(xmlElement);
				}
			}
			if (_PaymentsInprogress.Count == 0)
			{
				return;
			}
			if (xmlNode == null)
			{
				xmlNode = p_rootNode.OwnerDocument.CreateElement("Payments");
				p_rootNode.AppendChild(xmlNode);
			}
			XmlNode xmlNode3 = p_rootNode.OwnerDocument.CreateElement("Inprogress");
			xmlNode.AppendChild(xmlNode3);
			XmlElement xmlElement2 = null;
			PaymentInfo paymentInfo2 = null;
			for (int j = 0; j < _PaymentsInprogress.Count; j++)
			{
				paymentInfo2 = _PaymentsInprogress[j];
				xmlElement2 = xmlNode.OwnerDocument.CreateElement("Payment");
				xmlElement2.SetAttribute("ID", paymentInfo2.PaymentID);
				xmlElement2.SetAttribute("ProductID", paymentInfo2.ProductID);
				xmlElement2.SetAttribute("Receipt", paymentInfo2.Receipt);
				xmlElement2.SetAttribute("Verified", (!paymentInfo2.IsVerified) ? "0" : "1");
				if (paymentInfo2.IsCheating)
				{
					xmlElement2.SetAttribute("Cheating", "1");
				}
				if (paymentInfo2.IsRestored)
				{
					xmlElement2.SetAttribute("Restored", "1");
				}
				if (paymentInfo2.IsVerified)
				{
					xmlElement2.SetAttribute("Date", paymentInfo2.PaymentDate);
				}
				if (paymentInfo2.Signature != null)
				{
					xmlElement2.SetAttribute("Signature", paymentInfo2.Signature);
				}
				xmlNode3.AppendChild(xmlElement2);
			}
		}

		public static void LoadPayments(XmlNode p_rootNode)
		{
			ResetPayments();
			XmlNode xmlNode = p_rootNode["Payments"];
			if (xmlNode == null)
			{
				return;
			}
			XmlNode xmlNode2 = xmlNode["Inprogress"];
			XmlNode xmlNode3 = xmlNode["Completed"];
			if (xmlNode2 != null)
			{
				foreach (XmlNode item in xmlNode2)
				{
					string value = item.Attributes["ID"].Value;
					string value2 = item.Attributes["ProductID"].Value;
					string value3 = item.Attributes["Receipt"].Value;
					string p_signature = XmlUtils.ParseString(item.Attributes["Signature"]);
					bool flag = XmlUtils.ParseBool(item.Attributes["Verified"]);
					bool isCheating = XmlUtils.ParseBool(item.Attributes["Cheating"]);
					bool isRestored = XmlUtils.ParseBool(item.Attributes["Restored"]);
					PaymentInfo paymentInfo;
					if (flag)
					{
						string value4 = item.Attributes["Date"].Value;
						paymentInfo = PaymentInfo.CreateVerified_NotConfirmed(value2, value, value3, p_signature, value4);
					}
					else
					{
						paymentInfo = PaymentInfo.CreateNotVerified(value2, value, value3, p_signature);
					}
					paymentInfo.IsCheating = isCheating;
					paymentInfo.IsRestored = isRestored;
					_PaymentsInprogress.Add(paymentInfo);
				}
			}
			if (xmlNode3 == null)
			{
				return;
			}
			foreach (XmlNode item2 in xmlNode3)
			{
				string value5 = item2.Attributes["ID"].Value;
				string value6 = item2.Attributes["ProductID"].Value;
				bool flag2 = XmlUtils.ParseBool(item2.Attributes["Verified"]);
				bool isCheating2 = XmlUtils.ParseBool(item2.Attributes["Cheating"]);
				bool isRestored2 = XmlUtils.ParseBool(item2.Attributes["Restored"]);
				PaymentInfo paymentInfo;
				if (flag2)
				{
					string value7 = item2.Attributes["Date"].Value;
					paymentInfo = PaymentInfo.CreateCompletedVerified(value6, value5, value7);
				}
				else
				{
					paymentInfo = ((item2.Attributes["VerificationFailed"] == null) ? PaymentInfo.CreateCompletedNotVerified(value6, value5) : PaymentInfo.CreateCompletedFailed(value6, value5));
				}
				paymentInfo.IsCheating = isCheating2;
				paymentInfo.IsRestored = isRestored2;
				_PaymentsCompleted.Add(paymentInfo);
			}
		}

		private static void ResetPayments()
		{
			_MoneySpent = 0f;
			_CheatingCount = 0;
			_PaymentsInprogress.Clear();
			_PaymentsCompleted.Clear();
		}
	}
}
