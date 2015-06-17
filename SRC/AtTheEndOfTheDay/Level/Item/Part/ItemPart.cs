using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class ItemPart : GameComponent
    {
        public static readonly Color DefaultPartColor = Color.Red;

        public ItemPart(Game game)
            : base(game)
        {
            Color = DefaultPartColor;
        }

        public virtual Color Color { get; set; }

        public abstract void Attach(Item item);
        public abstract void Detach(Item item);
        public abstract void Render(Item item, Effect shader);
    }
}
