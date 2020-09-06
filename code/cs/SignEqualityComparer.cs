using System.Collections.Generic;

namespace frz
{
	/// <summary>
	/// Vergleicht zwei Sign.
	/// </summary>
	public sealed class SignEqualityComparer : IEqualityComparer<Sign>
	{
		/// <summary>
		/// Vergleicht zwei Sign über "symbol".
		/// </summary>
		public bool Equals(Sign x, Sign y)
		{
			if (ReferenceEquals(x, y))
				return true;
			if (ReferenceEquals(x, null))
				return false;
			if (ReferenceEquals(y, null))
				return false;
			if (x.GetType() != y.GetType())
				return false;
			return string.Equals(x.Symbol, y.Symbol);
		}

		public int GetHashCode(Sign obj)
		{
			unchecked
			{
				return ((obj.Symbol != null ? obj.Symbol.GetHashCode() : 0) * 397) ^ 0;
			}
		}
	}
}

