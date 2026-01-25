using System.Collections.Generic;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class PromoEffectDialogContent : DialogContent
	{
		[SerializeField]
		private LabelAlias _ProductDescription;

		public void Init()
		{
			List<DialogButtonData> list = new List<DialogButtonData>();
			list.Add(new DialogButtonData(OnCloseTap, "^GUI.Buttons.Close^", ButtonUI.Type.Blue));
			List<Product> productsByGroup = ProductManager.Current.GetProductsByGroup("Promo");
			_ProductDescription.SetAlias(productsByGroup[0].Description);
			Init(list);
		}

		public void OnCloseTap(BaseDialog dialog)
		{
			dialog.Dismiss();
		}

		public void OnTimerExpire()
		{
			base.Parent.Dismiss();
		}
	}
}
