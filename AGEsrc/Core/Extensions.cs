using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AGE
{
	static class Extensions
	{
		//Math
		/// <summary>
		/// Устанавливает компонент вектора.
		/// </summary>
		/// <param name="axis">X,x или Y,y</param>
		/// <param name="value">Новое значение указанной координаты</param>
		static public Vector2 Set(ref this Vector2 vector, char axis, float value)
		{
			if(axis == 'X' || axis == 'x')
				vector = new Vector2(value, vector.Y);
			else if(axis == 'Y' || axis == 'y')
				vector = new Vector2(vector.X, value);
			else throw new Exception($"Incorrect axis {axis}");

			return vector;
		}
	}
}
