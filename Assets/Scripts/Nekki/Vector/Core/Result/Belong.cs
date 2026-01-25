using System;
using Nekki.Vector.Core.Detector;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Result
{
	public class Belong
	{
		private QuadRunner _Platform;

		private DetectorLine _Detector;

		public QuadRunner Platform
		{
			get
			{
				return _Platform;
			}
		}

		public DetectorLine Detector
		{
			get
			{
				return _Detector;
			}
		}

		public Belong(QuadRunner p_platform, DetectorLine p_detector)
		{
			if (p_platform == null)
			{
				throw new Exception();
			}
			if (p_detector == null)
			{
				throw new Exception();
			}
			_Platform = p_platform;
			_Detector = p_detector;
		}
	}
}
