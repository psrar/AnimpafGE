using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AnimpafGE.Physics
{
	public enum RigidType
	{
		/// <summary>К статическим объектам не применяется никакая сила</summary>
		Static = 0,
		/// <summary>Динамические объекты перемещаются свободно под действием внешних сил или кода</summary>
		Dynamic = 1
	}
	static public class Physics
	{
		static public void DrawLineBresenham(int x, int y, int x2, int y2)
		{
			int w = x2 - x;
			int h = y2 - y;
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if(w < 0) dx1 = -1;
			else if(w > 0) dx1 = 1;

			if(h < 0) dy1 = -1;
			else if(h > 0) dy1 = 1;

			dx2 = dx1;

			int longest = Math.Abs(w);
			int shortest = Math.Abs(h);
			if(!(longest > shortest))
			{
				longest = Math.Abs(h);
				shortest = Math.Abs(w);
				if(h < 0) dy2 = -1; else if(h > 0) dy2 = 1;
				dx2 = 0;
			}
			int numerator = longest >> 1;
			for(int i = 0; i <= longest; i++)
			{
				putpixel(x, y, color);
				numerator += shortest;
				if(!(numerator < longest))
				{
					numerator -= longest;
					x += dx1;
					y += dy1;
				}
				else
				{
					x += dx2;
					y += dy2;
				}
			}
		}

		public struct Line
		{
			Point StartPoint, EndPoint;
			public static Line Zero { get; } = new Line(0, 0, 0, 0);

			public Line(Point startPoint, Point endPoint)
			{
				StartPoint = startPoint;
				EndPoint = endPoint;
			}
			public Line(Vector2 startPoint, Vector2 endPoint)
			{
				StartPoint = startPoint.ToPoint();
				EndPoint = endPoint.ToPoint();
			}
			public Line(int x1, int y1, int x2, int y2)
			{
				StartPoint = new Point(x1, y1);
				EndPoint = new Point(x2, y2);
			}
		}

		public struct BoxCollider
		{
			Rectangle Rectangle { get; set; }

			public BoxCollider(int x, int y, int width, int height) => Rectangle = new Rectangle(x, y, width, height);
			public BoxCollider(Point position, Point size) => Rectangle = new Rectangle(position, size);
			public BoxCollider(Vector2 position, Vector2 size) => Rectangle = new Rectangle(position.ToPoint(), size.ToPoint());

			public enum Borders
			{
				Up, Right, Down, Left
			}
			public Line GetBorder(Borders border)
			{
				return border switch
				{
					Borders.Up => new Line(Rectangle.X, Rectangle.Y, Rectangle.Right, Rectangle.Top),
					Borders.Right => new Line(Rectangle.Right, Rectangle.Top, Rectangle.Right, Rectangle.Bottom),
					Borders.Down => new Line(Rectangle.Left, Rectangle.Bottom, Rectangle.Right, Rectangle.Bottom),
					Borders.Left => new Line(Rectangle.Left, Rectangle.Bottom, Rectangle.Left, Rectangle.Y),
					_ => new Line(),
				};
			}
		}

	}
}
