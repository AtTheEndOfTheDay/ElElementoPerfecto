
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
        Nivel nivel1;
        Nivel nivel2;

        TgcTexture madera = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Madera.jpg");
        TgcTexture metal = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Metal.jpg");
        TgcTexture texturaCannon = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Cannon.png");

        TgcSceneLoader loader = new TgcSceneLoader();
        TgcScene scene;
        Cannon cannon;

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
        public static string alumnoMeshFolder()
        {
            return GuiController.Instance.AlumnoEjemplosMediaDir + "MeshCreator\\Meshes\\";
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
            GuiController.Instance.ThirdPersonCamera.setCamera(new Vector3(0, 0, 0), 0, 25);

            scene = loader.loadSceneFromFile(EjemploAlumno.alumnoMeshFolder() + "Cannon-TgcScene.xml");
            cannon = new Cannon(scene.Meshes[0], texturaCannon);
            cannon.mesh.setColor(Color.Black);
            cannon.mesh.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            cannon.mesh.Rotation = new Vector3(0, 0, 3.14f / 4);
            cannon.mesh.move(new Vector3(16, -8f, 1f));

            List<ItemUsuario> itemsUsuarioNivel1 = new List<ItemUsuario>();
            itemsUsuarioNivel1.Add(cannon); //Importa el orden, por como los muestra el menu
            itemsUsuarioNivel1.Add(cannon); 
            //Terminan las inicializaciones de mentira

            nivel1 = new Nivel(madera, madera, madera, new Pelota(0.5f, new Vector3(0, 2, 0.25f), metal),
                                     new List<Item>(), itemsUsuarioNivel1);

            //Inicializaciones de menria
            List<ItemUsuario> itemsUsuarioNivel2 = new List<ItemUsuario>();
            itemsUsuarioNivel2.Add(cannon); //Importa el orden, por como los muestra el menu
            itemsUsuarioNivel2.Add(cannon);
            itemsUsuarioNivel2.Add(cannon);
            itemsUsuarioNivel2.Add(cannon);
            itemsUsuarioNivel2.Add(cannon);
            Pared obstaculo1 = new Pared(TgcBox.fromSize(new Vector3(-10, -6, 0), new Vector3(5, 0.25f, 0.25f), madera), 1);
            Pared obstaculo2 = new Pared(TgcBox.fromSize(new Vector3(-5, -4, 0), new Vector3(5, 0.25f, 0.25f), madera), 1);
            Pared obstaculo3 = new Pared(TgcBox.fromSize(new Vector3(5, -2, 0), new Vector3(5, 0.25f, 0.25f), madera), 1);
            Pared obstaculo4 = new Pared(TgcBox.fromSize(new Vector3(10, 0, 0), new Vector3(5, 0.25f, 0.25f), madera), 1);
            List<Item> itemsNivel2 = new List<Item>();
            itemsNivel2.Add(obstaculo1);
            itemsNivel2.Add(obstaculo2);
            itemsNivel2.Add(obstaculo3);
            itemsNivel2.Add(obstaculo4);
            //Terminan las inicializaciones de mentira
            nivel2 = new Nivel(metal, metal, metal, new Pelota(1f, new Vector3(10, 5, 0.25f), madera),
                                     itemsNivel2, itemsUsuarioNivel2);

            nivelActual = nivel1;

        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.F1))
            {
                nivelActual.reiniciar();
                nivelActual = nivel1;
            }                   
            else
                if (input.keyDown(Key.F2))
                {
                    nivelActual.reiniciar();
                    nivelActual = nivel2;
                }
            
            nivelActual.render(elapsedTime);


        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            nivelActual.dispose();
            nivel1.dispose();
            nivel2.dispose();
        }
        
    }
}
