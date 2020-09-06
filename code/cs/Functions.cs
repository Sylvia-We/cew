using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Gtk;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace frz
{
	public static class Functions
	{

		/// <summary>
		/// Variablendeklarationen
		/// </summary>
		public static MainWindow win_ = null;

		public static String mode = "lautentwicklung";

		/* alle Verzeichnisse, Textdateien etc.*/
		public static string debugDir = Path.Combine("CEW", "Debug");
		public static string debugWin = Path.Combine("CEW", "Debug", "win.txt");
		public static String debugTextWin = "";

		public static string projectFile = Path.Combine("CEW", "cew.project");
		public static string debugFileRheinfelder = Path.Combine("CEW", "Debug", "rheinfelder.debug");
		public static string logoFile = System.IO.Path.Combine("CEW", "logo_a2.png");
		public static string testFilesFile = System.IO.Path.Combine("CEW", "Testlisten");
		public static string evaluationFile = System.IO.Path.Combine("CEW", "Auswertung", "auswertung.txt");
		public static string outputFile = System.IO.Path.Combine("CEW");


		public static string detail = "";
		public static string nameLang1 = "";
		public static string nameLang2 = "";
		public static int timeLang1 = 0;
		public static int timeLang2 = 0;
		/* Listen mit allen Tabellen (Lexikon, Flexion, Allo, finalCat) */
		public static List<AlloTbl> alloListLang1 = new List<AlloTbl>();
		public static List<AlloTbl> alloListLang2 = new List<AlloTbl>();
		public static List<FlexTbl> flexListLang1 = new List<FlexTbl>();
		public static List<FlexTbl> flexListLang2 = new List<FlexTbl>();
		public static List<LexTbl> lexListLang1 = new List<LexTbl>();
		public static List<LexTbl> lexListLang2 = new List<LexTbl>();
		public static List<String> finalCatListLang1 = new List<String>();
		public static List<String> finalCatListLang2 = new List<String>();
		public static Dictionary<String, List<String>> newCatDict = new Dictionary<String, List<String>>();
		public static Dictionary<Tuple<String, String>, List<KeyValuePair<String, String>>> matchCatDict =
			new Dictionary<Tuple<String, String>, List<KeyValuePair<String, String>>>();
		/* Dictionary<Tuple<Wortart vorher, Wortart danach>, 
			* List<KeyValuePait<Kategorie vorher, Kategorie danach >>> */
		public static List<GraphPhon> TransListLang1 = new List<GraphPhon>();
		public static List<GraphPhon> TransListLang2 = new List<GraphPhon>();
		public static List<Variable> varListLang1 = new List<Variable>();
		public static List<SyllableRuleList> syllRuleList = new List<SyllableRuleList>();
		public static List<AccentRule> accListLang1 = new List<AccentRule>();
		public static List<SoundChangeRule> rulListLang1 = new List<SoundChangeRule>();
		public static Dictionary<String, String> phonOutListLang1 = new Dictionary<String, String>();
		public static Dictionary<String, String> phonOutDiaListLang1 = new Dictionary<String, String>();
		public static int longestPhon = 0;
		public static int longestGraph = 0;
		public static Dictionary<String, String> graphOutListLang1 = new Dictionary<String, String>();
		public static Dictionary<String, String> graphOutDiaListLang1 = new Dictionary<String, String>();
		public static Trie stemTrieLang1 = new Trie();
		public static Trie stemTrieLang2 = new Trie();
		public static Trie affixTrieLang1 = new Trie();
		public static Trie affixTrieLang2 = new Trie();
		public static Trie suffixTrieLang1 = new Trie();
		public static Trie suffixTrieLang2 = new Trie();
		public static bool diacriticInput = true;
		/* Debug */
		public static StringBuilder debugText = new StringBuilder();
		public static int tabNum = 0;
		public static Stopwatch allWatch = new Stopwatch();
		public static TimeSpan allTs = new TimeSpan();
		public static String timeText = "";
		public static List<String> nuclei = new List<String>();
		public static List<Wordform> multiChangePaths = new List<Wordform>();
		public static Wordform reflex;

		public static void SetMW(MainWindow value)
		{
			win_ = value;
		}

		/// <summary>
		/// Vorverarbeitung:
		/// - ruft Einlesen der Projektdatei auf
		/// - ruft Allomorphgenerierung auf
		/// </summary>
		public static void Preprocess()
		{
			/* Debug: 
			 * Leeren der Debug-Dateien 
			 * foreach (string f in Directory.GetFiles(debugDir))
			 * {
			 * System.IO.File.WriteAllText(f, string.Empty);
			 * }
			 */

			/* Einlesen der Projektdatei */
			ReadProjectFile(projectFile);

			/* Allomorphgenerierung*/
			GenerateAllomorphs();
		}

		/// <summary>
		/// Projektdatei einlesen
		/// und Einlesen der restlichen Dateien starten
		/// </summary>
		/// <param name="file">File.</param>
		public static void ReadProjectFile(string file)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";
			/* Dictionary: Tupel (Parametertyp, Sprache), Pfad */
			Dictionary<Tuple<string, string>, string> typeLangPath = new Dictionary<Tuple<string, string>, string>();

			while (true)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{

					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{

						string[] langFuncPath = sLine.Split(':');
						string langFromFile = langFuncPath[0].Trim();
						string typeFromFile = langFuncPath[1].Trim();
						string path = langFuncPath[2].Trim();


						if (typeFromFile.Equals("name"))
						{
							if (langFromFile.Equals("lang1"))
							{
								nameLang1 = path.Trim('"');
							}
							else if (langFromFile.Equals("lang2"))
							{
								nameLang2 = path.Trim('"');
							}
						}
						else if (typeFromFile.Equals("time"))
						{
							if (langFromFile.Equals("lang1"))
							{
								timeLang1 = Int32.Parse(path.Trim('"'));
							}
							else if (langFromFile.Equals("lang2"))
							{
								timeLang2 = Int32.Parse(path.Trim('"'));
							}
						}
						else
						{
							/* Path */
							String[] fullPath = path.Split('/');
							String pathToFile = Path.Combine(fullPath);

							/* Tupel: Parametertyp, Sprache */
							Tuple<string, string> typeLang = new Tuple<string, string>(typeFromFile, langFromFile);
							typeLangPath.Add(typeLang, pathToFile);
						}
					}
				}
				else
				{
					break;
				}
			}

			objReader.Close();

			/* .graph zuerst einlesen, um Output-Text-Zuordnung zur Verfügung zu haben  */
			Tuple<string, string> graphLang1 = new Tuple<string, string>("graph", "lang1");
			if (typeLangPath.ContainsKey(graphLang1))
			{
				foreach (string f in Directory.GetFiles(typeLangPath[graphLang1], "*.graph"))
				{
					ReadFiles.ReadGraph(f, "lang1");
				}
			}

			Tuple<string, string> phonLang1 = new Tuple<string, string>("phon", "lang1");
			if (typeLangPath.ContainsKey(phonLang1))
			{
				foreach (string f in Directory.GetFiles(typeLangPath[phonLang1], "*.phon"))
				{
					ReadFiles.ReadPhon(f, "lang1");
				}
			}


			/* .graph zuerst einlesen, um Output-Text-Zuordnung zur Verfügung zu haben  */
			Tuple<string, string> graphLang2 = new Tuple<string, string>("graph", "lang2");
			if (typeLangPath.ContainsKey(graphLang2))
			{
				foreach (string f in Directory.GetFiles(typeLangPath[graphLang2], "*.graph"))
				{
					ReadFiles.ReadGraph(f, "lang2");
				}
			}

			Tuple<string, string> phonLang2 = new Tuple<string, string>("phon", "lang2");
			if (typeLangPath.ContainsKey(phonLang2))
			{
				foreach (string f in Directory.GetFiles(typeLangPath[phonLang2], "*.phon"))
				{
					ReadFiles.ReadPhon(f, "lang2");
				}
			}


			foreach (KeyValuePair<Tuple<string, string>, string> kvp in typeLangPath)
			{
				if (!kvp.Key.Equals(graphLang1) && !kvp.Key.Equals(phonLang1) && !kvp.Key.Equals(graphLang2) && !kvp.Key.Equals(phonLang2))
				{
					foreach (string f in Directory.GetFiles(kvp.Value, "*." + kvp.Key.Item1))
					{
						switch (kvp.Key.Item1)
						{
							case "allo":
								ReadFiles.ReadAllo(f, kvp.Key.Item2);
								break;
							case "flex":
								ReadFiles.ReadFlex(f, kvp.Key.Item2);
								break;
							case "lex":
								ReadFiles.ReadLex(f, kvp.Key.Item2);
								break;
							case "cat":
								ReadFiles.ReadCat(f, kvp.Key.Item2);
								break;
							case "suffix":
								ReadFiles.ReadSuffix(f, kvp.Key.Item2);
								break;
							case "affix":
								ReadFiles.ReadAffix(f, kvp.Key.Item2);
								break;
							case "trans":
								ReadFiles.ReadTrans(f, kvp.Key.Item2);
								break;
							case "acc":
								ReadFiles.ReadAcc(f, kvp.Key.Item2);
								break;
							case "var":
								ReadFiles.ReadVar(f, kvp.Key.Item2);
								break;
							case "syll":
								ReadFiles.ReadSyll(f, kvp.Key.Item2);
								break;
							case "rul":
								ReadFiles.ReadRul(f, kvp.Key.Item2);
								break;
							case "ncat":
								ReadFiles.ReadNCat(f, kvp.Key.Item2);
								break;
							case "match":
								ReadFiles.ReadMatch(f, kvp.Key.Item2);
								break;
							default:
								Functions.ShowErrorMessage("Dateierweiterung ist unbekannt: " + kvp.Key.Item1);
								break;

						}

					}
				}
			}

		}

		/// <summary>
		/// Allomorphe generieren
		/// </summary>
		public static void GenerateAllomorphs()
		{

			string warningStr = "";

			/* Lang1 */
			foreach (AlloTbl atbl in Functions.alloListLang1)
			{


				if (Functions.GetLexTable(lexListLang1, atbl.Tbl) == null)
				{
					warningStr += "Lexikontabelle " + atbl.Tbl + " fehlt" + Environment.NewLine;
				}
				else
				{
					LexTbl aktTb = new LexTbl(Functions.GetLexTable(lexListLang1, atbl.Tbl));


					Dictionary<string, string> additional = aktTb.Additional;
					additional.AddRange(atbl.Additional);

					/* für jeden Lexikoneintrag: Musterabgleich mit Allo-Regeln und ersetzen*/
					foreach (AlloRule ar in atbl.ArList)
					{
						foreach (List<Sign> entry in aktTb.Entries)
						{

							ar.PatBefore.Trim();
							Match m = Regex.Match(ConvertSignListToGraphText(entry), ar.PatBefore); /* Lemma und pat_before abgleichen*/

							string allomorph = ar.PatAfter;

							if (m.Success)
							{
								/* in pat_after $1, $2... durch die Werte in runden Klammern (aus Abgleich mit Lemma) ersetzen*/
								for (int i = 1; i <= m.Groups.Count; i++)
								{
									string allopart = m.Groups[i].Value;
									allomorph = allomorph.Replace("$" + i, allopart);
								}
							}
							stemTrieLang1.Insert(ConvertStringToSignList(allomorph), "", atbl, entry, ar.Cats, additional, aktTb.Pos);

						}
					}
				}
			}

			/* Lang2 */
			foreach (AlloTbl atbl in Functions.alloListLang2)
			{


				if (Functions.GetLexTable(lexListLang2, atbl.Tbl) == null)
				{
					warningStr += "Lexikontabelle " + atbl.Tbl + " fehlt" + Environment.NewLine;
				}
				else
				{
					LexTbl aktTb = new LexTbl(Functions.GetLexTable(lexListLang2, atbl.Tbl));


					Dictionary<string, string> additional = aktTb.Additional;
					additional.AddRange(atbl.Additional);

					/* für jeden Lexikoneintrag: Musterabgleich mit Allo-Regeln und ersetzen*/
					foreach (AlloRule ar in atbl.ArList)
					{
						foreach (List<Sign> entry in aktTb.Entries)
						{

							ar.PatBefore.Trim();
							Match m = Regex.Match(ConvertSignListToGraphText(entry), ar.PatBefore); /* Lemma und pat_before abgleichen*/

							string allomorph = ar.PatAfter;

							if (m.Success)
							{
								/* in pat_after $1, $2... durch die Werte in runden Klammern (aus Abgleich mit Lemma) ersetzen*/
								for (int i = 1; i <= m.Groups.Count; i++)
								{
									string allopart = m.Groups[i].Value;
									allomorph = allomorph.Replace("$" + i, allopart);
								}
							}
							stemTrieLang2.Insert(ConvertStringToSignList(allomorph), "", atbl, entry, ar.Cats, additional, aktTb.Pos);

						}
					}
				}
			}

			if (warningStr != "")
			{
				ShowWarningMessage(warningStr);
			}


		}

		/// <summary>
		/// Startet für das Eingabewort die Wortformerkennung.
		/// Falls vom Benutzer keine Quantitäten eingegeben werden, 
		/// wird die Wortformerkennung für alle Möglichkeiten gestartet.
		/// </summary>
		public static bool PrepareForRecognition(string word, ref int numWellfLang1, ref int numWellfLang2, ref int numGrade1,
												 ref int numGrade2, String comment)
		{

			word = Functions.PrepareInput(word);
			Functions.printDebugText(Environment.NewLine + "---> #" + word + "# <---" + Environment.NewLine, true);
			int grade = 0;

			if (word == null)
				return false;
			Wordform input = new Wordform(word);
			List<Wordform> allWordForms = new List<Wordform>();

			bool found_one = false;

			/* Wortformerkennung*/

			List<Wordform> wfList = Functions.RecognizeWord(input, "lang1");
			allWordForms.AddRange(wfList);

			if (wfList.Count != 0)
			{
				found_one = true;
				int nAmbig = 0;
				numWellfLang1++;

				foreach (Wordform wfDev in wfList)
				{
					if (wfList.Count > 1)
					{
						nAmbig++;
						win_.AddToTextView(new List<string> { "", nAmbig.ToString() + ".", "", comment }, "Honeydew");
					}

					win_.AddToTextView(wfDev, false, comment, "Honeydew", "\u25b6", "lang1");
					bool otherChanges = false;

					/* Regelliste */

					List<SoundChangeRule> sortedRulList = Functions.rulListLang1.OrderBy(o => o.StartDate).ToList();
					sortedRulList.RemoveAll(x => x.EndDate < wfDev.Time);

					wfDev.ChangeSound(win_, 0, 0, true, otherChanges, sortedRulList);

					/* Attribute aus Reflex löschen */
					Wordform wfDevWithoutAtt = new Wordform(ConvertSignListToGraph(reflex.Word).Remove(reflex.Word.Count - 1, 1).Remove(0, 1));

					int matchLevel = 0;

					/* WFR lang2 */
					List<Wordform> wfListLang2 = Functions.RecognizeWord(wfDevWithoutAtt, "lang2");

					/* Übereinstimmungsgrad Stufe 1: graphische Formen sind gleich */
					if (wfListLang2.Count != 0)
					{
						matchLevel = 1;
						if (grade < 1)
							grade = 1;
						int nAmbigLang2 = 0;

						/* Phon lang2 */
						foreach (Wordform wfModern in wfListLang2)
						{
							wfModern.Phonetic = TransformGraphToPhon(wfModern, 0, 0, "lang2");

							if (IsEqual(reflex.Phonetic, wfModern.Phonetic) == true)
							{
								/* Übereinstimmungsgrad Stufe 2: phonologische Formen sind gleich */
								matchLevel = 2;

								if (grade < 2)
									grade = 2;
							}

						}

						if (matchLevel == 2)
						{
							/* Match Etymon und Reflex */
							List<Wordform> wfListLang2Matched = MatchEtymonReflex(reflex, wfListLang2);

							if (wfListLang2Matched.Count != 0)
							{

								matchLevel = 3;

								if (grade < 3)
									grade = 3;

								win_.AddToTextView(new List<string> {
								"",
								"",
								"Übereinstimmungsgrad 3",
								""
								}, "#BDFFBD");

								foreach (Wordform wfLang2 in wfListLang2Matched)
								{
									if (wfListLang2Matched.Count > 1)
									{
										nAmbigLang2++;
										win_.AddToTextView(new List<string> { "", nAmbigLang2.ToString() + ".", "", comment }, "#BDFFBD");
									}

									String color = "#BDFFBD";
									win_.AddToTextView(wfLang2, false, comment, color, "= ", "lang2");
								}
							}
							else
							{
								win_.AddToTextView(new List<string> {
								"",
								"",
								"Übereinstimmungsgrad 2" + Environment.NewLine +
								"Moderne Wortform(en) mit gleicher Graphie und Phonie:",
								""
							}, "Lavender Blush");

								foreach (Wordform wfLang2 in wfListLang2)
								{
									if (wfListLang2.Count > 1)
									{
										nAmbigLang2++;
										win_.AddToTextView(new List<string> { "", nAmbigLang2.ToString() + ".", "", comment }, "Honeydew");
									}


									String color = "Honeydew";
									win_.AddToTextView(wfLang2, true, comment, color, "\u225f", "lang2");
								}
							}
						}
						else
						{
							win_.AddToTextView(new List<string> {
								"",
								"",
								"Übereinstimmungsgrad 1" + Environment.NewLine +
								"Moderne Wortform(en) mit gleicher Graphie:",
								""
							}, "Lavender Blush");

							foreach (Wordform wfLang2 in wfListLang2)
							{
								if (wfListLang2.Count > 1)
								{
									nAmbigLang2++;
									win_.AddToTextView(new List<string> { "", nAmbigLang2.ToString() + ".", "", comment }, "Honeydew");
								}


								String color = "Honeydew";
								win_.AddToTextView(wfLang2, true, comment, color, "\u225f", "lang2");
							}
						}
					}
					else
					{
						win_.AddToTextView(new List<string> { "", "", "Keine moderne Entsprechung gefunden!", "" }, "Lavender Blush");
					}

					win_.AddToTextView(new List<String> { "", "", "", "" }, "White");

					/* ------------ MultiChange -------------- */
					List<Wordform> oldMultiChangePaths = Functions.DeepClone(Functions.multiChangePaths);

					for (int i = oldMultiChangePaths.Count - 1; i >= 0; i--)
					{
						matchLevel = 0;
						Wordform wf2 = oldMultiChangePaths[i].CloneWfObjectExtensions();

						sortedRulList = Functions.rulListLang1.OrderBy(o => o.StartDate).ToList();
						sortedRulList.RemoveAll(x => x.EndDate < wf2.Time);

						foreach (String scr in wf2.AppliedRules)
						{
							sortedRulList.RemoveAll(x => x.Id == scr);
						}

						if (wf2.PrintRule != null)
						{
							wf2.PrintSoundChange(wf2.PrintRule, wf2, win_);
							wf2.PrintRule = null;
						}
						wf2.ChangeSound(win_, 0, 0, true, otherChanges, sortedRulList);


						/* Attribute aus Reflex löschen */
						wfDevWithoutAtt = new Wordform(ConvertSignListToGraph(reflex.Word).Remove(reflex.Word.Count - 1, 1).Remove(0, 1));

						/* WFR lang2 */
						List<Wordform> wfListLang2_2 = Functions.RecognizeWord(wfDevWithoutAtt, "lang2");

						if (wfListLang2_2.Count != 0)
						{
							matchLevel = 1;

							if (grade < 1)
								grade = 1;
							int nAmbigLang2 = 0;

							/* Phon lang2 */
							foreach (Wordform wfModernMulti in wfListLang2_2)
							{
								wfModernMulti.Phonetic = TransformGraphToPhon(wfModernMulti, 0, 0, "lang2");

								if (IsEqual(reflex.Phonetic, wfModernMulti.Phonetic) == true)
								{
									/* Übereinstimmungsgrad Stufe 2: phonologische Formen sind gleich */
									matchLevel = 2;

									if (grade < 2)
										grade = 2;
								}
							}

							if (matchLevel == 2)
							{

								/* Match Etymon und Reflex */
								List<Wordform> wfListLang2Matched = MatchEtymonReflex(reflex, wfListLang2_2);

								if (wfListLang2Matched.Count != 0)
								{

									matchLevel = 3;

									if (grade < 3)
										grade = 3;

									win_.AddToTextView(new List<string> {
								"",
								"",
								"Übereinstimmungsgrad 3",
								""
								}, "#BDFFBD");

									foreach (Wordform wfLang2 in wfListLang2Matched)
									{
										if (wfListLang2Matched.Count > 1)
										{
											nAmbigLang2++;
											win_.AddToTextView(new List<string> { "", nAmbigLang2.ToString() + ".", "", comment }, "#BDFFBD");
										}


										String color = "#BDFFBD";
										win_.AddToTextView(wfLang2, false, comment, color, "= ", "lang2");
									}
								}
								else
								{
									win_.AddToTextView(new List<string> {
								"",
								"",
								"Übereinstimmungsgrad 2" + Environment.NewLine +
								"Moderne Wortform(en) mit gleicher Graphie und Phonie:",
								""
							}, "Lavender Blush");

									foreach (Wordform wfLang2 in wfListLang2_2)
									{
										if (wfListLang2_2.Count > 1)
										{
											nAmbigLang2++;
											win_.AddToTextView(new List<string> { "", nAmbigLang2.ToString() + ".", "", comment }, "Honeydew");
										}


										String color = "Honeydew";
										win_.AddToTextView(wfLang2, true, comment, color, "\u225f", "lang2");
									}
								}
							}
							else
							{
								win_.AddToTextView(new List<string> {
								"",
								"",
								"Übereinstimmungsgrad 1" + Environment.NewLine +
								"Moderne Wortform(en) mit gleicher Graphie:",
								""
							}, "Lavender Blush");

								foreach (Wordform wfLang2 in wfListLang2_2)
								{
									if (wfListLang2_2.Count > 1)
									{
										nAmbigLang2++;
										win_.AddToTextView(new List<string> { "", nAmbigLang2.ToString() + ".", "", comment }, "Honeydew");
									}


									String color = "Honeydew";
									win_.AddToTextView(wfLang2, true, comment, color, "\u225f", "lang2");
								}
							}
						}
						else
						{
							win_.AddToTextView(new List<string> { "", "", "Keine moderne Entsprechung gefunden!", "" }, "Lavender Blush");
						}

						win_.AddToTextView(new List<String> { "", "", "", "" }, "White");

						Functions.multiChangePaths.RemoveAt(i);
					}
				}
			}

			/* Ausgabe */
			if (grade == 1)
			{
				numGrade1++;
			}
			else if (grade == 2)
			{
				numGrade1++;
				numGrade2++;
			}
			else if (grade == 3)
			{
				numGrade1++;
				numGrade2++;
				numWellfLang2++;
			}

			if (found_one == false)
			{

				/* alle Leerzeichen entfernen */

				string output_not_found = input.PrintWord("GraphOutputWord");
				win_.AddToTextView(new List<string> { "\u25b6", output_not_found, "nicht erkannt", comment }, "Lavender Blush");
				win_.AddToTextView(new List<string> { "", "", "", "" }, "White");

			}

			/* Debug
             * File.WriteAllText(Functions.debugWin, Functions.debugTextWin);
             * File.WriteAllText(debugFileRheinfelder, Functions.debugText.ToString());
             */

			return true;
		}

		/// <summary>
		/// Wortform erkennen:
		/// durchläuft das Wort und sucht nach einem passenden Allomporh
		/// wurde ein Allomorph gefunden, dann wird die restliche Zeichenkette geprüft:
		/// ob ein Infix und ein Suffix oder nur ein Suffix folgen
		/// </summary>
		public static List<Wordform> RecognizeWord(Wordform word, string lang)
		{

			List<Wordform> wfList = new List<Wordform>();
			Trie stemTrie = new Trie();
			Trie suffixTrie = new Trie();
			Trie affixTrie = new Trie();

			if (lang == "lang2")
			{
				stemTrie = stemTrieLang2;
				affixTrie = affixTrieLang2;
				suffixTrie = suffixTrieLang2;
			}
			else if (lang == "lang1")
			{
				stemTrie = stemTrieLang1;
				affixTrie = affixTrieLang1;
				suffixTrie = suffixTrieLang1;
			}

			Allomorph possAllo = new Allomorph();

			/* jeden einzelnen Buchstaben durchlaufen*/
			for (int i = 0; i < word.Word.Count; i++)
			{
				possAllo.Allo.Add(ObjectExtensions.Copy(word.Word[i]));

				/* überprüfen, ob die Buchstabenfolge als Allomorph existiert*/
				List<Allomorph> amList = stemTrie.Search(possAllo.Allo);
				if (amList != null)
				{
					List<Sign> suffix = word.Word.GetRange(i + 1, word.Word.Count - (i + 1));
					List<int> suffixPos = new List<int>();
					for (int j = i + 1; j < i + 1 + word.Word.Count - (i + 1); j++)
					{
						suffixPos.Add(j);
					}

					/* Wort = Stamm (Zeichenkette ist komplett eingelesen)*/
					if (i == word.Word.Count - 1)
					{

						List<Wordform> wfListPart = new List<Wordform>();

						foreach (Allomorph am in amList)
						{


							wfListPart.AddRange(Wellformed(word, am, null, null, lang, null));
						}

						wfList.AddRange(wfListPart);
					}

					/* Wort = Allo + Suffix (restliche Buchstabenfolge ist ein Suffix)*/
					if (suffixTrie.Search(suffix) != null)
					{

						List<Wordform> wfListPart = new List<Wordform>();

						foreach (Allomorph am in amList)
						{
							wfListPart.AddRange(Wellformed(word, am, null, suffix, lang, suffixPos));
						}

						wfList.AddRange(wfListPart);
						wfListPart = new List<Wordform>();
					}

					/* Wort = Allo + Infixe + Suffix (Zeichenkette ist noch nicht komplett eingelesen)*/
					if (i != word.Word.Count - 1)
					{
						List<List<Sign>> affixList = new List<List<Sign>>();
						RecognizeWordWithAffix(word, wfList, amList, suffix, affixTrie, suffixTrie, affixList, lang);

					}
				}
			}

			List<Wordform> removeList = new List<Wordform>();

			foreach (Wordform wortf in wfList)
			{
				if (wortf.Cat == null)
				{
					removeList.Add(wortf);
				}
			}

			foreach (Wordform wortf in removeList)
			{
				wfList.Remove(wortf);
			}

			return wfList;

		}

		/// <summary>
		/// Rekursive Funktion für Wörter mit nichtfinalen Affixen.
		/// </summary>

		public static void RecognizeWordWithAffix(Wordform word, List<Wordform> wfList, List<Allomorph> amList, List<Sign> suffix, Trie affixTrie, Trie suffixTrie,
												   List<List<Sign>> affixList, string lang)
		{
			List<Sign> affix = new List<Sign>();

			/* erstes Infix finden*/
			for (int x = 0; x < suffix.Count - 1; x++)
			{

				List<List<Sign>> affixListCopy = new List<List<Sign>>(affixList);
				affixListCopy.Add(affix);

				affix.Add(suffix[x]);

				if (affixTrie.Search(affix) != null)
				{

					List<Sign> suffix2 = new List<Sign>(suffix.GetRange(x + 1, suffix.Count - (x + 1)));
					List<int> suffixPos = new List<int>();
					for (int j = x + 1; j < x + 1 + suffix.Count - (x + 1); j++)
					{
						suffixPos.Add(j);
					}

					if (suffixTrie.Search(suffix2) != null)
					{
						/* Abbruchbedingung */

						List<Wordform> wfListPart = new List<Wordform>();

						foreach (Allomorph am in amList)
						{
							wfListPart.AddRange(Wellformed(word, am, affixListCopy, suffix2, lang, suffixPos));
						}

						wfList.AddRange(wfListPart);
						wfListPart = new List<Wordform>();
					}
					RecognizeWordWithAffix(word, wfList, amList, suffix2, affixTrie, suffixTrie, affixListCopy, lang);
				}
			}
		}

		/// <summary>
		/// Prüft, ob die Kombination zulässig ist.
		/// </summary>
		public static List<Wordform> Wellformed(Wordform word, Allomorph am, List<List<Sign>> affixList, List<Sign> suffix,
												string lang, List<int> suffixPos)
		{

			List<Wordform> wfList1Allo = new List<Wordform>();
			List<FlexTbl> ftbls = new List<FlexTbl>();
			List<String> finalCat = new List<String>();
			int time = 0;

			if (lang == "lang1")
			{

				ftbls = Functions.GetFlexTable(flexListLang1, am.Tbl);
				finalCat = Functions.finalCatListLang1;
				time = timeLang1;

			}
			else if (lang == "lang2")
			{

				ftbls = Functions.GetFlexTable(flexListLang2, am.Tbl);
				finalCat = Functions.finalCatListLang2;
				time = timeLang2;
			}

			/* Allo*/
			if ((affixList == null) && (suffix == null))
			{
				/* [
				 * L_coin
				 * cat: f Sg
				 * pos: noun
				 * ]
				 */

				foreach (string cat in am.Cats)
				{
					if (finalCat.Contains(cat))
					{

						Allomorph amCopy = DeepClone(am);
						Wordform wordCopy = new Wordform(word);
						wordCopy.AddAttributes(amCopy.Lemma, cat, amCopy.Tbl, lang, time, amCopy.Additional, win_, amCopy.Allo,
											   null, ref suffix, amCopy.Pos, null);
						wfList1Allo.Add(wordCopy);
					}
				}

				/* Allo + Suffix*/
			}
			else if ((affixList == null) && (suffix != null))
			{
				/*	[
				*	F_femme
				* 	]
				*	{f Sg} s -> {f Pl}
				*/

				foreach (FlexTbl ftbl in ftbls)
				{
					foreach (FlexRule fr in ftbl.Frlist)
					{

						string am_cat = "";
						foreach (string s in am.Cats)
						{
							am_cat += s + " / ";
						}

						if ((IsEqual(fr.Affix, suffix)) && (am.CatsEqual(fr.CatBefore)) && (finalCat.Contains(fr.CatAfter)))
						{

							Allomorph amCopy = DeepClone(am);
							Wordform wordCopy = new Wordform(word);
							List<Sign> suffixCopy = wordCopy.Word.GetRange(wordCopy.Word.Count - suffix.Count, suffix.Count);
							wordCopy.AddAttributes(amCopy.Lemma, fr.CatAfter, amCopy.Tbl, lang, time, amCopy.Additional, win_, amCopy.Allo, null,
												   ref suffixCopy, amCopy.Pos, suffixPos);

							wordCopy.Word[amCopy.Allo.Count].SegmEnd = true;
							wordCopy.Word[amCopy.Allo.Count + 1].SegmStart = true;
							wordCopy.Additional.AddRange(ftbl.Additional);

							wfList1Allo.Add(wordCopy);
						}
					}
				}

			}
			else
			{
				/* [
				 * F_joli
				 * ]
				 * {m Sg} e -> {f Sg}
				 * {m Sg} s -> {m Pl}
				 * {f Sg} s -> {f Pl}
				 */

				foreach (FlexTbl ftbl in ftbls)
				{
					foreach (FlexRule fr in ftbl.Frlist)
					{

						/* Kategorie Allomorph und Anfangskategorie FlexPattern (Infix1) abgleichen,
						 * Affix der FlexRule = Affix aus Liste
						 */

						if ((Array.Exists(am.Cats, element => element == fr.CatBefore)) && (Functions.IsEqual(fr.Affix, affixList[0])))
						{

							/* Zähler für Rekursion,
							 * Position für Segmentierung */
							int count = 0;
							int position = am.Allo.Count + affixList[0].Count;

							/* Wordform kopieren, falls FlexRule in Sackgasse führt */
							Wordform wordCopy = new Wordform(word);
							List<Sign> suffixCopy = wordCopy.Word.GetRange(wordCopy.Word.Count - suffix.Count, suffix.Count);
							wordCopy.Word[am.Allo.Count - 1].SegmEnd = true;
							wordCopy.Word[am.Allo.Count].SegmStart = true;
							wordCopy.Word[position - 1].SegmEnd = true;
							wordCopy.Word[position].SegmStart = true;

							WellformedWithAffix(wordCopy, am, ftbls, fr.CatAfter, affixList, suffixCopy,
												finalCat, lang, count, position, wfList1Allo, time, suffixPos);
						}
						/* falls nicht: weitersuchen */
					}
				}
			}
			return wfList1Allo;
		}

		/// <summary>
		/// Rekursive Funktion zum Überprüfen, ob Wortformen mit mindestens einem nichtfinalen Affix wohlgeformt sind.
		/// </summary>
		public static void WellformedWithAffix(Wordform wordCopy, Allomorph am, List<FlexTbl> ftbls, string catAfter,
												List<List<Sign>> affixList, List<Sign> suffix, List<String> finalCat,
												string lang, int count, int position, List<Wordform> wfList1Allo, int time,
											  List<int> suffixPos)
		{

			foreach (FlexTbl ftbl in ftbls)
			{
				foreach (FlexRule fr in ftbl.Frlist)
				{
					Wordform wordCopyX = wordCopy.CloneWfObjectExtensions();

					if (catAfter == fr.CatBefore)
					{

						if ((affixList.Count == count + 1) && (Functions.IsEqual(fr.Affix, suffix)) && finalCat.Contains(fr.CatAfter))
						{
							/* Abbruchbedingung */

							Allomorph amCopy = DeepClone(am);
							wordCopyX.AddAttributes(amCopy.Lemma, fr.CatAfter, amCopy.Tbl, lang, time, amCopy.Additional, win_, amCopy.Allo, affixList,
												   ref suffix, amCopy.Pos, suffixPos);
							wordCopyX.Additional.AddRange(ftbl.Additional);

							wfList1Allo.Add(wordCopyX);

						}
						else if ((affixList.Count > count + 1) && (Functions.IsEqual(fr.Affix, affixList[count + 1])))
						{
							/* aktuelles Affix ist noch nicht das Suffix */
							count++;
							position += affixList[count].Count;
							/* Wordform kopieren, falls FlexRule in Sackgasse führt */
							Wordform wordCopyCopy = new Wordform(wordCopyX);
							wordCopyCopy.Word[position - 1].SegmEnd = true;
							wordCopyCopy.Word[position].SegmStart = true;
							WellformedWithAffix(wordCopyCopy, am, ftbls, fr.CatAfter, affixList, suffix, finalCat, lang, count, position,
												wfList1Allo, time, suffixPos);
						}
						/* falls nicht: weitersuchen */
					}
					/* falls nicht: weitersuchen */
				}
			}
		}

		/// <summary>
		/// Prüfen, ob Bedingungen für Graphem-Phonem-Umwandlung wahr sind.
		/// </summary>
		public static bool CheckConditions(GraphPhon gp, int posInPhon, int posInGraph, List<int?> graphPos, List<Sign> word,
										   List<Sign> startOfPhoneticWord)
		{
			if (gp.ConditionList.Count == 0)
			{ /* keine Bedingungen*/
				return true;
			}
			else
			{ /* Bedingungen*/

				bool allConditions = true;

				foreach (AttributeValue aw in gp.ConditionList)
				{
					if (CheckTransCondition(word, aw.Attribute, aw.Value, posInPhon, posInGraph, graphPos, startOfPhoneticWord,
											gp.Graph.Length) == false)
					{
						allConditions = false;
						return false;
					}
				}
				if (allConditions == true)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Diakritische Sonderzeichen aus Eingabe durch Textsymbole ersetzen
		/// </summary>
		public static string PrepareInput(string stIn)
		{
			String textString = "";

			if (diacriticInput == true)
			{
				/* Input mit Diakritika */

				foreach (char c in stIn)
				{
					var myKey = graphOutListLang1.FirstOrDefault(x => x.Value == c.ToString().Normalize()).Key;
					if (myKey != null)
					{
						/* falls Key gefunden wird */
						textString += myKey;
					}
					else
					{
						ShowWarningMessage("Zeichen " + c.ToString() + " in " + stIn + " wurde nicht als Graphem in .graph gefunden.");
						return null;
					}
				}
			}
			else
			{
				/* Input in Notation der Textdateien */
				textString = stIn;

			}
			return textString;
		}

		/// <summary>
		/// Gibt alle Flex-Tabellen der Liste mit dem vorgegebenen Namen zurück
		/// </summary>
		public static List<FlexTbl> GetFlexTable(List<FlexTbl> ftlist, string tblname)
		{

			List<FlexTbl> ftbls = new List<FlexTbl>();

			foreach (FlexTbl ft in ftlist)
			{

				if (ft.Tbl == tblname)
				{
					ftbls.Add(ft);
				}
			}

			return ftbls;
		}

		/// <summary>
		/// Gibt alle Lex-Tabellen der Liste mit dem vorgegebenen Namen zurück
		/// </summary>
		public static LexTbl GetLexTable(List<LexTbl> lexlist, string tblname)
		{
			foreach (LexTbl el in lexlist)
			{
				if (el.Tbl == tblname)
				{
					return el;
				}
			}
			return null;
		}

		/// <summary>
		/// Sucht Graphem in Liste von Graphem-Phonem-Paaren.
		/// </summary>
		public static List<GraphPhon> SearchGraphInList(List<GraphPhon> gplist, string graph)
		{
			List<GraphPhon> list = gplist.FindAll(item => item.Graph == graph);
			return list;
		}

		/// <summary>
		/// Überprüft die Bedingungen für die Graphem-Phonem-Zuordnung.
		/// z.B. {Linker Kontext: [$V_PHON]; Rechter Kontext: <$V_GRAPH>}
		/// </summary>
		public static bool CheckTransCondition(List<Sign> word, string attribute, string value, int posInPhon, int posInGraph,
											   List<int?> graphPos, List<Sign> startOfPhoneticWord, int graphCount)
		{
			List<string> conditionValuesVariables = new List<string>(value.Split(' '));

			bool allTrue = true;

			/* für alle aufeinanderfolgende Wertvariablen ([$V_PHON] <$LIQUIDA>) */
			/* alle Zweige müssen true sein */
			for (int i = 0; i < conditionValuesVariables.Count; i++)
			{

				int positionShift = conditionValuesVariables.Count - i;

				Match graphOrPhonMatch = Regex.Match(conditionValuesVariables[i], @"^[<\[](.*?)[>\]]$");
				Match graphMatch = Regex.Match(conditionValuesVariables[i], @"<(.*?)>");
				Match phonMatch = Regex.Match(conditionValuesVariables[i], @"\[(.*?)\]");

				if (graphOrPhonMatch.Success)
				{

					List<string> conditionValues = new List<string>();
					Match name = Regex.Match(graphOrPhonMatch.Groups[1].Value, @"^(\$.*)$");

					/* falls Listenelement eine Variable ist (z.B. $V_GRAPH) */
					if (name.Success)
					{
						Variable found = Functions.varListLang1.Find(x => x.Name == name.Groups[1].Value);
						if (found != null)
						{
							conditionValues.AddRange(found.Values);
						}
						else
						{
							ShowErrorMessage("Variable " + name.Groups[1].Value + " aus .trans ist nicht definiert.");
						}
					}
					else
					{
						conditionValues.Add(graphOrPhonMatch.Groups[1].Value);
					}

					if (attribute == "Rechter Kontext")
					{

						if (phonMatch.Success)
						{
							throw new SymbolNotFoundException("Phone sind für den rechten Kontext noch nicht bekannt.");
						}
						else if (graphMatch.Success)
						{

							int shiftedPosition = posInGraph + i + graphCount;

							foreach (string s in conditionValues)
							{

								if (shiftedPosition < word.Count)
								{ /* noch kein Wortende */

									if ((shiftedPosition == word.Count - 1) && (s == "#"))
									{ /* rechter Kontext ist Wortende */
										allTrue = true;
										goto nextConditionValuesVariable;
									}
									else if (word[shiftedPosition].Symbol == s)
									{
										allTrue = true;
										goto nextConditionValuesVariable;
									}
								}
								else if (s == "#")
								{ /* Wortende */
									allTrue = true;
									goto nextConditionValuesVariable;
								}
							}
						}
						else
						{
							throw new SymbolNotFoundException("Bedingungen müssen als Grapheme \"<>\" oder Phone \"[]\" gekennzeichnet sein.");
						}
					}
					else if (attribute == "Linker Kontext")
					{

						foreach (string s in conditionValues)
						{

							if (graphMatch.Success)
							{
								int shiftedPosition = posInGraph - positionShift;

								if (shiftedPosition < 0)
								{ /* Wortanfang */

									if (s == "#")
									{
										allTrue = true;
										goto nextConditionValuesVariable;
									}
									else
									{
										allTrue = false;
										goto nextConditionValuesVariable;
									}
								}
								else
								{

									if (word[shiftedPosition].Symbol == s)
									{
										allTrue = true;
										goto nextConditionValuesVariable;
									}
								}

							}
							else if (phonMatch.Success)
							{
								int shiftedPosition = posInPhon - positionShift;

								if (shiftedPosition < 0)
								{ /* Wortanfang */

									if (s == "#")
									{
										allTrue = true;
										goto nextConditionValuesVariable;
									}
									else
									{
										allTrue = false;
										goto nextConditionValuesVariable;
									}
								}
								else
								{
									if (startOfPhoneticWord[shiftedPosition].Symbol == s)
									{
										allTrue = true;
										goto nextConditionValuesVariable;
									}
								}
							}
						}

						allTrue = false;
						goto nextConditionValuesVariable;

					}
					else
					{
						ShowErrorMessage("Attribut aus .trans nicht gefunden: " + attribute);
					}
				}
				else
				{
					ShowErrorMessage("Wert aus .trans nicht gefunden: " + value);
				}
				return false;

			nextConditionValuesVariable:
				if (allTrue == true)
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			if (allTrue == true)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Fügt ein Dictionary zu einem anderen Dictionary hinzu
		/// und prüft, ob Attribute doppelt definiert sind.
		/// </summary>
		public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
		{

			if (collection != null)
			{
				foreach (var item in collection)
				{
					if (!source.ContainsKey(item.Key))
					{
						source.Add(item.Key, item.Value);
					}
					else
					{
						ShowWarningMessage("Attribut ist doppelt definiert: " + item.Key);
					}
				}
			}
		}

		/// <summary>
		/// Zeigt einen MessageDialog mit einer Warnung an.
		/// </summary>
		public static void ShowWarningMessage(String s)
		{
			s = s.Replace("<", "&lt;");
			s = s.Replace(">", "&gt;");
			MessageDialog msdSame = new MessageDialog(win_, DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, s);
			msdSame.Title = "Warning";
			if ((ResponseType)msdSame.Run() == ResponseType.Close)
			{
				msdSame.Destroy();
			}
		}

		/// <summary>
		/// Zeigt einen MessageDialog mit einem Fehler an.
		/// </summary>
		public static void ShowErrorMessage(String s)
		{
			s = s.Replace("<", "&lt;");
			s = s.Replace(">", "&gt;");
			MessageDialog msdSame = new MessageDialog(win_, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, s);
			msdSame.Title = "Error";
			msdSame.Text = s;
			if ((ResponseType)msdSame.Run() == ResponseType.Close)
			{
				msdSame.Destroy();
				throw new SymbolNotFoundException(s);
			}
		}

		/// <summary>
		/// Konvertiert einen Unicode-String (U+0304) in einen string.
		/// </summary>
		public static String ConvertUnicodeToString(string unicode)
		{
			String codePoint = unicode.ToString().Replace("U+", "").ToString();
			int code = int.Parse(codePoint, System.Globalization.NumberStyles.HexNumber);
			return char.ConvertFromUtf32(code).ToString();
		}

		/// <summary>
		/// Wandelt die Symbole einer List<Sign> in einen String (GraphText) um.
		/// </summary>
		public static string ConvertSignListToGraphText(List<Sign> ll)
		{

			string str = "";
			foreach (Sign l in ll)
			{

				str += l.Symbol;
			}
			return str;
		}

		/// <summary>
		/// Wandelt die Symbole einer List<Sign> in einen String (Graph) um.
		/// </summary>
		public static string ConvertSignListToGraph(List<Sign> ll)
		{

			string str = "";
			foreach (Sign l in ll)
			{

				str += l.PrintGraph();
			}
			return str;
		}

		/// <summary>
		/// Wandelt die Symbole einer List<Sign> in einen String (Graph) um.
		/// </summary>
		public static string ConvertSignListToIPA(List<Sign> ll)
		{

			string str = "";
			foreach (Sign l in ll)
			{

				str += l.PrintIpa();
			}
			return str;
		}

		/// <summary>
		/// Wandelt einen String in eine List<Sign> um.
		/// </summary>
		public static List<Sign> ConvertStringToSignList(String str)
		{
			/* signLength ist längestes Zeichen oder Länge von str */
			int signLength = Math.Min(Math.Max(longestPhon, longestGraph), str.Length);

			List<Sign> ll = new List<Sign>();

			for (int i = 0; i < str.Length; i++)
			{

				/* falls das längste Sign + i größer als String, 
				 * dann signLength auf Länge des Strings - i setzen */
				if (signLength + i > str.Length)
					signLength = str.Length - i;

				for (int j = signLength; j > 0; j--)
				{
					String sign = str.Substring(i, j);
					if (graphOutListLang1.ContainsKey(sign))
					{
						ll.Add(new Sign(ObjectExtensions.Copy(sign)));
						i = i + j - 1;

						break;
					}
					else if (j == 1)
					{
						ll.Add(new Sign(str[i].ToString()));
					}
				}
			}
			return ll;
		}

		public static List<List<Sign>> ConvertStringToSignListList(String str, String phonOrGraph, String type)
		{

			List<List<Sign>> list = new List<List<Sign>>();

			if (type == "before")
			{

				Match VarRegex = Regex.Match(str, @"^(\$.+)$");

				if (VarRegex.Success)
				{
					/* str ist Variable */
					Variable Vars = Functions.varListLang1.Find(x => x.Name == VarRegex.Groups[1].Value);
					if (Vars == null)
						Functions.ShowErrorMessage("Variable " + VarRegex.Groups[1].Value + " ist nicht definiert.");
					foreach (String s1 in Vars.Values)
					{
						if (phonOrGraph == "phon")
						{
							list.Add(Functions.ConvertStringToSignListPhon(s1));
						}
						else if (phonOrGraph == "graph")
						{
							list.Add(Functions.ConvertStringToSignList(s1));
						}
					}
				}
				else
				{
					if (str == "")
					{
						list.Add(new List<Sign> { new Sign("") });
					}
					else if (str == "#")
					{
						list.Add(new List<Sign> { new Sign("#") });
					}
					else
					{
						if (phonOrGraph == "phon")
						{
							list.Add(Functions.ConvertStringToSignListPhon(str));
						}
						else if (phonOrGraph == "graph")
						{
							list.Add(Functions.ConvertStringToSignList(str));
						}
					}
				}
			}
			else if (type == "context")
			{
				/* Mögliche Werte:
				 * s
				 * st
				 * s, r
				 * st, sl
				 * $V
				 * $V $N
				 * $V s
				 * $V, s
				 */

				// 1. verschiedene Werte durch , getrennt
				String[] strings = str.Split(',');

				foreach (String s in strings)
				{
					String sTrim = s.Trim();

					if (sTrim.Contains(" "))
					{
						/* Variablen [und Signs] */
						String[] varsAndSigns = sTrim.Split(' ');
						List<List<List<Sign>>> varsAndSignsList = new List<List<List<Sign>>>();

						foreach (String varOrSign in varsAndSigns)
						{

							Match VarRegex = Regex.Match(varOrSign, @"^(\$.+)$");
							List<List<Sign>> varOrSignList = new List<List<Sign>>();

							if (VarRegex.Success)
							{
								/* Variable */
								Variable Vars = Functions.varListLang1.Find(x => x.Name == VarRegex.Groups[1].Value);
								if (Vars == null)
									ShowErrorMessage("Variable " + VarRegex.Groups[1].Value + " ist nicht definiert.");
								foreach (String s1 in Vars.Values)
								{
									if (phonOrGraph == "phon")
									{
										varOrSignList.Add(Functions.ConvertStringToSignListPhon(s1));
									}
									else if (phonOrGraph == "graph")
									{
										varOrSignList.Add(Functions.ConvertStringToSignList(s1));
									}
								}
							}
							else
							{
								/* Signs */
								if (varOrSign == "#")
								{
									varOrSignList.Add(new List<Sign> { new Sign("#") });
								}
								else
								{
									if (phonOrGraph == "phon")
									{
										varOrSignList.Add(Functions.ConvertStringToSignListPhon(varOrSign));
									}
									else if (phonOrGraph == "graph")
									{
										varOrSignList.Add(Functions.ConvertStringToSignList(varOrSign));
									}
								}
							}
							varsAndSignsList.Add(varOrSignList);
						}

						List<List<Sign>> currList = new List<List<Sign>>();
						List<List<Sign>> cartesianList = varsAndSignsList[0];

						/* Kartesisches Produkt der Listen */
						for (int i = 1; i < varsAndSignsList.Count; i++)
						{
							currList = cartesianList;
							cartesianList = CartesianJoinListLists(currList, varsAndSignsList[i]);
						}

						list = cartesianList;

					}
					else
					{
						/* nur Phone/Grapheme oder nue eine variable (kein Leerzeichen) */
						if (sTrim == "")
						{
							return list;
						}
						else if (sTrim == "#")
						{
							list.Add(new List<Sign> { new Sign("#") });
						}
						else
						{

							Match VarRegex = Regex.Match(sTrim, @"^(\$.+)$");

							if (VarRegex.Success)
							{
								/* Variable */
								Variable Vars = Functions.varListLang1.Find(x => x.Name == VarRegex.Groups[1].Value);
								if (Vars == null)
									ShowErrorMessage("Variable " + VarRegex.Groups[1].Value + " ist nicht definiert.");
								foreach (String s1 in Vars.Values)
								{
									if (phonOrGraph == "phon")
									{
										list.Add(Functions.ConvertStringToSignListPhon(s1));
									}
									else if (phonOrGraph == "graph")
									{
										list.Add(Functions.ConvertStringToSignList(s1));
									}
								}
							}
							else
							{
								/* Signs */
								if (sTrim == "#")
								{
									list.Add(new List<Sign> { new Sign("#") });
								}
								else
								{
									if (phonOrGraph == "phon")
									{
										list.Add(Functions.ConvertStringToSignListPhon(sTrim));
									}
									else if (phonOrGraph == "graph")
									{
										list.Add(Functions.ConvertStringToSignList(sTrim));
									}
								}
							}
						}
					}

				}
			}
			return list;
		}

		/// <summary>
		/// Wandelt einen String in eine List<Sign> von Phonen um.
		/// </summary>
		public static List<Sign> ConvertStringToSignListPhon(String str)
		{
			/* signLength ist längestes Zeichen oder Länge von str */
			int signLength = Math.Min(Math.Max(longestPhon, longestGraph), str.Length);

			List<Sign> ll = new List<Sign>();

			for (int i = 0; i < str.Length; i++)
			{

				/* falls das längste Sign + i größer als String, 
				 * dann signLength auf Länge des Strings - i setzen */
				if (signLength + i > str.Length)
					signLength = str.Length - i;

				for (int j = signLength; j > 0; j--)
				{
					String sign = str.Substring(i, j);
					if (phonOutListLang1.ContainsKey(sign))
					{
						ll.Add(new Sign(ObjectExtensions.Copy(sign)));
						i = i + j - 1;

						break;
					}
					else if (j == 1)
					{
						ll.Add(new Sign(str[i].ToString()));
					}
				}
			}
			return ll;
		}

		/// <summary>
		/// Erzeugt eine tiefe Kopie eines Objekts.
		/// </summary>
		public static T DeepClone<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T)formatter.Deserialize(ms);
			}
		}

		/// <summary>
		/// Prüft, ob zwei Sign identisch sind.
		/// </summary>
		public static bool IsEqual(List<Sign> ll1, List<Sign> ll2)
		{

			bool equal = false;

			if (ll1.Count != ll2.Count)
				return false;

			for (int i = 0; i < ll1.Count; i++)
			{
				if (ll2.Count > i)
				{
					if (ll1[i].Symbol == ll2[i].Symbol)
					{
						equal = true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

			return equal;
		}

		/// <summary>
		/// Transcripts only.
		/// </summary>
		public static bool RecognizeOnly(String word, String lang, ref int numWellf)
		{
			Functions.printDebugText(Environment.NewLine + "---> #" + word + "# <---" + Environment.NewLine, true);

			if (word == null)
				return false;
			Wordform input = new Wordform(word);

			List<Wordform> wfList = Functions.RecognizeWord(input, lang);

			if (wfList.Count != 0)
			{
				int nAmbig = 0;


				foreach (Wordform wfDev in wfList)
				{
					if (wfList.Count > 1)
					{
						nAmbig++;
						win_.AddToTextView(new List<string> { "", nAmbig.ToString() + ".", "", "" }, "Honeydew");
					}

					/* Transkription für Lang2 */
					if (lang == "lang2")
					{
						wfDev.Phonetic = TransformGraphToPhon(wfDev, 0, 0, lang);
						win_.AddToTextView(wfDev, false, "", "Honeydew", "\u25b6", "lang2");
					}
					else
					{
						win_.AddToTextView(wfDev, false, "", "Honeydew", "\u25b6", "lang1");
					}
				}
				if (lang == "lang1")
					numWellf++;
			}
			else
			{
				String output_not_found = input.PrintWord("GraphOutputWord");
				win_.AddToTextView(new List<string> { "\u25b6", output_not_found, "nicht erkannt", "" }, "Lavender Blush");
			}

			return true;
		}

		/// <summary>
		/// Transcripts only.
		/// </summary>
		public static bool TranscriptOnly(String word, String lang)
		{
			Functions.printDebugText(Environment.NewLine + "---> #" + word + "# <---" + Environment.NewLine, true);

			if (word == null)
				return false;
			Wordform input = new Wordform(word);

			/* Wortgrenzen einfügen */
			input.Word.Insert(0, new Sign("#"));
			input.Word.Add(new Sign("#"));

			input.Phonetic = TransformGraphToPhon(input, 0, 0, lang);
			String color = "#FFFFFF";
			win_.AddToTextView(input.PrintShort("", "\u25b6"), color);
			return true;
		}

		/// <summary>
		/// Grapheme in Phone umwandeln
		/// </summary>
		public static List<Sign> TransformGraphToPhon(Wordform wf, int startPhon, int startGraph, String lang)
		{

			List<Sign> phoneticWord = new List<Sign>();
			List<GraphPhon> transList = new List<GraphPhon>();
			if (lang == "lang1")
			{
				transList = Functions.TransListLang1;
			}
			else if (lang == "lang2")
			{
				transList = Functions.TransListLang2;
			}

			int posInPhon = startPhon - 1;
			int SignMappingGraph1 = startGraph - 1;
			int SignMappingPhon1 = startPhon - 1;

			for (int i = startGraph; i < wf.Word.Count; i++)
			{

				int nextGraphI = i;
				int posNextGraphI = i;

				List<GraphPhon> multiGraphInList = new List<GraphPhon>();
				List<GraphPhon> graphInList = new List<GraphPhon>();
				List<int?> SignMappingGraph = new List<int?>();
				List<int?> SignMappingPhon = new List<int?>();
				SignMappingGraph1++;
				SignMappingPhon1++;
				SignMappingGraph.Add(SignMappingGraph1);
				SignMappingPhon.Add(SignMappingPhon1);
				posInPhon++;

				/* Wortgrenzen */
				if (wf.Word[i].Symbol == "#")
				{
					phoneticWord.Add(new Sign("#"));
					SignMapping sm = new SignMapping(SignMappingGraph, SignMappingPhon);
					wf.MappingList.Add(sm);
					continue;
				}

				/* Monographem, auch Diphtonge (da sie in .graph stehen) */
				graphInList = new List<GraphPhon>(Functions.SearchGraphInList(transList, wf.Word[i].Symbol));

				/* Multigraphemeinheiten: Affrikate, Aspiranten...*/
				string sym = wf.Word[i].Symbol;
				String symSave = sym;

				for (int j = wf.Word.Count - 1; j > i + 1; j--)
				{
					sym = symSave;
					foreach (Sign s in wf.Word.GetRange(i + 1, j - i - 1))
					{
						sym += s.Symbol;
					}
					multiGraphInList = new List<GraphPhon>(Functions.SearchGraphInList(transList, sym));

					/* prüfen, ob SegmentationBorder im Multigraphem gefunden wurde */
					if (multiGraphInList.Any())
					{
						for (int k = i; k < j - 1; k++)
						{
							if (wf.Word[k].SegmEnd == true)
							{
								multiGraphInList.Clear();
								break;
							}
						}

						posNextGraphI = j - 1;
						SignMappingGraph.Insert(1, j - 1);
						SignMappingGraph1++;

						/* für alle GraphPhon Konditionen testen
                        * falls Kondition stimmt, dann abbrechen und i+2 */
						for (int k = 0; k < multiGraphInList.Count; k++)
						{
							GraphPhon gp = multiGraphInList[k];

							if (Functions.CheckConditions(gp, posInPhon, i, SignMappingGraph, wf.Word, phoneticWord) == true)
							{

								Match phonGroup = Regex.Match(gp.Phon, @"^(.*?)\.(.*?)$");
								nextGraphI = posNextGraphI;

								if (phonGroup.Success)
								{
									/* Phongruppen*/
									string[] phons = gp.Phon.Split('.');

									for (int l = 0; l < phons.Count(); l++)
									{
										Sign s = ObjectExtensions.Copy(wf.Word[i]);
										s.Symbol = phons[l];
										phoneticWord.Add(s);
										if (l > 0)
										{
											SignMappingPhon.Add(i + l);
											SignMappingPhon1++;
										}
									}
									SignMapping sm = new SignMapping(SignMappingGraph, SignMappingPhon);
									wf.MappingList.Add(sm);
								}
								else
								{
									if (gp.Phon == "")
									{
										SignMapping sm = new SignMapping(SignMappingGraph, new List<int?> { null });
										wf.MappingList.Add(sm);
									}
									else
									{
										Sign l = ObjectExtensions.Copy(wf.Word[i]);
										l.Symbol = gp.Phon;
										phoneticWord.Add(l);
										SignMapping sm = new SignMapping(SignMappingGraph, SignMappingPhon);
										wf.MappingList.Add(sm);
									}
								}
								goto TransformationFound;
							}
						}
					}
				}

				if (graphInList.Any())
				{

					/* für alle GraphPhon Konditionen testen
					 * falls Kondition stimmt, dann abbrechen und i+2
					 */
					for (int k = 0; k < graphInList.Count; k++)
					{
						GraphPhon gp = graphInList[k];

						if (Functions.CheckConditions(gp, posInPhon, i, SignMappingGraph, wf.Word, phoneticWord) == true)
						{

							Match phonGroup = Regex.Match(gp.Phon, @"^(.*?)\.(.*?)$");

							if (phonGroup.Success)
							{
								/* Phongruppen: x - ks */
								string[] phons = gp.Phon.Split('.');

								for (int j = 0; j < phons.Count(); j++)
								{
									Sign l = ObjectExtensions.Copy(wf.Word[i]);
									l.Symbol = phons[j];
									phoneticWord.Add(l);
									if (j > 0)
									{
										SignMappingPhon.Add(i + j);
										SignMappingPhon1++;
									}
								}
								SignMapping sm = new SignMapping(SignMappingGraph, SignMappingPhon);
								wf.MappingList.Add(sm);
							}
							else
							{
								if (gp.Phon == "")
								{
									SignMapping sm = new SignMapping(SignMappingGraph, new List<int?> { null });
									wf.MappingList.Add(sm);
								}
								else
								{
									Sign l = ObjectExtensions.Copy(wf.Word[i]);
									l.Symbol = gp.Phon;
									phoneticWord.Add(l);
									SignMapping sm = new SignMapping(SignMappingGraph, SignMappingPhon);
									wf.MappingList.Add(sm);
								}
							}
							goto TransformationFound;
						}
					}
				}
				else
				{
					Functions.ShowWarningMessage("Graphem in Transkriptionsdatei (.trans) nicht gefunden: " + wf.Word[i].Symbol);
					return null;
				}

			TransformationFound:
				i = nextGraphI;
				continue;
			}
			return phoneticWord;
		}

		public static string SpliceText(String str, int width, String delimiter)
		{
			int signsAfterLastBreak = str.Count();
			int searchStart = 0;
			int index = 0;

			while (signsAfterLastBreak > width)
			{

				Match lastWhite = Regex.Match(str.Substring(searchStart, width), delimiter, RegexOptions.RightToLeft);
				if (lastWhite.Success)
				{
					if (lastWhite.Index == 0)
						break;

					index = lastWhite.Index + searchStart + 1;

					var aStringBuilder = new StringBuilder(str);
					aStringBuilder.Insert(index, Environment.NewLine);
					str = aStringBuilder.ToString();

				}
				else
				{
					break;
				}

				searchStart = index;
				signsAfterLastBreak = str.Count() - index;
			}

			return str;
		}

		public static List<List<Sign>> CartesianJoinListLists(List<List<Sign>> a, List<List<Sign>> b)
		{
			List<List<Sign>> newList = new List<List<Sign>>();

			foreach (List<Sign> lsA in a)
			{
				foreach (List<Sign> lsB in b)
				{
					List<Sign> newLs = new List<Sign>();
					newLs.AddRange(lsA);
					newLs.AddRange(lsB);
					newList.Add(newLs);
				}
			}

			return newList;
		}

		public static void printDebugText(String str, bool tabs)
		{
			if (tabs == true)
			{
				for (int i = 0; i < Functions.tabNum; i++)
				{
					Functions.debugText.Append("\t");
				}
			}
			Functions.debugText.Append(str);
		}

		/// <summary>
		/// Konvertiert die Zahlen 1-10 in römische Ziffern.
		/// </summary>
		public static String ToRoman(int num)
		{
			List<String> roman = new List<String> { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
			return roman[num];
		}

		/// <summary>
		/// Match Etymon und Reflex
		/// Prüft Wortart und Kategorie
		/// </summary>
		public static List<Wordform> MatchEtymonReflex(Wordform etymon, List<Wordform> reflex)
		{
			List<Wordform> matched = new List<Wordform>();

			foreach (KeyValuePair<Tuple<String, String>, List<KeyValuePair<String, String>>> entry in Functions.matchCatDict)
			{
				if (entry.Key.Item1 == etymon.Pos)
				{
					foreach (Wordform wf in reflex)
					{
						if (entry.Key.Item2 == wf.Pos)
						{
							/* Wortarten von Etymon und Reflex sind als matchendes Tupel in .match */
							if (entry.Value != null)
							{
								foreach (KeyValuePair<String, String> catPairs in entry.Value)
								{
									if ((catPairs.Key == etymon.Cat) && (catPairs.Value == wf.Cat))
										matched.Add(wf);
								}
							}
						}
					}
				}
			}

			return matched;
		}
	}
}

