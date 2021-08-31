using AGE.Graphics;
using AGE.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static AGE.Extensions;

namespace AGE.ECS.Components
{
	/// <summary>Компонент, задающий расположение и масштаб объекта на сцене.<br/>
	/// Принадлежит каждому объекту по умолчанию.</summary>
	public class Transform : Component
	{
		/// <summary>Расположение объекта на сцене</summary>
		public Vector2 Position = Vector2.Zero;
		/// <summary>Масштаб объекта</summary>
		public Vector2 Scaling { get; private set; } = Vector2.One;
		/// <summary>Вращение объекта</summary>
		public float Rotation = 0;

		/// <summary>Метод грубого (нефизичного) перемещения объекта</summary>
		/// <param name="translation">Вектор перемещения</param>
		public void Translate(Vector2 translation) => Position += translation;

		/// <summary>Вращает объект на angle градусов</summary>
		public void Rotate(float angle) => Rotation += MathHelper.ToRadians(angle);

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
		public Texture2D Sprite;
		private SpriteBatch Batch;
		public Color Color = Color.White;
		public SpriteEffects Mirroring = SpriteEffects.None;
		int Layer = 0;

		public Vector2 TopLeft;
		public Vector2 BottomRight;

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

	public class PolygonRenderer : Component
	{
		private SpriteBatch Batch;
		public Effect PolygonEffect;

		public List<Vertex2D> Vertices { get; private set; } = new List<Vertex2D>();
		public List<Vector2> Positions { get; private set; } = new List<Vector2>();
		public List<Edge2D> Edges { get; private set; } = new List<Edge2D>();
		public List<(Vector2 s, Vector2 e)> LineSegments { get; private set; } = new List<(Vector2 s, Vector2 e)>();
		public Vector2 TopLeft { get; private set; } = Vector2.Zero;

		public Texture2D Texture;
		public Color Color = Color.White;
		public SpriteEffects Mirroring = SpriteEffects.None;
		int Layer = 0;

		public override void Init()
		{
			Batch = ParentScene.spriteBatch;
			Texture = new Texture2D(ParentScene.ParentGame.GraphicsDevice, 1, 1);
			PolygonEffect = ParentScene.PolygonEffect;

			ParentScene.PolygonRenderers.Add(this);
		}

		public void CreateTexture(int width, int height)
		{
			Texture = new Texture2D(ParentScene.ParentGame.GraphicsDevice, width, height);
			Texture.SetData<uint>(new uint[width * height].Populate(0xFFFFFFFF));
			PolygonEffect.Parameters["dimensions"].SetValue(new Vector2(Texture.Width, Texture.Height));
		}

		public void AddVertices(params Vertex2D[] vertices)
		{
			Vertices.AddRange(vertices);
			UpdateEdges();
		}

		public void InsertVertex(int pos, Vertex2D vertex)
		{
			Vertices.Insert(pos, vertex);
			UpdateEdges();
		}

		public void RemoveVertex(Vertex2D vertex)
		{
			Vertices.Remove(vertex);
			UpdateEdges();
		}
		public void RemoveVertex(int pos)
		{
			Vertices.RemoveAt(pos);
			UpdateEdges();
		}
		public void RemoveVertex(int pos, int count)
		{
			for(int i = 0; i < count; i++)
				Vertices.RemoveAt(pos);
			UpdateEdges();
		}

		private void UpdateEdges()
		{
			Positions.Clear();
			Edges.Clear();
			for(int i = 0; i < Vertices.Count; i++)
				Positions.Add(Vertices[i].Position.ToVector2());

			for(int i = 0; i < Vertices.Count - 1;)
				Edges.Add(new Edge2D(Vertices[i], Vertices[++i]));

			Edges.Add(new Edge2D(Vertices.Last(), Vertices.First()));

			PolygonEffect.Parameters["vertices"].SetValue(Positions.ToArray());
			PolygonEffect.Parameters["verticesCount"].SetValue(Positions.Count);
		}

		public int Count() => Vertices.Count;

		public override void Process()
		{
			TopLeft = Entity.Transform.Position - Texture.Bounds.Size.ToVector2() / 2;

			for(int i = 0; i < Vertices.Count; i++)
			{
				Edges[i].StartPosition = Edges[i].Start.Position.ToVector2() + TopLeft;
				Edges[i].EndPosition = Edges[i].End.Position.ToVector2() + TopLeft;
			}

			LineSegments.Clear();
			foreach(Edge2D edge in Edges)
				LineSegments.Add((edge.StartPosition, edge.EndPosition));
		}

		public void RenderPolygon()
		{
			if(Vertices.Count > 1)
			{
				Batch.Draw(Texture,                          // Texture
					Entity.Transform.Position,              // Position
					null,                                   // Source rectangle
					Color,                                  // Color
					Entity.Transform.Rotation,              // Rotation
					Texture.Bounds.Size.ToVector2() / 2,     // Origin
					Entity.Transform.Scaling,               // Scale
					Mirroring,                              // Mirroring effect
					Layer);                                 // Depth
			}
		}
	}

	/// <summary>Компонент, отвечающий за перемещение объекта.</summary>
	public class RigidBody : Component
	{
		public RigidType RigidType = RigidType.Dynamic;
		public float Mass = 1;
		public bool UseGravity = true;
		Vector2 Gravity = new Vector2(0, 9800 / 2f);

		public Vector2 Velocity;
		public float VelocitySave = 0.98f;
		public Vector2 Overlap = Vector2.Zero;

