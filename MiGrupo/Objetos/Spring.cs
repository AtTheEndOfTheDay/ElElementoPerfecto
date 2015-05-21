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
            var size = unMesh.BoundingBox.calculateSize();
            var baseDistance = new Vector3(0, -.5f * size.Y, 0);
            auxSpring = new Spring(unMesh, texture, uncolor)
            {
                _ExtendedScaleY = y,
                _ContractedScaleY = y * (1 - _CompressionRatio),
                _ContractionSpeed = y * _CompressionRatio * _BouncePerSecond,
                _BounceDistance = baseDistance * _CompressionRatio,
                _BaseDistance = baseDistance,
            };
            size = 1.1f * new Vector3(size.X, size.Y * .05f, size.Z);
            var box = TgcBox.fromSize(size, Color.FromArgb(127, 82, 23));
            auxSpring._Top = box.toMesh(unMesh.Name + "_Top");
            box.dispose();

            size = new Vector3(size.X, size.Y * 3f, size.Z);
            box = TgcBox.fromSize(size, Color.FromArgb(56, 56, 56));
            auxSpring._Base = box.toMesh(unMesh.Name + "_Base");
            box.dispose();

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

        #region Bounceing
        private const float _CompressionRatio = .3f;
        private const float _BouncePerSecond = 8f;
        private bool _IsBounceing = false;
        private float _ExtendedScaleY;
        private float _ContractedScaleY;
        private float _ContractionSpeed;
        private Vector3 _BounceStartPosition;
        private Vector3 _BounceDistance;
        private Vector3 _BounceSpeed;
        #endregion Bounceing
        private TgcMesh _Top;
        private TgcMesh _Base;
        private Vector3 _BaseDistance;

        public override void interactuarConPelota(Pelota pelota, float elapsedTime)
        {
            if (_IsBounceing)
            {
                var y = _ScaleY + _ContractionSpeed * elapsedTime;
                if (y > _ExtendedScaleY)
                {
                    _IsBounceing = false;
                    _ScaleY = _ExtendedScaleY;
                    mesh.Position = _BounceStartPosition;
                    rotate(Vector3.Empty);
                }
                else if (y < _ContractedScaleY)
                {
                    _ContractionSpeed *= -1;
                    _BounceSpeed *= -1;
                }
                else
                {
                    _ScaleY = y;
                    mesh.move(_BounceSpeed * elapsedTime);
                    _Top.move(_BounceSpeed * elapsedTime * 2f);
                }
            }
        }

        private float _ScaleY
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
            var rotation = _RotationMatrix;
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

        private Matrix _RotationMatrix
        {
            get { return Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z); ;}
        }

        public override Vector3 getLugarDelContenedor()
        {
            return lugarDelContenedor;
        }

        public override void move(Vector3 movement)
        {
            base.move(movement);
            _Top.move(movement);
            _Base.move(movement);
        }

        public override void rotate(Vector3 rotacion)
        {
            base.rotate(rotacion);
            var baseY = Vector3.TransformCoordinate(_BaseDistance, _RotationMatrix);
            _Top.Rotation += rotacion;
            _Top.Position = mesh.Position - baseY;
            _Base.Rotation += rotacion;
            _Base.Position = mesh.Position + baseY;
        }
        public override void render()
        {
            base.render();
            if (getenEscena())
            {
                _RenderBox(_Top);
                _RenderBox(_Base);
            }
        }
        private void _RenderBox(TgcMesh box)
        {
            box.Effect = GuiController.Instance.Shaders.TgcMeshPointLightShader;

            //Cargar variables shader de la luz
            box.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
            box.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(new Vector3(-13.1f, 10.5f, 10)));
            box.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
            box.Effect.SetValue("lightIntensity", 15);
            box.Effect.SetValue("lightAttenuation", 1);

            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            box.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            box.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            box.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            box.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            box.Effect.SetValue("materialSpecularExp", 10f);

            box.render();
        }
        public override void dispose()
        {
            base.dispose();
            _Top.dispose();
            _Base.dispose();
        }
    }
}
