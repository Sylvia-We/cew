using System;
using System.Collections.Generic;

namespace frz
{
	public class Variable
	{
		private string name;
		private List<String> values;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public Variable(string name, List<String> values)
		{
			this.name = name;
			this.values = values;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public Variable(Variable v)
		{
			this.name = v.Name;
			this.values = new List<string>(Functions.DeepClone(v.Values));
		}

		public String Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public List<String> Values
		{
			get { return this.values; }
			set { this.values = value; }
		}

		/// <summary>
		/// Gibt Variable als String zurück.
		/// </summary>
		public string Print()
		{
			string prt = "[name] " + this.name + Environment.NewLine + "[values] ";
			foreach (string s in this.values)
			{
				prt += s + Environment.NewLine;
			}
			return prt;
		}
	}
}

