using System.Globalization;

namespace YamlDotNet.Core.Events
{
	public class SequenceStart : NodeEvent
	{
		private readonly bool isImplicit;

		private readonly SequenceStyle style;

		public override int NestingIncrease
		{
			get
			{
				return 1;
			}
		}

		internal override EventType Type
		{
			get
			{
				return EventType.SequenceStart;
			}
		}

		public bool IsImplicit
		{
			get
			{
				return isImplicit;
			}
		}

		public override bool IsCanonical
		{
			get
			{
				return !isImplicit;
			}
		}

		public SequenceStyle Style
		{
			get
			{
				return style;
			}
		}

		public SequenceStart(string anchor, string tag, bool isImplicit, SequenceStyle style, Mark start, Mark end)
			: base(anchor, tag, start, end)
		{
			this.isImplicit = isImplicit;
			this.style = style;
		}

		public SequenceStart(string anchor, string tag, bool isImplicit, SequenceStyle style)
			: this(anchor, tag, isImplicit, style, Mark.Empty, Mark.Empty)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Sequence start [anchor = {0}, tag = {1}, isImplicit = {2}, style = {3}]", base.Anchor, base.Tag, isImplicit, style);
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
