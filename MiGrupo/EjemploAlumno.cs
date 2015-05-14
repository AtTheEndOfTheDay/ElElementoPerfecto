
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
            

            Color color = Color.Red;
            fondo = TgcBox.fromSize(new Vector3(145, 0, 0), new Vector3(1200, 900, 0), textPiso);
            
            piso = new Pared(TgcBox.fromSize(new Vector3(145, -408, 0), new Vector3(1200, 10, 10), textPiso),1);
            lateralDerecha = new Pared(TgcBox.fromSize(new Vector3(-445, 0, 0), new Vector3(10, 900, 10), textPiso),2);
            lateralIzquierda = new Pared(TgcBox.fromSize(new Vector3(750, 0, 0), new Vector3(10, 900, 10), textPiso),3);


            pelota = new Pelota();

            itemsInScenario.Add(piso);
            itemsInScenario.Add(lateralDerecha);
            itemsInScenario.Add(lateralIzquierda);

            construccion = new Construccion(itemsInScenario,pelota);
            play = new Play(itemsInScenario,pelota);
            stage = construccion;

            

            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(centro, 0, 1000);
           // GuiController.Instance.ThirdPersonCamera.

            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
     
            menu = new MenuObjetos();
            

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
            }
            else
            {
                if ((input.keyDown(Key.C)) && (stage.Equals(play)))
                {
                    pelota.reiniciar();
                    stage = construccion;
                }
            }
            
            stage.interaccion(input,elapsedTime);

            stage.aplicarMovimientos(elapsedTime);

            stage.render();
            /*
            if (input.keyDown(Key.A))
            {

                movement.X = 1;
                pelota.rotateZ(-15 * elapsedTime);
            }
            else if (input.keyDown(Key.D))
            {
                movement.X = -1;
                pelota.rotateZ(15 * elapsedTime);
            }

            //Vector3 originalPos = pelota.Position;
            */

            /*
            movement.Y = -1.5f;
            if (TgcCollisionUtils.testSphereAABB(pelota.esfera.BoundingSphere, piso.BoundingBox))
            {
                movement.Y = 1.5f;
            }
            if (TgcCollisionUtils.testSphereAABB(pelota.esfera.BoundingSphere, lateralDerecha.BoundingBox))
            {
                movement.X = 1;
            }
            if (TgcCollisionUtils.testSphereAABB(pelota.esfera.BoundingSphere, lateralIzquierda.BoundingBox))
            {
                movement.X = -1;
            }
            */


            //Aplicar movimiento
            //movement *= MOVEMENT_SPEED * elapsedTime;
            //pelota.move(movement);


            fondo.render();
            //piso.render();
            //lateralDerecha.render();
            //lateralIzquierda.render();
            //pelota.render();
            //pelota.updateValues();
            
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            menu.renderMenu();

            stage.mostrarStage();

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            menu.disposeMenu();
            fondo.dispose();
            piso.dispose();
            lateralDerecha.dispose();
            lateralIzquierda.dispose();
        }

    }
}
