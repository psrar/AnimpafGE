using System;
using AnimpafGE.ECS;
using AnimpafGE.PixelPerfect.ECS;
using AnimpafGE.ECS.Components;
using AnimpafGE.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

class PSceneSample : PScene
{
	public PSceneSample(Game game, int pixelSize, int width, int height, string name = null)
		: base(game, pixelSize, width, height, name)
	{
		//Write your Constructor code here
	}

	public override void Initialize()
	{
		base.Initialize();

		//Write your Initialize code here
	}

	public override void LoadContent()
	{
		//Write your Loading code here
	}

	public override void Process(GameTime gameTime)
	{
		base.Process(gameTime);

		//Write your Process (Update) code here
	}

	public override void Render(GameTime gameTime)
	{
		long ticks = DateTime.Now.Ticks;
		base.Render(gameTime);

		//Write your Render code here

		Trace.WriteLine("Ticks: " + (DateTime.Now.Ticks - ticks));
	}
}