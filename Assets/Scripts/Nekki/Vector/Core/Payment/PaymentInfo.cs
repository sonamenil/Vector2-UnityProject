namespace Nekki.Vector.Core.Payment
{
	public class PaymentInfo
	{
		private enum VerificationStatus
		{
			Verified = 0,
			NotVerified = 1,
			Faled = 2
		}

		public int RetryAttempts;

		private VerificationStatus _Status;

		private bool _IsConfirmed;

		private string _ProductID;

		private string _PaymentID;

		private string _Receipt;

		private string _Signature;

		private string _PaymentDate;

		private bool _IsCheating;

		private bool _IsRestored;

		public bool IsConfirmed
		{
			get
			{
				return _IsConfirmed;
			}
		}

		public string ProductID
		{
			get
			{
				return _ProductID;
			}
		}

		public string PaymentID
		{
			get
			{
				return _PaymentID;
			}
		}

		public string Receipt
		{
			get
			{
				return _Receipt;
			}
		}

		public string Decode64Receipt
		{
			get
			{
				return StringUtils.FromBase64(_Receipt);
			}
		}

		public string Signature
		{
			get
			{
				return _Signature;
			}
		}

		public string PaymentDate
		{
			get
			{
				return _PaymentDate;
			}
			set
			{
				_PaymentDate = value;
			}
		}

		public bool IsVerified
		{
			get
			{
				return _Status == VerificationStatus.Verified;
			}
		}

		public bool IsNotVerified
		{
			get
			{
				return _Status == VerificationStatus.NotVerified;
			}
		}

		public bool IsVerificationFailed
		{
			get
			{
				return _Status == VerificationStatus.Faled;
			}
		}

		public bool IsCheating
		{
			get
			{
				return _IsCheating;
			}
			set
			{
				_IsCheating = value;
			}
		}

		public bool IsRestored
		{
			get
			{
				return _IsRestored;
			}
			set
			{
				_IsRestored = value;
			}
		}

		private PaymentInfo(string p_productID, string p_paymentID, string p_receipt)
		{
			_ProductID = p_productID;
			_PaymentID = p_paymentID;
			_Receipt = p_receipt;
			_IsCheating = false;
		}

		public static PaymentInfo CreateNotVerified(string p_productID, string p_paymentID, string p_receipt, string p_signature)
		{
			PaymentInfo paymentInfo = new PaymentInfo(p_productID, p_paymentID, p_receipt);
			paymentInfo._Signature = p_signature;
			paymentInfo._Status = VerificationStatus.NotVerified;
			paymentInfo._IsConfirmed = false;
			return paymentInfo;
		}

		public static PaymentInfo CreateVerified_NotConfirmed(string p_productID, string p_paymentID, string p_receipt, string p_signature, string p_date)
		{
			PaymentInfo paymentInfo = new PaymentInfo(p_productID, p_paymentID, p_receipt);
			paymentInfo._Signature = p_signature;
			paymentInfo._PaymentDate = p_date;
			paymentInfo._Status = VerificationStatus.Verified;
			paymentInfo._IsConfirmed = false;
			return paymentInfo;
		}

		public static PaymentInfo CreateCompletedVerified(string p_productID, string p_paymentID, string p_date)
		{
			PaymentInfo paymentInfo = new PaymentInfo(p_productID, p_paymentID, null);
			paymentInfo._PaymentDate = p_date;
			paymentInfo._Status = VerificationStatus.Verified;
			paymentInfo._IsConfirmed = true;
			return paymentInfo;
		}

		public static PaymentInfo CreateCompletedNotVerified(string p_productID, string p_paymentID)
		{
			PaymentInfo paymentInfo = new PaymentInfo(p_productID, p_paymentID, null);
			paymentInfo._Status = VerificationStatus.NotVerified;
			paymentInfo._IsConfirmed = false;
			return paymentInfo;
		}

		public static PaymentInfo CreateCompletedFailed(string p_productID, string p_paymentID)
		{
			PaymentInfo paymentInfo = new PaymentInfo(p_productID, p_paymentID, null);
			paymentInfo._Status = VerificationStatus.Faled;
			paymentInfo._IsConfirmed = false;
			return paymentInfo;
		}

		public void VerifyFaled()
		{
			_Status = VerificationStatus.Faled;
			_IsConfirmed = false;
		}

		public void VerifyComplete()
		{
			_Status = VerificationStatus.Verified;
			_IsConfirmed = false;
		}

		public void ComfirmedComplete()
		{
			_IsConfirmed = true;
		}

		public override string ToString()
		{
			return string.Format("[PaymentInfo: IsConfirmed={0}, ProductID={1}, PaymentID={2}, IsVerified={3}, IsVerificationFailed={4}, IsCheating={5}]", IsConfirmed, ProductID, PaymentID, IsVerified, IsVerificationFailed, IsCheating);
		}
	}
}
