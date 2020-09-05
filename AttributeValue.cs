using System;

namespace frz
{
	public class AttributeValue
	{
		private string attribute;
		private string value;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AttributeValue(string attribute, string value)
		{
			this.attribute = attribute;
			this.value = value;
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public AttributeValue(AttributeValue av)
		{
			this.attribute = av.Attribute;
			this.value = av.Value;
		}

		public string Attribute
		{
			get { return this.attribute; }
			set { this.attribute = value; }
		}

		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		/// <summary>
		/// Gibt AttributeValue als String zurück.
		/// </summary>
		public string Print()
		{
			string print = " Attribut: " + this.attribute + ", Wert: " + this.value + Environment.NewLine;
			return print;
		}
	}
}

