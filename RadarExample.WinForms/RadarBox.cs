using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RadarExample.WinForms
{
	class RadarBox : PictureBox, IDisposable
	{
		readonly Pen p = new Pen (Color.Green, 1f);
		readonly Pen pCollision = new Pen (Color.Red, 1f);

		public List<Point> Points = new List<Point> ();
		List<Point> drawPoints = new List<Point> ();
		Point Center = new Point ();

		public int Angle { get; set; }
		public int Distance { get; private set; }

		int HAND;
		int x, y;       //HAND coordinate
		int tx, ty, lim = 20;

		int lines = 4;
		int lineSeparation;

		Bitmap bmp;
		Graphics g;

		Font drawFont = new Font ("Arial", 8);
		SolidBrush drawBrush = new SolidBrush (Color.White);
		StringFormat drawFormat = new StringFormat ();

		public void Draw ()
		{
			g = Graphics.FromImage (bmp);
			g.Clear (Color.Black);

			DrawRadar ();
			DrawHand ();

			Image = bmp;
			g.Dispose ();
		}

		void DrawRadar ()
		{
			//draw circle
			for (int i = 0; i < lines; i++) {
				var item = lineSeparation * i;
				g.DrawEllipse (p, item / 2, item / 2, Width - item, Height - item);
				g.DrawString (string.Format("{0}cm", (10 * i)), drawFont, drawBrush, Center.X + (item/2) + 5, Center.Y - 17, drawFormat);
			}

			//draw perpendicular line
			g.DrawLine (p, new Point (Center.X, 0), new Point (Center.X, Height)); // UP-DOWN
			g.DrawLine (p, new Point (0, Center.Y), new Point (Width, Center.Y)); //LEFT-RIGHT

			g.DrawString (string.Format ("Object: {0}", "Out of Range"), drawFont, drawBrush, 15, 10, drawFormat);
			g.DrawString (string.Format ("Angle: {0}º", 450 - Angle), drawFont, drawBrush, 15, 30, drawFormat);

			g.DrawString (string.Format ("Distance: {0}cm", Distance), drawFont, drawBrush, 15, 50, drawFormat);
		}

		void DrawHand ()
		{
			//calculate x, y coordinate of HAND
			int tu = (Angle - lim) % 360;

			if (Angle >= 0 && Angle <= 180) {
				//right half
				//u in degree is converted into radian.

				x = Center.X + (int)(HAND * Math.Sin (Math.PI * Angle / 180));
				y = Center.Y - (int)(HAND * Math.Cos (Math.PI * Angle / 180));
			} else {
				x = Center.X - (int)(HAND * -Math.Sin (Math.PI * Angle / 180));
				y = Center.Y - (int)(HAND * Math.Cos (Math.PI * Angle / 180));
			}

			if (tu >= 0 && tu <= 180) {
				//right half
				//tu in degree is converted into radian.
				tx = Center.X + (int)(HAND * Math.Sin (Math.PI * tu / 180));
				ty = Center.Y - (int)(HAND * Math.Cos (Math.PI * tu / 180));
			} else {
				tx = Center.X - (int)(HAND * -Math.Sin (Math.PI * tu / 180));
				ty = Center.Y - (int)(HAND * Math.Cos (Math.PI * tu / 180));
			}

			var lineStart = new Point (Center.X, Center.Y);
			var lineEnd = new Point (x, y);

			int lowerDistance = Height;
			drawPoints.Clear ();

			Distance = Height - lowerDistance;

			for (int i = 0; i < Points.Count; i++) {
				var distance = Points [i].Distance (Center);

				if (Points [i].Intresects (lineStart, lineEnd)) {
					if (distance < lowerDistance) {
						lowerDistance = distance;
						Distance = Height - lowerDistance;
					}
					drawPoints.Add (Points [i]);
				}
			}

			if (lowerDistance == Height) {
				g.DrawLine (p, lineStart, lineEnd);
			} else {
				var point = lineStart.Distance (lineEnd, lowerDistance);
				g.DrawLine (p, lineStart, point);
				g.DrawLine (pCollision, point, lineEnd);
			}

			for (int i = 0; i < Points.Count; i++) {
				g.DrawEllipse (drawPoints.Contains (Points [i]) ? Pens.Red : p, Points [i].X - 2, Points [i].Y - 2, 5, 5);
			}
		}

		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);
			ChangeSize ();
		}

		public void ChangeSize ()
		{
			lineSeparation = Height / lines;

			//create Bitmap
			bmp = new Bitmap (Width, Height);

			//graphics
			g = Graphics.FromImage (bmp);

			//center
			Center.X = Width / 2;
			Center.Y = Height / 2;

			//initial degree of HAND
			HAND = Center.X;
		}

		protected override void Dispose (bool disposing)
		{
			p.Dispose ();
			base.Dispose (disposing);
		}
	}
}
