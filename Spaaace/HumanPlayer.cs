using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;


namespace Spaaace
{
	public	class Command :Entity
	{
		public Player Owner;
		public Planet Source, Target;
		public float MinShipsOnSource = 10;

		public Command(Player owner, Planet source, Planet target) {
			this.Owner = owner;
			this.Target = target;
			this.Source = source;
		}

		public override void Update (float dt)
		{
			if (Source.Owner == this.Owner) {
				if (Source.ShipCount > MinShipsOnSource) {
					Source.LaunchShips (Target, (int)( Source.ShipCount - MinShipsOnSource));
				}
			} else {
				this.Alive = false;
			}
		}

		public override void Draw (Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			batch.DrawLine (Source.Position, Target.Position, Color.Wheat, 4);
		}

	}

	public class HumanPlayer:Player
	{
		MouseState curMs, prevMs;
		KeyboardState curKs, prevKs;

		Planet planetAtMouse, selectedPlanet;



		public HumanPlayer ()
		{
			prevMs = Mouse.GetState ();
			prevKs = Keyboard.GetState ();
		}

		public override void Update (float dt)
		{
			curMs = Mouse.GetState ();
			curKs = Keyboard.GetState ();

			planetAtMouse = World.GetPlanetAtPosition (new Vector2 (curMs.X, curMs.Y));

			if (prevMs.LeftButton == ButtonState.Pressed && curMs.LeftButton == ButtonState.Released) {


				if (selectedPlanet != null) {
					// we already selected a planet
					if (planetAtMouse != null) {
						// and we clicked another planet

						// launch ships
						//selectedPlanet.LaunchShips (planetAtMouse);
						var command = World.FindCommand (this, selectedPlanet);
						if (command == null)
							World.AddEntity (new Command (this, selectedPlanet, planetAtMouse));
						else {
							command.Alive = true;
							command.Target = planetAtMouse;
						}
						selectedPlanet = null;
					} else {
						// and we didnt click another planet
						selectedPlanet = null;
					}
				} else {
					// we dont have anything selected
					if (planetAtMouse != null) {
						// and we clicked another planet
						// select planet if we own it!
						if(planetAtMouse.Owner == this) 
							selectedPlanet = planetAtMouse;
					} else {
						// and we didnt click another planet
						selectedPlanet = null;
					}
				}

			}



			prevMs = curMs;
			prevKs = curKs;
			base.Update (dt);
		}

		public override void Draw (Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			if (planetAtMouse != null) {
				batch.DrawCircle (planetAtMouse.Position, planetAtMouse.Size + 2,  64, Color.Wheat, 4);
			}
			if (selectedPlanet != null) {				
				batch.DrawCircle (selectedPlanet.Position, selectedPlanet.Size + 2,  64, Color.AliceBlue, 4);
				batch.DrawLine (selectedPlanet.Position, new Vector2 (curMs.X, curMs.Y), Color.AliceBlue, 6);
			}
		}
	}
}

