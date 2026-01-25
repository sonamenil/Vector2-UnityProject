namespace Nekki.Vector.Core.GameManagement
{
	public class StarReward
	{
		public enum Type
		{
			Single = 0,
			Repeated = 1
		}

		private string _BaseAlias = "^Missions.StarRewards.{0}.{1}^";

		private string _BaseIcon = "star_rewards.rank{0}";

		private string _Name;

		private Type _Type;

		private int _Rarity;

		private string _SingleRewardIconName;

		public bool IsSingle
		{
			get
			{
				return _Type == Type.Single;
			}
		}

		public string Title
		{
			get
			{
				return string.Format(_BaseAlias, Preset, "Title");
			}
		}

		public string Description
		{
			get
			{
				return string.Format(_BaseAlias, Preset, "Description");
			}
		}

		public string IconName
		{
			get
			{
				return string.Format(_BaseIcon, _Name);
			}
		}

		public int Rarity
		{
			get
			{
				int result = 1;
				int.TryParse(ZoneResource<ZoneBalanceManager>.Current.GetBalance("StarRewards", _Name, _Type.ToString(), "Rarity"), out result);
				return result;
			}
		}

		public int RarityModifier
		{
			get
			{
				int result = 1;
				int.TryParse(BalanceManager.Current.GetBalance("Missions", "StarRankCapacity", _Rarity.ToString()), out result);
				return result;
			}
		}

		public int Cost
		{
			get
			{
				int result = 0;
				int.TryParse(ZoneResource<ZoneBalanceManager>.Current.GetBalance("StarRewards", _Name, _Type.ToString(), "Cost"), out result);
				return result;
			}
		}

		public string Preset
		{
			get
			{
				return ZoneResource<ZoneBalanceManager>.Current.GetBalance("StarRewards", _Name, _Type.ToString(), "Preset");
			}
		}

		public string SingleRewardIconName
		{
			get
			{
				if (_Type == Type.Single)
				{
					return ZoneResource<ZoneBalanceManager>.Current.GetBalance("StarRewards", _Name, _Type.ToString(), "SingleRewardIcon");
				}
				return null;
			}
		}

		public string SingleRewardArtName
		{
			get
			{
				if (_Type == Type.Single)
				{
					return ZoneResource<ZoneBalanceManager>.Current.GetBalance("StarRewards", _Name, _Type.ToString(), "SingleRewardArt");
				}
				return null;
			}
		}

		public int StarsCount
		{
			get
			{
				return Cost / RarityModifier;
			}
		}

		public StarReward(string p_name, Type p_type)
		{
			_Name = p_name;
			_Type = p_type;
			_Rarity = Rarity;
			_SingleRewardIconName = SingleRewardIconName;
		}

		public static StarReward Create(string p_name, Type p_type)
		{
			return new StarReward(p_name, p_type);
		}

		public static int GetCoolnessMin(string p_rewardName)
		{
			int result = 0;
			int.TryParse(ZoneResource<ZoneBalanceManager>.Current.GetBalance("StarRewards", p_rewardName, Type.Single.ToString(), "CoolnessMin"), out result);
			return result;
		}
	}
}
