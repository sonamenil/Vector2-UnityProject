using System;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.Game
{
	public static class ChitProtector
	{
		public static void InitObscuredTypes()
		{
			UnityEngine.Random.InitState(DateTime.UtcNow.Second);
			ObscuredInt.SetNewCryptoKey(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			ObscuredFloat.SetNewCryptoKey(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			ObscuredString.SetNewCryptoKey(CreateRandomString(UnityEngine.Random.Range(4, 7)));
		}

		public static void ChangeCryptoKeyOnFocus()
		{
			InitObscuredTypes();
			if (DataLocal.IsCurrentExists)
			{
				UserItem itemByName = DataLocal.Current.GetItemByName(DataLocal.Money1Name);
				if (itemByName != null)
				{
					itemByName.ChangeCryptoKeyOnAttributes();
				}
				itemByName = DataLocal.Current.GetItemByName(DataLocal.Money2Name);
				if (itemByName != null)
				{
					itemByName.ChangeCryptoKeyOnAttributes();
				}
				itemByName = DataLocal.Current.GetItemByName(DataLocal.Money3Name);
				if (itemByName != null)
				{
					itemByName.ChangeCryptoKeyOnAttributes();
				}
			}
		}

		public static string CreateRandomString(int p_len)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < p_len; i++)
			{
				stringBuilder.Append((char)UnityEngine.Random.Range(33, 128));
			}
			return stringBuilder.ToString();
		}
	}
}
