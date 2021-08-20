using AGE.ECS;
using AGE.PixelPerfect.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using AGE.Physics;
using static System.Diagnostics.Trace;

namespace AGE.PixelPerfect.Components
{
	public class PTransform : Component
	{
		public Vector2 Position { get; set; } = Vector2.Zero;
		public Vector2 PixelPosition { get; set; } = Vector2.Zero;
		public Vector2 IndexPosition = Vector2.Zero;
		public int S { get; set; }

		static public Vector2 halfS;

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
		new PEntity Entity { get; set; }
		PTransform Transform;
		PScene PScene { get; set; }

		public Vector2 Velocity = Vector2.Zero;
		public Vector2 Acceleration = Vector2.Zero;
		public float AccelerationSave = 0.95f;
		public float VelocitySave = 0.95f;
		public RigidType Type;

		public bool UseGravity { get; set; } = false;
		public bool IsStatic { get; set; } = false;
		int posX, posY, posXE, posYE, dx, dy;

		readonly bool[] clampedSide = new bool[8];
		readonly PEntity[] sidePixel = new PEntity[8];
		//0- up 1- right 2- down 3- left 4- topright 5- bottomright 6- bottomleft 7- topleft

		public delegate void CollisionHandler(PEntity pixelCollided, Entity collider, Side side = Side.None);
		public event CollisionHandler Collided = delegate { };

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
				if(Entity.ParentComplexObject == null || Entity.ParentComplexObject.RigidBody == null)
					ProcessWithoutComplexObject();
			}

			void ProcessWithoutComplexObject()
			{
				if(UseGravity)
					Velocity = Vector2.Clamp(Velocity + PhysicalConstants.Gravity * Scene.DeltaTime, Vector2.One * -20000, Vector2.One * 20000);

				if(Acceleration != Vector2.Zero)
					Velocity = Vector2.Clamp(Velocity + Acceleration, Vector2.One * -20000, Vector2.One * 20000);

				if(ParentScene.RenderFrame % 10 == 0)
				{
					if(Vector2.Distance(Acceleration, Vector2.Zero) < 3)
						Acceleration = Vector2.Zero;
					if(Vector2.Distance(Velocity, Vector2.Zero) < 3)
						Velocity = Vector2.Zero;
				}

				if(Velocity != Vector2.Zero)
				{
					CheckClamping();
					if(clampedSide[1] && Velocity.X > 0)
					{
						Collided(Entity, PScene.GetPixel(posX + 1, posY).RigidBody.AddForce(Velocity.X, 0), Side.Right);
						Velocity *= Vector2.UnitY;
					}
					else if(clampedSide[3] && Velocity.X < 0)
					{
						Collided(Entity, PScene.GetPixel(posX - 1, posY).RigidBody.AddForce(Velocity.X, 0), Side.Left);
						Velocity *= Vector2.UnitY;
					}
					if(Velocity.Y < 0)
					{
						if(clampedSide[0])
						{
							Collided(Entity, PScene.GetPixel(posX, posY - 1).RigidBody.AddForce(0, Velocity.Y), Side.Top);
							Velocity *= Vector2.UnitX;
						}
						else if((clampedSide[4] && Velocity.X > 0))
						{
							Collided(Entity, PScene.GetPixel(posX + 1, posY - 1).RigidBody.AddForce(Velocity));
							Velocity = Vector2.Zero;
						}
						else if(clampedSide[7] && Velocity.X < 0)
						{
							Collided(Entity, PScene.GetPixel(posX - 1, posY - 1).RigidBody.AddForce(Velocity));
							Velocity = Vector2.Zero;
						}
					}
					else if(Velocity.Y > 0)
					{
						if(clampedSide[2])
						{
							Collided(Entity, PScene.GetPixel(posX, posY + 1).RigidBody.AddForce(0, Velocity.Y), Side.Bottom);
							Velocity *= Vector2.UnitX;
						}
						else if(clampedSide[5] && Velocity.X > 0)
						{
							Collided(Entity, PScene.GetPixel(posX + 1, posY + 1).RigidBody.AddForce(Velocity));
							Velocity = Vector2.Zero;
						}
						else if(clampedSide[6] && Velocity.X < 0)
						{
							Collided(Entity, PScene.GetPixel(posX - 1, posY + 1).RigidBody.AddForce(Velocity));
							Velocity = Vector2.Zero;
						}
					}

					if(Velocity != Vector2.Zero)
					{
						Vector2 ExpectedPosition = Transform.Position + Velocity * Scene.DeltaTime;
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

					PScene.MovePixel(posX, posY, Transform.Position);
				}
			}
		}

