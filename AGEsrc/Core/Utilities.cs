using System;
using Microsoft.Xna.Framework;

namespace AnimpafGE
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
			int rndInt = rnd.Next(20);
			return new Color(baseColor.R + rndInt, baseColor.G + rndInt, baseColor.B + rndInt);
		}

		/// <summary>
		/// Min and max are inclusive int vectors.
		/// </summary>
		/// <param name="max"></param>
		/// <param name="min"></param>
		/// <returns></returns>
		static public Vector2 GetRandomPosition(Vector2 min, Vector2 max)
		{
			Random rnd = new Random();
			return new Vector2(rnd.Next((int)min.X, (int)max.X + 1), rnd.Next((int)min.Y, (int)max.Y + 1));
		}
	}

}
