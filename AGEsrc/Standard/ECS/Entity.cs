using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using AnimpafGE.ECS.Components;
using AnimpafGE.PixelPerfect;
using AnimpafGE.PixelPerfect.ECS;

namespace AnimpafGE.ECS
{
	public class Entity
	{
		public Scene ParentScene { get; set; }
		public string ID { get; set; }
		public Transform Transform { get; set; }
		List<Component> Components { get; set; } = new List<Component>();

		public Entity(Scene scene)
		{
			ID = this.GetHashCode().ToString();
			ParentScene = scene;

			if(GetType() != typeof(PEntity) && GetType() != typeof(PComplexEntity))
			{
				Transform = AddComponent<Transform>();
				scene.Objects.Add(this);
			}
		}
		public Entity(Scene scene, Vector2 position)
		{
			ParentScene = scene;
			ID = this.GetHashCode().ToString();

			if(GetType() != typeof(PEntity) && GetType() != typeof(PComplexEntity))
			{
				Transform = AddComponent<Transform>().SetPosition(position);
				scene.Objects.Add(this);
			}
		}

		/// <summary>
		/// Обработчик компонентов этого объекта
		/// </summary>
		public virtual void Process()
		{
			foreach(Component component in Components)
				component.Process();
		}

		/// <summary>
		/// Добавляет объекту компонент
		/// </summary>
		/// <param name="component"></param>
		public T AddComponent<T>() where T : Component, new()
		{
			T component = new T
			{
				Entity = this,
				ParentScene = ParentScene
			};
			component.Init();

			if(!Components.OfType<T>().Any())
			{
				Components.Add(component);
				return component;
			}
			else
			{
				Trace.WriteLine($"Добавление компонента {component} невозможно:\n" +
			  $"объект {ID} уже имеет данный компонент.");
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
				return null;
		}
	}
}
