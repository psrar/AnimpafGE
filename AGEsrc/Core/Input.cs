using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace AGE.Input
{
	static public class InputProcessor
	{
		static List<Keys> TrackedButtons = new List<Keys>();
		static public List<Keys> PushedKeys = new List<Keys>();

		public delegate void ButtonHandler(Keys key);
		public delegate void TouchHandler(Vector2 touchPosition);

		static public event ButtonHandler ButtonClicked = delegate { };
		static public event ButtonHandler ButtonReleased = delegate { };
		static public event ButtonHandler ButtonHeld = delegate { };
		static public event TouchHandler TouchHeld = delegate { };

		static public void Process()
		{
			TouchCollection touches = TouchPanel.GetState();
			if(touches.Count > 0)
				TouchHeld(touches[0].Position);

			foreach(Keys key in TrackedButtons)
				if(Keyboard.GetState().IsKeyDown(key))
				{
					if(PushedKeys.Contains(key))
					{
						ButtonHeld(key);
					}
					else
					{
						PushedKeys.Add(key);
						ButtonClicked(key);
					}
				}
				else
				{
					if(PushedKeys.Contains(key))
					{
						PushedKeys.Remove(key);
						ButtonReleased(key);
					}
				}
		}

		static public void TrackButton(params Keys[] keys)
		{
			foreach(Keys key in keys)
				if(TrackedButtons.Contains(key))
					throw new Exception($"Попытка отследить кнопку {key}, которая уже отслеживается.");

			TrackedButtons.AddRange(keys);
		}
	}
}
