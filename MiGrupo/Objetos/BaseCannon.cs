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
    class BaseCannon : Item
    {
        static Vector3 lugarDelContenedor = new Vector3(-15.75f, -7.5f, 1);

        public BaseCannon(TgcMesh unMesh, Vector3 escalado):base()
        {
            mesh = unMesh;
            mesh.Scale = new Vector3(escalado.X, escalado.Y / 3, escalado.Z);
            mesh.setColor(Color.FromArgb(54, 22, 3));
            mesh.rotateY((float)Math.PI / 2);
            orientedBB = TgcObb.computeFromAABB(mesh.BoundingBox);
            orientedBB.setRenderColor(Color.Red);
            orientedBB.move(new Vector3(0, -1.3f, 0));
            mesh.Scale = escalado;
        }

        /*
        public override void interactuarConPelota(Pelota pelota, float elapsedTime)
        {
            return new Vector3(0, 0, 0);
            //TODO
        }
        */

        public override bool debeRebotar(TgcSphere esfera)
        {
            return true;
            //TODO
        }

        public override float getCoefRebote(Vector3 normal)
        {
            return 0.5f;
        }
        public override Vector3 getLugarDelContenedor()
        {
            return lugarDelContenedor;
        }
    }
}
