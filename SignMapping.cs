using System;
using System.Collections.Generic;

namespace frz
{
	[Serializable]
	public class SignMapping
	{
		private List<int?> graphPos;
		private List<int?> phonPos;
		private List<List<int?>> syllPos = new List<List<int?>>();

		public SignMapping(List<int?> graphPos, List<int?> phonPos)
		{
			this.graphPos = graphPos;
			this.phonPos = phonPos;
		}

		public List<int?> GraphPos
		{
			get { return this.graphPos; }
			set { this.graphPos = value; }
		}

		public List<int?> PhonPos
		{
			get { return this.phonPos; }
			set { this.phonPos = value; }
		}

		/// <summary>
		/// Positionen in der Silbe. Äußere Liste enthält mehrere Elemente, wenn an der Stelle des Mappings mehrere Signs stehen. 
		/// Der erste Wert in der List bestimmt die Silbe, der zweite die Position in der Silbe.
		/// </summary>
		/// <value>Silbenposition {0,1}: 1. Silbe, 2. Zeichen</value>
		public List<List<int?>> SyllPos
		{
			get { return this.syllPos; }
			set { this.syllPos = value; }
		}
	}
}

