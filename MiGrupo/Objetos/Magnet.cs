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
        const float MIN_COS_ATRACTION = 0.7071f; //coseno de 45

        static Vector3 lugarDelContenedor = new Vector3(-15.75f, -7.5f, 1);

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

        public override void interactuarConPelota(Pelota pelota, float elapsedTime)
        {
            Vector3 distanceWithBall = pelota.esfera.Position - mesh.Position;
            Vector3 normalDistance = Vector3.Normalize(distanceWithBall);
            Vector3 directionMagnet = Vector3.TransformCoordinate(new Vector3(0,1,0),Matrix.RotationYawPitchRoll(mesh.Rotation.Y,mesh.Rotation.X,mesh.Rotation.Z));


            if(Vector3.Dot(normalDistance,directionMagnet)>MIN_COS_ATRACTION)
            {
                pelota.aumentarVelocidad(-normalDistance * (0.5f / distanceWithBall.LengthSq()) * elapsedTime * 100);
            }

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

        public override Vector3 getLugarDelContenedor()
        {
            return lugarDelContenedor;
        }
    }
}
