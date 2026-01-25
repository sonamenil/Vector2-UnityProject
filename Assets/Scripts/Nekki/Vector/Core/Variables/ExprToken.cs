using System.Collections.Generic;

namespace Nekki.Vector.Core.Variables
{
	public class ExprToken
	{
		public enum TypeToken
		{
			NOTHING = -1,
			NUMBER = 0,
			VARIABLE = 1,
			OPERATOR = 2,
			UNKNOWN = 3
		}

		public enum OperatorID
		{
			ADD = 0,
			SUB = 1,
			MULT = 2,
			DIV = 3,
			MOD = 4,
			POW = 5,
			ROOT = 6,
			BRACK_L = 7,
			BRACK_R = 8,
			UNKONOWN = 9
		}

		private Variable _Var;

		private TypeToken _Type;

		private OperatorID _Operator;

		private int _Priority;

		public static Dictionary<OperatorID, int> OPERATOR_PRIORITY = new Dictionary<OperatorID, int>();

		public Variable Var
		{
			get
			{
				return _Var;
			}
		}

		public OperatorID Operator
		{
			get
			{
				return _Operator;
			}
		}

		public TypeToken Type
		{
			get
			{
				return _Type;
			}
		}

		public int OperatorPriority
		{
			get
			{
				return OPERATOR_PRIORITY[_Operator] + _Priority * OPERATOR_PRIORITY[OperatorID.BRACK_L];
			}
			set
			{
				_Priority = value;
			}
		}

		private ExprToken()
		{
			_Type = TypeToken.NOTHING;
			_Operator = OperatorID.UNKONOWN;
			_Priority = 1;
			_Var = null;
		}

		public static ExprToken create(Variable p_var, TypeToken p_type, OperatorID p_operator = OperatorID.UNKONOWN)
		{
			ExprToken exprToken = new ExprToken();
			exprToken._Var = p_var;
			exprToken._Type = p_type;
			exprToken._Operator = p_operator;
			return exprToken;
		}

		public static ExprToken create(Variable p_var)
		{
			ExprToken exprToken = new ExprToken();
			exprToken._Var = p_var;
			exprToken._Type = TypeToken.NUMBER;
			exprToken._Operator = OperatorID.UNKONOWN;
			return exprToken;
		}

		public void reset(Variable p_var, TypeToken p_type, OperatorID p_operator = OperatorID.UNKONOWN)
		{
			_Var = p_var;
			_Type = p_type;
			_Operator = p_operator;
		}

		public void reset(Variable p_var)
		{
			_Var = p_var;
			_Type = TypeToken.NUMBER;
			_Operator = OperatorID.UNKONOWN;
		}
	}
}
