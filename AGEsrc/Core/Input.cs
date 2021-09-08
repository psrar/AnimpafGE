using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using AGE.ECS;
using System.Diagnostics;

namespace AGE.Input
{
	public enum Directions { Up, Right, Down, Left }
	public enum MouseButtons { Left, Middle, Right }


	public class InputProcessor
	{
		public MouseState MouseState { get; private set; }
		private MouseState oldMouseState = Mouse.GetState();

		public TouchCollection Touches { get; private set; }

		List<Keys> TrackedButtons = new List<Keys>();
		public List<Keys> PushedKeys = new List<Keys>();
		public Keys[] DirectionKeys { get; private set; } = new Keys[4];

		public delegate void ButtonHandler(Keys key);
		public delegate void MouseHandler(MouseState mouseState, MouseButtons button);
		public delegate void TouchHandler(TouchCollection touches);

		public event ButtonHandler ButtonClicked = delegate { };
		public event ButtonHandler ButtonReleased = delegate { };
		public event ButtonHandler ButtonHeld = delegate { };
		public event MouseHandler MouseButtonClicked = delegate { };
		public event TouchHandler Touching = delegate { };

		private static Vector2 axis;
		public static Vector2 Axis
		{
			get => axis;
			set => axis = value;
		}

		public InputProcessor(Scene scene) => scene.TrackInputProcessor(this);

		public void Process()
		{
			Touches = TouchPanel.GetState();
			MouseState = Mouse.GetState();

			if(MouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
				MouseButtonClicked(MouseState, MouseButtons.Left);
			if(MouseState.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton == ButtonState.Released)
				MouseButtonClicked(MouseState, MouseButtons.Middle);
			if(MouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released)
				MouseButtonClicked(MouseState, MouseButtons.Right);

			if(Touches.Count > 0)
				Touching(Touches);

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
				axis.SetAxis('y', -1);
			else if(PushedKeys.Contains(DirectionKeys[2]))
				axis.SetAxis('y', 1);
			else
				Axis *= Vector2.UnitX;

			if(PushedKeys.Contains(DirectionKeys[1]))
				axis.SetAxis('x', 1);
			else if(PushedKeys.Contains(DirectionKeys[3]))
				axis.SetAxis('x', -1);
			else
				Axis *= Vector2.UnitY;

			oldMouseState = MouseState;
		}

		public void TrackButton(params Keys[] keys)
		{
			foreach(Keys key in keys)
				if(TrackedButtons.Contains(key))
					throw new Exception($"Попытка отследить кнопку {key}, которая уже отслеживается.");

			TrackedButtons.AddRange(keys);
		}
		public void AlignDirectionButton(Directions direction, Keys key)
		{
			switch(direction)
			{
				case Directions.Up:
					DirectionKeys[0] = key;
					break;
				case Directions.Right:
					DirectionKeys[1] = key;
					break;
				case Directions.Down:
					DirectionKeys[2] = key;
					break;
				case Directions.Left:
					DirectionKeys[3] = key;
					break;
			}
		}
	}
}
