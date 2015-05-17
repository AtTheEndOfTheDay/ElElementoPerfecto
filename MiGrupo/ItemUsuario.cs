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

namespace AlumnoEjemplos.MiGrupo
{
    class ItemUsuario : Item
    {
        public bool enEscena = false;
        public TgcMesh mesh;
        TgcTexture textura;
        public ItemUsuario(TgcMesh unMesh, TgcTexture texture)
        {
            mesh = unMesh;
            textura = texture;
        }

        void Item.interactuar(TgcD3dInput input, float elapsedTime)
        {
            //No Implementado
        }

        void Item.interactuarConPelota(TgcD3dInput input, float elapsedTime, Pelota pelota)
        {
            //No Implementado
        }

        void Item.render()
        {
            render();
        }

        bool Item.esMovil()
        {
            return true;
        }

        void Item.aplicarMovimientos(float elapsedTime)
        {
            //No Implementado
        }

        Vector3 Item.velocidad()
        { 
            return new Vector3(0, 0, 0); 
        }
        void Item.dispose()
        {
            mesh.dispose();
        }

        public TgcTexture getTexture()
        {
            return textura;
        }

        internal void render()
        {
            if (enEscena)
            {
                mesh.render();
            }     
        }
    }
}
