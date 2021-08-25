using System;
using System.Diagnostics;
using System.Collections.Generic;
using AGE.ECS;
using AGE.PixelPerfect.Components;
using AGE.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AGE.PixelPerfect.ECS
{
	public class PScene : Scene
	{
		public int PixelSize { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int Size { get; private set; }
		public int VirtualWidth { get; private set; }
		public int VirtualHeight { get; private set; }
		public int VirtualSize { get; private set; }

		public TextureCanvas Background { get; protected set; }
		public List<PEntity> Pixels { get; set; } = new List<PEntity>();

		public PEntity[] PhysicsMap;
		public int[] StateMap;

		public PScene(Game game, int pixelSize, int width, int height, string name = null) : base(game)
		{
			Name = name ?? "SimpleScene";
			ParentGame.IsMouseVisible = true;

			PixelSize = pixelSize;
			Width = width;
			Height = height;
			Size = width * height;
			VirtualWidth = width / pixelSize;
			VirtualHeight = height / pixelSize;
			VirtualSize = VirtualWidth * VirtualHeight;

			PhysicsMap = new PEntity[VirtualSize];
			StateMap = new int[VirtualSize];
			InitBackground();
		}

		public override void Initialize()
		{
			base.Initialize();

			PEntity border;
			for(int x = 0; x < VirtualWidth; x++)
			{
				border = AddPixel(new Vector2(x * PixelSize, Height - PixelSize), Color.Red);
				border.RigidBody.IsStatic = true;

				border = AddPixel(new Vector2(x * PixelSize, 0), Color.Red);
				border.RigidBody.IsStatic = true;
			}
			for(int y = 0; y < VirtualHeight; y++)
			{
				border = AddPixel(new Vector2(Width - PixelSize, y * PixelSize), Color.Red);
				border.RigidBody.IsStatic = true;

				border = AddPixel(new Vector2(0, y * PixelSize), Color.Red);
				border.RigidBody.IsStatic = true;
			}
		}

		public override void LoadContent() { }

		public override void Process(GameTime gameTime)
		{
			base.Process(gameTime);

			for(int i = 0; i < PhysicsMap.Length; i++)
			{
				PhysicsMap[i] = null;
				StateMap[i] = 0;
			}

			foreach(PEntity pEntity in Pixels)
			{
				PhysicsMap[pEntity.Index] = pEntity;
				SetPixelState(pEntity.Index, 1);
			}
		}

		public override void Render(GameTime gameTime)
		{
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			spriteBatch.Draw(Background.Texture, Vector2.Zero, Color.White);

			foreach(Entity entity in Objects)
				entity.Process();

			foreach(PEntity pEntity in Pixels)
			{
				pEntity.Process();
			}

			spriteBatch.End();
		}

		public PEntity AddPixel(Vector2 position, Color color, bool isVisible = true)
		{
			PEntity newEntity = new PEntity(this, position, color, isVisible);
			Pixels.Add(newEntity);
			return newEntity;
		}

		public virtual void InitBackground(Color? color = null)
		{
			Background = new TextureCanvas(Core.GraphicsManager.GraphicsDevice, Core.GraphicsManager.PreferredBackBufferWidth,
				Core.GraphicsManager.PreferredBackBufferHeight, color);
		}

		public PEntity GetPixel(int x, int y) => PhysicsMap[x + y * VirtualWidth];
		public PEntity GetPixel(int index) => PhysicsMap[index];
		public PEntity GetPixel(Vector2 position) => PhysicsMap[WorldPositionToIndex(position)];
		public PEntity SetPixel(int x, int y, PEntity pEntity) => PhysicsMap[x + y * VirtualWidth] = pEntity;
		public PEntity SetPixel(int index, PEntity pEntity) => PhysicsMap[index] = pEntity;
		public PEntity SetPixel(Vector2 position, PEntity pEntity) => PhysicsMap[WorldPositionToIndex(position)] = pEntity;

		public int GetPixelState(int x, int y) => StateMap[x + y * VirtualWidth];
		public int GetPixelState(int index) => StateMap[index];
		public int GetPixelState(Vector2 position) => StateMap[WorldPositionToIndex(position)];
		public int SetPixelState(int x, int y, int value) => StateMap[x + y * VirtualWidth] = value;
		public int SetPixelState(int index, int value) => StateMap[index] = value;
		public int SetPixelState(Vector2 position, int value) => StateMap[WorldPositionToIndex(position)] = value;

		public void MovePixel(Vector2 startPos, Vector2 endPos)
		{
			SetPixelState(endPos, GetPixelState(startPos));
			SetPixelState(startPos, 0);
			SetPixel(endPos, GetPixel(startPos));
			SetPixel(startPos, null);
		}
		public void MovePixel(int x, int y, Vector2 endPos)
		{
			int r = GetPixelState(x, y);
			SetPixelState(x, y, 0);
			SetPixelState(endPos, r);
			PEntity rEntity = GetPixel(x, y);
			SetPixel(x, y, null);
			SetPixel(endPos, rEntity);
		}

		public int WorldPositionToIndex(Vector2 position) =>
			(int)(position.X / PixelSize + VirtualWidth * (int)(position.Y / PixelSize));
	}
}
