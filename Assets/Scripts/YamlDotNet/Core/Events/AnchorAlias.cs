using System.Globalization;

namespace YamlDotNet.Core.Events
{
	public class AnchorAlias : ParsingEvent
	{
		private readonly string value;

		internal override EventType Type
		{
			get
			{
				return EventType.Alias;
			}
		}

		public string Value
		{
			get
			{
				return value;
			}
		}

		public AnchorAlias(string value, Mark start, Mark end)
			: base(start, end)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new YamlException(start, end, "Anchor value must not be empty.");
			}
			if (!NodeEvent.anchorValidator.IsMatch(value))
			{
				throw new YamlException(start, end, "Anchor value must contain alphanumerical characters only.");
			}
			this.value = value;
		}

		public AnchorAlias(string value)
			: this(value, Mark.Empty, Mark.Empty)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Alias [value = {0}]", value);
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
