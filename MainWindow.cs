using System;
using Gtk;
using Gdk;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace frz
{
	public partial class MainWindow : Gtk.Window
	{
		private static int numWellfLang1 = 0;
		private static int numWellfLang2 = 0;
		private static int numGrade1 = 0;
		private static int numGrade2 = 0;
		Gtk.TreeStore treeStore = new Gtk.TreeStore(typeof(MyTreeNode));
		Gtk.TreeView tree = new Gtk.TreeView();

		public MainWindow() : base(Gtk.WindowType.Toplevel)
		{
			this.SetSizeRequest(this.Screen.Width / 2, this.Screen.Height - 100);
			this.Build();
			this.SetIconFromFile(Functions.logoFile);

			scrolledwindow1.Add(tree);

			Gtk.TreeViewColumn arrowColumn = new Gtk.TreeViewColumn();
			Gtk.CellRendererText arrowCell = new Gtk.CellRendererText();
			arrowColumn.PackStart(arrowCell, true);

			Gtk.TreeViewColumn inputColumn = new Gtk.TreeViewColumn();
			inputColumn.Title = "";
			Gtk.CellRendererText inputCell = new Gtk.CellRendererText();
			inputColumn.PackStart(inputCell, true);

			Gtk.TreeViewColumn analysis1Column = new Gtk.TreeViewColumn();
			Gtk.CellRendererText analysis1Cell = new Gtk.CellRendererText();
			analysis1Column.PackStart(analysis1Cell, true);

			Gtk.TreeViewColumn analysis2Column = new Gtk.TreeViewColumn();
			Gtk.CellRendererText analysis2Cell = new Gtk.CellRendererText();
			analysis2Column.PackStart(analysis2Cell, true);

			arrowColumn.SetCellDataFunc(arrowCell, new Gtk.TreeCellDataFunc(RenderArrowCell));
			inputColumn.SetCellDataFunc(inputCell, new Gtk.TreeCellDataFunc(RenderInputCell));
			analysis1Column.SetCellDataFunc(analysis1Cell, new Gtk.TreeCellDataFunc(RenderAnalysis1Cell));
			analysis2Column.SetCellDataFunc(analysis2Cell, new Gtk.TreeCellDataFunc(RenderAnalysis2Cell));

			tree.Model = treeStore;

			tree.AppendColumn(arrowColumn);
			tree.AppendColumn(inputColumn);
			tree.AppendColumn(analysis1Column);
			tree.AppendColumn(analysis2Column);

			tree.ExpanderColumn = analysis1Column;

			scrolledwindow1.ShowAll();

			Start.Clicked += OnGo;
			entry1.Activated += OnGo;

		}

		///<summary>
		///true, if this form is locked and the cursor has the
		///shape of an hourglass (Windows) or watch (Linux).
		///</summary>
		public bool Hourglass
		{

			set
			{
				if (value == true)
				{
					this.GdkWindow.Cursor = new Cursor(Gdk.CursorType.Watch);
				}
				else
				{
					this.GdkWindow.Cursor = new Cursor(Gdk.CursorType.LeftPtr);
				}
			}
		}

		private void RenderArrowCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			MyTreeNode node = (MyTreeNode)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).CellBackground = "White";
			(cell as Gtk.CellRendererText).Text = node.arrow;
			(cell as Gtk.CellRendererText).Weight = node.inputBold;
			(cell as Gtk.CellRendererText).Family = "DejaVu Sans";
			(cell as Gtk.CellRendererText).Size = node.inputSize;
		}

		private void RenderInputCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			MyTreeNode node = (MyTreeNode)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).CellBackground = node.color;
			(cell as Gtk.CellRendererText).Text = node.input;
			(cell as Gtk.CellRendererText).Weight = node.inputBold;
			(cell as Gtk.CellRendererText).Family = "DejaVu Sans";
			(cell as Gtk.CellRendererText).Size = node.inputSize;
		}

		private void RenderAnalysis1Cell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			MyTreeNode node = (MyTreeNode)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).CellBackground = node.color;
			(cell as Gtk.CellRendererText).Text = node.analysis1;
			(cell as Gtk.CellRendererText).Family = "DejaVu Sans";
			(cell as Gtk.CellRendererText).Size = node.analysisSize;
		}

		private void RenderAnalysis2Cell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			MyTreeNode node = (MyTreeNode)model.GetValue(iter, 0);
			(cell as Gtk.CellRendererText).CellBackground = node.color;
			(cell as Gtk.CellRendererText).Text = node.analysis2;
			(cell as Gtk.CellRendererText).Family = "DejaVu Sans";
			(cell as Gtk.CellRendererText).Size = node.analysisSize;
		}

		/// <summary>
		/// Wordform zum TextView hinzufügen.
		/// </summary>
		public void AddToTextView(Wordform wf, bool changed, String comment, String color, String arrowType, String lang)
		{
			TreePath path;

			/* Wortform + IPA  = IterNode */
			List<string> shortLine = new List<string>(wf.PrintShort(comment, ""));
			MyTreeNode shortNode = new MyTreeNode(arrowType, Functions.SpliceText(shortLine.ElementAt(1), 20, @"\s"), shortLine.ElementAt(2),
									   Functions.SpliceText(shortLine.ElementAt(3), 60, @"\s"));
			shortNode.color = color;
			if (changed == false)
			{
				shortNode.inputBold = 600;
				shortNode.inputSize = Convert.ToInt32(10 * Pango.Scale.PangoScale);
			}
			Gtk.TreeIter iter = treeStore.AppendValues(shortNode);
			path = treeStore.GetPath(iter);
			tree.ScrollToCell(path, null, true, 0, 0);

			/* Attribute */
			bool grey = false;

			if (changed == true)
			{
				foreach (List<string> line in wf.PrintChanged())
				{
					MyTreeNode node = new MyTreeNode(line.ElementAt(0), Functions.SpliceText(line.ElementAt(1), 20, @"\s"), line.ElementAt(2),
										  Functions.SpliceText(line.ElementAt(3), 60, @"\s"));
					if (grey == false)
					{
						node.color = "white";
						grey = true;
					}
					else
					{
						node.color = "Gray98";
						grey = false;
					}
					treeStore.AppendValues(iter, node);
					path = treeStore.GetPath(iter);
					tree.ScrollToCell(path, null, true, 0, 0);
				}

				treeStore.AppendValues(iter, new MyTreeNode("", "", "", ""));
				path = treeStore.GetPath(iter);
				tree.ScrollToCell(path, null, true, 0, 0);
			}
			else
			{
				List<List<String>> print = new List<List<string>>();

				if (lang == "lang1")
				{
					print = wf.Print();
				}
				else
				{
					print = wf.PrintLang2();
				}

				foreach (List<string> line in print)
				{
					MyTreeNode node = new MyTreeNode(line.ElementAt(0), Functions.SpliceText(line.ElementAt(1), 20, @"\s"), line.ElementAt(2),
										  Functions.SpliceText(line.ElementAt(3), 60, @"\s"));
					if (grey == false)
					{
						node.color = "white";
						grey = true;
					}
					else
					{
						node.color = "Gray98";
						grey = false;
					}
					treeStore.AppendValues(iter, node);
					path = treeStore.GetPath(iter);
					tree.ScrollToCell(path, null, true, 0, 0);
				}

				treeStore.AppendValues(iter, new MyTreeNode("", "", "", ""));
				path = treeStore.GetPath(iter);
				tree.ScrollToCell(path, null, true, 0, 0);
			}

			while (Application.EventsPending())
			{
				Application.RunIteration();
			}
		}

		/// <summary>
		/// SoundChangeRule zum TextView hinzufügen.
		/// </summary>
		public void AddToTextView(SoundChangeRule scr, List<Change> lastChanges, int multiPathNum)
		{
			TreePath path;
			Gtk.TreeIter iter;

			/* MultiPath */
			if (multiPathNum != 0)
			{
				MyTreeNode multiPathNode = new MyTreeNode("", "\u2502 (" + Functions.ToRoman(multiPathNum) + ")", "", "");
				multiPathNode.color = "Honeydew";
				multiPathNode.inputBold = 600;
				multiPathNode.inputSize = Convert.ToInt32(10 * Pango.Scale.PangoScale);
				iter = treeStore.AppendValues(multiPathNode);
				path = treeStore.GetPath(iter);
				tree.ScrollToCell(path, null, true, 0, 0);
			}

			/* Id + Name = IterNode */
			List<String> idName = scr.PrintShort();

			MyTreeNode idNameNode = new MyTreeNode(idName.ElementAt(0), Functions.SpliceText(idName.ElementAt(1), 20, @"\s"),
										Functions.SpliceText(idName.ElementAt(2), 50, @","),
										Functions.SpliceText(idName.ElementAt(3), 60, @"\s"));
			idNameNode.color = "Honeydew";
			iter = treeStore.AppendValues(idNameNode);
			path = treeStore.GetPath(iter);
			tree.ScrollToCell(path, null, true, 0, 0);

			/* Attribute */
			bool grey = false;

			List<List<String>> printList = scr.Print(lastChanges);

			foreach (List<string> line in printList)
			{
				MyTreeNode node = new MyTreeNode(line.ElementAt(0), Functions.SpliceText(line.ElementAt(1), 20, @"\s"),
									  Functions.SpliceText(line.ElementAt(2), 50, @","),
									  Functions.SpliceText(line.ElementAt(3), 60, @"\s"));
				if (grey == false)
				{
					node.color = "white";
					grey = true;
				}
				else
				{
					node.color = "Gray98";
					grey = false;
				}
				treeStore.AppendValues(iter, node);
				path = treeStore.GetPath(iter);
				tree.ScrollToCell(path, null, true, 0, 0);
			}

			/* Pfeilspitze */
			MyTreeNode arrowNode = new MyTreeNode("", "\u25bc", "", "");
			arrowNode.color = "Honeydew";
			iter = treeStore.AppendValues(arrowNode);
			path = treeStore.GetPath(iter);
			tree.ScrollToCell(path, null, true, 0, 0);

			while (Application.EventsPending())
			{
				Application.RunIteration();
			}
		}

		public void AddToTextView(List<String> s, String color)
		{
			MyTreeNode node = new MyTreeNode(s.ElementAt(0), Functions.SpliceText(s.ElementAt(1), 20, @"\s"), s.ElementAt(2),
								  Functions.SpliceText(s.ElementAt(3), 60, @"\s"));
			node.color = color;
			Gtk.TreeIter iter = treeStore.AppendValues(node);
			TreePath path = treeStore.GetPath(iter);
			tree.ScrollToCell(path, null, true, 0, 0);

			while (Application.EventsPending())
			{
				Application.RunIteration();
			}
		}

		protected override bool OnDeleteEvent(Gdk.Event ev)
		{
			Gtk.Application.Quit();
			return true;
		}

		/// <summary>
		/// ???
		/// </summary>
		protected void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Application.Quit();
			a.RetVal = true;
		}

		/// <summary>
		/// Testdatei öffnen, einlesen und WFR starten
		/// </summary>
		protected void OnOpen(object sender, System.EventArgs e)
		{
			FileChooserDialog chooser = new FileChooserDialog(
				"Datei auswählen...", this, FileChooserAction.Open,
				"Abbrechen", ResponseType.Cancel, "Öffnen", ResponseType.Accept);

			chooser.SetCurrentFolder(Functions.testFilesFile);

			if (chooser.Run() == (int)ResponseType.Accept)
			{
				/* keine Diakritika verwenden */
				diacriticsToggleAction.Active = false;
				hbox1.Sensitive = false;
				hbox3.Sensitive = false;
				hbox4.Sensitive = false;
				Functions.diacriticInput = false;

				System.IO.StreamReader file = System.IO.File.OpenText(chooser.Filename);
				chooser.Destroy();

				Hourglass = true;
				while (Application.EventsPending())
				{
					Application.RunIteration();
				}

				string sLine = "";
				int numTotalLang1 = 0;
				int numTotalLang2 = 0;
				numWellfLang1 = 0;
				numWellfLang2 = 0;
				numGrade1 = 0;
				numGrade2 = 0;
				calcResultOutputLine1_1.LabelProp = "";
				calcResultOutputLine1_2.LabelProp = "";
				calcResultOutputLine2.LabelProp = "";
				calcResultOutputLine3.LabelProp = "";
				calcResultOutputLine4.LabelProp = "";

				Stopwatch watch = new Stopwatch();
				watch.Start();
				Functions.allWatch.Start();


				/* Testdatei einlesen*/
				while (sLine != null)
				{
					sLine = file.ReadLine();

					if (sLine != null)
					{

						string line = sLine.Trim();
						String comm = "";

						Match comment = Regex.Match(line, @"^(#|\[|\])");
						Match empty = Regex.Match(line, @"^$");
						Match endComment = Regex.Match(sLine, @"^.+[^\[<](#[^\]>].*)$");

						/* Kommentare am Zeilenende entfernen */
						if (endComment.Success)
						{
							line = line.Split('#')[0].TrimEnd();
							comm = endComment.Groups[1].Value;
						}

						if (!comment.Success && !empty.Success)
						{
							calcResultOutputLine1_1.LabelProp = "";
							calcResultOutputLine1_2.LabelProp = "";
							calcResultOutputLine2.LabelProp = "";
							calcResultOutputLine3.LabelProp = "";
							calcResultOutputLine4.LabelProp = "";

							switch (Functions.mode)
							{
								case "lautentwicklung":
									numTotalLang1++;
									numTotalLang2++;
									Functions.PrepareForRecognition(line, ref numWellfLang1, ref numWellfLang2, ref numGrade1,
																	ref numGrade2, comm);
									break;
								case "transkription_lang1":
									Functions.TranscriptOnly(line, "lang1");
									break;
								case "transkription_lang2":
									Functions.TranscriptOnly(line, "lang2");
									break;
								case "wfe_lang1":
									numTotalLang1++;
									Functions.RecognizeOnly(line, "lang1", ref numWellfLang1);
									break;
								case "wfe_lang2":
									Functions.RecognizeOnly(line, "lang2", ref numWellfLang2);
									break;

								default:
									numTotalLang1++;
									numTotalLang2++;
									Functions.PrepareForRecognition(line, ref numWellfLang1, ref numWellfLang2, ref numGrade1,
																	ref numGrade2, comm);
									break;
							}

						}
					}
				}

				double prozentLang1;
				double prozentLang2;
				double prozentGrade1;
				double prozentGrade2;

				if (numTotalLang1 == 0)
				{
					prozentLang1 = 0;
				}
				else
				{
					prozentLang1 = Math.Round((double)numWellfLang1 / (double)numTotalLang1 * 100, 1);
				}

				if (numTotalLang2 == 0)
				{
					prozentLang2 = 0;
					prozentGrade1 = 0;
					prozentGrade2 = 0;
				}
				else
				{
					prozentLang2 = Math.Round((double)numWellfLang2 / (double)numTotalLang2 * 100, 1);
					prozentGrade1 = Math.Round((double)numGrade1 / (double)numTotalLang2 * 100, 1);
					prozentGrade2 = Math.Round((double)numGrade2 / (double)numTotalLang2 * 100, 1);
				}

				/* Analyseausgabe*/
				calcResultOutputLine1_1.LabelProp = "Erkannt (Quellsprache ): " + numWellfLang1 + " / " + numTotalLang1 + " (" + prozentLang1 + "%)";
				calcResultOutputLine2.LabelProp = "Übereinstimmungsgrad 1: " + numGrade1 + " / " + numTotalLang2 + " (" + prozentGrade1 + "%)";
				calcResultOutputLine3.LabelProp = "Übereinstimmungsgrad 2: " + numGrade2 + " / " + numTotalLang2 + " (" + prozentGrade2 + "%)";
				calcResultOutputLine4.LabelProp = "Übereinstimmungsgrad 3: " + numWellfLang2 + " / " + numTotalLang2 + " (" + prozentLang2 + "%)";

				/* Analyseausgabe Analysedauer */
				watch.Stop();
				TimeSpan ts = watch.Elapsed;
				string elapsedTime = String.Format("{0:00} min {1:00} s {2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
				calcResultOutputLine1_2.LabelProp += "Analysedauer: " + elapsedTime;

				file.Close();

				Functions.allWatch.Stop();
				Functions.allTs = Functions.allWatch.Elapsed;
				Functions.timeText += String.Format("{0:00} min {1:00} s {2:00}", Functions.allTs.Minutes, Functions.allTs.Seconds,
													Functions.allTs.Milliseconds + Environment.NewLine);
				Functions.allWatch.Reset();

				/* Debug
				 * File.WriteAllText(Functions.evaluationFile, Functions.timeText);
				 */

				Hourglass = false;
			}

			chooser.Destroy();
		}

		/// <summary>
		/// save result
		/// </summary>
		protected void OnSave(object sender, System.EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			TreeIter iter;
			tree.Model.GetIterFirst(out iter);
			for (int i = 0; i < tree.Model.IterNChildren(); i++)
			{
				foreach (TreeViewColumn column in tree.Columns)
				{
					/* Only output visible columns */
					if (column.Visible)
					{
						/* Loop through CellRenderers to make sure we have a CellRendererText */
						string value = null;
						column.CellSetCellData(tree.Model, iter, false, false);
						foreach (CellRenderer renderer in column.CellRenderers)
						{
							CellRendererText text = renderer as CellRendererText;
							if (text != null)
							{
								/* Setting value indicates this column had a CellRendererText and should be included */
								if (value == null)
								{
									value = String.Empty;
								}

								/* Append to the value */
								if ((text.Text != null) && (text.Text != ""))
								{
									value += text.Text + " ";
								}
							}
						}
						if (value != null)
						{
							sb.Append(value);
						}
					}
				}
				sb.Append(Environment.NewLine);


				TreeIter child;
				if (tree.Model.IterChildren(out child, iter))
				{
					do
					{
						foreach (TreeViewColumn column in tree.Columns)
						{
							/* Only output visible columns */
							if (column.Visible)
							{
								/* Loop through CellRenderers to make sure we have a CellRendererText */
								string value = null;
								column.CellSetCellData(tree.Model, child, false, false);
								foreach (CellRenderer renderer in column.CellRenderers)
								{
									CellRendererText text = renderer as CellRendererText;
									if (text != null)
									{
										/* Setting value indicates this column had a CellRendererText and should be included */
										if (value == null)
										{
											value = String.Empty;
										}

										/* Append to the value */
										if ((text.Text != null) && (text.Text != ""))
										{
											value += text.Text + " ";
										}
									}
								}
								if (value != null)
								{
									sb.Append(value);
								}

							}
						}
						sb.Append(Environment.NewLine);
					} while (tree.Model.IterNext(ref child));
				}
				tree.Model.IterNext(ref iter);
				sb.Append(Environment.NewLine);
			}

			/* Analyseausgabe */
			sb.Append("Analyseergebnis:" + Environment.NewLine);
			sb.Append("----------------" + Environment.NewLine);
			sb.Append(calcResultOutputLine1_1.Text + Environment.NewLine);
			sb.Append(calcResultOutputLine2.Text + Environment.NewLine);
			sb.Append(calcResultOutputLine3.Text + Environment.NewLine);
			sb.Append(calcResultOutputLine4.Text + Environment.NewLine);
			sb.Append(calcResultOutputLine1_2.Text);

			FileChooserDialog chooser = new FileChooserDialog(
											"Name auswählen...", this, FileChooserAction.Save, "Abbrechen", ResponseType.Cancel, "Speichern", ResponseType.Accept);
			chooser.SetCurrentFolder(Functions.outputFile);

			chooser.Filter = new FileFilter();
			chooser.Filter.AddPattern("*.txt");

			if (chooser.Run() == (int)ResponseType.Accept)
			{
				File.WriteAllText(chooser.Filename, sb.ToString());
			}
			chooser.Destroy();
		}

		/// <summary>
		/// Beenden.
		/// </summary>
		protected void OnExit(object sender, System.EventArgs e)
		{
			Application.Quit();
		}

		/// <summary>
		/// Raises the about event.
		/// </summary>
		protected void OnAbout(object sender, System.EventArgs e)
		{
			AboutDialog about = new AboutDialog();

			about.ProgramName = "CEW";
			about.Version = "1.0";
			about.Authors = new string[] { "Sylvia Weber" };
			about.Copyright = "2018";
			about.Logo = new Gdk.Pixbuf(Functions.logoFile);

			String dissTitle = "Konzeption und Umsetzung eines computergenerierten etymologischen Wörterbuchs am Beispiel des Französischen";

			about.Comments = "Software zur Dissertationsschrift" + Environment.NewLine + Environment.NewLine +
				"Weber, Sylvia (2018): " + dissTitle + ", Diss., Friedrich-Alexander-Universität Erlangen-Nürnberg.";

			about.Run();
			about.Destroy();
		}

		/// <summary>
		/// Eingegebenes Wort analysieren
		/// </summary>
		protected void OnGo(object sender, System.EventArgs e)
		{
			if (entry1.Text == "")
				return;
			string etymon = entry1.Text.Trim();
			calcResultOutputLine1_1.LabelProp = "";
			calcResultOutputLine1_2.LabelProp = "";
			calcResultOutputLine2.LabelProp = "";
			calcResultOutputLine3.LabelProp = "";
			calcResultOutputLine4.LabelProp = "";

			Hourglass = true;
			while (Application.EventsPending())
			{
				Application.RunIteration();
			}

			bool val = false;

			switch (Functions.mode)
			{
				case "lautentwicklung":
					Functions.PrepareForRecognition(etymon, ref numWellfLang1, ref numWellfLang2, ref numGrade1, ref numGrade2, "");
					break;
				case "transkription_lang1":
					Functions.TranscriptOnly(etymon, "lang1");
					break;
				case "transkription_lang2":
					Functions.TranscriptOnly(etymon, "lang2");
					break;
				case "wfe_lang1":
					Functions.RecognizeOnly(etymon, "lang1", ref numWellfLang1);
					break;
				case "wfe_lang2":
					Functions.RecognizeOnly(etymon, "lang2", ref numWellfLang2);
					break;

				default:
					Functions.PrepareForRecognition(etymon, ref numWellfLang1, ref numWellfLang2, ref numGrade1, ref numGrade2, "");
					break;
			}

			Hourglass = false;

			if (val == true)
				entry1.Text = "";
		}

		/// <summary>
		/// Textfeld leeren
		/// </summary>
		protected void OnClear(object sender, System.EventArgs e)
		{
			treeStore.Clear();
			calcResultOutputLine1_1.LabelProp = "";
			calcResultOutputLine1_2.LabelProp = "";
			calcResultOutputLine2.LabelProp = "";
			calcResultOutputLine3.LabelProp = "";
			calcResultOutputLine4.LabelProp = "";
		}

		protected void OnRereadFiles(object sender, EventArgs e)
		{
			Functions.detail = "";
			Functions.nameLang1 = "";
			Functions.nameLang2 = "";
			Functions.timeLang1 = 0;
			Functions.timeLang2 = 0;
			Functions.alloListLang1 = new List<AlloTbl>();
			Functions.alloListLang2 = new List<AlloTbl>();
			Functions.flexListLang1 = new List<FlexTbl>();
			Functions.flexListLang2 = new List<FlexTbl>();
			Functions.lexListLang1 = new List<LexTbl>();
			Functions.lexListLang2 = new List<LexTbl>();
			Functions.finalCatListLang1 = new List<String>();
			Functions.finalCatListLang2 = new List<String>();
			Functions.TransListLang1 = new List<GraphPhon>();
			Functions.TransListLang2 = new List<GraphPhon>();
			Functions.varListLang1 = new List<Variable>();
			Functions.syllRuleList = new List<SyllableRuleList>();
			Functions.accListLang1 = new List<AccentRule>();
			Functions.rulListLang1 = new List<SoundChangeRule>();
			Functions.phonOutListLang1 = new Dictionary<String, String>();
			Functions.phonOutDiaListLang1 = new Dictionary<String, String>();
			Functions.longestPhon = 0;
			Functions.longestGraph = 0;
			Functions.graphOutListLang1 = new Dictionary<String, String>();
			Functions.graphOutDiaListLang1 = new Dictionary<String, String>();
			Functions.stemTrieLang1 = new Trie();
			Functions.stemTrieLang2 = new Trie();
			Functions.affixTrieLang1 = new Trie();
			Functions.affixTrieLang2 = new Trie();
			Functions.suffixTrieLang1 = new Trie();
			Functions.suffixTrieLang2 = new Trie();
			Functions.newCatDict = new Dictionary<String, List<String>>();
			Functions.matchCatDict = new Dictionary<Tuple<string, string>, List<KeyValuePair<string, string>>>();

			Functions.ReadProjectFile(Functions.projectFile);
			Functions.GenerateAllomorphs();
		}

		protected void OnDiacritics(object sender, EventArgs e)
		{
			if (diacriticsToggleAction.Active == true)
			{
				/* Graphem-Buttons einblenden */
				hbox1.Sensitive = true;
				hbox3.Sensitive = true;
				hbox4.Sensitive = true;
				Functions.diacriticInput = true;
			}
			else
			{
				/* Graphem-Buttons ausgrauen */
				hbox1.Sensitive = false;
				hbox3.Sensitive = false;
				hbox4.Sensitive = false;
				Functions.diacriticInput = false;
			}
		}

		protected void OnExpand(object sender, EventArgs e)
		{
			if (expandAction.Active == true)
			{
				tree.ExpandAll();
			}
			else
			{
				tree.CollapseAll();
			}
		}

		protected void ClickedEventHandler(object sender, EventArgs e)
		{
			Button button = (sender as Button);
			entry1.Text = entry1.Text + button.Label;
			entry1.HasFocus = true;
			entry1.Position = -1;
		}

		public void CreateGraphButtons()
		{
			int num = 0;

			hbox1.Forall((element) => hbox1.Remove(element));
			hbox3.Forall((element) => hbox3.Remove(element));
			hbox4.Forall((element) => hbox4.Remove(element));

			foreach (KeyValuePair<string, string> entry in Functions.graphOutListLang1)
			{

				Button button1;

				button1 = new Button(entry.Value);
				button1.Relief = Gtk.ReliefStyle.None;
				button1.Clicked += ClickedEventHandler;
				button1.TooltipText = "Graphem " + entry.Value;

				if (num < 40)
				{
					hbox1.PackStart(button1, false, false, 0);
				}
				else if (num >= 40 && num < 80)
				{
					hbox3.PackStart(button1, false, false, 0);
				}
				else if (num >= 80 && num < 120)
				{
					hbox4.PackStart(button1, false, false, 0);
				}

				button1.Show();
				num++;
			}
		}

		protected void OnModusAction(object sender, EventArgs e)
		{
			if (comboboxModus.Active == 0)
			{
				Functions.mode = "lautentwicklung";
			}
			else if (comboboxModus.Active == 2)
			{
				Functions.mode = "transkription_lang2";
			}
			else if (comboboxModus.Active == 1)
			{
				Functions.mode = "transkription_lang1";
			}
			else if (comboboxModus.Active == 4)
			{
				Functions.mode = "wfe_lang2";
			}
			else if (comboboxModus.Active == 3)
			{
				Functions.mode = "wfe_lang1";
			}
		}
	}
}