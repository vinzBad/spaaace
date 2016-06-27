using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGame.Extended.Shapes;
using OpenTK.Graphics.ES11;

namespace Spaaace
{
	public class World:DrawableGameComponent
	{
		public Random Random = new Random (3);
		public SpriteFont DefaultFont;

		SpriteBatch batch;
		List<Entity> entities = new List<Entity> ();
		List<Planet> planets = new List<Planet> ();
		List<Player> players = new List<Player> ();
		List<Command> commands = new List<Command> ();

		List<Ship> ships = new List<Ship> ();
		List<Entity> entitiesToAdd = new List<Entity> ();

		List<Ship>[,] shipLookup;
		int lookupResolution = 64;

		public bool GameHasEnded = false;

		public World (Game game) : base (game)
		{
			game.Components.Add (this);
			game.IsMouseVisible = true;
		}

		protected override void LoadContent ()
		{
			
			batch = new SpriteBatch (this.GraphicsDevice);
			base.LoadContent ();
		}

		public override void Update (GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if(shipLookup != null)
				UpdateShipLookup ();
			
			// update all active entities
			entities.ForEach (
				e => {
					if (e.Active)
						e.Update (dt);
				});

			// remove "dead" entities
			entities.RemoveAll (e => !e.Alive);
			ships.RemoveAll (s => !s.Alive);
			commands.RemoveAll (c => !c.Alive);

			// add new entities
			entities.AddRange (entitiesToAdd);
			entitiesToAdd.Clear ();

			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			batch.Begin ();

			// draw all visible entities
			entities.ForEach (
				e => {
					if (e.Visible)
						e.Draw (batch);
				});

			if (false && shipLookup != null) {
				for (int x = 0; x < shipLookup.GetLength (0); x++) {
					
					batch.DrawLine (
						new Vector2 (x * lookupResolution, 0),
						new Vector2 (x * lookupResolution, GraphicsDevice.Viewport.Bounds.Height),
						Color.White,
						2
					);
				}

				for (int y = 0; y < shipLookup.GetLength (1); y++) {
					batch.DrawLine (
						new Vector2 (0, y * lookupResolution),
						new Vector2 (GraphicsDevice.Viewport.Bounds.Width, y * lookupResolution),
						Color.White,
						2
					);
				}
			}
			
			batch.End ();

			base.Draw (gameTime);
		}

		public void AddEntity (Entity entity)
		{
			if (entity.GetType () == typeof(Planet))
				planets.Add ((Planet)entity);
			else if (entity.GetType () == typeof(Player))
				players.Add ((Player) entity);
			else if (entity.GetType () == typeof(Ship))
				ships.Add ((Ship) entity);
			else if (entity.GetType () == typeof(Command))
				commands.Add ((Command) entity);
			
			entity.World = this;
			entitiesToAdd.Add (entity);
		}

		public void ClearEntities ()
		{
			ships.Clear ();
			players.Clear ();
			planets.Clear ();
			entities.Clear ();
			commands.Clear ();

			CreateShipLookup ();
		}

		void CreateShipLookup() {
			var bounds = this.GraphicsDevice.Viewport.Bounds;
			int width = bounds.Width / lookupResolution + 1;
			int height = bounds.Height / lookupResolution + 1;

			shipLookup = new List<Ship>[width,height];		

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					shipLookup [x,y] = new List<Ship> ();
				}
			}
		}

		void UpdateShipLookup() {
			for (int x = 0; x < shipLookup.GetLength (0); x++) {
				for (int y = 0; y < shipLookup.GetLength(1); y++) {
					shipLookup [x,y].Clear ();
				}
			}

			for (int i = 0; i < ships.Count; i++) {
				var ship = ships [i];
				int x = (int) ship.Position.X / lookupResolution;
				int y = (int) ship.Position.Y / lookupResolution;
				shipLookup [x,y].Add (ship);

			}
		}

		public Command FindCommand(Player player, Planet source)
		{
			return commands.Find (c => (c.Owner == player && c.Source == source));
		}

		public void GeneratePlanets ()
		{
			int maxIterations = 1000;
			int curIterations = 0;
			var bounds = this.GraphicsDevice.Viewport.Bounds;
			var center = new Vector2 (bounds.Width / 2, bounds.Height / 2);
			// create sun
			AddEntity (new Planet (){ Position = center, Size = 96, TintColor = Color.LightGoldenrodYellow });

			int planetCount = 70;
			//var planet = new Planet ();
			for (int i = 0; i < planetCount; i++) {
				int size = Random.Next (30, 55);

				var position = new Vector2 (
					               Random.Next ((int)(bounds.Left + size * 1.25f), (int)(bounds.Right - size * 1.25f)),
					               Random.Next ((int)(bounds.Top + size * 1.25f), (int)(bounds.Bottom - size * 1.25f))
				               );

				// as long as you find planets which are near the current position 
				// create new position
				curIterations = 0;
				while (planets.Find (p => 
					(p.Position - position).Length () < size + p.Size * 2) != null) {

					position = new Vector2 (
						Random.Next ((int)(bounds.Left + size * 1.25f), (int)(bounds.Right - size * 1.25f)),
						Random.Next ((int)(bounds.Top + size * 1.25f), (int)(bounds.Bottom - size * 1.25f))
					);

					if (curIterations > maxIterations)
						break;
					
					curIterations++;
				}

				AddEntity (new Planet () { Size = size, Position = position});
			}		
		}

		public Planet GetPlanetAtPosition(Vector2 position) {
			return planets.Find (
				p=> (p.Position - position).Length () <= p.Size
			);
		}

		public Planet GetRandomPlanet() {
			return planets [Random.Next (planets.Count)];
		}

		List<Ship> shipsInRange = new List<Ship>();

		public Ship[] GetShipsInRange(Vector2 position, float range) {
			shipsInRange.Clear ();
			// square the range, because LengthSquared is faster
			range = range * range;
			for (int i = 0; i < ships.Count; i++) {
				if ((ships [i].Position - position).LengthSquared () < range)
					shipsInRange.Add (ships [i]);
			}
			return shipsInRange.ToArray ();
		}

		public Planet GetNearestPlanet(Vector2 position, List<Planet> exclude) {
			Planet nearest = GetRandomPlanet ();
			if (exclude.Count == planets.Count)
				return null;
			
			while(exclude.Contains (nearest))
				nearest = GetRandomPlanet ();
			
			float dist = (position - nearest.Position).Length ();

			planets.ForEach (
				p => {
					if(!exclude.Contains (p)) {

					
					float curDist = (p.Position - position).Length () ;

					if(curDist < dist) {
						dist = curDist;
						nearest = p;
					} 
					}
				}
			);

			return nearest;
		}
			
		public List<Planet> GetPlanetsOwnedBy(Player player) {
			return planets.FindAll (
				p=> p.Owner == player
			);
		}


		public int NumberOfPlanets () {
			return planets.Count;
		}
	}

}
