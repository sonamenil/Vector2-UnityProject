using Nekki.Vector.Core.Payment;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs.Payment.Handlers
{
	public abstract class BaseHandler : MonoBehaviour
	{
		protected PaymentDialog _Parent;

		public virtual void Init(PaymentDialog p_parent)
		{
			_Parent = p_parent;
		}

		public virtual void Free()
		{
		}

		public abstract void UseProduct(Product p_product);
	}
}
