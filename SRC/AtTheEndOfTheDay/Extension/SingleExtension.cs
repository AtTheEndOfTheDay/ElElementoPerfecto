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
    internal static class SingleExtension
    {
        public const Single Tolerance = .001f;
        public static Boolean TolerantEquals(this Single a, Single b)
        {
            return Tolerance + b > a && a > b - Tolerance;
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
