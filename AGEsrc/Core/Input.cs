using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
		static public Keys[] DirectionKeys { get; private set; } = new Keys[4];

		public delegate void ButtonHandler(Keys key);
		public delegate void TouchHandler(Vector2 touchPosition);

		static public event ButtonHandler ButtonClicked = delegate { };
		static public event ButtonHandler ButtonReleased = delegate { };
		static public event ButtonHandler ButtonHeld = delegate { };
		static public event TouchHandler TouchHeld = delegate { };

		public enum Direction { Up, Right, Down, Left }
		private static Vector2 axis;
		public static Vector2 Axis
		{
			get => axis;
			set => axis = value;
		}

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

			if(PushedKeys.Contains(DirectionKeys[0]))
				axis.Set('y', -1);
			else if(PushedKeys.Contains(DirectionKeys[2]))
				axis.Set('y', 1);
			else
				Axis *= Vector2.UnitX;

			if(PushedKeys.Contains(DirectionKeys[1]))
				axis.Set('x', 1);
			else if(PushedKeys.Contains(DirectionKeys[3]))
				axis.Set('x', -1);
			else
				Axis *= Vector2.UnitY;
		}

		static public void TrackButton(params Keys[] keys)
		{
			foreach(Keys key in keys)
				if(TrackedButtons.Contains(key))
					throw new Exception($"Попытка отследить кнопку {key}, которая уже отслеживается.");

			TrackedButtons.AddRange(keys);
		}
		static public void AlignDirectionButton(Direction direction, Keys key)
		{
			switch(direction)
			{
				case Direction.Up:
					DirectionKeys[0] = key;
					break;
				case Direction.Right:
					DirectionKeys[1] = key;
					break;
				case Direction.Down:
					DirectionKeys[2] = key;
					break;
				case Direction.Left:
					DirectionKeys[3] = key;
					break;
			}
		}
	}
}
