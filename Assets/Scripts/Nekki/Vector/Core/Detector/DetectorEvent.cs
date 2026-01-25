using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Detector
{
	public class DetectorEvent
	{
		public enum DetectorEventType
		{
			None = 0,
			On = 1,
			Off = 2
		}

		private DetectorLine _Detector;

		private DetectorEventType _Type;

		private int _Side;

		public DetectorLine Detector
		{
			get
			{
				return _Detector;
			}
		}

		public DetectorEventType Type
		{
			get
			{
				return _Type;
			}
		}

		public int Side
		{
			get
			{
				return _Side;
			}
		}

		public QuadRunner Platform
		{
			get
			{
				return _Detector.Platform;
			}
		}

		public ModelNode Node
		{
			get
			{
				return _Detector.Node;
			}
		}

		public bool IsVertical
		{
			get
			{
				return _Detector.Type == DetectorLine.DetectorType.Vertical;
			}
		}

		public bool IsHorizontal
		{
			get
			{
				return _Detector.Type == DetectorLine.DetectorType.Horizontal;
			}
		}

		public DetectorEvent(DetectorLine p_detector, DetectorEventType p_type, int p_side)
		{
			_Detector = p_detector;
			_Type = p_type;
			_Side = p_side;
		}

		public void DeltaPosition(Vector3f p_velocity)
		{
			if (_Type == DetectorEventType.On)
			{
				_Detector.DeltaPosition(Platform.DeltaEdge(_Detector.Position, _Side).Add(p_velocity));
			}
		}
	}
}
