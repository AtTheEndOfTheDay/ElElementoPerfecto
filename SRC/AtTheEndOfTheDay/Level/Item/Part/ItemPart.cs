using System;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class ItemPart : GameComponent
    {
        public ItemPart(Game game) : base(game) { }
        public abstract void Attach(Item item);
        public abstract void Detach(Item item);
        public abstract void Render(Item item, Effect shader);
    }
}
