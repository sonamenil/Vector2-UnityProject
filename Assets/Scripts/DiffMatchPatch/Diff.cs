namespace DiffMatchPatch
{
	public class Diff
	{
		public Operation operation;

		public string text;

		public Diff(Operation operation, string text)
		{
			this.operation = operation;
			this.text = text;
		}

		public override string ToString()
		{
			string text = this.text.Replace('\n', '¶');
			return string.Concat("Diff(", operation, ",\"", text, "\")");
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			Diff diff = obj as Diff;
			if (diff == null)
			{
				return false;
			}
			return diff.operation == operation && diff.text == text;
		}

		public bool Equals(Diff obj)
		{
			if (obj == null)
			{
				return false;
			}
			return obj.operation == operation && obj.text == text;
		}

		public override int GetHashCode()
		{
			return text.GetHashCode() ^ operation.GetHashCode();
		}
	}
}
