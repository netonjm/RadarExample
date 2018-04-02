using System;
using System.Drawing;
using System.Windows.Forms;

namespace RadarExample.WinForms
{
	public partial class Form1 : Form
	{
		Timer t = new Timer ();
		RadarBox radar;
		Random rnd = new Random (DateTime.Now.Millisecond);
		bool inverse;

		public Form1 ()
		{
			InitializeComponent ();

			this.Width = 700;
			this.Height = 700;

			//background color
			BackColor = Color.Black;

			radar = new RadarBox () { Angle = 275 };
			radar.Size = new Size (Width + 10, Height + 10);

			Controls.Add (radar);

			this.Height = 430;

			//Creation of some test points
			for (int i = 0; i < 70; i++) {
				radar.Points.Add (new Point (rnd.Next (30, Width-50), rnd.Next (30, Height/2)));
			}

			radar.MouseClick += (s, e) => radar.Angle = e.X;

			Resize += (s, e) => ResizeRadar ();

			ResizeRadar ();

			//timer
			t.Interval = 110; //in millisecond
			t.Tick += OnTick;
			t.Start ();
		}

		void ResizeRadar () 
		{
			radar.Location = new Point ((Width / 2) - (radar.Width / 2), Height - 30 - (radar.Width / 2));
		}

		void OnTick (object sender, EventArgs e)
		{
			radar.Draw ();

			if (inverse) {
				radar.Angle--;
			} else {
				radar.Angle++;
			}

			if (radar.Angle == 450) {
				inverse = true;
			} else if (radar.Angle == 275) {
				inverse = false;
			}
		}
	}
}
