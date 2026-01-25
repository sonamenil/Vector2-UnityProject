using System.IO;
using Nekki.Vector.Core.DataValidation;

namespace Nekki.Vector.Core
{
	public static class UserDataCreator
	{
		private static string OldGamedataPath
		{
			get
			{
				return VectorPaths.CurrentStorage + "/gamedata";
			}
		}

		public static void CopyFromGamedataToUserdata()
		{
			if (!DeviceInformation.IsEmulator && Directory.Exists(OldGamedataPath) && !Directory.Exists(VectorPaths.StorageUserData))
			{
				Directory.Move(OldGamedataPath, VectorPaths.StorageUserData);
				ResourcesValidator.RemoveOldFileValidation();
				string path = string.Format("{0}/Resources", VectorPaths.StorageUserData);
				if (Directory.Exists(path))
				{
					Directory.Delete(path, true);
				}
			}
		}
	}
}
