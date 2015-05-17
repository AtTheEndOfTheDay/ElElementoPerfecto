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

    class Construccion : Stage
    {
        private List<Item> itemsInScenario = new List<Item>();
        private Pelota pelota;
        private TgcBox caja;
         Vector2 mouseVector;
         Vector2 anteriorMouse;
        TgcPickingRay pickingRay = new TgcPickingRay();
        bool selected = false;
        bool aparece = false;
        Vector3 collisionPoint;
        TgcBox selectedMesh;
        TgcBox elemCreado = TgcBox.fromSize(new Vector3(10, 5, 0), new Vector3(1.5f, 1.5f, 0));

        public Construccion(List<Item> unosItems,Pelota unaPelota, TgcBox unacaja)
        {
            itemsInScenario = unosItems;
            pelota = unaPelota;
            caja = unacaja;
        }

        void Stage.interaccion(TgcD3dInput input, float elapsedTime)
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
               selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);

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
              TgcBoundingBox aabb = elemCreado.BoundingBox;

                //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint
               selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);

                if (selected)
                {
                    selectedMesh = elemCreado;
                   movimiento2.X = -(mouseVector.X - anteriorMouse.X);
                   movimiento2.Y = -(mouseVector.Y - anteriorMouse.Y);
                }
            }

            if (selected && aparece)
            {
                selectedMesh.BoundingBox.render();
               // collisionPointMesh.Position = collisionPoint;
             //   collisionPointMesh.render();
            }

            if (aparece)
            {
               elemCreado.render();
            }

            anteriorMouse = mouseVector;
            elemCreado.move(movimiento2 * 0.0345f);
        }

        void Stage.aplicarMovimientos(float elapsedTime)
        {
            //supongo que nada se mueve por si solo cuando construyo, sino debería ir aquí
        }

        void Stage.render()
        {
            pelota.render();
            caja.render();
            
            foreach (var item in itemsInScenario)
            {
                item.render();
            }
        }

        String Stage.getNombre()
        {
            return "Construccion";
        }
    }
}
