using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public abstract class PrototypeInterval
	{
		protected enum IntervalType
		{
			Move = 0,
			Color = 1,
			Size = 2,
			Rotate = 3,
			Delay = 4,
			Activation = 5,
			Execute = 6,
			ToPlayer = 7,
			SwarmLookAtUser = 8,
			SwarmParticleIdle = 9,
			SwarmParticleShuffle = 10,
			SwarmSetMatrix = 11,
			SwarmParticleMoveToSinglePoint = 12,
			SwarmRotateParticles = 13,
			SwarmResize = 14,
			SwarmTranslateParticles = 15,
			Layer = 16
		}

		protected IntervalType _Type;

		protected int _Frames;

		protected int _CurrentFrame;

		protected int _CurrentFrameFake;

		public bool IsIntervalMove
		{
			get
			{
				return _Type == IntervalType.Move;
			}
		}

		public bool IsIntervalDelay
		{
			get
			{
				return _Type == IntervalType.Delay;
			}
		}

		public bool IsIntervalRotate
		{
			get
			{
				return _Type == IntervalType.Rotate;
			}
		}

		public bool IsIntervalResize
		{
			get
			{
				return _Type == IntervalType.Size;
			}
		}

		public bool IsIntervalColor
		{
			get
			{
				return _Type == IntervalType.Color;
			}
		}

		public bool IsChangePosition
		{
			get
			{
				return _Type == IntervalType.Move || _Type == IntervalType.Rotate || _Type == IntervalType.Size || _Type == IntervalType.ToPlayer;
			}
		}

		public int Frames
		{
			get
			{
				return _Frames;
			}
		}

		public int CurrentFrame
		{
			get
			{
				return _CurrentFrame;
			}
		}

		public PrototypeInterval()
		{
			_Frames = 0;
			_CurrentFrame = 0;
		}

		public static PrototypeInterval Create(XmlNode p_node)
		{
			PrototypeInterval prototypeInterval = null;
			switch (p_node.Name)
			{
			case "MoveInterval":
				prototypeInterval = new IntervalMove();
				break;
			case "SizeInterval":
				prototypeInterval = new IntervalSize();
				break;
			case "DelayInterval":
				prototypeInterval = new IntervalDelay();
				break;
			case "ColorInterval":
				prototypeInterval = new IntervalColor();
				break;
			case "RotationInterval":
				prototypeInterval = new IntervalRotate();
				break;
			case "ActivationInterval":
				prototypeInterval = new IntervalActivation();
				break;
			case "ExecuteInterval":
				prototypeInterval = new IntervalExecute();
				break;
			case "ToPlayerInterval":
				prototypeInterval = new IntervalToPlayer();
				break;
			case "IntervalSwarmLookAtUser":
				prototypeInterval = new IntervalSwarmLookAtUser();
				break;
			case "IntervalSwarmParticleIdleAnimation":
				prototypeInterval = new IntervalSwarmParticleIdle();
				break;
			case "IntervalSwarmParticleMoveToSinglePoint":
				prototypeInterval = new IntervalSwarmParticleMoveToSinglePoint();
				break;
			case "IntervalSwarmParticleShuffle":
				prototypeInterval = new IntervalSwarmParticleShuffle();
				break;
			case "IntervalSwarmResize":
				prototypeInterval = new IntervalSwarmResize();
				break;
			case "IntervalSwarmRotateParticles":
				prototypeInterval = new IntervalSwarmRotateParticles();
				break;
			case "IntervalSwarmTranslateParticles":
				prototypeInterval = new IntervalSwarmTranslateParticles();
				break;
			case "ChangeLayerInterval":
				prototypeInterval = new IntervalLayer();
				break;
			}
			if (prototypeInterval == null)
			{
				return null;
			}
			prototypeInterval.Parse(p_node);
			return prototypeInterval;
		}

		protected virtual void Parse(XmlNode p_node)
		{
			_Frames = (int)XmlUtils.ParseFloat(p_node.Attributes["Frames"], 1f);
		}

		public abstract bool Iteration(TransformInterface Runner);

		public virtual void Reset()
		{
			_CurrentFrame = 0;
		}
	}
}
