using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AnimpafGE.ECS.Components;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace AnimpafGE.ECS
{
	class Scene
	{
		Game ParentGame { get; set; }

		public string Name { get; set; }
		public List<Entity> Objects { get; set; } = new List<Entity>();

		Entity player;
		public Camera2D cam;

		const float playerSpeed = 10f;

		public Scene(Game game)
		{
			player = new(this);
			ParentGame = game;

			Core.Graphics.PreferredBackBufferWidth = 1920;
			Core.Graphics.PreferredBackBufferHeight = 1080;
			Core.Graphics.IsFullScreen = true;
			Core.Graphics.ApplyChanges();
		}

		public void Initialize()
		{
			cam = new Camera2D();

			var Center = new Vector2(Core.Graphics.PreferredBackBufferWidth / 2, Core.Graphics.PreferredBackBufferHeight / 2);

			player = new Entity(this);
			player.AddComponent(new SpriteRenderer());
			player.Transform.Position = Center;
			cam.Pos = Center;
		}

		public void Process()
		{
			// TODO: Add your update logic here

			if(Keyboard.GetState().IsKeyDown(Keys.W))
				player.Transform.Translate(Vector2.UnitY * -playerSpeed);
			if(Keyboard.GetState().IsKeyDown(Keys.D))
				player.Transform.Translate(Vector2.UnitX * playerSpeed);
			if(Keyboard.GetState().IsKeyDown(Keys.S))
				player.Transform.Translate(Vector2.UnitY * playerSpeed);
			if(Keyboard.GetState().IsKeyDown(Keys.A))
				player.Transform.Translate(Vector2.UnitX * -playerSpeed);
			if(Keyboard.GetState().IsKeyDown(Keys.OemCloseBrackets))
				player.Transform.Scale(Vector2.One * -playerSpeed / 8);
			if(Keyboard.GetState().IsKeyDown(Keys.OemOpenBrackets))
				player.Transform.Scale(Vector2.One * playerSpeed / 8);

			if(Keyboard.GetState().IsKeyDown(Keys.T))
				cam.Move(Vector2.UnitY * -playerSpeed / 10);
			if(Keyboard.GetState().IsKeyDown(Keys.H))
				cam.Move(Vector2.UnitX * playerSpeed / 10);
			if(Keyboard.GetState().IsKeyDown(Keys.G))
				cam.Move(Vector2.UnitY * playerSpeed / 10);
			if(Keyboard.GetState().IsKeyDown(Keys.F))
				cam.Move(Vector2.UnitX * -playerSpeed / 10);
			if(Keyboard.GetState().IsKeyDown(Keys.OemPlus))
				cam.Zoom += playerSpeed / 20;
			if(Keyboard.GetState().IsKeyDown(Keys.OemMinus))
				cam.Zoom += -playerSpeed / 20;
		}

		public void Render()
		{
			foreach(Entity entity in Objects)
				entity.Process();
		}

	}
}
