using System;
using System.Text.RegularExpressions;
using ExpressionEvaluator;
using System.Collections;
using System.Collections.Generic;

namespace frz
{
	public class SyllableRuleList
	{
		private int startDate;
		private int endDate;
		private List<SyllableRule> syllrulList;

		public SyllableRuleList(String time, List<SyllableRule> syllrulList)
		{
			/* time: 0--300 */
			Match timeRegex = Regex.Match(time, @"^(.*?)--(.*?)$");

			if (timeRegex.Success)
			{
				if (timeRegex.Groups[1].Value.Trim() != "")
				{
					startDate = Int32.Parse(timeRegex.Groups[1].Value.Trim());
				}
				else
				{
					throw new SymbolNotFoundException("Startdatum der Silbentrennregel fehlt.");
				}

				if (timeRegex.Groups[2].Value.Trim() != "")
				{
					endDate = Int32.Parse(timeRegex.Groups[2].Value.Trim());
				}
				else
				{
					throw new SymbolNotFoundException("Enddatum der Silbentrennregel fehlt.");
				}
			}
			else
			{
				if (time == "")
				{
					throw new SymbolNotFoundException("Startdatum und Enddatum der Silbentrennregel fehlen.");
				}
				else
				{
					throw new SymbolNotFoundException("Zeit nicht erkannt: " + time);
				}
			}

			this.syllrulList = syllrulList;
		}

		public SyllableRuleList(SyllableRuleList srl)
		{
			this.startDate = srl.startDate;
			this.endDate = srl.endDate;
			this.syllrulList = srl.syllrulList;
		}

		public int StartDate
		{
			get { return this.startDate; }
			set { this.startDate = value; }
		}

		public int EndDate
		{
			get { return this.endDate; }
			set { this.endDate = value; }
		}

		public List<SyllableRule> SyllrulList
		{
			get { return this.syllrulList; }
			set { this.syllrulList = value; }
		}
	}

	/// <summary>
	/// Syllable rule
	/// V K . K2 V2 {K is K2}
	/// </summary>
	public class SyllableRule
	{
		private string[] pattern;
		private int posOfBorder;
		private string[] conditions;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public SyllableRule(string pattern, string conditions)
		{
			this.pattern = pattern.Replace(" .", "").Split(' ');
			this.posOfBorder = Array.IndexOf(pattern.Split(' '), ".");
			if (conditions == "")
			{
				this.conditions = null;
			}
			else
			{
				this.conditions = conditions.Split(',');
			}
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public SyllableRule(SyllableRule sr)
		{
			this.pattern = sr.Pattern;
			this.posOfBorder = sr.PosOfBorder;
			this.conditions = sr.Conditions;
		}

		public string[] Pattern
		{
			get { return this.pattern; }
			set { this.pattern = value; }
		}

		public int PosOfBorder
		{
			get { return this.posOfBorder; }
			set { this.posOfBorder = value; }
		}

		public string[] Conditions
		{
			get { return this.conditions; }
			set { this.conditions = value; }
		}

		/// <summary>
		/// prüfen, ob Laute auf Muster passen (z.B. o in V)
		/// </summary>
		public bool FindSoundsInPattern(string sound, int positionInPattern)
		{
			if (this.pattern[positionInPattern][0] == '$')
			{
				Variable found = Functions.varListLang1.Find(x => x.Name == this.pattern[positionInPattern].ToString());

				if (found == null)
				{
					throw new SymbolNotFoundException("Variable ist nicht definiert: " + this.pattern[positionInPattern].ToString());
				}
				else
				{

					foreach (string value in found.Values)
					{

						if (value == sound)
						{
							return true;
						}
					}
				}
			}
			else if (this.pattern[positionInPattern][0].ToString() == sound)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Prüft Bedingungen einer SyllableRule.
		/// </summary>
		public bool CheckConditions(List<Sign> partOfWord)
		{
			if (this.conditions == null)
			{
				return true;
			}
			else
			{
				/* Variablen Werte aus partOfWord zuweisen */
				Hashtable varValue = new Hashtable();

				for (int i = 0; i < this.pattern.Length; i++)
				{
					varValue.Add(this.pattern[i], partOfWord[i].Symbol);
				}

				/* V K . K2 V {K is K2} (gg) */
				foreach (string conditionAsString in this.conditions)
				{

					string[] elements = conditionAsString.Trim().Split(' ');
					string[] newElements = new string[elements.Length];
					string condition;

					/* Werte ersetzen und in neues Array schreiben */
					for (int i = 0; i < elements.Length; i++)
					{

						/* Operatoren ersetzen */
						if (elements[i] == "is")
						{
							newElements[i] = "==";
						}
						else if (elements[i] == "isnot")
						{
							newElements[i] = "!=";
						}
						else if (elements[i] == "and")
						{
							newElements[i] = "&&";
						}
						else if (elements[i] == "or")
						{
							newElements[i] = "||";
						}
						else if (varValue.ContainsKey(elements[i]))
						{ /* K == K2 */
							newElements[i] = "\"" + varValue[elements[i]] + "\"";
						}
						else if (Functions.varListLang1.Find(x => x.Name == elements[i].ToString()) != null)
						{ /* K2 == MUTA */
							Variable found = Functions.varListLang1.Find(x => x.Name == elements[i].ToString());

							if (found.Values.Contains(newElements[i - 2].Trim('"')))
							{

								if (newElements[i - 1] == "==")
								{

									newElements[i - 2] = "true";
									newElements[i - 1] = "";
									newElements[i] = "";
								}
								else if (newElements[i - 1] == "!=")
								{

									newElements[i - 2] = "false";
									newElements[i - 1] = "";
									newElements[i] = "";
								}
							}
							else
							{
								if (newElements[i - 1] == "==")
								{

									newElements[i - 2] = "false";
									newElements[i - 1] = "";
									newElements[i] = "";

								}
								else if (newElements[i - 1] == "!=")
								{

									newElements[i - 2] = "true";
									newElements[i - 1] = "";
									newElements[i] = "";
								}
							}

							if (newElements[i] == null)
							{
								return false;
							}
						}
						else
						{
							throw new SymbolNotFoundException("Symbol nicht gefunden: " + elements[i]);
						}
					}

					condition = string.Join("", newElements);
					var expression = new CompiledExpression(condition);
					var result = expression.Eval();
					if (result.ToString() == "False")
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Gibt SyllableRule als String zurück.
		/// </summary>
		public string Print()
		{
			string str = String.Join(" ", this.pattern) + "; Position of Border: " + this.posOfBorder + "; ";
			if (this.conditions != null)
			{
				str += String.Join(" / ", this.conditions);
			}
			return str;
		}
	}
}

