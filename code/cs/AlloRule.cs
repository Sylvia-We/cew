using System;

namespace frz
{
	public class AlloRule
	{
		private string patBefore;
		private string patAfter;
		private string[] cats;

		public AlloRule(string patBefore, string patAfter, string[] cats)
		{
			this.patBefore = patBefore;
			this.patAfter = patAfter;
			this.cats = cats;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public AlloRule(AlloRule ar)
		{
			this.patBefore = ar.patBefore;
			this.patAfter = ar.patAfter;
			this.cats = ar.cats;
		}

		public string PatBefore
		{
			get { return this.patBefore; }
		}

		public string PatAfter
		{
			get { return this.patAfter; }
		}

		public string[] Cats
		{
			get { return this.cats; }
		}

		/// <summary>
		/// Gibt AlloRule als String zurück.
		/// </summary>
		public string Print()
		{
			string prt = this.patBefore + " -> " + this.patAfter + " { ";
			foreach (string c in this.cats)
			{
				prt += c + "/";
			}
			prt += " }" + Environment.NewLine;
			return prt;
		}
	}
}

