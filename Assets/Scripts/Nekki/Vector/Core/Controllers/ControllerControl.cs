using System.Collections.Generic;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Animation.Events;
using Nekki.Vector.Core.Models;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerControl
	{
		private int _Interval;

		private int _OnInterval;

		private KeyVariables _KeyVariables;

		private ModelHuman _Model;

		private bool _IsPlay;

		public KeyVariables KeyVariables
		{
			get
			{
				return _KeyVariables;
			}
		}

		public bool Enable
		{
			get
			{
				return _IsPlay;
			}
			set
			{
				_IsPlay = value;
			}
		}

		public ControllerControl(ModelHuman p_parent)
		{
			_Model = p_parent;
			_Interval = 21;
			_OnInterval = 0;
			_IsPlay = true;
		}

		public bool SetKeyVariable(KeyVariables p_value)
		{
			if (_IsPlay && RunMainController.IsPaused && !RunMainController.IsDebugPaused)
			{
				return false;
			}
			if (!_IsPlay)
			{
				if (!ControllerTutorial.Check(p_value))
				{
					return false;
				}
				_IsPlay = true;
			}
			_KeyVariables = p_value;
			_OnInterval = 0;
			_Model.ControllerTrigger.SetKeyEvent(p_value.ToString());
			return true;
		}

		public void SetKeyVariable_force(KeyVariables p_value)
		{
			_OnInterval = 0;
			_KeyVariables = p_value;
		}

		public void ClearKey()
		{
			_KeyVariables = null;
		}

		public void Render()
		{
			if (_KeyVariables != null)
			{
				if (_OnInterval >= _Interval)
				{
					ClearKey();
				}
				else
				{
					_OnInterval++;
				}
			}
		}

		public void StartStop()
		{
			_IsPlay = !_IsPlay;
			if (!_IsPlay)
			{
				ClearKey();
			}
		}

		public List<AnimationReaction> GetValidateReactions(KeyVariables p_value, AnimationEventKey p_anim_key, int p_sign)
		{
			if (p_value == null)
			{
				return null;
			}
			if (p_anim_key.IsKey(p_value, p_sign))
			{
				return p_anim_key.Reaction;
			}
			return null;
		}

		public void Reset()
		{
			ClearKey();
			_IsPlay = true;
		}
	}
}
