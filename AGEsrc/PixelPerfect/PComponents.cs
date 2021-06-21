﻿using AnimpafGE.ECS;
using AnimpafGE.PixelPerfect.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static System.Diagnostics.Trace;

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
		static readonly Texture2D Pixel = new Texture2D(Core.Graphics.GraphicsDevice, 1, 1);
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
		public bool IsStatic { get; set; } = false;

		int posX, posY;

		readonly bool[] clampedSide = new bool[8];
		PEntity[] sidePixel = new PEntity[8];
		//0- up 1- right 2- down 3- left 4- topright 5- bottomright 6- bottomleft 7- topleft

		int posXE, posYE, dx, dy;

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

			if(!IsStatic)
			{
				float delta = Scene.DeltaTime;

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

				if(Velocity != Vector2.Zero)
				{
					CheckClamping();
					if(clampedSide[1] && Velocity.X > 0)
					{
						PScene.GetPixel(posX + 1, posY).RigidBody.AddForce(Velocity.X, 0);
						Velocity *= Vector2.UnitY;
					}
					else if(clampedSide[3] && Velocity.X < 0)
					{
						PScene.GetPixel(posX - 1, posY).RigidBody.AddForce(Velocity.X, 0);
						Velocity *= Vector2.UnitY;
					}
					if(Velocity.Y < 0)
					{
						if(clampedSide[0])
						{
							PScene.GetPixel(posX, posY - 1).RigidBody.AddForce(0, Velocity.Y);
							Velocity *= Vector2.UnitX;
						}
						else if((clampedSide[4] && Velocity.X > 0) || (clampedSide[7] && Velocity.X < 0))
						{
							PScene.GetPixel(posX + Math.Sign(Velocity.X), posY - 1).RigidBody.AddForce(Velocity);
							Velocity = Vector2.Zero;
						}
					}
					else if(Velocity.Y > 0)
					{
						if(clampedSide[2])
						{
							PScene.GetPixel(posX, posY + 1).RigidBody.AddForce(0, Velocity.Y);
							Velocity *= Vector2.UnitX;
						}
						else if((clampedSide[5] && Velocity.X > 0) || (clampedSide[6] && Velocity.X < 0))
						{
							PScene.GetPixel(posX + Math.Sign(Velocity.X), posY + 1).RigidBody.AddForce(Velocity);
							Velocity = Vector2.Zero;
						}
					}

					if(Velocity != Vector2.Zero)
					{
						Vector2 ExpectedPosition = Transform.Position + Velocity * delta;
						posXE = (int)ExpectedPosition.X / PScene.PixelSize;
						posYE = (int)ExpectedPosition.Y / PScene.PixelSize;
						dx = posXE - posX;
						dy = posYE - posY;

						if(!(dy == 0 && dx == 0))
						{
							if(Math.Abs(dx) < 2 && Math.Abs(dy) < 2)
								Transform.Position = ExpectedPosition;
							else
							{
								Vector2 PathResult = DrawPathBresenham(posX, posY);
								if(PathResult == Vector2.Zero)
									Transform.Position = ExpectedPosition;
								else
									Transform.Position = PathResult * PScene.PixelSize;
							}
						}
						else
							Transform.Position = ExpectedPosition;

						Velocity *= VelocitySave;
					}

					PScene.SetPixelState(Transform.Position, 2);
				}
			}
		}

		public Vector2 DrawPathBresenham(int x, int y)
		{
			int absW = Math.Abs(dx);
			int absH = Math.Abs(dy);

			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if(dx < 0) dx1 = -1;
			else if(dx > 0) dx1 = 1;

			if(dy < 0) dy1 = -1;
			else if(dy > 0) dy1 = 1;

			int longest;
			int shortest;
			if(absW <= absH)
			{
				longest = absH;
				shortest = absW;
				if(dy < 0) dy2 = -1; else if(dy > 0) dy2 = 1;
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
			sidePixel[0] = PScene.GetPixel(posX, posY - 1);
			sidePixel[1] = PScene.GetPixel(posX + 1, posY);
			sidePixel[2] = PScene.GetPixel(posX, posY + 1);
			sidePixel[3] = PScene.GetPixel(posX - 1, posY);
			sidePixel[4] = PScene.GetPixel(posX + 1, posY - 1);
			sidePixel[5] = PScene.GetPixel(posX + 1, posY + 1);
			sidePixel[6] = PScene.GetPixel(posX - 1, posY + 1);
			sidePixel[7] = PScene.GetPixel(posX - 1, posY - 1);
			for(int i = 0; i < 8; i++)
				clampedSide[i] = !(sidePixel[i] is null);
		}

		public void AddForce(Vector2 force)
		{
			if(!IsStatic)
				Velocity += force;
		}
		public void AddForce(int x, int y)
		{
			Vector2 force = new Vector2(x, y);
			if(!IsStatic)
				Velocity += force;
		}
		public void AddForce(float x, float y)
		{
			Vector2 force = new Vector2(x, y);
			if(!IsStatic)
				Velocity += force;
		}
	}
}
