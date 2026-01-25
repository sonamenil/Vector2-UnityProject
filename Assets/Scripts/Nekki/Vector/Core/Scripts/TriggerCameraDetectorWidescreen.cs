using System;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Scripts
{
	public class TriggerCameraDetectorWidescreen
	{
		public Action OnBecameVisibleEvent = delegate
		{
		};

		public Action OnBecameInvisibleEvent = delegate
		{
		};

		private Rectangle _Rect = new Rectangle(0f, 0f, 0f, 0f);

		private TriggerRunner _Base;

		private bool _IsVisible;

		public TriggerRunner Base
		{
			get
			{
				return _Base;
			}
			set
			{
				_Base = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				return _IsVisible;
			}
			set
			{
				_IsVisible = value;
			}
		}

		public void Update(Rectangle p_viewport)
		{
			if (_Base != null && RunMainController.IsRunNow && RunMainController.Scene != null)
			{
				if (_Rect != _Base.Rectangle)
				{
					_Rect.Set(_Base.Rectangle);
				}
				if (!_IsVisible && _Rect.Intersect(p_viewport))
				{
					_IsVisible = true;
					OnBecameVisibleEvent();
				}
				else if (_IsVisible && !_Rect.Intersect(p_viewport))
				{
					_IsVisible = false;
					OnBecameInvisibleEvent();
				}
			}
		}

		public void End()
		{
			_Base = null;
			OnBecameVisibleEvent = null;
			OnBecameInvisibleEvent = null;
		}
	}
}
