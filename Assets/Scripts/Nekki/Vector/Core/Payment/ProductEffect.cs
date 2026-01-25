using System.Xml;
using Nekki.Vector.Core.Payment.ProductEffects;

namespace Nekki.Vector.Core.Payment
{
	public abstract class ProductEffect
	{
		protected ProductEffectType _Type;

		protected bool _IsVusual;

		public ProductEffectType Type
		{
			get
			{
				return _Type;
			}
		}

		public bool IsCurrency
		{
			get
			{
				return _Type == ProductEffectType.Currency;
			}
		}

		public bool IsItem
		{
			get
			{
				return _Type == ProductEffectType.Item;
			}
		}

		public bool IsMakePaid
		{
			get
			{
				return _Type == ProductEffectType.MakePaid;
			}
		}

		public bool IsVisual
		{
			get
			{
				return _IsVusual;
			}
		}

		public virtual bool NeedRestart
		{
			get
			{
				return false;
			}
		}

		public ProductEffect(XmlNode p_node)
		{
			_IsVusual = XmlUtils.ParseBool(p_node.Attributes["Visual"], true);
		}

		public static ProductEffect Create(XmlNode p_node)
		{
			switch (XmlUtils.ParseString(p_node.Attributes["Type"], string.Empty))
			{
			case "Currency":
				return new ProductEffect_Currency(p_node);
			case "Item":
			case "Preset":
				return new ProductEffect_Item(p_node);
			case "MakePaid":
				return new ProductEffect_MakePaid(p_node);
			default:
				return null;
			}
		}

		public T As<T>() where T : ProductEffect
		{
			return this as T;
		}

		public abstract void Activate();
	}
}
