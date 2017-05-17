using System;


using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FirstApp.Model;
using FirstApp.View;
using Microsoft.Xna.Framework.Input.Touch;
using FirstApp.Controller;

namespace FirstApp.Model
{
	class Player
	{
		// Animation representing the player

		private Game1 game;
		private Animation playerAnimation;

		public Animation PlayerAnimation
		{
			get
			{
				return playerAnimation;
			}
			set
			{
				playerAnimation = value;
			}
		}

		// Position of the Player relative to the upper left side of the screen



		public Vector2 Position;



		// State of the player

		private bool active;

		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active = value;
			}
		}

		// Amount of hit points that player has

		private int health;

		public int Health
		{
			get
			{
				return health;
			}
			set
			{
				health = value;
			}
		}

		// Get the width of the player ship
		public int Width
		{
			get { return PlayerAnimation.FrameWidth; }
		}

		// Get the height of the player ship
		public int Height
		{
			get { return PlayerAnimation.FrameHeight; }
		}

		// Initialize the player
		public void Initialize(Animation animation , Vector2 position)
		{
			PlayerAnimation = animation;

			// Set the starting position of the player around the middle of the screen and to the back
			Position = position;

			// Set the player to be active
			Active = true;

			// Set the player health
			Health = 100;
		}

		// Update the player animation
		public void Update(GameTime gameTime)
		{
			PlayerAnimation.Position = Position;
			PlayerAnimation.Update(gameTime);
		}

		// Draw the player
		public void Draw(SpriteBatch spriteBatch)
		{
			PlayerAnimation.Draw(spriteBatch);
		}

		public void fire(String bType)
		{
			if(bType.Equals("tri"))
			{
				game.AddProjectile(this.Position.X  , this.Position.Y  , 2 , 0 , 0 , 0);
				game.AddProjectile(this.Position.X  , this.Position.Y  , 2 , 0 , 2 , 0);
				game.AddProjectile(this.Position.X  , this.Position.Y  , 2 , 0 , -2 , 0);

			}
			if(bType.Equals("single"))
			{
				game.AddProjectile(this.Position.X  , this.Position.Y  , 2 , 0 , 0 , 0);
			}
			   
		}

		public Player(Game1 gameController)
		{
			this.game = gameController;
		}
	}

}

