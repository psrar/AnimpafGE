using System;
using System.Collections.Generic;
using System.Text;

namespace AGE
{
	static public class Easing
	{
		static public float EaseInOutQuad(float t)
		{
			return t < 0.5f ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
		}
	}
}
