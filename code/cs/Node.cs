using System.Collections.Generic;

namespace frz
{
	/* Quelle: http://counterhelix.com/2011/08/16/basic-trie-data-structure-implementation/
	   letzter Aufruf: 14.03.2013 */
	public class Node
	{
		private Sign sig;
		private bool last;
		private List<Allomorph> alloList = new List<Allomorph>();
		private Dictionary<Sign, Node> children;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public Node(Sign c)
		{
			children = new Dictionary<Sign, Node>(new SignEqualityComparer());
			alloList = new List<Allomorph>();
			last = false;
			sig = c;
		}

		public Sign Sig
		{
			get { return this.sig; }
			set { this.sig = value; }
		}

		public bool Last
		{
			get { return this.last; }
			set { this.last = value; }
		}

		public List<Allomorph> AlloList
		{
			get
			{
				if (this.last == true)
				{
					return this.alloList;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.last == true)
				{
					this.alloList = value;
				}
				else
				{
					this.alloList = null;
				}
			}
		}

		public Dictionary<Sign, Node> Children
		{
			get { return this.children; }
			set { this.children = value; }
		}

		/// <summary>
		/// gibt Kindknoten mit dem Eintrag "c" zurück
		/// </summary>
		public Node ChildNode(Sign c)
		{
			if (this.children != null)
			{
				if (this.children.ContainsKey(c))
				{
					return this.children[c];
				}
			}
			return null;
		}
	}
}

