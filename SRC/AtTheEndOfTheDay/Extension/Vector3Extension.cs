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
    internal static class Vector3Extension
    {
        #region Constants
        public static readonly Vector3 One = new Vector3(1, 1, 1);
        public static readonly Vector3 Right = new Vector3(1, 0, 0);
        public static readonly Vector3 Left = -Right;
        public static readonly Vector3 Top = new Vector3(0, 1, 0);
        public static readonly Vector3 Bottom = -Top;
        public static readonly Vector3 Front = new Vector3(0, 0, 1);
        public static readonly Vector3 Back = -Front;
        #endregion Constants

        public static Single MinCoordinate(this Vector3 v)
        {
            return Math.Min(Math.Min(v.X, v.Y), v.Z);
        }
        public static Single MaxCoordinate(this Vector3 v)
        {
            return Math.Max(Math.Max(v.X, v.Y), v.Z);
        }
        public static Vector3 SetX(this Vector3 v, Single x)
        {
            v.X = x; return v;
        }
        public static Vector3 SetY(this Vector3 v, Single y)
        {
            v.Y = y; return v;
        }
        public static Vector3 SetZ(this Vector3 v, Single z)
        {
            v.Z = z; return v;
        }
        public static Vector3 AddX(this Vector3 v, Single step)
        {
            v.X += step; return v;
        }
        public static Vector3 AddY(this Vector3 v, Single step)
        {
            v.Y += step; return v;
        }
        public static Vector3 AddZ(this Vector3 v, Single step)
        {
            v.Z += step; return v;
        }
        public static Vector3 MultX(this Vector3 v, Single step)
        {
            v.X *= step; return v;
        }
        public static Vector3 MultY(this Vector3 v, Single step)
        {
            v.Y *= step; return v;
        }
        public static Vector3 MultZ(this Vector3 v, Single step)
        {
            v.Z *= step; return v;
        }
        public static Vector3 AdvanceX(this Vector3 v, Single step, Single to)
        {
            v.X = v.X.AdvanceTo(step, to); return v;
        }
        public static Vector3 AdvanceY(this Vector3 v, Single step, Single to)
        {
            v.Y = v.Y.AdvanceTo(step, to); return v;
        }
        public static Vector3 AdvanceZ(this Vector3 v, Single step, Single to)
        {
            v.Z = v.Z.AdvanceTo(step, to); return v;
        }
        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
        }
        public static Vector3 MemberwiseMult(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }
        public static Vector3 MemberwiseDiv(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }
        public static Vector3 Orthogonal(this Vector3 v, Vector3 direction)
        {
            var ortho = Vector3.Cross(v, direction);
            if (ortho == Vector3.Empty) return Vector3.Empty;
            ortho = Vector3.Cross(ortho, v);
            return ortho;
        }
        public static Vector3 Orthonormal(this Vector3 v, Vector3 direction)
        {
            var ortho = Vector3.Cross(v, direction);
            if (ortho == Vector3.Empty) return Vector3.Empty;
            ortho = Vector3.Cross(ortho, v);
            ortho.Normalize();
            return ortho;
        }
        public static Vector3 ClampX(this Vector3 v, Single min, Single max)
        {
            v.X = v.X.Clamp(min, max); return v;
        }
        public static Vector3 ClampY(this Vector3 v, Single min, Single max)
        {
            v.Y = v.Y.Clamp(min, max); return v;
        }
        public static Vector3 ClampZ(this Vector3 v, Single min, Single max)
        {
            v.Z = v.Z.Clamp(min, max); return v;
        }
        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(
                v.X.Clamp(min.X, max.X),
                v.Y.Clamp(min.Y, max.Y),
                v.Z.Clamp(min.Z, max.Z)
            );
        }
        public static Matrix StarMatrix(this Vector3 v)
        {
            return new Matrix()
            {
                M11=   0, M12=-v.Z, M13= v.Y,
                M21= v.Z, M22=   0, M23=-v.X,
                M31=-v.Y, M32= v.X, M33=   0,
                M44 = 0,
            };
        }
        public static Single[] ToArray(this Vector3 v)
        {
            return new Single[] { v.X, v.Y, v.Z };
        }
        public static Vector3 ToVector3(this Single[] a)
        {
            return new Vector3(a[0], a[1], a[2]);
        }
    }
}
