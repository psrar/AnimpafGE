using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AGE
{
	static public class Extensions
	{
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

		static public float Distance(this Point point, Point destination) =>
			MathF.Sqrt(MathF.Pow(point.X - destination.X, 2) + MathF.Pow(point.Y - destination.Y, 2));

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

		public static T[] Populate<T>(this T[] arr, T value)
		{
			for(int i = 0; i < arr.Length; i++)
				arr[i] = value;

			return arr;
		}

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position,
			float scale, Color color)
		{
			spriteBatch.Draw(texture, position, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position,
			float scale, Vector2 origin, Color color)
		{
			spriteBatch.Draw(texture, position, null, color, 0, origin, scale, SpriteEffects.None, 0);
		}

		public static void SetPixel(this Texture2D texture, int x, int y, Color color)
		{
			Rectangle r = new Rectangle(x, y, 1, 1);
			Color[] c = new Color[1];
			c[0] = color;

			texture.SetData<Color>(0, r, c, 0, 1);
		}

		public static void Fill(this Texture2D texture, Color color) =>
			texture.SetData<uint>(new uint[texture.Width * texture.Height].Populate<uint>(color.PackedValue));

		public static float Cross(this Vector2 a, Vector2 v) =>
			a.X * v.Y - a.Y * v.X;

		public static bool IsZero(this double d) =>
			Math.Abs(d) < 1e-10; //1e - 10 - accuracy
	}
}
