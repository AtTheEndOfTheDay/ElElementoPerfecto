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
    class Cannon : Item
    {
        const float pi = (float)Math.PI;
        static Vector3 lugarDelContenedor = new Vector3(-15f, -8.5f, 1);

        public BaseCannon baseCannon;
        public float potencia = 0.6f;
        public bool cargado = false;


        public static Cannon CrearCannon(TgcMesh meshCannon, TgcMesh meshBase, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion)
        {
            return CrearCannon(meshCannon, meshBase, texture, uncolor, escalado, rotacion, GetLugarRelativoContenedor(meshCannon.Position));
        }

        public static Cannon CrearCannon(TgcMesh meshCannon, TgcMesh meshBase, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion, Vector3 movimiento)
        {
            Cannon auxCannon;
            meshCannon.Scale = escalado;

            auxCannon = new Cannon(meshCannon, meshBase, texture, uncolor, escalado);

            ((Item)auxCannon).rotate(rotacion);
            ((Item)auxCannon).move(movimiento);

            return auxCannon;
        }

        public Cannon(TgcMesh unMesh, TgcMesh meshBase, TgcTexture texture, Color uncolor, Vector3 escalado)
            : base(unMesh, texture)
        {
            mesh.setColor(uncolor);
            baseCannon = new BaseCannon(meshBase, escalado);
        }

        public override Vector3 interactuarConPelota()
        {
            return new Vector3(-potencia, potencia, 0);
        }

        public override bool debeRebotar(TgcSphere esfera)
        {
            if (cargado)
            {
                cargado = false;
                return false;
            }                
            else
                return true;
        }

        public override float getCoefRebote(Vector3 normal)
        {
            return 0.25f;
        }
        
        public static Vector3 GetLugarRelativoContenedor(Vector3 posicion)
        {
            return new Vector3(-15f - posicion.X, -8.5f - posicion.Y, 1 - posicion.Z);
        }

        public override Vector3 getLugarDelContenedor()
        {
            return lugarDelContenedor;
        }

        public override void move(Vector3 movement)
        {
            baseCannon.move(movement);
            base.move(movement);
        }

        public override void rotate(Vector3 rotacion)
        {
            Vector3 siguienteRotacion = mesh.Rotation + rotacion;
            if ((siguienteRotacion.Z < pi / 2) && (siguienteRotacion.Z > -pi / 2))
                base.rotate(rotacion);
        }

        public override void setenEscena(bool aparece)
        {
            baseCannon.setenEscena(aparece);
            base.setenEscena(aparece);
        }
    }
}
