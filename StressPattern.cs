using System;

namespace frz
{
	public class StressPattern
	{
		String syllSymbol;
		bool primStress;
		bool secStress;
		bool startOfWord;
		bool endOfWord;

		public StressPattern (String syllSymbol, bool primStress, bool secStress, bool startOfWord, bool endOfWord)
		{
			this.syllSymbol = syllSymbol;
			this.primStress = primStress;
			this.secStress = secStress;
			this.startOfWord = startOfWord;
			this.endOfWord = endOfWord;
		}

		public String SyllSymbol {
			get{ return this.syllSymbol; }
			set{ this.syllSymbol = value;}
		}

		public bool PrimStress {
			get{ return this.primStress; }
			set{ this.primStress = value;}
		}

		public bool SecStress {
			get{ return this.secStress; }
			set{ this.secStress = value;}
		}

		public bool StartOfWord {
			get{ return this.startOfWord; }
			set{ this.startOfWord = value;}
		}

		public bool EndOfWord {
			get{ return this.endOfWord; }
			set{ this.endOfWord = value;}
		}

		public String Print ()
		{
			if (primStress == true) 
				return "\u0022" + syllSymbol;
			else if (secStress == true) 
				return "%" + syllSymbol;
			else
				return syllSymbol;
		}
	}
}

