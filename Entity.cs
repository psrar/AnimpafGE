using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using AnimpafGE.ECS.Components;

namespace AnimpafGE.ECS
{
	class Entity
	{
		public Scene ParentScene { get; set; }
		public string Name { get; set; }
		public Transform Transform { get; set; }
		List<Component> Components { get; set; } = new List<Component>();

		public Entity(Scene scene, Vector2 position)
		{
			ParentScene = scene;
			Name = this.GetHashCode().ToString();
			Transform = (Transform)AddComponent<Transform>(new Transform(position));
		}

		public Entity(Scene scene)
		{
			ParentScene = scene;
			Name = this.GetHashCode().ToString();
			Transform = (Transform)AddComponent<Transform>(new Transform());

			scene.Objects.Add(this);
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
		public Component AddComponent<T>(Component component) where T : Component
		{

			if(!Components.OfType<T>().Any())
			{
				Components.Add(component);
				component.Entity = this;
				return component;
			}
			else
			{
				Trace.WriteLine($"Добавление компонента {component} невозможно:\n" +
			  $"объект {Name} уже имеет данный компонент.");
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
			List<T> complist = Components.OfType<T>().ToList();
			if(complist.Any())
				return complist[0];
			else
				throw new Exception("Попытка найти несуществующий или неприкрепленный компонент для объекта " + Name);
			//foreach(Component component in Components)
			//	if(component is T t)
			//	{
			//		Trace.WriteLine($"Component {component.GetType()} found.");
			//		return t;
			//	}
		}
	}
}
