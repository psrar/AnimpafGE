using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using AGE.ECS.Components;
using AGE.ECS;
using Microsoft.Xna.Framework;

namespace AGE.ECS
{
	public abstract class Component
	{
		static readonly Type[] Unprocessable = new Type[] { typeof(Transform), typeof(Animator) };
		public Entity Entity { get; set; }
		public Scene ParentScene { get; set; }
		public string Name { get; set; }
		public bool Enabled { get; set; } = true;

		public Component()
		{
			Name = this.GetType().ToString();
		}

		public virtual void Init() { }

		public virtual void Process()
		{
			if(!Unprocessable.Contains(this.GetType()))
				if(Enabled)
					Process();
		}
	}
}
