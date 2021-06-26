using AnimpafGE.PixelPerfect.ECS;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using AnimpafGE.ECS;
using AnimpafGE.PixelPerfect.Components;
using AnimpafGE.Physics;

namespace AnimpafGE.PixelPerfect
{
	public class PComplexEntity : Entity
	{
		public PScene PScene { get; set; }

		public new PComplexTransform Transform { get; set; }
		public PComplexRigidBody RigidBody { get; set; }
		public List<PEntity> CapturedPixels { get; set; } = new List<PEntity>();

		public PComplexEntity(Scene scene) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PComplexTransform>();
			RigidBody = AddComponent<PComplexRigidBody>();
			scene.Objects.Add(this);
		}
		public PComplexEntity(Scene scene, params PEntity[] pEntities) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PComplexTransform>();
			RigidBody = AddComponent<PComplexRigidBody>();
			CapturePixel(pEntities);

			scene.Objects.Add(this);
		}
		public PComplexEntity(Scene scene, Vector2 position) : base(scene, position)
		{
			PScene = (PScene)scene;
			ID = this.GetHashCode().ToString();

			if(GetType() != typeof(AnimpafGE.PixelPerfect.ECS.PEntity))
			{
				Transform = AddComponent<PComplexTransform>();
				scene.Objects.Add(this);
			}
		}

		public PEntity CapturePixel(params PEntity[] pixels)
		{
			foreach(var pixel in pixels)
			{
				CapturedPixels.Add(pixel);
				RigidBody.CaptureRigidBody(pixel);
				pixel.ParentComplexObject = this;
				CenterPositions();
			}
			return pixels.Last();
		}

		public void CenterPositions()
		{
			foreach(PEntity entity in CapturedPixels)
				entity.Transform.Position = entity.Transform.IndexPosition * PScene.PixelSize;
		}
	}

	public class PComplexTransform : Component
	{
		public Vector2 CenterPosition { get; private set; }

		public override void Init()
		{

		}

		public override void Process()
		{

		}

		public void CalculateCenterPosition()
		{

		}
	}

	public class PComplexRigidBody : Component
	{
		public new PComplexEntity Entity { get; set; }
		public List<PRigidBody> CapturedRigidBodies { get; set; } = new List<PRigidBody>();

		public Vector2 LocalVelocity { get; set; } = Vector2.Zero;
		public bool UseGravity { get; set; } = false;

		public delegate void ForceHandler(Vector2 force);
		public event ForceHandler LocalForceAdded = delegate { };

		public delegate void ObjectCollisionHandler(PEntity pixelCollided, Entity collider, Side side);
		public event ObjectCollisionHandler ObjectCollided = delegate { };

		public override void Init()
		{
			Entity = (PComplexEntity)base.Entity;

			foreach(PEntity Child in ((PComplexEntity)Entity).CapturedPixels)
				if(Child.RigidBody != null)
				{
					CapturedRigidBodies.Add(Child.RigidBody);
					LocalForceAdded += Child.RigidBody.OnLocalForceAdded;
				}
		}

		public override void Process()
		{
			if(UseGravity)
				LocalVelocity = Vector2.Clamp(LocalVelocity + PhysicalConstants.Gravity * Scene.DeltaTime, Vector2.One * -20000, Vector2.One * 20000);

			if(ParentScene.RenderFrame % 10 == 0)
			{
				if(Vector2.Distance(LocalVelocity, Vector2.Zero) < 10)
					LocalVelocity = Vector2.Zero;
			}

			foreach(PRigidBody pBody in CapturedRigidBodies)
			{

			}
		}

		public void CaptureRigidBody(PEntity pEntity)
		{
			if(Entity.CapturedPixels.Contains(pEntity))
			{
				if(pEntity.RigidBody != null)
				{
					CapturedRigidBodies.Add(pEntity.RigidBody);
					LocalForceAdded += pEntity.RigidBody.OnLocalForceAdded;
					pEntity.RigidBody.Collided += OnPixelCollided;
					Entity.CenterPositions();
				}
				else
					Trace.TraceError("У объекта нет компонента PRigidBody");
			}
			else throw new Exception("Попытка захватить компонент PRigidBody в PComplexRigidBody, " +
				"пока объект не привязан к объекту PComplexEntity. " +
				"Сначала используйте метод CapturePixel класса PComplexEntity.");
		}

		public void AddForce(Vector2 force)
		{
			LocalForceAdded(force);
		}
		public void AddForce(int x, int y)
		{
			Vector2 force = new Vector2(x, y);
			LocalForceAdded(force);
		}
		public void AddForce(float x, float y)
		{
			Vector2 force = new Vector2(x, y);
			LocalForceAdded(force);
		}

		public void OnPixelCollided(PEntity pixelCollided, Entity collider, Side side)
		{
			if(collider is PEntity && pixelCollided.ParentComplexObject != (collider as PEntity).ParentComplexObject)
				ObjectCollided(pixelCollided, collider, side);
		}
	}
}
