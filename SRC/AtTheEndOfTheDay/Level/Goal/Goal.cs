using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public interface IGoal : IGameComponent
    {
        void FindTargets(Item[] items);
        Boolean IsMeet { get; }
        void Dispose();
    }
}
