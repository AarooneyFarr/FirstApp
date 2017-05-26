using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FirstApp.View;

namespace FirstApp.Model
{
	public class Powerup
	{


		public Texture2D powerUpTexture;


		public Vector2 Position;


		public bool Active;


		public int Health;


		public int Damage;

		// The amount of score the enemy will give to the player
		public int Value;

		private String type;


		public String Type
		{
			get
			{
				return type;
			}
			set
			{
				value = type;
			}
		}

		public int Width
		{
			get { return powerUpTexture.Width; }
		}


		public int Height
		{
			get { return powerUpTexture.Height; }
		}


		float powerupMoveSpeed;

		public void Initialize(Texture2D texture, Vector2 position)
		{
			// Load the enemy ship texture
			powerUpTexture = texture;

			// Set the position of the enemy
			Position = position;

			// We initialize the enemy to be active so it will be update in the game
			Active = true;


			// Set the health of the enemy
			Health = 10000;

			// Set the amount of damage the enemy can do
			Damage = 0;

			// Set how fast the enemy moves
			powerupMoveSpeed = 6f;


			// Set the score value of the enemy
			Value = 0;

			// Set the type to random
			//type = randomType();

		}

		public void Update(GameTime gameTime)
		{
			// The enemy always moves to the left so decrement it's xposition
			Position.X -= powerupMoveSpeed;

			type = randomType();



			// If the enemy is past the screen or its health reaches 0 then deactivateit
			if(Position.X < -Width || Health <= 0)
			{
				// By setting the Active flag to false, the game will remove this objet fromthe
				// active game list
				Active = false;
			}
		}


		public void Draw(SpriteBatch spriteBatch)
		{
			// Draw the animation
			spriteBatch.Draw(powerUpTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//TODO add power up stuff
		}

		private String randomType()
		{
			String ranType = "";

			if(this.Position.Y % 10 < 5)
			{
				ranType = "crazy";
			}
			else
			{
				ranType = "tri";
			}

			return ranType;

		}

		public Powerup()
		{
			//type = randomType();

		}
	}
}
