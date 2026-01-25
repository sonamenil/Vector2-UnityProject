using System.Collections.Generic;

namespace Nekki.Vector.Core
{
	public class BufferFrames : KeyFrames
	{
		public bool IsBufferEmpty
		{
			get
			{
				return _Size - _ActiveFrame == 0;
			}
		}

		public new void Reset()
		{
			_Size = _ActiveFrame;
		}

		public void InitBuffer(int p_frames)
		{
			_ActiveFrame = 0;
			_Size = p_frames;
			if (_Data.Count < _Size)
			{
				ResizeData(_Size - _Data.Count, 46);
			}
		}

		public void VelocityBuffer(Vector3f p_point)
		{
			for (int i = _ActiveFrame; i < _Size; i++)
			{
				List<Vector3f> list = _Data[i];
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Add(p_point);
				}
			}
		}
	}
}
