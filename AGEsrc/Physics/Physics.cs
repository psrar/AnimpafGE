using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AGE.Physics
{
	public static class PhysicalConstants
	{
		static public Vector2 Gravity = new Vector2(0, 9800 / 1.6f);
	}

	public enum RigidType
	{
		/// <summary>К статическим объектам не применяется никакая сила</summary>
		Static = 0,
		/// <summary>Динамические объекты перемещаются свободно под действием внешних сил или кода</summary>
		Dynamic = 1
	}
	public enum Side
	{
		None = -1,
		Top = 0,
		TopRight = 1,
		Right = 2,
		BottomRight = 3,
		Bottom = 4,
		BottomLeft = 5,
		Left = 6,
		TopLeft = 7
	}

	static public class Physics
	{
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
	}
}
