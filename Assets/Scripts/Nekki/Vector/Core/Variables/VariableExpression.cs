using UnityEngine;

namespace Nekki.Vector.Core.Variables
{
	public class VariableExpression : Variable
	{
		private ExprParser _Parser;

		private float _Result;

		private bool _IsCalculated;

		public override IVariableParent Parent
		{
			set
			{
				base.Parent = value;
				_Parser.Parent = value;
			}
		}

		public bool IsCalculated
		{
			get
			{
				return _IsCalculated;
			}
		}

		public override int ValueInt
		{
			get
			{
				CalculateExpression();
				return (int)_Result;
			}
		}

		public override float ValueFloat
		{
			get
			{
				CalculateExpression();
				return _Result;
			}
		}

		public override string ValueString
		{
			get
			{
				CalculateExpression();
				return _Result.ToString();
			}
		}

		public override string DebugStringValue
		{
			get
			{
				return _Parser.Expression;
			}
		}

		protected internal VariableExpression(string p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			_Type = VariableType.Expression;
			p_value = p_value.Replace("{", string.Empty);
			p_value = p_value.Replace("}", string.Empty);
			p_value = p_value.Replace(" ", string.Empty);
			_Parser = ExprParser.Create(p_value, p_parent);
			if (_Parser == null)
			{
				Debug.LogError("TRIGGER EXPRESSION: expression is incorrect!");
			}
			else if (_Parser.RequiredVariables.Count == 0)
			{
				_IsCalculated = _Parser.TryGetResult(ref _Result);
			}
			else
			{
				InitVariables();
			}
		}

		private void InitVariables()
		{
			if (_Parser.RequiredVariables.Count == 0)
			{
				return;
			}
			foreach (string requiredVariable in _Parser.RequiredVariables)
			{
				Variable p_var;
				if (Variable.GetTypeByNameAndValue(string.Empty, requiredVariable) == VariableType.Function)
				{
					p_var = Variable.CreateVariable(requiredVariable, string.Empty, _Parent);
					_Parser.AddVariable(requiredVariable, p_var);
					continue;
				}
				p_var = _Parent.GetVariable(requiredVariable);
				if (p_var != null)
				{
					_Parser.AddVariable(requiredVariable, p_var);
				}
				else
				{
					Debug.LogError("In Expression {" + _Parser.Expression + "} unknown var= " + requiredVariable);
				}
			}
			_Parser.RequiredVariables.Clear();
		}

		public void CalculateExpression()
		{
			if (!_IsCalculated)
			{
				InitVariables();
				_IsCalculated = _Parser.TryGetResult(ref _Result);
			}
		}

		public override void Log(string name)
		{
			VectorLog.RunLog((!string.IsNullOrEmpty(base.Name)) ? string.Format("{0}: {1}: {2}: {3}", name, base.Name, _Parser.Expression, ValueFloat) : string.Format("{0}: {1}: {2}", name, _Parser.Expression, ValueFloat));
			VectorLog.Tab(1);
			_Parser.Log();
			VectorLog.Untab(1);
		}
	}
}
