using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AnimpafGE.ECS;

namespace AnimpafGE
{
	class Graphics
	{
		static public void SimpleDraw(SpriteBatch batch, Vector2 position, Texture2D sprite)
		{
			batch.Begin(samplerState: SamplerState.PointClamp);
			batch.Draw(sprite, position, null, Color.White);
			batch.End();
		}
		static public void SimpleDraw(SpriteBatch batch, Vector2 position, Texture2D sprite, float scale)
		{
			batch.Begin(samplerState: SamplerState.PointClamp);
			batch.Draw(sprite, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			batch.End();
		}
	}
}
