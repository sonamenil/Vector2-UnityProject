using System.Xml;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Payment.ProductEffects
{
	public class ProductEffect_MakePaid : ProductEffect
	{
		public const string TypeName = "MakePaid";

		public override bool NeedRestart
		{
			get
			{
				return false;
			}
		}

		public ProductEffect_MakePaid(XmlNode p_node)
			: base(p_node)
		{
			_Type = ProductEffectType.MakePaid;
		}

		public override void Activate()
		{
			DataLocal.Current.IsPaidVersion = true;
		}
	}
}
