using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimpafGE.ECS;
using AnimpafGE.ECS.Components;
using AnimpafGE.PixelPerfect.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimpafGE.PixelPerfect.Components
{
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
	}

	public class PRigidBody : Component
	{
		static public Vector2 Gravity = new Vector2(0, 9800 * 4);

		new PEntity Entity { get; set; }
		PTransform Transform;
		PScene PScene { get; set; }

		public Vector2 Velocity { get; set; } = Vector2.Zero;
		public Vector2 Acceleration { get; set; } = Vector2.Zero;
		public float AccelerationSave { get; set; } = 0.95f;
		public float VelocitySave { get; set; } = 0.95f;
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
			if(isStatic)
				PScene.SetPixelState(posX, posY, 10);

			Vector2 ExpectedPosition = Vector2.Zero;
			Vector2 PathResult = Vector2.Zero;

			if(!isStatic)
			{
				float delta = Scene.DeltaTime;

				if(UseGravity)
					Acceleration += Gravity;

				if(Acceleration != Vector2.Zero)
				{
					Velocity += Acceleration * delta;
				}

				if(ParentScene.RenderFrame % 10 == 0)
				{
					if(Vector2.Distance(Acceleration, Vector2.Zero) < 4)
						Acceleration = Vector2.Zero;
					if(Vector2.Distance(Velocity, Vector2.Zero) < 4)
						Velocity = Vector2.Zero;
				}

				if(Velocity != Vector2.Zero)
				{
					Velocity = Vector2.Clamp(Velocity, Vector2.One * -1960, Vector2.One * 1960);
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
					if(Velocity != Vector2.Zero)
					{
						ExpectedPosition = Transform.Position + Velocity * delta;
						PathResult = DrawPathBresenham(posX, posY,
							(int)ExpectedPosition.X / PScene.PixelSize,
							(int)ExpectedPosition.Y / PScene.PixelSize);
						if(PathResult == Vector2.Zero)
							Transform.Position = ExpectedPosition;
						else
							Transform.Position = PathResult * PScene.PixelSize;
					}
					else
						Transform.Position = Vector2.Clamp(Transform.Position + Velocity * delta, clampMin, clampMax);
				}

				PScene.SetPixelState(Transform.Position, 2);

				if(Acceleration == Vector2.Zero)
					Velocity *= VelocitySave;
			}
		}

		public Vector2 DrawPathBresenham(int x, int y, int x2, int y2)
		{
			if(x == x2 && y == y2)
				return Vector2.Zero;

			int w = x2 - x;
			int h = y2 - y;
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if(w < 0) dx1 = -1;
			else if(w > 0) dx1 = 1;

			if(h < 0) dy1 = -1;
			else if(h > 0) dy1 = 1;

			dx2 = dx1;

			int longest = Math.Abs(w);
			int shortest = Math.Abs(h);
			if(!(longest > shortest))
			{
				longest = Math.Abs(h);
				shortest = Math.Abs(w);
				if(h < 0) dy2 = -1; else if(h > 0) dy2 = 1;
				dx2 = 0;
			}
			int numerator = longest >> 1;
			for(int i = 0; i <= longest; i++)
			{
				numerator += shortest;
				if(!(numerator < longest))
				{
					numerator -= longest;
					x += dx1;
					y += dy1;

					if(PScene.GetPixelState(x, y) != 0)
						return new Vector2(x - dx1, y - dy1);
				}
				else
				{
					x += dx2;
					y += dy2;

					if(PScene.GetPixelState(x, y) != 0)
						return new Vector2(x - dx2, y - dy2);
				}
			}

			return Vector2.Zero;
		}

		public void AddForce(Vector2 force)
		{
			if(!isStatic)
				Velocity += force;
		}
	}
}
