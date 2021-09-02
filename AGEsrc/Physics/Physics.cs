using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AGE.Graphics;

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

	static public class Physics2D
	{
		public static bool LinesIntersection(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, out Vector2 intersectionPoint)
		{
			// Line AB represented as a1x + b1y = c1 
			float a1 = e1.Y - s1.Y;
			float b1 = s1.X - e1.X;
			float c1 = a1 * (s1.X) + b1 * (s1.Y);

			// Line CD represented as a2x + b2y = c2 
			float a2 = e2.Y - s2.Y;
			float b2 = s2.X - e2.X;
			float c2 = a2 * (s2.X) + b2 * (s2.Y);

			float determinant = a1 * b2 - a2 * b1;

			if(determinant == 0)
			{
				intersectionPoint = Vector2.Zero;
				return false;
			}
			else
			{
				float x = (b2 * c1 - b1 * c2) / determinant;
				float y = (a1 * c2 - a2 * c1) / determinant;
				intersectionPoint = new Vector2(x, y);
				return true;
			}
		}

		public static bool LineSegmentsIntersection(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2)
		{
			bool isIntersecting = false;

			float denominator = (e2.Y - s2.Y) * (e1.X - s1.X) - (e2.X - s2.X) * (e1.Y - s1.Y);

			//Make sure the denominator is > 0, if so the lines are parallel
			if(denominator != 0)
			{
				float u_a = ((e2.X - s2.X) * (s1.Y - s2.Y) - (e2.Y - s2.Y) * (s1.X - s2.X)) / denominator;
				float u_b = ((e1.X - s1.X) * (s1.Y - s2.Y) - (e1.Y - s1.Y) * (s1.X - s2.X)) / denominator;

				//Is intersecting if u_a and u_b are between 0 and 1
				if(u_a >= 0 && u_a <= 1 && u_b >= 0 && u_b <= 1)
				{
					isIntersecting = true;
				}
			}

			return isIntersecting;
		}
		public static bool LineSegmentsIntersection(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, out Vector2 intersectionPoint)
		{
			if(LineSegmentsIntersection(s1, e1, s2, e2))
			{
				// Line AB represented as a1x + b1y = c1 
				float a1 = e1.Y - s1.Y;
				float b1 = s1.X - e1.X;
				float c1 = a1 * (s1.X) + b1 * (s1.Y);

				// Line CD represented as a2x + b2y = c2 
				float a2 = e2.Y - s2.Y;
				float b2 = s2.X - e2.X;
				float c2 = a2 * (s2.X) + b2 * (s2.Y);

				float determinant = a1 * b2 - a2 * b1;

				float x = (b2 * c1 - b1 * c2) / determinant;
				float y = (a1 * c2 - a2 * c1) / determinant;
				intersectionPoint = new Vector2(x, y);
				return true;
			}
			else
			{
				intersectionPoint = Vector2.Zero;
				return false;
			}
		}
		public static Vector2[] LineSegmentsIntersection(Vector2 s1, Vector2 e1, params (Vector2 s, Vector2 e)[] lines)
		{
			List<Vector2> intersections = new List<Vector2>();
			Vector2 i;
			foreach(var line in lines)
				if(LineSegmentsIntersection(s1, e1, line.s, line.e, out i))
					intersections.Add(i);

			return intersections.ToArray();
		}
		public static (Vector2 point, Edge2D edge)[] LineSegmentsIntersection(Vector2 s1, Vector2 e1, params Edge2D[] edges)
		{
			List<(Vector2 point, Edge2D edge)> intersections = new List<(Vector2 point, Edge2D edge)>();
			Vector2 i;
			foreach(var edge in edges)
				if(LineSegmentsIntersection(s1, e1, edge.StartPosition, edge.EndPosition, out i))
					intersections.Add((i, edge));

			return intersections.ToArray();
		}

		public static float FindDistanceToSegment(
	Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, out Vector2 closestPoint)
		{
			float dx = segmentEnd.X - segmentStart.X;
			float dy = segmentEnd.Y - segmentStart.Y;
			if((dx == 0) && (dy == 0))
			{
				closestPoint = segmentStart;
				return Vector2.Distance(closestPoint, point);
			}

			float t = ((point.X - segmentStart.X) * dx + (point.Y - segmentStart.Y) * dy) /
				(dx * dx + dy * dy);

			if(t < 0)
				closestPoint = segmentStart;
			else if(t > 1)
				closestPoint = segmentEnd;
			else
				closestPoint = new Vector2(segmentStart.X + t * dx, segmentStart.Y + t * dy);

			return Vector2.Distance(closestPoint, point);
		}
	}
}
