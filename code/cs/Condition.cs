
namespace frz
{
	/// <summary>
	/// Bedingungen in Betonungsregeln
	/// Accent: 
	/// 	Y: ^.*:.*$ ; ...
	/// 	attribute: Y
	/// 	values: ^.*:.*$ ; ...
	/// </summary>
	public class Condition
	{
		private string attribute;
		private string value;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public Condition(string attribute, string value)
		{
			this.attribute = attribute;
			this.value = value;
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
	}
}

