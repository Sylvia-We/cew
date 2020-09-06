using System.Collections.Generic;
using System;

namespace frz
{
	/// <summary>
	/// Flex pattern.
	/// {m Sg} s -> {m Pl}
	/// </summary>
	public class FlexRule
	{

		private string catBefore;
		private List<Sign> affix;
		private string catAfter;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public FlexRule(string catBefore, List<Sign> affix, string catAfter)
		{
			this.catBefore = catBefore;
			this.affix = affix;
			this.catAfter = catAfter;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public FlexRule(FlexRule fr)
		{
			this.catBefore = fr.CatBefore;
			this.affix = new List<Sign>(Functions.DeepClone(fr.Affix));
			this.catAfter = this.CatAfter;
		}

		public string CatBefore
		{
			get { return this.catBefore; }
		}

		public List<Sign> Affix
		{
			get { return this.affix; }
		}

		public string CatAfter
		{
			get { return this.catAfter; }
		}

		/// <summary>
		/// Gibt FlexRule als String zurück.
		/// </summary>
		public string Print()
		{
			string prt = " { " + this.catBefore + " } " + this.affix + " -> " + " { " + this.catAfter + " } " + Environment.NewLine;
			return prt;
		}
	}
}

