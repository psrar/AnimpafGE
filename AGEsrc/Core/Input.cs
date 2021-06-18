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

		static public void Process()
		{
			foreach(Button button in TrackedButtons)
				if(Keyboard.GetState().IsKeyDown(button.Key))
				{
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

		static public bool TrackButton(Keys key)
		{
			foreach(Button button in TrackedButtons)
				if(button.Key == key)
				{
					Trace.WriteLine("Попытка отследить кнопку, которая уже отслеживается. Метод TrackButton прерван.");
					return false;
				}

			TrackedButtons.Add(new Button(key));
			return true;
		}
	}
}
