using System.Xml;
using UnityEngine;

public class ComparisonExpression
{
	public enum ComparisonType
	{
		COMPARISON_NONE = 0,
		COMPARISON_EQUAL = 1,
		COMPARISON_GREATER = 2,
		COMPARISON_GREATER_EQUAL = 3,
		COMPARISON_LESS = 4,
		COMPARISON_LESS_EQUAL = 5
	}

	private const float FLT_VALUE_DELTA = 1E-05f;

	protected bool _isTrue;

	protected ComparisonType _comparisonType;

	protected float _first;

	protected float _second;

	public ComparisonExpression(XmlNode node)
	{
		_comparisonType = GetTypeFromString(node.Name);
		if (node.Attributes != null)
		{
			_isTrue = !bool.Parse(node.Attributes["Not"].Value);
		}
	}

	public bool Compare()
	{
		bool flag = true;
		switch (_comparisonType)
		{
		case ComparisonType.COMPARISON_EQUAL:
			flag = Mathf.Abs(_first - _second) < 1E-05f;
			break;
		case ComparisonType.COMPARISON_GREATER:
			flag = _first - _second > 1E-05f;
			break;
		case ComparisonType.COMPARISON_GREATER_EQUAL:
			flag = _first - _second > -1E-05f;
			break;
		case ComparisonType.COMPARISON_LESS:
			flag = _first - _second < -1E-05f;
			break;
		case ComparisonType.COMPARISON_LESS_EQUAL:
			flag = _first - _second < 1E-05f;
			break;
		}
		return (!_isTrue) ? (!flag) : flag;
	}

	public static ComparisonType GetTypeFromString(string name)
	{
		switch (name)
		{
		case "Equal":
			return ComparisonType.COMPARISON_EQUAL;
		case "Greater":
			return ComparisonType.COMPARISON_GREATER;
		case "GreaterEqual":
			return ComparisonType.COMPARISON_GREATER_EQUAL;
		case "Less":
			return ComparisonType.COMPARISON_LESS;
		case "LessEqual":
			return ComparisonType.COMPARISON_LESS_EQUAL;
		default:
			return ComparisonType.COMPARISON_NONE;
		}
	}
}
