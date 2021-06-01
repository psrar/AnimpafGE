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
		static int mapx = 100, mapy = 100, pixelSize = 10;
		Entity[,] pixelMap = new Entity[mapx, mapy];

		public PixelScene(Game game, string name = null) : base(game)
		{
			Name = name ?? "SimpleScene";
			ParentGame.IsMouseVisible = true;
			Core.Graphics.IsFullScreen = true;
		}

		public override void Initialize()
		{
			base.Initialize();

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
		}

		public override void LoadContent()
		{
			//Write your loading code here
		}

		public override void Process()
		{
			//Write your Process (Update) code here
		}

		public override void Render()
		{
			base.Render();

			//Write your render code here
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