using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Scripts
{
	public class CustomAnimationSprite : AnimationSprite
	{
		private List<KeyValuePair<Sprite, int>> _Frames;

		private SpriteRenderer _SpriteRender;

		private int _CurrentFrame;

		private float _Times = 1f;

		private float _FPS = 10f;

		public override void Init(string p_name, SpriteRenderer p_spriteRender)
		{
			_Frames = ResourcesMap.GetCustomFramesSequence(p_name);
			_TotalFrames = TotalFrames();
			if (_TotalFrames == 0)
			{
				DebugUtils.Dialog(string.Format("Error create animation by name={0}", p_name), false);
				return;
			}
			_SpriteRender = p_spriteRender;
			_SpriteRender.sprite = _Frames[0].Key;
		}

		public override void SetSpriteFrame(int p_index)
		{
			if (p_index < _TotalFrames)
			{
				int i;
				for (i = 0; p_index > _Frames[i].Value; i++)
				{
					p_index -= _Frames[i].Value;
				}
				_SpriteRender.sprite = _Frames[i].Key;
			}
		}

		private void FixedUpdate()
		{
			if (base.IsWork)
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

		private int TotalFrames()
		{
			int num = 0;
			foreach (KeyValuePair<Sprite, int> frame in _Frames)
			{
				num += frame.Value;
			}
			return num;
		}
	}
}
