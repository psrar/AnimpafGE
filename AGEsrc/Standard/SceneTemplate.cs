using System;
using AGE.ECS;
using AGE.ECS.Components;
using AGE.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace AGE.ECS
{
	class SceneTemplate : Scene
	{
		InputProcessor BasicInput;

		public SceneTemplate(Game game, string name = null) : base(game)
		{
			Name = name ?? "SimpleScene";
		}

		public override void Initialize()
		{
			base.Initialize();

			//Write your initialization logic here

			BasicInput = new InputProcessor(this);
		}

		public override void LoadContent()
		{
			BasicInput.ButtonClicked += OnButtonClicked;
			BasicInput.ButtonReleased += OnButtonReleased;
			BasicInput.ButtonHeld += OnButtonHeld;
			BasicInput.TouchHeld += OnTouchHeld;

			//Write your loading code here
		}


		public override void Process(GameTime gameTime)
		{
			base.Process(gameTime);

			//Write your Process (Update) code here
		}

		public override void Render(GameTime gameTime)
		{
			base.Render(gameTime);

			//Write your render code here
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
}
