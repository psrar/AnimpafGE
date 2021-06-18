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
		static public Vector2 Gravity = new Vector2(0, 9800 * 2);

		new PEntity Entity { get; set; }
		PTransform Transform;
		PScene PScene { get; set; }

		public Vector2 Velocity { get; set; } = Vector2.Zero;
		public Vector2 Acceleration { get; set; } = Vector2.Zero;
		public float AccelerationSave { get; set; } = 0.95f;
		public float VelocitySave { get; set; } = 0.1f;
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

				if(Acceleration == Vector2.Zero)
					Velocity *= VelocitySave;
			}
		}
	}
}
