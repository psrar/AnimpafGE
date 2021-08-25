using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AGE.ECS;
using AGE.ECS.Components;
using AGE.Input;

namespace AGE.Arcade
{
	public class FighterLayout
	{
		public Keys Up;
		public Keys Right;
		public Keys Down;
		public Keys Left;
		public Keys LightAttack;
		public Keys HeavyAttack;
		public Keys Block;

		public FighterLayout(Keys up, Keys right, Keys down, Keys left, Keys lightAttack, Keys heavyAttack, Keys block)
		{
			Up = up;
			Right = right;
			Down = down;
			Left = left;
			LightAttack = lightAttack;
			HeavyAttack = heavyAttack;
			Block = block;
		}
	}

	public abstract class Fighter : Entity
	{
		protected InputProcessor Input;
		protected RigidBody Body;
		protected SpriteRenderer Renderer;
		protected BoxCollider Collider;

		FighterLayout Layout;

		public Fighter(Scene scene, Vector2 position, float scale, Texture2D sprite, FighterLayout layout = null) : base(scene, position)
		{
			Body = AddComponent<RigidBody>();
			Renderer = AddComponent<SpriteRenderer>();
			Renderer.Sprite = sprite;
			Collider = AddComponent<BoxCollider>();
			Transform.SetScaling(scale);

			if(layout != null)
			{
				Layout = layout;

				Input = new InputProcessor(ParentScene);

				Input.TrackButton(layout.Up);
				Input.TrackButton(layout.Right);
				Input.TrackButton(layout.Down);
				Input.TrackButton(layout.Left);
				Input.TrackButton(layout.LightAttack);
				Input.TrackButton(layout.HeavyAttack);
				Input.TrackButton(layout.Block);

				Input.AlignDirectionButton(Direction.Up, layout.Up);
				Input.AlignDirectionButton(Direction.Right, layout.Right);
				Input.AlignDirectionButton(Direction.Down, layout.Down);
				Input.AlignDirectionButton(Direction.Left, layout.Left);

				Input.ButtonClicked += OnButtonClicked;
			}
		}

		public void OnButtonClicked(Keys pressedKey)
		{
			if(pressedKey == Layout.Up)
				Up();
			else if(pressedKey == Layout.Right)
				Right();
			else if(pressedKey == Layout.Down)
				Down();
			else if(pressedKey == Layout.Left)
				Left();
			else if(pressedKey == Layout.LightAttack)
				LightAttack();
			else if(pressedKey == Layout.HeavyAttack)
				HeavyAttack();
			else if(pressedKey == Layout.Block)
				Block();
		}

		public abstract void Up();
		public abstract void Right();
		public abstract void Down();
		public abstract void Left();
		public abstract void LightAttack();
		public abstract void HeavyAttack();
		public abstract void Block();
	}
}
