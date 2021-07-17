using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static AGE.Extensions;
using AGE.Physics;
using AGE.Graphics;
using System.Threading;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AGE.ECS.Components
{
	/// <summary>Компонент, задающий расположение и масштаб объекта на сцене.<br/>
	/// Принадлежит каждому объекту по умолчанию.</summary>
	public class Transform : Component
	{
		/// <summary>Расположение объекта на сцене</summary>
		public Vector2 Position { get; set; } = Vector2.Zero;
		/// <summary>Масштаб объекта</summary>
		public Vector2 Scaling { get; private set; } = Vector2.One;
		/// <summary>Вращение объекта</summary>
		public float Rotation { get; set; } = 0;

		/// <summary>Метод грубого (нефизичного) перемещения объекта</summary>
		/// <param name="translation">Вектор перемещения</param>
		public void Translate(Vector2 translation) => Position += translation;


		/// <summary>Устанавливает вращение в градусах</summary>
		public void Rotate(float angle) => Rotation = MathHelper.ToRadians(angle);

		/// <summary>Перемещение объекта</summary>
		public void Locate(Vector2 position) => Position = position;
		/// <summary>Перемещение объекта</summary>
		public void Locate(float position) => Position = new Vector2(position);

		public void SetScaling(Vector2 scaling)
		{
			Scaling = scaling;
			ScaleChanged();
		}
		public void SetScaling(float scaling)
		{
			Scaling = new Vector2(scaling);
			ScaleChanged();
		}
		public void Scale(Vector2 scaling)
		{
			Scaling += scaling;
			ScaleChanged();
		}
		public void Scale(float scaling)
		{
			Scaling += new Vector2(scaling, scaling);
			ScaleChanged();
		}

		public delegate void ScaleHandler();
		public event ScaleHandler ScaleChanged = delegate { };
	}

	/// <summary>Компонент, отвечающий за отрисовку спрайта на сцене.</summary>
	public class SpriteRenderer : Component
	{
		public Texture2D Sprite { get; set; }
		SpriteBatch Batch { get; set; }
		public Color Color { get; set; } = Color.White;
		public SpriteEffects Mirroring { get; set; } = SpriteEffects.None;
		int Layer { get; set; } = 0;

		public Vector2 TopLeft { get; set; }
		public Vector2 BottomRight { get; set; }


		public override void Init()
		{
			Batch = ParentScene.spriteBatch;
			if(Sprite != null)
			{
				TopLeft = new Vector2(Sprite.Bounds.Size.X * Entity.Transform.Scaling.X / 2,
					Sprite.Bounds.Size.Y * Entity.Transform.Scaling.Y / 2);
				BottomRight = TopLeft;
			}
			else
			{
				TopLeft = Entity.Transform.Position;
				BottomRight = TopLeft;
			}
		}

		public override void Process()
		{
			if(Sprite != null)
			{
				Batch.Draw(Sprite,                          // Texture
					Entity.Transform.Position,              // Position
					null,                                   // Source rectangle
					Color,                                  // Color
					Entity.Transform.Rotation,              // Rotation
					Sprite.Bounds.Size.ToVector2() / 2,     // Origin
					Entity.Transform.Scaling,               // Scale
					Mirroring,                              // Mirroring effect
					Layer);                                 // Depth
			}
		}

		public void SetRandomColor()
		{
			Random rnd = new Random();
			Color = new Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
		}
	}

	/// <summary>Компонент, отвечающий за перемещение объекта.</summary>
	public class RigidBody : Component
	{
		public RigidType RigidType { get; set; } = RigidType.Dynamic;
		public bool UseGravity { get; set; } = true;
		Vector2 Gravity = new Vector2(0, 9800 / 2);

		public Vector2 Velocity { get; set; }

		float deltaTime;

		public override void Process()
		{
			deltaTime = ParentScene.GameTime.ElapsedGameTime.Milliseconds / 1000f;
			if(UseGravity)
				Velocity += Gravity * deltaTime;

			Entity.Transform.Position += Velocity * deltaTime;
		}

		public void OnCollision(Side side)
		{
			switch(side)
			{
				case Side.Top:
					break;
				case Side.Right:
					break;
				case Side.Bottom:
					Entity.Transform.Position = new Vector2(Entity.Transform.Position.X,
						Entity.ParentScene.maxCoord.Y - 140);
					if(Velocity.Y > 0)
						Velocity *= Vector2.UnitX;
					break;
				case Side.Left:
					break;
			}
		}
	}

	/// <summary>Компонент, задающий границы коллайдера для объекта</summary>
	public class BoxCollider : Component
	{
		public Rectangle Collider = new Rectangle();

		public delegate void CollisionHandler(Side side);
		public event CollisionHandler Collided;

		public override void Init()
		{
			if(Entity.GetComponent<RigidBody>() is null)
				throw new Exception("Компонент BoxCollider требует наличия у объекта компонента RigidBody");

			Collided += Entity.GetComponent<RigidBody>().OnCollision;

			Entity.Transform.ScaleChanged += OnTransformScaled;
		}

		public BoxCollider SetSize(int size)
		{
			Collider = new Rectangle(IncrementVector(Entity.Transform.Position, -size / 2).ToPoint(),
				new Point(size));
			return this;
		}
		public BoxCollider SetSize(int width, int height)
		{
			Vector2 size = new Vector2(width, height) * Entity.Transform.Scaling;
			Collider = new Rectangle((Entity.Transform.Position - size / 2).ToPoint(),
				new Point(width, height));
			return this;
		}

		private void OnTransformScaled()
		{
			int newSizeX = Collider.Width * (int)Entity.Transform.Scaling.X;
			int newSizeY = Collider.Height * (int)Entity.Transform.Scaling.Y;
			Collider =
				new Rectangle((Entity.Transform.Position - new Vector2(newSizeX, newSizeY) / 2).ToPoint(),
				new Point(newSizeX, newSizeY));
		}

		public override void Process()
		{
			Collider.Location = (Entity.Transform.Position - Collider.Size.ToVector2() / 2).ToPoint();

			if(Collider.Bottom > Entity.ParentScene.maxCoord.Y)
				Collided(Side.Bottom);
		}
	}

	/// <summary>Компонент, анимирующий объект поспрайтово.</summary>
	public class Animator : Component
	{
		SpriteRenderer spriteRenderer;

		public bool Cycled = true;
		/// <summary>Интервал между кадрами анимации в миллисекундах</summary>
		public int Interval { get; set; } = 1000;

		Dictionary<string, Animation> StateMap = new Dictionary<string, Animation>();
		string CurrentState = "";
		Animation CurrentAnimation;

		int frame = 0;
		bool animationRunning = false;
		Timer timer;

		public override void Init()
		{
			spriteRenderer = Entity.GetComponent<SpriteRenderer>();
			TimerCallback timerCallback = new TimerCallback(NextSprite);
		}

		public void CreateState(string stateName, Animation animation)
		{
			if(animation.SpriteSheet is null || animation.SpriteSheet.Length == 0)
				throw new Exception("Анимация, которую вы пытаетесь добавить, не содержит кадров.");
			if(StateMap.ContainsKey(stateName))
				throw new Exception($"Заданное состояние \"{stateName}\" уже имеется в аниматоре.");

			StateMap.Add(stateName, animation);
		}

		public void RemoveState(string stateName)
		{
			if(StateMap.ContainsKey(stateName))
			{
				if(CurrentState == stateName)
					CurrentState = "";
				StateMap.Remove(stateName);
			}
			else
				throw new Exception($"В аниматоре не содержится состояние " +
					$"\"{stateName}\", которое вы пытаетесь удалить.");
		}

		public void ChangeState(string stateName)
		{
			if(stateName != CurrentState)
			{
				if(StateMap.ContainsKey(stateName))
				{
					frame = 0;
					CurrentState = stateName;
					CurrentAnimation = StateMap[CurrentState];
				}
				else
					throw new Exception($"В аниматоре не содержится состояние " +
						$"\"{stateName}\", которое вы пытаетесь установить.");
			}
		}

		public void Start()
		{
			if(Entity.GetComponent<SpriteRenderer>() is null)
				throw new Exception("Аниматор не может функционировать без компонента SpriteRenderer.");
			if(StateMap.Count == 0)
				throw new Exception("Аниматор не содержит состояний.");
			if(CurrentState == "")
				throw new Exception("Не выбрано состояние аниматора.");


			timer = new Timer(NextSprite, null, 0, Interval);
			animationRunning = true;
		}

		public void Stop()
		{
			if(animationRunning)
				timer.Dispose();
			else
				throw new Exception("Попытка остановить аниматор, но он не был запущен.");
		}

		private void NextSprite(object obj)
		{
			spriteRenderer.Sprite = CurrentAnimation.SpriteSheet[++frame];

			if(frame == CurrentAnimation.SpriteSheet.Length - 1)
			{
				if(Cycled)
				{
					frame = -1;
				}
				else
				{
					timer.Dispose();
					animationRunning = false;
				}
			}
		}
	}
}
