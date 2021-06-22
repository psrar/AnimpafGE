using AnimpafGE.PixelPerfect.ECS;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using AnimpafGE.ECS;
using AnimpafGE.PixelPerfect.Components;

namespace AnimpafGE.PixelPerfect
{
	public class PComplexEntity : Entity
	{
		public new PComplexTransform Transform { get; set; }
		public PComplexRigidBody RigidBody { get; set; }
		public List<PEntity> CapturedPixels { get; set; } = new List<PEntity>();

		public PComplexEntity(Scene scene) : base(scene)
		{
			Transform = AddComponent<PComplexTransform>();
			RigidBody = AddComponent<PComplexRigidBody>();
			scene.Objects.Add(this);
		}
		public PComplexEntity(Scene scene, params PEntity[] pEntities) : base(scene)
		{
			Transform = AddComponent<PComplexTransform>();
			RigidBody = AddComponent<PComplexRigidBody>();
			CapturePixel(pEntities);

			scene.Objects.Add(this);
		}
		public PComplexEntity(Scene scene, Vector2 position) : base(scene, position)
		{
			ParentScene = scene;
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
			}
			return pixels.Last();
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
		public List<PRigidBody> ChildrenBodies { get; set; } = new List<PRigidBody>();

		public delegate void ForceHandler(Vector2 force);
		public event ForceHandler LocalForceAdded = delegate { };

		public delegate void ObjectCollisionHandler(PEntity pixelCollided, Entity collider);
		public event ObjectCollisionHandler ObjectCollided = delegate { };

		public override void Init()
		{
			Entity = (PComplexEntity)base.Entity;

			foreach(PEntity Child in ((PComplexEntity)Entity).CapturedPixels)
				if(Child.RigidBody != null)
				{
					ChildrenBodies.Add(Child.RigidBody);
					LocalForceAdded += Child.RigidBody.OnLocalForceAdded;
				}
		}

		public void CaptureRigidBody(PEntity pEntity)
		{
			if(Entity.CapturedPixels.Contains(pEntity))
			{
				if(pEntity.RigidBody != null)
				{
					ChildrenBodies.Add(pEntity.RigidBody);
					LocalForceAdded += pEntity.RigidBody.OnLocalForceAdded;
					pEntity.RigidBody.Collided += OnPixelCollided;
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

		public override void Process() { }

		public void OnPixelCollided(PEntity pixelCollided, Entity collider)
		{
			if(collider is PEntity && pixelCollided.ParentComplexObject != (collider as PEntity).ParentComplexObject)
				ObjectCollided(pixelCollided, collider);
		}
	}
}
