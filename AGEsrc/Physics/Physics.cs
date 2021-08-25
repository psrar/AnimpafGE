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

	static public class Physics2D
	{
        public static bool LineLineIntersect(Vector2 a, Vector2 b, Vector2 c,
            Vector2 d, out Vector2 point)
        {
            point = Vector2.Zero;

            double r, s;
            double denominator = (b.X - a.X) * (d.Y - c.Y) - (b.Y - a.Y) * (d.X - c.X);

            // If the denominator in above is zero, AB & CD are colinear
            if(denominator == 0)
            {
                return false;
            }

            double numeratorR = (a.Y - c.Y) * (d.X - c.X) - (a.X - c.X) * (d.Y - c.Y);
            r = numeratorR / denominator;

            double numeratorS = (a.Y - c.Y) * (b.X - a.X) - (a.X - c.X) * (b.Y - a.Y);
            s = numeratorS / denominator;

            // non-intersecting
            if(r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            // find intersection point
            point.X = (float)(a.X + (r * (b.X - a.X)));
            point.Y = (float)(a.Y + (r * (b.Y - a.Y)));

            return true;
        }

    }
}
