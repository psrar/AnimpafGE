using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using AnimpafGE.ECS.Components;

namespace AnimpafGE.ECS
{
	class Entity
	{
		public string Name { get; set; }
		public Transform Transform { get; set; }
		List<Component> Components { get; set; } = new List<Component>();

		public Entity(Vector2 position) => Transform = (Transform)AddComponent(new Transform(position));

		public Entity() => Transform = (Transform)AddComponent(new Transform());

		/// <summary>
		/// Обработчик компонентов этого объекта
		/// </summary>
		public void Process()
		{
			foreach(Component component in Components)
				component.Process();
		}

		/// <summary>
		/// Добавляет объекту компонент
		/// </summary>
		/// <param name="component"></param>
		public Component AddComponent(Component component)
		{
			if(!Components.Contains(component))
			{
				Components.Add(component);
				component.Entity = this;
				return component;
			}
			else
			{
				Trace.WriteLine($"Добавление компонента {component.ToString()} неудачно:\n" +
			  $"объект {this.ToString()} уже имеет данный компонент.");
				return null;
			}
		}

		/// <summary>
		/// Получает компонент типа T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetComponent<T>() where T : Component
		{
			foreach(Component component in Components)
				if(component is T)
				{
					Trace.WriteLine($"Component {component.GetType()} found.");
					return (T)component;
				}
			return null;
		}
	}
}
