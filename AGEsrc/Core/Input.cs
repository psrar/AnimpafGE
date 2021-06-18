using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AnimpafGE.Input
{
	static public class InputProcessor
	{
		static List<Button> TrackedButtons = new List<Button>();

		public delegate void ButtonHandler(Keys key);
		static public event ButtonHandler ButtonClicked;
		static public event ButtonHandler ButtonHeld;

		static public void Process()
		{
			foreach(Button button in TrackedButtons)
				if(Keyboard.GetState().IsKeyDown(button.Key))
				{
					ButtonHeld(button.Key);
					if(!button.Processed)
					{
						button.Processed = true;
						ButtonClicked(button.Key);
					}
				}
				else button.Processed = false;
		}

		private class Button
		{
			public Keys Key;
			public bool IsDown = false;
			public bool Processed = false;

			public Button(Keys trackedKey) => Key = trackedKey;
		}

		static public void TrackButton(params Keys[] keys)
		{
			foreach(Keys key in keys)
			{
				bool flag = false;
				foreach(Button button in TrackedButtons)
					if(button.Key == key)
					{
						Trace.WriteLine($"Попытка отследить кнопку {key}, которая уже отслеживается.");
						flag = true;
						break;
					}
				if(!flag)
					TrackedButtons.Add(new Button(key));
			}
		}
	}
}
