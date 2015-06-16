using System;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class GameComponent : IDisposable
    {
        protected readonly Game Game;
        public GameComponent(Game game) { Game = game; }
        public abstract void Dispose();
    }
}
