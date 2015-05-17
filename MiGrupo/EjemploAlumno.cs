
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
        Nivel nivelActual;

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
            //Propios de cada nivel, delegar al nivel
                   
            
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(new Vector3(0, 0, 0), 0, 100);

            //Es como un mock de un Cannon, un Magnet y un Spring, de momento solo para obtener la textura y ver que el menu funcione
            Pared supuestoCannon = new Pared(TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 
                                            TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Cannon.png")), 1);
            Pared supuestoMagnet = new Pared(TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(0, 0, 0),
                                            TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Magnet.png")), 1);
            Pared supuestoSpring = new Pared(TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(0, 0, 0),
                                            TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Spring.png")), 1);
            List<Item> listaUsuario = new List<Item>();
            listaUsuario.Add(supuestoCannon); //Importa el orden, por como los muestra el menu
            listaUsuario.Add(supuestoMagnet); 
            listaUsuario.Add(supuestoCannon);
            listaUsuario.Add(supuestoMagnet);
            listaUsuario.Add(supuestoSpring);
            //Terminan las inicializaciones de mentira

            nivelActual = new Nivel (TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Madera.jpg"),
                                     TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Madera.jpg"),
                                     TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Madera.jpg"),
                                     new Pelota(2f, new Vector3(0, 10, 0), TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Metal.jpg")),
                                     new List<Item>(),
                                     listaUsuario);

            

        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            nivelActual.render(elapsedTime);
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            nivelActual.dispose();
        }
        
    }
}
