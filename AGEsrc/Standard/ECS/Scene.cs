using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using AGE.Input;
using AGE.ECS.Components;
using System;

namespace AGE.ECS
{
	public abstract class Scene
	{
		public Game ParentGame;
		public ContentManager Content;

		public GameTime GameTime;
		static public float DeltaTime;
		public int UpdateFrame;
		public int RenderFrame;

		List<InputProcessor> InputProcessors = new List<InputProcessor>();

		public string Name;
		public List<Entity> Objects = new List<Entity>();
		public List<BoxCollider> Colliders = new List<BoxCollider>();

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
			foreach(var item in InputProcessors)
				item.Process();

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

		public void TrackInputProcessor(InputProcessor inputProcessor)
		{
			if(InputProcessors.Contains(inputProcessor))
				throw new Exception("Этот InputProcessor уже отслеживается в данной сцене.");

			InputProcessors.Add(inputProcessor);
		}
	}
}
