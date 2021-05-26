using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimpafGE.ECS;
using Microsoft.Xna.Framework;

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
			//throw new NotImplementedException();
		}

		public override void Process()
		{
			//throw new NotImplementedException();
		}

		public new void Render()
		{
			base.Render();
		}
	}
}
