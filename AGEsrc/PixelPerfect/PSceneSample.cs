using System;
using AGE.ECS;
using AGE.PixelPerfect.ECS;
using AGE.Input;
using AGE.ECS.Components;
using AGE.Graphics;
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
		InputProcessor.ButtonClicked += OnButtonClicked;
		InputProcessor.ButtonReleased += OnButtonReleased;
		InputProcessor.ButtonHeld += OnButtonHeld;
		InputProcessor.TouchHeld += OnTouchHeld;

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


	private void OnButtonClicked(Keys key)
	{
		throw new NotImplementedException();
	}
	private void OnButtonReleased(Keys key)
	{
		throw new NotImplementedException();
	}
	private void OnButtonHeld(Keys key)
	{
		throw new NotImplementedException();
	}
	private void OnTouchHeld(Vector2 touchPosition)
	{
		throw new NotImplementedException();
	}
}