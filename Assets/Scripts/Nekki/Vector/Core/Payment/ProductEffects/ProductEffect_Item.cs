using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Payment.ProductEffects
{
	public class ProductEffect_Item : ProductEffect
	{
		public const string TypeName_Item = "Item";

		public const string TypeName_Preset = "Preset";

		private Preset _Preset;

		private Variable _Quantity;

		private bool _ItemMode;

		public Preset Preset
		{
			get
			{
				return _Preset;
			}
		}

		public int Quantity
		{
			get
			{
				return _Quantity.ValueInt;
			}
		}

		public bool ItemMode
		{
			get
			{
				return _ItemMode;
			}
		}

		public string SpriteName
		{
			get
			{
				switch (_Preset.Name)
				{
				case "BaseBoosterpack":
					return "common.payment_boosterpack_icon";
				case "Coupon_CardsBoost":
					return "boost_coupons.coupon_cards_boost_blue";
				default:
					return string.Empty;
				}
			}
		}

		public ProductEffect_Item(XmlNode p_node)
			: base(p_node)
		{
			string text = XmlUtils.ParseString(p_node.Attributes["Preset"], string.Empty);
			_Type = ProductEffectType.Item;
			_Preset = PresetsManager.GetPresetByName(text);
			_Quantity = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Quantity"], "1"), null);
			_ItemMode = XmlUtils.ParseString(p_node.Attributes["Type"], "Item") == "Item";
			if (_Preset == null)
			{
				DebugUtils.LogError("[Payment]: Invalid preset in product effect: " + text);
			}
		}

		public override void Activate()
		{
			if (_Preset == null)
			{
				DebugUtils.LogError("[Payment]: Invalid preset in product effect!");
				return;
			}
			MainRandom.InitRandomIfNotYet();
			if (_ItemMode)
			{
				ActivateItem();
			}
			else
			{
				ActivatePreset();
			}
		}

		private void ActivateItem()
		{
			for (int i = 0; i < _Quantity.ValueInt; i++)
			{
				_Preset.RunPreset();
			}
		}

		private void ActivatePreset()
		{
			_Preset.RunPreset();
		}
	}
}
