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
    class Cannon : ItemUsuario
    {
        public float potencia = 0.6f;
        
        public Cannon(TgcMesh mesh, TgcTexture texture)
            : base(mesh, texture)
        {

        }
        public override float getCoefRebote()
        {
            return 0.5f;
        }

        public override Vector3 interactuarConPelota()
        {
            return new Vector3(-potencia, potencia, 0);
        }
        public override bool debeRebotar(TgcSphere esfera)
        {
            if (esfera.Position == mesh.Position)
                return true;
            else
                return false;
        }
        
    }
}
