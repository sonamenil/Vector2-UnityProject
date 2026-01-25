using System.Globalization;
using System.IO;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public class EventReader
	{
		private readonly IParser parser;

		private bool endOfStream;

		public IParser Parser
		{
			get
			{
				return parser;
			}
		}

		public EventReader(IParser parser)
		{
			this.parser = parser;
			MoveNext();
		}

		public T Expect<T>() where T : ParsingEvent
		{
			T val = Allow<T>();
			if (val == null)
			{
				ParsingEvent current = parser.Current;
				throw new YamlException(current.Start, current.End, string.Format(CultureInfo.InvariantCulture, "Expected '{0}', got '{1}' (at {2}).", typeof(T).Name, current.GetType().Name, current.Start));
			}
			return val;
		}

		public bool Accept<T>() where T : ParsingEvent
		{
			ThrowIfAtEndOfStream();
			return parser.Current is T;
		}

		private void ThrowIfAtEndOfStream()
		{
			if (endOfStream)
			{
				throw new EndOfStreamException();
			}
		}

		public T Allow<T>() where T : ParsingEvent
		{
			if (!Accept<T>())
			{
				return (T)null;
			}
			T result = (T)parser.Current;
			MoveNext();
			return result;
		}

		public T Peek<T>() where T : ParsingEvent
		{
			if (!Accept<T>())
			{
				return (T)null;
			}
			return (T)parser.Current;
		}

		public void SkipThisAndNestedEvents()
		{
			int num = 0;
			do
			{
				num += Peek<ParsingEvent>().NestingIncrease;
				MoveNext();
			}
			while (num > 0);
		}

		private void MoveNext()
		{
			endOfStream = !parser.MoveNext();
		}
	}
}
