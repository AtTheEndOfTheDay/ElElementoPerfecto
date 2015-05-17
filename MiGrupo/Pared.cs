using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AlumnoEjemplos.MiGrupo
{
    class Pared : Item
    {
        public TgcBox caja;
        TgcObb orientedBB;
        int caraRebote;

        public Pared(TgcBox unaCaja,int unaCara)
        {
            caja = unaCaja;
            orientedBB = TgcObb.computeFromAABB(caja.BoundingBox);
            caraRebote = unaCara;
        }

        void Item.interactuar(TgcD3dInput input,float elapsedTime)
        {}

        void Item.interactuarConPelota(TgcD3dInput input, float elapsedTime,Pelota pelota)
        {
            if (TgcCollisionUtils.testSphereOBB(pelota.esfera.BoundingSphere, orientedBB))
            {
                pelota.rebotar((Item)this, 0.5f, orientedBB.Orientation[caraRebote]);
                
               
            }
        }

        void Item.aplicarMovimientos(float elapsedTime)
        {
            //las paredes no se mueven
        }

        void Item.render()
        {
            caja.render();
        }

        bool Item.esMovil()
        { return false; }

        Vector3 Item.velocidad()
        { return new Vector3(0,0,0); }

        public void dispose()
        {
            caja.dispose();
        }
        public TgcTexture getTexture()
        {
            return caja.Texture;
        }
    }
}
