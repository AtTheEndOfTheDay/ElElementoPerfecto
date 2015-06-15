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
    internal static class TgcObbExtension
    {
        private static readonly Vector3[] _RotationEmpty = { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };
        public static void SetOrientation(this TgcObb obb)
        {
            var e = _RotationEmpty;
            obb.SetOrientation(e[0], e[1], e[2]);
        }
        public static void SetOrientation(this TgcObb obb, Vector3 rotation)
        {
            obb.SetOrientation(Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z));
        }
        public static void SetOrientation(this TgcObb obb, Vector3 o0, Vector3 o1, Vector3 o2)
        {
            var o = obb.Orientation = obb.Orientation;
            o[0] = o0; o[1] = o1; o[2] = o2;
        }
        public static void SetOrientation(this TgcObb obb, Matrix rotation)
        {
            obb.SetOrientation(
                new Vector3(rotation.M11, rotation.M12, rotation.M13),
                new Vector3(rotation.M21, rotation.M22, rotation.M23),
                new Vector3(rotation.M31, rotation.M32, rotation.M33)
            );
        }
        public static Matrix GetOrientation(this TgcObb obb)
        {
            var o = obb.Orientation;
            return new Matrix()
            {
                M11= o[0].X, M12= o[0].Y, M13= o[0].Z,
                M21= o[1].X, M22= o[1].Y, M23= o[1].Z,
                M31= o[2].X, M32= o[2].Y, M33= o[2].Z,
                M44=1,
            };
        }
        public static Vector3 ClosestPoint(this TgcObb obb, Vector3 p)
        {
            return p
                .ToObbSpace(obb)
                .Clamp(-obb.Extents, obb.Extents)
                .FromObbSpace(obb);
        }
        public static Vector3 ToObbSpace(this  Vector3 p, TgcObb obb)
        {
            var t = p - obb.Center;
            var o = obb.Orientation;
            return new Vector3(Vector3.Dot(t, o[0]), Vector3.Dot(t, o[1]), Vector3.Dot(t, o[2]));
        }
        public static Vector3 FromObbSpace(this  Vector3 p, TgcObb obb)
        {
            var o = obb.Orientation;
            return obb.Center + p.X * o[0] + p.Y * o[1] + p.Z * o[2];
        }
    }
}
