using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Transformations
{
	public abstract class IntervalSwarm : PrototypeInterval
	{
		protected Swarm _Swarm;

		public IntervalSwarm()
		{
		}

		protected bool InitSwarm(TransformInterface p_runner)
		{
			if (_Swarm == null)
			{
				_Swarm = p_runner as Swarm;
				if (_Swarm == null)
				{
					return false;
				}
			}
			return true;
		}

		protected virtual void InitIteration()
		{
		}

		public override void Reset()
		{
			base.Reset();
			_Swarm = null;
		}
	}
}
