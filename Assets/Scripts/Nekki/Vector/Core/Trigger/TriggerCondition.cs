namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerCondition
	{
		protected bool _IsNot;

		public abstract bool Check();

		public abstract void Log(bool result);
	}
}
