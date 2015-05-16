
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
        MenuObjetos menu;

        TgcText2d textStage;

        TgcBox fondo;
        Item piso;
        Item lateralDerecha;
        Item lateralIzquierda;


        Pelota pelota;
        Vector3 iniPelota = new Vector3(0, 100, 0);
        Stage construccion;
        Stage play;
        Stage stage;


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

        public static string alumnoTextureFolder()
        {
            return GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            List<Item> itemsInScenario = new List<Item>();

            TgcTexture textPiso = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Fondo.jpg");            
            
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            Vector3 centro = new Vector3(0, 0, 0);


            GuiController.Instance.Modifiers.addFloat("medida", 0, 150, 85);
            
            fondo = TgcBox.fromSize(new Vector3(11.5f, 0, 0), new Vector3(128, 83, 0), textPiso);

            piso = new Pared(TgcBox.fromSize(new Vector3(11.5f, -41, 0), new Vector3(128, 1, 1), textPiso), 1);
            lateralDerecha = new Pared(TgcBox.fromSize(new Vector3(-53f, 0, 0), new Vector3(1, 83, 1), textPiso), 1);
            lateralIzquierda = new Pared(TgcBox.fromSize(new Vector3(75f, 0, 0), new Vector3(1, 83, 1), textPiso), 1);


            pelota = new Pelota();

            itemsInScenario.Add(piso);
            itemsInScenario.Add(lateralDerecha);
            itemsInScenario.Add(lateralIzquierda);

            construccion = new Construccion(itemsInScenario,pelota);
            play = new Play(itemsInScenario,pelota);
            stage = construccion;

            

            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(centro, 0, 100);

            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
     
            menu = new MenuObjetos(12);

            textStage = new TgcText2d();
            
            textStage.Color = Color.White;
            textStage.Position = new Point(0,0);
            textStage.Text = "Construccion";

        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput input = GuiController.Instance.D3dInput;
            Vector3 movement = new Vector3(0, 0, 0);

            if ((input.keyDown(Key.Return))&&(stage.Equals(construccion)))
            {
                pelota.reiniciar();
                stage = play;
                textStage.Text = stage.getNombre();
            }
            else
            {
                if ((input.keyDown(Key.C)) && (stage.Equals(play)))
                {
                    pelota.reiniciar();
                    stage = construccion;
                    textStage.Text = stage.getNombre();
                }
                    
            }
            
            stage.interaccion(input,elapsedTime);

            stage.aplicarMovimientos(elapsedTime);

            stage.render();
            

            fondo.render();
            
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            menu.renderMenu(12);

            textStage.render();

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            menu.disposeMenu(12);
            fondo.dispose();
            piso.dispose();
            lateralDerecha.dispose();
            lateralIzquierda.dispose();
        }

    }
}
