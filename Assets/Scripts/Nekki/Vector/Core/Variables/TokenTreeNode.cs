namespace Nekki.Vector.Core.Variables
{
	public class TokenTreeNode
	{
		public TokenTreeNode Left;

		public TokenTreeNode Right;

		public ExprToken Data;

		public int Level;

		public TokenTreeNode(ExprToken p_data, int p_level)
		{
			Left = (Right = null);
			Data = p_data;
			Level = p_level;
		}

		public void DeleteTree()
		{
			if (Left != null)
			{
				Left.DeleteTree();
				Left = null;
			}
			if (Right != null)
			{
				Right.DeleteTree();
				Right = null;
			}
			Data = null;
		}
	}
}
