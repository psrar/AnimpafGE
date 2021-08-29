using AGE.ECS.Components;
using AGE.Graphics;
using AGE.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace AGE.ECS
{
	public abstract class Scene
	{
		public Game ParentGame;
		public ContentManager Content;
		public readonly PrimitivesHandler PrimitivesHandler;
		public readonly Effect PolygonEffect;

		public GameTime GameTime;
		static public float DeltaTime;
		public int UpdateFrame;
		public int RenderFrame;

		List<InputProcessor> InputProcessors = new List<InputProcessor>();

		public string Name;
		public List<Entity> Objects = new List<Entity>();
		public List<BoxCollider> Colliders = new List<BoxCollider>();
		public List<PolygonRenderer> PolygonRenderers = new List<PolygonRenderer>();

		public SpriteBatch spriteBatch;

		public Vector2 minCoord;
		public Vector2 maxCoord;

		protected Scene(Game game)
		{
			ParentGame = game;
			Content = ParentGame.Content;
			PrimitivesHandler = new PrimitivesHandler(this);

			byte[] bytes = File.ReadAllBytes("Content/PolygonFX.mgfx");
			PolygonEffect = new Effect(ParentGame.GraphicsDevice, bytes);
		}

		public virtual void Initialize()
		{
			spriteBatch = new SpriteBatch(Core.GraphicsManager.GraphicsDevice);

			minCoord = Vector2.Zero;
			maxCoord = new Vector2(ParentGame.Window.ClientBounds.Width,
				ParentGame.Window.ClientBounds.Height);
		}

		public abstract void LoadContent();

		public virtual void Process(GameTime gameTime)
		{
			UpdateFrame++;

			foreach(var item in InputProcessors)
				item.Process();

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

			RenderPolygons();
		}

		public void RenderPolygons()
		{
			spriteBatch.Begin(SpriteSortMode.Immediate);
			PolygonEffect.CurrentTechnique.Passes[0].Apply();
			foreach(PolygonRenderer renderer in PolygonRenderers)
				if(renderer.Enabled)
					renderer.RenderPolygon();
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
