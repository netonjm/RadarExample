using System;
using System.Drawing;

namespace RadarExample.WinForms
{
	static class PointExtensions
	{
		public static int Distance (this Point sender, Point point)
		{
			return (int)Math.Round (Math.Sqrt (Math.Pow ((sender.X - point.X), 2) + Math.Pow ((sender.Y - point.Y), 2)));
		}

		public static bool Intresects (this Point sender, Point lineInit, Point lineEnd)
		{
			return Distance (lineInit, sender) + Distance (lineEnd, sender) == Distance (lineInit, lineEnd);
		}

		public static Point Distance (this Point p1, Point p2, int distance)
		{
			double len = p1.Distance (p2);
			double ratio = distance / len;
			int x = (int)Math.Round (ratio * p2.X + (1.0 - ratio) * p1.X);
			int y = (int)Math.Round (ratio * p2.Y + (1.0 - ratio) * p1.Y);
			return new Point (x, y);
		}
	}
}
