using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;

namespace Nekki.Vector.Core.Payment
{
	public class Product
	{
		public const string Group_Currency = "Currency";

		public const string Group_Promo = "Promo";

		public const string Group_Premium = "Premium";

		public const string Group_Boosterpacks = "Boosterpacks";

		public const string Group_Ads = "Ads";

		public string Id { get; private set; }

		public bool IsConsumable { get; private set; }

		public string Group { get; private set; }

		public string Icon { get; private set; }

		public float PriceInUSD { get; private set; }

		public bool IsFree { get; private set; }

		public string TimerName { get; private set; }

		public string SaleText { get; private set; }

		public string LockCounter { get; private set; }

		public string Title { get; private set; }

		public string Description { get; private set; }

		public string Price { get; private set; }

		public string PriceSymbol { get; private set; }

		public bool IsAvaliable { get; private set; }

		public List<ProductEffect> Effects { get; private set; }

		public int EffectsCount
		{
			get
			{
				return (Effects != null) ? Effects.Count : 0;
			}
		}

		public bool IsCurrency
		{
			get
			{
				return Group == "Currency";
			}
		}

		public bool IsBoosterPack
		{
			get
			{
				return Group == "Boosterpacks";
			}
		}

		public bool IsPromo
		{
			get
			{
				return Group == "Promo";
			}
		}

		public bool IsPremium
		{
			get
			{
				return Group == "Premium";
			}
		}

		public bool IsAds
		{
			get
			{
				return Group == "Ads";
			}
		}

		public bool CanShow
		{
			get
			{
				if (LockCounter == null)
				{
					return true;
				}
				return CounterController.Current.ProductIsUnlock(LockCounter);
			}
		}

		public bool IsNeedRestart
		{
			get
			{
				if (Effects == null)
				{
					return false;
				}
				foreach (ProductEffect effect in Effects)
				{
					if (effect.NeedRestart)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Product(XmlNode p_node)
		{
			Id = XmlUtils.ParseString(p_node.Attributes["Id"], string.Empty);
			IsConsumable = XmlUtils.ParseBool(p_node.Attributes["IsConsumable"], true);
			Group = XmlUtils.ParseString(p_node.Attributes["Group"], "Unknown");
			Icon = XmlUtils.ParseString(p_node.Attributes["Icon"], string.Empty);
			Title = XmlUtils.ParseString(p_node.Attributes["Title"], string.Empty);
			Description = XmlUtils.ParseString(p_node.Attributes["Description"], string.Empty);
			TimerName = XmlUtils.ParseString(p_node.Attributes["Timer"]);
			Price = XmlUtils.ParseString(p_node.Attributes["Price"], string.Empty);
			PriceSymbol = XmlUtils.ParseString(p_node.Attributes["PriceSymbol"], string.Empty);
			PriceInUSD = (string.IsNullOrEmpty(Price) ? 0f : float.Parse(Price));
			IsFree = string.IsNullOrEmpty(Price);
			SaleText = XmlUtils.ParseString(p_node.Attributes["SaleText"]);
			LockCounter = XmlUtils.ParseString(p_node.Attributes["LockCounter"]);
			IsAvaliable = true;
			if (DeviceInformation.IsAndroid)
			{
				Price = Price + " " + PriceSymbol;
			}
			Effects = LoadEffects(p_node);
		}

		public Product(string p_id, string p_title, string p_description, string p_price, string p_priceSymbol)
		{
			Id = p_id;
			Title = p_title;
			Description = p_description;
			Price = p_price;
			PriceSymbol = p_priceSymbol;
		}

		private List<ProductEffect> LoadEffects(XmlNode p_node)
		{
			List<ProductEffect> list = new List<ProductEffect>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				ProductEffect productEffect = ProductEffect.Create(childNode);
				if (productEffect == null)
				{
					DebugUtils.Log("[Payment]: unknown effect in product - " + ToString());
				}
				else
				{
					list.Add(productEffect);
				}
			}
			if (list.Count == 0)
			{
				DebugUtils.Log("[Payment]: no valid effects in product - " + ToString());
				return null;
			}
			return list;
		}

		public void Activate()
		{
			if (Effects == null)
			{
				return;
			}
			foreach (ProductEffect effect in Effects)
			{
				effect.Activate();
			}
		}

		public void ReplaceData(Product p_other)
		{
			Title = p_other.Title;
			Description = p_other.Description;
			Price = p_other.Price.Replace('\u00a0', ' ');
			PriceSymbol = p_other.PriceSymbol;
		}

		public void UpdateAvaliable()
		{
			IsAvaliable = IsConsumable || !PaymentController.IsTransactionComplete(Id);
		}

		public override string ToString()
		{
			return string.Format("[Product]: Id = {0}, IsConsumable={1}, Group = {2}, Icon = {3}, Title = {4}, Description = {5}, Price = {6}, CurrencySymbol = {7}", Id, IsConsumable, Group, Icon, Title, Description, Price, PriceSymbol);
		}
	}
}
