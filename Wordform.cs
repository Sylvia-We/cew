using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace frz
{
	/// <summary>
	/// Wortform.
	/// Merkmale einer Wortform
	/// </summary>
	[Serializable]
	public class Wordform
	{
		/// <summary>
		/// Graphisch wie in Textdatei
		/// </summary>
		private List<Sign> word;
		private List<Sign> lemma;
		/// <summary>
		/// Phonologisch wie in Textdatei (Sampa).
		/// </summary>
		private List<Sign> phonetic;
		private List<Sign> suffix;
		private List<int> suffixPos;
		private String cat;
		private String tbl;
		private String lang;
		private String pos;
		private List<Syllable> syllables;
		private int syllNum;
		private int? time;
		private int? rootTime;
		private List<String> appliedRules = new List<String>();
		private List<SignMapping> mappingList = new List<SignMapping>();
		private Dictionary<String, String> additional = new Dictionary<string, string>();
		private List<Change> lastChanges = new List<Change>();
		private SoundChangeRule printRule;
		public List<String> repeatRules = new List<String>();
		private int multiPathNum = 0;
		List<SoundChangeRule> sortedRulList = new List<SoundChangeRule>();
		String printColor;

		/// <summary>
		/// Konsruktor: Wortform für Input
		/// </summary>
		public Wordform(string word)
		{
			this.word = Functions.ConvertStringToSignList(word);
		}

		/// <summary>
		/// Kopierkonstruktor mit tiefer Kopie.
		/// </summary>
		public Wordform(Wordform wf)
		{
			this.word = new List<Sign>(Functions.DeepClone(wf.Word));
			if (wf.Lemma != null)
				this.lemma = new List<Sign>(Functions.DeepClone(wf.Lemma));
			if (wf.Phonetic != null)
				this.phonetic = new List<Sign>(Functions.DeepClone(wf.Phonetic));
			if (!String.IsNullOrEmpty(wf.Pos))
				this.pos = wf.Pos;
			if (!String.IsNullOrEmpty(wf.Cat))
				this.cat = wf.Cat;
			if (!String.IsNullOrEmpty(wf.Tbl))
				this.tbl = wf.Tbl;
			if (!String.IsNullOrEmpty(wf.Lang))
				this.lang = wf.Lang;
			if (wf.Syllables != null)
				this.syllables = wf.Syllables;
			this.syllNum = wf.SyllNum;
			if (wf.Time != null)
				this.time = wf.Time;
			if (wf.Additional != null)
				this.additional = new Dictionary<string, string>(wf.Additional);
			if (wf.MappingList != null)
				this.mappingList = new List<SignMapping>(wf.MappingList);
			// Vorsicht: flache Kopie:
			if (wf.PrintRule != null)
				this.printRule = wf.PrintRule;
			if (wf.lastChanges != null)
				this.lastChanges = new List<Change>(Functions.DeepClone(wf.lastChanges));
			if (wf.Suffix != null)
				this.suffix = Functions.DeepClone(wf.Suffix);
			if (wf.SuffixPos != null)
				this.suffixPos = Functions.DeepClone(wf.SuffixPos);
			this.multiPathNum = wf.multiPathNum;
		}

		public List<Sign> Word
		{
			get { return this.word; }
		}

		public List<Sign> Lemma
		{
			get { return this.lemma; }
			set { this.lemma = value; }
		}

		public List<Sign> Phonetic
		{
			get { return this.phonetic; }
			set { this.phonetic = value; }
		}

		public String Cat
		{
			get { return this.cat; }
			set { this.cat = value; }
		}

		public String Tbl
		{
			get { return this.tbl; }
			set { this.tbl = value; }
		}


		public String Pos
		{
			get { return this.pos; }
			set { this.pos = value; }
		}

		public String Lang
		{
			get { return this.lang; }
			set { this.lang = value; }
		}

		public Dictionary<String, String> Additional
		{
			get { return this.additional; }
			set { this.additional = value; }
		}

		public List<Syllable> Syllables
		{
			get { return this.syllables; }
		}

		public int SyllNum
		{
			get { return this.syllNum; }
		}

		public int? Time
		{
			get { return this.time; }
			set { this.time = value; }
		}

		public int? RootTime
		{
			get { return this.rootTime; }
			set { this.rootTime = value; }
		}

		public List<String> AppliedRules
		{
			get { return this.appliedRules; }
			set { this.appliedRules = value; }
		}

		public List<SignMapping> MappingList
		{
			get { return this.mappingList; }
			set { this.mappingList = value; }
		}

		public List<Change> LastChanges
		{
			get { return this.lastChanges; }
			set { this.lastChanges = value; }
		}

		public String PrintColor
		{
			get { return this.printColor; }
			set { this.printColor = value; }
		}

		public SoundChangeRule PrintRule
		{
			get { return this.printRule; }
			set { this.printRule = value; }
		}

		public List<String> RepeatRules
		{
			get { return this.repeatRules; }
			set { this.repeatRules = value; }
		}

		public List<Sign> Suffix
		{
			get { return this.suffix; }
			set { this.suffix = value; }
		}

		public List<int> SuffixPos
		{
			get { return this.suffixPos; }
			set { this.suffixPos = value; }
		}

		public int MultiPathNum
		{
			get { return this.multiPathNum; }
			set { this.multiPathNum = value; }
		}

		public List<SoundChangeRule> SortedRuleList
		{
			get { return this.sortedRulList; }
			set { this.sortedRulList = value; }
		}

		/// <summary>
		/// Fügt Attribute zur Wordform hinzu.
		/// </summary>
		public void AddAttributes(List<Sign> lemma, string cat, string tbl, string lang, int time, Dictionary<String, String> additional,
								   MainWindow win_, List<Sign> stem, List<List<Sign>> affixes, ref List<Sign> suffix, String pos,
								 List<int> suffixPos)
		{
			this.lemma = lemma;
			this.cat = cat;
			this.pos = pos;
			this.tbl = tbl;
			this.lang = lang;
			this.time = time;
			this.rootTime = time;

			foreach (KeyValuePair<string, string> entry in additional)
			{
				if (entry.Key == "Wortart")
				{
					this.pos = entry.Value;
				}
				else
				{
					this.additional.Add(entry.Key, entry.Value);
				}
			}

			/* Wortgrenzen einfügen */
			this.Word.Insert(0, new Sign("#"));
			this.Word.Add(new Sign("#"));

			if (lang == "lang1")
			{
				this.phonetic = Functions.TransformGraphToPhon(this, 0, 0, lang);

				if (this.phonetic != null)
				{
					this.SeparateSyllables();
					this.FindStress();
				}

				if (suffix != null)
				{
					this.suffix = suffix;
					this.suffixPos = suffixPos;
				}
			}
		}

		public Wordform CloneWfObjectExtensions()
		{
			Wordform wf2 = ObjectExtensions.Copy(this);
			if (this.suffix != null)
			{
				List<Sign> suffixCopy = this.suffix;
				this.suffix = wf2.Word.GetRange(wf2.Word.Count - suffixCopy.Count - 1, suffixCopy.Count);
				this.suffixPos = new List<int> { wf2.Word.Count - suffixCopy.Count - 1,
					wf2.Word.Count - suffixCopy.Count - 1 + suffixCopy.Count -1};
			}
			return wf2;
		}

		/// <summary>
		/// Silbentrennung
		/// </summary>
		public void SeparateSyllables()
		{
			/* alle gesetzten Silben zurücksetzen */
			this.syllables = new List<Syllable>();

			foreach (Sign s in this.phonetic)
			{
				s.SyllStart = false;
				s.SyllEnd = false;
			}

			foreach (Sign s in this.word)
			{
				s.SyllStart = false;
				s.SyllEnd = false;
			}

			SyllableRuleList syllrulList = null;

			foreach (SyllableRuleList srl in Functions.syllRuleList)
			{
				if ((this.time >= srl.StartDate) && (this.time <= srl.EndDate))
				{
					syllrulList = new SyllableRuleList(srl);
				}
			}

			if (syllrulList == null)
			{
				Functions.ShowErrorMessage("Keine Silbentrennregel für das Jahr" + this.time + " gefunden");
			}

			syllNum = 1;
			syllables = new List<Syllable>();
			Syllable syll = new Syllable();
			List<List<int?>> mapping = new List<List<int?>>();

			/* Mapping löschen */
			foreach (SignMapping sm in this.mappingList)
			{
				sm.SyllPos.Clear();
			}

			/* für jedes Phone*/
			for (int i = 0; i < this.phonetic.Count; i++)
			{

				bool nextLoop = false;

				syll.Syll.Add(phonetic[i]);

				/* Zwischenspeichern für Mapping:
				 * Mapping-Index, PhonPos-Index, Silbe, Position in Silbe
				 */
				int mappingIndex = 99;
				int posInPhon = 99;

				foreach (SignMapping sm in this.mappingList)
				{
					foreach (int? iPhonPos in sm.PhonPos)
					{
						if (iPhonPos == null)
							continue;

						if ((int)iPhonPos == i)
						{
							mappingIndex = this.mappingList.IndexOf(sm);
							posInPhon = sm.PhonPos.IndexOf(iPhonPos);
							goto mappingIndexFound;
						}
					}
				}

			mappingIndexFound:
				mapping.Add(new List<int?> { mappingIndex, posInPhon });

				/* für jede Trennregel*/
				foreach (SyllableRule syllrul in syllrulList.SyllrulList)
				{

					List<List<int?>> mapping_c = new List<List<int?>>(Functions.DeepClone(mapping));

					/* Wort ist kürzer als Muster*/
					if (i + syllrul.Pattern.Length > this.phonetic.Count)
					{
						continue;
					}

					List<Sign> partOfWord = new List<Sign>();
					Syllable syllEnd = new Syllable();

					/* für jeden Regelteil*/
					for (int j = 0; j < syllrul.Pattern.Length; j++)
					{

						if (i >= this.phonetic.Count)
						{
							break;
						}

						/* prüfe, ob Wortteil entspricht*/
						partOfWord.Add(this.phonetic[i + j]);
						if ((j != 0) && (j <= (syllrul.PosOfBorder - 1)))
						{
							syllEnd.Syll.Add(phonetic[i + j]);
						}

						if (syllrul.FindSoundsInPattern(this.phonetic[i + j].Symbol, j) == false)
						{
							break;
						}

						/* Mapping für Zeichen zwischen erstem und letztem Regelteil */
						if ((j != 0) && (j <= (syllrul.PosOfBorder - 1)))
						{
							foreach (SignMapping sm in this.mappingList)
							{
								foreach (int? iPhonPos in sm.PhonPos)
								{
									if (iPhonPos == null)
										continue;

									if ((int)iPhonPos == i + j)
									{
										mappingIndex = this.mappingList.IndexOf(sm);
										posInPhon = sm.PhonPos.IndexOf(iPhonPos);
										mapping_c.Add(new List<int?> { mappingIndex, posInPhon });
										goto mappingIndexFound_c;
									}
								}
							}
						}

					mappingIndexFound_c:


						/* letzter Regelteil: Bedingungen prüfen*/
						if (j == syllrul.Pattern.Length - 1)
						{
							if (syllrul.CheckConditions(partOfWord) == true)
							{

								/* Silbengrenzen setzen*/
								this.phonetic[i + syllrul.PosOfBorder - 1].SyllEnd = true;
								this.word[i + syllrul.PosOfBorder - 1].SyllEnd = true;

								Syllable newSyll = syll.Concat(syllEnd);
								syllables.Add(newSyll);

								/* Mappen */
								int posInSyll = 0;

								mapping = new List<List<int?>>(Functions.DeepClone(mapping_c));

								foreach (List<int?> map in mapping)
								{

									for (int k = 0; k < (int)map[1]; k++)
									{
										if (mappingList[(int)map[0]].SyllPos.ElementAtOrDefault(k) == null)
											mappingList[(int)map[0]].SyllPos.Add(new List<int?> { null, null });
									}
									mappingList[(int)map[0]].SyllPos.Add(new List<int?> { syllNum - 1, posInSyll });

									posInSyll++;
								}

								syllNum++;
								syll = new Syllable();
								syllEnd = new Syllable();
								mapping = new List<List<int?>>();
								i += syllrul.PosOfBorder - 1;

								if (this.phonetic.Count < i + 1)
								{
									this.phonetic[i + syllrul.PosOfBorder].SyllStart = true;
								}
								if (this.word.Count < i + 1)
								{
									this.word[i + syllrul.PosOfBorder].SyllStart = true;
								}

								nextLoop = true;
								break;
							}
						}
					}
					if (nextLoop == true)
						break;
				}
			}
			//
			/* letzte Silbe hinzufügen und mappen */
			syllables.Add(syll);

			int posInSyll2 = 0;

			foreach (List<int?> map in mapping)
			{

				for (int k = 0; k < (int)map[1]; k++)
				{
					if ((int)map[0] >= mappingList.Count)
					{
						Functions.ShowErrorMessage("Mapping-Fehler in " + this.PrintWord("GraphOutputWord") + "!");
					}
					else
					{
						if (mappingList[(int)map[0]].SyllPos.ElementAtOrDefault(k) == null)
							mappingList[(int)map[0]].SyllPos.Add(new List<int?> { null, null });
					}
				}
				mappingList[(int)map[0]].SyllPos.Add(new List<int?> { syllNum - 1, posInSyll2 });

				posInSyll2++;
			}
		}

		public void FindStress()
		{

			bool accentFound = false;

			foreach (AccentRule ar in Functions.accListLang1)
			{

				if (syllNum == ar.Pattern.Count() - 2)
				{
					if (ar.Conditions == null)
					{
						/* alle Silben durchlaufen */
						for (int i = 1; i <= syllables.Count; i++)
						{

							if (i == ar.PrimStress)
							{
								syllables[i - 1].PrimStress = true;
								accentFound = true;
							}
							else if (i == ar.SecStress)
							{
								syllables[i - 1].SecStress = true;
							}

							if ((i == syllables.Count) && (accentFound == true))
							{
								goto accentFound;
							}

						}

					}
					else
					{
						/* alle Silben durchlaufen */
						for (int i = 1; i <= syllables.Count; i++)
						{

							string syll = "";

							foreach (Sign l in syllables[i - 1].Syll)
							{
								syll += l.Symbol;
							}

							foreach (Condition c in ar.Conditions)
							{
								if (ar.Pattern[i] == c.Attribute)
								{

									Match m = Regex.Match(syll, c.Value);
									if (!m.Success)
									{
										/* lösche alle vorher gesetzten Betonungen */
										foreach (Syllable s in syllables)
										{
											s.PrimStress = false;
											s.SecStress = false;
										}

										goto MatchFailed;
									}
								}
							}
							if (i == ar.PrimStress)
							{
								syllables[i - 1].PrimStress = true;
								accentFound = true;
							}
							else if (i == ar.SecStress)
							{
								syllables[i - 1].SecStress = true;
							}

							if ((i == syllables.Count) && (accentFound == true))
							{
								goto accentFound;
							}

						}
					}
				}

			MatchFailed:
				continue;
			}

		accentFound:
			return;
		}

		public Wordform CallCheckRuleAndHandleDependencies(SoundChangeRule scr, Wordform wf, MainWindow win_, int? stPosPhon, int? stPosGraph,
															bool removeAppliedRules, bool otherChanges, List<SoundChangeRule> sortedRuleList)
		{

			Wordform wf2 = scr.CheckRule(wf, win_, stPosPhon, stPosGraph, otherChanges);

			if (wf2 != null)
			{
				if (otherChanges == false)
				{
					if (wf2.printRule == null)
					{
						PrintSoundChange(scr, wf2, win_);
					}
					else
					{
						PrintSoundChange(wf2.printRule, wf2, win_);
						wf2.printRule = null;
					}
				}

				if (wf2.repeatRules.Count != 0 && wf2.repeatRules.Last() == scr.Id)
				{

					wf2.repeatRules.RemoveAt(wf2.repeatRules.Count - 1);

					Wordform wf3 = CallCheckRuleAndHandleDependencies(scr, wf2, win_, stPosPhon, stPosGraph, removeAppliedRules, otherChanges,
									   sortedRuleList);
					if (wf3 != null)
					{
						return wf3;
					}

				}
				sortedRuleList.Remove(scr);
				sortedRuleList.RemoveAll(x => x.EndDate < wf2.time);
				Wordform wf4 = wf2.ChangeSound(win_, 0, 0, true, otherChanges, sortedRuleList);

				if ((otherChanges == true) && (wf4 != null))
				{
					return wf4;
				}
				else
				{
					return wf2;
				}
			}
			return null;

		}

		public Wordform ChangeSound(MainWindow win_, int? stPosPhon, int? stPosGraph, bool removeAppliedRules, bool otherChanges,
									 List<SoundChangeRule> sortedRuleList)
		{

			/* prüft ab dem Datum der aktuellen Wortform alle Regeln
			 * und ruft SoundChangeRule.CheckRule auf
			 * falls eine Regel erfolgreich war: 
			 * 		legt sie eine neue Wortform an;
			 * 		prüft sie, ob die Wortform ein gültiges nfrz. Wort ist;
			 * 		und ruft sich selbst mit der neuen Wortform auf
			 * falls nicht wird die Lautentwicklung abgebrochen
			 * und das Wort wird der frz. Wortformerkennung übergeben
			 */

			Functions.debugText.Append("sortedRuleList: ");
			foreach (SoundChangeRule scr in sortedRuleList)
			{
				Functions.debugText.Append(scr.Id + ", ");
			}
			Functions.debugText.Append(Environment.NewLine);


			foreach (SoundChangeRule scr in sortedRuleList)
			{

				Wordform wf2 = CallCheckRuleAndHandleDependencies(scr, this, win_, stPosPhon, stPosGraph, removeAppliedRules, otherChanges,
								   sortedRuleList);
				if (wf2 != null)
					return wf2;
			}
			return null;
		}

		/// <summary>
		/// (vorher: Returns graphPos, phonPos, Mapping-Index, Mapping-GraphPos, Mapping-PhonPos, lastPhonPos, lastGraphPos)
		/// Returns phonPos, lastPhonPos, graphPos, lastGraphPos, Mapping-Index, Mapping-GraphPos, Mapping-PhonPos
		public List<int?> CompareMapping(Dictionary<int?, int?> phonPos, Dictionary<int?, int?> graphPos)
		{

			if (graphPos != null && phonPos != null && (graphPos.Count == 0 || phonPos.Count == 0))
			{
				/* in Word oder Phonetic gefunden */
				return null;
			}
			else if ((graphPos != null) && (graphPos.Count != 0) && (phonPos != null) && (phonPos.Count != 0))
			{

				foreach (KeyValuePair<int?, int?> gP in graphPos)
				{
					foreach (KeyValuePair<int?, int?> pP in phonPos)
					{
						/* prüfe, ob Mapping übereinstimmt */

						for (int i = 0; i < this.mappingList.Count; i++)
						{
							if (this.mappingList[i].GraphPos.Contains((int)gP.Key) && this.mappingList[i].PhonPos.Contains((int)pP.Key))
							{

								/* falls Übereinstimmung: gib graphPos, phonPos und mapping zurück */
								return new List<int?> {
									(int)pP.Key,
									(int)pP.Value,
									(int)gP.Key,
									(int)gP.Value,
									i,
									this.mappingList [i].GraphPos.IndexOf ((int)gP.Key),
									this.GetIndexFromGraph ((int)gP.Value).Item2,
									this.mappingList [i].PhonPos.IndexOf ((int)pP.Key),
									this.GetIndexFromPhon ((int)pP.Value).Item2
								};
							}
						}
					}
				}
			}
			else
			{

				if ((graphPos != null) && (graphPos.Count != 0))
				{
					foreach (KeyValuePair<int?, int?> gP in graphPos)
					{
						for (int i = 0; i < this.mappingList.Count; i++)
						{
							if (this.mappingList[i].GraphPos.Contains((int)gP.Key))
							{
								return new List<int?> {
									null,
									null,
									(int)gP.Key,
									(int)gP.Value,
									i,
									this.mappingList [i].GraphPos.IndexOf ((int)gP.Key),
									this.mappingList [i].GraphPos.IndexOf ((int)gP.Value),
									null,
									null
								};
							}
						}
					}

				}
				else if ((phonPos != null) && (phonPos.Count != 0))
				{
					foreach (KeyValuePair<int?, int?> pP in phonPos)
					{
						for (int i = 0; i < this.mappingList.Count; i++)
						{
							if (this.mappingList[i].PhonPos.Contains((int)pP.Key))
							{
								return new List<int?> {
									(int)pP.Key,
									(int)pP.Value,
									null,
									null,
									i,
									null,
									null,
									this.mappingList [i].PhonPos.IndexOf ((int)pP.Key),
									this.mappingList [i].PhonPos.IndexOf ((int)pP.Value)
								};
							}
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Gibt alle gefundenen Grapheme in Word als Dictionary (Beginn- und Endposition) zurück.
		/// </summary>
		/// <param name="graph">Graph.</param>
		/// <param name="startPosGraph">Start position graph.</param>
		public Dictionary<int?, int?> SearchGraphInWord(List<Sign> graph, int startPosGraph)
		{

			Dictionary<int?, int?> graphPos = new Dictionary<int?, int?>();

			/* alle Zeichen des graphischen Wortes durchlaufen */
			for (int i = startPosGraph; i < this.word.Count; i++)
			{
				for (int k = 0; k < graph.Count; k++)
				{

					/* falls Wort nicht so lang ist */
					if (i + k >= word.Count)
					{
						break;
					}

					if (this.word[i + k].IsEqual(graph[k]) == true)
					{

						if (k == graph.Count - 1)
						{
							graphPos.Add(i, i + k);
						}
						continue;
					}
					else
					{
						/* nicht gefunden */
						if (i == this.word.Count - 1)
						{
							return graphPos;
						}
						else
						{
							break;
						}
					}
				}
			}
			return graphPos;
		}

		/// <summary>
		/// Gibt alle gefundenen Phone in Phonetic als Dictionary (Beginn- und Endposition) zurück.
		/// </summary>
		public Dictionary<int?, int?> SearchPhonInWord(List<Sign> phon, int startPosPhon)
		{

			Dictionary<int?, int?> phonPos = new Dictionary<int?, int?>();

			/* alle Zeichen des phonologischen Wortes durchlaufen */
			for (int i = startPosPhon; i < this.phonetic.Count; i++)
			{
				for (int k = 0; k < phon.Count; k++)
				{
					/* wenn Zeichen im Wort und erstes Graphem gleich sind, 
					* dann alle folgenden Zeichen vergleichen */

					/* falls Wort nicht so lang ist */
					if (i + k >= this.phonetic.Count)
					{
						break;
					}

					if (this.phonetic[i + k].IsEqual(phon[k]) == true)
					{

						/* beim letzten Zeichen des Phons	*/
						if (k == phon.Count - 1)
						{

							phonPos.Add(i, i + k);
						}
						continue;
					}
					else
					{
						/* nicht gefunden */
						if (i == this.phonetic.Count - 1)
						{
							return phonPos;
						}
						else
						{
							break;
						}
					}
				}
			}

			return phonPos;
		}

		/// <summary>
		/// Gibt Lautwandel aus.
		/// </summary>
		/// <returns>The sound change.</returns>
		public void PrintSoundChange(SoundChangeRule scr, Wordform wf2, MainWindow win_)
		{
			Functions.reflex = new Wordform(wf2.CloneWfObjectExtensions());
			win_.AddToTextView(scr, wf2.lastChanges, wf2.MultiPathNum);
			win_.AddToTextView(wf2, true, "", "Honeydew", "", "lang1");
			wf2.multiPathNum = 0;
		}

		/// <summary>
		/// Gibt Attribute aus Wordform als String zurückgeben.
		/// Optionen:
		/// - "GraphOutputWord": Word mit diakritischen Zeichen
		/// - "GraphOutputLemma": Lemma mit diakritischen Zeichen
		/// - "GraphText": Word wie in Textdatei
		/// - "GraphOutputSegm": Word mit diakritischen Zeichen und Segmentierung
		/// - "PhonSampa": Phonetic in Sampa
		/// - "PhonIpa": Phonetic in Ipa
		/// - "Syll": Phonetic in Ipa
		/// </summary>
		public string PrintWord(string modus)
		{

			string output = "";

			List<Sign> outputSignsP = new List<Sign>();
			List<Syllable> outputSylls = new List<Syllable>();

			List<Sign> outputSignsW = new List<Sign>(Functions.DeepClone(this.word));
			if (outputSignsW[0].Symbol == "#")
				outputSignsW.RemoveAt(0);
			if (outputSignsW[outputSignsW.Count - 1].Symbol == "#")
				outputSignsW.RemoveAt(outputSignsW.Count - 1);

			if (this.phonetic != null)
			{
				outputSignsP = new List<Sign>(Functions.DeepClone(this.phonetic));
				if (outputSignsP[0].Symbol == "#")
					outputSignsP.RemoveAt(0);
				if (outputSignsP[outputSignsP.Count - 1].Symbol == "#")
					outputSignsP.RemoveAt(outputSignsP.Count - 1);
			}

			if (this.syllables != null)
			{
				outputSylls = new List<Syllable>(Functions.DeepClone(this.syllables));
				if (outputSylls[0].Syll[0].Symbol == "#")
					outputSylls[0].Syll.RemoveAt(0);
				if (outputSylls[outputSylls.Count - 1].Syll[outputSylls[outputSylls.Count - 1].Syll.Count - 1].Symbol == "#")
					outputSylls[outputSylls.Count - 1].Syll.RemoveAt(outputSylls[outputSylls.Count - 1].Syll.Count - 1);
			}

			switch (modus)
			{

				/* Wortform */
				case "GraphOutputWord":
					output += Functions.ConvertSignListToGraph(outputSignsW);
					break;

				/* Lemma */
				case "GraphOutputLemma":
					output += Functions.ConvertSignListToGraph(this.lemma);
					break;

				/* Textdatei */
				case "GraphText":
					output += Functions.ConvertSignListToGraphText(outputSignsW);
					break;

				/* Segm. */
				case "GraphOutputSegm":
					foreach (Sign l in outputSignsW)
					{
						if (l.SegmEnd == true)
						{
							output += l.PrintGraph() + " + ";
						}
						else
						{
							output += l.PrintGraph();
						}
					}
					break;

				/* Phon. (Sampa) */
				case "PhonSampa":
					foreach (Sign l in outputSignsP)
					{
						output += l.Symbol;
					}
					break;

				/* Phon. (IPA) */
				case "PhonIpa":
					output += Functions.ConvertSignListToIPA(outputSignsP);
					break;

				/* Silben */
				case "Syll":

					foreach (Sign l in outputSignsP)
					{

						output += l.PrintIpa();

						if (l.SyllEnd == true)
						{
							output += ".";
						}
					}
					break;
				case "Acc":
					foreach (Syllable s in outputSylls)
					{

						if (s.PrimStress == true)
						{
							output += "\u02C8";
						}
						if (s.SecStress == true)
						{
							output += "\u02CC";
						}

						foreach (Sign l in s.Syll)
						{
							output += l.PrintIpa();

							if (l.SyllEnd == true)
							{
								output += ".";
							}
						}
					}
					break;
			}
			return output;

		}

		/// <summary>
		/// Gibt die Wordform als List<List<String>> zurück.
		/// </summary>
		public List<List<string>> Print()
		{
			List<List<string>> prt = new List<List<string>>();
			List<string> line = new List<String> {
				"",
				"",
				"Lemma: ",
				this.PrintWord ("GraphOutputLemma")
			};
			prt.Add(line);
			line = new List<String> { "", "", "Textdatei: ", this.PrintWord("GraphText") };
			prt.Add(line);
			line = new List<String> { "", "", "Wortart: ", this.pos };
			prt.Add(line);
			line = new List<String> { "", "", "Gram. Kat.: ", this.cat };
			prt.Add(line);
			line = new List<String> { "", "", "Zeit: ", this.time.ToString() };
			prt.Add(line);

			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				line = new List<String> { "", "", kvp.Key + ": ", kvp.Value };
				prt.Add(line);
			}

			line = new List<String> { "", "", "Tabelle: ", this.tbl };
			prt.Add(line);
			line = new List<String> { "", "", "Segm.: ", PrintWord("GraphOutputSegm") };
			prt.Add(line);

			if (this.phonetic != null)
			{
				line = new List<String> { "", "", "Phon. (SAMPA): ", "[" + PrintWord("PhonSampa") + "]" };
				prt.Add(line);
				line = new List<String> { "", "", "Silben: ", "[" + this.PrintWord("Syll") + "]" };
				prt.Add(line);
				line = new List<String> { "", "", "Betonung: ", "[" + this.PrintWord("Acc") + "]" };
				prt.Add(line);
			}

			return prt;
		}

		public List<string> PrintShort(String comment, String arrow)
		{
			List<string> line = new List<String> {
				arrow,
				this.PrintWord ("GraphOutputWord") + "  [" + PrintWord ("PhonIpa") + "]",
				"",
				comment
			};

			return line;
		}

		/// <summary>
		/// Prints the changed Wordform.
		/// </summary>
		public List<List<string>> PrintChanged()
		{
			List<List<string>> prt = new List<List<string>>();
			List<string> line = new List<String> { "", "", "Wortart: ", this.pos };
			prt.Add(line);
			line = new List<String> { "", "", "Gram. Kat.: ", this.cat };
			prt.Add(line);
			line = new List<String> { "", "", "Zeit: ", this.time.ToString() };
			prt.Add(line);
			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				line = new List<String> { "", "", kvp.Key + ": ", kvp.Value };
				prt.Add(line);
			}

			line = new List<String> { "", "", "Tabelle: ", this.tbl };
			prt.Add(line);

			if (this.phonetic != null)
			{
				line = new List<String> { "", "", "Phon. (SAMPA): ", "[" + PrintWord("PhonSampa") + "]" };
				prt.Add(line);
				line = new List<String> { "", "", "Silben: ", "[" + this.PrintWord("Syll") + "]" };
				prt.Add(line);
				line = new List<String> { "", "", "Betonung: ", "[" + this.PrintWord("Acc") + "]" };
				prt.Add(line);
			}

			return prt;
		}

		/// <summary>
		/// Prints the Wordform of Lang2
		/// </summary>
		public List<List<string>> PrintLang2()
		{

			List<List<string>> prt = new List<List<string>>();
			List<string> line = new List<String> {
				"",
				"",
				"Lemma: ",
				this.PrintWord ("GraphOutputLemma")
			};
			prt.Add(line);
			line = new List<String> { "", "", "Textdatei: ", this.PrintWord("GraphText") };
			prt.Add(line);
			line = new List<String> { "", "", "Wortart: ", this.pos };
			prt.Add(line);
			line = new List<String> { "", "", "Gram. Kat.: ", this.cat };
			prt.Add(line);
			line = new List<String> { "", "", "Zeit: ", this.time.ToString() };
			prt.Add(line);

			line = new List<String> { "", "", "Tabelle: ", this.tbl };
			prt.Add(line);

			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				line = new List<String> { "", "", kvp.Key + ": ", kvp.Value };
				prt.Add(line);
			}
			line = new List<String> { "", "", "Segm.: ", PrintWord("GraphOutputSegm") };
			prt.Add(line);

			if (this.phonetic != null)
			{
				line = new List<String> { "", "", "Phon. (SAMPA): ", "[" + PrintWord("PhonSampa") + "]" };
				prt.Add(line);
			}

			return prt;
		}

		public Tuple<int, int> GetIndexFromGraph(int graphPos)
		{

			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].GraphPos.FindIndex(c => c == graphPos);

				if (index != -1)
					return Tuple.Create(i, index);
			}
			return null;
		}

		public Tuple<int, int> GetIndexFromPhon(int phonPos)
		{

			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].PhonPos.FindIndex(c => c == phonPos);

				if (index != -1)
					return Tuple.Create(i, index);
			}
			return null;
		}

		/// <summary>
		/// Sucht anhand des graphischen Zeichens die über die MappingList verknüpfte Position in Phonetic
		/// </summary>
		/// <returns>The phon position from graph.</returns>
		/// <param name="graphPos">Graph position.</param>
		public int? GetPhonPosFromGraph(int graphPos)
		{

			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].GraphPos.FindIndex(c => c == graphPos);

				if (index != -1)
				{
					if (index == 0)
					{
						return this.mappingList[i].PhonPos[index];
					}
					else
					{
						if (this.mappingList[i].PhonPos[index] != null)
						{
							return this.mappingList[i].PhonPos[index];
						}
						else
						{
							return this.mappingList[i].PhonPos[0];
						}
					}
				}
			}
			throw new SymbolNotFoundException("Position " + graphPos + " in " + this.PrintWord("GraphOutputWord") + " nicht gefunden.");
		}

		public int? GetGraphPosFromPhon(int phonPos)
		{

			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].PhonPos.FindIndex(c => c == phonPos);

				if (index != -1)
				{
					if (index == 0)
					{
						return this.mappingList[i].GraphPos[index];
					}
					else
					{
						if ((this.mappingList[i].GraphPos.Count > 1) && (this.mappingList[i].GraphPos[index] != null))
						{
							return this.mappingList[i].GraphPos[index];
						}
						else
						{
							return this.mappingList[i].GraphPos[0];
						}
					}
				}
			}
			throw new SymbolNotFoundException("Position " + phonPos + " in " + this.PrintWord("PhonSampa") + " nicht gefunden.");
		}

		public void SetMappingForPhon(int posBefore, int posAfter)
		{
			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].PhonPos.FindIndex(c => c == posBefore);

				if (index != -1)
				{
					this.mappingList[i].PhonPos[index] = posAfter;
					return;
				}
			}
			throw new SymbolNotFoundException("Position " + posBefore + " in " + this.PrintWord("PhonSampa") + " nicht gefunden.");
		}

		public void SetMappingForGraph(int posBefore, int posAfter)
		{
			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].GraphPos.FindIndex(c => c == posBefore);

				if (index != -1)
				{
					this.mappingList[i].GraphPos[index] = posAfter;
					return;
				}
			}
			throw new SymbolNotFoundException("Position " + posBefore + " in " + this.PrintWord("GraphOutputWord") + " nicht gefunden.");
		}

		public void DeletePhonFromMapping(int pos)
		{

			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].PhonPos.FindIndex(c => c == pos);

				if (index != -1)
				{
					if (index == 0)
					{
						if (this.mappingList[i].PhonPos.Count == 1)
						{
							this.mappingList[i].PhonPos[index] = null;
						}
						else
						{
							this.MappingList[i].PhonPos.RemoveAt(index);
						}
					}
					else
					{
						this.MappingList[i].PhonPos.RemoveAt(index);
					}
					return;
				}
			}
			throw new SymbolNotFoundException("Position " + pos + " in " + this.PrintWord("PhonSampa") + " nicht gefunden.");

		}

		public void DeleteGraphFromMapping(int pos)
		{

			for (int i = 0; i < this.mappingList.Count; i++)
			{

				int index = this.mappingList[i].GraphPos.FindIndex(c => c == pos);

				if (index != -1)
				{
					if (index == 0)
						this.mappingList[i].GraphPos[index] = null;
					else
						this.MappingList[i].GraphPos.RemoveAt(index);
					return;
				}
			}
			throw new SymbolNotFoundException("Position " + pos + " in " + this.PrintWord("GraphOutputWord") + " nicht gefunden.");

		}
	}
}

