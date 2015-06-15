using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class Collider : IPart
    {
        public static Boolean IsVisible = true;
        public static readonly Color Color = Color.Green;

        public Single Friction = .1f;
        public Single Restitution = .7f;

        public abstract void Attach(Item item);
        public abstract void Detach(Item item);
        public abstract void Render(Item item, Effect shader);
        public abstract void Dispose();

        public abstract Boolean Intercepts(TgcRay ray);
        public virtual Boolean Collides(Collider other)
        {
            return other.Collides(this);
        }
        public virtual Collision Collide(Collider other)
        {
            var c = other.Collide(this);
            c.Invert();
            return c;
        }
    }
}
