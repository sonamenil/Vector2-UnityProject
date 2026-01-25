using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.GameManagement
{
	public class ReplacementData
	{
		public Variable Name { get; private set; }

		public Variable Filename { get; private set; }

		public ReplacementData(Variable p_name, Variable p_filename)
		{
			Name = p_name;
			Filename = p_filename;
		}
	}
}
