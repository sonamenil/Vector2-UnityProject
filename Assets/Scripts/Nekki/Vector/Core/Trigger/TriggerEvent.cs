namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerEvent
	{
		public enum EventType
		{
			TRE_ENTER = 1,
			TRE_EXIT = 2,
			TRE_LINE = 3,
			TRE_TIMEOUT = 4,
			TRE_KEY = 5,
			TRE_ACTIVATE = 6,
			TRE_COLISION = 7,
			TRE_VAR_CHANGE = 8,
			TRE_ON_SHOW = 9,
			TRE_ON_HIDE = 10,
			TRE_ON_SHOW_WIDESCREEN = 11,
			TRE_ON_HIDE_WIDESCREEN = 12,
			TRE_ON_START_GAME = 13,
			TRE_GLOBAL_TIMEOUT = 14,
			TRE_SWARM_ARRIVAL = 15,
			TRE_SWARM_DEPARTURE = 16,
			TRE_SWARM_DEC = 17,
			TRE_END_GAME = 18,
			TRE_ACTIVATE_NEAR_PLAYER = 19,
			TRE_ON_DEATH = 20,
			TQE_ON_SCREEN = 100,
			TQE_ON_CALL = 101,
			TQE_ON_BUY_ITEM = 102,
			TPEE_FLOOR_START = 200,
			TPEE_FLOOR_END = 201,
			TPEE_ACTIVATE = 202,
			TPEE_ANIMATION_START = 203
		}

		protected EventType _Type;

		public EventType Type
		{
			get
			{
				return _Type;
			}
		}

		public virtual bool IsEqual(TriggerEvent p_value)
		{
			return p_value._Type == _Type;
		}

		public override string ToString()
		{
			switch (_Type)
			{
			case EventType.TRE_KEY:
				return "Event: -- KEY";
			case EventType.TRE_ACTIVATE:
				return "Event : -- ACTIVATE";
			case EventType.TRE_ENTER:
				return "Event : -- ENTER";
			case EventType.TRE_EXIT:
				return "Event : -- EXIT";
			case EventType.TRE_LINE:
				return "Event : -- LINE";
			case EventType.TRE_TIMEOUT:
				return "Event : -- TIMEOUT";
			case EventType.TRE_COLISION:
				return "Event : -- COLLISION";
			case EventType.TRE_VAR_CHANGE:
				return "Event : -- ValueChange";
			case EventType.TRE_ON_START_GAME:
				return "Event : -- OnStartGame";
			case EventType.TRE_END_GAME:
				return "Event : -- EndGame";
			case EventType.TRE_ON_HIDE:
				return "Event : -- OnHide";
			case EventType.TRE_ON_HIDE_WIDESCREEN:
				return "Event : -- OnHideWidescreen";
			case EventType.TRE_ON_SHOW:
				return "Event : -- OnShow";
			case EventType.TRE_ON_SHOW_WIDESCREEN:
				return "Event : -- OnShowWidescreen";
			case EventType.TRE_GLOBAL_TIMEOUT:
				return "Event : -- OnGlobalTimer";
			case EventType.TRE_ACTIVATE_NEAR_PLAYER:
				return "Event : -- ACTIVATE NEAR PLAYER";
			case EventType.TRE_ON_DEATH:
				return "Event : -- OnDeath";
			default:
				return string.Empty;
			}
		}

		public string GetStringType()
		{
			return _Type.ToString();
		}
	}
}
