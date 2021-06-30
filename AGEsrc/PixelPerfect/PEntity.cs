using AGE.ECS;
using AGE.PixelPerfect.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AGE.PixelPerfect.ECS
{
	public class PEntity : Entity
	{
		public PScene PScene { get; set; }

		public new PTransform Transform { get; set; }
		public PRigidBody RigidBody { get; set; }
		public PComplexEntity ParentComplexObject { get; set; }
		public PRenderer Renderer { get; set; }

		public int Index { get; set; }

		public PEntity(Scene scene) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PTransform>();
			RigidBody = AddComponent<PRigidBody>();
			Renderer = AddComponent<PRenderer>();
		}
		public PEntity(Scene scene, Vector2 position, bool isVisible = true) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PTransform>();
			Transform.Position = position;
			RigidBody = AddComponent<PRigidBody>();
			Renderer = AddComponent<PRenderer>();
			Renderer.Enabled = isVisible;
		}
		public PEntity(Scene scene, Vector2 position, Color color, bool isVisible = true) : base(scene)
		{
			PScene = (PScene)scene;
			Transform = AddComponent<PTransform>();
			Transform.Position = position;
			Transform.CalculateIndex();
			RigidBody = AddComponent<PRigidBody>();
			Renderer = AddComponent<PRenderer>();
			Renderer.Color = color;
			Renderer.Enabled = isVisible;
		}

		public override void Process()
		{
			base.Process();

			Transform.CalculateIndex();
			Index = (int)(Transform.IndexPosition.X + PScene.VirtualWidth * Transform.IndexPosition.Y);
		}
	}
}
