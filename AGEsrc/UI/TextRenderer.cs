using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AGE.ECS;

namespace AGE.UI
{
	public class TextRenderer : Component
	{
		public string Text = "Hello World!";
		public Color Color = Color.Black;
		public SpriteFont SpriteFont;

		private SpriteBatch Batch;

		public override void Init()
		{
			Batch = ParentScene.spriteBatch;
		}

		public override void Process()
		{
			if(SpriteFont is null)
				throw new Exception("SpriteFont должен быть назначен внутри вашего проекта.");

			Batch.DrawString(SpriteFont, Text, Entity.Transform.Position, Color);
		}
	}
}
