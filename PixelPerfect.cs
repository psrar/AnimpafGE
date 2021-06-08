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
			Transform.S = size;
			AddComponent<PRenderer>().Init();
			GetComponent<PRenderer>().Enabled = isVisible;
		}

		public override void Process()
		{
			base.Process();
		}
	}

	public class PTransform : Component
	{
		public Vector2 Position { get; set; } = Vector2.Zero;
		public Vector2 PixelPosition { get; set; } = Vector2.Zero;
		public int S { get; set; } = 20;

		public override void Process()
		{
			PixelPosition = new Vector2((int)Position.X / S, (int)Position.Y / S);
		}
	}

	public class PRenderer : Component
	{
		static Texture2D Pixel = new Texture2D(Core.Graphics.GraphicsDevice, 1, 1);
		static SpriteBatch Batch { get; set; }

		PTransform Transform;
		Vector2 halfS;

		new PEntity Entity { get; set; }
		public Color Color { get; set; } = Color.White;

		Vector2 PPosition;
		Vector2 Origin;

		int Layer { get; set; } = 0;

		public PRenderer() => Pixel.SetData<uint>(new uint[1] { Color.PackedValue }, 0, 1);

		public override void Init()
		{
			Entity = (PEntity)base.Entity;
			Batch = Entity.ParentScene.spriteBatch;

			Transform = Entity.Transform;
			halfS = Vector2.One / 2 * Transform.S;
			Origin = new Vector2(0.5f);
		}

		public override void Process()
		{
			if(Enabled)
			{
				PPosition = Transform.PixelPosition * Entity.Transform.S + halfS;
				Batch.Draw(Pixel,                           // Texture
					PPosition,                              // Position
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
			Random rnd = new();
			Color = new Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
		}
	}

	public class PRigidBody : Component
	{
		static public Vector2 Gravity = new Vector2(0, 9800 * 2);

		new PEntity Entity { get; set; }

		public Vector2 Velocity { get; set; }
		public Vector2 Acceleration { get; set; }
		public float Inertia { get; set; } = 1.05f;
		public float Friction { get; set; } = 2;
		public bool UseGravity { get; set; } = false;

		public override void Init()
		{
			Entity = (PEntity)base.Entity;
		}

		public override void Process()
		{
			float delta = Scene.DeltaTime;
			if(UseGravity)
				Velocity += Gravity * delta;

			Velocity /= Friction;

			Velocity += Acceleration * delta;
			if(Acceleration != Vector2.Zero)
				Acceleration /= Inertia;

			if(Entity.ParentScene.RenderFrame % 10 == 0)
			{
				if(Vector2.Distance(Acceleration, Vector2.Zero) < 4)
					Acceleration = Vector2.Zero;
				if(Vector2.Distance(Velocity, Vector2.Zero) < 4)
					Velocity = Vector2.Zero;
			}

			Velocity = Vector2.Clamp(Velocity, Vector2.One * -1960, Vector2.One * 1960);
			Entity.Transform.Position += Velocity * delta;
		}
	}

	public class PScene : Scene
	{
		int PixelSize { get; set; }
		int Width { get; set; }
		int Height { get; set; }

		public TextureCanvas Background { get; protected set; }
		public List<PEntity> Pixels { get; set; } = new();

		public PScene(Game game, string name = null) : base(game)
		{
			Name = name ?? "SimpleScene";
			ParentGame.IsMouseVisible = true;

			InitBackground();
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void LoadContent()
		{

		}

		public override void Process(GameTime gameTime)
		{
			base.Process(gameTime);
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

		public PEntity AddPixel(int size, Vector2 position, bool isVisible = true)
		{
			Pixels.Add(new PEntity(this, size, position, isVisible));
			return Pixels.Last();
		}

		public virtual void InitBackground(Color? color = null)
		{
			Background = new(Core.Graphics.GraphicsDevice, 20, Core.Graphics.PreferredBackBufferWidth,
				Core.Graphics.PreferredBackBufferHeight, color);
		}
	}
}
