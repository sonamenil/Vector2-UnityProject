using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Variables
{
	public class ExprParser
	{
		private IVariableParent _Parent;

		private string _Expression;

		private int _Position;

		private float _Result;

		private bool _NeedReCallc;

		private TokenTreeNode _Root;

		private List<ExprToken> _TokenList;

		private List<ExprToken> _TokensForRemove;

		private HashSet<string> _VariableName;

		public IVariableParent Parent
		{
			set
			{
				_Parent = value;
				foreach (ExprToken token in _TokenList)
				{
					if (token.Type == ExprToken.TypeToken.VARIABLE)
					{
						token.Var.Parent = value;
					}
				}
			}
		}

		public string Expression
		{
			get
			{
				return _Expression;
			}
		}

		public HashSet<string> RequiredVariables
		{
			get
			{
				return _VariableName;
			}
		}

		private ExprParser()
		{
			if (ExprToken.OPERATOR_PRIORITY.Count == 0)
			{
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.BRACK_L] = 99;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.BRACK_R] = 99;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.ROOT] = 4;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.POW] = 4;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.MOD] = 3;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.MULT] = 3;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.DIV] = 3;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.ADD] = 1;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.SUB] = 2;
				ExprToken.OPERATOR_PRIORITY[ExprToken.OperatorID.UNKONOWN] = 100;
			}
			_Root = null;
			_Position = 0;
			_VariableName = new HashSet<string>();
			_TokenList = new List<ExprToken>();
			_TokensForRemove = new List<ExprToken>();
		}

		public static ExprParser Create(string p_value, IVariableParent p_parent)
		{
			ExprParser exprParser = new ExprParser();
			exprParser._Expression = p_value;
			exprParser._Parent = p_parent;
			exprParser._NeedReCallc = false;
			if (!exprParser.Parse())
			{
				exprParser = null;
				return null;
			}
			exprParser.ApplyTokenRules();
			exprParser.BuildTree(exprParser._TokenList, ref exprParser._Root, 0);
			return exprParser;
		}

		private bool Parse()
		{
			_VariableName.Clear();
			ExprToken exprToken = null;
			while (_Position < _Expression.Length)
			{
				exprToken = GetNextToken();
				if (exprToken == null)
				{
					break;
				}
				_TokenList.Add(exprToken);
			}
			if (_Position < _Expression.Length)
			{
				Clear(true);
				return false;
			}
			return true;
		}

		private void ApplyTokenRules()
		{
			if (_TokenList.Count > 1 && (_TokenList[0].Operator == ExprToken.OperatorID.ADD || _TokenList[0].Operator == ExprToken.OperatorID.SUB))
			{
				_TokenList.Insert(0, ExprToken.create(Variable.CreateVariable("0", "number")));
			}
			int num = _TokenList.Count - 3;
			for (int i = 0; i < num; i++)
			{
				if (_TokenList[i].Operator == ExprToken.OperatorID.BRACK_L && _TokenList[i + 1].Operator == ExprToken.OperatorID.SUB && _TokenList[i + 2].Type == ExprToken.TypeToken.NUMBER && _TokenList[i + 3].Operator == ExprToken.OperatorID.BRACK_R)
				{
					_TokenList.Insert(i + 1, ExprToken.create(Variable.CreateVariable("0", "number")));
					i += 3;
				}
			}
			SetOperatorsPriority();
		}

		private void SetOperatorsPriority()
		{
			int num = 1;
			foreach (ExprToken token in _TokenList)
			{
				if (token.Operator == ExprToken.OperatorID.BRACK_L || token.Operator == ExprToken.OperatorID.BRACK_R)
				{
					num += ((token.Operator == ExprToken.OperatorID.BRACK_L) ? 1 : (-1));
					_TokensForRemove.Add(token);
				}
				else
				{
					token.OperatorPriority = num;
				}
			}
			ForDeleteClear(true);
		}

		public void Trace()
		{
			string text = string.Empty;
			foreach (ExprToken token in _TokenList)
			{
				if (token.Type == ExprToken.TypeToken.OPERATOR)
				{
					switch (token.Operator)
					{
					case ExprToken.OperatorID.ADD:
						text += "+";
						break;
					case ExprToken.OperatorID.SUB:
						text += "-";
						break;
					case ExprToken.OperatorID.MULT:
						text += "*";
						break;
					case ExprToken.OperatorID.DIV:
						text += "/";
						break;
					case ExprToken.OperatorID.POW:
						text += "^";
						break;
					case ExprToken.OperatorID.ROOT:
						text += "|";
						break;
					case ExprToken.OperatorID.BRACK_L:
						text += "(";
						break;
					case ExprToken.OperatorID.BRACK_R:
						text += ")";
						break;
					}
					string text2 = text;
					text = text2 + "[" + token.OperatorPriority + "]";
				}
				else
				{
					text = ((token.Type != ExprToken.TypeToken.VARIABLE) ? (text + token.Var.ValueFloat) : (text + token.Var.ValueString));
				}
			}
			Debug.Log(text);
		}

		public void Log()
		{
			Log(_Root);
		}

		private void Log(TokenTreeNode p_node)
		{
			if (p_node != null)
			{
				if (p_node.Data.Type == ExprToken.TypeToken.VARIABLE || p_node.Data.Type == ExprToken.TypeToken.NUMBER)
				{
					p_node.Data.Var.Log("Var");
				}
				Log(p_node.Left);
				Log(p_node.Right);
			}
		}

		private void BuildTree(List<ExprToken> p_tokens, ref TokenTreeNode p_node, int p_level)
		{
			if (p_tokens.Count != 0)
			{
				int minPriorityTokenIndex = GetMinPriorityTokenIndex(p_tokens);
				p_node = new TokenTreeNode(p_tokens[minPriorityTokenIndex], p_level);
				p_level++;
				List<ExprToken> list = new List<ExprToken>();
				List<ExprToken> list2 = new List<ExprToken>();
				for (int i = 0; i < minPriorityTokenIndex; i++)
				{
					list.Add(p_tokens[i]);
				}
				for (int j = minPriorityTokenIndex + 1; j < p_tokens.Count; j++)
				{
					list2.Add(p_tokens[j]);
				}
				if (list.Count != 0)
				{
					BuildTree(list, ref p_node.Left, p_level);
				}
				if (list2.Count != 0)
				{
					BuildTree(list2, ref p_node.Right, p_level);
				}
			}
		}

		private static int GetMinPriorityTokenIndex(List<ExprToken> p_tokens)
		{
			int num = int.MaxValue;
			int result = 0;
			for (int num2 = p_tokens.Count - 1; num2 >= 0; num2--)
			{
				int operatorPriority = p_tokens[num2].OperatorPriority;
				if (operatorPriority < num)
				{
					num = operatorPriority;
					result = num2;
				}
			}
			return result;
		}

		public bool TryGetResult(ref float p_result)
		{
			p_result = CalcTree(_Root);
			Clear(false);
			return !_NeedReCallc;
		}

		private float CalcTree(TokenTreeNode p_node)
		{
			if (p_node != null)
			{
				if (p_node.Data.Type == ExprToken.TypeToken.OPERATOR)
				{
					return GetOperatorResult(p_node.Data.Operator, p_node.Left, p_node.Right);
				}
				switch (p_node.Data.Var.Type)
				{
				case VariableType.Float:
				case VariableType.Function:
				case VariableType.Expression:
				case VariableType.ObscuredFloat:
					return p_node.Data.Var.ValueFloat;
				default:
					return p_node.Data.Var.ValueInt;
				}
			}
			return 0f;
		}

		private float GetOperatorResult(ExprToken.OperatorID p_operator, TokenTreeNode p_left, TokenTreeNode p_right)
		{
			switch (p_operator)
			{
			case ExprToken.OperatorID.ADD:
				return CalcTree(p_left) + CalcTree(p_right);
			case ExprToken.OperatorID.SUB:
				return CalcTree(p_left) - CalcTree(p_right);
			case ExprToken.OperatorID.MULT:
				return CalcTree(p_left) * CalcTree(p_right);
			case ExprToken.OperatorID.DIV:
				return CalcTree(p_left) / CalcTree(p_right);
			case ExprToken.OperatorID.MOD:
				return CalcTree(p_left) % CalcTree(p_right);
			case ExprToken.OperatorID.POW:
				return (float)Math.Pow(CalcTree(p_left), CalcTree(p_right));
			default:
				return 0f;
			}
		}

		private ExprToken GetNextToken()
		{
			if (_Position >= _Expression.Length)
			{
				return null;
			}
			if (isDelimeter(_Expression[_Position]))
			{
				return GetOperatorToken();
			}
			if (isDigit(_Expression[_Position]))
			{
				return GetNumberToken();
			}
			if (isLetter(_Expression[_Position]))
			{
				return GetVariableToken();
			}
			return null;
		}

		private ExprToken GetOperatorToken()
		{
			ExprToken.OperatorID p_operator = ExprToken.OperatorID.UNKONOWN;
			switch (_Expression[_Position++])
			{
			case '+':
				p_operator = ExprToken.OperatorID.ADD;
				break;
			case '-':
				p_operator = ExprToken.OperatorID.SUB;
				break;
			case '*':
				p_operator = ExprToken.OperatorID.MULT;
				break;
			case '/':
				p_operator = ExprToken.OperatorID.DIV;
				break;
			case '%':
				p_operator = ExprToken.OperatorID.MOD;
				break;
			case '^':
				p_operator = ExprToken.OperatorID.POW;
				break;
			case '|':
				p_operator = ExprToken.OperatorID.ROOT;
				break;
			case '(':
				p_operator = ExprToken.OperatorID.BRACK_L;
				break;
			case ')':
				p_operator = ExprToken.OperatorID.BRACK_R;
				break;
			}
			return ExprToken.create(null, ExprToken.TypeToken.OPERATOR, p_operator);
		}

		private ExprToken GetNumberToken()
		{
			string text = string.Empty;
			while (_Expression.Length > _Position && isDigit(_Expression[_Position]))
			{
				text += _Expression[_Position++];
			}
			return ExprToken.create(Variable.CreateVariable(text, "number"));
		}

		private ExprToken GetVariableToken()
		{
			string text = string.Empty;
			while (_Expression.Length > _Position && (isLetter(_Expression[_Position]) || isDigit(_Expression[_Position])))
			{
				text += _Expression[_Position++];
			}
			_VariableName.Add(text);
			_NeedReCallc = true;
			return ExprToken.create(Variable.CreateVariable(text, "var", _Parent), ExprToken.TypeToken.VARIABLE);
		}

		private bool isDelimeter(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return "+-/*%^!()".IndexOf(c) != -1;
		}

		private bool isDigit(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return "0123456789.".IndexOf(c) != -1;
		}

		private bool isLetter(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return !isDigit(c) && !isDelimeter(c);
		}

		public void AddVariable(string p_name, Variable p_var)
		{
			if (p_name.Contains("?"))
			{
				return;
			}
			foreach (ExprToken token in _TokenList)
			{
				if (token.Var != null && token.Var.IsStringVar && token.Var.ValueString == p_name)
				{
					token.reset(p_var);
				}
			}
		}

		private void ForDeleteClear(bool p_destroy = true)
		{
			if (_TokensForRemove.Count == 0)
			{
				return;
			}
			foreach (ExprToken item in _TokensForRemove)
			{
				_TokenList.Remove(item);
			}
			_TokensForRemove.Clear();
		}

		private void Clear(bool p_fromDestructor = false)
		{
			ForDeleteClear(true);
			if (p_fromDestructor)
			{
				_TokenList.Clear();
				if (_Root != null)
				{
					_Root.DeleteTree();
				}
			}
		}
	}
}
