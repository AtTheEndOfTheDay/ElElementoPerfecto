using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class Goal : GameComponent
    {
        public Goal(Game game) : base(game) { }
        public abstract void FindTargets(Item[] items);
        public abstract Boolean IsMeet { get; }
        public abstract void Dispose();
    }
}
