using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using AnimpafGE.Input;

namespace AnimpafGE.ECS
{
	public abstract class Scene
	{
		protected Game ParentGame { get; set; }
		public ContentManager Content { get; set; }

		public GameTime GameTime { get; set; }
		static public float DeltaTime { get; set; }
		public int UpdateFrame { get; set; }
		public int RenderFrame { get; set; }

		public string Name { get; set; }
		public List<Entity> Objects { get; set; } = new List<Entity>();

		public SpriteBatch spriteBatch;

		public Vector2 maxCoord;
		public Vector2 minCoord;

		protected Scene(Game game)
		{
			ParentGame = game;
			Content = ParentGame.Content;
		}

		public virtual void Initialize()
		{
			spriteBatch = new SpriteBatch(Core.Graphics.GraphicsDevice);

			minCoord = Vector2.Zero;
			maxCoord = new Vector2(ParentGame.Window.ClientBounds.Width,
				ParentGame.Window.ClientBounds.Height);


			if(TouchPanel.GetState().IsConnected)
			{
				minCoord = Vector2.Zero;
				 maxCoord = new Vector2(Core.Graphics.PreferredBackBufferWidth, Core.Graphics.PreferredBackBufferHeight);
			}
		}

		public abstract void LoadContent();

		public virtual void Process(GameTime gameTime)
		{
			InputProcessor.Process();

			UpdateFrame++;
			GameTime = gameTime;
			DeltaTime = gameTime.ElapsedGameTime.Milliseconds / 1000f;
		}

		public virtual void Render(GameTime gameTime)
		{
			RenderFrame++;

			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			foreach(Entity entity in Objects)
				entity.Process();

			spriteBatch.End();
		}
	}
}
