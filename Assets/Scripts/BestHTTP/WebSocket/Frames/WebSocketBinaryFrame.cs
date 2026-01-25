using System;
using System.IO;

namespace BestHTTP.WebSocket.Frames
{
	public class WebSocketBinaryFrame : IWebSocketFrameWriter
	{
		private static readonly byte[] NoData = new byte[0];

		public virtual WebSocketFrameTypes Type
		{
			get
			{
				return WebSocketFrameTypes.Binary;
			}
		}

		public bool IsFinal { get; protected set; }

		protected byte[] Data { get; set; }

		protected ulong Pos { get; set; }

		protected ulong Length { get; set; }

		public WebSocketBinaryFrame(byte[] data)
			: this(data, 0uL, (ulong)((data == null) ? 0 : data.Length), true)
		{
		}

		public WebSocketBinaryFrame(byte[] data, bool isFinal)
			: this(data, 0uL, (ulong)((data == null) ? 0 : data.Length), isFinal)
		{
		}

		public WebSocketBinaryFrame(byte[] data, ulong pos, ulong length, bool isFinal)
		{
			Data = data;
			Pos = pos;
			Length = length;
			IsFinal = isFinal;
		}

		public virtual byte[] Get()
		{
			if (Data == null)
			{
				Data = NoData;
			}
			using (MemoryStream memoryStream = new MemoryStream((int)Length + 9))
			{
				byte b = (byte)(IsFinal ? 128u : 0u);
				memoryStream.WriteByte((byte)((uint)b | (uint)Type));
				if (Length < 126)
				{
					memoryStream.WriteByte((byte)(0x80 | (byte)Length));
				}
				else if (Length < 65535)
				{
					memoryStream.WriteByte(254);
					byte[] bytes = BitConverter.GetBytes((ushort)Length);
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes, 0, bytes.Length);
					}
					memoryStream.Write(bytes, 0, bytes.Length);
				}
				else
				{
					memoryStream.WriteByte(byte.MaxValue);
					byte[] bytes2 = BitConverter.GetBytes(Length);
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes2, 0, bytes2.Length);
					}
					memoryStream.Write(bytes2, 0, bytes2.Length);
				}
				byte[] bytes3 = BitConverter.GetBytes(GetHashCode());
				memoryStream.Write(bytes3, 0, bytes3.Length);
				for (ulong num = Pos; num < Pos + Length; num++)
				{
					memoryStream.WriteByte((byte)(Data[num] ^ bytes3[(num - Pos) % 4]));
				}
				return memoryStream.ToArray();
			}
		}
	}
}
