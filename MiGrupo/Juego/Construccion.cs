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
        List<Item> itemsDelUsuario = new List<Item>();
        Pelota pelota;
        MenuObjetos menu;

        Vector2 mouseVector;
        Vector2 anteriorMouse;
        Vector3 movimientoObjeto;
        Item objetoPickeado;
        Item objetoAnterior;
        TgcPickingRay pickingRay = new TgcPickingRay();
        bool selected = false;
        bool agarrado = false;
        Vector3 collisionPoint;

        public Construccion(List<Item> unosItemsDelUsuario, Pelota unaPelota, MenuObjetos menuActual)
        {
            pelota = unaPelota;
            menu = menuActual;
            itemsDelUsuario = unosItemsDelUsuario;
        }

        void Etapa.interaccion(TgcD3dInput input, float elapsedTime)
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
                if (input.keyDown(Key.RightArrow))
                {
                    objetoPickeado.rotate(new Vector3(0, 0, 3 * elapsedTime));
                }
                if (input.keyDown(Key.LeftArrow))
                {
                    objetoPickeado.rotate(new Vector3(0, 0, -3 * elapsedTime));
                }
            }
            
        }
        String Etapa.getNombre()
        {
            return "Construccion";
        }
    }
}
