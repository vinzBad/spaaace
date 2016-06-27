using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System.Security.Cryptography;
using System.Runtime.InteropServices;


namespace Spaaace
{
	public class Ship:Entity
	{
		public Player Owner;
		public Planet Target;
		public Vector2 Velocity;
		public float MaxSpeed = 60;
		public float MaxForce = 40;
		public float MaxRange = 15;
		public float Mass = 8;

		Vector2 acceleration = Vector2.Zero;

		public Ship (Player owner, Planet target)
		{
			this.Owner = owner;
			this.Target = target;
		}


		Vector2 desired = Vector2.Zero;
		Vector2 steering = Vector2.Zero;

		public override void Update (float dt)
		{

			// avoid other ships
			/*
			var otherShips = World.GetShipsInRange (Position, MaxRange);

			for (int i = 0; i < otherShips.Length; i++) {
				var other = otherShips [i];

				desired = this.Position - other.Position;
				// var dist = desired.Length ();
				desired.Normalize ();
				desired *= MaxSpeed; //- MaxSpeed * dist / MaxRange;

				steering = desired - Velocity;
				if (!steering.IsNaN ())
					ApplyForce (steering);
			}
			*/


			// seek behaviour
			desired = Target.Position - this.Position;
			if (desired.Length () < Target.Size) {
				this.Alive = false;

				if (Target.Owner == this.Owner) {
					Target.ShipCount++;
				} else if (Target.Owner == null) {
					Target.ShipCount--;

				} else {
					Target.ShipCount -= 0.5f;
				}

				if (Target.ShipCount <= 0) {
					Target.Owner = this.Owner;
				}
			}
			desired.Normalize ();
			desired *= MaxSpeed;
			steering = desired - Velocity;
			ApplyForce (steering);

			// apply acceleration
			this.Velocity += acceleration;
			acceleration = Vector2.Zero;

			//apply velocity
			this.Position += this.Velocity * dt;


		}

		public override void Draw (Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			batch.DrawPoint (Position, Owner.TintColor, 6);
		}

		public void ApplyForce (Vector2 force)
		{
			force = force.Truncate (MaxForce);
			acceleration += force / Mass;
		}
	}
}

