using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{

    class Construccion : Etapa
    {
        private List<Item> itemsInScenario = new List<Item>();
        private Pelota pelota;
        Vector2 mouseVector;
        Vector2 anteriorMouse;
        List<Item> objetos;
        Item objetoAMover;

        TgcPickingRay pickingRay = new TgcPickingRay();
        bool selected = false;
        bool agarrado = false;
        Vector3 collisionPoint;
        MenuObjetos menu;

        public Construccion(List<Item> unosItems,Pelota unaPelota, MenuObjetos menuActual, List<Item> objetosDelUsuario)
        {
            itemsInScenario = unosItems;
            pelota = unaPelota;
            menu = menuActual;
            objetos = objetosDelUsuario;
        }

        void Etapa.interaccion(TgcD3dInput input, float elapsedTime)
        {
        
            //aqui iría lo de arrastrar los objetos de la barra a la pantalla
            //poder mover la lista en el menu de elementos, etc

        Vector3 movimiento2 = new Vector3(0, 0, 0);

            mouseVector = new Vector2(GuiController.Instance.D3dInput.Xpos, GuiController.Instance.D3dInput.Ypos);
         if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
             //Actualizar Ray de colisión en base a posición del mouse
             pickingRay.updateRay();
             
             menu.poneloEnPantallaSiToco(mouseVector.X, mouseVector.Y);
            } 

            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();
                foreach (Item objeto in objetos)
                {
                    TgcBoundingBox aabb = objeto.mesh.BoundingBox;
                    selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);
                  
                    if (selected)
                    {
                        objetoAMover = objeto;
                        objetoAMover.enEscena = false;
                    }
                }           
            }

            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();
               //Testear Ray contra el AABB de todos los meshes
                foreach (Item objeto in objetos)
                {
                    TgcBoundingBox aabb = objeto.mesh.BoundingBox;
                    selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);
                  
                    if (selected)
                    {
                        objetoAMover = objeto;
                        agarrado = true;
                    }
                }

                if (agarrado)
                {
                   movimiento2.X = -(mouseVector.X - anteriorMouse.X);
                   movimiento2.Y = -(mouseVector.Y - anteriorMouse.Y);

                }
            }

            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                agarrado = false;
            }
            if (agarrado && objetoAMover.enEscena)
            {
               objetoAMover.mesh.move(movimiento2 * 0.032f);
            }
            anteriorMouse = mouseVector;

        }

        void Etapa.aplicarMovimientos(float elapsedTime)
        {
            //supongo que nada se mueve por si solo cuando construyo, sino debería ir aquí
        }

        void Etapa.render()
        {
            pelota.render();

            foreach (var item in itemsInScenario)
            {
                item.iluminar();
            }
            
            foreach (var item in itemsInScenario)
            {
                item.render();
            }
        }

        String Etapa.getNombre()
        {
            return "Construccion";
        }
    }
}
