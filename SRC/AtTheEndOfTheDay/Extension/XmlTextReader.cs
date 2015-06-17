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
                        type.GetProperty(xml.Name, _BindingFlags)
                            .SetValue(instance, xml.Value.ParseValue(), null);
            }
            catch (Exception e) { }
            return instance;
        }
    }
}
