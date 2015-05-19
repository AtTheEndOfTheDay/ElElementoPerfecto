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
    class Spring : Item
    {
        public Spring(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion, Vector3 movimiento)
            : base(unMesh, texture)
        {
            //TODO: agregar tapas
            mesh.setColor(uncolor);
            mesh.move(movimiento);
            mesh.Scale = escalado;
            mesh.Rotation = rotacion;
        }

        public Spring(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion)
            : base(unMesh, texture)
        {
            mesh.setColor(uncolor);
            llevarAContenedor();
            mesh.Scale = escalado;
            mesh.Rotation = rotacion;
        }


        public override Vector3 interactuarConPelota()
        {
            return new Vector3 (0, 0, 0);
            //TODO
        }

        public override bool debeRebotar(TgcSphere esfera)
        {
            return true;
            //TODO
        }

        public override float getCoefRebote(Vector3 normal)
        {
            return 0.5f;
        }

        public override void llevarAContenedor()
        {
            mesh.move(new Vector3(-16f - mesh.Position.X, -8f - mesh.Position.Y, 1 - mesh.Position.Z));
        }
    }
}
