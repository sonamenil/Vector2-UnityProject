using System.Collections.Generic;
using System.Xml;

public class QualityCondition
{
	private readonly List<ComparisonExpression> _expressions = new List<ComparisonExpression>();

	public string Name { get; private set; }

	public QualityCondition(XmlNode node)
	{
		if (node.Attributes != null)
		{
			Name = node.Attributes["Name"].Value;
		}
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			_expressions.Add(new ComparisonExpression(node.ChildNodes[i]));
		}
	}

	public bool IsTrue()
	{
		for (int i = 0; i < _expressions.Count; i++)
		{
			if (!_expressions[i].Compare())
			{
				return false;
			}
		}
		return true;
	}
}
