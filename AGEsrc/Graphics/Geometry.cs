using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AGE;
using AGE.ECS;
using System.IO;

namespace AGE.Graphics
{
	public class Vertex2D
	{
		public Point Position;

		public Vertex2D(Point position) => Position = position;

		public Vector2 Vector2Position() => Position.ToVector2();
	}

	public class Edge2D
	{
		public Vertex2D Start, End;
		public Vector2 StartPosition, EndPosition;

		public Edge2D(Vertex2D start, Vertex2D end)
		{
			Start = start;
			End = end;
			StartPosition = start.Vector2Position();
			EndPosition = end.Vector2Position();
		}
	}
}

public class PrimitivesHandler
{
	static readonly private Texture2D pixel = new Texture2D(Core.GraphicsManager.GraphicsDevice, 1, 1);
	static private Texture2D circle;

	readonly private Scene Scene;

	public PrimitivesHandler(Scene scene)
	{
		Scene = scene;

		pixel.SetData<uint>(new uint[] { 0xffffffff });
		circle = Texture2D.FromStream(Core.GraphicsManager.GraphicsDevice,
			new FileStream("Content/Circle.png", FileMode.Open));
	}

	public void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
	{
		spriteBatch.Begin();
		spriteBatch.Draw(pixel,
			point1,
			null,
			color,
			(float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X),
			new Vector2(0f, 0.5f),
			new Vector2(Vector2.Distance(point1, point2), thickness),
			SpriteEffects.None,
			0);
		spriteBatch.End();
	}

	public void DrawLine(Texture2D texture, Point p1, Point p2, Color color)
	{
		int x0 = p1.X; int y0 = p1.Y;
		int x1 = p2.X; int y1 = p2.Y;

		if(x0 < 0 || x1 > texture.Width || y0 < 0 || y1 > texture.Height)
			throw new Exception("Координаты точек отрезка выходят за пределы текстуры");

		int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
		int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
		int err = (dx > dy ? dx : -dy) / 2, e2;
		for(; ; )
		{
			texture.SetPixel(x0, y0, color);
			if(x0 == x1 && y0 == y1) break;
			e2 = err;
			if(e2 > -dx) { err -= dy; x0 += sx; }
			if(e2 < dy) { err += dx; y0 += sy; }
		}
	}

	public void DrawCircle(SpriteBatch spriteBatch, Vector2 position, int radius, Color color)
	{
		spriteBatch.Begin();
		spriteBatch.Draw(circle, position, 0.003f * radius, new Vector2(256, 256), color);
		spriteBatch.End();
	}

	//public Texture2D CreateCircle(int radius)
	//{
	//	int outerRadius = radius * 2 + 2;//So circle doesn't go out of bounds
	//	Texture2D texture = new Texture2D(Core.GraphicsManager.GraphicsDevice, outerRadius, outerRadius);

	//	Color[] data = new Color[outerRadius * outerRadius];

	//	//Colour the entire texture transparent first.
	//	for(int i = 0; i < data.Length; i++)
	//		data[i] = Color.Transparent;

	//	//Work out the minimum step necessary using trigonometry + sine approximation.
	//	double angleStep = 1f / radius;

	//	for(double angle = 0; angle < Math.PI * 2; angle += angleStep)
	//	{
	//		//Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
	//		int x = (int)Math.Round(radius + radius * Math.Cos(angle));
	//		int y = (int)Math.Round(radius + radius * Math.Sin(angle));

	//		data[y * outerRadius + x + 1] = Color.White;
	//	}

	//	texture.SetData(data);
	//	return texture;
	//}
}
