using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AGE;
using AGE.ECS;

namespace AGE.ECS
{
	public class SampleGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		Scene activeScene;

		public SampleGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.IsFullScreen = true;
			_graphics.ApplyChanges();
			Core.GraphicsManager = _graphics;
			Core.Content = Content;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			activeScene = new SceneTemplate(this, "MainScene"); //Put your scene here
			activeScene.Initialize();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here

			activeScene.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			activeScene.Process(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			activeScene.Render(gameTime);

			base.Draw(gameTime);
		}
	}
}
