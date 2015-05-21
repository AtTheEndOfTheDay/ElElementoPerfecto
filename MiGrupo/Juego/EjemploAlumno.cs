
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
        
        Item itemSelected = null;
        TgcPickingRay pickingRay = new TgcPickingRay();
        Vector3 distanceBetweenPickAndCenter;
        bool hitItem = false;
        bool dragging = false;
        bool firstPickInItem = true;
        bool itemInRestrictedArea = false;


        TgcText2d textStage = new TgcText2d();

        public static bool mostrarOBBs = false;
        public static bool mostrarBBs = false;

        TgcBox objetoGanador;
        TgcSprite cartelGanador;
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

            fondo = TgcBox.fromSize(new Vector3(3, 0.2f, -2), new Vector3(40, 30, 0));

            Size screenSize = GuiController.Instance.Panel3d.Size;
            if ((screenSize.Width * 9 / screenSize.Height * 16) == 1) 
                contenedor = TgcBox.fromSize(new Vector3(-16.05f, -8.25f, 0), new Vector3(5.7f, 4.6f, 0), TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\Laja.jpg"));
            else
                contenedor = TgcBox.fromSize(new Vector3(-16.65f, -8.25f, 0), new Vector3(6f, 4.6f, 0), TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\Laja.jpg"));
            
            setNivel();
           
            GuiController.Instance.Modifiers.addBoolean("Mostrar OBBs", "Mostrar OBBs", false);
            GuiController.Instance.Modifiers.addBoolean("Mostrar BBs", "Mostrar BBs", false);
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
            else if (input.keyDown(Key.F3))
            {
                numeroDeNivel = 3;
                setNivel();
            }
            else if (input.keyDown(Key.F4))
            {
                numeroDeNivel = 4;
                setNivel();
            }
            else if (input.keyDown(Key.F5))
            {
                numeroDeNivel = 5;
                setNivel();
            }
            else if ((TgcCollisionUtils.testSphereAABB(pelota.esfera.BoundingSphere, objetoGanador.BoundingBox)))
            {
                etapa = ganador;
            }
            else if (input.keyDown(Key.Space))
            {
                etapa = play;
                textStage.Text = "Play - Presione C para volver a la etapa de construccion";
            }
            else if (input.keyDown(Key.C))
            {
                fabricaDeNiveles.reiniciar(numeroDeNivel, pelota);
                etapa = construccion;
                textStage.Text = "Construccion - Presione espacio para empezar";
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
            objetoGanador.render();

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

        private void setNivel()
        {
            fabricaDeNiveles.iniciarNivel(numeroDeNivel, out cartelGanador, out objetoGanador, out pelota, itemsDeNivel, itemsDelUsuario, out menu, fondo);

            etapa = construccion;
            textStage.Color = Color.White;
            textStage.Position = new Point(0, 0);
            textStage.Text = "Construccion - Presione espacio para empezar";

            itemSelected = null;

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
            evaluoClickEnMenu(input);

            borrarSiClickea(input);

            pickDragAndDrop(input);

            if (itemSelected != null)
            {
                rotateItemSelected(input, elapsedTime);
                
                itemSelected.getOBB().render();
            }

        }

            private void rotateItemSelected(TgcD3dInput input, float elapsedTime)
            {
                if (input.keyDown(Key.D))
                {
                    itemSelected.rotate(new Vector3(0, 0, 3 * elapsedTime));
                }
                if (input.keyDown(Key.A))
                {
                    itemSelected.rotate(new Vector3(0, 0, -3 * elapsedTime));
                }
            }

            private void evaluoClickEnMenu(TgcD3dInput input)
            {
                Vector2 mouseVector = new Vector2(GuiController.Instance.D3dInput.Xpos, GuiController.Instance.D3dInput.Ypos);

                if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
                {
                    pickingRay.updateRay();
                    menu.poneloEnPantallaSiToco(mouseVector.X, mouseVector.Y);
                }
            }

            private void borrarSiClickea(TgcD3dInput input)
            {
                Vector3 collisionPoint;

                if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
                {
                    pickingRay.updateRay();
                    foreach (Item item in itemsDelUsuario)
                    {
                        if (item.getenEscena())
                        {
                            hitItem = TgcCollisionUtils.intersectRayObb(pickingRay.Ray, item.getOBB(), out collisionPoint);

                            if (hitItem)
                            {
                                //Lo quito de mi mundo 3D
                                item.setenEscena(false);
                                itemSelected = null;
                                item.llevarAContenedor();
                            }
                        }

                    }
                }
            }

            private void pickDragAndDrop(TgcD3dInput input)
            {
                float instanteT;
                Vector3 pickPoint = new Vector3(0, 0, 0);
                Vector3 collisionPoint;
                                
                if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                {
                    pickingRay.updateRay();

                    foreach (Item item in itemsDelUsuario)
                    {
                        if (item.getenEscena())
                        {
                            hitItem = TgcCollisionUtils.intersectRayObb(pickingRay.Ray, item.getOBB(), out collisionPoint);

                            if ((hitItem) && (!dragging))
                            {
                                dragging = true;
                                if (item != itemSelected)
                                {
                                    itemSelected = item;
                                }
                                firstPickInItem = true;
                            }
                        }
                    }

                    if (dragging)
                    {
                        dragging = TgcCollisionUtils.intersectRayPlane(pickingRay.Ray, new Plane(0, 0, 1, -itemSelected.getPosition().Z), out instanteT, out pickPoint);
                    }

                    if (dragging)
                    {
                        Vector3 puntoAnterior;

                        if (firstPickInItem)
                        {
                            distanceBetweenPickAndCenter = pickPoint - itemSelected.getPosition();
                            firstPickInItem = false;
                        }

                        puntoAnterior = itemSelected.getPosition();

                        itemSelected.setPosition(pickPoint - distanceBetweenPickAndCenter);



                        if ((itemSelected.getPosition().X < -12.42f) || (itemSelected.getPosition().X > 17.9f) || (itemSelected.getPosition().Y < -9.9f) || (itemSelected.getPosition().Y > 9.9f))
                        {
                            itemInRestrictedArea = true;
                        }
                        else
                        {
                            itemInRestrictedArea = false;

                            foreach (Item item in items)
                            {
                                if ((item != itemSelected) && (item.getenEscena()))
                                {
                                    if (TgcCollisionUtils.testObbObb(item.getOBB(), itemSelected.getOBB()))
                                    {
                                        itemInRestrictedArea = true;
                                    }
                                }
                            }
                        }

                        if (itemInRestrictedArea)
                        {
                            itemSelected.getOBB().setRenderColor(Color.Red);
                        }
                        else
                        {
                            itemSelected.getOBB().setRenderColor(Color.Blue);
                        }
                    }
                }

                if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                {
                    dragging = false;

                    if (itemInRestrictedArea)
                    {
                        itemSelected.llevarAContenedor();
                        itemInRestrictedArea = false;

                        itemSelected.getOBB().setRenderColor(Color.Blue);
                    }
                        
                }

            }

        void play(TgcD3dInput input, float elapsedTime)
        {
            pelota.interactuar(input, elapsedTime);

            foreach (Item item in items)
            {
                item.interactuarConPelota(pelota,elapsedTime);
            }

            pelota.aplicarMovimientos(elapsedTime, items);
        }

        void ganador(TgcD3dInput input, float elapsedTime)
        {

            GuiController.Instance.Drawer2D.beginDrawSprite();
            cartelGanador.render();
            GuiController.Instance.Drawer2D.endDrawSprite();

            textStage.Text = "Yey!!! \\O/";

            if (input.keyDown(Key.R))
            {
                fabricaDeNiveles.reiniciar(numeroDeNivel, pelota);
                etapa = construccion;
                textStage.Text = "Construccion - Presione espacio para empezar";
            }
            if (input.keyDown(Key.Return))
            {
                if (numeroDeNivel == 5)
                    numeroDeNivel = 0;
                numeroDeNivel++;
                setNivel();
            }
        }

    }
}