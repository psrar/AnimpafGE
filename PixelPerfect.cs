using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimpafGE.ECS;
using AnimpafGE.ECS.Components;
using AnimpafGE.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimpafGE.PixelPerfect
{
	public class PEntity : Entity
	{
		public PScene PScene { get; set; }
		public new PTransform Transform { get; set; }
		public int Index { get; set; }

		public PEntity(Scene scene) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PTransform>();
			AddComponent<PRenderer>().Init();
		}
		public PEntity(Scene scene, int size, Vector2 position, bool isVisible = true) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PTransform>();
			Transform.Position = position;
			Transform.Init();
			AddComponent<PRenderer>().Init();
			GetComponent<PRenderer>().Enabled = isVisible;
		}
		public PEntity(Scene scene, int size, Vector2 position, Color color, bool isVisible = true) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PTransform>();
			Transform.Position = position;
			Transform.Init();
			AddComponent<PRenderer>().Init();
			GetComponent<PRenderer>().Color = color;
			GetComponent<PRenderer>().Enabled = isVisible;
		}

		public override void Process()
		{
			base.Process();

			Index = (int)(Transform.IndexPosition.X + PScene.VirtualWidth * Transform.IndexPosition.Y);
		}
	}

	public class PTransform : Component
	{
		public Vector2 Position { get; set; } = Vector2.Zero;
		public Vector2 PixelPosition { get; set; } = Vector2.Zero;
		public Vector2 IndexPosition = Vector2.Zero;
		public int S { get; set; }

		Vector2 halfS;

		public override void Init()
		{
			S = ((PScene)ParentScene).PixelSize;
			halfS = Vector2.One / 2 * S;
		}

		public override void Process()
		{
			PixelPosition = Vector2.Floor(Position / S) * S + halfS;
			IndexPosition = (PixelPosition - halfS) / S;
		}
	}

	public class PRenderer : Component
	{
		static Texture2D Pixel = new Texture2D(Core.Graphics.GraphicsDevice, 1, 1);
		static SpriteBatch Batch { get; set; }

		PTransform Transform;

		new PEntity Entity { get; set; }
		public Color Color { get; set; } = Color.White;

		Vector2 Origin;

		int Layer { get; set; } = 0;

		public PRenderer() => Pixel.SetData<uint>(new uint[1] { Color.PackedValue }, 0, 1);

		public override void Init()
		{
			Entity = (PEntity)base.Entity;
			Batch = ParentScene.spriteBatch;

			Transform = Entity.Transform;
			Origin = new Vector2(0.5f);
		}

		public override void Process()
		{
			if(Enabled)
			{
				Batch.Draw(Pixel,                           // Texture
					Transform.PixelPosition,                // Position
					null,                                   // Source rectangle
					Color,                                  // Color
					0,                                      // Rotation
					Origin,                                 // Origin
					Entity.Transform.S,                     // Scale
					SpriteEffects.None,                     // Mirroring effect
					Layer);                                 // Depth
			}
		}

		public void SetRandomColor()
		{
			Random rnd = new Random();
			Color = new Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
		}
	}

	public class PRigidBody : Component
	{
		static public Vector2 Gravity = new Vector2(0, 9800 * 2);

		new PEntity Entity { get; set; }
		PTransform Transform;
		PScene PScene { get; set; }

		public Vector2 Velocity { get; set; }
		public Vector2 Acceleration { get; set; }
		public float Inertia { get; set; } = 1.05f;
		public float Friction { get; set; } = 2;
		public bool UseGravity { get; set; } = false;
		public bool isStatic { get; set; } = false;

		Vector2 clampMin;
		Vector2 clampMax;

		public override void Init()
		{
			Entity = (PEntity)base.Entity;
			Transform = Entity.Transform;
			PScene = (PScene)ParentScene;

			clampMin = Vector2.One * PScene.PixelSize;
			clampMax = PScene.maxCoord - Vector2.One * PScene.PixelSize * 2;
		}

		public override void Process()
		{
			int posX = (int)Transform.IndexPosition.X;
			int posY = (int)Transform.IndexPosition.Y;

			if(!isStatic)
			{
				float delta = Scene.DeltaTime;

				if(UseGravity)
					Velocity += Gravity * delta;

				Velocity /= Friction;

				Velocity += Acceleration * delta;
				if(Acceleration != Vector2.Zero)
					Acceleration /= Inertia;

				if(ParentScene.RenderFrame % 10 == 0)
				{
					if(Vector2.Distance(Acceleration, Vector2.Zero) < 4)
						Acceleration = Vector2.Zero;
					if(Vector2.Distance(Velocity, Vector2.Zero) < 4)
						Velocity = Vector2.Zero;
				}

				if(Velocity != Vector2.Zero)
				{
					if(Velocity.Y != 0)
						if(PScene.GetPixelState(posX, posY + 1) != 0 && Velocity.Y > 0)
							Velocity *= Vector2.UnitX;
						else
							if(PScene.GetPixelState(posX, posY - 1) != 0 && Velocity.Y < 0)
							Velocity *= Vector2.UnitX;

					if(Velocity.X != 0)
						if(PScene.GetPixelState(posX + 1, posY) != 0 && Velocity.X > 0)
							Velocity *= Vector2.UnitY;
						else
							if(PScene.GetPixelState(posX - 1, posY) != 0 && Velocity.X < 0)
								Velocity *= Vector2.UnitY;

					//if(false)
					//{
					//	float Angle = MathF.Atan2(Velocity.Y, Velocity.X);
					//	if(Angle > 0 && Angle < MathHelper.PiOver2)
					//	{
					//		if(PScene.GetPixelState(posX + 1, posY + 1) != 0)
					//			Velocity = Vector2.Zero;
					//	}
					//	else if(Angle > MathHelper.PiOver2 && Angle < MathHelper.Pi)
					//	{
					//		if(PScene.GetPixelState(posX - 1, posY + 1) != 0)
					//			Velocity = Vector2.Zero;
					//	}
					//	else if(Angle > MathHelper.Pi && Angle < 3 * MathHelper.PiOver2)
					//	{
					//		if(PScene.GetPixelState(posX - 1, posY - 1) != 0)
					//			Velocity = Vector2.Zero;
					//	}
					//	else if(Angle < 0)
					//	{
					//		if(PScene.GetPixelState(posX + 1, posY - 1) != 0)
					//			Velocity = Vector2.Zero;
					//	}
					//}
				}

				Velocity = Vector2.Clamp(Velocity, Vector2.One * -1960, Vector2.One * 1960);
				Transform.Position =
				Vector2.Clamp(Transform.Position + Velocity * delta, clampMin, clampMax);
				PScene.SetPixelState(Transform.Position, 2);
			}
		}
	}

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

			for(int x = 0; x < VirtualWidth; x++)
			{
				PEntity border = AddPixel(new Vector2(x * PixelSize, Height), Color.Red);
				border.AddComponent<PRigidBody>().isStatic = true;
			}
			for(int x = 0; x < VirtualWidth; x++)
			{
				PEntity border = AddPixel(new Vector2(x * PixelSize, 0), Color.Red);
				border.AddComponent<PRigidBody>().isStatic = true;
			}
			for(int y = 0; y < VirtualHeight; y++)
			{
				PEntity border = AddPixel(new Vector2(Width, y * PixelSize), Color.Red);
				border.AddComponent<PRigidBody>().isStatic = true;
			}
			for(int y = 0; y < VirtualHeight; y++)
			{
				PEntity border = AddPixel(new Vector2(0, y * PixelSize), Color.Red);
				border.AddComponent<PRigidBody>().isStatic = true;
			}
		}

		public override void LoadContent()
		{
		}

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
			int time = DateTime.Now.Millisecond;
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			spriteBatch.Draw(Background.Texture, Vector2.Zero, Color.White);

			foreach(Entity entity in Objects)
				entity.Process();

			spriteBatch.End();
			Trace.WriteLine("Ms: " + (DateTime.Now.Millisecond - time));
		}

		public PEntity AddPixel(Vector2 position, Color color, bool isVisible = true)
		{
			PEntity newEntity = new PEntity(this, PixelSize, position, color, isVisible);
			Pixels.Add(newEntity);
			return newEntity;
		}

		public virtual void InitBackground(Color? color = null)
		{
			Background = new TextureCanvas(Core.Graphics.GraphicsDevice, 20, Core.Graphics.PreferredBackBufferWidth,
				Core.Graphics.PreferredBackBufferHeight, color);
		}

		public PEntity GetPixel(int x, int y) => PhysicsMap[x + y * VirtualWidth];

		public int WorldPositionToIndex(Vector2 position) => (int)(position.X / PixelSize + VirtualWidth * (int)(position.Y / PixelSize));

		public int SetPixelState(int x, int y, int value) => StateMap[x + y * VirtualWidth] = value;
		public int SetPixelState(int index, int value) => StateMap[index] = value;
		public int SetPixelState(Vector2 position, int value) => StateMap[WorldPositionToIndex(position)] = value;
		public int GetPixelState(int x, int y) => StateMap[x + y * VirtualWidth];
		public int GetPixelState(int index) => StateMap[index];
	}
}
