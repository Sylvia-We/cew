
// This file has been generated by the GUI designer. Do not modify.
namespace frz
{
	public partial class MainWindow
	{
		private global::Gtk.UIManager UIManager;

		private global::Gtk.Action AnsichtAction;

		private global::Gtk.Action DateiAction;

		private global::Gtk.Action HilfeAction;

		private global::Gtk.Action WortlisteFfnenAction;

		private global::Gtk.Action ErgebnisInDateiSpeichernAction;

		private global::Gtk.Action SchlieenAction;

		private global::Gtk.Action NormalansichtAction;

		private global::Gtk.Action DokumentationAction;

		private global::Gtk.Action AboutAction;

		private global::Gtk.Action ExtrasAction;

		private global::Gtk.Action openAction;

		private global::Gtk.Action saveAction;

		private global::Gtk.Action LeerenAction;

		private global::Gtk.Action deleteAction;

		private global::Gtk.Action clearAction;

		private global::Gtk.ToggleAction expandAction;

		private global::Gtk.Action BearbeitenAction;

		private global::Gtk.Action TextdateienNeuEinlesenAction;

		private global::Gtk.ToggleAction diacriticsToggleAction;

		private global::Gtk.Action ModusAction;

		private global::Gtk.RadioAction LautentwicklungAction;

		private global::Gtk.RadioAction TranskriptionDerZielspracheAction;

		private global::Gtk.RadioAction WortformerkennungDerQuellspracheAction;

		private global::Gtk.RadioAction WortformerkennungDerZielspracheAction;

		private global::Gtk.RadioAction Action;

		private global::Gtk.RadioAction TranskriptionDerQuellspracheAction;

		private global::Gtk.VBox vbox1;

		private global::Gtk.MenuBar menubar1;

		private global::Gtk.HBox hbox2;

		private global::Gtk.Toolbar toolbar1;

		private global::Gtk.Label modusLabel;

		private global::Gtk.ComboBoxEntry comboboxModus;

		private global::Gtk.Entry entry1;

		private global::Gtk.Button Start;

		private global::Gtk.HBox hbox1;

		private global::Gtk.HBox hbox3;

		private global::Gtk.HBox hbox4;

		private global::Gtk.ScrolledWindow scrolledwindow1;

		private global::Gtk.HBox hbox5;

		private global::Gtk.VBox vbox2;

		private global::Gtk.Label calcResultOutputLine1_1;

		private global::Gtk.Label calcResultOutputLine2;

		private global::Gtk.VBox vbox3;

		private global::Gtk.Label calcResultOutputLine3;

		private global::Gtk.Label calcResultOutputLine4;

		private global::Gtk.VBox vbox4;

