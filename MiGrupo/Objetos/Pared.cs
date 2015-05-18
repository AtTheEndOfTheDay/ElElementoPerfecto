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

        public Pared(TgcMesh mesh, TgcTexture texture):base(mesh,texture)
        {
            enEscena = true;
        }

        public override Vector3 interactuarConPelota()
        {
            return new Vector3(0, 0, 0);
        }
        
        public override bool debeRebotar(TgcSphere esfera)
        {
            return true;
        }
    }
}
