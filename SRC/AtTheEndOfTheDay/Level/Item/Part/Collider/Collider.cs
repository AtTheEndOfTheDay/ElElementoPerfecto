using System;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class Collider : ItemPart
    {
        public static Boolean IsVisible = true;
        public const Single DefaultFriction = .1f;
        public const Single DefaultRestitution = .7f;
        public static readonly Color DefaultColor = Color.Green;

        public Collider(Game game)
            : base(game)
        {
            Friction = DefaultFriction;
            Restitution = DefaultRestitution;
            Color = DefaultColor;
            Scale = Item.DefaultScale;
            Position = Item.DefaultPosition;
            Rotation = Item.DefaultRotation;
        }

        public Single Friction { get; set; }
        public Single Restitution { get; set; }
        public virtual Color Color { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

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
