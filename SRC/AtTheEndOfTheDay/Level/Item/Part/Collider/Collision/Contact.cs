using System;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Contact
    {
        public Vector3 PointA { get; private set; }
        public Vector3 PointB { get; private set; }
        public Vector3 NormalAB { get; private set; }
        public Single Depth { get; private set; }
        public Contact(Vector3 pointA, Vector3 pointB, Vector3 normalAB, Single depth)
        {
            PointA = pointA;
            PointB = pointB;
            NormalAB = normalAB;
            Depth = depth;
        }
        public void Invert()
        {
            NormalAB *= -1;
            var p = PointA;
            PointA = PointB;
            PointB = p;
        }
    }
}
