using System.Collections.Generic;
using System.Net.NetworkInformation;
using CUDLR;
using UnityEngine;

namespace Nekki.Vector.Core.Game
{
	public static class CUDLRConsole
	{
		private const string _UIDs = "and_4df17ff36d579f85and_015d3248b8300406and_003c92d3470a132band_85HBCMC222GKand_06285a3bios_6B40EACD-A63E-4678-B2B2-DCEB7EB6BE64ios_7BDA9571-0D5A-4C71-B37D-62F0A60E27F2ios_221B7B48-69AF-417E-9073-5153C8364223ios_DA641991-42F4-453C-B181-CCF7B123A542ios_27F408D3-1534-4FBB-89F0-2329B10D3B67ios_69E6CB4F-56CD-4280-ACF0-8F8DBC3756D3and_E5OKCY366676and_04157df458b90c15and_16aedfb17d03and_F1NKBC022629and_32048fa40c3e613dios_01961E4F-7B6B-4F53-864C-39E73B5DF6C2ios_83E2D8A6-7BC1-47C4-B985-716920C696B8ios_16AC2D7A-CB3B-469F-86D8-0B14CD9B9ABEios_54317CB8-3DF1-45E5-90D9-6D05C7327F33ios_6613F08E-D50A-4FDA-95FC-BBC26E637D49ios_90C88173-5F11-4319-80E1-1BC43B4F81B8and_BH9089HH13ios_BDF0952D-7027-4124-80A0-8D9AC3F8EAC0ios_FB8D31A9-E9C4-4D84-A7D0-2A38E9573811ios_D2B966B5-26B9-4606-895E-C47D12A9C63Aios_C430BC82-E8DC-45F4-973B-AE36F4FE6ED5and_HT4CCJT01259and_YT910WRYRPand_06157df625d0bc3a";

		private static bool IsPlatformSupported
		{
			get
			{
				return DeviceInformation.IsAndroid || DeviceInformation.IsiOS;
			}
		}

		private static bool IsCUDLRAllowed
		{
			get
			{
				return "and_4df17ff36d579f85and_015d3248b8300406and_003c92d3470a132band_85HBCMC222GKand_06285a3bios_6B40EACD-A63E-4678-B2B2-DCEB7EB6BE64ios_7BDA9571-0D5A-4C71-B37D-62F0A60E27F2ios_221B7B48-69AF-417E-9073-5153C8364223ios_DA641991-42F4-453C-B181-CCF7B123A542ios_27F408D3-1534-4FBB-89F0-2329B10D3B67ios_69E6CB4F-56CD-4280-ACF0-8F8DBC3756D3and_E5OKCY366676and_04157df458b90c15and_16aedfb17d03and_F1NKBC022629and_32048fa40c3e613dios_01961E4F-7B6B-4F53-864C-39E73B5DF6C2ios_83E2D8A6-7BC1-47C4-B985-716920C696B8ios_16AC2D7A-CB3B-469F-86D8-0B14CD9B9ABEios_54317CB8-3DF1-45E5-90D9-6D05C7327F33ios_6613F08E-D50A-4FDA-95FC-BBC26E637D49ios_90C88173-5F11-4319-80E1-1BC43B4F81B8and_BH9089HH13ios_BDF0952D-7027-4124-80A0-8D9AC3F8EAC0ios_FB8D31A9-E9C4-4D84-A7D0-2A38E9573811ios_D2B966B5-26B9-4606-895E-C47D12A9C63Aios_C430BC82-E8DC-45F4-973B-AE36F4FE6ED5and_HT4CCJT01259and_YT910WRYRPand_06157df625d0bc3a".Contains(DeviceInformation.GetServerUniqueID);
			}
		}

		public static void Init()
		{
			if (!IsPlatformSupported)
			{
				DebugUtils.LogToConsole("Can't start CUDLR for unsupported platform!", true);
				return;
			}
			if (!IsCUDLRAllowed)
			{
				DebugUtils.LogToConsole("Can't start CUDLR for unregistered device <" + DeviceInformation.GetServerUniqueID + ">!", true);
				return;
			}
			Server server = Server.Init(44444);
			HashSet<LogType> hashSet = new HashSet<LogType>();
			hashSet.Add(LogType.Log);
			hashSet.Add(LogType.Exception);
			hashSet.Add(LogType.Assert);
			hashSet.Add(LogType.Error);
			HashSet<LogType> p_enabledLogTypes = hashSet;
			server.SetupEnabledLogTypes(p_enabledLogTypes);
			switch (server.InitStatus)
			{
			case Server.ServerInitStatus.Success:
				DebugUtils.LogToConsole("Starting CUDLR Server on 'http://" + ":" + server.Port + "'", true);
				break;
			case Server.ServerInitStatus.Error_NoFreePorts:
				DebugUtils.LogToConsole("Can't start CUDLR because no free ports!", true);
				break;
			default:
				DebugUtils.LogToConsole("Can't start CUDLR because unexpected error", true);
				break;
			}
		}
	}
}
