using System.Collections.Generic;

namespace frz
{
	public class GraphPhon
	{
		private string graph;
		private string phon;
		private List<AttributeValue> conditionList = new List<AttributeValue>();

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public GraphPhon(string graph, string phon, List<AttributeValue> attributeValue)
		{
			this.graph = graph;
			this.phon = phon;
			this.conditionList = new List<AttributeValue>(attributeValue);
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public GraphPhon(GraphPhon gp)
		{
			this.graph = gp.Graph;
			this.phon = gp.Phon;
			this.conditionList = new List<AttributeValue>(Functions.DeepClone(gp.ConditionList));
		}

		public string Graph
		{
			get { return this.graph; }
			set { this.graph = value; }
		}

		public string Phon
		{
			get { return this.phon; }
			set { this.phon = value; }
		}

		public List<AttributeValue> ConditionList
		{
			get { return this.conditionList; }
			set { this.conditionList = value; }
		}

		/// <summary>
		/// Gibt GraphPhon als String zurück.
		/// </summary>
		public string Print()
		{
			string print = " Graphem: " + this.graph + ", Phon: " + this.phon;

			if (this.conditionList.Count != 0)
			{
				print += ", Attribute: ";
				foreach (AttributeValue aw in this.conditionList)
				{
					print += aw.Print() + " ";
				}
			}
			return print;
		}
	}
}

