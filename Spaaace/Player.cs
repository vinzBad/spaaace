using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Extended.Shapes;

namespace Spaaace
{
	public class Player: Entity
	{
		public Color TintColor = Color.Magenta;
		//public List<Planet> ownedPlanets = new List<Planet>();

		public float ProdMulti = 4;
		public float MaxProdMulti = 4;
		public float MinProdMulti = 0.25f;

		public Player ()
		{
		}


		float Scale(float value, float min, float max, float newMin, float newMax) {
			/*
			      (b-a)(x - min)
			f(x) = --------------  + a
          		   max - min			  
			 */

			return (value - min) / max * (newMax - newMin) + newMin; // 0-1

		}

		public override void Update (float dt)
		{
			var ownedPlanets = World.GetPlanetsOwnedBy (this);
			ProdMulti = Scale (ownedPlanets.Count, 1, World.NumberOfPlanets () / 2, MaxProdMulti, MinProdMulti);
			if (ProdMulti < MinProdMulti)
				ProdMulti = MinProdMulti;
			Think (dt, ownedPlanets);
			base.Update (dt);
		}

		public virtual void Think(float dt, List<Planet> ownedPlanets) {
			
		}

		/*
		public override void Draw (Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			ownedPlanets.ForEach ( 
				p => {
					batch.DrawCircle(p.Position, p.Size - 4, 64, TintColor, 4);
				});
		}
		*/
	}
}

