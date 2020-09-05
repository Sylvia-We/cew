using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace frz
{
	public static class ReadFiles
	{
		private static MainWindow win_ = null;

		public static void SetMW(MainWindow value)
		{
			win_ = value;
		}

		public static void ReadGraph(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{

					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{

						Match graphRegex = Regex.Match(sLine, @"^\s*(.*?)\s+->\s+(.*?)\s*$");

						if (graphRegex.Groups[1].Value.Length > Functions.longestGraph)
						{
							Functions.longestGraph = graphRegex.Groups[1].Value.Length;
						}

						if (lang.Equals("lang1"))
						{
							String[] unicodes = graphRegex.Groups[2].Value.Split(',');
							String s = "";

							foreach (String unicode in unicodes)
							{
								s += Functions.ConvertUnicodeToString(unicode);
							}

							if (Functions.graphOutListLang1.ContainsKey(graphRegex.Groups[1].Value.Trim()))
							{
								Functions.ShowErrorMessage("Graphem " + graphRegex.Groups[1].Value.Trim() +
														   " ist in der graph-Datei mehrmals vorhanden.");
							}
							else
							{
								Functions.graphOutListLang1.Add(graphRegex.Groups[1].Value.Trim(), s.Normalize());
							}

						}
						else
						{
							Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
						}
					}
				}
			}
			objReader.Close();

			/* graphButtons erneut füllen */
			win_.CreateGraphButtons();
		}

		public static void ReadPhon(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{

					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{

						Match phonRegex = Regex.Match(sLine, @"^\s*(.*?)\s+->\s+(.*?)\s*$");

						if (phonRegex.Groups[1].Value.Length > Functions.longestPhon)
						{
							Functions.longestPhon = phonRegex.Groups[1].Value.Length;
						}

						if (lang.Equals("lang1"))
						{
							if (Functions.phonOutListLang1.ContainsKey(phonRegex.Groups[1].Value.Trim()))
								Functions.ShowErrorMessage("Phon \"" + phonRegex.Groups[1].Value.Trim() + " \" ist in der Datei " + file +
								" mehrmals definiert.");
							else
								Functions.phonOutListLang1.Add(phonRegex.Groups[1].Value.Trim(), phonRegex.Groups[2].Value.Trim());

						}
						else
						{
							Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
						}
					}
				}
			}

			objReader.Close();
		}

		public static void ReadAllo(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";
			bool header = false;
			List<string> tbls = new List<string>();
			AlloTbl at;
			List<AlloRule> arList = new List<AlloRule>();
			Dictionary<string, string> additionalAtt = new Dictionary<string, string>();

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");
					Match tblStart = Regex.Match(sLine, @"^\s*\[\s*$");
					Match tblEnd = Regex.Match(sLine, @"^\s*\]\s*$");
					Match tblName = Regex.Match(sLine, @"^\s*[AFL]_(.*)$");
					Match alloline = Regex.Match(sLine, @"^(.*)->(.*)\{(.*)\}\s*");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{

						/* spezielle Zeilen der Allo-Datei einlesen */

						if (tblStart.Success)
						{
							/* bei "[": vorherige Tabelle speichern,
								 * Variablen zurücksetzen */

							header = true;

							foreach (string t in tbls)
							{
								at = new AlloTbl(t, additionalAtt, arList);

								if (lang.Equals("lang1"))
								{
									Functions.alloListLang1.Add(at);
								}
								else if (lang.Equals("lang2"))
								{
									Functions.alloListLang2.Add(at);
								}
								else
								{
									Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
								}
							}

							tbls = new List<string>();
							arList = new List<AlloRule>();
							additionalAtt = new Dictionary<string, string>();

						}
						else if (tblName.Success)
						{
							/* bei "A_": Tabellenname speichern */

							Match slash = Regex.Match(sLine, @"/");

							/* gleicher Allomorphbildung verschiedener Tabellen (A_chanter / A_rejeter)*/
							if (slash.Success)
							{

								tbls.AddRange(sLine.Split('/'));

								for (int t = 0; t < tbls.Count; t++)
								{
									tbls[t] = Regex.Replace(tbls[t], @"^\s*A_", "");
									tbls[t] = Regex.Replace(tbls[t], @"\s*$", "");
								}

							}
							else
							{
								sLine = Regex.Replace(sLine, @"^\s*A_", "");
								sLine = Regex.Replace(sLine, @"\s*$", "");
								tbls.Add(sLine);
							}


						}
						else if (tblEnd.Success)
						{
							/* bei "]": */

							header = false;

						}
						else if (alloline.Success)
						{
							/* bei einer Regelzeile: Regel speichern */

							/* mehrere mögliche Endkategorien*/
							string[] cats = alloline.Groups[3].Value.Split('/');

							foreach (string c in cats)
							{
								c.Trim();
								c.TrimStart('{');
								c.TrimEnd('}');
							}

							AlloRule ar = new AlloRule(alloline.Groups[1].Value.Trim(), alloline.Groups[2].Value.Trim(), cats);
							arList.Add(ar);

						}
						else
						{

							Match att = Regex.Match(sLine, @"^(.*):(.*)$");

							if ((header == true) && att.Success)
							{
								/* Attribute und Werte speichern */
								additionalAtt.Add(att.Groups[1].Value.Trim(), att.Groups[2].Value.Trim());
							}
							else
							{
								Functions.ShowWarningMessage("Zeile nicht erkannt: " + sLine);
							}
						}
					}
				}

			}

			foreach (string t in tbls)
			{
				at = new AlloTbl(t, additionalAtt, arList);

				if (lang.Equals("lang1"))
				{
					Functions.alloListLang1.Add(at);
				}
				else if (lang.Equals("lang2"))
				{
					Functions.alloListLang2.Add(at);
				}
				else
				{
					Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
				}
			}

			objReader.Close();
		}

		public static void ReadFlex(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";
			bool header = false;
			List<string> tbls = new List<string>();
			FlexTbl ft;
			List<FlexRule> frlist = new List<FlexRule>();
			Dictionary<string, string> additionalAtt = new Dictionary<string, string>();

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{

					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");
					Match tblStart = Regex.Match(sLine, @"^\s*\[\s*$");
					Match tblEnd = Regex.Match(sLine, @"^\s*\]\s*$");
					Match tblName = Regex.Match(sLine, @"^\s*[AFL]_(.*)$");
					Match flexline = Regex.Match(sLine, @"\{(.*)\}(.*)->(.*?)\{(.*)\}\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{


						if (tblStart.Success)
						{
							/* bei "[": vorherige Tabelle speichern,
								 * Variablen zurücksetzen */

							header = true;

							foreach (string t in tbls)
							{

								ft = new FlexTbl(t, additionalAtt, frlist);

								if (lang.Equals("lang1"))
								{
									Functions.flexListLang1.Add(ft);
								}
								else if (lang.Equals("lang2"))
								{
									Functions.flexListLang2.Add(ft);
								}
								else
								{
									Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
								}
							}

							tbls = new List<string>();
							frlist = new List<FlexRule>();
							additionalAtt = new Dictionary<string, string>();

						}
						else if (tblName.Success)
						{
							/* bei "F_": Tabellenname speichern */

							Match slash = Regex.Match(sLine, @"/");

							/* gleicher Flexbildung verschiedener Tabellen (F_chanter / F_rejeter)*/
							if (slash.Success)
							{

								tbls.AddRange(sLine.Split('/'));

								for (int t = 0; t < tbls.Count; t++)
								{
									tbls[t] = Regex.Replace(tbls[t], @"^\s*F_", "");
									tbls[t] = Regex.Replace(tbls[t], @"\s*$", "");
								}

							}
							else
							{
								sLine = Regex.Replace(sLine, @"^\s*F_", "");
								sLine = Regex.Replace(sLine, @"\s*$", "");
								tbls.Add(sLine);
							}

						}
						else if (tblEnd.Success)
						{
							/* bei "]": */

							header = false;

						}
						else if (flexline.Success)
						{
							/* bei einer Regelzeile: Regel speichern */

							string finalcat = flexline.Groups[4].Value.Trim();

							if (Regex.Match(finalcat, @"/").Success)
							{

								string[] finalcats = finalcat.Split('/');

								foreach (string fc in finalcats)
								{

									string t = Regex.Match(fc, @"^(\s*\{)?(.*?)(\}\s*)?$").Groups[2].Value.Trim();
									FlexRule fr = new FlexRule(flexline.Groups[1].Value.Trim(), Functions.ConvertStringToSignList(flexline.Groups[2].Value.Trim()), t);
									frlist.Add(fr);
								}
							}
							else
							{
								FlexRule fr = new FlexRule(flexline.Groups[1].Value.Trim(), Functions.ConvertStringToSignList(flexline.Groups[2].Value.Trim()), finalcat);
								frlist.Add(fr);
							}

						}
						else
						{

							Match att = Regex.Match(sLine, @"^(.*):(.*)$");

							if ((header == true) && att.Success)
							{
								/* Attribute und Werte speichern */

								additionalAtt.Add(att.Groups[1].Value.Trim(), att.Groups[2].Value.Trim());

							}
							else
							{
								Functions.ShowWarningMessage("Zeile nicht erkannt: " + sLine);
							}
						}

					}
				}
			}

			foreach (string t in tbls)
			{
				ft = new FlexTbl(t, additionalAtt, frlist);

				if (lang.Equals("lang1"))
				{
					Functions.flexListLang1.Add(ft);
				}
				else if (lang.Equals("lang2"))
				{
					Functions.flexListLang2.Add(ft);
				}
				else
				{
					Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
				}
			}

			objReader.Close();
		}

		public static void ReadLex(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";
			bool header = false;
			List<string> tbls = new List<string>();

			LexTbl lt;
			String pos = "";
			List<List<Sign>> entries = new List<List<Sign>>();

			Dictionary<string, string> additionalAtt = new Dictionary<string, string>();


			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{

					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");
					Match tblStart = Regex.Match(sLine, @"^\s*\[\s*$");
					Match tblEnd = Regex.Match(sLine, @"^\s*\]\s*$");
					Match tblName = Regex.Match(sLine, @"^\s*[AFL]_(.*)$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{


						if (tblStart.Success)
						{
							/* bei "[": vorherige Tabelle speichern,
								 * Variablen zurücksetzen */

							header = true;

							foreach (string t in tbls)
							{
								lt = new LexTbl(t, additionalAtt, entries, pos);
								if (lang.Equals("lang1"))
								{
									Functions.lexListLang1.Add(lt);
								}
								else if (lang.Equals("lang2"))
								{
									Functions.lexListLang2.Add(lt);
								}
								else
								{
									Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
								}
							}

							tbls = new List<string>();
							entries = new List<List<Sign>>();
							additionalAtt = new Dictionary<string, string>();
							pos = "";

						}
						else if (tblName.Success)
						{
							/* bei "L_": Tabellenname speichern */
							tbls.Add(tblName.Groups[1].Value);

						}
						else if (tblEnd.Success)
						{
							/* bei "]": */
							header = false;

						}
						else
						{

							Match att = Regex.Match(sLine, @"^(.*):(.*)$");

							if ((header == true) && att.Success)
							{
								/* Attribute und Werte speichern */
								if (att.Groups[1].Value.Trim() == "Wortart")
									pos = att.Groups[2].Value.Trim();
								else
									additionalAtt.Add(att.Groups[1].Value.Trim(), att.Groups[2].Value.Trim());

							}
							else
							{
								entries.Add(Functions.ConvertStringToSignList(sLine.Trim()));
							}
						}
					}
				}

			}
			foreach (string t in tbls)
			{
				lt = new LexTbl(t, additionalAtt, entries, pos);
				if (lang.Equals("lang1"))
				{
					Functions.lexListLang1.Add(lt);
				}
				else if (lang.Equals("lang2"))
				{
					Functions.lexListLang2.Add(lt);
				}
				else
				{
					Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
				}
			}
			objReader.Close();

		}

		public static void ReadCat(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						Match cat = Regex.Match(sLine, @"^\s*{(.*)}\s*$");

						if (lang.Equals("lang1"))
						{
							Functions.finalCatListLang1.Add(cat.Groups[1].Value);
						}
						else if (lang.Equals("lang2"))
						{
							Functions.finalCatListLang2.Add(cat.Groups[1].Value);
						}
						else
						{
							Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
						}
					}
				}
			}
			objReader.Close();
		}

		public static void ReadSuffix(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						if (lang == "lang1")
						{
							Functions.suffixTrieLang1.Insert(Functions.ConvertStringToSignList(sLine), "suffix", null, null, null, null, "");
						}
						else if (lang == "lang2")
						{
							Functions.suffixTrieLang2.Insert(Functions.ConvertStringToSignList(sLine), "suffix", null, null, null, null, "");
						}
						else
						{
							Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
						}
					}
				}
			}
			objReader.Close();
		}

		public static void ReadAffix(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						if (lang == "lang1")
						{
							Functions.affixTrieLang1.Insert(Functions.ConvertStringToSignList(sLine), "affix", null, null, null, null, null);
						}
						else if (lang == "lang2")
						{
							Functions.affixTrieLang2.Insert(Functions.ConvertStringToSignList(sLine), "affix", null, null, null, null, null);
						}
						else
						{
							Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
						}
					}
				}
			}
			objReader.Close();
		}

		public static void ReadTrans(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						Match transLine = Regex.Match(sLine, @"^(.*?)\s+->\s*(.*?)\s*(\{(.*?)\})?$");
						Match conditionsMatch = Regex.Match(sLine, @"^(.*?)\{(.*?)\}\s*$");

						if (transLine.Success)
						{

							string graph = transLine.Groups[1].Value;
							string phon = transLine.Groups[2].Value;

							List<AttributeValue> attributeValueList = new List<AttributeValue>();

							if (conditionsMatch.Success)
							{

								string[] conditions = conditionsMatch.Groups[2].Value.Split(';');

								foreach (string cond in conditions)
								{
									Match condMatch = Regex.Match(cond, @"^\s*(.*?)\s*:\s*(.*?)\s*$");
									string conditionAttribute = condMatch.Groups[1].Value;
									string conditionValue = condMatch.Groups[2].Value;
									AttributeValue attributWert = new AttributeValue(conditionAttribute, conditionValue);
									attributeValueList.Add(attributWert);
								}
							}
							GraphPhon graphphon = new GraphPhon(graph, phon, attributeValueList);

							if (lang == "lang1")
							{
								Functions.TransListLang1.Add(graphphon);
							}
							else if (lang == "lang2")
							{
								Functions.TransListLang2.Add(graphphon);
							}
							else
							{
								Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
							}
						}
						else
						{
							Functions.ShowWarningMessage(sLine + " aus Transkriptionsdatei nicht erkannt");
						}
					}
				}
			}
			objReader.Close();
		}

		public static void ReadAcc(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						/* %X "Y Z {Y: ^.*DIPH.*$ || ^.*:.*$ || ^.*K$} */
						Match accLine = Regex.Match(sLine, @"^(.*?)\s*(\{(.*?)\})?$");
						Match accCondMatch = Regex.Match(sLine, @"^(.*?)\{(.*?)\}\s*$");

						if (accLine.Success)
						{

							string conditions = "";

							if (accCondMatch.Success)
							{
								conditions = accLine.Groups[3].Value;
							}
							AccentRule accRule = new AccentRule(accLine.Groups[1].Value, conditions);

							if (lang == "lang1")
							{
								Functions.accListLang1.Add(accRule);
							}
							else
							{
								Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
							}
						}
						else
						{
							Functions.ShowWarningMessage("Symbol nicht gefunden: " + sLine);
						}
					}
				}
			}
			objReader.Close();
		}

		public static void ReadVar(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						Match var = Regex.Match(sLine, @"^\s*(\$.*?)\s*:\s*(.*)\s*$");

						List<string> values = new List<string>(var.Groups[2].Value.Replace(" ", "").Split(','));
						Variable variable = new Variable(var.Groups[1].Value.Trim(), values);

						if (lang.Equals("lang1"))
						{
							Functions.varListLang1.Add(variable);
						}
						else
						{
							Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
						}
					}
				}
			}
			objReader.Close();

			foreach (Variable vari in Functions.varListLang1)
			{
				if (vari.Name == "$V")
					Functions.nuclei = vari.Values;
			}
		}

		public static void ReadSyll(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";
			bool insideHeadOfRule = false;
			String time = "";
			SyllableRuleList srl = null;
			List<SyllableRule> lsr = new List<SyllableRule>();

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");
					Match tblStart = Regex.Match(sLine, @"^\s*\[\s*$");
					Match tblEnd = Regex.Match(sLine, @"^\s*\]\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						if (tblStart.Success)
						{
							/* Header beginnt */
							insideHeadOfRule = true;

							if (lsr.Count > 0)
							{
								/* vorherige Tabelle speichern */
								srl = new SyllableRuleList(time, lsr);
								Functions.syllRuleList.Add(srl);
								time = "";
								lsr = new List<SyllableRule>();
							}
						}
						else if (tblEnd.Success)
						{
							/* Header endet */
							insideHeadOfRule = false;
						}
						else
						{
							if (insideHeadOfRule == true)
							{
								Match attValueRegex = Regex.Match(sLine, @"^\s*(.*?)\s*:\s*(.*)\s*$");

								if (attValueRegex.Success)
								{
									switch (attValueRegex.Groups[1].Value)
									{
										case "Zeit":
											time = attValueRegex.Groups[2].Value;
											break;
										default:
											break;
									}
								}
								else
								{
									throw new SymbolNotFoundException("Zeile nicht erkannt: " + sLine);
								}
							}
							else
							{
								Match syll = Regex.Match(sLine, @"^\s*(.*?)\s*({(.*?)})?$");

								SyllableRule syllrule = new SyllableRule(syll.Groups[1].Value.Trim(), syll.Groups[3].Value.Trim());
								lsr.Add(syllrule);
							}
						}
					}
				}
			}

			/* letzte Tabelle speichern */
			srl = new SyllableRuleList(time, lsr);
			Functions.syllRuleList.Add(srl);
			time = "";
			lsr = new List<SyllableRule>();

			objReader.Close();
		}

		public static void ReadRul(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			String sLineWithComment = "";
			String sLine = "";
			String id = "";
			String name = "";
			List<Change> changes = new List<Change>();
			String stress = "";
			String syllType = "";
			String left = "";
			String right = "";
			String time = "";
			String earlier = "";
			String specialCase = "";
			String andPreCond = "";
			String orPreCond = "";
			String cat = "";
			String tbl = "";
			Dictionary<String, String> addChanges = new Dictionary<string, string>();
			Dictionary<String, String> additional = new Dictionary<string, string>();
			Dictionary<String, String> addConditions = new Dictionary<string, string>();
			bool insideHeader = false;
			bool insideCond = false;

			while (objReader.Peek() > -1)
			{

				sLineWithComment = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLineWithComment, @"^\s*(#)");
					Match endComment = Regex.Match(sLineWithComment, @"^.+[^\[<]#[^\]>]");
					Match empty = Regex.Match(sLineWithComment, @"^\s*$");
					Match headStart = Regex.Match(sLineWithComment, @"^\s*\[\s*$");
					Match headEnd = Regex.Match(sLineWithComment, @"^\s*\]\s*$");

					sLine = sLineWithComment;

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						Match startRuleRegex = Regex.Match(sLine, @"^\s*\{\s*$");
						Match endRuleRegex = Regex.Match(sLine, @"^\s*\}\s*$");
						Match attValueRegex = Regex.Match(sLine, @"^\s*(.*?)\s*:\s*(.*)\s*$");

						if (headStart.Success)
						{
							/* Header beginnt */
							insideHeader = true;
						}
						else if (headEnd.Success)
						{
							/* Header endet */
							insideHeader = false;
						}
						else if (startRuleRegex.Success)
						{
							/* Bedingungen beginnen */
							insideCond = true;
						}
						else if (endRuleRegex.Success)
						{
							/* Bedingungen enden:
								 * Speichern 
								 */
							insideCond = false;
							SoundChangeRule scr = new SoundChangeRule(id, name, changes, stress, syllType, time, left, right, andPreCond, orPreCond,
													  earlier, specialCase, additional, addConditions, tbl, cat);
							int insertIndex = Functions.rulListLang1.Count;

							/* an der zeitlich richtigen Stelle einfügen */
							for (int i = 0; i < Functions.rulListLang1.Count; i++)
							{
								if (scr.StartDate < Functions.rulListLang1[i].StartDate)
								{
									insertIndex = i;
									break;
								}
							}

							Functions.rulListLang1.Insert(insertIndex, scr);
							id = "";
							name = "";
							stress = "";
							syllType = "";
							left = "";
							right = "";
							time = "";
							earlier = "";
							specialCase = "";
							andPreCond = "";
							orPreCond = "";
							tbl = "";
							cat = "";
							changes = new List<Change>();
							additional = new Dictionary<String, String>();
							addConditions = new Dictionary<String, String>();
							addChanges = new Dictionary<String, String>();

						}
						else if (insideHeader == false && insideCond == false)
						{
							/* Listen standardmäßig mit leerem String füllen */
							List<List<Sign>> phonBefore = Functions.ConvertStringToSignListList("", "phon", "before");
							List<Sign> phonAfter = Functions.ConvertStringToSignListPhon("");
							List<List<Sign>> graphBefore = Functions.ConvertStringToSignListList("", "graph", "before");
							List<Sign> graphAfter = Functions.ConvertStringToSignList("");

							List<String> changeList = new List<String>(sLine.Split(';'));

							foreach (String ch in changeList)
							{
								Match phonChangeRegex = Regex.Match(ch.Trim(), @"^\s*(\[(.*?)\]\s*>\s*\[(.*?)\])?\s*$");
								Match graphChangeRegex = Regex.Match(ch.Trim(), @"^\s*;?\s*(<(.*?)>\s*>\s*<(.*?)>)?\s*$");

								if (phonChangeRegex.Success)
								{
									phonBefore = Functions.ConvertStringToSignListList(phonChangeRegex.Groups[2].Value, "phon", "before");
									phonAfter = Functions.ConvertStringToSignListPhon(phonChangeRegex.Groups[3].Value);
								}
								else if (graphChangeRegex.Success)
								{
									graphBefore = Functions.ConvertStringToSignListList(graphChangeRegex.Groups[2].Value, "graph", "before");
									graphAfter = Functions.ConvertStringToSignList(graphChangeRegex.Groups[3].Value);
								}
								else
								{
									Match attValueChangeRegex = Regex.Match(ch.Trim(), @"^\s*(.*?)\s*:\s*(.*)\s*$");
									try
									{
										addChanges.Add(attValueChangeRegex.Groups[1].Value, attValueChangeRegex.Groups[2].Value);
									}
									catch (Exception e)
									{
										Functions.ShowErrorMessage(attValueChangeRegex.Groups[1].Value + " (" + e + ")");
									}
								}
							}

							changes.Add(new Change(phonBefore, phonAfter, graphBefore, graphAfter, addChanges));

						}
						else if (insideHeader == true)
						{

							if (attValueRegex.Success)
							{
								switch (attValueRegex.Groups[1].Value)
								{
									case "Id":
										id = attValueRegex.Groups[2].Value;
										break;
									case "Name":
										name = attValueRegex.Groups[2].Value;
										break;
									default:
										try
										{
											additional.Add(attValueRegex.Groups[1].Value, attValueRegex.Groups[2].Value);
										}
										catch (Exception e)
										{
											Functions.ShowErrorMessage(attValueRegex.Groups[1].Value + " (" + e + ")");
										}
										break;
								}
							}
						}
						else if (insideCond == true)
						{
							/* alle anderen Attribute */

							if (attValueRegex.Success)
							{
								switch (attValueRegex.Groups[1].Value)
								{
									case "Betonung":
										Match stressRegex = Regex.Match(sLineWithComment, @"^\s*(.*?)\s*:\s*(.*)\s*$");
										stress = stressRegex.Groups[2].Value;
										break;
									case "Silbe":
										syllType = attValueRegex.Groups[2].Value;
										break;
									case "Zeit":
										time = attValueRegex.Groups[2].Value;
										break;
									case "Linker Kontext":
										left = attValueRegex.Groups[2].Value;
										break;
									case "Rechter Kontext":
										right = attValueRegex.Groups[2].Value;
										break;
									case "Und-Vorbedingung":
										andPreCond = attValueRegex.Groups[2].Value;
										break;
									case "Oder-Vorbedingung":
										orPreCond = attValueRegex.Groups[2].Value;
										break;
									case "Früher":
										earlier = attValueRegex.Groups[2].Value;
										break;
									case "Spezialfall":
										specialCase = attValueRegex.Groups[2].Value;
										break;
									case "Tabelle":
										tbl = attValueRegex.Groups[2].Value;
										break;
									case "Gram. Kat.":
										cat = attValueRegex.Groups[2].Value;
										break;
									default:
										try
										{
											addConditions.Add(attValueRegex.Groups[1].Value, attValueRegex.Groups[2].Value);
										}
										catch (Exception e)
										{
											Functions.ShowErrorMessage(attValueRegex.Groups[1].Value + " (" + e + ")");
										}
										break;
								}
							}
							else
							{
								Functions.ShowErrorMessage("Syntaxfehler! Zeile nicht erkannt: " + sLine);
							}
						}
					}
				}
			}
			objReader.Close();
		}

		public static void ReadNCat(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						Match ncat = Regex.Match(sLine, @"^\s*{(.*?)}\s*->\s*{(.*)}\s*$");

						if (ncat.Success)
						{
							if (Functions.newCatDict.ContainsKey(ncat.Groups[1].Value))
							{
								Functions.newCatDict[ncat.Groups[1].Value].Add(ncat.Groups[2].Value);
							}
							else
							{
								Functions.newCatDict.Add(ncat.Groups[1].Value, new List<String> { ncat.Groups[2].Value });
							}
						}
						else
						{
							Functions.ShowWarningMessage("Sprache nicht gefunden: " + lang);
						}
					}
				}
			}
			objReader.Close();
		}

		public static void ReadMatch(string file, string lang)
		{
			StreamReader objReader = new StreamReader(file);
			string sLine = "";
			Tuple<String, String> currPos = new Tuple<String, String>("", "");

			while (objReader.Peek() > -1)
			{

				sLine = objReader.ReadLine();

				if (sLine != null)
				{
					Match comment = Regex.Match(sLine, @"^\s*(#)");
					Match endComment = Regex.Match(sLine, @"^.+#");
					Match empty = Regex.Match(sLine, @"^\s*$");

					/* Kommentare am Zeilenende entfernen */
					if (endComment.Success)
					{
						sLine = sLine.Split('#')[0].TrimEnd();
					}

					/* Zeilen, die verarbeitet werden */
					if ((!comment.Success) && (!empty.Success))
					{
						Match catMatch = Regex.Match(sLine, @"^\s*{(.*?)}\s*->\s*{(.*)}\s*$");

						if (catMatch.Success)
						{
							/* cat-Zeile */
							if ((currPos.Item1 != "") && (currPos.Item2 != ""))
							{
								if (Functions.matchCatDict[currPos] == null)
									Functions.matchCatDict[currPos] = new List<KeyValuePair<string, string>>();
								Functions.matchCatDict[currPos].Add(new KeyValuePair<string, string>(catMatch.Groups[1].Value, catMatch.Groups[2].Value));
							}
						}
						else
						{
							/* pos-Zeile */
							Match posMatch = Regex.Match(sLine, @"^\s*(.*?)\s*->\s*(.*)\s*:\s*$");
							currPos = new Tuple<String, String>(posMatch.Groups[1].Value, posMatch.Groups[2].Value);
							if (!Functions.matchCatDict.ContainsKey(currPos))
								Functions.matchCatDict.Add(currPos, null);
						}
					}
				}
			}
			objReader.Close();
		}
	}
}
