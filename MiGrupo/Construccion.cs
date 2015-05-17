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
        private TgcBox caja;
         Vector2 mouseVector;
         Vector2 anteriorMouse;
         List<ItemUsuario> objetos;
         ItemUsuario objetoAMover;

        TgcPickingRay pickingRay = new TgcPickingRay();
        bool selected = false;
        bool aparece = false;
        bool agarrado = false;
        Vector3 collisionPoint;
        TgcBox selectedMesh;
        TgcBox elemCreado = TgcBox.fromSize(new Vector3(10, 5, 0), new Vector3(1.5f, 1.5f, 0));
        MenuObjetos menu;

        public Construccion(List<Item> unosItems,Pelota unaPelota, TgcBox unacaja, MenuObjetos menuActual, List<ItemUsuario> objetosDelUsuario)
        {
            itemsInScenario = unosItems;
            pelota = unaPelota;
            caja = unacaja;
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

                //Testear Ray contra el AABB de todos los meshes
            TgcBoundingBox aabb = caja.BoundingBox;

                //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint
             //  selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);

               if (menu.estaEnMenu(mouseVector.X, mouseVector.Y))
               {
                   selected = true; 
               }

                if (selected)
                {
                aparece = true;
                selectedMesh = caja;
                }
            } 

            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();
                //Testear Ray contra el AABB de todos los meshes
            TgcBoundingBox aabb = elemCreado.BoundingBox;

                //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint
            selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);

                if (selected)
                {
                    aparece = false;
                    selected = false;
             
                }
                
            }

            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();
               //Testear Ray contra el AABB de todos los meshes
                foreach (ItemUsuario objeto in objetos)
                {
                    TgcBoundingBox aabb = objeto.mesh.BoundingBox;
                    selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);
                    objetoAMover = objeto;
                    if (selected)
                    {
                        agarrado = true;
                    }
                }
                //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint
             //  selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);

                if (agarrado)
                {
                    selectedMesh = elemCreado;
                   movimiento2.X = -(mouseVector.X - anteriorMouse.X);
                   movimiento2.Y = -(mouseVector.Y - anteriorMouse.Y);
                   //agarrado = true;

                }
            }

            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                agarrado = false;
            }
            if (agarrado)
            {
                selectedMesh.BoundingBox.render();
               objetoAMover.mesh.move(movimiento2 * 0.032f);
               // collisionPointMesh.Position = collisionPoint;
             //   collisionPointMesh.render();
            }

            if (aparece)
            {
               elemCreado.render();
            }

            anteriorMouse = mouseVector;

           // objetoAMover.mesh.move(movimiento2 * 0.345f);
        }

        void Etapa.aplicarMovimientos(float elapsedTime)
        {
            //supongo que nada se mueve por si solo cuando construyo, sino debería ir aquí
        }

        void Etapa.render()
        {
            pelota.render();
            caja.render();
            
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
