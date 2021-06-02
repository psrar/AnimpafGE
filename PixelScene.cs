using System;
using AnimpafGE.ECS;
using AnimpafGE.ECS.Components;
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

			pixelSize = 30;
			mapx = Core.Graphics.PreferredBackBufferWidth / pixelSize + 1;
			mapy = Core.Graphics.PreferredBackBufferHeight / pixelSize + 1;
			pixelMap = new Entity[mapx, mapy];

			//Write your initialization logic here

			pixelMap = new Entity[mapx, mapy];

			for(int y = 0; y < mapy; y++)
			{
				for(int x = 0; x < mapx; x++)
				{
					pixelMap[x, y] = new Entity(this);
					pixelMap[x, y].AddComponent<SpriteRenderer>().Sprite = Content.Load<Texture2D>("Pixel");
				}
			}
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
			base.Render(gameTime);

			//Write your render code here
			Trace.WriteLine("Ms: " + (DateTime.Now.Millisecond - time));
		}

		public void InitPixelMap()
		{
			Core.Graphics.GraphicsDevice.Clear(Color.White);
			for(int y = 0; y < mapy; y++)
			{
				for(int x = 0; x < mapx; x++)
				{
					pixelMap[x, y].Transform.Scaling = new Vector2(pixelSize);
					pixelMap[x, y].Transform.Position = new Vector2(x * pixelSize, y * pixelSize);
					pixelMap[x, y].GetComponent<SpriteRenderer>().SetRandomColor();
				}
			}
		}
	}
}