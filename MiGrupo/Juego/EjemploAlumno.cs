
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

        Pelota[] pelotas;
        TgcSceneLoader loader = new TgcSceneLoader();
        TgcScene scene;
        Cannon cannon;
        Cannon cannon2;

        /// <summary>
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
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
        /// Completar con la descripci�n del TP
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
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
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
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.F1))
            {
                iniciarNivel1();
                nivelActual = nivel1;
            }                   
            else if (input.keyDown(Key.F2))
                {
                    iniciarNivel2();
                    nivelActual = nivel2;
            }
            else if (input.keyDown(Key.Space))
            {
                nivelActual.pasaAPlay();
            }
            else if (input.keyDown(Key.C))
            {
                reiniciar(nivelActual);
                nivelActual.pasaAConstruccion();
            }    
            
            nivelActual.render(elapsedTime);

        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
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
            cannon = new Cannon(scene.Meshes[0], texturaCannon);
            cannon.mesh.setColor(Color.Black);
            cannon.mesh.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            cannon.mesh.Rotation = new Vector3(0, 0, pi / 4);
            cannon.mesh.move(new Vector3(16 - cannon.mesh.Position.X, -8f -cannon.mesh.Position.Y, 1f - cannon.mesh.Position.Z));
            cannon.enEscena = true;
            cannon.cargado = true;
            List<Item> itemsNivel1 = new List<Item>();
            itemsNivel1.Add(cannon);
            nivel1 = new Nivel(1, madera, madera, madera, pelotas[0], itemsNivel1, new List<Item>());
        }

        private void iniciarNivel2()
        {
            pelotas[1] = new Pelota(1f, new Vector3(10, 5, 1), madera);
            cannon2 = new Cannon(scene.Meshes[3], texturaCannon);
            cannon2.mesh.setColor(Color.Black);
            cannon2.mesh.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            cannon2.mesh.Rotation = new Vector3(0, 0/2, 0);
            cannon2.llevarAContenedor();
            cannon2.enEscena = false;

            List<Item> itemsUsuarioNivel2 = new List<Item>();
            itemsUsuarioNivel2.Add(cannon2); //Importa el orden, por como los muestra el menu
            Pared obstaculo1 = new Pared(TgcBox.fromSize(new Vector3(-10, -6, 1), new Vector3(5, 0.25f, 1), madera).toMesh("Obstaculo1"), madera);
            Pared obstaculo2 = new Pared(TgcBox.fromSize(new Vector3(-5, -4, 1), new Vector3(5, 0.25f, 1), madera).toMesh("Obstaculo2"), madera);
            Pared obstaculo3 = new Pared(TgcBox.fromSize(new Vector3(5, -2, 1), new Vector3(5, 0.25f, 1), madera).toMesh("Obstaculo3"), madera);
            Pared obstaculo4 = new Pared(TgcBox.fromSize(new Vector3(10, 0, 1), new Vector3(5, 0.25f, 1), madera).toMesh("Obstaculo4"), madera);
            List<Item> itemsNivel2 = new List<Item>();
            itemsNivel2.Add(obstaculo1);
            itemsNivel2.Add(obstaculo2);
            itemsNivel2.Add(obstaculo3);
            itemsNivel2.Add(obstaculo4);

            nivel2 = new Nivel(2, metal, metal, metal, pelotas[1], itemsNivel2, itemsUsuarioNivel2);
        }

        private void reiniciar(Nivel nivel)
        {
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