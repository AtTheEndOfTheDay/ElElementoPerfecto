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
        Vector3 movimientoObjeto;
        List<Item> objetos;
        Item objetoPickeado;
        Item objetoAnterior;
        bool primeraVez = true;

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
            
            movimientoObjeto = new Vector3(0, 0, 0);
            mouseVector = new Vector2(GuiController.Instance.D3dInput.Xpos, GuiController.Instance.D3dInput.Ypos);

            //Evalua si clickea un sprite del menu para que el objeto aparezca en el contenedor
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                pickingRay.updateRay();
                menu.poneloEnPantallaSiToco(mouseVector.X, mouseVector.Y);
            } 
            
            //Evalua si toco el boton derecho para ver si borra
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                pickingRay.updateRay();
                foreach (Item objeto in objetos)
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
                
             //Evalua si debe arrastrar el objeto y lo arrastra
             if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
             {
                 pickingRay.updateRay();
             
                 foreach (Item objeto in objetos)
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
                objetoPickeado.recibiOrdenDelUsuario(input);
            }

            anteriorMouse = mouseVector;
            objetoAnterior = objetoPickeado;
        }

        void Etapa.aplicarMovimientos(float elapsedTime)
        {
           
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
