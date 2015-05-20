
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;




/*using System.IO; // Manejo de archivos

using System.Linq;

using TgcViewer.Utils.TgcKeyFrameLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.TgcGeometry;
 */


 

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    /// 
    

    public class EjemploAlumno : TgcExample
    {
        NivelFactory fabricaDeNiveles = new NivelFactory();

        int numeroDeNivel = 1;
        TgcBox fondo;
        MenuObjetos menu;
        TgcBox contenedor;
        Pelota pelota;
        List<Item> itemsDelUsuario = new List<Item>();
        List<Item> itemsDeNivel = new List<Item>();
        //Action<TgcD3dInput, float> etapa;  TODO: revisar...no puedo inicializar una vez al principio me falta el elapsed time
        Etapa etapa;
        Construccion construccion;
        Play play;
        TgcText2d textStage = new TgcText2d();

        public static bool mostrarOBBs=false;
        public static bool mostrarBBs = false;

        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "At_the_end_of_the_day";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "Elemento Perfecto";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "MiIdea - Descripcion de la idea";
        }


        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {          
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(new Vector3(0, 0, 0), 0, 25);

            fondo = TgcBox.fromSize(new Vector3(2.85f, 0, 0), new Vector3(32, 20.75f, 0));
            contenedor = TgcBox.fromSize(new Vector3(-16.05f, -8.25f, 0), new Vector3(5.7f, 4.6f, 0), TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\Laja.jpg"));

            setNivel();
           
            GuiController.Instance.Modifiers.addBoolean("Mostrar OBBs", "Mostrar OBBs", true);
            GuiController.Instance.Modifiers.addBoolean("Mostrar BBs", "Mostrar BBs", true);
        }

        private void setNivel()
        {
            pelota = fabricaDeNiveles.pelotaNivel(numeroDeNivel);
            fabricaDeNiveles.iniciarNivel(numeroDeNivel, itemsDeNivel, itemsDelUsuario, menu, fondo, contenedor);
            menu = fabricaDeNiveles.menuNivel(itemsDelUsuario);
            construccion = new Construccion(itemsDelUsuario, pelota, menu);
            play = new Play(itemsDeNivel, itemsDelUsuario, pelota);

            etapa = construccion;
            textStage.Color = Color.White;
            textStage.Position = new Point(0, 0);
            textStage.Text = "Construccion";

            foreach (Item objeto in itemsDelUsuario)
            {
                objeto.iluminar();
            }

            foreach (Item objeto in itemsDeNivel)
            {
                objeto.iluminar();
            }
        }

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {

            mostrarOBBs = (bool)GuiController.Instance.Modifiers["Mostrar OBBs"];
            mostrarBBs = (bool)GuiController.Instance.Modifiers["Mostrar BBs"];

            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.F1))
            {
                numeroDeNivel = 1;
                setNivel();
            }                   
            else if (input.keyDown(Key.F2))
            {
                numeroDeNivel = 2;
                setNivel();
            }
            else if (input.keyDown(Key.Space))
            {
                etapa = play;
                textStage.Text = etapa.getNombre();
            }
            else if (input.keyDown(Key.C))
            {
                fabricaDeNiveles.reiniciar(numeroDeNivel, pelota);
                etapa = construccion;
                textStage.Text = etapa.getNombre();
            }

            etapa.interaccion(input, elapsedTime);

            fondo.render();
            contenedor.render();

            foreach (Item objeto in itemsDelUsuario)
            {
                objeto.iluminar();
                objeto.render();
            }
            
            foreach (Item objeto in itemsDeNivel)
            {
                objeto.iluminar();
                objeto.render();
            }

            pelota.render();

            GuiController.Instance.Drawer2D.beginDrawSprite();
            menu.renderMenu(itemsDelUsuario.Count);
            textStage.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            pelota.dispose();
            fondo.dispose();
            contenedor.dispose();
            fabricaDeNiveles.dispose();
        }
    }
}
