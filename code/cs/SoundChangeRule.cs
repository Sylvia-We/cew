using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace frz
{
	[Serializable]
	public class Change
	{
		private List<List<Sign>> phonBefore = new List<List<Sign>>();
		private List<Sign> phonAfter = new List<Sign>();
		private List<List<Sign>> graphBefore = new List<List<Sign>>();
		private List<Sign> graphAfter = new List<Sign>();
		private Dictionary<String, String> otherChanges = new Dictionary<String, String>();

		public Change(List<List<Sign>> phonBefore, List<Sign> phonAfter, List<List<Sign>> graphBefore, List<Sign> graphAfter,
					   Dictionary<String, String> otherChanges)
		{
			this.phonBefore = phonBefore;
			this.phonAfter = phonAfter;
			this.graphBefore = graphBefore;
			this.graphAfter = graphAfter;
			this.otherChanges = otherChanges;
		}

		public List<List<Sign>> PhonBefore
		{
			get { return this.phonBefore; }
			set { this.phonBefore = value; }
		}

		public List<Sign> PhonAfter
		{
			get { return this.phonAfter; }
			set { this.phonAfter = value; }
		}

		public List<List<Sign>> GraphBefore
		{
			get { return this.graphBefore; }
			set { this.graphBefore = value; }
		}

		public List<Sign> GraphAfter
		{
			get { return this.graphAfter; }
			set { this.graphAfter = value; }
		}

		public Dictionary<String, String> OtherChanges
		{
			get { return this.otherChanges; }
			set { this.otherChanges = value; }
		}
	}

	[Serializable]
	public class SoundChangeRule
	{
		private String id;
		private String name;
		private List<Change> changes = new List<Change>();
		private List<StressPattern> stressPatterns = new List<StressPattern>();
		private String syllType;
		private int startDate;
		private int endDate;
		private List<String> orPreCond;
		private List<String> andPreCond;
		private List<String> earlier;
		private List<String> specialCases;
		private String leftStr;
		private String rightStr;
		private List<List<Sign>> leftPhon;
		private List<List<Sign>> rightPhon;
		private List<List<Sign>> leftGraph;
		private List<List<Sign>> rightGraph;
		private Dictionary<String, String> additional = new Dictionary<string, string>();
		private Dictionary<String, String> addConditions = new Dictionary<string, string>();
		private String tbl;
		private String cat;
		private bool earlierIsTrue = false;
		private bool specialCaseIsTrue = false;

		public SoundChangeRule(String id, String name, List<Change> changes, String stress, String syllType, String time, String left, String right,
								String andPreCond, String orPreCond, String earlier, String specialCase, Dictionary<String, String> additional,
								Dictionary<String, String> addConditions, String tbl, String cat)
		{
			if (id == "")
			{
				Functions.ShowErrorMessage("Regel ohne Id! Syntax überprüfen! (Name: " + name + ")");
			}
			else
			{
				this.id = id;
			}
			this.name = name;
			this.changes = changes;

			if (this.changes.Count == 0)
				Functions.ShowWarningMessage("Regel " + this.id + " enthält keinen Wandel oder Syntax ist fehlerhaft.");

			/* stress: "A _ B# */
			if (stress != "")
			{
				String[] stressPatterns = stress.Split(' ');

				for (int i = 0; i < stressPatterns.Length; i++)
				{
					stressPatterns[i].Trim();
					bool primStress = false;
					bool secStress = false;
					bool startOfWord = false;
					bool endOfWord = false;

					if (stressPatterns[i].Contains("\u0022"))
					{
						primStress = true;
						stressPatterns[i] = Regex.Replace(stressPatterns[i], "\"", "");
					}

					if (stressPatterns[i].Contains("%"))
					{
						secStress = true;
						stressPatterns[i] = Regex.Replace(stressPatterns[i], "%", "");
					}

					if ((i == 0) && stressPatterns[i].StartsWith("#"))
					{
						startOfWord = true;
						stressPatterns[i] = Regex.Replace(stressPatterns[i], "^#", "");
					}

					if ((i == stressPatterns.Length - 1) && stressPatterns[i].EndsWith("#"))
					{
						endOfWord = true;
						stressPatterns[i] = Regex.Replace(stressPatterns[i], "#$", "");
					}

					this.stressPatterns.Add(new StressPattern(stressPatterns[i], primStress, secStress, startOfWord, endOfWord));
				}
			}
			else
			{
				this.stressPatterns = null;
			}

			this.syllType = syllType;

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
					Functions.ShowErrorMessage("Regel " + this.id + " hat kein Anfangsdatum. (Notation: 1--2)");
				}

				if (timeRegex.Groups[2].Value.Trim() != "")
				{
					endDate = Int32.Parse(timeRegex.Groups[2].Value.Trim());
				}
				else
				{
					Functions.ShowErrorMessage("Regel " + this.id + " hat kein Enddatum. (Notation: 1--2)");
				}
			}
			else
			{
				Functions.ShowErrorMessage("Regel " + this.id + " hat kein korrektes Datum. (Notation: 1--2)");
			}

			/* left: #
			 * right: # */
			this.leftStr = left;
			this.rightStr = right;
			List<String> leftRight = new List<String>();
			leftRight.Add(left);
			leftRight.Add(right);

			/* für links und rechts */
			for (int i = 0; i < leftRight.Count; i++)
			{

				if (leftRight[i] == "")
				{
					if (i == 0)
					{
						this.leftPhon = new List<List<Sign>> { new List<Sign> { new Sign("") } };
						this.leftGraph = new List<List<Sign>> { new List<Sign> { new Sign("") } };
					}
					else
					{
						this.rightPhon = new List<List<Sign>> { new List<Sign> { new Sign("") } };
						this.rightGraph = new List<List<Sign>> { new List<Sign> { new Sign("") } };
					}
				}
				else
				{
					String[] PhoGra = leftRight[i].Split(';');

					/* für Grapheme und Phone */
					foreach (String str in PhoGra)
					{
						Match phonRegex = Regex.Match(str, @"^\s*(\[.*?\])\s*$");
						Match graphRegex = Regex.Match(str, @"^\s*(<.*?>)\s*$");

						String s = str.TrimStart("[".ToCharArray()).TrimEnd("]".ToCharArray()).
							TrimStart("<".ToCharArray()).TrimEnd(">".ToCharArray());

						List<List<Sign>> contextList = new List<List<Sign>>();

						if (s == "#")
						{
							contextList = new List<List<Sign>> { new List<Sign> { new Sign("#") } };
						}
						else
						{
							String[] Arr = s.Split(',');
							foreach (String p in Arr)
							{
								if (phonRegex.Success)
								{
									contextList.AddRange(Functions.ConvertStringToSignListList(p.Trim(), "phon", "context"));
								}
								else if (graphRegex.Success)
								{
									contextList.AddRange(Functions.ConvertStringToSignListList(p.Trim(), "graph", "context"));
								}
							}
						}

						if (i == 0)
						{
							if (phonRegex.Success)
								this.leftPhon = contextList;
							else if (graphRegex.Success)
								this.leftGraph = contextList;
							else
								throw new SymbolNotFoundException("Linker Kontext konnte nicht korrekt ausgewertet werden: " + left);
						}
						else if (i == 1)
						{
							if (phonRegex.Success)
								this.rightPhon = contextList;
							else if (graphRegex.Success)
								this.rightGraph = contextList;
							else
								throw new SymbolNotFoundException("Rechter Kontext konnte nicht korrekt ausgewertet werden: " + right);
						}
					}
				}
			}

			if (this.leftPhon == null)
				this.leftPhon = new List<List<Sign>> { new List<Sign> { new Sign("") } };
			if (this.leftGraph == null)
				this.leftGraph = new List<List<Sign>> { new List<Sign> { new Sign("") } };
			if (this.rightPhon == null)
				this.rightPhon = new List<List<Sign>> { new List<Sign> { new Sign("") } };
			if (this.rightGraph == null)
				this.rightGraph = new List<List<Sign>> { new List<Sign> { new Sign("") } };

			/* preCond: V_1, V_2 */
			if (andPreCond != "")
			{
				String[] andPreConds = andPreCond.Split(',');
				foreach (String pC in andPreConds)
				{
					String prC = pC.Trim();
					if (this.andPreCond == null)
					{
						this.andPreCond = new List<String>();
					}
					this.andPreCond.Add(prC);
				}
			}

			if (orPreCond != "")
			{
				String[] orPreConds = orPreCond.Split(',');
				foreach (String pC in orPreConds)
				{
					String prC = pC.Trim();
					if (this.orPreCond == null)
					{
						this.orPreCond = new List<String>();
					}
					this.orPreCond.Add(prC);
				}
			}

			/* specialCase: V_1, V_2 */
			if (specialCase != "")
			{
				String[] specialCases = specialCase.Split(',');
				foreach (String sC in specialCases)
				{
					if (this.specialCases == null)
					{
						this.specialCases = new List<String>();
					}
					this.specialCases.Add(sC.Trim());
				}
			}
			else
			{
				this.specialCases = new List<String>();
			}

			/* earlier: V_1, V_2 */
			if (earlier != "")
			{
				String[] earlierArr = earlier.Split(',');
				foreach (String ear in earlierArr)
				{
					if (this.earlier == null)
					{
						this.earlier = new List<String>();
					}
					this.earlier.Add(ear.Trim());
				}
			}
			else
			{
				this.earlier = new List<String>();
			}

			this.additional = additional;
			this.addConditions = addConditions;
			this.tbl = tbl;
			this.cat = cat;
		}

		public int StartDate
		{
			get { return this.startDate; }
		}

		public int EndDate
		{
			get { return this.endDate; }
		}

		public String Id
		{
			get { return this.id; }
		}

		/// <summary>
		/// Testen, ob SoundChangeRule auf Wortform angewendet werden kann
		/// </summary>
		/// <returns>Changed Wordform</returns>
		/// <param name="wf">Wordform</param>
		public Wordform CheckRule(Wordform wf, MainWindow win_, int? stPosPhon, int? stPosGraph, bool otherChanges)
		{

			Functions.printDebugText("\t " + id + "\t\t" + wf.PrintShort("", "").ElementAt(1) + Environment.NewLine, true);
			Wordform wf2 = wf.CloneWfObjectExtensions();

			if (otherChanges == false)
				wf2.LastChanges = new List<Change>();
			bool changed = false;

			foreach (Change change in this.changes)
			{
				/* alle Subregeln durchlaufen */

				if (change.PhonBefore[0][0].Symbol == "")
				{
					/* change.PhonBefore ist leer d.h. Phon wird nicht geändert oder hinzugefügt */
					if ((change.PhonAfter.Count == 0) || (change.PhonAfter[0].Symbol == ""))
					{
						/* ---------- PHON NICHT ÄNDERN ---------- */

						if ((change.GraphBefore[0][0].Symbol == "") && ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == "")))
						{
							/* ---------- MORPHOLOGISCHE ÄNDERUNG ---------- */
							wf2 = ExecuteOtherChanges(change, ref wf2, ref changed, win_, stPosPhon, stPosGraph, otherChanges);

						}
						else if (change.GraphBefore[0][0].Symbol == "")
						{
							/* ---------- Graph Hinzufügen ---------- */
							AddGraphOnly();

						}
						else if ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == ""))
						{
							/* ---------- Graph Entfernen ---------- */

							foreach (List<Sign> graphSignsBefore in change.GraphBefore)
							{

								int startPosGraph;
								if (stPosGraph == null)
									startPosGraph = 0;
								else
									startPosGraph = (int)stPosGraph;

								bool continueGraphSearch = true;

								while (continueGraphSearch == true)
								{
									if (startPosGraph >= wf2.Word.Count - 1)
										break;
									RemoveGraphOnly();
								}
							}
						}
						else
						{
							/* ---------- Graph Ersetzen ---------- */

							foreach (List<Sign> graphSignsBefore in change.GraphBefore)
							{

								int startPosGraph;
								if (stPosGraph == null)
									startPosGraph = 0;
								else
									startPosGraph = (int)stPosGraph;

								bool continueGraphSearch = true;

								while (continueGraphSearch == true)
								{
									if (startPosGraph >= wf2.Word.Count - 1)
										break;
									ReplaceGraphOnly(ref wf2, ref startPosGraph, ref changed, ref continueGraphSearch, change,
										graphSignsBefore, win_, stPosPhon, stPosGraph, otherChanges);
								}
							}
						}
					}
					else
					{
						/* ---------- PHON HINZUFÜGEN ---------- */

						if ((change.GraphBefore[0][0].Symbol == "") && ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == "")))
						{
							/* ---------- Graph nicht ändern ---------- */
							AddPhonOnly();

						}
						else if (change.GraphBefore[0][0].Symbol == "")
						{
							/* ---------- Graph Hinzufügen ---------- */
							AddPhonAddGraph(ref wf2, ref changed, change, win_, stPosPhon, stPosGraph, otherChanges);

						}
						else if ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == ""))
						{
							/* ---------- Graph Entfernen ---------- */
							foreach (List<Sign> graphSignsBefore in change.GraphBefore)
							{

								int startPosGraph;
								if (stPosGraph == null)
									startPosGraph = 0;
								else
									startPosGraph = (int)stPosGraph;

								bool continueGraphSearch = true;

								while (continueGraphSearch == true)
								{
									if (startPosGraph >= wf2.Word.Count - 1)
										break;
									AddPhonRemoveGraph();
								}
							}
						}
						else
						{
							/* ---------- Graph Ersetzen ---------- */
							foreach (List<Sign> graphSignsBefore in change.GraphBefore)
							{

								int startPosGraph;
								if (stPosGraph == null)
									startPosGraph = 0;
								else
									startPosGraph = (int)stPosGraph;

								bool continueGraphSearch = true;

								while (continueGraphSearch == true)
								{
									if (startPosGraph >= wf2.Word.Count - 1)
										break;
									AddPhonReplaceGraph();
								}
							}
						}
					}
				}
				else
				{
					/* für alle phonBefore und für alle graphBefore schauen, ob eine Kombination matched
				 * falls ja, dann position und matchende Zeichen speichern */
					foreach (List<Sign> phonSignsBefore in change.PhonBefore)
					{

						bool continuePhonSearch = true;
						int startPosPhon;
						if (stPosPhon == null)
							startPosPhon = 0;
						else
							startPosPhon = (int)stPosPhon;

						while (continuePhonSearch == true)
						{

							if (startPosPhon >= wf2.Phonetic.Count - 1)
								break;

							if ((change.PhonAfter.Count == 0) || (change.PhonAfter[0].Symbol == ""))
							{
								/* ---------- PHON ENTFERNEN ---------- */

								Dictionary<int?, int?> foundPhonPos = wf2.SearchPhonInWord(phonSignsBefore, startPosPhon);

								if ((foundPhonPos != null) && (foundPhonPos.Count != 0))
								{

									if ((change.GraphBefore[0][0].Symbol == "") && ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == "")))
									{
										/* ---------- Graph nicht ändern ---------- */
										RemovePhonOnly(ref wf2, wf, ref startPosPhon, ref changed, ref continuePhonSearch, change,
											phonSignsBefore, foundPhonPos, win_, stPosPhon, stPosGraph, otherChanges);

									}
									else if ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == ""))
									{
										/* ---------- Graph Entfernen ---------- */

										foreach (List<Sign> graphSignsBefore in change.GraphBefore)
										{

											bool continueGraphSearch = true;
											int startPosGraph;
											if (stPosGraph == null)
												startPosGraph = 0;
											else
												startPosGraph = (int)stPosGraph;

											while (continueGraphSearch == true)
											{

												if (startPosGraph >= wf2.Word.Count - 1)
													break;
												RemovePhonRemoveGraph(ref wf2, wf, ref startPosGraph, ref startPosPhon, ref changed,
													ref continuePhonSearch, ref continueGraphSearch, change,
													graphSignsBefore, phonSignsBefore, foundPhonPos, win_, stPosPhon, stPosGraph,
													otherChanges);
											}
											continuePhonSearch = continueGraphSearch;
										}
									}
									else if (change.GraphBefore[0][0].Symbol == "")
									{
										/* ---------- Graph Hinzufügen ---------- */
										RemovePhonAddGraph();

									}
									else
									{
										/* ---------- Graph Ersetzen ---------- */
										foreach (List<Sign> graphSignsBefore in change.GraphBefore)
										{

											bool continueGraphSearch = true;
											int startPosGraph;
											if (stPosGraph == null)
												startPosGraph = 0;
											else
												startPosGraph = (int)stPosGraph;

											while (continueGraphSearch == true)
											{

												if (startPosGraph >= wf2.Word.Count - 1)
													break;

												RemovePhonReplaceGraph();
											}
											continuePhonSearch = continueGraphSearch;
										}
									}
								}
								else
								{
									continuePhonSearch = false;
								}
							}
							else
							{
								/* ----------PHON ERSETZEN ---------- */
								Dictionary<int?, int?> foundPhonPos = wf2.SearchPhonInWord(phonSignsBefore, startPosPhon);

								if ((foundPhonPos != null) && (foundPhonPos.Count != 0))
								{

									if ((change.GraphBefore[0][0].Symbol == "") && ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == "")))
									{
										/* ---------- Graph nicht ändern ---------- */
										ReplacePhonOnly(ref wf2, ref startPosPhon, ref changed, ref continuePhonSearch, change,
											phonSignsBefore, foundPhonPos, win_, stPosPhon, stPosGraph, otherChanges);

									}
									else if (change.GraphBefore[0][0].Symbol == "")
									{
										/* ---------- Graph Hinzufügen ---------- */
										ReplacePhonAddGraph();

									}
									else if ((change.GraphAfter.Count == 0) || (change.GraphAfter[0].Symbol == ""))
									{
										/* ---------- Graph Entfernen ---------- */
										foreach (List<Sign> graphSignsBefore in change.GraphBefore)
										{

											bool continueGraphSearch = true;
											int startPosGraph;
											if (stPosGraph == null)
												startPosGraph = 0;
											else
												startPosGraph = (int)stPosGraph;

											while (continueGraphSearch == true)
											{

												if (startPosGraph >= wf2.Word.Count - 1)
													break;
												ReplacePhonRemoveGraph();
											}
											continuePhonSearch = continueGraphSearch;
										}

									}
									else
									{
										/* ---------- Graph Ersetzen ---------- */
										foreach (List<Sign> graphSignsBefore in change.GraphBefore)
										{

											bool continueGraphSearch = true;
											int startPosGraph;
											if (stPosGraph == null)
												startPosGraph = 0;
											else
												startPosGraph = (int)stPosGraph;

											while (continueGraphSearch == true)
											{

												if (startPosGraph >= wf2.Word.Count - 1)
													break;
												ReplacePhonReplaceGraph(ref wf2, ref startPosGraph, ref startPosPhon, ref changed,
													ref continuePhonSearch, ref continueGraphSearch, change,
													graphSignsBefore, phonSignsBefore, foundPhonPos, win_, stPosPhon,
													stPosGraph, otherChanges);
											}
											continuePhonSearch = continueGraphSearch;
										}
									}
								}
								else
								{
									continuePhonSearch = false;
								}
							}

							if (specialCaseIsTrue == true)
							{
								specialCaseIsTrue = false;
								return wf2;
							}
							if (earlierIsTrue == true)
							{
								earlierIsTrue = false;
								return wf2;
							}
						}
					}
				}
			}

			if (changed == true)
			{
				return wf2;
			}
			else
			{
				return null;
			}
		}

		public Wordform ExecuteOtherChanges(Change change, ref Wordform wf2, ref bool changed, MainWindow win_, int? stPosPhon, int? stPosGraph,
									  bool otherChanges)
		{
			Dictionary<String, bool> newCats = new Dictionary<String, bool>();

			if (CheckMorphConditions(this.id, true, true, true, true, true, ref wf2, win_, stPosPhon, stPosGraph, otherChanges, ref newCats) == true)
			{

				/* Additionals ändern */

				foreach (KeyValuePair<String, String> kvp in change.OtherChanges)
				{
					if (kvp.Key == "Gram. Kat.")
					{
						wf2.Cat = kvp.Value;
						changed = true;
					}
					else if (wf2.Additional.ContainsKey(kvp.Key))
					{
						wf2.Additional[kvp.Key] = kvp.Value;
						changed = true;
					}
					else
					{
						wf2.Additional.Add(kvp.Key, kvp.Value);
						changed = true;
					}
				}

				/* neue Deklinationstabelle */
				foreach (KeyValuePair<String, String> kvp in change.OtherChanges)
				{

					if (kvp.Key == "Tabelle")
					{
						String oldCat = wf2.Cat;

						/* Kategorie von wf_c in newCat suchen und ersetzen */
						if (Functions.newCatDict.ContainsKey(oldCat))
						{
							for (int i = 0; i < Functions.newCatDict[oldCat].Count; i++)
							{
								Wordform wf_c = wf2.CloneWfObjectExtensions();

								String newCat = Functions.newCatDict[oldCat][i];

								wf_c.Cat = newCat;
								wf_c.Tbl = kvp.Value;

								FlexTbl tblResult = Functions.flexListLang1.FirstOrDefault(s => s.Tbl == kvp.Value);

								if (tblResult != null)
								{
									FlexRule ruleResult = tblResult.Frlist.FirstOrDefault(s => s.CatAfter == newCat);

									if (ruleResult != null)
									{
										int suffStartGraph = wf_c.Word.Count - wf_c.Suffix.Count - 1;
										int suffStartPhon = (int)wf_c.GetPhonPosFromGraph(suffStartGraph);
										int wordCount = wf_c.Word.Count;
										int phoneticCount = wf_c.Phonetic.Count;

										/* altes Suffix löschen und aus MappingList entfernen */
										wf_c.Word.RemoveRange(suffStartGraph, wordCount - suffStartGraph);
										wf_c.Phonetic.RemoveRange(suffStartPhon, phoneticCount - suffStartPhon);

										for (int j = suffStartGraph; j < wordCount; j++)
										{
											wf_c.DeleteGraphFromMapping(j);
										}

										for (int j = suffStartPhon; j < phoneticCount; j++)
										{
											wf_c.DeletePhonFromMapping(j);
										}

										RemoveNullMappings(wf_c);

										/* neues Suffix einfügen, transkribieren und mappen */
										wf_c.Word.AddRange(Functions.DeepClone(ruleResult.Affix));
										wf_c.Word.Add(new Sign("#"));
										wf_c.Suffix = wf_c.Word.GetRange(suffStartGraph, ruleResult.Affix.Count);
										wf_c.Phonetic.AddRange(Functions.TransformGraphToPhon(wf_c, suffStartPhon, suffStartGraph, "lang1"));

										/* neues Suffix Lautentwicklung durchlaufen lassen und evt. MappingList korrigieren */
										int? saveTime = wf_c.Time;
										wf_c.Time = wf_c.RootTime;
										wf_c.LastChanges.Add(change);
										wf_c.PrintRule = this;
										otherChanges = true;

										/* nur angewandte Regeln und Regeln, die vor der Wortform liegen in sortedRuleList */

										List<SoundChangeRule> sortedRuleList = Functions.rulListLang1.OrderBy(o => o.StartDate).ToList();
										sortedRuleList.RemoveAll(x => x.EndDate >= saveTime);

										foreach (String ruleName in wf_c.AppliedRules)
										{
											SoundChangeRule result = sortedRuleList.SingleOrDefault(s => s.Id == ruleName);
											if (result == null)
											{
												sortedRuleList.Add(Functions.rulListLang1.SingleOrDefault(s => s.Id == ruleName));
											}
										}
										sortedRuleList.Remove(this);

										foreach (SoundChangeRule scr in sortedRuleList)
										{

											Wordform wf3 = wf_c.CallCheckRuleAndHandleDependencies(scr, wf_c, win_, stPosPhon, stPosGraph, true, otherChanges,
															   sortedRuleList);
											if (wf3 != null)
											{
												wf3.PrintRule = null;
												wf3.Time = saveTime;

												/* neue Zeit zuweisen */
												if (wf3.Time < this.startDate)
													wf3.Time = this.startDate;

												wf_c = wf3.CloneWfObjectExtensions();

												break;
											}
										}

										changed = true;
									}
								}

								if (Functions.newCatDict[oldCat].Count > 1)
								{
									wf_c.MultiPathNum = Functions.newCatDict[oldCat].Count - i;
								}

								if (i == Functions.newCatDict[oldCat].Count - 1)
								{
									wf2 = wf_c.CloneWfObjectExtensions();
								}
								else
								{
									wf_c.PrintRule = this;
									Functions.multiChangePaths.Add(wf_c);
								}
							}
						}
						else
						{
							Functions.ShowErrorMessage("Kategorie " + oldCat + " in .ncat nicht gefunden! (Regel " + this.Id + ")");
							Functions.printDebugText("\t\tKategorie nicht in .ncat", true);
						}
					}
				}
				if (changed == true)
				{
					wf2.AppliedRules.Add(this.id);
				}

				return wf2;
			}
			return null;
		}

		public void AddGraphOnly()
		{
			throw new NotImplementedException();
		}

		public void RemoveGraphOnly()
		{
			throw new NotImplementedException();
		}

		public void ReplaceGraphOnly(ref Wordform wf2, ref int startPosGraph, ref bool changed, ref bool continueGraphSearch,
									  Change change, List<Sign> graphSignsBefore, MainWindow win_, int? stPosPhon, int? stPosGraph, bool otherChanges)
		{
			List<int?> resultOrNull = null;
			List<int> result = new List<int>();
			Dictionary<String, bool> newCats = new Dictionary<String, bool>();

			Dictionary<int?, int?> foundGraphPos = wf2.SearchGraphInWord(graphSignsBefore, startPosGraph);
			if ((foundGraphPos != null) && (foundGraphPos.Count != 0))
			{
				resultOrNull = wf2.CompareMapping(null, foundGraphPos);

				if (resultOrNull != null)
				{
					result = resultOrNull.Where(x => x != null).Cast<int>().ToList();
					int graphPos = result[0];
					int lastGraphPos = result[1];
					int mappingIndex = result[2];
					int posInGraphPosMapping = result[3];
					int lastPosInGraphPosMapping = result[4];

					Functions.printDebugText("\t\t<" + wf2.Word[graphPos].Symbol + "> \t(Ids: + " + graphPos + ")" + Environment.NewLine, true);

					if (CheckConditions(this.id, true, true, true, true, true, true, true, true, true, true, true, ref wf2, null, null, graphPos, lastGraphPos,
							mappingIndex, 1, graphSignsBefore.Count, win_, stPosPhon, stPosGraph, otherChanges, ref newCats) == true)
					{

						Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
						SaveStressBefore(wf2);

						/* Wort wandeln */
						wf2.Word.RemoveRange(graphPos, graphSignsBefore.Count);
						wf2.Word.InsertRange(graphPos, ObjectExtensions.Copy(change.GraphAfter));

						/* Suffix wurde geändert */
						UpdateSuffix(ref wf2, graphPos, graphSignsBefore.Count, change.GraphAfter.Count);

						for (int i = 0; i < change.GraphAfter.Count; i++)
						{
							wf2.Word[graphPos + i].AppliedRules.Add(this.id);
						}

						if (graphSignsBefore.Count < change.GraphAfter.Count)
						{
							/* ---------- <A> > <BC> ---------- */
							RemapGraphWithMoreSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos, lastGraphPos,
								mappingIndex, posInGraphPosMapping);

						}
						else if (graphSignsBefore.Count > change.GraphAfter.Count)
						{
							/* ---------- [AB] > [C] ---------- */
							RemapGraphWithLessSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos);
							throw new NotImplementedException();
						}
						else
						{
							/* ---------- [A] > [B] ---------- */
							/* tue nichts */
						}

						/* neue Zeit zuweisen */
						if (wf2.Time < this.startDate)
							wf2.Time = this.startDate;

						/* neue Silbengrenzen finden*/
						wf2.SeparateSyllables();
						ResetStress(wf2, change);
						wf2.LastChanges.Add(change);
						changed = true;
						wf2.AppliedRules.Add(this.id);

						/* Kategorie splitten und mehrere Pfade verfolgen? */
						if (newCats.Count > 1)
						{
							int num = 2;
							for (int i = 0; i < newCats.Count; i++)
							{
								//if (i == newCats.Count - 1) {
								if (newCats.ElementAt(i).Value == true)
								{
									wf2.Cat = newCats.ElementAt(i).Key;
									wf2.MultiPathNum = 1;
								}
								else
								{
									Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
									wf_c.Cat = newCats.ElementAt(i).Key;
									wf_c.MultiPathNum = num;
									num++;
									wf_c.AppliedRules.Add(this.id);
									Functions.multiChangePaths.Add(wf_c);
								}
							}
						}
					}
					else
					{
						if (specialCaseIsTrue == true)
						{
							continueGraphSearch = false;
							return;
						}
						if (earlierIsTrue == true)
						{
							continueGraphSearch = false;
							return;
						}
					}
					startPosGraph = graphPos + 1;
				}
				else
				{
					continueGraphSearch = false;
				}

				if (startPosGraph >= wf2.Word.Count - 1)
					continueGraphSearch = false;
			}
			else
			{
				continueGraphSearch = false;
			}
		}

		public void AddPhonOnly()
		{
			throw new NotImplementedException();
		}

		public void AddPhonAddGraph(ref Wordform wf2, ref bool changed, Change change, MainWindow win_, int? stPosPhon, int? stPosGraph,
									 bool otherChanges)
		{
			/* sowohl phon als auch graph sind vorher leer */
			/* Position anhand von Left/RightSign/Phon feststellen */
			int startPosPhon;
			if (stPosPhon == null)
				startPosPhon = 0;
			else
				startPosPhon = (int)stPosPhon;

			int startPosGraph;
			if (stPosGraph == null)
				startPosGraph = 0;
			else
				startPosGraph = (int)stPosGraph;

			List<int> result = new List<int>();
			List<Dictionary<int?, int?>> allFoundGraphPosLeft = new List<Dictionary<int?, int?>>();
			List<Dictionary<int?, int?>> allFoundPhonPosLeft = new List<Dictionary<int?, int?>>();
			List<Dictionary<int?, int?>> allFoundGraphPosRight = new List<Dictionary<int?, int?>>();
			List<Dictionary<int?, int?>> allFoundPhonPosRight = new List<Dictionary<int?, int?>>();
			List<List<int?>> allResultLeft = new List<List<int?>>();
			List<List<int?>> allResultRight = new List<List<int?>>();
			Dictionary<String, bool> newCats = new Dictionary<String, bool>();

			if (!(this.leftGraph.Count == 1 && this.leftGraph[0].Count == 1 && this.leftGraph[0][0].Symbol == ""))
			{
				/* linker graph. Kontext ist angegeben */
				foreach (List<Sign> leftG in this.leftGraph)
				{
					Dictionary<int?, int?> foundGraphPosLeft = wf2.SearchGraphInWord(leftG, startPosGraph);
					if (foundGraphPosLeft != null)
					{
						if (foundGraphPosLeft.ContainsKey(wf2.Word.Count - 1) && foundGraphPosLeft[wf2.Word.Count - 1] == wf2.Word.Count - 1)
						{
							foundGraphPosLeft.Remove(wf2.Word.Count - 1);
						}
						if (foundGraphPosLeft != null)
							allFoundGraphPosLeft.Add(foundGraphPosLeft);
					}
				}
				if (allFoundGraphPosLeft.Count == 0)
					return;
			}

			if (!(this.leftPhon.Count == 1 && this.leftPhon[0].Count == 1 && this.leftPhon[0][0].Symbol == ""))
			{
				/* linker phon. Kontext ist angegeben */
				foreach (List<Sign> leftP in this.leftPhon)
				{
					Dictionary<int?, int?> foundPhonPosLeft = wf2.SearchPhonInWord(leftP, startPosPhon);
					if (foundPhonPosLeft != null)
					{
						if (foundPhonPosLeft.ContainsKey(wf2.Phonetic.Count - 1) && foundPhonPosLeft[wf2.Phonetic.Count - 1] == wf2.Phonetic.Count - 1)
						{
							foundPhonPosLeft.Remove(wf2.Phonetic.Count - 1);
						}
						if (foundPhonPosLeft != null)
							allFoundPhonPosLeft.Add(foundPhonPosLeft);
					}
				}
				if (allFoundPhonPosLeft.Count == 0)
					return;
			}

			if (!(this.leftGraph.Count == 1 && this.leftGraph[0].Count == 1 && this.leftGraph[0][0].Symbol == ""))
			{
				if (!(this.leftPhon.Count == 1 && this.leftPhon[0].Count == 1 && this.leftPhon[0][0].Symbol == ""))
				{
					/* linker phon. und graph. Kontext sind angegeben */
					foreach (Dictionary<int?, int?> foundGraphPosLeft in allFoundGraphPosLeft)
					{
						foreach (Dictionary<int?, int?> foundPhonPosLeft in allFoundPhonPosLeft)
						{
							List<int?> resultLeft = wf2.CompareMapping(foundPhonPosLeft, foundGraphPosLeft);
							if (resultLeft != null)
								allResultLeft.Add(resultLeft);
						}
					}
					if (allResultLeft.Count == 0)
						return;
				}
				else
				{
					/* nur linker graph. Kontext sind angegeben */
					foreach (Dictionary<int?, int?> foundGraphPosLeft in allFoundGraphPosLeft)
					{
						List<int?> resultLeft = wf2.CompareMapping(null, foundGraphPosLeft);
						if (resultLeft != null)
							allResultLeft.Add(resultLeft);
					}
					if (allResultLeft.Count == 0)
						return;
				}
			}
			else
			{
				if (!(this.leftPhon.Count == 1 && this.leftPhon[0].Count == 1 && this.leftPhon[0][0].Symbol == ""))
				{
					/* nur linker phon. Kontext sind angegeben */
					foreach (Dictionary<int?, int?> foundPhonPosLeft in allFoundPhonPosLeft)
					{
						List<int?> resultLeft = wf2.CompareMapping(foundPhonPosLeft, null);
						if (resultLeft != null)
							allResultLeft.Add(resultLeft);
					}
					if (allResultLeft.Count == 0)
						return;
				}
			}

			if (!(this.rightGraph.Count == 1 && this.rightGraph[0].Count == 1 && this.rightGraph[0][0].Symbol == ""))
			{
				/* rechter graph. Kontext ist angegeben */
				foreach (List<Sign> rightG in this.rightGraph)
				{
					Dictionary<int?, int?> foundGraphPosRight = wf2.SearchGraphInWord(rightG, startPosGraph);
					if (foundGraphPosRight != null)
					{
						if (foundGraphPosRight.ContainsKey(0) && foundGraphPosRight[0] == 0)
						{
							foundGraphPosRight.Remove(0);
						}
						if (foundGraphPosRight != null)
							allFoundGraphPosRight.Add(foundGraphPosRight);
					}
				}
				if (allFoundGraphPosRight.Count == 0)
					return;
			}

			if (!(this.rightPhon.Count == 1 && this.rightPhon[0].Count == 1 && this.rightPhon[0][0].Symbol == ""))
			{
				/* rechter phon. Kontext ist angegeben */
				foreach (List<Sign> rightP in this.rightPhon)
				{
					Dictionary<int?, int?> foundPhonPosRight = wf2.SearchPhonInWord(rightP, startPosPhon);
					if (foundPhonPosRight != null)
					{
						if (foundPhonPosRight.ContainsKey(0) && foundPhonPosRight[0] == 0)
						{
							foundPhonPosRight.Remove(0);
						}
						if (foundPhonPosRight != null)
							allFoundPhonPosRight.Add(foundPhonPosRight);
					}
				}
				if (allFoundPhonPosRight.Count == 0)
					return;
			}

			if (!(this.rightGraph.Count == 1 && this.rightGraph[0].Count == 1 && this.rightGraph[0][0].Symbol == ""))
			{
				if (!(this.rightPhon.Count == 1 && this.rightPhon[0].Count == 1 && this.rightPhon[0][0].Symbol == ""))
				{
					/* rechter phon. und graph. Kontext sind angegeben */
					foreach (Dictionary<int?, int?> foundGraphPosRight in allFoundGraphPosRight)
					{
						foreach (Dictionary<int?, int?> foundPhonPosRight in allFoundPhonPosRight)
						{
							List<int?> resultRight = wf2.CompareMapping(foundPhonPosRight, foundGraphPosRight);
							if (resultRight != null)
								allResultRight.Add(resultRight);
						}
					}
					if (allResultRight.Count == 0)
						return;
				}
				else
				{
					/* nur rechter graph. Kontext sind angegeben */
					foreach (Dictionary<int?, int?> foundGraphPosRight in allFoundGraphPosRight)
					{
						List<int?> resultRight = wf2.CompareMapping(null, foundGraphPosRight);
						if (resultRight != null)
							allResultRight.Add(resultRight);
					}
					if (allResultRight.Count == 0)
						return;
				}
			}
			else
			{
				if (!(this.rightPhon.Count == 1 && this.rightPhon[0].Count == 1 && this.rightPhon[0][0].Symbol == ""))
				{
					/* nur rechter phon. Kontext sind angegeben */
					foreach (Dictionary<int?, int?> foundPhonPosRight in allFoundPhonPosRight)
					{
						List<int?> resultRight = wf2.CompareMapping(foundPhonPosRight, null);
						if (resultRight != null)
							allResultRight.Add(resultRight);
					}
					if (allResultRight.Count == 0)
						return;
				}
			}


			if (allResultLeft.Count == 0)
			{
				/* --- linker Kontext ist nicht angegeben --- */

				foreach (List<int?> resultRight in allResultRight)
				{
					/* Bedingungen (außer Zeit, Left, Right, StressPattern) prüfen */
					if (CheckConditions(this.id, false, false, false, false, false, false, true, true, true, true, true, ref wf2,
							null, null, null, null, 99, 99, 99, win_, stPosPhon, stPosGraph, otherChanges, ref newCats) == true)
					{
						Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
						result = resultRight.Where(x => x != null).Cast<int>().ToList();

						int phonPos = 99;
						int lastPhonPos;
						int graphPos = 99;
						int lastGraphPos;
						int mappingIndex;
						int posInGraphPosMapping;
						int posInPhonPosMapping;

						if (resultRight[0] == null)
						{
							/* phonPos ist null */
							graphPos = result[0];
							lastGraphPos = result[1];
							mappingIndex = result[2];
							posInGraphPosMapping = result[3];

							phonPos = (int)wf2.MappingList[(int)mappingIndex].PhonPos[0];

						}
						else if (resultRight[2] == null)
						{
							/* graphPos ist null */
							phonPos = result[0];
							lastPhonPos = result[1];
							mappingIndex = result[2];
							posInPhonPosMapping = result[3];

							graphPos = (int)wf2.MappingList[(int)mappingIndex].GraphPos[0];

						}
						else
						{
							phonPos = result[0];
							lastPhonPos = result[1];
							graphPos = result[2];
							lastGraphPos = result[3];
							mappingIndex = result[4];
							posInGraphPosMapping = result[5];
							posInPhonPosMapping = result[6];
						}

						SaveStressBefore(wf2);

						/* Wort wandeln */
						wf2.Word.InsertRange(graphPos, ObjectExtensions.Copy(change.GraphAfter));
						wf2.Phonetic.InsertRange(phonPos, ObjectExtensions.Copy(change.PhonAfter));

						int graphNumDiff = change.GraphAfter.Count;
						int phonNumDiff = change.PhonAfter.Count;

						/* neues SignMapping einfügen */
						SignMapping sm = new SignMapping(new List<int?> { graphPos }, new List<int?> { phonPos });
						wf2.MappingList.Insert(mappingIndex, sm);

						/* folgende Grapheme in MappingList neu verlinken */
						for (int i = mappingIndex + 1; i < wf2.MappingList.Count; i++)
						{
							for (int j = 0; j < wf2.MappingList[i].GraphPos.Count; j++)
							{
								wf2.MappingList[i].GraphPos[j] += 1;
							}
						}

						/* folgende Phone in MappingList neu verlinken */
						for (int i = mappingIndex + 1; i < wf2.MappingList.Count; i++)
						{
							for (int j = 0; j < wf2.MappingList[i].PhonPos.Count; j++)
							{
								wf2.MappingList[i].PhonPos[j] += 1;
							}
						}

						/* neue Zeit zuweisen */
						if (wf2.Time < this.startDate)
							wf2.Time = this.startDate;

						/* neue Silbengrenzen finden und Betonung neu setzen */
						wf2.SeparateSyllables();
						ResetStress(wf2, change);
						wf2.LastChanges.Add(change);
						changed = true;
						wf2.AppliedRules.Add(this.id);

						/* Kategorie splitten und mehrere Pfade verfolgen? */
						if (newCats.Count > 1)
						{
							int num = 2;
							for (int i = 0; i < newCats.Count; i++)
							{
								if (newCats.ElementAt(i).Value == true)
								{
									wf2.Cat = newCats.ElementAt(i).Key;
									wf2.MultiPathNum = 1;
								}
								else
								{
									Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
									wf_c.Cat = newCats.ElementAt(i).Key;
									wf_c.MultiPathNum = num;
									num++;
									wf_c.AppliedRules.Add(this.id);
									Functions.multiChangePaths.Add(wf_c);
								}
							}
						}

						if (resultRight[0] != null)
							startPosGraph = (int)resultRight[0];
						else
							startPosGraph = (int)wf2.MappingList[(int)resultRight[2]].GraphPos[0];

						if (resultRight[1] != null)
							startPosPhon = (int)resultRight[1];
						else
							startPosPhon = (int)wf2.MappingList[(int)resultRight[2]].PhonPos[0];
					}
					else
					{
						startPosGraph++;
						startPosPhon++;

					}
				}
			}
			else if (allResultRight.Count == 0)
			{
				/* --- rechter Kontext ist nicht angegeben --- */

				foreach (List<int?> resultLeft in allResultLeft)
				{

					/* Bedingungen (außer Zeit, Left, Right, StressPattern) prüfen */
					if (CheckConditions(this.id, false, false, false, false, false, false, true, true, true, true, true, ref wf2,
							null, null, null, null, 99, 99, 99, win_, stPosPhon, stPosGraph, otherChanges, ref newCats) == true)
					{
						Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
						result = resultLeft.Where(x => x != null).Cast<int>().ToList();

						int phonPos = 99;
						int lastPhonPos;
						int graphPos = 99;
						int lastGraphPos;
						int mappingIndex;
						int posInGraphPosMapping;
						int posInPhonPosMapping;

						if (resultLeft[0] == null)
						{
							/* phonPos ist null */
							graphPos = result[0] + 1;
							lastGraphPos = result[1] + 1;
							mappingIndex = result[2] + 1;
							posInGraphPosMapping = result[3];

							phonPos = (int)wf2.MappingList[(int)mappingIndex].PhonPos[0];

						}
						else if (resultLeft[2] == null)
						{
							/* graphPos ist null */
							phonPos = result[0] + 1;
							lastPhonPos = result[1] + 1;
							mappingIndex = result[2] + 1;
							posInPhonPosMapping = result[3];

							graphPos = (int)wf2.MappingList[(int)mappingIndex].GraphPos[0];

						}
						else
						{
							phonPos = result[0] + 1;
							lastPhonPos = result[1] + 1;
							graphPos = result[2] + 1;
							lastGraphPos = result[3] + 1;
							mappingIndex = result[4] + 1;
							posInGraphPosMapping = result[5];
							posInPhonPosMapping = result[6];
						}

						SaveStressBefore(wf2);

						/* Wort wandeln */
						wf2.Word.InsertRange(graphPos, ObjectExtensions.Copy(change.GraphAfter));
						wf2.Phonetic.InsertRange(phonPos, ObjectExtensions.Copy(change.PhonAfter));

						int graphNumDiff = change.GraphAfter.Count;
						int phonNumDiff = change.PhonAfter.Count;

						/* neues SignMapping einfügen */
						SignMapping sm = new SignMapping(new List<int?> { graphPos }, new List<int?> { phonPos });
						wf2.MappingList.Insert(mappingIndex, sm);

						/* folgende Grapheme in MappingList neu verlinken */
						for (int i = mappingIndex + 1; i < wf2.MappingList.Count; i++)
						{
							for (int j = 0; j < wf2.MappingList[i].GraphPos.Count; j++)
							{
								wf2.MappingList[i].GraphPos[j] += 1;
							}
						}

						/* folgende Phone in MappingList neu verlinken */
						for (int i = mappingIndex + 1; i < wf2.MappingList.Count; i++)
						{
							for (int j = 0; j < wf2.MappingList[i].PhonPos.Count; j++)
							{
								wf2.MappingList[i].PhonPos[j] += 1;
							}
						}

						/* neue Zeit zuweisen */
						if (wf2.Time < this.startDate)
							wf2.Time = this.startDate;

						/* neue Silbengrenzen finden und Betonung neu setzen */
						wf2.SeparateSyllables();
						ResetStress(wf2, change);
						wf2.LastChanges.Add(change);
						changed = true;
						wf2.AppliedRules.Add(this.id);

						/* Kategorie splitten und mehrere Pfade verfolgen? */
						if (newCats.Count > 1)
						{
							int num = 2;
							for (int i = 0; i < newCats.Count; i++)
							{
								if (newCats.ElementAt(i).Value == true)
								{
									wf2.Cat = newCats.ElementAt(i).Key;
									wf2.MultiPathNum = 1;
								}
								else
								{
									Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
									wf_c.Cat = newCats.ElementAt(i).Key;
									wf_c.MultiPathNum = num;
									num++;
									wf_c.AppliedRules.Add(this.id);
									Functions.multiChangePaths.Add(wf_c);
								}
							}
						}

						if (resultLeft[0] != null)
							startPosGraph = (int)resultLeft[0];
						else
							startPosGraph = (int)wf2.MappingList[(int)resultLeft[2]].GraphPos[0];

						if (resultLeft[1] != null)
							startPosPhon = (int)resultLeft[1];
						else
							startPosPhon = (int)wf2.MappingList[(int)resultLeft[2]].PhonPos[0];

					}
					else
					{
						startPosGraph++;
						startPosPhon++;
					}
				}
			}
			else
			{
				/* --- beide Kontexte sind angegeben --- */

				foreach (List<int?> resultLeft in allResultLeft)
				{
					foreach (List<int?> resultRight in allResultRight)
					{

						/* prüfen ob linker und rechter Kontext nebeneinander liegen */
						if ((resultLeft[4] == resultRight[4] - 1) && ((resultRight[7] == null) || (resultRight[7] == 0)))
						{

							/* Bedingungen (außer Zeit, Left, Right, StressPattern) prüfen */
							if (CheckConditions(this.id, false, false, false, false, false, false, true, true, true, true, true, ref wf2,
									null, null, null, null, 99, 99, 99, win_, stPosPhon, stPosGraph, otherChanges, ref newCats) == true)
							{

								Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
								result = resultRight.Where(x => x != null).Cast<int>().ToList();

								int phonPos = 99;
								int lastPhonPos;
								int graphPos = 99;
								int lastGraphPos;
								int mappingIndex;
								int posInGraphPosMapping;
								int posInPhonPosMapping;

								if (resultRight[0] == null)
								{
									/* phonPos ist null */
									graphPos = result[0];
									lastGraphPos = result[1];
									mappingIndex = result[2];
									posInGraphPosMapping = result[3];

									phonPos = (int)wf2.MappingList[(int)mappingIndex].PhonPos[0];

								}
								else if (resultRight[2] == null)
								{
									/* graphPos ist null */
									phonPos = result[0];
									lastPhonPos = result[1];
									mappingIndex = result[2];
									posInPhonPosMapping = result[3];

									graphPos = (int)wf2.MappingList[(int)mappingIndex].GraphPos[0];

								}
								else
								{
									phonPos = result[0];
									lastPhonPos = result[1];
									graphPos = result[2];
									lastGraphPos = result[3];
									mappingIndex = result[4];
									posInGraphPosMapping = result[5];
									posInPhonPosMapping = result[6];
								}

								SaveStressBefore(wf2);

								/* Wort wandeln */
								wf2.Word.InsertRange(graphPos, ObjectExtensions.Copy(change.GraphAfter));
								wf2.Phonetic.InsertRange(phonPos, ObjectExtensions.Copy(change.PhonAfter));

								int graphNumDiff = change.GraphAfter.Count;
								int phonNumDiff = change.PhonAfter.Count;

								/* neues SignMapping einfügen */
								SignMapping sm = new SignMapping(new List<int?> { graphPos }, new List<int?> { phonPos });
								wf2.MappingList.Insert(mappingIndex, sm);

								/* folgende Grapheme in MappingList neu verlinken */
								for (int i = mappingIndex + 1; i < wf2.MappingList.Count; i++)
								{
									for (int j = 0; j < wf2.MappingList[i].GraphPos.Count; j++)
									{
										wf2.MappingList[i].GraphPos[j] += 1;
									}
								}

								/* folgende Phone in MappingList neu verlinken */
								for (int i = mappingIndex + 1; i < wf2.MappingList.Count; i++)
								{
									for (int j = 0; j < wf2.MappingList[i].PhonPos.Count; j++)
									{
										wf2.MappingList[i].PhonPos[j] += 1;
									}
								}

								/* neue Zeit zuweisen */
								if (wf2.Time < this.startDate)
									wf2.Time = this.startDate;

								/* neue Silbengrenzen finden und Betonung neu setzen */
								wf2.SeparateSyllables();
								ResetStress(wf2, change);
								wf2.LastChanges.Add(change);
								changed = true;
								wf2.AppliedRules.Add(this.id);

								/* Kategorie splitten und mehrere Pfade verfolgen? */
								if (newCats.Count > 1)
								{
									int num = 2;
									for (int i = 0; i < newCats.Count; i++)
									{
										if (newCats.ElementAt(i).Value == true)
										{
											wf2.Cat = newCats.ElementAt(i).Key;
											wf2.MultiPathNum = 1;
										}
										else
										{
											Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
											wf_c.Cat = newCats.ElementAt(i).Key;
											wf_c.MultiPathNum = num;
											num++;
											wf_c.AppliedRules.Add(this.id);
											Functions.multiChangePaths.Add(wf_c);
										}
									}
								}

								if (resultRight[0] != null)
									startPosGraph = (int)resultRight[0];
								else
									startPosGraph = (int)wf2.MappingList[(int)resultRight[2]].GraphPos[0];

								if (resultRight[1] != null)
									startPosPhon = (int)resultRight[1];
								else
									startPosPhon = (int)wf2.MappingList[(int)resultRight[2]].PhonPos[0];

							}
							else
							{
								startPosGraph++;
								startPosPhon++;
							}
						}
					}
				}
			}
		}

		public void AddPhonRemoveGraph()
		{
			throw new NotImplementedException();
		}

		public void AddPhonReplaceGraph()
		{
			throw new NotImplementedException();
		}

		public void RemovePhonOnly(ref Wordform wf2, Wordform wf, ref int startPosPhon, ref bool changed, ref bool continuePhonSearch,
									Change change, List<Sign> phonSignsBefore, Dictionary<int?, int?> foundPhonPos, MainWindow win_,
									int? stPosPhon, int? stPosGraph, bool otherChanges)
		{

			List<int?> resultOrNull = null;
			List<int> result = new List<int>();
			Dictionary<String, bool> newCats = new Dictionary<String, bool>();

			resultOrNull = wf2.CompareMapping(foundPhonPos, null);

			if (resultOrNull != null)
			{

				result = resultOrNull.Where(x => x != null).Cast<int>().ToList();
				int phonPos = result[0];
				int lastPhonPos = result[1];
				int mappingIndex = result[2];
				int posInPhonPosMapping = result[3];
				int lastPosInPhonPosMapping = result[4];

				Functions.printDebugText("\t\t[" + wf2.Phonetic[phonPos].Symbol + "] \t(Ids: + " + phonPos + ")" + Environment.NewLine, true);

				if (CheckConditions(this.id, true, true, true, true, true, true, false, true, true, true, true, ref wf2, phonPos, lastPhonPos,
						null, null, mappingIndex, phonSignsBefore.Count, 1, win_, stPosPhon, stPosGraph, otherChanges, ref newCats) == true)
				{

					Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
					SaveStressBefore(wf2);

					/* Wort wandeln */
					wf2.Phonetic.RemoveRange(phonPos, phonSignsBefore.Count);

					/* Mapping neu */
					RemapPhonWithLessSigns(wf2, phonSignsBefore.Count, change.PhonAfter.Count, phonPos);

					/* neue Zeit zuweisen */
					if (wf2.Time < this.startDate)
						wf2.Time = this.startDate;

					/* neue Silbengrenzen finden und Betonung neu setzen */
					wf2.SeparateSyllables();
					ResetStress(wf2, change);
					wf2.LastChanges.Add(change);
					changed = true;
					wf2.AppliedRules.Add(this.id);

					/* Kategorie splitten und mehrere Pfade verfolgen? */
					if (newCats.Count > 1)
					{
						int num = 2;
						for (int i = 0; i < newCats.Count; i++)
						{
							if (newCats.ElementAt(i).Value == true)
							{
								wf2.Cat = newCats.ElementAt(i).Key;
								wf2.MultiPathNum = 1;
							}
							else
							{
								Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
								wf_c.Cat = newCats.ElementAt(i).Key;
								wf_c.MultiPathNum = num;
								num++;
								wf_c.AppliedRules.Add(this.id);
								Functions.multiChangePaths.Add(wf_c);
							}
						}
					}
				}
				else
				{
					if (specialCaseIsTrue == true)
					{
						continuePhonSearch = false;
						return;
					}
					if (earlierIsTrue == true)
					{
						continuePhonSearch = false;
						return;
					}
				}
				startPosPhon = phonPos + 1;

			}
			else
			{
				continuePhonSearch = false;
			}

			if (startPosPhon >= wf2.Phonetic.Count - 1)
				continuePhonSearch = false;
		}

		public void RemovePhonRemoveGraph(ref Wordform wf2, Wordform wf, ref int startPosGraph, ref int startPosPhon, ref bool changed,
										   ref bool continuePhonSearch, ref bool continueGraphSearch,
										   Change change, List<Sign> graphSignsBefore, List<Sign> phonSignsBefore, Dictionary<int?, int?> foundPhonPos,
										   MainWindow win_, int? stPosPhon, int? stPosGraph, bool otherChanges)
		{
			List<int?> resultOrNull = null;
			List<int> result = new List<int>();
			Dictionary<String, bool> newCats = new Dictionary<String, bool>();

			Dictionary<int?, int?> foundGraphPos = wf2.SearchGraphInWord(graphSignsBefore, startPosGraph);

			if ((foundGraphPos != null) && (foundGraphPos.Count != 0))
			{
				resultOrNull = wf2.CompareMapping(foundPhonPos, foundGraphPos);

				if (resultOrNull != null)
				{

					result = resultOrNull.Where(x => x != null).Cast<int>().ToList();
					int phonPos = result[0];
					int lastPhonPos = result[1];
					int graphPos = result[2];
					int lastGraphPos = result[3];
					int mappingIndex = result[4];
					int posInGraphPosMapping = result[5];
					int lastPosInGraphPosMapping = result[6];
					int posInPhonPosMapping = result[7];
					int lastPosInPhonPosMapping = result[8];

					Functions.printDebugText("\t\t[" + wf2.Phonetic[phonPos].Symbol + "] ; <" + wf2.Word[graphPos].Symbol +
											 "> \t(Ids: + " + phonPos + "," + graphPos + ")" + Environment.NewLine, true);

					if (CheckConditions(this.id, true, true, true, true, true, true, false, true, true, true, true, ref wf2, phonPos, lastPhonPos,
							graphPos, lastGraphPos, mappingIndex, phonSignsBefore.Count, graphSignsBefore.Count, win_, stPosPhon,
							stPosGraph, otherChanges, ref newCats) == true)
					{

						Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
						SaveStressBefore(wf2);

						/* Wort wandeln */
						wf2.Word.RemoveRange(graphPos, graphSignsBefore.Count);
						wf2.Phonetic.RemoveRange(phonPos, phonSignsBefore.Count);

						/* Suffix wurde geändert */
						UpdateSuffix(ref wf2, graphPos, graphSignsBefore.Count, change.GraphAfter.Count);

						/* Mapping neu */
						RemapGraphWithLessSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos);
						RemapPhonWithLessSigns(wf2, phonSignsBefore.Count, change.PhonAfter.Count, phonPos);
						RemoveNullMappings(wf2);

						/* neue Zeit zuweisen */
						if (wf2.Time < this.startDate)
							wf2.Time = this.startDate;

						/* neue Silbengrenzen finden und Betonung neu setzen */
						wf2.SeparateSyllables();
						ResetStress(wf2, change);
						wf2.LastChanges.Add(change);
						changed = true;
						wf2.AppliedRules.Add(this.id);

						/* Kategorie splitten und mehrere Pfade verfolgen? */
						if (newCats.Count > 1)
						{
							int num = 2;
							for (int i = 0; i < newCats.Count; i++)
							{
								if (newCats.ElementAt(i).Value == true)
								{
									wf2.Cat = newCats.ElementAt(i).Key;
									wf2.MultiPathNum = 1;
								}
								else
								{
									Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
									wf_c.Cat = newCats.ElementAt(i).Key;
									wf_c.MultiPathNum = num;
									num++;
									wf_c.AppliedRules.Add(this.id);
									Functions.multiChangePaths.Add(wf_c);
								}
							}
						}
					}
					else
					{
						if (specialCaseIsTrue == true)
						{
							continueGraphSearch = false;
							return;
						}
						if (earlierIsTrue == true)
						{
							continueGraphSearch = false;
							return;
						}
					}
					continuePhonSearch = true;
					continueGraphSearch = true;
					startPosGraph = graphPos + 1;
					startPosPhon = phonPos + 1;

				}
				else
				{
					continuePhonSearch = false;
					continueGraphSearch = false;
				}

				if ((startPosPhon >= wf2.Phonetic.Count - 1) && (startPosGraph >= wf2.Word.Count - 1))
				{
					continuePhonSearch = false;
					continueGraphSearch = false;
				}
			}
			else
			{
				continueGraphSearch = false;
			}
		}

		public void RemovePhonAddGraph()
		{
			throw new NotImplementedException();
		}

		public void RemovePhonReplaceGraph()
		{
			throw new NotImplementedException();
		}

		public void ReplacePhonOnly(ref Wordform wf2, ref int startPosPhon, ref bool changed, ref bool continuePhonSearch,
									 Change change, List<Sign> phonSignsBefore, Dictionary<int?, int?> foundPhonPos, MainWindow win_,
									 int? stPosPhon, int? stPosGraph, bool otherChanges)
		{
			List<int?> resultOrNull = null;
			List<int> result = new List<int>();
			Dictionary<String, bool> newCats = new Dictionary<String, bool>();

			resultOrNull = wf2.CompareMapping(foundPhonPos, null);

			if (resultOrNull != null)
			{
				result = resultOrNull.Where(x => x != null).Cast<int>().ToList();
				int phonPos = result[0];
				int lastPhonPos = result[1];
				int mappingIndex = result[2];
				int posInPhonPosMapping = result[3];
				int lastPosInPhonPosMapping = result[4];

				Functions.printDebugText("\t\t[" + wf2.Phonetic[phonPos].Symbol + "] \t(Ids: + " + phonPos + ")" + Environment.NewLine, true);

				if (CheckConditions(this.id, true, true, true, true, true, true, true, true, true, true, true, ref wf2, phonPos, lastPhonPos,
						null, null, mappingIndex, phonSignsBefore.Count, 1, win_, stPosPhon, stPosGraph, otherChanges, ref newCats) == true)
				{

					Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
					SaveStressBefore(wf2);
					List<Sign> phonAfterWithStress = SetStressAfter(wf2, change, phonPos, phonSignsBefore);

					/* Wort wandeln */
					wf2.Phonetic.RemoveRange(phonPos, phonSignsBefore.Count);
					wf2.Phonetic.InsertRange(phonPos, ObjectExtensions.Copy(phonAfterWithStress));

					for (int i = 0; i < change.PhonAfter.Count; i++)
					{
						wf2.Phonetic[phonPos + i].AppliedRules.Add(this.id);
					}

					/* Mapping neu */

					if (phonSignsBefore.Count < change.PhonAfter.Count)
					{
						/* ---------- [A] > [BC] ---------- */
						RemapPhonWithMoreSigns(wf2, phonSignsBefore.Count, change.PhonAfter.Count, phonPos, lastPhonPos,
							mappingIndex, posInPhonPosMapping);

					}
					else if (phonSignsBefore.Count > change.PhonAfter.Count)
					{
						/* ---------- [AB] > [C] ---------- */

						int phonNumDiff = phonSignsBefore.Count - change.PhonAfter.Count;

						/* phon. Überhangzeichen löschen und mit phon. Überhangzeichen gemappte graph. Zeichen auf Zielphonem mappen */
						for (int i = phonPos + phonSignsBefore.Count - phonNumDiff; i < phonPos + phonSignsBefore.Count; i++)
						{

							Tuple<int, int> indexTuple = wf2.GetIndexFromPhon(phonPos);
							int? gp = wf2.GetGraphPosFromPhon(i);
							if (gp != null)
							{
								wf2.DeleteGraphFromMapping((int)gp);
								wf2.MappingList[indexTuple.Item1].GraphPos.Add(gp);
							}
							wf2.DeletePhonFromMapping(i);
							RemoveNullMappings(wf2);

						}

						/* folgende Phone in MappingList neu verlinken */
						for (int i = phonPos + phonSignsBefore.Count - phonNumDiff; i < wf2.Phonetic.Count(); i++)
						{
							wf2.SetMappingForPhon(i + phonNumDiff, i);
						}

					}
					/* else
					 * ---------- [A] > [B] ----------
					 * tue nichts */

					/* neue Zeit zuweisen */
					if (wf2.Time < this.startDate)
						wf2.Time = this.startDate;

					/* neue Silbengrenzen finden und Betonung neu setzen */
					wf2.SeparateSyllables();
					ResetStress(wf2, change);
					wf2.LastChanges.Add(change);
					changed = true;
					wf2.AppliedRules.Add(this.id);

					/* Kategorie splitten und mehrere Pfade verfolgen? */
					if (newCats.Count > 1)
					{
						int num = 2;
						for (int i = 0; i < newCats.Count; i++)
						{
							if (newCats.ElementAt(i).Value == true)
							{
								wf2.Cat = newCats.ElementAt(i).Key;
								wf2.MultiPathNum = 1;
							}
							else
							{
								Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
								wf_c.Cat = newCats.ElementAt(i).Key;
								wf_c.MultiPathNum = num;
								num++;
								wf_c.AppliedRules.Add(this.id);
								Functions.multiChangePaths.Add(wf_c);
							}
						}
					}
				}
				else
				{
					if (specialCaseIsTrue == true)
					{
						continuePhonSearch = false;
						return;
					}
					if (earlierIsTrue == true)
					{
						continuePhonSearch = false;
						return;
					}
				}
				startPosPhon = phonPos + 1;

			}
			else
			{
				continuePhonSearch = false;
			}

			if (startPosPhon >= wf2.Phonetic.Count - 1)
				continuePhonSearch = false;
		}

		public void ReplacePhonRemoveGraph()
		{
			throw new NotImplementedException();
		}

		public void ReplacePhonAddGraph()
		{
			throw new NotImplementedException();
		}

		public void ReplacePhonReplaceGraph(ref Wordform wf2, ref int startPosGraph, ref int startPosPhon, ref bool changed,
											 ref bool continuePhonSearch, ref bool continueGraphSearch, Change change, List<Sign> graphSignsBefore,
											 List<Sign> phonSignsBefore, Dictionary<int?, int?> foundPhonPos, MainWindow win_, int? stPosPhon,
											 int? stPosGraph, bool otherChanges)
		{

			List<int?> resultOrNull = null;
			List<int> result = new List<int>();
			Dictionary<String, bool> newCats = new Dictionary<String, bool>();

			Dictionary<int?, int?> foundGraphPos = wf2.SearchGraphInWord(graphSignsBefore, startPosGraph);
			if ((foundGraphPos != null) && (foundGraphPos.Count != 0))
			{
				resultOrNull = wf2.CompareMapping(foundPhonPos, foundGraphPos);

				if (resultOrNull != null)
				{
					result = resultOrNull.Where(x => x != null).Cast<int>().ToList();
					int phonPos = result[0];
					int lastPhonPos = result[1];
					int graphPos = result[2];
					int lastGraphPos = result[3];
					int mappingIndex = result[4];
					int posInGraphPosMapping = result[5];
					int lastPosInGraphPosMapping = result[6];
					int posInPhonPosMapping = result[7];
					int lastPosInPhonPosMapping = result[8];

					Functions.printDebugText("\t\t[" + wf2.Phonetic[phonPos].Symbol + "] ; <" + wf2.Word[graphPos].Symbol +
											 "> \t(Ids: + " + phonPos + "," + graphPos + ")" + Environment.NewLine, true);

					if (CheckConditions(this.id, true, true, true, true, true, true, true, true, true, true, true, ref wf2, phonPos, lastPhonPos,
							graphPos, lastGraphPos, mappingIndex, phonSignsBefore.Count, graphSignsBefore.Count, win_, stPosPhon,
							stPosGraph, otherChanges, ref newCats) == true)
					{

						Wordform wf_old = new Wordform(wf2.CloneWfObjectExtensions());
						SaveStressBefore(wf2);
						List<Sign> phonAfterWithStress = SetStressAfter(wf2, change, phonPos, phonSignsBefore);

						/* Wort wandeln */
						wf2.Word.RemoveRange(graphPos, graphSignsBefore.Count);
						wf2.Word.InsertRange(graphPos, ObjectExtensions.Copy(change.GraphAfter));
						wf2.Phonetic.RemoveRange(phonPos, phonSignsBefore.Count);
						wf2.Phonetic.InsertRange(phonPos, ObjectExtensions.Copy(phonAfterWithStress));

						/* Suffix wurde geändert */
						UpdateSuffix(ref wf2, graphPos, graphSignsBefore.Count, change.GraphAfter.Count);

						for (int i = 0; i < change.GraphAfter.Count; i++)
						{
							wf2.Word[graphPos + i].AppliedRules.Add(this.id);
						}

						for (int i = 0; i < change.PhonAfter.Count; i++)
						{
							wf2.Phonetic[phonPos + i].AppliedRules.Add(this.id);
						}

						if (phonSignsBefore.Count < change.PhonAfter.Count)
						{
							/* ---------- [A] > [BC] ---------- */
							RemapPhonWithMoreSigns(wf2, phonSignsBefore.Count, change.PhonAfter.Count, phonPos, lastPhonPos,
								mappingIndex, posInPhonPosMapping);

							if (graphSignsBefore.Count < change.GraphAfter.Count)
							{
								/* ---------- <A> > <BC> ---------- */
								RemapGraphWithMoreSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos, lastGraphPos,
									mappingIndex, posInGraphPosMapping);

							}
							else if (graphSignsBefore.Count > change.GraphAfter.Count)
							{
								/* ---------- <AB> > <C> ---------- */
								throw new NotImplementedException();
							}
							else
							{
								/* ---------- <A> > <B> ---------- */

								if ((startPosPhon >= wf2.Phonetic.Count - 1) && (startPosGraph >= wf2.Word.Count - 1))
								{
									continuePhonSearch = false;
									continueGraphSearch = false;
								}
							}

						}
						else if (phonSignsBefore.Count > change.PhonAfter.Count)
						{
							/* ---------- [AB] > [C] ---------- */
							RemapPhonWithLessSigns(wf2, phonSignsBefore.Count, change.PhonAfter.Count, phonPos);

							if (graphSignsBefore.Count < change.GraphAfter.Count)
							{
								/* ---------- <A> > <BC> ---------- */
								RemapGraphWithMoreSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos, lastGraphPos,
									mappingIndex, posInGraphPosMapping);
								RemoveNullMappings(wf2);
							}
							else if (graphSignsBefore.Count > change.GraphAfter.Count)
							{
								/* ---------- <AB> > <C> ---------- */
								RemapGraphWithLessSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos);
								RemoveNullMappings(wf2);
							}
							else
							{
								/* ---------- <A> > <B> ---------- */
								RemapNullGraphSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, phonSignsBefore.Count,
													change.PhonAfter.Count, graphPos);/*Tue nichts */
								RemoveNullMappings(wf2);
							}
						}
						else
						{
							/* ---------- [A] > [B] ---------- */

							if (graphSignsBefore.Count < change.GraphAfter.Count)
							{
								/* ---------- <A> > <BC> ---------- */
								RemapGraphWithMoreSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos, lastGraphPos,
									mappingIndex, posInGraphPosMapping);

							}
							else if (graphSignsBefore.Count > change.GraphAfter.Count)
							{
								/* ---------- <AB> > <C> ---------- */
								RemapGraphWithLessSigns(wf2, graphSignsBefore.Count, change.GraphAfter.Count, graphPos);

							}
							/* else
							 * ---------- <A> > <B> ---------- 
							 * Tue nichts*/
						}

						/* neue Zeit zuweisen */
						if (wf2.Time < this.startDate)
							wf2.Time = this.startDate;

						/* neue Silbengrenzen finden und Betonung neu setzen */
						wf2.SeparateSyllables();
						ResetStress(wf2, change);
						wf2.LastChanges.Add(change);
						changed = true;
						wf2.AppliedRules.Add(this.id);

						/* Kategorie splitten und mehrere Pfade verfolgen? */
						if (newCats.Count > 1)
						{
							int num = 2;
							for (int i = 0; i < newCats.Count; i++)
							{
								if (newCats.ElementAt(i).Value == true)
								{
									wf2.Cat = newCats.ElementAt(i).Key;
									wf2.MultiPathNum = 1;
								}
								else
								{
									Wordform wf_c = new Wordform(wf_old.CloneWfObjectExtensions());
									wf_c.Cat = newCats.ElementAt(i).Key;
									wf_c.MultiPathNum = num;
									num++;
									wf_c.AppliedRules.Add(this.id);
									Functions.multiChangePaths.Add(wf_c);
								}
							}
						}
					}
					else
					{
						if (specialCaseIsTrue == true)
						{
							continueGraphSearch = false;
							return;
						}
						if (earlierIsTrue == true)
						{
							continueGraphSearch = false;
							return;
						}
					}
					startPosGraph = graphPos + 1;
					startPosPhon = phonPos + 1;
				}
				else
				{
					continuePhonSearch = false;
					continueGraphSearch = false;
				}

				if ((startPosPhon >= wf2.Phonetic.Count - 1) && (startPosGraph >= wf2.Word.Count - 1))
				{
					continuePhonSearch = false;
					continueGraphSearch = false;
				}
			}
			else
			{
				continueGraphSearch = false;
			}
		}

		public bool CheckConditions(String id, bool checkLeftPhon, bool checkRightPhon, bool checkLeftGraph, bool checkRightGraph,
									 bool checkStress, bool checkSyll, bool checkAppliedRules, bool checkPreCond, bool checkEarlier,
									 bool checkSpecialCase, bool checkOtherConditions, ref Wordform wf2, int? phonPos, int? lastPhonPos, int? graphPos,
									 int? lastGraphPos, int mappingIndex, int phonSignsBeforeCount, int graphSignsBeforeCount, MainWindow win_,
									 int? stPosPhon, int? stPosGraph, bool otherChanges, ref Dictionary<String, bool> newCats)
		{

			if (checkLeftPhon == true)
			{
				Functions.printDebugText("\t\t\tLeftPhon (", true);
				Functions.printDebugText("...", false);

				bool result = CheckLeftPhon(wf2, phonPos, graphPos);
				Functions.printDebugText("): " + result + Environment.NewLine, false);
				if (result == false)
					return false;
			}

			if (checkRightPhon == true)
			{
				Functions.printDebugText("\t\t\tRightPhon (", true);
				Functions.printDebugText("...", false);

				bool result = CheckRightPhon(wf2, lastPhonPos, lastGraphPos);
				Functions.printDebugText("): " + result + Environment.NewLine, false);
				if (result == false)
					return false;
			}

			if (checkLeftGraph == true)
			{
				Functions.printDebugText("\t\t\tLeftGraph (", true);
				Functions.printDebugText("...", false);

				bool result = CheckLeftGraph(wf2, phonPos, graphPos);
				Functions.printDebugText("): " + result + Environment.NewLine, false);
				if (result == false)
					return false;
			}

			if (checkRightGraph == true)
			{
				Functions.printDebugText("\t\t\tRightGraph (", true);
				Functions.printDebugText("...", false);

				bool result = CheckRightGraph(wf2, lastPhonPos, lastGraphPos);
				Functions.printDebugText("): " + result + Environment.NewLine, false);
				if (result == false)
					return false;
			}

			if (checkStress == true)
			{
				bool result = CheckStressPattern(wf2.Syllables, wf2.MappingList, mappingIndex);
				Functions.printDebugText("\t\t\tStress: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			if (checkSyll == true)
			{
				bool result = CheckSyll(wf2, phonPos, graphPos);
				Functions.printDebugText("\t\t\tSyll: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			if (checkAppliedRules == true)
			{
				bool result = CheckAppliedRules(wf2, phonPos, phonSignsBeforeCount, graphPos, graphSignsBeforeCount);
				Functions.printDebugText("\t\t\tAppl Rules: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			if (checkPreCond == true)
			{
				bool result = CheckPreConditions(wf2);
				Functions.printDebugText("\t\t\tPrecond: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			if (checkEarlier == true)
			{
				Functions.printDebugText("\t\t\tEarlier Start ...." + Environment.NewLine, true);
				Wordform result = CheckEarlier(wf2, win_, stPosPhon, stPosGraph, otherChanges);
				Functions.printDebugText("\t\t\t... Earlier End" + Environment.NewLine, true);
				if (result != null)
				{
					wf2 = result;
					return false;
				}
				Functions.printDebugText("\t\t\tEarlier: nein " + Environment.NewLine, true);
			}

			if (checkSpecialCase == true)
			{
				Functions.printDebugText("\t\t\tSpecialCase Start ...." + Environment.NewLine, true);
				Wordform result = CheckSpecialCases(wf2, win_, stPosPhon, stPosGraph, otherChanges);
				Functions.printDebugText("\t\t\t... SpecialCase End" + Environment.NewLine, true);
				if (result != null)
				{
					wf2 = result;
					return false;
				}
				Functions.printDebugText("\t\t\tSpecialCase: nein " + Environment.NewLine, true);
			}

			if (checkOtherConditions == true)
			{
				bool result = CheckOtherConditions(wf2);
				Functions.printDebugText("\t\t\tOther Conditions: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			bool tblResult = CheckTbl(wf2);
			Functions.printDebugText("\t\t\tTbl: " + tblResult + Environment.NewLine, true);
			if (tblResult == false)
				return false;

			bool catResult = CheckCat(wf2, ref newCats);
			Functions.printDebugText("\t\t\tCat: " + catResult + Environment.NewLine, true);
			if (catResult == false)
				return false;

			return true;
		}

		public bool CheckMorphConditions(String id, bool checkAppliedRules, bool checkPreCond, bool checkEarlier, bool checkSpecialCase,
										  bool checkOtherConditions, ref Wordform wf2, MainWindow win_, int? stPosPhon, int? stPosGraph,
										  bool otherChanges, ref Dictionary<String, bool> newCats)
		{
			if (checkAppliedRules == true)
			{
				bool result = CheckAppliedRules(wf2, 0, wf2.Phonetic.Count, 0, wf2.Word.Count);
				Functions.printDebugText("\t\t\tAppl Rules: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			if (checkPreCond == true)
			{
				bool result = CheckPreConditions(wf2);
				Functions.printDebugText("\t\t\tPrecond: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			if (checkEarlier == true)
			{
				Functions.printDebugText("\t\t\tEarlier Start ...." + Environment.NewLine, true);
				Wordform result = CheckEarlier(wf2, win_, stPosPhon, stPosGraph, otherChanges);
				Functions.printDebugText("\t\t\t... Earlier End" + Environment.NewLine, true);
				if (result != null)
				{
					wf2 = result;
					return false;
				}
				Functions.printDebugText("\t\t\tEarlier: nein " + Environment.NewLine, true);
			}

			if (checkSpecialCase == true)
			{
				Functions.printDebugText("\t\t\tSpecialCase Start ...." + Environment.NewLine, true);
				Wordform result = CheckSpecialCases(wf2, win_, stPosPhon, stPosGraph, otherChanges);
				Functions.printDebugText("\t\t\t... SpecialCase End" + Environment.NewLine, true);
				if (result != null)
				{
					wf2 = result;
					return false;
				}
				Functions.printDebugText("\t\t\tSpecialCase: nein " + Environment.NewLine, true);
			}

			if (checkOtherConditions == true)
			{
				bool result = CheckOtherConditions(wf2);
				Functions.printDebugText("\t\t\tOther Conditions: " + result + Environment.NewLine, true);
				if (result == false)
					return false;
			}

			bool tblResult = CheckTbl(wf2);
			Functions.printDebugText("\t\t\tTbl: " + tblResult + Environment.NewLine, true);
			if (tblResult == false)
				return false;

			bool catResult = CheckCat(wf2, ref newCats);
			Functions.printDebugText("\t\t\tCat: " + catResult + Environment.NewLine, true);
			if (catResult == false)
				return false;

			return true;
		}

		public bool CheckStressPattern(List<Syllable> wfSyllables, List<SignMapping> mappingList, int editingIndex)
		{
			if ((this.stressPatterns == null) || (this.stressPatterns.Count == 0))
			{
				/* falls kein Stress in Rule */
				return true;
			}
			else
			{

				/* für Abgleich Anfang und Ende des Wortes als Silbe (# "A _ #) */
				List<Syllable> stressSylls = ObjectExtensions.Copy(wfSyllables);

				/* jede Silbe durchlaufen und prüfen, ob Muster passt */
				for (int i = 0; i < stressSylls.Count; i++)
				{

					/* für jeden Teil des Musters prüfen, ob es ab i passt */
					for (int j = 0; j < this.stressPatterns.Count; j++)
					{

						if (i + j > stressSylls.Count - 1)
							return false;

						/* Muster markiert Anfang und Ende des Wortes */
						if (this.stressPatterns[j].StartOfWord == true)
						{
							if ((i != 0) || (stressSylls[i + j].Syll[0].Symbol != "#"))
								break;
						}

						if (this.stressPatterns[j].EndOfWord == true)
						{
							if ((i + j != stressSylls.Count - 1) || (stressSylls[i + j].Syll[stressSylls[i + j].Syll.Count - 1].Symbol != "#"))
								break;
						}

						if (this.stressPatterns[j].PrimStress == true)
						{
							if (stressSylls[i + j].PrimStress == false)
								break;
						}
						else if (stressPatterns[j].SecStress == true)
						{
							if (stressSylls[i + j].SecStress == false)
								break;
						}
						else
						{
							if (stressSylls[i + j].PrimStress == true)
								break;
							if (stressSylls[i + j].SecStress == true)
								break;
						}

						if (this.stressPatterns[j].SyllSymbol == "_")
						{
							/* wird Silbe gerade bearbeitet? */
							foreach (List<int?> sP in mappingList[editingIndex].SyllPos)
							{
								if (sP[0] != i + j)
								{
									goto NotEditedSyll;
								}
							}
						}

						if (j == this.stressPatterns.Count - 1)
							return true;
					}
				NotEditedSyll:
					continue;
				}
			}
			return false;
		}

		public bool CheckSyll(Wordform wf, int? phonPos, int? graphPos)
		{
			int position = 99;

			if (phonPos != null)
			{
				position = (int)phonPos;
			}
			else
			{

				foreach (SignMapping sm in wf.MappingList)
				{
					foreach (int? gP in sm.GraphPos)
					{
						if (gP == graphPos)
						{
							position = (int)sm.PhonPos.ElementAt(0);
						}
					}
				}
			}

			if (position == 99)
			{
				throw new SymbolNotFoundException("Keine phonPos");
			}

			if ((this.syllType == null) || (this.syllType == ""))
				return true;
			else if ((wf.Phonetic[position].SyllEnd == true) && (this.syllType == "["))
				return true;
			else if ((wf.Phonetic[position].SyllEnd == false) && (this.syllType == "]"))
				return true;
			else
				return false;
		}

		public bool CheckLeftPhon(Wordform wf, int? phonPos, int? graphPos)
		{
			int? position = 99;

			if (phonPos != null)
			{
				position = (int)phonPos;
			}
			else
			{

				foreach (SignMapping sm in wf.MappingList)
				{
					foreach (int? gP in sm.GraphPos)
					{
						if (gP == graphPos)
						{
							position = sm.PhonPos.ElementAt(0);
						}
					}
				}
			}

			if ((this.leftPhon.Count == 1) && (this.leftPhon[0].Count == 1) &&
				(this.leftPhon[0][0].IsEqual(new Sign(""))))
			{
				return true;
			}
			else
			{
				if (position == 0)
				{
					/* kein linker Kontext */
					return false;
				}

				if (position == 99)
				{
					throw new SymbolNotFoundException("Keine phonPos");
				}

				foreach (List<Sign> signs in this.leftPhon)
				{

					/* ab dem Zeichen alle Zeichen nach links überprüfen */
					for (int i = (int)position - 1; i > position - signs.Count - 1; i--)
					{
						if (i == -1)
						{
							break;
						}

						if (!signs[signs.Count - ((int)position - i)].IsEqual(wf.Phonetic[i]))
						{
							break;
						}

						if (i == position - signs.Count)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool CheckRightPhon(Wordform wf, int? phonPos, int? graphPos)
		{
			int? position = 99;

			if (phonPos != null)
			{
				position = (int)phonPos;
			}
			else
			{

				foreach (SignMapping sm in wf.MappingList)
				{
					foreach (int? gP in sm.GraphPos)
					{
						if (gP == graphPos)
						{
							position = sm.PhonPos.ElementAt(0);
						}
					}
				}
			}

			if ((this.rightPhon.Count == 1) && (this.rightPhon[0].Count == 1) &&
				(this.rightPhon[0][0].IsEqual(new Sign(""))))
			{
				return true;

			}
			else
			{
				if (position == wf.Phonetic.Count - 1)
				{
					/* kein rechter Kontext */
					return false;
				}

				if (position == 99)
				{
					throw new SymbolNotFoundException("Keine phonPos");
				}

				foreach (List<Sign> signs in this.rightPhon)
				{

					/* ab dem Zeichen alle Zeichen nach rechts überprüfen */
					for (int i = (int)position + 1; i < position + signs.Count + 1; i++)
					{
						if (i == wf.Phonetic.Count)
						{
							return false;
						}

						if (!signs[i - (int)position - 1].IsEqual(wf.Phonetic[i]))
						{
							break;
						}

						if (i == position + signs.Count)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool CheckLeftGraph(Wordform wf, int? phonPos, int? graphPos)
		{
			int? position = 99;

			if (graphPos != null)
			{
				position = (int)graphPos;
			}
			else
			{

				foreach (SignMapping sm in wf.MappingList)
				{
					foreach (int? pP in sm.PhonPos)
					{
						if (pP == phonPos)
						{
							position = sm.GraphPos.ElementAt(0);
						}
					}
				}
			}

			if ((this.leftGraph.Count == 1) && (this.leftGraph[0].Count == 1) &&
				(this.leftGraph[0][0].IsEqual(new Sign(""))))
			{
				return true;

			}
			else
			{

				if (position == 0)
				{
					/* kein linker Kontext */
					return false;
				}

				if (position == 99)
				{
					throw new SymbolNotFoundException("Keine phonPos");
				}

				foreach (List<Sign> signs in this.leftGraph)
				{

					/* ab dem Zeichen alle Zeichen nach links überprüfen */
					for (int i = (int)position - 1; i > position - signs.Count - 1; i--)
					{
						if (i == -1)
						{
							break;
						}

						if (!signs[signs.Count - ((int)position - i)].IsEqual(wf.Word[i]))
						{
							break;
						}

						if (i == position - signs.Count)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool CheckRightGraph(Wordform wf, int? phonPos, int? graphPos)
		{

			int? position = 99;

			if (graphPos != null)
			{
				position = (int)graphPos;
			}
			else
			{

				foreach (SignMapping sm in wf.MappingList)
				{
					foreach (int? pP in sm.PhonPos)
					{
						if (pP == phonPos)
						{
							position = sm.GraphPos.ElementAt(0);
						}
					}
				}
			}

			if ((this.rightGraph.Count == 1) && (this.rightGraph[0].Count == 1) &&
				(this.rightGraph[0][0].IsEqual(new Sign(""))))
			{
				return true;

			}
			else
			{
				if (position == wf.Word.Count - 1)
				{
					/* kein rechter Kontext */
					return false;
				}

				if (position == 99)
				{
					throw new SymbolNotFoundException("Keine phonPos");
				}

				foreach (List<Sign> signs in this.rightGraph)
				{

					/* ab dem Zeichen alle Zeichen nach rechts überprüfen */
					for (int i = (int)position + 1; i < position + signs.Count + 1; i++)
					{
						if (i == wf.Word.Count)
						{
							return false;
						}

						if (!signs[i - (int)position - 1].IsEqual(wf.Word[i]))
						{
							break;
						}

						if (i == position + signs.Count)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Prüft, ob die angegebenen Regeln angewandt wurden oder nicht.
		/// </summary>
		public bool CheckPreConditions(Wordform wf)
		{
			if (CheckOrPreConditions(wf) == true && CheckAndPreConditions(wf) == true)
				return true;
			return false;
		}

		/// <summary>
		/// Prüft, ob die angegebenen Regeln angewandt wurden.
		/// </summary>
		public bool CheckOrPreConditions(Wordform wf)
		{
			if (this.orPreCond == null)
			{
				return true;
			}
			else
			{

				foreach (String pPC in this.orPreCond)
				{

					if (pPC.StartsWith("!"))
					{
						if (!wf.AppliedRules.Contains(pPC.TrimStart('!')))
							return true;
					}
					else
					{
						if (wf.AppliedRules.Contains(pPC))
							return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Prüft, ob die angegebenen Regeln nicht angewandt wurden.
		/// </summary>
		public bool CheckAndPreConditions(Wordform wf)
		{
			Functions.debugTextWin += "Id: " + this.Id + Environment.NewLine + "Und: ";
			if (this.andPreCond == null)
			{
				Functions.debugTextWin += "---" + Environment.NewLine;
				return true;
			}
			else
			{
				foreach (String pPC in this.andPreCond)
				{

					Functions.debugTextWin += pPC + ", ";

					if (pPC.StartsWith("!"))
					{
						if (wf.AppliedRules.Contains(pPC.TrimStart('!')))
						{

							Functions.debugTextWin += "== false" + Environment.NewLine;
							return false;
						}
					}
					else
					{
						if (!wf.AppliedRules.Contains(pPC))
						{
							Functions.debugTextWin += " == false" + Environment.NewLine;
							return false;
						}
					}
				}

				Functions.debugTextWin += "== true" + Environment.NewLine;
				return true;
			}
		}

		public bool CheckAppliedRules(Wordform wf, int? phonPos, int phonSignsBeforeCount, int? graphPos, int graphSignsBeforeCount)
		{
			if (phonPos == null)
			{
				return true;
			}
			else
			{
				for (int i = 0; i < phonSignsBeforeCount; i++)
				{
					foreach (String str in wf.Phonetic[(int)phonPos + i].AppliedRules)
					{
						if (str == this.id)
							return false;
					}
				}
			}

			if (graphPos == null)
			{
				return true;
			}
			else
			{
				for (int i = 0; i < graphSignsBeforeCount; i++)
				{
					foreach (String str in wf.Word[(int)graphPos + i].AppliedRules)
					{
						if (str == this.id)
							return false;
					}
				}
			}

			return true;
		}

		public Wordform CallCheckRuleAndHandleDependencies(Wordform wf, MainWindow win_, String type, int? stPosPhon, int? stPosGraph, bool otherChanges)
		{
			Functions.tabNum += 3;
			Wordform wf2 = this.CheckRule(wf, win_, stPosPhon, stPosGraph, otherChanges);
			Functions.tabNum -= 3;
			if (wf2 != null)
			{
				if (wf2.PrintRule == null)
					wf2.PrintRule = this;
				return wf2;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Prüft, ob eine chronologisch früher Regel zutrifft
		/// </summary>
		/// <returns><c>true</c>, wenn eine frühere Regel zur aktuellen matcht, <c>false</c> otherwise.</returns>
		/// <param name="wf">Wf.</param>
		public Wordform CheckEarlier(Wordform wf, MainWindow win_, int? stPosPhon, int? stPosGraph, bool otherChanges)
		{
			if (this.earlier.Count == 0)
			{
				return null;
			}

			foreach (String ear in this.earlier)
			{
				if (!wf.AppliedRules.Contains(ear))
				{
					SoundChangeRule scr = Functions.rulListLang1.FirstOrDefault(s => s.id == ear);

					if (scr == null)
					{
						Functions.ShowErrorMessage("Frühere Regel " + ear + " zu Regel " + this.id + " ist nicht definiert.");
					}
					else
					{

						if (scr.EndDate >= wf.Time)
						{

							Wordform wf2 = scr.CallCheckRuleAndHandleDependencies(wf, win_, "earlier", stPosPhon, stPosGraph, otherChanges);
							if (wf2 != null)
							{
								this.earlierIsTrue = true;
								wf2.repeatRules.Add(this.id);
								return wf2;
							}
						}
						else
						{
							continue;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Prüft Spezialfälle.
		/// </summary>
		/// <returns><c>true</c>, wenn ein Spezialfall zur aktuellen Regel gefunden wurde, <c>false</c> otherwise.</returns>
		/// <param name="wf">Wf.</param>
		public Wordform CheckSpecialCases(Wordform wf, MainWindow win_, int? stPosPhon, int? stPosGraph, bool otherChanges)
		{
			if (this.specialCases.Count == 0)
			{
				return null;
			}

			foreach (String specialCase in this.specialCases)
			{
				if (!wf.AppliedRules.Contains(specialCase))
				{
					SoundChangeRule scr = Functions.rulListLang1.FirstOrDefault(s => s.id == specialCase);

					if (scr == null)
					{
						Functions.ShowErrorMessage("Spezialfall " + specialCase + " zu Regel " + this.id + " ist nicht definiert.");
					}
					else
					{
						if (scr.EndDate >= wf.Time)
						{
							Wordform wf2 = scr.CallCheckRuleAndHandleDependencies(wf, win_, "special", stPosPhon, stPosGraph, otherChanges);
							if (wf2 != null)
							{
								this.specialCaseIsTrue = true;
								wf2.repeatRules.Add(this.id);
								return wf2;
							}
						}
						else
						{
							return null;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Prüft weitere Bedingungen
		/// </summary>
		/// <returns><c>true</c>, if other conditions was checked, <c>false</c> otherwise.</returns>
		public bool CheckOtherConditions(Wordform wf2)
		{
			foreach (KeyValuePair<String, String> kvp in this.addConditions)
			{
				if (wf2.Additional.ContainsKey(kvp.Key) && wf2.Additional[kvp.Key] == kvp.Value)
				{
					continue;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Prüft die Tabelle
		/// </summary>
		/// <returns><c>true</c>, if other conditions was checked, <c>false</c> otherwise.</returns>
		public bool CheckTbl(Wordform wf2)
		{

			if (this.tbl == "" || this.tbl == wf2.Tbl)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Prüft die Kategorie
		/// </summary>
		public bool CheckCat(Wordform wf2, ref Dictionary<String, bool> newCats)
		{
			if (this.cat == "" || this.cat == wf2.Cat)
			{
				return true;
			}
			else
			{
				/* falls this.cat in Liste von Dict-Values existiert */

				foreach (KeyValuePair<String, List<String>> entry in Functions.newCatDict)
				{
					foreach (String valCat in entry.Value)
					{
						if ((valCat == this.cat) && (entry.Key == wf2.Cat))
						{
							newCats.Add(valCat, true);
							foreach (String vC in entry.Value)
							{
								if (vC != this.cat)
								{
									newCats.Add(vC, false);
								}
							}
							return true;
						}
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Betonung im Zeichen selbst speichern
		/// </summary>
		/// <param name="wf2">Wf2.</param>
		/// <param name="nuclei">Nuclei.</param>
		public void SaveStressBefore(Wordform wf2)
		{

			/* über alle Silben iterieren */
			for (int i = 0; i < wf2.Syllables.Count; i++)
			{
				if (wf2.Syllables[i].PrimStress == true)
				{
					/* über alle Silben-Zeichen iterieren */
					for (int j = 0; j < wf2.Syllables[i].Syll.Count; j++)
					{
						/* Indizes (Silbe:Zeichen) zu Phonetic mappen */
						foreach (SignMapping sm in wf2.MappingList)
						{
							foreach (List<int?> sP in sm.SyllPos)
							{
								if ((sP[0] == i) && (sP[1] == j))
								{
									/* über Liste von Phonetic-Zeichen iterieren und prüfen, welche in V */
									foreach (int k in sm.PhonPos)
									{
										if (Functions.nuclei.Contains(wf2.Phonetic[k].Symbol))
										{
											/*  Betonung speichern */
											wf2.Phonetic[k].PrimStress = true;
											goto PrimStressFound;
										}
									}
								}
							}
						}
					}
				}

			PrimStressFound:

				if (wf2.Syllables[i].SecStress == true)
				{
					/* über alle Silben-Zeichen iterieren */
					for (int j = 0; j < wf2.Syllables[i].Syll.Count; j++)
					{
						/* Indizes (Silbe:Zeichen) zu Phonetic mappen */
						foreach (SignMapping sm in wf2.MappingList)
						{
							foreach (List<int?> sP in sm.SyllPos)
							{
								if ((sP[0] == i) && (sP[1] == j))
								{
									/* über Liste von Phonetic-Zeichen iterieren und prüfen, welche in V */
									foreach (int k in sm.PhonPos)
									{
										if (Functions.nuclei.Contains(wf2.Phonetic[k].Symbol))
										{
											/*  Betonung speichern */
											wf2.Phonetic[k].SecStress = true;
											goto SecStressFound;
										}
									}
								}
							}
						}
					}
				}

			SecStressFound:
				continue;
			}
		}

		/// <summary>
		/// falls zu ersetzendes Sign Betonung enthält, dann betonung in PhonAfter speichern
		/// </summary>
		public List<Sign> SetStressAfter(Wordform wf2, Change change, int phonPos, List<Sign> phonSignsBefore)
		{
			List<Sign> phonAfterWithStress = Functions.DeepClone(change.PhonAfter);

			/* für alle zu ersetzenden Zeichen prüfen, ob sie PrimStress enthalten */
			for (int i = phonPos; i < phonPos + phonSignsBefore.Count; i++)
			{
				if (wf2.Phonetic[i].PrimStress == true)
				{
					for (int j = 0; j < change.PhonAfter.Count; j++)
					{
						if (Functions.nuclei.Contains(change.PhonAfter[j].Symbol))
							phonAfterWithStress[j].PrimStress = true;
					}
				}
				if (wf2.Phonetic[i].SecStress == true)
				{
					for (int j = 0; j < change.PhonAfter.Count; j++)
					{
						if (Functions.nuclei.Contains(change.PhonAfter[j].Symbol))
							phonAfterWithStress[j].SecStress = true;
					}
				}
			}

			return phonAfterWithStress;
		}

		/// <summary>
		/// Betonung für Silbe neu setzen
		/// </summary>
		public void ResetStress(Wordform wf2, Change change)
		{
			foreach (Syllable syll in wf2.Syllables)
			{
				foreach (Sign sign in syll.Syll)
				{
					if (sign.PrimStress == true)
						syll.PrimStress = true;

					if (sign.SecStress == true)
						syll.SecStress = true;
				}
			}

			foreach (Sign sign in change.PhonAfter)
			{
				sign.PrimStress = false;
				sign.SecStress = false;
			}
		}

		public void RemapPhonWithLessSigns(Wordform wf, int phonSignsBeforeCount, int changePhonAfterCount, int phonPos)
		{

			int phonNumDiff = phonSignsBeforeCount - changePhonAfterCount;

			/* Überhangzeichen löschen */
			for (int i = phonPos + phonSignsBeforeCount - phonNumDiff; i < phonPos + phonSignsBeforeCount; i++)
			{
				wf.DeletePhonFromMapping(i);
			}

			/* folgende Phone in MappingList neu verlinken */
			for (int i = phonPos + phonSignsBeforeCount - phonNumDiff; i < wf.Phonetic.Count(); i++)
			{
				wf.SetMappingForPhon(i + phonNumDiff, i);
			}
		}

		public void RemapNullGraphSigns(Wordform wf, int graphSignsBeforeCount, int graphSignsAfterCount, int phonSignsBeforeCount, int changePhonAfterCount, int graphPos)
		{
			int graphPhonNumDiff = graphSignsBeforeCount - changePhonAfterCount;

			Tuple<int, int> indices = wf.GetIndexFromGraph(graphPos + graphPhonNumDiff - 1);
			int newInd = indices.Item2;

			for (int i = graphPos + graphPhonNumDiff; i < graphPos + graphSignsBeforeCount; i++)
			{
				/* Überhangzeichen neu verlinken */
				newInd++;
				Tuple<int, int> indices2 = wf.GetIndexFromGraph(i);
				wf.MappingList[indices2.Item1].GraphPos[indices2.Item2] = null;
				wf.MappingList[indices.Item1].GraphPos.Add(i);
			}
		}

		public void RemapGraphWithLessSigns(Wordform wf, int graphSignsBeforeCount, int changeGraphAfterCount, int graphPos)
		{

			int graphNumDiff = graphSignsBeforeCount - changeGraphAfterCount;

			/* Überhangzeichen löschen */
			for (int i = graphPos + graphSignsBeforeCount - graphNumDiff; i < graphPos + graphSignsBeforeCount; i++)
			{
				wf.DeleteGraphFromMapping(i);
			}

			/* folgende Grapheme in MappingList neu verlinken */
			for (int i = graphPos + graphSignsBeforeCount - graphNumDiff; i < wf.Word.Count(); i++)
			{
				wf.SetMappingForGraph(i + graphNumDiff, i);
			}
		}

		public void RemapPhonWithMoreSigns(Wordform wf, int phonSignsBeforeCount, int changePhonAfterCount, int phonPos, int lastPhonPos,
											int mappingIndex, int posInPhonPosMapping)
		{

			int phonNumDiff = changePhonAfterCount - phonSignsBeforeCount;

			/* folgende Phone in MappingList neu verlinken */
			for (int i = wf.Phonetic.Count() - phonNumDiff - 1; i > lastPhonPos; i--)
			{
				wf.SetMappingForPhon(i, i + phonNumDiff);
			}

			/* neues Mapping für zusätzliche Zeichen setzen */
			for (int i = 1; i < changePhonAfterCount; i++)
			{
				/* falls Zeichen noch nicht mit diesem Index verknüpft sind, 
				 * d.h. erstes und letztes Zeichen haben versch. Indizes*/
				Tuple<int, int> indexFromPhon = wf.GetIndexFromPhon(phonPos + i);
				if (indexFromPhon != null)
				{
					if (indexFromPhon.Item1 != mappingIndex)
					{
						wf.DeletePhonFromMapping(phonPos + i);
						wf.MappingList[mappingIndex].PhonPos.Insert(wf.MappingList[mappingIndex].PhonPos.Count - 1 + i, phonPos + i);
					}
				}
				else
				{
					wf.MappingList[mappingIndex].PhonPos.Insert(posInPhonPosMapping + i, phonPos + i);
				}
			}
		}

		public void RemapGraphWithMoreSigns(Wordform wf, int graphSignsBeforeCount, int changeGraphAfterCount, int graphPos, int lastGraphPos,
											 int mappingIndex, int posInGraphPosMapping)
		{

			int graphNumDiff = changeGraphAfterCount - graphSignsBeforeCount;

			/* folgende Grapheme in MappingList neu verlinken */
			for (int i = wf.Word.Count() - graphNumDiff - 1; i > lastGraphPos; i--)
			{
				wf.SetMappingForGraph(i, i + graphNumDiff);
			}

			/* neues Mapping für zusätzliche Zeichen setzen */
			for (int i = 1; i < changeGraphAfterCount; i++)
			{

				/* falls Zeichen noch nicht mit diesem Index verknüpft sind, 
				 * d.h. erstes und letztes Zeichen haben versch. Indizes*/
				Tuple<int, int> indexFromGraph = wf.GetIndexFromGraph(graphPos + i);
				if (indexFromGraph != null)
				{
					if (indexFromGraph.Item1 != mappingIndex)
					{
						wf.DeleteGraphFromMapping(graphPos + i);
						wf.MappingList[mappingIndex].GraphPos.Insert(wf.MappingList[mappingIndex].GraphPos.Count - 1 + i, graphPos + i);
					}
				}
				else
				{
					wf.MappingList[mappingIndex].GraphPos.Insert(posInGraphPosMapping + i, graphPos + i);
				}
			}
		}

		public void RemoveNullMappings(Wordform wf2)
		{
			/* falls alle Signs in Phon und Graph gelöscht wurden, Element aus MappingList entfernen */
			for (int i = wf2.MappingList.Count - 1; i >= 0; i--)
			{
				for (int j = 0; j < wf2.MappingList[i].GraphPos.Count; j++)
				{
					if (wf2.MappingList[i].GraphPos[j] == null)
					{
						if (j == wf2.MappingList[i].GraphPos.Count - 1)
						{
							/* alle Graph-Signs an dieser Mapping-Stelle wurden gelöscht */
							for (int k = 0; k < wf2.MappingList[i].PhonPos.Count; k++)
							{
								if (wf2.MappingList[i].PhonPos[k] == null)
								{
									if (k == wf2.MappingList[i].PhonPos.Count - 1)
									{
										/* alle Phon-Signs an dieser Mapping-Stelle wurden gelöscht */
										wf2.MappingList.RemoveAt(i);
										if (i > wf2.MappingList.Count - 1)
											break;
									}
									else
									{
										continue;
									}
								}
								else
								{
									break;
								}
							}
							if (i > wf2.MappingList.Count - 1)
								break;
						}
						else
						{
							continue;
						}
					}
					else
					{
						break;
					}
				}
			}
		}

		public static void UpdateSuffix(ref Wordform wf2, int graphPos, int graphSignsBeforeCount, int changeGraphAfterCount)
		{
			if (wf2.Suffix != null)
			{
				int graphSignsDiff = changeGraphAfterCount - graphSignsBeforeCount;

				if (graphPos > wf2.SuffixPos[0])
				{
					/* Suffix wurde geändert */
					for (int i = 0; i < wf2.Suffix.Count; i++)
					{
						if (Object.ReferenceEquals(wf2.Word[wf2.Word.Count - wf2.Suffix.Count - 1 + i], wf2.Suffix[i]) == false)
						{
							wf2.Suffix = wf2.Word.GetRange(wf2.Word.Count - graphSignsDiff - wf2.Suffix.Count - 1,
														   wf2.Suffix.Count + graphSignsDiff);
							wf2.SuffixPos = new List<int>();
							for (int j = wf2.Word.Count - wf2.Suffix.Count - 1;
								 j < wf2.Word.Count - wf2.Suffix.Count - 1 + wf2.Suffix.Count; j++)
							{
								wf2.SuffixPos.Add(j);
							}
							break;
						}
					}
				}
				else
				{
					/* Stamm wurde geändert */
					for (int i = 0; i < wf2.Suffix.Count; i++)
					{
						if (Object.ReferenceEquals(wf2.Word[wf2.Word.Count - wf2.Suffix.Count - 1 + i], wf2.Suffix[i]) == false)
						{
							wf2.Suffix = wf2.Word.GetRange(wf2.Word.Count - wf2.Suffix.Count - 1, wf2.Suffix.Count);
							wf2.SuffixPos = new List<int>();
							for (int j = wf2.Word.Count - wf2.Suffix.Count - 1;
								 j < wf2.Word.Count - wf2.Suffix.Count - 1 + wf2.Suffix.Count; j++)
							{
								wf2.SuffixPos.Add(j);
							}
							break;
						}
					}

				}
			}
		}

		public List<String> PrintShort()
		{
			List<String> line = new List<String> { "", "\u2502", this.id, this.name };
			return line;
		}

		public List<List<string>> Print(List<Change> lastChanges)
		{

			List<List<string>> prt = new List<List<string>>();
			List<string> line = new List<String>();

			foreach (Change change in lastChanges)
			{

				if (change.PhonBefore[0][0].Symbol == "" && change.PhonAfter.Count == 0 && change.GraphBefore[0][0].Symbol == "" &&
					change.GraphAfter.Count == 0)
				{

				}
				else
				{

					String pBefore = "";
					String gBefore = "";

					for (int i = 0; i < change.PhonBefore.Count; i++)
					{
						pBefore += Functions.ConvertSignListToIPA(change.PhonBefore[i]);
						if (i != change.PhonBefore.Count - 1)
						{
							pBefore += ",";
						}
					}

					for (int i = 0; i < change.GraphBefore.Count; i++)
					{
						gBefore += Functions.ConvertSignListToIPA(change.GraphBefore[i]);
						if (i != change.GraphBefore.Count - 1)
						{
							gBefore += ",";
						}
					}

					line = new List<String> {
						"",
						"\u2502",
						"[" + pBefore + "] > [" + Functions.ConvertSignListToIPA (change.PhonAfter) + "]" + " ; "
						+ "\u27e8" + gBefore + "\u27e9 > \u27e8" + Functions.ConvertSignListToGraph (change.GraphAfter) + "\u27e9",
						""
					};

					prt.Add(line);
				}

				foreach (KeyValuePair<String, String> kvp in change.OtherChanges)
				{
					line = new List<String> { "", "\u2502", "> " + kvp.Key + ": ", kvp.Value };
					prt.Add(line);
				}
			}


			line = new List<String> { "", "\u2502", "[", "" };
			prt.Add(line);

			foreach (KeyValuePair<String, String> kvp in this.additional)
			{
				line = new List<String> { "", "\u2502", kvp.Key + ": ", kvp.Value };
				prt.Add(line);
			}

			line = new List<String> { "", "\u2502", "]", "" };
			prt.Add(line);

			line = new List<String> { "", "\u2502", "{", "" };
			prt.Add(line);

			line = new List<String> { "", "\u2502", "Zeit:", this.startDate + "\u2013" + this.endDate };
			prt.Add(line);

			if ((this.earlier != null) && (this.earlier.Count != 0))
			{
				line = new List<String> { "", "\u2502", "Früher:", String.Join(", ", this.earlier.ToArray()) };
				prt.Add(line);
			}

			if ((this.specialCases != null) && (this.specialCases.Count != 0))
			{
				line = new List<String> { "", "\u2502", "Spezialfall:", String.Join(", ", this.specialCases.ToArray()) };
				prt.Add(line);
			}

			if (this.andPreCond != null)
			{
				line = new List<String> { "", "\u2502", "Und-Vorbedingung:", String.Join(", ", this.andPreCond.ToArray()) };
				prt.Add(line);
			}

			if (this.orPreCond != null)
			{
				line = new List<String> { "", "\u2502", "Oder-Vorbedingung:", String.Join(", ", this.orPreCond.ToArray()) };
				prt.Add(line);
			}

			if (this.leftStr != "")
			{
				line = new List<String> { "", "\u2502", "Linker Kontext:", this.leftStr };
				prt.Add(line);
			}

			if (this.rightStr != "")
			{
				line = new List<String> { "", "\u2502", "Rechter Kontext:", this.rightStr };
				prt.Add(line);
			}

			if (this.stressPatterns != null)
			{
				String str = "";
				foreach (StressPattern sp in this.stressPatterns)
				{
					str += sp.Print();
				}
				line = new List<String> { "", "\u2502", "Betonung:", str };
				prt.Add(line);
			}

			if (!String.IsNullOrEmpty(this.syllType))
			{
				line = new List<String> { "", "\u2502", "Silbe:", this.syllType };
				prt.Add(line);
			}

			if (!String.IsNullOrEmpty(this.cat))
			{
				line = new List<String> { "", "\u2502", "Gram. Kat.:", this.cat };
				prt.Add(line);
			}

			if (!String.IsNullOrEmpty(this.tbl))
			{
				line = new List<String> { "", "\u2502", "Tabelle:", this.tbl };
				prt.Add(line);
			}

			if (this.addConditions != null && this.addConditions.Count != 0)
			{
				foreach (KeyValuePair<String, String> kvp in this.addConditions)
				{
					line = new List<String> { "", "\u2502", kvp.Key + ": ", kvp.Value };
					prt.Add(line);
				}
			}

			line = new List<String> { "", "\u2502", "}", "" };
			prt.Add(line);

			return prt;
		}
	}
}

