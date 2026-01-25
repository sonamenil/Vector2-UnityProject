namespace Nekki.Vector.Core.GameManagement
{
	public abstract class ZoneResource<T> where T : ZoneResource<T>, new()
	{
		private static T _Current;

		protected abstract string ResourceId { get; }

		protected string FilePath
		{
			get
			{
				return ZoneManager.GetResourceFilePath(ResourceId);
			}
		}

		protected bool IsNeedReload
		{
			get
			{
				return ZoneManager.IsResourceNeedReload(ResourceId);
			}
		}

		public static T Current
		{
			get
			{
				if (_Current == null)
				{
					Reload();
				}
				return _Current;
			}
		}

		protected abstract void Parse();

		public static void Reset()
		{
			_Current = (T)null;
		}

		public static void Reload()
		{
			_Current = new T();
			_Current.Parse();
		}

		public static void ResetIfNeed()
		{
			if (_Current != null && _Current.IsNeedReload)
			{
				Reset();
			}
		}

		public static void ReloadIfNeed()
		{
			if (_Current != null && _Current.IsNeedReload)
			{
				Reload();
			}
		}
	}
}
