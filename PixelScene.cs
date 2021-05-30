using System;
using AnimpafGE.ECS;
using AnimpafGE.ECS.Components;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace AnimpafGE.ECS
{
	class PixelScene : Scene
	{
		public PixelScene(Game game, string name = null) : base(game)
		{
			Name = name ?? "SimpleScene";
		}

		public override void Initialize()
		{
			base.Initialize();

			//Write your initialization logic here
		}

		public override void LoadContent()
		{
			//Write your loading code here
		}

		public override void Process()
		{
			//Write your Process (Update) code here
		}

		public override void Render()
		{
			base.Render();

			//Write your render code here
		}
	}
}