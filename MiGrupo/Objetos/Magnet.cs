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
    class Magnet : Item
    {
        public static Magnet CrearMagnet(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion, Vector3 movimiento)
        {
            Magnet auxMagnet;

            unMesh.Scale = escalado;

            auxMagnet = new Magnet(unMesh, texture, uncolor);

            ((Item)auxMagnet).rotate(rotacion);
            ((Item)auxMagnet).move(movimiento);

            return auxMagnet;
        }

        public static Magnet CrearMagnet(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion)
        { 
            return CrearMagnet(unMesh, texture, uncolor, escalado, rotacion, GetLugarRelativoContenedor(unMesh.Position));
        }

        public Magnet(TgcMesh unMesh, TgcTexture texture, Color uncolor)
            : base(unMesh, texture)
        {
            mesh.setColor(uncolor);
        }

        /*
        public Magnet(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion, Vector3 movimiento)
            : base(unMesh, texture)
        {
            mesh.setColor(uncolor);
            mesh.move(movimiento);
            mesh.Scale = escalado;
            mesh.Rotation = rotacion;
        }

        public Magnet(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion)
            : base(unMesh, texture)
        {
            mesh.setColor(uncolor);
            llevarAContenedor();
            mesh.Scale = escalado;
            mesh.Rotation = rotacion;
        }
        */
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

        public static Vector3 GetLugarRelativoContenedor(Vector3 posicion)
        {
            return new Vector3(-15.75f - posicion.X, -7.5f - posicion.Y, 1 - posicion.Z);
        }
    }
}