		float deltaTime;

		public override void Process()
		{
			if(RigidType != RigidType.Static)
			{
				deltaTime = ParentScene.GameTime.ElapsedGameTime.Milliseconds / 1000f;
				if(UseGravity)
					Velocity += Gravity * deltaTime;

				Entity.Transform.Position += Velocity * deltaTime;
				Entity.GetComponent<BoxCollider>()?.Process();

				Velocity *= VelocitySave;
			}
		}

		public void OnCollision(Side side, BoxCollider collider, Vector2 overlap)
		{
			Overlap = overlap;

			switch(side)
			{
				case Side.Top:
					Entity.Transform.Position += Vector2.UnitY * Overlap.Y;
					collider.RigidBody.Velocity += Vector2.UnitY * Velocity.Y * Mass / collider.RigidBody.Mass / 50;
					Velocity *= Vector2.UnitX;
					break;
				case Side.Right:
					Entity.Transform.Position += Vector2.UnitX * Overlap.X;
					collider.RigidBody.Velocity += Vector2.UnitX * Velocity.X * Mass / collider.RigidBody.Mass / 50;
					Velocity *= Vector2.UnitY;
					break;
				case Side.Bottom:
					Entity.Transform.Position += Vector2.UnitY * Overlap.Y;
					if(collider != null)
						collider.RigidBody.Velocity += Vector2.UnitY * Velocity.Y * Mass / collider.RigidBody.Mass / 50;
					Velocity *= Vector2.UnitX;
					break;
				case Side.Left:
					Entity.Transform.Position += Vector2.UnitX * Overlap.X;
					collider.RigidBody.Velocity += Vector2.UnitX * Velocity.X * Mass / collider.RigidBody.Mass / 50;
					Velocity *= Vector2.UnitY;
					break;
			}
		}
	}

	/// <summary>Компонент, задающий границы коллайдера для объекта.<br/>
	/// Если у объекта должен быть спрайт, сначала добавьте его.</summary>
	public class BoxCollider : Component
	{
		public Rectangle Collider = new Rectangle();

		public RigidBody RigidBody;
		public Transform Transform;

		public delegate void CollisionHandler(Side side, BoxCollider collider, Vector2 overlap);
		public event CollisionHandler Collided;

		public override void Init()
		{
			if(Entity.GetComponent<RigidBody>() is null)
				throw new Exception("Компонент BoxCollider требует наличия у объекта компонента RigidBody");

			RigidBody = Entity.GetComponent<RigidBody>();
			Transform = Entity.GetComponent<Transform>();
			Collided += RigidBody.OnCollision;

			Transform.ScaleChanged += OnTransformScaled;

			if(Entity.GetComponent<SpriteRenderer>()?.Sprite != null)
				SetSizeBasedOnSprite();
		}

		public BoxCollider SetSizeBasedOnSprite()
		{
			SpriteRenderer spriteRenderer = Entity.GetComponent<SpriteRenderer>();
			if(spriteRenderer != null)
				if(spriteRenderer.Sprite != null)
					SetSize(spriteRenderer.Sprite.Width * Transform.Scaling.X,
						spriteRenderer.Sprite.Height * Transform.Scaling.Y);
				else throw new Exception("У данного объекта отсутствует спрайт.");
			else throw new Exception("У данного объекта отсутствует компонент SpriteRenderer.");
			return this;
		}

		public BoxCollider SetSize(int size)
		{
			Collider = new Rectangle(IncrementVector(Transform.Position, -size / 2).ToPoint(),
				new Point(size));
			return this;
		}
		public BoxCollider SetSize(float width, float height)
		{
			Vector2 size = new Vector2(width, height) * Transform.Scaling;
			Collider = new Rectangle((Transform.Position - size / 2).ToPoint(),
				new Point((int)MathF.Round(width), (int)MathF.Round(height)));
			return this;
		}

		private void OnTransformScaled()
		{
			int newSizeX = Collider.Width * (int)Transform.Scaling.X;
			int newSizeY = Collider.Height * (int)Transform.Scaling.Y;
			Collider =
				new Rectangle((Transform.Position - new Vector2(newSizeX, newSizeY) / 2).ToPoint(),
				new Point(newSizeX, newSizeY));
		}

		public override void Process()
		{
			if(RigidBody.RigidType != RigidType.Static)
			{
				Collider.Location = (Vector2.Round(Transform.Position) - Collider.Size.ToVector2() / 2).ToPoint();

				if(Collider.Bottom > Entity.ParentScene.maxCoord.Y)
					Collided(Side.Bottom, null,
						Vector2.UnitY * (Entity.ParentScene.maxCoord.Y - Collider.Bottom));

				foreach(BoxCollider coll in ParentScene.Colliders)
				{
					if(coll != this)
					{
						Rectangle c = coll.Collider;

						if(Collider.Intersects(c))
						{
							Vector2 overlap = Collider.GetIntersectionDepth(c);
							if(Math.Abs(overlap.X) < Math.Abs(overlap.Y))
								if(overlap.X < 0)
									Collided(Side.Right, coll, overlap);
								else
									Collided(Side.Left, coll, overlap);
							else
								if(overlap.Y < 0)
								Collided(Side.Top, coll, overlap);
							else
								Collided(Side.Bottom, coll, overlap);
						}
					}
				}
			}
		}
	}

	public class TriggerArea
	{
		public Rectangle Area = new Rectangle();
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
