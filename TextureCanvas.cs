using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimpafGE.Graphics
{
	class TextureCanvas
	{
		public Texture2D Texture { get; set; }
		public UInt32[] Pixels { get; private set; }
		public UInt32[,] Pixels2D { get; set; }
		private int Width, Height, PixelSize;

		public TextureCanvas(GraphicsDevice graphicsDevice, int pixelSize, int width, int height, Color? color = null)
		{
			Width = width;
			Height = height;
			PixelSize = pixelSize;

			Texture = new Texture2D(graphicsDevice, width, height);
			Pixels = new uint[width * height];
			Pixels2D = new uint[width, height];

			if(color is null)
			{
				Random rnd = new();
				for(int i = 0; i < Pixels.Length; i++)
					Pixels[i] = new Color(150, 150, rnd.Next(230, 256)).PackedValue;
			}
			else
			{
				uint colorPackedValue = color.Value.PackedValue;
				for(int i = 0; i < Pixels.Length; i++)
					Pixels[i] = colorPackedValue;
			}

			PixelsToPixels2D();

			Texture.SetData<uint>(Pixels);
		}

		public uint[,] PixelsToPixels2D()
		{
			int i = 0;
			for(int y = 0; y < Height; y++)
				for(int x = 0; x < Width; x++)
				{
					Pixels2D[x, y] = Pixels[i++];
				}
			return Pixels2D;
		}

		/// <summary>
		/// Полностью перерисовывает текстуру. Рекомендуется при полном изменении или инициализации текстуры.
		/// </summary>
		/// <returns></returns>
		public uint[] Pixels2DToPixels()
		{
			int i = 0;
			for(int y = 0; y < Height; y++)
				for(int x = 0; x < Width; x++)
				{
					Pixels[i++] = Pixels2D[x, y];
				}

			Texture.SetData<uint>(Pixels);
			return Pixels;
		}

		public void DrawPixel(int x, int y, Color? color = null)
		{
			if(x > Width || x < 0 || y > Height || y < 0)
				throw new ArgumentOutOfRangeException("Drawn pixel was out of TextureCanvas");
			else
			{
				color ??= Color.Black;
				Texture.SetData(0, new Rectangle(x, y, 1, 1), new Color[] { (Color)color }, 0, 1);
			}
		}

		public void DrawLine(Vector2 start, Vector2 end, Color? color = null)
		{

		}

		public void DrawRectangle(Rectangle rectangle, Color? color = null)
		{
			try
			{
				color ??= Color.Black;
				var colorRect = new Color[rectangle.Width * rectangle.Height];
				for(int i = 0; i < colorRect.Length; i++)
					colorRect[i] = (Color)color;

				Texture.SetData(0, rectangle, colorRect, 0, rectangle.Width * rectangle.Height);
			}
			catch(Exception e)
			{
				Trace.WriteLine(e.Message);
				throw;
			}
		}
	}
}
