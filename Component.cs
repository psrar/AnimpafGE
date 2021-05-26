using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using AnimpafGE.ECS.Components;
using Microsoft.Xna.Framework;

namespace AnimpafGE.ECS
{
	class Component
	{
		public Entity Entity { get; set; }
		public string Name { get; set; }
		bool Enabled { get; set; } = true;

		public Component()
		{
			Name = this.GetType().ToString();
		}

		public virtual void Process()
		{
			if(!(this is Transform))
			{
				if(Enabled)
					this.Process();
			}
		}
	}
}
