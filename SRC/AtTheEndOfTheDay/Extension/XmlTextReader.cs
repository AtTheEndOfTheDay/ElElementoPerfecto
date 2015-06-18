using System;
using System.Xml;
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
    public static class XmlTextReaderExtension
    {
        private const BindingFlags _BindingFlags = BindingFlags.Public | BindingFlags.Instance;
        public static T SetObjectProperties<T>(this XmlTextReader xml, T instance, Type type = null)
        {
            try
            {
                type = type ?? instance.GetType();
                while (xml.MoveToNextAttribute())
                    if (!String.IsNullOrWhiteSpace(xml.Value))
                        try
                        {
                            var prop = type.GetProperty(xml.Name, _BindingFlags);
                            var value = prop.PropertyType.IsArray
                                ? xml.Value.ParseArray(prop.PropertyType.GetElementType())
                                : xml.Value.ParseValue();
                            prop.SetValue(instance, value, null);
                        }
                        catch (Exception e) { }
            }
            catch (Exception e) { }
            return instance;
        }
    }
}
