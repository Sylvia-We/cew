using Gtk;
using Gdk;

namespace frz
{
	public class Splash : Gtk.Window
	{
		public Splash() : base(Gtk.WindowType.Toplevel)
		{

			/* set background image */
			Gdk.Pixbuf one = new Gdk.Pixbuf(Functions.logoFile);

			Gtk.VBox vbox = new Gtk.VBox();

			Gtk.Image image = new Gtk.Image(one);
			vbox.Add(image);

			Label startLabel = new Label();
			startLabel.Text = "CEW startet...";
			vbox.Add(startLabel);

			this.Add(vbox);

			this.SetSizeRequest(500, 300);
			this.WindowPosition = WindowPosition.Center;
			this.Decorated = false;
			this.HasFocus = false;
			this.HasFrame = false;
			this.KeepAbove = true;
			this.Modal = false;
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			this.TypeHint = Gdk.WindowTypeHint.Splashscreen;
			this.ShowAll();

			Hourglass = true;
			while (Application.EventsPending())
			{
				Application.RunIteration();
			}
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
	}
}

