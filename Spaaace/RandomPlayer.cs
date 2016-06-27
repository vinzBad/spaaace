using System;
using System.Xml.Xsl.Runtime;
using System.Reflection;
using System.Collections.Generic;

namespace Spaaace
{
	public class RandomPlayer:Player
	{
		public RandomPlayer ()
		{
			
		}


		public override void Think(float dt, List<Planet> ownedPlanets) {
			for (int i = 0; i < ownedPlanets.Count; i++) {


				var planet = ownedPlanets [i];
				if (planet.ShipCount > 10) {					
					var target = World.GetNearestPlanet (planet.Position, ownedPlanets);
					if (target != null)
						planet.LaunchShips (target);
					else
						World.GameHasEnded = true;
				}
			}
		}
	}
}

