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
        static Vector3 lugarDelContenedor = new Vector3(-16f, -8f, 1);

        public static Spring CrearSpring(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion)
        {
            return CrearSpring(unMesh, texture, uncolor, escalado, rotacion, lugarDelContenedor- unMesh.Position);
        }

        public static Spring CrearSpring(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion, Vector3 movimiento)
        {

            Spring auxSpring;

            unMesh.Scale = escalado;

            auxSpring = new Spring(unMesh, texture, uncolor);

            ((Item)auxSpring).rotate(rotacion);
            ((Item)auxSpring).move(movimiento);

            return auxSpring;
        }

        public Spring(TgcMesh unMesh, TgcTexture texture, Color uncolor)
            : base(unMesh, texture)
        {
            //TODO: agregar tapas
            mesh.setColor(uncolor);
        }

        /*
        public override Vector3 interactuarConPelota(Pelota pelota, float elapsedTime)
        {
            return new Vector3 (0, 0, 0);
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
