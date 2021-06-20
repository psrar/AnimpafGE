using AnimpafGE.ECS;
using AnimpafGE.PixelPerfect.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
			CalculateIndex();
		}

		public void CalculateIndex()
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
		static public Vector2 Gravity = new Vector2(0, 9800 / 1.6f);

		new PEntity Entity { get; set; }
		PTransform Transform;
		PScene PScene { get; set; }

		public Vector2 Velocity { get; set; } = Vector2.Zero;
		public Vector2 Acceleration { get; set; } = Vector2.Zero;
		public float AccelerationSave { get; set; } = 0.95f;
		public float VelocitySave { get; set; } = 0.95f;
		public bool UseGravity { get; set; } = false;
		public bool isStatic { get; set; } = false;

		int posX, posY;
		bool[] clampedSide = new bool[4];
		//1- up 2- right 3- down 4- left

		public override void Init()
		{
			Entity = (PEntity)base.Entity;
			Transform = Entity.Transform;
			PScene = (PScene)ParentScene;
		}

		public override void Process()
		{
			posX = (int)Transform.IndexPosition.X;
			posY = (int)Transform.IndexPosition.Y;

			if(!isStatic)
			{
				float delta = Scene.DeltaTime;
				int posXE, posYE, dx, dy;

				if(UseGravity)
					Velocity = Vector2.Clamp(Velocity + Gravity * delta, Vector2.One * -20000, Vector2.One * 20000);

				if(Acceleration != Vector2.Zero)
					Velocity = Vector2.Clamp(Velocity + Acceleration, Vector2.One * -20000, Vector2.One * 20000);

				if(ParentScene.RenderFrame % 10 == 0)
				{
					if(Vector2.Distance(Acceleration, Vector2.Zero) < PScene.PixelSize)
						Acceleration = Vector2.Zero;
					if(Vector2.Distance(Velocity, Vector2.Zero) < PScene.PixelSize)
						Velocity = Vector2.Zero;
				}

				CheckClamping();
				if(clampedSide[0] && Velocity.Y < 0)
					Velocity *= Vector2.UnitX;
				if(clampedSide[1] && Velocity.X > 0)
					Velocity *= Vector2.UnitY;
				if(clampedSide[2] && Velocity.Y > 0)
					Velocity *= Vector2.UnitX;
				if(clampedSide[3] && Velocity.X < 0)
					Velocity *= Vector2.UnitY;

				if(Velocity != Vector2.Zero)
				{
					Vector2 ExpectedPosition = Transform.Position + Velocity * delta;
					posXE = (int)ExpectedPosition.X / PScene.PixelSize;
					posYE = (int)ExpectedPosition.Y / PScene.PixelSize;
					dx = posXE - posX;
					dy = posYE - posY;

					if(!(dy == 0 && dx == 0))
					{
						if(Math.Abs(dx) + Math.Abs(dy) < 2)
							Transform.Position = ExpectedPosition;
						else
						{
							Vector2 PathResult = DrawPathBresenham(posX, posY, posXE, posYE);
							if(PathResult == Vector2.Zero)
								Transform.Position = ExpectedPosition;
							else
								Transform.Position = PathResult * PScene.PixelSize;
						}
					}
					else
						Transform.Position = ExpectedPosition;

					PScene.SetPixelState(Transform.Position, 2);

					Velocity *= VelocitySave;
				}
			}
		}

		public Vector2 DrawPathBresenham(int x, int y, int x2, int y2)
		{
			int w = x2 - x;
			int h = y2 - y;
			int absW = Math.Abs(w);
			int absH = Math.Abs(h);

			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if(w < 0) dx1 = -1;
			else if(w > 0) dx1 = 1;

			if(h < 0) dy1 = -1;
			else if(h > 0) dy1 = 1;

			int longest;
			int shortest;
			if(absW <= absH)
			{
				longest = absH;
				shortest = absW;
				if(h < 0) dy2 = -1; else if(h > 0) dy2 = 1;
			}
			else
			{
				longest = absW;
				shortest = absH;
				dx2 = dx1;
			}
			int numerator = longest / 2;
			for(int i = 0; i <= longest; i++)
			{
				numerator += shortest;
				if(numerator >= longest)
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

		public void CheckClamping()
		{
			if(PScene.GetPixelState(posX, posY - 1) == 1)
				clampedSide[0] = true;
			else clampedSide[0] = false;
			if(PScene.GetPixelState(posX + 1, posY) == 1)
				clampedSide[1] = true;
			else clampedSide[1] = false;
			if(PScene.GetPixelState(posX, posY + 1) == 1)
				clampedSide[2] = true;
			else clampedSide[2] = false;
			if(PScene.GetPixelState(posX - 1, posY) == 1)
				clampedSide[3] = true;
			else clampedSide[3] = false;
		}

		public void AddForce(Vector2 force)
		{
			if(!isStatic)
				Velocity += force;
		}
	}
}
