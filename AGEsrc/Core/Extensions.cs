using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AGE
{
	static public class Extensions
	{
		//Math
		/// <summary>Устанавливает компонент вектора.</summary>
		/// <param name="axis">X,x или Y,y</param>
		/// <param name="value">Новое значение указанной координаты</param>
		static public Vector2 SetAxis(ref this Vector2 vector, char axis, float value)
		{
			if(axis == 'X' || axis == 'x')
				vector = new Vector2(value, vector.Y);
			else if(axis == 'Y' || axis == 'y')
				vector = new Vector2(vector.X, value);
			else throw new Exception($"Incorrect axis {axis}");

			return vector;
		}

		static public Vector2 IncrementVector(Vector2 vector, float value)
			=> vector += new Vector2(value);
		static public Vector2 IncrementVector(ref Vector2 vector, float value) =>
			vector += new Vector2(value);

		/// <summary>
		/// Calculates the signed depth of intersection between two rectangles.
		/// </summary>
		/// <returns>
		/// The amount of overlap between two intersecting rectangles. These
		/// depth values can be negative depending on which wides the rectangles
		/// intersect. This allows callers to determine the correct direction
		/// to push objects in order to resolve collisions.
		/// If the rectangles are not intersecting, Vector2.Zero is returned.
		/// </returns>
		public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
		{
			// Calculate half sizes.
			float halfWidthA = rectA.Width / 2.0f;
			float halfHeightA = rectA.Height / 2.0f;
			float halfWidthB = rectB.Width / 2.0f;
			float halfHeightB = rectB.Height / 2.0f;

			// Calculate centers.
			Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
			Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

			// Calculate current and minimum-non-intersecting distances between centers.
			float distanceX = centerA.X - centerB.X;
			float distanceY = centerA.Y - centerB.Y;
			float minDistanceX = halfWidthA + halfWidthB;
			float minDistanceY = halfHeightA + halfHeightB;

			// If we are not intersecting at all, return (0, 0).
			if(Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
				return Vector2.Zero;

			// Calculate and return intersection depths.
			float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
			float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
			return new Vector2(depthX, depthY);
		}

		/// <summary>
		/// Gets the position of the center of the bottom edge of the rectangle.
		/// </summary>
		public static Vector2 GetBottomCenter(this Rectangle rect)
		{
			return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
		}
	}
}
