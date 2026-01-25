using System;
using System.Collections.Generic;

namespace Nekki.Social
{
	public class UserInfo
	{
		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public string PhotoURL { get; private set; }

		public string UserID { get; private set; }

		internal UserInfo(string source)
		{
			string[] array = source.Split('|');
			UserID = array[0];
			PhotoURL = array[1];
			if (array[2].Contains(" "))
			{
				string[] array2 = array[2].Split(' ');
				FirstName = array2[0];
				LastName = array2[1];
			}
			else
			{
				FirstName = array[2];
				LastName = string.Empty;
			}
		}

		internal UserInfo(string userID, string photoURL, string lastName, string firstName)
		{
			FirstName = firstName;
			LastName = lastName;
			PhotoURL = photoURL;
			UserID = userID;
		}

		private UserInfo()
		{
		}

		internal static Dictionary<string, UserInfo> GetInfos(string source)
		{
			Dictionary<string, UserInfo> dictionary = new Dictionary<string, UserInfo>();
			string[] array = source.Split(new char[1] { '^' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				UserInfo userInfo = new UserInfo(array[i]);
				if (!dictionary.ContainsKey(userInfo.UserID))
				{
					dictionary.Add(userInfo.UserID, userInfo);
				}
				else
				{
					dictionary[userInfo.UserID] = userInfo;
				}
			}
			return dictionary;
		}

		public static implicit operator bool(UserInfo user)
		{
			return user != null;
		}
	}
}
