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
            return CrearSpring(unMesh, texture, uncolor, escalado, rotacion, lugarDelContenedor - unMesh.Position);
        }

        public static Spring CrearSpring(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion, Vector3 movimiento)
        {

            Spring auxSpring;

            unMesh.Scale = escalado;

            var y = escalado.Y;
            var h = unMesh.BoundingBox.calculateSize().Y;
            auxSpring = new Spring(unMesh, texture, uncolor)
            {
                _ExtendedScaleY = y,
                _ContractedScaleY = y * (1 - _CompressionRatio),
                _ContractionSpeed = y * _CompressionRatio * _BouncePerSecond,
                _BounceDistance = new Vector3(0, -.5f * h * _CompressionRatio, 0),
            };

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

        private const float _CompressionRatio = .3f;
        private const float _BouncePerSecond = 8f;
        private bool _IsBounceing = false;
        private float _ExtendedScaleY;
        private float _ContractedScaleY;
        private float _ContractionSpeed;
        private Vector3 _BounceStartPosition;
        private Vector3 _BounceDistance;
        private Vector3 _BounceSpeed;
        public override void interactuarConPelota(Pelota pelota, float elapsedTime)
        {
            if (_IsBounceing)
            {
                var y = ScaleY + _ContractionSpeed * elapsedTime;
                if (y > _ExtendedScaleY)
                {
                    _IsBounceing = false;
                    ScaleY = _ExtendedScaleY;
                    mesh.Position = _BounceStartPosition;
                }
                else if (y < _ContractedScaleY)
                {
                    _ContractionSpeed *= -1;
                    _BounceSpeed *= -1;
                }
                else
                {
                    ScaleY = y;
                    mesh.move(_BounceSpeed * elapsedTime);
                }
            }
            //TODO
        }

        private float ScaleY
        {
            get { return mesh.Scale.Y; }
            set
            {
                var s = mesh.Scale;
                mesh.Scale = new Vector3(s.X, value, s.Z);
            }
        }

        public override bool debeRebotar(TgcSphere esfera)
        {
            return true;
            //TODO
        }

        private Vector3 _VersorY = new Vector3(0, 1, 0);
        public override float getCoefRebote(Vector3 normal)
        {
            var rotation = Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z);
            var up = Vector3.TransformCoordinate(_VersorY, rotation);
            _IsBounceing = .999999f < Vector3.Dot(normal, up);
            if (_IsBounceing)
            {
                _BounceStartPosition = mesh.Position;
                //ScaleY = _ContractedScaleY;
                _ContractionSpeed *= -1;
                _BounceSpeed = Vector3.TransformCoordinate(_BounceDistance * _BouncePerSecond, rotation);
                return 1.5f;
            }
            return .5f;
        }

        public override Vector3 getLugarDelContenedor()
        {
            return lugarDelContenedor;
        }
    }
}
