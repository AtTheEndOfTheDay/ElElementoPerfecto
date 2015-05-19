
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
        float pi = (float)Math.PI;
        Nivel nivelActual;
        Nivel nivel1;
        Nivel nivel2;

        TgcTexture madera = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Madera.jpg");
        TgcTexture metal = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Metal.jpg");
        TgcTexture texturaCannon = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Cannon.png");
        TgcTexture texturaMagnet = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Magnet.png");
        TgcTexture texturaSpring = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Spring.png");

        Pelota[] pelotas;
        TgcSceneLoader loader = new TgcSceneLoader();
        TgcScene scene;

        Cannon cannon;
        Cannon cannon2;
        Magnet magnet1;
        Magnet magnet2;
        Spring spring1;

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


            pelotas = new Pelota[2];
            scene = loader.loadSceneFromFile(EjemploAlumno.alumnoMeshFolder() + "Elements-TgcScene.xml");
            
            iniciarNivel1();
            iniciarNivel2();
            
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
                reiniciar(nivel1);
                nivelActual = nivel1;
            }                   
            else if (input.keyDown(Key.F2))
                {
                    reiniciar(nivel2);
                    nivelActual = nivel2;
            }
            else if (input.keyDown(Key.Space))
            {
                nivelActual.pasaAPlay();
            }
            else if (input.keyDown(Key.C))
            {
                reiniciar(nivelActual);
            }    
            
            nivelActual.render(elapsedTime);

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            nivel1.dispose();
            nivel2.dispose();
        }
        private void iniciarNivel1()
        {
            pelotas[0] = new Pelota(0.5f, new Vector3(16, -8f, 1), metal);
            
            //Items del Nivel
            cannon = new Cannon(scene.Meshes[0], texturaCannon, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi/4), new Vector3(16, -8f, 1f));
            cannon.enEscena = true;
            cannon.cargado = true;
            List<Item> itemsNivel1 = new List<Item>();
            itemsNivel1.Add(cannon);
            //Fin Items del Nivel

            //Items del Usuario
            magnet1 = new Magnet(scene.Meshes[3], texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
            magnet2 = new Magnet(scene.Meshes[3].clone("magnet2"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
            spring1 = new Spring(scene.Meshes[1], texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
           // spring1 = new Spring(scene.Meshes[3], texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, pi / 2, pi));
            List<Item> itemsUsuario1 = new List<Item>();
            itemsUsuario1.Add(magnet1);
            itemsUsuario1.Add(magnet2);
            itemsUsuario1.Add(spring1);
            //Fin Items del Usuario

            nivel1 = new Nivel(1, madera, madera, madera, pelotas[0], itemsNivel1, itemsUsuario1);
        }

        private void iniciarNivel2()
        {
            pelotas[1] = new Pelota(1f, new Vector3(10, 5, 1), madera);

            //Items del Nivel
            Pared obstaculo1 = Pared.CrearPared(new Vector3(-10, -6, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "Obstaculo1");
            Pared obstaculo2 = Pared.CrearPared(new Vector3(-5, -4, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "Obstaculo2");
            Pared obstaculo3 = Pared.CrearPared(new Vector3(5, -2, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "Obstaculo3");
            Pared obstaculo4 = Pared.CrearPared(new Vector3(10, 0, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, FastMath.PI / 4), madera, "Obstaculo4");
            List<Item> itemsNivel2 = new List<Item>();
            itemsNivel2.Add(obstaculo1);
            itemsNivel2.Add(obstaculo2);
            itemsNivel2.Add(obstaculo3);
            itemsNivel2.Add(obstaculo4);
            
            //Fin Items del Nivel

            //Items del Usuario
            cannon2 = new Cannon(scene.Meshes[0].clone("Cannon2"), texturaCannon, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi/4));
            List<Item> itemsUsuarioNivel2 = new List<Item>();
            itemsUsuarioNivel2.Add(cannon2);
            //Fin Items del Usuario  

            nivel2 = new Nivel(2, metal, metal, metal, pelotas[1], itemsNivel2, itemsUsuarioNivel2);
        }

        private void reiniciar(Nivel nivel)
        {
            nivelActual.pasaAConstruccion();
            switch (nivel.numeroDeNivel)
            {
                case 1:
                    pelotas[0].reiniciar();
                    cannon.cargado = true;
                    break;
                case 2:
                    pelotas[1].reiniciar();
                    break;
            }
        }
        
    }
}
