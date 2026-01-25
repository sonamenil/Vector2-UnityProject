using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public abstract class Token
	{
		private readonly Mark start;

		private readonly Mark end;

		public Mark Start
		{
			get
			{
				return start;
			}
		}

		public Mark End
		{
			get
			{
				return end;
			}
		}

		protected Token(Mark start, Mark end)
		{
			this.start = start;
			this.end = end;
		}
	}
}