		private global::Gtk.Label calcResultOutputLine1_2;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget frz.MainWindow
			this.UIManager = new global::Gtk.UIManager();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup("Default");
			this.AnsichtAction = new global::Gtk.Action("AnsichtAction", "Ansicht", null, null);
			this.AnsichtAction.ShortLabel = "Ansicht";
			w1.Add(this.AnsichtAction, null);
			this.DateiAction = new global::Gtk.Action("DateiAction", "Datei", null, null);
			this.DateiAction.ShortLabel = "Datei";
			w1.Add(this.DateiAction, null);
			this.HilfeAction = new global::Gtk.Action("HilfeAction", "Hilfe", null, null);
			this.HilfeAction.ShortLabel = "Hilfe";
			w1.Add(this.HilfeAction, null);
			this.WortlisteFfnenAction = new global::Gtk.Action("WortlisteFfnenAction", "Wortliste öffnen", null, null);
			this.WortlisteFfnenAction.ShortLabel = "Wortliste öffnen";
			w1.Add(this.WortlisteFfnenAction, null);
			this.ErgebnisInDateiSpeichernAction = new global::Gtk.Action("ErgebnisInDateiSpeichernAction", "Ergebnis in Datei speichern", null, null);
			this.ErgebnisInDateiSpeichernAction.ShortLabel = "Ergebnis in Datei speichern";
			w1.Add(this.ErgebnisInDateiSpeichernAction, null);
			this.SchlieenAction = new global::Gtk.Action("SchlieenAction", "Schließen", null, null);
			this.SchlieenAction.ShortLabel = "Schließen";
			w1.Add(this.SchlieenAction, null);
			this.NormalansichtAction = new global::Gtk.Action("NormalansichtAction", "Normalansicht", null, null);
			this.NormalansichtAction.ShortLabel = "Normalanischt";
			w1.Add(this.NormalansichtAction, null);
			this.DokumentationAction = new global::Gtk.Action("DokumentationAction", "Dokumentation", null, null);
			this.DokumentationAction.ShortLabel = "Dokumentation";
			w1.Add(this.DokumentationAction, null);
			this.AboutAction = new global::Gtk.Action("AboutAction", "About", null, null);
			this.AboutAction.ShortLabel = "About";
			w1.Add(this.AboutAction, null);
			this.ExtrasAction = new global::Gtk.Action("ExtrasAction", "Extras", null, null);
			this.ExtrasAction.ShortLabel = "Extras";
			w1.Add(this.ExtrasAction, null);
			this.openAction = new global::Gtk.Action("openAction", null, "Testliste öffnen", "gtk-open");
			w1.Add(this.openAction, null);
			this.saveAction = new global::Gtk.Action("saveAction", null, "Ausgabe speichern", "gtk-save");
			w1.Add(this.saveAction, null);
			this.LeerenAction = new global::Gtk.Action("LeerenAction", "Leeren", null, null);
			this.LeerenAction.ShortLabel = "Leeren";
			w1.Add(this.LeerenAction, null);
			this.deleteAction = new global::Gtk.Action("deleteAction", "_Löschen", "Ausgabefenster leeren", "gtk-delete");
			this.deleteAction.ShortLabel = "_Löschen";
			w1.Add(this.deleteAction, null);
			this.clearAction = new global::Gtk.Action("clearAction", null, null, "gtk-clear");
			w1.Add(this.clearAction, null);
			this.expandAction = new global::Gtk.ToggleAction("expandAction", null, "Alle Details anzeigen", "gtk-add");
			w1.Add(this.expandAction, null);
			this.BearbeitenAction = new global::Gtk.Action("BearbeitenAction", "Bearbeiten", null, null);
			this.BearbeitenAction.ShortLabel = "Bearbeiten";
			w1.Add(this.BearbeitenAction, null);
			this.TextdateienNeuEinlesenAction = new global::Gtk.Action("TextdateienNeuEinlesenAction", "Textdateien neu einlesen", null, null);
			this.TextdateienNeuEinlesenAction.ShortLabel = "Textdateien neu einlesen";
			w1.Add(this.TextdateienNeuEinlesenAction, null);
			this.diacriticsToggleAction = new global::Gtk.ToggleAction("diacriticsToggleAction", null, "Eingabe mit/ohne Diakritika", "gtk-strikethrough");
			this.diacriticsToggleAction.Active = true;
			w1.Add(this.diacriticsToggleAction, null);
			this.ModusAction = new global::Gtk.Action("ModusAction", "Modus", null, null);
			this.ModusAction.ShortLabel = "Modus";
			w1.Add(this.ModusAction, null);
			this.LautentwicklungAction = new global::Gtk.RadioAction("LautentwicklungAction", "Lautentwicklung", null, null, 0);
			this.LautentwicklungAction.Group = new global::GLib.SList(global::System.IntPtr.Zero);
			this.LautentwicklungAction.ShortLabel = "Lautentwicklungen";
			w1.Add(this.LautentwicklungAction, null);
			this.TranskriptionDerZielspracheAction = new global::Gtk.RadioAction("TranskriptionDerZielspracheAction", "Transkription der Zielsprache", null, null, 0);
			this.TranskriptionDerZielspracheAction.Group = this.LautentwicklungAction.Group;
			this.TranskriptionDerZielspracheAction.ShortLabel = "Transkription in der Zielsprache";
			w1.Add(this.TranskriptionDerZielspracheAction, null);
			this.WortformerkennungDerQuellspracheAction = new global::Gtk.RadioAction("WortformerkennungDerQuellspracheAction", "Wortformerkennung der Quellsprache", null, null, 0);
			this.WortformerkennungDerQuellspracheAction.Group = this.LautentwicklungAction.Group;
			this.WortformerkennungDerQuellspracheAction.ShortLabel = "Wortformerkennung der Quellsprache";
			w1.Add(this.WortformerkennungDerQuellspracheAction, null);
			this.WortformerkennungDerZielspracheAction = new global::Gtk.RadioAction("WortformerkennungDerZielspracheAction", "Wortformerkennung der Zielsprache", null, null, 0);
			this.WortformerkennungDerZielspracheAction.Group = this.LautentwicklungAction.Group;
			this.WortformerkennungDerZielspracheAction.ShortLabel = "Wortformerkennung der Zielsprache";
			w1.Add(this.WortformerkennungDerZielspracheAction, null);
			this.Action = new global::Gtk.RadioAction("Action", null, null, null, 0);
			this.Action.Group = this.WortformerkennungDerZielspracheAction.Group;
			w1.Add(this.Action, null);
			this.TranskriptionDerQuellspracheAction = new global::Gtk.RadioAction("TranskriptionDerQuellspracheAction", "Transkription der Quellsprache", null, null, 0);
			this.TranskriptionDerQuellspracheAction.Group = this.WortformerkennungDerZielspracheAction.Group;
			this.TranskriptionDerQuellspracheAction.ShortLabel = "Transkription der Quellsprache";
			w1.Add(this.TranskriptionDerQuellspracheAction, null);
			this.UIManager.InsertActionGroup(w1, 0);
			this.AddAccelGroup(this.UIManager.AccelGroup);
			this.Name = "frz.MainWindow";
			this.Title = "CEW";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child frz.MainWindow.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 1;
			// Container child vbox1.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString(@"<ui><menubar name='menubar1'><menu name='DateiAction' action='DateiAction'><menuitem name='WortlisteFfnenAction' action='WortlisteFfnenAction'/><menuitem name='ErgebnisInDateiSpeichernAction' action='ErgebnisInDateiSpeichernAction'/><menuitem name='SchlieenAction' action='SchlieenAction'/></menu><menu name='BearbeitenAction' action='BearbeitenAction'><menuitem name='TextdateienNeuEinlesenAction' action='TextdateienNeuEinlesenAction'/></menu><menu name='AnsichtAction' action='AnsichtAction'><menuitem name='LeerenAction' action='LeerenAction'/></menu><menu name='HilfeAction' action='HilfeAction'><menuitem name='AboutAction' action='AboutAction'/></menu></menubar></ui>");
			this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget("/menubar1")));
			this.menubar1.Name = "menubar1";
			this.vbox1.Add(this.menubar1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.menubar1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(5));
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 4;
			// Container child hbox2.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString(@"<ui><toolbar name='toolbar1'><toolitem name='openAction' action='openAction'/><toolitem name='saveAction' action='saveAction'/><toolitem name='deleteAction' action='deleteAction'/><toolitem name='expandAction' action='expandAction'/><toolitem name='diacriticsToggleAction' action='diacriticsToggleAction'/></toolbar></ui>");
			this.toolbar1 = ((global::Gtk.Toolbar)(this.UIManager.GetWidget("/toolbar1")));
			this.toolbar1.Name = "toolbar1";
			this.toolbar1.ShowArrow = false;
			this.toolbar1.ToolbarStyle = ((global::Gtk.ToolbarStyle)(0));
			this.toolbar1.IconSize = ((global::Gtk.IconSize)(2));
			this.hbox2.Add(this.toolbar1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.toolbar1]));
			w3.Position = 0;
			// Container child hbox2.Gtk.Box+BoxChild
			this.modusLabel = new global::Gtk.Label();
			this.modusLabel.Name = "modusLabel";
			this.modusLabel.LabelProp = "Modus:";
			this.hbox2.Add(this.modusLabel);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.modusLabel]));
			w4.Position = 1;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.comboboxModus = global::Gtk.ComboBoxEntry.NewText();
			this.comboboxModus.AppendText("Lautentwicklung");
			this.comboboxModus.AppendText("Transkription der Quellsprache");
			this.comboboxModus.AppendText("Transkription der Zielsprache");
			this.comboboxModus.AppendText("Wortformerkennung der Quellsprache");
			this.comboboxModus.AppendText("Wortformerkennung der Zielsprache");
			this.comboboxModus.TooltipMarkup = "Modus der Analyse";
			this.comboboxModus.WidthRequest = 255;
			this.comboboxModus.Name = "comboboxModus";
			this.comboboxModus.Active = 0;
			this.hbox2.Add(this.comboboxModus);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.comboboxModus]));
			w5.Position = 2;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.entry1 = new global::Gtk.Entry();
			this.entry1.CanFocus = true;
			this.entry1.Name = "entry1";
			this.entry1.IsEditable = true;
			this.entry1.InvisibleChar = '●';
			this.hbox2.Add(this.entry1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.entry1]));
			w6.Position = 3;
			w6.Expand = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.Start = new global::Gtk.Button();
			this.Start.TooltipMarkup = "Analyse starten";
			this.Start.CanFocus = true;
			this.Start.Name = "Start";
			this.Start.UseUnderline = true;
			this.Start.Label = "Start";
			this.hbox2.Add(this.Start);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.Start]));
			w7.Position = 4;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox1.Add(this.hbox2);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox2]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			w8.Padding = ((uint)(5));
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Homogeneous = true;
			this.vbox1.Add(this.hbox1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
			w9.Position = 2;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox();
			this.hbox3.Name = "hbox3";
			this.hbox3.Homogeneous = true;
			this.vbox1.Add(this.hbox3);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox3]));
			w10.Position = 3;
			w10.Expand = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox4 = new global::Gtk.HBox();
			this.hbox4.Name = "hbox4";
			this.vbox1.Add(this.hbox4);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox4]));
			w11.Position = 4;
			w11.Expand = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow();
			this.scrolledwindow1.CanFocus = true;
			this.scrolledwindow1.Name = "scrolledwindow1";
			this.scrolledwindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			this.vbox1.Add(this.scrolledwindow1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.scrolledwindow1]));
			w12.Position = 5;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox5 = new global::Gtk.HBox();
			this.hbox5.Name = "hbox5";
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox();
			this.vbox2.WidthRequest = 250;
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.calcResultOutputLine1_1 = new global::Gtk.Label();
			this.calcResultOutputLine1_1.WidthRequest = 0;
			this.calcResultOutputLine1_1.Name = "calcResultOutputLine1_1";
			this.vbox2.Add(this.calcResultOutputLine1_1);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.calcResultOutputLine1_1]));
			w13.Position = 0;
			w13.Expand = false;
			w13.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.calcResultOutputLine2 = new global::Gtk.Label();
			this.calcResultOutputLine2.Name = "calcResultOutputLine2";
			this.vbox2.Add(this.calcResultOutputLine2);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.calcResultOutputLine2]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			this.hbox5.Add(this.vbox2);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.vbox2]));
			w15.Position = 0;
			w15.Expand = false;
			w15.Fill = false;
			w15.Padding = ((uint)(10));
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox();
			this.vbox3.WidthRequest = 250;
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.calcResultOutputLine3 = new global::Gtk.Label();
			this.calcResultOutputLine3.WidthRequest = 400;
			this.calcResultOutputLine3.Name = "calcResultOutputLine3";
			this.vbox3.Add(this.calcResultOutputLine3);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.calcResultOutputLine3]));
			w16.Position = 0;
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.calcResultOutputLine4 = new global::Gtk.Label();
			this.calcResultOutputLine4.Name = "calcResultOutputLine4";
			this.vbox3.Add(this.calcResultOutputLine4);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.calcResultOutputLine4]));
			w17.Position = 1;
			w17.Expand = false;
			w17.Fill = false;
			this.hbox5.Add(this.vbox3);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.vbox3]));
			w18.Position = 1;
			w18.Expand = false;
			w18.Fill = false;
			w18.Padding = ((uint)(10));
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox();
			this.vbox4.WidthRequest = 250;
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.calcResultOutputLine1_2 = new global::Gtk.Label();
			this.calcResultOutputLine1_2.Name = "calcResultOutputLine1_2";
			this.vbox4.Add(this.calcResultOutputLine1_2);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox4[this.calcResultOutputLine1_2]));
			w19.Position = 0;
			w19.Expand = false;
			w19.Fill = false;
			this.hbox5.Add(this.vbox4);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.vbox4]));
			w20.Position = 2;
			w20.Expand = false;
			w20.Fill = false;
			w20.Padding = ((uint)(10));
			this.vbox1.Add(this.hbox5);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox5]));
			w21.Position = 6;
			w21.Expand = false;
			w21.Fill = false;
			w21.Padding = ((uint)(5));
			this.Add(this.vbox1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 822;
			this.DefaultHeight = 459;
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
			this.WortlisteFfnenAction.Activated += new global::System.EventHandler(this.OnOpen);
			this.ErgebnisInDateiSpeichernAction.Activated += new global::System.EventHandler(this.OnSave);
			this.SchlieenAction.Activated += new global::System.EventHandler(this.OnExit);
			this.AboutAction.Activated += new global::System.EventHandler(this.OnAbout);
			this.openAction.Activated += new global::System.EventHandler(this.OnOpen);
			this.saveAction.Activated += new global::System.EventHandler(this.OnSave);
			this.LeerenAction.Activated += new global::System.EventHandler(this.OnClear);
			this.deleteAction.Activated += new global::System.EventHandler(this.OnClear);
			this.clearAction.Activated += new global::System.EventHandler(this.OnClear);
			this.expandAction.Toggled += new global::System.EventHandler(this.OnExpand);
			this.TextdateienNeuEinlesenAction.Activated += new global::System.EventHandler(this.OnRereadFiles);
			this.diacriticsToggleAction.Toggled += new global::System.EventHandler(this.OnDiacritics);
			this.LautentwicklungAction.Changed += new global::Gtk.ChangedHandler(this.OnModusAction);
			this.TranskriptionDerZielspracheAction.Changed += new global::Gtk.ChangedHandler(this.OnModusAction);
			this.WortformerkennungDerQuellspracheAction.Changed += new global::Gtk.ChangedHandler(this.OnModusAction);
			this.WortformerkennungDerZielspracheAction.Changed += new global::Gtk.ChangedHandler(this.OnModusAction);
			this.TranskriptionDerQuellspracheAction.Changed += new global::Gtk.ChangedHandler(this.OnModusAction);
			this.comboboxModus.Changed += new global::System.EventHandler(this.OnModusAction);
		}
	}
}
