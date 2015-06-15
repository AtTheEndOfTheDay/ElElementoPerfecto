using System;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public interface IPart : IDisposable
    {
        void Attach(Item item);
        void Detach(Item item);
        void Render(Item item, Effect shader);
    }
}
