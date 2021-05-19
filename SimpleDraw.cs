using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Animpaf
{
	class Graphics
	{
		static public void SimpleDraw(SpriteBatch batch, Texture2D sprite)
		{
			batch.Begin(samplerState: SamplerState.PointClamp);
			batch.Draw(sprite, Vector2.Zero, null, Color.White);
			batch.End();
		}
		static public void SimpleDraw(SpriteBatch batch, Texture2D sprite, float scale)
		{
			batch.Begin(samplerState: SamplerState.PointClamp);
			batch.Draw(sprite, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			batch.End();
		}
	}
}
