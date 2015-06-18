using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Gravitor : Item
    {
        public override void Act(Interactive interactive, Single deltaTime)
        {
            interactive.Momentum += Scale * interactive.Mass;
        }
        public override void ButtonSignal(Object[] signal)
        {
            if (signal.Length == 0) return;
            var value = signal[0];
            if (value is Vector3)
                Scale = (Vector3)value;
            else try { Scale *= Convert.ToSingle(value); }
                catch (Exception) { }
        }
    }
}
