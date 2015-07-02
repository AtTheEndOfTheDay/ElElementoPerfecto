using System;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public interface IGoal : IGameComponent, IDisposable
    {
        void FindTargets(Item[] items);
        Boolean IsMeet { get; }
    }
}
