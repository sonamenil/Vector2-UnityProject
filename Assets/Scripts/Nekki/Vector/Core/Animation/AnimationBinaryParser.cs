using System.Collections.Generic;
using System.IO;
using Nekki.Vector.Core.Frame;
using UnityEngine;

namespace Nekki.Vector.Core.Animation
{
	public static class AnimationBinaryParser
	{
		private static Dictionary<string, Provider> _CachedBinary;

		private static bool CreatecacheBinary()
		{
			if (_CachedBinary == null)
			{
				_CachedBinary = new Dictionary<string, Provider>();
			}
			return true;
		}

		public static void ClearCachedBinary()
		{
			if (_CachedBinary != null)
			{
				_CachedBinary.Clear();
				_CachedBinary = null;
			}
		}

		public static Provider ParseFile(string p_fileName, bool p_useCache = true)
		{
			if (p_useCache && CreatecacheBinary() && _CachedBinary.ContainsKey(p_fileName))
			{
				return _CachedBinary[p_fileName];
			}
			byte[] binary = ResourceManager.GetBinary(p_fileName);
			MemoryStream memoryStream = new MemoryStream(binary);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			Provider provider = new Provider(num);
			for (int i = 0; i < num; i++)
			{
				binaryReader.ReadByte();
				int num2 = binaryReader.ReadInt32();
				Vector3[] array = new Vector3[46];
				for (int j = 0; j < num2; j++)
				{
					if (j >= 46)
					{
						binaryReader.ReadSingle();
						binaryReader.ReadSingle();
						binaryReader.ReadSingle();
					}
					else
					{
						array[j].x = binaryReader.ReadSingle();
						array[j].y = 0f - binaryReader.ReadSingle();
						array[j].z = binaryReader.ReadSingle();
					}
				}
				provider.Add(array, i);
			}
			binaryReader.Close();
			memoryStream.Close();
			binary = null;
			if (p_useCache)
			{
				_CachedBinary.Add(p_fileName, provider);
			}
			return provider;
		}
	}
}
