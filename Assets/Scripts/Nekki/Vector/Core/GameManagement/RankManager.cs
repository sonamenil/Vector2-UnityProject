using System.Collections.Generic;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.GameManagement
{
	public static class RankManager
	{
		private static int _Rank;

		private static int _RankOnFloorStart;

		public static int Rank
		{
			get
			{
				RefreshRank();
				return _Rank;
			}
		}

		public static int Experience
		{
			get
			{
				return DataLocal.Current.Money1;
			}
			private set
			{
				DataLocal.Current.Money1 = value;
			}
		}

		public static int RankOnFloorStart
		{
			get
			{
				return _RankOnFloorStart;
			}
			set
			{
				_RankOnFloorStart = value;
			}
		}

		public static void ResetRank()
		{
			_Rank = 1;
		}

		public static void ResetExp()
		{
			if ((int)DataLocal.Current.Money1 != 0)
			{
				Experience = 0;
			}
		}

		public static List<int> GetRanksGainedOnFloor()
		{
			RefreshRank();
			List<int> list = new List<int>();
			for (int i = _RankOnFloorStart + 1; i <= _Rank; i++)
			{
				list.Add(i);
			}
			return list;
		}

		public static void RefreshRank()
		{
			_Rank = RankFromPoints(Experience);
		}

		public static int RankFromPoints(int p_points)
		{
			int num = 0;
			for (int i = 0; p_points >= i; i += GetExpDeltaForRank(num))
			{
				num++;
				if (StarterPacksManager.SelectedStarterPack == null)
				{
					break;
				}
			}
			return num;
		}

		public static float CurrentPointsPart()
		{
			RefreshRank();
			return PointsRankCostRatio(_Rank, Experience);
		}

		public static float PointsPartAtFloorStart()
		{
			return PointsRankCostRatio(_RankOnFloorStart, EndFloorManager.StartMoney1);
		}

		public static float PointsRankCostRatio(int p_current_rank, int p_all_points)
		{
			float num = 0f;
			float num2 = p_all_points;
			for (int i = 1; i < p_current_rank; i++)
			{
				num2 -= (float)GetExpDeltaForRank(i);
			}
			return num2 / (float)GetExpDeltaForRank(p_current_rank);
		}

		public static int GetExpDeltaForRank(int rank)
		{
			Variable variable = Variable.CreateVariable(BalanceManager.Current.GetBalance("StarterPacks", StarterPacksManager.SelectedStarterPack.Name, "Ranks", "Points", rank.ToString()), string.Empty);
			return variable.ValueInt;
		}
	}
}
