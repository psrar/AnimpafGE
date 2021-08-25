using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AGE.Graphics
{
	public class Vertex2D
	{
		public Vector2 Position;

		public Vertex2D(Vector2 position) => Position = position;
	}

	public struct Edge
	{
		Vertex2D StartVertex, EndVertex;
		Vector2 StartPosition, EndPosition;

		public Edge(Vertex2D startVertex, Vertex2D endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;
			StartPosition = startVertex.Position;
			EndPosition = endVertex.Position;
		}
	}

	public class PrimitivesHandler
	{
		static readonly private Texture2D pixel = new Texture2D(Core.GraphicsManager.GraphicsDevice, 1, 1);

		public PrimitivesHandler() => pixel.SetData<uint>(new uint[] { 0xffffffff });

		public void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
		{
			spriteBatch.Draw(pixel,
				point1,
				null,
				color,
				(float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X),
				new Vector2(0f, 0.5f),
				new Vector2(Vector2.Distance(point1, point2), thickness),
				SpriteEffects.None,
				0);
		}
	}
}
