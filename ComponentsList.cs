using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AnimpafGE.ECS.Components
{
	class SpriteRenderer : Component
	{
		Texture2D Sprite { get; set; }
		SpriteBatch Batch { get; }
		Color Color { get; set; } = Color.White;

		public new void Process()
		{
			Trace.WriteLine("Child process method");
			Batch.Begin();
			Batch.Draw(Sprite, Entity.Position, Color);
			Batch.End();
		}
	}
}
