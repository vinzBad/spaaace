using System;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;


namespace Spaaace
{
	public class Planet:Entity
	{
		public float Size = 40;
		public Color TintColor = Color.Gray;
		public Player Owner = null;
		public float ShipCount = 20;
		public float ShipsPerSecond = 1;
		public float MaxShipCount = 200;
		public float DoubledProductionCount = 10;
		public float MinShipOnPlanet = 15;

		public Planet ()
		{
		}

		public override void Update (float dt)
		{
			if (Owner != null) {
				ShipCount += ShipsPerSecond * dt * Owner.ProdMulti;
				if(ShipCount < DoubledProductionCount)
					ShipCount += ShipsPerSecond * dt;
				if (ShipCount > MaxShipCount)
					ShipCount = MaxShipCount;
			}
		}

		public override void Draw (Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			var color = TintColor;
			if (Owner != null)
				color = Owner.TintColor;

			batch.DrawString(World.DefaultFont, Math.Floor (ShipCount).ToString (), Position, Color.White);
			if (Owner != null)
				batch.DrawString(World.DefaultFont, Owner.ProdMulti.ToString (), Position + new Vector2(0,16), Color.White);
			batch.DrawCircle (Position, Size, 64, color, 4);
		}

		public void LaunchShips(Planet target, int count = 0) {
			if(ShipCount <= MinShipOnPlanet) 
				return;
			if (target == this)
				return;

			var shipCount = count;
			if (count == 0)
			  shipCount = (int)ShipCount / 2;
			
			ShipCount -= shipCount;

			for (int i = 0; i < shipCount; i++) {
				var s = new Ship (Owner, target);
				// spread the ships randomly inside the planet the reduce glitches
				var delta = new Vector2 (
					World.Random.Next (-(int)Size/2,(int) Size/2),
					World.Random.Next (-(int)Size/2, (int)Size/2)
				            );
				s.Position = Position + delta;
				World.AddEntity (s);
			}
		}
	}
}

