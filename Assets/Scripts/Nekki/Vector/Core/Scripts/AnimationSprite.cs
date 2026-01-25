using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts
{
	public class AnimationSprite : MonoBehaviour
	{
		private List<Sprite> _Frames;

		private SpriteRenderer _SpriteRender;

		public int _TotalFrames;

		private int _CurrentFrame;

		private float _Times = 1f;

		private float _FPS = 10f;

		private int _Iterations = -1;

		private bool _IsWork;

		public float FPS
		{
			set
			{
				_FPS = value;
			}
		}

		public int Iterations
		{
			set
			{
				_Iterations = value;
			}
		}

		public bool IsWork
		{
			get
			{
				return _IsWork;
			}
			set
			{
				_IsWork = value;
			}
		}

		public virtual void Init(string p_name, SpriteRenderer p_spriteRender)
		{
			_Frames = ResourcesMap.GetFramesSequence(p_name);
			_TotalFrames = _Frames.Count;
			if (_TotalFrames == 0)
			{
				DebugUtils.Dialog(string.Format("Error create animation by name={0}", p_name), false);
				return;
			}
			_SpriteRender = p_spriteRender;
			_SpriteRender.sprite = _Frames[0];
		}

		public virtual void SetSpriteFrame(int p_index)
		{
			if (p_index < _TotalFrames)
			{
				_SpriteRender.sprite = _Frames[p_index];
			}
		}

		public void SetSpriteAnimation()
		{
			SetSpriteFrame(_CurrentFrame);
			_CurrentFrame++;
			if (_CurrentFrame < _TotalFrames)
			{
				return;
			}
			_CurrentFrame = 0;
			if (_Iterations != -1)
			{
				_Iterations--;
				if (_Iterations <= 0)
				{
					_IsWork = false;
				}
			}
		}

		private void FixedUpdate()
		{
			if (_IsWork)
			{
				if (_Times >= 1f / _FPS)
				{
					SetSpriteAnimation();
					_Times = 0f;
				}
				else
				{
					_Times += Time.deltaTime;
				}
			}
		}
	}
}
