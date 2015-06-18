using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public static class SingleExtension
    {
        private const Single _Tolerance = .001f;
        public static Boolean TolerantEquals(this Single a, Single b, Single tolerance = _Tolerance)
        {
            return tolerance + b > a && a > b - tolerance;
        }
        public static Single Abs(this Single s)
        {
            return Math.Abs(s);
        }
        public static Single AdvanceTo(this Single x, Single step, Single to)
        {
            return x < to
                ? Math.Min(to, x + step)
                : Math.Max(to, x - step);
        }
        public static Single Clamp(this Single x, Single min, Single max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }
    }
}
