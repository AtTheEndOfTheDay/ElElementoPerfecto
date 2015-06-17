using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    /// <summary>
    /// The Perfect Element is a game, where you have to make a ball enter a cubic dimension portal through a series of levels.
    /// In order to do that, you have to use whatever item you have at your disposal to skip the obstacles between the ball and the portal.
    /// </summary>
    public class ThePerfectElementExample : TgcExample
    {
        #region TextProperties
        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override String getCategory()
        {
            return "AtTheEndOfTheDay";
        }
        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override String getName()
        {
            return "The Perfect Element";
        }
        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override String getDescription()
        {
            return "The Perfect Element is a game, where you have to make a ball enter a cubic dimension portal through a series of levels.\nIn order to do that, you have to use whatever item you have at your disposal to skip the obstacles between the ball and the portal.";
        }
        #endregion TextProperties

        #region Modifiers
        private void _AddModifiers()
        {
            var mods = GuiController.Instance.Modifiers;
            mods.addBoolean("IsMeshVisible", "Mostrar Meshes", Game.Current.IsMeshVisible);
            mods.addBoolean("IsColliderVisible", "Mostrar Colliders", Game.Current.IsColliderVisible);
        }
        private void _SetModifiers()
        {
            var mods = GuiController.Instance.Modifiers;
            Game.Current.IsMeshVisible = (Boolean)mods["IsMeshVisible"];
            Game.Current.IsColliderVisible = (Boolean)mods["IsColliderVisible"];
        }
        #endregion Modifiers

        #region GamePlay
        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            _AddModifiers();
            Game.Current.Init(GuiController.Instance.AlumnoEjemplosMediaDir + "AtTheEndOfTheDay\\");
        }
        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            _SetModifiers();
            Game.Current.Play(elapsedTime);
        }
        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            Game.Current.Dispose();
        }
        #endregion GamePlay
    }
}
