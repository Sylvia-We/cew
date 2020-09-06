using System;
using System.Collections.Generic;

namespace frz
{
	/// <summary>
	/// Sign. Kann ein graphisches oder ein phonologisches Zeichen sein.
	/// symbol speichert die Repräsentation in der Textdatei.
	/// alternative speichert die Repräsentation in der Ausgabe,
	/// für graphische Zeichen mit Längen, für phonetische als IPA.
	/// </summary>
	[Serializable]
	public class Sign
	{
		private string symbol;
		private bool syllStart;
		private bool syllEnd;
		private bool segmStart;
		private bool segmEnd;
		private bool primStress;
		private bool secStress;
		private List<String> appliedRules = new List<String>();

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public Sign(String symbol)
		{
			this.symbol = symbol;
			this.syllStart = false;
			this.syllEnd = false;
			this.segmStart = false;
			this.segmEnd = false;
		}

		public string Symbol
		{
			get { return symbol; }
			set { this.symbol = value; }
		}

		public bool SyllStart
		{
			get { return syllStart; }
			set { this.syllStart = value; }
		}

		public bool SyllEnd
		{
			get { return syllEnd; }
			set { this.syllEnd = value; }
		}

		public bool SegmStart
		{
			get { return segmStart; }
			set { this.segmStart = value; }
		}

		public bool SegmEnd
		{
			get { return segmEnd; }
			set { this.segmEnd = value; }
		}

		public bool PrimStress
		{
			get { return primStress; }
			set { this.primStress = value; }
		}

		public bool SecStress
		{
			get { return secStress; }
			set { this.secStress = value; }
		}

		public List<String> AppliedRules
		{
			get { return appliedRules; }
			set { this.appliedRules = value; }
		}

		/// <summary>
		/// Vergleicht zwei Sign über "symbol".
		/// </summary>
		public bool IsEqual(Sign y)
		{
			if (ReferenceEquals(this, y))
				return true;
			if (ReferenceEquals(this, null))
				return false;
			if (ReferenceEquals(y, null))
				return false;
			if (this.GetType() != y.GetType())
				return false;
			if (string.Equals(this.Symbol, y.Symbol))
				return true;
			return false;
		}

		/// <summary>
		/// Graphem mit Diakritika.
		/// </summary>
		public String PrintGraph()
		{
			if (Functions.graphOutListLang1.ContainsKey(this.symbol))
			{
				return Functions.graphOutListLang1[this.symbol];
			}
			else
			{
				return this.symbol;
			}
		}

		public string PrintIpa()
		{

			if (Functions.phonOutListLang1.ContainsKey(this.symbol))
			{

				String s = "";
				String[] phons = Functions.phonOutListLang1[this.symbol].Split(',');

				foreach (string p in phons)
				{
					s += Functions.ConvertUnicodeToString(p);
				}
				return s.Normalize();
			}
			else
			{
				return this.symbol;
			}
		}
	}
}

