using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class ItemPart : IDisposable
    {
        public static readonly Color DefaultPartColor = Color.Red;

        public ItemPart()
        {
            Color = DefaultPartColor;
        }

        public virtual Color Color { get; set; }

        public abstract void Attach(Item item);
        public abstract void Detach(Item item);
        public abstract void Render(Item item, Effect shader);
        public abstract void Dispose();
    }
}
