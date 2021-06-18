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
	}

}
