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
		public Game Game { get; set; }
		public string Name { get; set; }
		public Transform Transform { get; set; }
		List<Component> Components { get; set; } = new List<Component>();

		public Entity(Game game, Vector2 position)
		{
			Game = game;
			Transform = (Transform)AddComponent(new Transform(position));
		}

		public Entity(Game game)
		{
			Game = game;
			Transform = (Transform)AddComponent(new Transform());
		}

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
				Trace.WriteLine($"Добавление компонента {component} неудачно:\n" +
			  $"объект {this} уже имеет данный компонент.");
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
				if(component is T t)
				{
					Trace.WriteLine($"Component {component.GetType()} found.");
					return t;
				}
			return null;
		}
	}
}
