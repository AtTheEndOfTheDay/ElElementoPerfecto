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
        private Game _Game = null;

        #region TextProperties
        /// <summary>
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
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
        /// Completar con la descripci�n del TP
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
            mods.addBoolean("Collider.IsVisible", "Mostrar Colliders", Collider.IsVisible);
            mods.addBoolean("MeshPart.IsVisible", "Mostrar Meshes", MeshPart.IsVisible);
            //GuiController.Instance.UserVars.addVar("Vx", 0f);
            //GuiController.Instance.UserVars.addVar("Vy", 0f);
            //GuiController.Instance.UserVars.addVar("Vz", 0f);
        }
        private void _SetModifiers()
        {
            var mods = GuiController.Instance.Modifiers;
            Collider.IsVisible = (Boolean)mods["Collider.IsVisible"];
            MeshPart.IsVisible = (Boolean)mods["MeshPart.IsVisible"];
        }
        #endregion Modifiers

        #region GamePlay
        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            _AddModifiers();
            _Game = new Game(GuiController.Instance.AlumnoEjemplosMediaDir + "AtTheEndOfTheDay\\");
        }
        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            _SetModifiers();
            _Game.Play(elapsedTime);
        }
        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            _Game.Dispose();
        }
        #endregion GamePlay
    }
}
