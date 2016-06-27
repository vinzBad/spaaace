using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace Spaaace
{
	public class Entity
	{
		public Vector2 Position;

		public bool Active = true;
		public bool Alive = true;
		public bool Visible = true;

		public World World;

		public Entity ()
		{
		}

		public virtual void Update(float dt) {
		
		}

		public virtual void Draw(SpriteBatch batch) {
		}
	}
}

