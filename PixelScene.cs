using System;
using AnimpafGE.ECS;
using AnimpafGE.ECS.Components;
using AnimpafGE.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AnimpafGE.ECS
{
	class PixelScene : Scene
	{
		static int pixelSize, mapx, mapy;
		Entity[,] pixelMap;

		TextureCanvas Background;

		Entity player;

		Vector2 mousePosition = Vector2.Zero;

		public PixelScene(Game game, string name = null) : base(game)
		{
			Name = name ?? "SimpleScene";
			ParentGame.IsMouseVisible = true;
		}

		public override void Initialize()
		{
			base.Initialize();

			InitPixelMap();

			player = new Entity(this);
			player.AddComponent<SpriteRenderer>().Sprite = Content.Load<Texture2D>("Player");
			player.AddComponent<RigidBody>().UseGravity = true;
			player.Transform.SetScaling(new Vector2(5));
		}

		public override void LoadContent()
		{
			//Write your loading code here
		}

		bool f = true;
		public override void Process(GameTime gameTime)
		{
			base.Process(gameTime);

			//Write your Process (Update) code here

			if(Mouse.GetState().LeftButton == ButtonState.Pressed)
			{
				mousePosition = Mouse.GetState().Position.ToVector2();
				mousePosition = Vector2.Clamp(new Vector2(mousePosition.X / pixelSize, mousePosition.Y / pixelSize),
					Vector2.Zero, ParentGame.Window.ClientBounds.Size.ToVector2() / pixelSize);
				if(true)
				{
					pixelMap[(int)mousePosition.X, (int)mousePosition.Y].GetComponent<SpriteRenderer>().Color = Color.Black;
				}
			}

			if(Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				if(f)
					player.GetComponent<RigidBody>().Velocity = new Vector2(0, -1200);
				f = false;
			}

			if(Keyboard.GetState().IsKeyUp(Keys.Space))
				f = true;

		}

		public override void Render(GameTime gameTime)
		{
			int time = DateTime.Now.Millisecond;
			//Write your render code here

			spriteBatch.Begin();
			spriteBatch.Draw(Background.Texture, Vector2.Zero, Color.White);
			spriteBatch.End();

			base.Render(gameTime);

			Trace.WriteLine("Ms: " + (DateTime.Now.Millisecond - time));

		}

		public void InitPixelMap()
		{
			Background = new TextureCanvas(Core.Graphics.GraphicsDevice, 20, Core.Graphics.PreferredBackBufferWidth,
				Core.Graphics.PreferredBackBufferHeight, Color.BlanchedAlmond);
		}
	}
}