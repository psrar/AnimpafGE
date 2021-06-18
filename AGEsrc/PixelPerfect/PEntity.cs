using AnimpafGE.ECS;
using AnimpafGE.PixelPerfect.Components;
using Microsoft.Xna.Framework;

namespace AnimpafGE.PixelPerfect.ECS
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
}
