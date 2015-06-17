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
    public static class TypeExtension
    {
        public static Object NewInstance(this Type type, params Object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }
        public static Type[] FindSubTypes(this Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t))
                .ToArray();
        }
        public static Type[] FindSubTypes(this Type type, Type[] types)
        {
            return types.Where(t => type.IsAssignableFrom(t)).ToArray();
        }
    }
}
