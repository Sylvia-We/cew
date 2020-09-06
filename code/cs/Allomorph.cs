using System;
using System.Collections.Generic;

namespace frz
{
	[Serializable]
	public class Allomorph
	{
		private List<Sign> allo;
		private List<Sign> lemma;
		private string tbl;
		private string[] cats;
		private String pos;
		private Dictionary<String, String> additional = new Dictionary<String, String>();

		/// <summary>
		/// Konstruktor für potentielle Allomorphe
		/// </summary>
		public Allomorph()
		{
			this.allo = new List<Sign>();
		}

		/// <summary>
		/// Konstruktor, um Allomorph in Trie zu speichern.
		/// </summary>
		public Allomorph(List<Sign> allo, List<Sign> lemma, string tbl, string[] cats, Dictionary<String, String> additional, String pos)
		{
			this.allo = allo;
			this.lemma = lemma;
			this.pos = pos;
			this.tbl = tbl;
			this.cats = cats;
			this.additional = additional;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public Allomorph(Allomorph am)
		{
			this.allo = new List<Sign>(Functions.DeepClone(am.Allo));
			this.lemma = new List<Sign>(Functions.DeepClone(am.Lemma));
			this.tbl = am.Tbl;
			this.pos = am.pos;
			this.cats = am.Cats;
			this.additional = new Dictionary<string, string>(am.Additional);
		}

		public List<Sign> Allo
		{
			get { return this.allo; }
			set { this.allo = value; }
		}

		public List<Sign> Lemma
		{
			get { return lemma; }
			set { this.lemma = value; }
		}

		public string Tbl
		{
			get { return tbl; }
			set { this.tbl = value; }
		}


		public String Pos
		{
			get { return pos; }
			set { this.pos = value; }
		}

		public string[] Cats
		{
			get { return cats; }
			set { this.cats = value; }
		}

		/// <summary>
		/// Weitere Attribute.
		/// </summary>
		public Dictionary<String, String> Additional
		{
			get { return this.additional; }
			set { this.additional = value; }
		}

		/// <summary>
		/// Vergleicht die Kategorie des Allomorphs mit einer anderen Kategorie (Typ string).
		/// </summary>
		public bool CatsEqual(string s)
		{
			foreach (string str in Cats)
			{
				if (str == s)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Attribut Allo in graphischer Ausgabe zurückgeben.
		/// </summary>
		public string PrintAllo()
		{
			string output = "";
			foreach (Sign l in this.allo)
			{
				output += l.PrintGraph();
			}
			return output;

		}

		/// <summary>
		/// Allomorph als String zurückgeben.
		/// </summary>
		public string Print()
		{
			string prt = "allo: " + this.PrintAllo() + ", lemma: " + this.lemma + ", tbl: " + this.tbl + ", cats: ";
			foreach (string c in this.cats)
			{
				prt += c + " / ";
			}

			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				prt += String.Format("{0,-30}{1,-13}{2,-20}", "", kvp.Key + ": ", kvp.Value) + Environment.NewLine;
			}

			return prt;
		}
	}
}

