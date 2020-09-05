using System;
using System.Collections.Generic;

namespace frz
{
	public class AlloTbl
	{
		private string tbl;
		private List<AlloRule> arList = new List<AlloRule>();
		private Dictionary<String, String> additional = new Dictionary<string, string>();

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AlloTbl(string tbl, Dictionary<String, String> additional, List<AlloRule> arList)
		{
			this.tbl = tbl;
			this.arList = arList;
			this.additional = additional;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public AlloTbl(AlloTbl at)
		{
			this.tbl = at.Tbl;
			this.arList = new List<AlloRule>(Functions.DeepClone(at.ArList));
			this.additional = new Dictionary<string, string>(at.Additional);
		}

		public List<AlloRule> ArList
		{
			get { return this.arList; }
		}

		public Dictionary<String, String> Additional
		{
			get { return this.additional; }
			set { this.additional = value; }
		}

		public string Tbl
		{
			get { return this.tbl; }
		}

		/// <summary>
		/// Gibt AlloTbl als String zurück.
		/// </summary>
		public string Print()
		{
			string prt = this.tbl + Environment.NewLine;

			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				prt += String.Format("{0,-30}{1,-13}{2,-20}", "", kvp.Key + ": ", kvp.Value) + Environment.NewLine;
			}

			foreach (AlloRule pat in this.arList)
			{
				prt += pat.Print();
			}
			return prt;
		}
	}
}

