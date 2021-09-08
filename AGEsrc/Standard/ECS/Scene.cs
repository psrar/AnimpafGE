using AGE.ECS.Components;
using AGE.Graphics;
using AGE.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AGE.ECS
{
	public abstract class Scene
	{
		public readonly Game ParentGame;
		public readonly ContentManager Content;
		public readonly PrimitivesHandler PrimitivesHandler;
		public readonly Effect PolygonEffect;

		public GameTime GameTime { get; private set; }
		public int UpdateFrame { get; private set; }
		public int RenderFrame { get; private set; }
		static public float DeltaTime { get; private set; }

		List<InputProcessor> InputProcessors = new List<InputProcessor>();

		public string Name;
		public List<Entity> Objects = new List<Entity>();
		public List<BoxCollider> Colliders = new List<BoxCollider>();
		public List<PolygonRenderer> PolygonRenderers = new List<PolygonRenderer>();

		private int objectsProcessQueue = 0;
		private int collidersProcessQueue = 0;
		private int polygonsProcessQueue = 0;

		public SpriteBatch spriteBatch;

		public Vector2 minCoord;
		public Vector2 maxCoord;
		public Color BackgroundColor = Color.CornflowerBlue;

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
			DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		public virtual void Render(GameTime gameTime)
		{
			RenderFrame++;

			Core.GraphicsManager.GraphicsDevice.Clear(BackgroundColor);

			RenderPolygons();

			spriteBatch.Begin();

			for(objectsProcessQueue = 0; objectsProcessQueue < Objects.Count; objectsProcessQueue++)
				Objects[objectsProcessQueue].Process();

			spriteBatch.End();
		}

		public void RenderPolygons()
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			PolygonEffect.CurrentTechnique.Passes[0].Apply();
			for(int i = 0; i < PolygonRenderers.Count; i++)
				if(PolygonRenderers[i].Enabled)
					PolygonRenderers[i].RenderPolygon();
			spriteBatch.End();
		}

		public void TrackInputProcessor(InputProcessor inputProcessor)
		{
			if(InputProcessors.Contains(inputProcessor))
				throw new Exception("Этот InputProcessor уже отслеживается в данной сцене.");

			InputProcessors.Add(inputProcessor);
		}

		public void DestroyEntity(Entity entity)
		{
			Objects.Remove(entity);
			objectsProcessQueue--;

			BoxCollider boxCollider = entity.GetComponent<BoxCollider>();
			if(boxCollider != null)
			{
				Colliders.Remove(boxCollider);
				collidersProcessQueue--;
			}
			PolygonRenderer polygon = entity.GetComponent<PolygonRenderer>();
			if(polygon != null)
			{
				PolygonRenderers.Remove(polygon);
				polygonsProcessQueue--;
			}

		}
	}
}
