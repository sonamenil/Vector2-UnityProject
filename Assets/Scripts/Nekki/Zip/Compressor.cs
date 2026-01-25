using System;
using System.IO;
using SevenZip;
using SevenZip.Compression.LZMA;

namespace Nekki.Zip
{
	public class Compressor
	{
		public enum CompressorTypes
		{
			LZMA = 0
		}

		private static int dictionary;

		private static bool eos;

		private static CoderPropID[] propIDs;

		private static object[] properties;

		public static CompressorTypes CompressorType { get; set; }

		static Compressor()
		{
			dictionary = 8388608;
			eos = false;
			propIDs = new CoderPropID[8]
			{
				CoderPropID.DictionarySize,
				CoderPropID.PosStateBits,
				CoderPropID.LitContextBits,
				CoderPropID.LitPosBits,
				CoderPropID.Algorithm,
				CoderPropID.NumFastBytes,
				CoderPropID.MatchFinder,
				CoderPropID.EndMarker
			};
			properties = new object[8] { dictionary, 2, 3, 0, 2, 128, "bt4", eos };
			CompressorType = CompressorTypes.LZMA;
		}

		public static void Zip(string zipFile, string sourceFile, CompressorTypes compressor = CompressorTypes.LZMA)
		{
			File.WriteAllBytes(zipFile, Compress(File.ReadAllBytes(sourceFile), compressor));
		}

		public static void Unzip(string zipFile, string outFile, CompressorTypes compressor = CompressorTypes.LZMA)
		{
			File.WriteAllBytes(outFile, Decompress(File.ReadAllBytes(zipFile), compressor));
		}

		public static byte[] Compress(byte[] inputBytes, CompressorTypes compressor = CompressorTypes.LZMA)
		{
			if (compressor == CompressorTypes.LZMA)
			{
				MemoryStream memoryStream = new MemoryStream(inputBytes);
				MemoryStream memoryStream2 = new MemoryStream();
				Encoder encoder = new Encoder();
				encoder.SetCoderProperties(propIDs, properties);
				encoder.WriteCoderProperties(memoryStream2);
				long length = memoryStream.Length;
				for (int i = 0; i < 8; i++)
				{
					memoryStream2.WriteByte((byte)(length >> 8 * i));
				}
				encoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
				return memoryStream2.ToArray();
			}
			return new byte[0];
		}

		public static byte[] Decompress(byte[] inputBytes, CompressorTypes compressor = CompressorTypes.LZMA)
		{
			if (compressor == CompressorTypes.LZMA)
			{
				using (MemoryStream memoryStream = new MemoryStream(inputBytes))
				{
					Decoder decoder = new Decoder();
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						byte[] array = new byte[5];
						if (memoryStream.Read(array, 0, 5) != 5)
						{
							throw new Exception("input .lzma is too short");
						}
						long num = 0L;
						for (int i = 0; i < 8; i++)
						{
							int num2 = memoryStream.ReadByte();
							if (num2 < 0)
							{
								throw new Exception("Can't Read 1");
							}
							num |= (long)(int)(byte)num2 << 8 * i;
						}
						decoder.SetDecoderProperties(array);
						long inSize = memoryStream.Length - memoryStream.Position;
						decoder.Code(memoryStream, memoryStream2, inSize, num, null);
						return memoryStream2.ToArray();
					}
				}
			}
			return new byte[0];
		}
	}
}
