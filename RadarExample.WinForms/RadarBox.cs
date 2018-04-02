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

		int Radius;
		int x, y;       //HAND coordinate
		int lim = 20;

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

			lastAngle = Angle;
		}

		static Point GetPointFromAngle (Point center, int radius, int angle, int lim)
		{
			int tu = (angle - lim) % 360;
			if (angle >= 0 && angle <= 180) {
				return new Point (
					center.X + (int)(radius * Math.Sin (Math.PI * angle / 180)),
					center.Y - (int)(radius * Math.Cos (Math.PI * angle / 180))
				);
			} 

			return new Point (
				center.X - (int)(radius * -Math.Sin (Math.PI * angle / 180)),
				center.Y - (int)(radius * Math.Cos (Math.PI * angle / 180))
			);
		}

		int GetCorrectedAngle (int angle) => angle >= 360 ? angle - 360 : angle;

		void DrawRadar ()
		{
			//draw circle
			for (int i = 0; i < lines; i++) {
				var item = lineSeparation * i;
				g.DrawEllipse (p, item / 2, item / 2, Width - item, Height - item);
				g.DrawString (string.Format("{0}cm", (10 * i)), drawFont, drawBrush, Center.X + (item/2) + 5, Center.Y - 17, drawFormat);
			}

			Point topPoint;
			for (int i = 270; i <= (270 + (30 * 9)); i += 30) {
				topPoint = GetPointFromAngle (Center, Radius, i, lim);
				g.DrawLine (p, new Point (Center.X, Center.Y), topPoint);
				g.DrawString (string.Format ("{0}º", GetCorrectedAngle (i)), drawFont, drawBrush, topPoint, drawFormat);
			}

			g.DrawString (string.Format ("Object: {0}", "Out of Range"), drawFont, drawBrush, 15, 10, drawFormat);
			g.DrawString (string.Format ("Angle: {0}º",  GetCorrectedAngle(Angle)), drawFont, drawBrush, 15, 30, drawFormat);
			g.DrawString (string.Format ("Distance: {0}cm", Distance), drawFont, drawBrush, 15, 50, drawFormat);
		}

		int lastAngle;

		void DrawHand ()
		{
			var lineStart = new Point (Center.X, Center.Y);
			var lineEnd = GetPointFromAngle (Center, Radius, Angle, lim);

			int lowerDistance = Height;
			drawPoints.Clear ();

			Distance = Height - lowerDistance;

			for (int i = 0; i < Points.Count; i++) {
				var distance = Points [i].Distance (Center);

				if (Points [i].Intresects (lineStart, lineEnd) ) {
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

			Radius = Math.Min (Width, Height) / 2;

			//center
			Center.X = Radius;
			Center.Y = Radius;
		}

		protected override void Dispose (bool disposing)
		{
			p.Dispose ();
			base.Dispose (disposing);
		}
	}
}
