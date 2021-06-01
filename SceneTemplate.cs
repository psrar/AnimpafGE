using System;
using AnimpafGE.ECS;
using AnimpafGE.ECS.Components;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace AnimpafGE.ECS
{
	class SceneTemplate : Scene
	{
		public SceneTemplate(Game game, string name = null) : base(game)
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

		public override void Process(GameTime gameTime)
		{
			//Write your Process (Update) code here
		}

		public override void Render(GameTime gameTime)
		{
			base.Render(gameTime);

			//Write your render code here
		}
	}
}
