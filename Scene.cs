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
	abstract class Scene
	{
		protected Game ParentGame { get; set; }
		protected ContentManager Content { get; set; }

		public string Name { get; set; }
		public List<Entity> Objects { get; set; } = new List<Entity>();

		protected Scene(Game game)
		{
			ParentGame = game;
			Content = ParentGame.Content;
		}

		public abstract void Initialize();

		public abstract void Process();

		public void Render()
		{
			foreach(Entity entity in Objects)
				entity.Process();
		}
	}
}
