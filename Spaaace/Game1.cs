using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;


using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework.Media;


namespace Spaaace
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		World world;

		KeyboardState prevKs = Keyboard.GetState ();
		MouseState prevMs = Mouse.GetState ();

		HumanPlayer player;
		Planet selectedPlanet = null;
		Planet planetAtMouse = null;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			world = new World (this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
            
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			world.DefaultFont = Content.Load<SpriteFont> ("inconsolata16");
			//TODO: use this.Content to load your game content here 
		}

		void SetupGame() {
			world.ClearEntities ();
			world.GeneratePlanets ();
			player = new HumanPlayer () { TintColor = Color.ForestGreen };
			world.GetRandomPlanet ().Owner = player;
			world.AddEntity (player);

			AddAi (Color.Fuchsia);
			AddAi (Color.Blue);
			AddAi (Color.Red);
			AddAi (Color.Peru);
			AddAi (Color.Yellow);
			AddAi (Color.Brown);
			//AddAi (Color.Green);
			AddAi (Color.Orange);
			AddAi (Color.AliceBlue);
			AddAi (Color.Aquamarine);
			AddAi (Color.Beige);
			AddAi (Color.BlanchedAlmond);
			AddAi (Color.BurlyWood);
			AddAi (Color.CornflowerBlue);
			AddAi (Color.DarkBlue);


		}

		void AddAi(Color color) {
			RandomPlayer ai = new RandomPlayer ();
			ai.TintColor = color;

			Planet aiPlanet = world.GetRandomPlanet ();
			while (aiPlanet.Owner != null)
				aiPlanet = world.GetRandomPlanet ();
			aiPlanet.Owner = ai;
			world.AddEntity (ai);
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif

			var ms = Mouse.GetState ();
			var ks = Keyboard.GetState ();
            
			// TODO: Add your update logic here
            
			if (prevKs.IsKeyDown (Keys.R) && ks.IsKeyUp (Keys.R)) {
				SetupGame ();
			}

			if (world.GameHasEnded) {
				SetupGame ();
				world.GameHasEnded = false;
			}
			prevKs = ks;
			prevMs = ms;

			base.Update (gameTime);
		}



		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.Black);
            
			//TODO: Add your drawing code here
			spriteBatch.Begin ();

			spriteBatch.End ();
            
			base.Draw (gameTime);
		}
	}
}

