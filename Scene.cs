using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimpafGE.ECS
{
	class Scene
	{
		public string Name { get; set; }
		public List<Entity> Objects { get; set; } = new List<Entity>();
	}
}
