using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using AnimpafGE.ECS.Components;
using Microsoft.Xna.Framework.Input;

namespace AnimpafGE.ECS
{
	abstract class Scene
	{
		protected Game ParentGame { get; set; }
		protected ContentManager Content { get; set; }

		public string Name { get; set; }
		public List<Entity> Objects { get; set; } = new List<Entity>();

		public SpriteBatch spriteBatch;

		protected Scene(Game game)
		{
			ParentGame = game;
			Content = ParentGame.Content;
		}

		public virtual void Initialize()
		{
			spriteBatch = new(Core.Graphics.GraphicsDevice);
		}

		public abstract void LoadContent();

		public abstract void Process(GameTime gameTime);

		public virtual void Render(GameTime gameTime)
		{

			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			foreach(Entity entity in Objects)
				entity.Process();

			spriteBatch.End();
		}
	}
}
