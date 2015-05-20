
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
        List<Item> items = new List<Item>();
        List<Item> itemsDelUsuario = new List<Item>();
        List<Item> itemsDeNivel = new List<Item>();

        Action<TgcD3dInput, float> etapa;
        Vector2 mouseVector;
        Vector2 anteriorMouse;
        Vector3 movimientoObjeto;
        Item objetoPickeado;
        Item objetoAnterior;
        TgcPickingRay pickingRay = new TgcPickingRay();
        bool selected = false;
        bool agarrado = false;
        Vector3 collisionPoint;

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

            fondo = TgcBox.fromSize(new Vector3(3, 0.2f, -2), new Vector3(33.5f, 23, 0));
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

            etapa = construccion;
            textStage.Color = Color.White;
            textStage.Position = new Point(0, 0);
            textStage.Text = "Construccion";

            items = new List<Item>();
            foreach (Item item in itemsDeNivel)
                items.Add(item);
            foreach (Item item in itemsDelUsuario)
                items.Add(item);

            foreach (Item objeto in items)
            {
                objeto.iluminar();
            }
        }

        void construccion(TgcD3dInput input, float elapsedTime)
        {
            movimientoObjeto = new Vector3(0, 0, 0);
            mouseVector = new Vector2(GuiController.Instance.D3dInput.Xpos, GuiController.Instance.D3dInput.Ypos);

            evaluoClickEnMenu();

            borrarSiClickea();

            pickDragAndDrop(input, elapsedTime);
             
            anteriorMouse = mouseVector;
            objetoAnterior = objetoPickeado;
        }

        private void evaluoClickEnMenu()
        {
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                pickingRay.updateRay();
                menu.poneloEnPantallaSiToco(mouseVector.X, mouseVector.Y);
            } 
        }

        private void borrarSiClickea()
        {
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                pickingRay.updateRay();
                foreach (Item objeto in itemsDelUsuario)
                {
                    TgcBoundingBox aabb = objeto.mesh.BoundingBox;
                    selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);

                    if (selected)
                    {
                        objeto.enEscena = false;
                        objeto.pickeado = false;
                        objeto.llevarAContenedor();
                    }
                }
            }
        }

        private void pickDragAndDrop(TgcD3dInput input, float elapsedTime)
        {
            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                pickingRay.updateRay();

                foreach (Item objeto in itemsDelUsuario)
                {
                    TgcBoundingBox aabb = objeto.mesh.BoundingBox;
                    selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);

                    if (selected && objeto.enEscena)
                    {
                        objetoPickeado = objeto;

                        if (objetoAnterior != null)
                            objetoAnterior.pickeado = false;
                        objetoPickeado.pickeado = true;
                        agarrado = true;

                    }
                }

                if (agarrado)
                {
                    movimientoObjeto.X = -(mouseVector.X - anteriorMouse.X);
                    movimientoObjeto.Y = -(mouseVector.Y - anteriorMouse.Y);

                }
            }

            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                agarrado = false;
            }

            if (agarrado && objetoPickeado.enEscena)
            {
                objetoPickeado.move(movimientoObjeto * 0.032f);
            }

            if (objetoPickeado != null && objetoPickeado.pickeado)
            {
                objetoPickeado.getOBB().render();
                if (input.keyDown(Key.D))
                {
                    objetoPickeado.rotate(new Vector3(0, 0, 3 * elapsedTime));
                }
                if (input.keyDown(Key.A))
                {
                    objetoPickeado.rotate(new Vector3(0, 0, -3 * elapsedTime));
                }
            }
            
        }

        void play(TgcD3dInput input, float elapsedTime)
        {
            pelota.interactuar(input, elapsedTime);

            pelota.aplicarMovimientos(elapsedTime, items);
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
                textStage.Text = "Play";
            }
            else if (input.keyDown(Key.C))
            {
                fabricaDeNiveles.reiniciar(numeroDeNivel, pelota);
                etapa = construccion;
                textStage.Text = "Construccion";
            }

            etapa(input, elapsedTime);

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
