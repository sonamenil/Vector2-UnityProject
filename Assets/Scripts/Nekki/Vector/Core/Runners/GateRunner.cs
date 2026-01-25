namespace Nekki.Vector.Core.Runners
{
	public class GateRunner : Runner
	{
		public enum Type
		{
			In = 0,
			Out = 1
		}

		private Type _TypeGate;

		public bool IsIn
		{
			get
			{
				return _TypeGate == Type.In;
			}
		}

		public bool IsOut
		{
			get
			{
				return _TypeGate == Type.Out;
			}
		}

		public GateRunner(float p_x, float p_y, string p_name, bool p_isIn, Element p_elements)
			: base(p_x, p_y, p_elements)
		{
			_TypeClass = TypeRunner.Gate;
			_TypeGate = ((!p_isIn) ? Type.Out : Type.In);
			_Name = p_name;
		}

		public override bool Render()
		{
			return true;
		}
	}
}
