using System;
using Microsoft.DirectX;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Contact
    {
        public Contact(Vector3 pointA, Vector3 pointB, Vector3 normalAB, Single depth)
        {
            PointA = pointA;
            PointB = pointB;
            NormalAB = normalAB;
            Depth = depth;
            MinimumTranslation = normalAB * depth;
        }
        public Vector3 PointA { get; private set; }
        public Vector3 PointB { get; private set; }
        public Vector3 NormalAB { get; private set; }
        public Single Depth { get; private set; }
        public Vector3 MinimumTranslation { get; private set; }
        public void Invert()
        {
            NormalAB *= -1;
            var p = PointA;
            PointA = PointB;
            PointB = p;
        }
    }
}
