﻿using System;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FirstApp.Model;
using FirstApp.View;
using Microsoft.Xna.Framework.Input.Touch;


namespace FirstApp.Controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		private Player player;



		// Keyboard states used to determine key presses
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;

		// Gamepad states used to determine button presses
		GamePadState currentGamePadState;
		GamePadState previousGamePadState;

		// A movement speed for the player
		float playerMoveSpeed;

		// Image used to display the static background
		Texture2D mainBackground;

		// Parallaxing Layers
		ParallaxingBackground bgLayer1;
		ParallaxingBackground bgLayer2;

		// Enemies
		Texture2D enemyTexture;
		List<Enemy> enemies;

		// Power-Ups
		Texture2D powerupTexture;
		List<Powerup> powerups;

		// The rate at which the enemies appear
		TimeSpan enemySpawnTime;
		TimeSpan previousSpawnTime;

		// The rate at which the powerups appear
		TimeSpan powerupSpawnTime;
		TimeSpan powPreviousSpawnTime;

		// A random number generator
		Random random;

		Texture2D projectileTexture;
		List<Projectile> projectiles;

		// The rate of fire of the player laser
		TimeSpan fireTime;
		TimeSpan previousFireTime;

		Texture2D explosionTexture;
		List<Animation> explosions;
		private String type;

		// The sound that is played when a laser is fired
		//SoundEffect laserSound;

		// The sound used when the player or an enemy dies
		//SoundEffect explosionSound;

		// The music played during gameplay
		//Song gameplayMusic;


		//Number that holds the player score
		int score;
		// The font used to display UI elements
		SpriteFont font;



		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			player = new Player(this);

			// Set a constant player move speed
			playerMoveSpeed = 8.0f;

			//Enable the FreeDrag gesture.
			TouchPanel.EnabledGestures = GestureType.FreeDrag;

			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();

			// Initialize the enemies list
			enemies = new List<Enemy>();

			powerups = new List<Powerup>();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Set the time keepers to zero
			powPreviousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast Powerup respawns
			powerupSpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator
			random = new Random();

			projectiles = new List<Projectile>();

			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(.15f);

			explosions = new List<Animation>();
			type = "single";


			//Set player's score to zero
			score = 0;

			base.Initialize();


		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load the player resources
			//Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X , GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture , Vector2.Zero , 115 , 69 , 8 , 30 , Color.White , 1f , true);

			Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X , GraphicsDevice.Viewport.TitleSafeArea.Y
			+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation , playerPosition);

			// Load the parallaxing background
			bgLayer1.Initialize(Content , "Texture/bgLayer1" , GraphicsDevice.Viewport.Width , -1);
			bgLayer2.Initialize(Content , "Texture/bgLayer2" , GraphicsDevice.Viewport.Width , -2);

			enemyTexture = Content.Load<Texture2D>("Animation/mineAnimation");

			projectileTexture = Content.Load<Texture2D>("Texture/rocket-1");
			powerupTexture = Content.Load<Texture2D>("Texture/power-up-1");

			explosionTexture = Content.Load<Texture2D>("Animation/explosion");

			// Load the music
			//gameplayMusic = Content.Load<Song>("sound/gameMusic");

			// Load the laser and explosion sound effect
			//laserSound = Content.Load<SoundEffect>("sound/laserFire");
			//explosionSound = Content.Load<SoundEffect>("sound/explosion");

			// Start the music right away
			//PlayMusic(gameplayMusic);

			// Load the score font
			font = Content.Load<SpriteFont>("Font/gameFont");

			mainBackground = Content.Load<Texture2D>("Texture/mainbackground");

			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);


			//Update the player
			UpdatePlayer(gameTime);

			// Update the parallaxing background
			bgLayer1.Update();
			bgLayer2.Update();
			// Update the enemies
			UpdateEnemies(gameTime);

			//Update Powerups
			UpdatePowerups(gameTime);

			// Update the collision
			UpdateCollision();

			// Update the projectiles
			UpdateProjectiles();

			// Update the explosions
			UpdateExplosions(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			// Start drawing
			spriteBatch.Begin();

			spriteBatch.Draw(mainBackground , Vector2.Zero , Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);

			// Draw the Enemies
			for(int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch);
			}

			// Draw the Powerups
			for(int i = 0; i<powerups.Count; i++)
			{
				powerups[i].Draw(spriteBatch);
			}

			// Draw the Projectiles
			for(int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Draw(spriteBatch);
			}

			// Draw the explosions
			for(int i = 0; i < explosions.Count; i++)
			{
				explosions[i].Draw(spriteBatch);
			}

			// Draw the score
			spriteBatch.DrawString(font , "score: " + score , new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X , GraphicsDevice.Viewport.TitleSafeArea.Y) , Color.White);
			// Draw the player health
			spriteBatch.DrawString(font , "health: " + player.Health , new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X , GraphicsDevice.Viewport.TitleSafeArea.Y + 30) , Color.White);

			// Draw the Player
			player.Draw(spriteBatch);

			// Stop drawing
			spriteBatch.End();



			base.Draw(gameTime);
		}

		private void UpdatePlayer(GameTime gameTime)
		{

			player.Update(gameTime);

			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

			// Use the Keyboard / Dpad
			if(currentKeyboardState.IsKeyDown(Keys.Left) ||
			currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if(currentKeyboardState.IsKeyDown(Keys.Right) ||
			currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if(currentKeyboardState.IsKeyDown(Keys.Up) ||
			currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if(currentKeyboardState.IsKeyDown(Keys.Down) ||
			currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}
			if(currentKeyboardState.IsKeyDown(Keys.A))
			{
				type = "OP";
			}

			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X , 0 , GraphicsDevice.Viewport.Width - player.Width);
			player.Position.Y = MathHelper.Clamp(player.Position.Y , 0 , GraphicsDevice.Viewport.Height - player.Height);

			// Fire only every interval we set as the fireTime
			if(gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;


				player.fire(type);


				// AddProjectile(player.Position + new Vector2(player.Width / 2 , -15));
				// AddProjectile(player.Position + new Vector2(player.Width / 2 , 30));
				// AddProjectile(player.Position + new Vector2(player.Width / 2 , -30));
				// AddProjectile(player.Position + new Vector2(player.Width / 2 , 45));
				// AddProjectile(player.Position + new Vector2(player.Width / 2 , -45));


				// Play the laser sound
				//laserSound.Play();
			}

			// reset score if player health goes to zero
			if(player.Health <= 0)
			{
				player.Health = 100;
				score = 0;
			}
		}

		private void AddEnemy()
		{
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture , Vector2.Zero , 47 , 61 , 8 , 30 , Color.White , 1f , true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2 , random.Next(100 , GraphicsDevice.Viewport.Height - 100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation , position);

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if(gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
			{
				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddEnemy();
			}

			// Update the Enemies
			for(int i = enemies.Count - 1; i >= 0; i--)
			{
				enemies[i].Update(gameTime);

				if(enemies[i].Active == false)
				{
					// If not active and health <= 0
					if(enemies[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(enemies[i].Position);
						// Play the explosion sound
						//explosionSound.Play();

						//Add to the player's score
						score += enemies[i].Value;
					}
					enemies.RemoveAt(i);
				}
			}
		}

		private void UpdatePowerups(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if(gameTime.TotalGameTime - powPreviousSpawnTime > powerupSpawnTime)
			{
				powPreviousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddPowerup();
			}

			// Update the Enemies
			for(int i = powerups.Count - 1; i >= 0; i--)
			{
				powerups[i].Update(gameTime);

				if(powerups[i].Active == false)
				{
					
					powerups.RemoveAt(i);
				}
			}
		}

		private void AddPowerup()
		{
			// Create the animation object
			Powerup powerup = new Powerup();

			// Initialize the animation with the correct animation information
			//enemyAnimation.Initialize(enemyTexture , Vector2.Zero , 47 , 61 , 8 , 30 , Color.White , 1f , true);


			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2 , random.Next(100 , GraphicsDevice.Viewport.Height - 100));

			// Create an enemy


			// Initialize the enemy
			powerup.Initialize(powerupTexture , position);

			// Add the enemy to the active enemies list
			powerups.Add(powerup);
		}

		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;
			Rectangle rectangle3;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X ,
			(int)player.Position.Y ,
			player.Width ,
			player.Height);

			// Do the collision between the player and the enemies
			for(int i = 0; i < enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X ,
				(int)enemies[i].Position.Y ,
				enemies[i].Width ,
				enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if(player.Health <= 0)
						player.Active = false;
				}


			}

			// Do the collision between the player and the Powerups
			for(int i = 0; i<powerups.Count; i++)
			{
				rectangle3 = new Rectangle((int)powerups[i].Position.X ,
				(int)powerups[i].Position.Y ,
				powerups[i].Width ,
				powerups[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle3))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= powerups[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					powerups[i].Health = 0;
					type = powerups[i].Type;

					// If the player health is less than zero we died
					if(player.Health <= 0)
						player.Active = false;
				}


			}

			// Projectile vs Enemy Collision
			for(int i = 0; i < projectiles.Count; i++)
			{
				for(int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X -
					projectiles[i].Width / 2 , (int)projectiles[i].Position.Y -
					projectiles[i].Height / 2 , projectiles[i].Width , projectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2 ,
					(int)enemies[j].Position.Y - enemies[j].Height / 2 ,
					enemies[j].Width , enemies[j].Height);

					// Determine if the two objects collided with each other
					if(rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}
		}

		public void AddProjectile(float x , float y , float speed , int gx , int gy, int angle)
		{
			Projectile projectile = new Projectile(x , y , speed , gx , gy, angle);
			Vector2 position = new Vector2(x , y);
			projectile.Initialize(GraphicsDevice.Viewport , projectileTexture , position);
			projectiles.Add(projectile);
		}

		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for(int i = projectiles.Count - 1; i >= 0; i--)
			{


				projectiles[i].Update();

				//if(projectiles.Count < 1000)
				//{
				// AddProjectile(projectiles[i].Position - new Vector2(0 , 20));
				//AddProjectile(projectiles[i].Position + new Vector2(0 , 20));
				//}

				if(projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				}


			}
		}

		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture , position , 134 , 134 , 12 , 45 , Color.White , 1f , false);
			explosions.Add(explosion);
		}

		private void UpdateExplosions(GameTime gameTime)
		{
			for(int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if(explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}

		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { }
		}




	}



}
