using System;

namespace frz
{
	public class MyTreeNode
	{
		public String arrow;
		public String input;
		public String analysis1;
		public String analysis2;
		public String color = "white";
		public int inputBold = (int)Pango.Weight.Normal;
		public int inputSize = Convert.ToInt32 (9 * Pango.Scale.PangoScale);
		public int analysisSize = Convert.ToInt32 (8 * Pango.Scale.PangoScale);

		public MyTreeNode (String arrow, String input, String analysis1, String analysis2)
		{
			this.arrow = arrow;
			this.input = input;
			this.analysis1 = analysis1;
			this.analysis2 = analysis2;
		}
	}
}

