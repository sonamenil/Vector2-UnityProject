using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerSpawns
	{
		private const string _SaveMeSpawnName = "SavemeSpawn";

		private const string _TeleportSpawnName = "TeleportSpawn";

		private List<SpawnRunner> _Spawns = new List<SpawnRunner>();

		public static bool IsSaveMe(SpawnRunner p_spawn)
		{
			return p_spawn.Name == "SavemeSpawn";
		}

		public static bool IsTeleport(SpawnRunner p_spawn)
		{
			return p_spawn.Name == "TeleportSpawn";
		}

		public void InitSpawns(List<SpawnRunner> p_spawns)
		{
			_Spawns.Clear();
			_Spawns.AddRange(p_spawns);
			_Spawns.Sort(delegate(SpawnRunner c1, SpawnRunner c2)
			{
				if (c1.Position.x < c2.Position.x)
				{
					return -1;
				}
				return (c1.Position.x != c2.Position.x) ? 1 : 0;
			});
		}

		public SpawnRunner GetNearestTeleportOrSaveMe(float p_modelPosition)
		{
			return GetNearestSpawn((SpawnRunner p_spawn) => IsSaveMe(p_spawn) || IsTeleport(p_spawn), p_modelPosition);
		}

		public SpawnRunner GetNearestSaveMe(float p_modelPosition)
		{
			return GetNearestSpawn((SpawnRunner p_spawn) => IsSaveMe(p_spawn), p_modelPosition);
		}

		public SpawnRunner GetNearestTeleport(float p_modelPosition)
		{
			return GetNearestSpawn((SpawnRunner p_spawn) => IsTeleport(p_spawn), p_modelPosition);
		}

		public SpawnRunner GetNearestSpawn(float p_modelPosition)
		{
			return GetNearestSpawn((SpawnRunner p_spawn) => true, p_modelPosition);
		}

		private SpawnRunner GetNearestSpawn(Predicate<SpawnRunner> p_condition, float p_modelPosition)
		{
			foreach (SpawnRunner spawn in _Spawns)
			{
				if (spawn.Position.x > p_modelPosition && spawn.IsEnabled && p_condition(spawn))
				{
					return spawn;
				}
			}
			return _Spawns[_Spawns.Count - 1];
		}
	}
}
