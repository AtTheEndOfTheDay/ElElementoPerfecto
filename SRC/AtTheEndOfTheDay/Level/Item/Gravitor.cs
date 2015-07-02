using System;
using Microsoft.DirectX;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Gravitor : Item
    {
        public override void Signal(Object[] signal)
        {
            if (signal.Length == 0) return;
            var value = signal[0];
            if (value is Vector3)
                Scale = (Vector3)value;
            else try { Scale *= Convert.ToSingle(value); }
                catch (Exception) { }
        }
        public override void Act(Interactive interactive, Single deltaTime)
        {
            interactive.Momentum += Scale * interactive.Mass;
        }
    }
}
