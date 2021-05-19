using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace AnimpafGE.ECS
{
	class Entity
	{
		string Name { get; set; }
		public Vector2 Position { get; set; }
		List<Component> Components { get; set; }

		public Entity()
		{
			Components = new List<Component>();
		}

		public void AddComponent(Component component)
		{
			if(!Components.Contains(component))
			{
				Components.Add(component);
			}
			else Trace.WriteLine($"Добавление компонента {component.ToString()} неудачно:\n" +
				$"объект {this.ToString()} уже имеет данный компонент.");
		}
	}
}
