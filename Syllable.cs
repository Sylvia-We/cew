using System;
using System.Collections.Generic;

namespace frz
{
	[Serializable]
	public class Syllable
	{
		private bool primStress;
		private bool secStress;
		private List<Sign> syll;

		public Syllable()
		{
			this.syll = new List<Sign>();
		}

		/// <summary>
		/// Kopierkonstruktor
		/// </summary>
		public Syllable(Syllable s)
		{
			this.primStress = s.primStress;
			this.secStress = s.secStress;
			this.syll = new List<Sign>(s.syll);
		}

		public Syllable(List<Sign> syll)
		{
			this.syll = syll;
		}

		public bool PrimStress
		{
			get { return this.primStress; }
			set { this.primStress = value; }
		}

		public bool SecStress
		{
			get { return this.secStress; }
			set { this.secStress = value; }
		}

		public List<Sign> Syll
		{
			get { return this.syll; }
			set { this.syll = value; }
		}

		public Syllable Concat(Syllable syll2)
		{
			Syllable both = new Syllable();
			both.Syll.AddRange(this.syll);
			both.Syll.AddRange(syll2.syll);
			return both;
		}
	}
}

