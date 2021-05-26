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
		/// <summary>
		/// Расположение объекта на сцене
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// Масштаб объекта
		/// </summary>
		public Vector2 Scaling { get; set; } = Vector2.One;

		/// <summary>
		/// Вращение объекта
		/// </summary>
		public float Rotation { get; set; } = 0;

		public Transform(Vector2? position = null) => Position = position is null ? Vector2.Zero : (Vector2)position;

		/// <summary>
		/// Метод грубого (нефизичного) перемещения объекта
		/// </summary>
		/// <param name="translation">Вектор перемещения</param>
		public void Translate(Vector2 translation) => Position += translation;
		/// <summary>
		/// Изменение размеров объекта
		/// </summary>
		/// <param name="scaling">Вектор увеличения</param>
		public void Scale(Vector2 scaling) => Scaling += scaling;
	}

	/// <summary>
	/// Компонент, отвечающий за отрисовку спрайта на сцене.
	/// </summary>
	class SpriteRenderer : Component
	{
		public Texture2D Sprite { get; set; }
		SpriteBatch Batch { get; }
		Color Color { get; set; } = Color.White;
		int Layer { get; set; } = 0;

		public SpriteRenderer() => Batch = new SpriteBatch(Core.graphicsDeviceManager.GraphicsDevice);

		public override void Process()
		{
			if(!(Sprite is null))
			{
				Batch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: ((Game1)Entity.Game).cam.GetMatrix(Core.graphicsDeviceManager.GraphicsDevice));
				Batch.Draw(Sprite,							// Texture
					Entity.Transform.Position,				// Position
					null,									// Source rectangle
					Color,							// Color
					0,                                      // Rotation
					Sprite.Bounds.Size.ToVector2() / 2,		// Origin
					Entity.Transform.Scaling,				// Scale
					SpriteEffects.None,						// Mirroring effect
					Layer);									// Depth
				Batch.End();
			}
			else
			{
				throw new System.Exception("Попытка вызвать обработку компонента, которого не " +
					"существует для данного объекта.");
			}
		}
	}

	/// <summary>
	/// Компнонент, отвечающий за применение к объекту физической силы
	/// </summary>
	class RigidBody
	{

	}
}
