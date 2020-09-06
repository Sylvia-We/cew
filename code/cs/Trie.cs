using System;
using System.Collections.Generic;

namespace frz
{
	/* Quelle: http://counterhelix.com/2011/08/16/basic-trie-data-structure-implementation/
	   letzter Aufruf: 14.03.2013 */
	public class Trie
	{
		private Node root;

		/// <summary>
		/// Konstruktor generiert Wurzel mit leerem Knoten
		/// </summary>
		public Trie ()
		{
			Sign l = new Sign (" ");
			root = new Node (l);
		}

		/// <summary>
		/// Neuen Knoten einfügen.
		/// </summary>
		public void Insert (List<Sign> ll, string tree, AlloTbl atbl, List<Sign> lemma, string[] cats, Dictionary<String,String> additional, String pos)
		{
			string tbl;

			if (tree.Equals ("")) {
				tbl = atbl.Tbl;
			} else {
				tbl = tree;
			}

			Node current = this.root;
	 
			if (ll.Count == 0) {
				current.Last = true;
			}
	 
			for (int i = 0; i < ll.Count; i++) {
				Node child = current.ChildNode (ll [i]);
				if (child != null) {
					current = child;
				} else {
					current.Children.Add (ll [i], new Node (ll [i]));
					current = current.ChildNode (ll [i]);
				}
	 
				if (i == ll.Count - 1) {

					current.Last = true;
					Allomorph allo = new Allomorph (ll, lemma, tbl, cats, additional, pos);
					current.AlloList.Add (allo);
					
				
				}
			}
		}

		/// <summary>
		/// Suche String in Trie und gebe Liste der Allomorphe zurück.
		/// </summary>
		public List<Allomorph> Search (List<Sign> s)
		{

			Node current = this.root;
			while (current != null) {
				for (int i = 0; i < s.Count; i++) {
					if (current.ChildNode (s [i]) == null)
						return null;
					else
						current = current.ChildNode (s [i]);
				}
	 
				if (current.Last == true) {
				
					return current.AlloList;
				} else {
					return null;
				}
			}
			return null;
		}
	}
}



