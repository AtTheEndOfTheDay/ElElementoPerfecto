using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.DirectX;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public static class StringExtension
    {
        public static Boolean IgnoreCaseEquals(this String a, String b)
        {
            return String.Compare(a, b, true) == 0;
        }
        public static Object ParseValue(this String s)
        {
            if (s.StartsWith("@")) return s.Substring(1);
            if (s.StartsWith("#")) return s.ParseColor();
            if (s.Contains(',')) return s.ParseVector();
            if (s.Contains('.')) return s.ParseSingle();
            Int32 param;
            if (Int32.TryParse(s, out param))
                return param;
            return s;
        }
        public static T[] ParseArray<T>(this String s)
        {
            return (T[])s.Split('•').ParseArray(typeof(T));
        }
        public static Array ParseArray(this String s, Type type)
        {
            return s.Split('•').ParseArray(type);
        }
        public static Array ParseArray(this String[] texts, Type type)
        {
            var len = texts.Length;
            var values = Array.CreateInstance(type, len);
            for (var i = 0; i < len; i++)
                values.SetValue(texts[i].ParseValue(), i);
            return values;
        }
        public static Single ParseSingle(this String s)
        {
            return Single.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static Color ParseColor(this String s)
        {
            return Color.FromArgb(Int32.Parse(s.Substring(1), NumberStyles.AllowHexSpecifier));
        }
        public static Object ParseVector(this String s)
        {
            var values = s.Split(',');
            switch (values.Length)
            {
                case 1: return values[0].ParseSingle();
                case 2: return values.ParseVector2();
                case 3: return values.ParseVector3();
            }
            return values.ParseVector4();
        }
        public static Vector2 ParseVector2(this String[] xy)
        {
            return new Vector2(
                xy[0].ParseSingle(),
                xy[1].ParseSingle()
            );
        }
        public static Vector3 ParseVector3(this String[] xyz)
        {
            return new Vector3(
                xyz[0].ParseSingle(),
                xyz[1].ParseSingle(),
                xyz[2].ParseSingle()
            );
        }
        public static Vector4 ParseVector4(this String[] xyzw)
        {
            return new Vector4(
                xyzw[0].ParseSingle(),
                xyzw[1].ParseSingle(),
                xyzw[2].ParseSingle(),
                xyzw[3].ParseSingle()
            );
        }
    }
}
