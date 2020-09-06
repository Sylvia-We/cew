using Gtk;

namespace frz
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();



			MainWindow win = new MainWindow();
			Splash spl = new Splash();
			Functions.SetMW(win); /* win in Functions bekannt machen*/
			ReadFiles.SetMW(win);

			Functions.Preprocess();


			/* Modus-Menü */
			Gtk.Action ModusAction = new Gtk.Action("ModusAction", "Modus", null, null);
			ModusAction.ShortLabel = "Modus";


			win.Show();
			spl.Destroy();
			Application.Run();
		}
	}
}