		public void ResolveCollisionsInCO()
		{
			PComplexRigidBody complexBody = Entity.ParentComplexObject.RigidBody;
			Vector2 localVelocity = complexBody.LocalVelocity;
			if(localVelocity != Vector2.Zero)
			{
				CheckClamping();
				if(clampedSide[1] && localVelocity.X > 0)
					Collided(Entity, PScene.GetPixel(posX + 1, posY).RigidBody.AddForce(localVelocity.X, 0), Side.Right);
				else if(clampedSide[3] && localVelocity.X < 0)
					Collided(Entity, PScene.GetPixel(posX - 1, posY).RigidBody.AddForce(localVelocity.X, 0), Side.Left);
				if(localVelocity.Y < 0)
				{
					if(clampedSide[0])
						Collided(Entity, PScene.GetPixel(posX, posY - 1).RigidBody.AddForce(0, localVelocity.Y), Side.Top);
					else if((clampedSide[4] && localVelocity.X > 0))
						Collided(Entity, PScene.GetPixel(posX + 1, posY - 1).RigidBody.AddForce(localVelocity));
					else if(clampedSide[7] && localVelocity.X < 0)
						Collided(Entity, PScene.GetPixel(posX - 1, posY - 1).RigidBody.AddForce(localVelocity));
				}
				else if(localVelocity.Y > 0)
				{
					if(clampedSide[2])
						Collided(Entity, PScene.GetPixel(posX, posY + 1).RigidBody.AddForce(0, localVelocity.Y), Side.Bottom);
					else if(clampedSide[5] && localVelocity.X > 0)
						Collided(Entity, PScene.GetPixel(posX + 1, posY + 1).RigidBody.AddForce(localVelocity));
					else if(clampedSide[6] && localVelocity.X < 0)
						Collided(Entity, PScene.GetPixel(posX - 1, posY + 1).RigidBody.AddForce(localVelocity));
				}
				Vector2 ExpectedPosition = Transform.Position + localVelocity * Scene.DeltaTime;
				posXE = (int)ExpectedPosition.X / PScene.PixelSize;
				posYE = (int)ExpectedPosition.Y / PScene.PixelSize;
				dx = posXE - posX;
				dy = posYE - posY;

				if(!(dy == 0 && dx == 0) && !(Math.Abs(dx) < 2 && Math.Abs(dy) < 2))
					DrawPathBresenham(posX, posY);
			}
		}

		public void MoveInCO()
		{
			PComplexRigidBody complexBody = Entity.ParentComplexObject.RigidBody;
			Vector2 localVelocity = complexBody.LocalVelocity;

			if((localVelocity = complexBody.LocalVelocity) != Vector2.Zero)
			{
				Vector2 ExpectedPosition = Transform.Position + localVelocity * Scene.DeltaTime;
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
						ExpectedPosition = Transform.Position + localVelocity * Scene.DeltaTime;
						if(PathResult == Vector2.Zero)
							Transform.Position = ExpectedPosition;
						else
							Transform.Position = PathResult * PScene.PixelSize;
					}
				}
				else
					Transform.Position = ExpectedPosition;
			}

			PScene.MovePixel(posX, posY, Transform.Position);
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
					{
						if(Entity.ParentComplexObject == null)
						{
							Collided(Entity, PScene.GetPixel(x, y));
							return new Vector2(x - dx1, y - dy1);
						}
						else
						{
							PEntity collider = PScene.GetPixel(x, y);
							if(Entity.ParentComplexObject != collider.ParentComplexObject)
							{
								Collided(Entity, collider);
								return new Vector2(x - dx1, y - dy1);
							}
						}
					}
				}
				else
				{
					x += dx2;
					y += dy2;

					if(PScene.GetPixelState(x, y) != 0)
					{
						if(Entity.ParentComplexObject == null)
						{
							Collided(Entity, PScene.GetPixel(x, y));
							return new Vector2(x - dx2, y - dy2);
						}
						else
						{
							PEntity collider = PScene.GetPixel(x, y);
							if(Entity.ParentComplexObject != collider.ParentComplexObject)
							{
								Collided(Entity, collider);
								return new Vector2(x - dx2, y - dy2);
							}
						}
					}
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
			if(Entity.ParentComplexObject is null)
				for(int i = 0; i < 8; i++)
					clampedSide[i] = !(sidePixel[i] is null);
			else
				for(int i = 0; i < 8; i++)
					clampedSide[i] = !(sidePixel[i] is null ||
						(Entity.ParentComplexObject != null && sidePixel[i].ParentComplexObject == Entity.ParentComplexObject));
		}

		public PEntity AddForce(Vector2 force)
		{
			if(!IsStatic)
				Velocity += force;
			return Entity;
		}
		public PEntity AddForce(int x, int y)
		{
			Vector2 force = new Vector2(x, y);
			if(!IsStatic)
				Velocity += force;
			return Entity;
		}
		public PEntity AddForce(float x, float y)
		{
			Vector2 force = new Vector2(x, y);
			if(!IsStatic)
				Velocity += force;
			return Entity;
		}

		public void OnLocalForceAdded(Vector2 force)
		{
			ResolveCollisionsInCO();
		}
	}
}
