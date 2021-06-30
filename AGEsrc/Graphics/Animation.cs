using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace AGE.Graphics
{
	public class Animation
	{
		public Texture2D[] SpriteSheet;

		public Animation LoadSheet(params string[] spriteNames)
		{
			SpriteSheet = new Texture2D[spriteNames.Length];

			for(int i = 0; i < spriteNames.Length; i++)
				if((SpriteSheet[i] = Core.Content.Load<Texture2D>(spriteNames[i])) is null)
					throw new Exception("Content Manager не содержит заданного файла спрайта: " + spriteNames[i] + '\n' +
						"Добавьте его через MGCB Editor.");

			return this;
		}
	}
}
