using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Reflection;
using AGE.ECS.Components;
using AGE.PixelPerfect;
using AGE.PixelPerfect.ECS;

namespace AGE.ECS
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
				Transform = AddComponent<Transform>();
				Transform.Position = position;
				scene.Objects.Add(this);
			}
		}

		/// <summary>
		/// Обработчик компонентов этого объекта
		/// </summary>
		public virtual void Process()
		{
			foreach(Component component in Components)
				if(component.Enabled)
					component.Process();
		}

		/// <summary>
		/// Добавляет объекту компонент
		/// </summary>
		/// <param name="component"></param>
		public T AddComponent<T>() where T : Component, new()
		{
			if(Components.OfType<T>().Any())
				throw new Exception($"Добавление компонента {typeof(T)} невозможно:\n" +
			  $"объект {ID} уже имеет данный компонент.");

			T component = new T
			{
				Entity = this,
				ParentScene = ParentScene
			};
			component.Init();

			Components.Add(component);

			if(typeof(T) == typeof(BoxCollider))
				ParentScene.Colliders.Add(component as BoxCollider);

			return component;
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
