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
    internal static class TypeExtension
    {
        public static Type[] LoadSubTypes(this Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t))
                .ToArray();
        }
        public static void InvokeMethod(this Type[] types, String methodName, BindingFlags bindingFlags, Object[] parameters = null)
        {
            var methods = types
                .Select(i => i.GetMethod(methodName, bindingFlags))
                .ToArray();
            foreach (var m in methods)
                if (m != null)
                    m.Invoke(null, parameters);
        }
        private const BindingFlags _BindingFlags = BindingFlags.Public | BindingFlags.Instance;
        public static T LoadFieldsFromText<T>(this T instance, String[] text)
        {
            foreach (var field in text)
            {
                if (String.IsNullOrWhiteSpace(field)) continue;
                var f = field.Split('=');
                var name = f[0].Trim();
                var value = f[1].Trim();
                typeof(T).GetField(name, _BindingFlags)
                    .SetValue(instance, value.GetParam());
            }
            return instance;
        }

        #region Creation
        public static List<T> CreateFromTextStart<T>(this Type[] types, String[] texts, String textStart, params Object[] startParams)
        {
            foreach (var text in texts)
                if (text.StartsWith(textStart))
                    return types.CreateFromText<T>(text.Substring(textStart.Length), startParams);
            return null;
        }
        public static List<T> CreateFromText<T>(this Type[] types, String text, params Object[] startParams)
        {
            var creations = new List<T>();
            var creationTexts = text.Split('\n');
            foreach (var creationText in creationTexts)
            {
                if (String.IsNullOrWhiteSpace(creationText)) continue;
                var creation = creationText.Split('=');
                var name = creation[0].Trim();
                Type type = null;
                try { type = types.First(t => t.Name.Equals(name)); }
                catch { throw new ArrayTypeMismatchException("Type not found."); }
                creations.Add(type.CreateFromText<T>(creation[1], startParams));
            }
            return creations;
        }
        public static T CreateFromText<T>(this Type type, String creation, params Object[] startParams)
        {
            return (T)Activator.CreateInstance(type,
                startParams.Concat(
                    _GetCreationParameters(creation)
            ).ToArray());
        }
        private static Object[] _GetCreationParameters(String creation)
        {
            var creationParams = new List<Object>();
            var parameters = creation.Split(';');
            foreach (var parameter in parameters)
            {
                if (String.IsNullOrWhiteSpace(parameter)) continue;
                var p = parameter.Trim();
                creationParams.Add(p.GetParam());
            }
            return creationParams.ToArray();
        }
        #endregion Creation
    }
}
