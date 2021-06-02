﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BIT;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AnimpafGE.ECS.Components
{
	/// <summary>
	/// Компонент, задающий расположение и масштаб объекта на сцене.
	/// Принадлежит каждому объекту по умолчанию.
	/// </summary>
	class Transform : Component
	{
		/// <summary>Расположение объекта на сцене</summary>
		public Vector2 Position { get; set; } = Vector2.Zero;
		/// <summary>Масштаб объекта</summary>
		public Vector2 Scaling { get; set; } = Vector2.One;
		/// <summary>Вращение объекта</summary>
		public float Rotation { get; set; } = 0;

		#region Setters
		public Transform SetPosition(Vector2 position)
		{
			Position = position;
			return this;
		}
		public Transform SetScaling(Vector2 scaling)
		{
			Scaling = scaling;
			return this;
		}
		public Transform SetRotation(float rotation)
		{
			Rotation = rotation;
			return this;
		}
		#endregion

		/// <summary>Метод грубого (нефизичного) перемещения объекта</summary>
		/// <param name="translation">Вектор перемещения</param>
		public void Translate(Vector2 translation) => Position += translation;

		/// <summary>Изменение размеров объекта</summary>
		/// <param name="scaling">Вектор увеличения</param>
		public void Scale(Vector2 scaling) => Scaling += scaling;
		/// <summary>Изменение размеров объекта</summary>
		/// <param name="scaling">Вектор увеличения</param>
		public void Scale(int scaling) => Scaling += new Vector2(scaling, scaling);
	}

	/// <summary>
	/// Компонент, отвечающий за отрисовку спрайта на сцене.
	/// </summary>
	class SpriteRenderer : Component
	{
		public Texture2D Sprite { get; set; }
		SpriteBatch Batch { get; set; }
		public Color Color { get; set; } = Color.White;
		int Layer { get; set; } = 0;

		public override void Init()
		{
			Batch = Entity.ParentScene.spriteBatch;
		}

		public override void Process()
		{
			if(!(Sprite is null))
			{
				Batch.Draw(Sprite,                          // Texture
					Entity.Transform.Position,              // Position
					null,                                   // Source rectangle
					Color,                                  // Color
					0,                                      // Rotation
					Sprite.Bounds.Size.ToVector2() / 2,     // Origin
					Entity.Transform.Scaling,               // Scale
					SpriteEffects.None,                     // Mirroring effect
					Layer);                                 // Depth
			}
		}

		public void SetRandomColor()
		{
			Random rnd = new();
			Color = new Color(rnd.Next(20), rnd.Next(20), rnd.Next(235, 256));
		}
	}

	/// <summary>
	/// Компонент, отвечающий за физику объекта
	/// </summary>
	class RigidBody : Component
	{
		enum RigidType
		{
			/// <summary>К статическим объектам не применяется никакая сила</summary>
			Static = 0,
			/// <summary>Динамические объекты перемещаются свободно под действием внешних сил или кода</summary>
			Dynamic = 1
		}

		public bool UseGravity { get; set; }
		Vector2 Gravity = new Vector2(0, 9800/2);

		public Vector2 Velocity { get; set; }

		float deltaTime;

		public override void Process()
		{
			deltaTime = Entity.ParentScene.GameTime.ElapsedGameTime.Milliseconds / 1000f;
			if(UseGravity)
				Velocity += Gravity * deltaTime;
			Entity.Transform.Position += Velocity * deltaTime;
		}
	}

	class BoxCollider : Component
	{
		Rectangle Collider { get; set; }

		public override void Init()
		{
			Collider = new Rectangle(Entity.Transform.Position.ToPoint(), Point.Zero);
		}
	}
}
