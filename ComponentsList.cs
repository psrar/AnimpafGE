using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AnimpafGE.ECS.Components
{
	class SpriteRenderer : Component
	{
		public Texture2D Sprite { get; set; }
		SpriteBatch Batch { get; }
		Color Color { get; set; } = Color.White;

		public SpriteRenderer() => Batch = new SpriteBatch(Core.graphicsDeviceManager.GraphicsDevice);

		public override void Process()
		{
			if(!(Sprite is null))
			{
				Trace.WriteLine("Child process method");
				Batch.Begin();
				Batch.Draw(Sprite, Entity.Position, Color);
				Batch.End();
			}
			else
			{
				throw new System.Exception("Попытка вызвать обработку компонента, которого не " +
					"существует для данного объекта.");
			}
		}
	}
}
