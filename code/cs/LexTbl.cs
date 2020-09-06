using System;
using System.Collections.Generic;

namespace frz
{
	public class LexTbl
	{
		private string tbl;
		private String pos;
		private Dictionary<String, String> additional = new Dictionary<string, string>();
		private List<List<Sign>> entries = new List<List<Sign>>();

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public LexTbl(string tbl, Dictionary<String, String> additional, List<List<Sign>> entries, String pos)
		{
			this.tbl = tbl;
			this.additional = additional;
			this.entries = entries;
			this.pos = pos;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public LexTbl(LexTbl lt)
		{
			this.tbl = lt.Tbl;
			this.additional = new Dictionary<string, string>(lt.Additional);
			this.entries = new List<List<Sign>>(Functions.DeepClone(lt.Entries));
			this.pos = lt.Pos;
		}

		public string Tbl
		{
			get { return this.tbl; }
		}


		public string Pos
		{
			get { return this.pos; }
		}

		public Dictionary<String, String> Additional
		{
			get { return this.additional; }
			set { this.additional = value; }
		}

		public List<List<Sign>> Entries
		{
			get { return this.entries; }
		}

		/// <summary>
		/// Gibt LexTbl als String zurück.
		/// </summary>
		public string Print()
		{
			string prt = "[tbl] " + this.tbl + Environment.NewLine;


			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				prt += String.Format("{0,-30}{1,-13}{2,-20}", "", kvp.Key + ": ", kvp.Value) + Environment.NewLine;
			}

			prt += "[entries] ";

			foreach (List<Sign> s in this.entries)
			{
				foreach (Sign l in s)
				{
					prt += l.PrintGraph();
				}
				prt += Environment.NewLine;
			}
			return prt;
		}
	}
}

