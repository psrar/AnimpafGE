using System;
using Microsoft.Xna.Framework;

namespace AGE
{
	static public class Utilities
	{
		static public Color GetRandomColor()
		{
			Random rnd = new Random();
			return new Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
		}
		static public Color GetRandomColor(Color baseColor)
		{
			Random rnd = new Random();
			int rndInt = rnd.Next(40);
			return new Color(baseColor.R + rndInt, baseColor.G + rndInt, baseColor.B + rndInt);
		}

		/// <summary>
		/// a and b are inclusive int vectors.
		/// </summary>
		static public Vector2 GetRandomPosition(Vector2 a, Vector2 b)
		{
			Random rnd = new Random();
			int minX = (int)MathF.Min(a.X, b.X);
			int maxX = (int)MathF.Max(a.X, b.X);
			int minY = (int)MathF.Min(a.Y, b.Y);
			int maxY = (int)MathF.Max(a.Y, b.Y);
			return new Vector2(rnd.Next(minX, maxX + 1), rnd.Next(minY, maxY + 1));
		}
	}

}
