using System;
using System.Collections.Generic;

namespace frz
{
	/// <summary>
	/// Flex tbl.
	/// 
	/// [ 
	/// F_digital 
	/// ]
	/// {m Sg} e -> {f Sg}
	/// {stem2} x -> {m Pl}
	/// {f Sg} s -> {f Pl}
	/// 
	/// </summary>
	public class FlexTbl
	{
		private string tbl;
		private Dictionary<String, String> additional = new Dictionary<string, string>();
		private List<FlexRule> frlist = new List<FlexRule>();

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public FlexTbl(string tbl, Dictionary<String, String> additional, List<FlexRule> frlist)
		{
			this.tbl = tbl;
			this.frlist = frlist;
			this.additional = additional;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public FlexTbl(FlexTbl ft)
		{
			this.tbl = ft.Tbl;
			this.frlist = new List<FlexRule>(Functions.DeepClone(ft.Frlist));
			this.additional = new Dictionary<string, string>(ft.Additional);
		}

		public string Tbl
		{
			get { return this.tbl; }
			set { this.tbl = value; }
		}

		public Dictionary<String, String> Additional
		{
			get { return this.additional; }
			set { this.additional = value; }
		}

		public List<FlexRule> Frlist
		{
			get { return this.frlist; }
			set { this.frlist = value; }
		}

		/// <summary>
		/// Gibt FlexTbl als String zurück.
		/// </summary>
		public string Print()
		{
			string prt = tbl + Environment.NewLine;

			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				prt += String.Format("{0,-30}{1,-13}{2,-20}", "", kvp.Key + ": ", kvp.Value) + Environment.NewLine;
			}

			foreach (FlexRule pat in this.frlist)
			{
				prt += pat.Print();
			}
			return prt;
		}
	}
}

