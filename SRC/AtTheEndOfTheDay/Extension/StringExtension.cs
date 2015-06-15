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

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    internal static class StringExtension
    {
        public static Object GetParam(this String s)
        {
            if (s.StartsWith("\"")) return s.Substring(1, s.Length - 2);
            if (s.Contains(',')) return s.GetVector();
            if (s.Contains('.')) return s.GetSingle();
            if (s.StartsWith("#")) return s.GetColor();
            Int32 param;
            if (Int32.TryParse(s, out param))
                return param;
            return null;
        }
        public static float GetSingle(this String s)
        {
            return Single.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static Color GetColor(this String s)
        {
            return Color.FromArgb(Int32.Parse(s.Substring(1), NumberStyles.AllowHexSpecifier));
        }
        public static Object GetVector(this String s)
        {
            var values = s.Split(',');
            switch (values.Length)
            {
                case 1: return values[0].GetSingle();
                case 2: return values.GetVector2();
                case 3: return values.GetVector3();
            }
            return values.GetVector4();
        }
        public static Vector2 GetVector2(this String[] xy)
        {
            return new Vector2(
                xy[0].GetSingle(),
                xy[1].GetSingle()
            );
        }
        public static Vector3 GetVector3(this String[] xyz)
        {
            return new Vector3(
                xyz[0].GetSingle(),
                xyz[1].GetSingle(),
                xyz[2].GetSingle()
            );
        }
        public static Vector4 GetVector4(this String[] xyzw)
        {
            return new Vector4(
                xyzw[0].GetSingle(),
                xyzw[1].GetSingle(),
                xyzw[2].GetSingle(),
                xyzw[3].GetSingle()
            );
        }
    }
}
