using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Cannon : Item
    {
        #region Constants
        private const Single _RotationSpeed = .7f;
        private const Single _BorderScale = .2f;
        private const Single _BodyObbScaleY = .8f;
        private const Single _BaseObbScaleY = .3f;
        private static readonly Vector3 _ObbExtents = Vector3Extension.One * 4f;
        private static readonly Vector3 _BaseObbTranslation = Vector3Extension.Bottom * 15f;
        private static readonly Vector3 _MinBorderScale = Vector3Extension.One * _BorderScale;
        private static readonly Vector3 _MaxBorderScale = new Vector3(1f, 0f, 1f) * _BorderScale;
        #endregion Constants

        #region Constructors
        #region DefaultForceConstructors
        public Cannon(Game game)
            : this(game, 1f, Vector3.Empty, Vector3.Empty, Vector3.Empty, Vector3.Empty, 1f) { }
        public Cannon(Game game, Vector3 position)
            : this(game, 1f, position, Vector3.Empty, Vector3.Empty, Vector3.Empty, 1f) { }
        public Cannon(Game game, Vector3 position, Vector3 rotation)
            : this(game, 1f, position, rotation, Vector3.Empty, Vector3.Empty, 1f) { }
        public Cannon(Game game, Vector3 position, Vector3 rotation, Vector3 rotationA)
            : this(game, 1f, position, rotation, rotationA, Vector3.Empty, 1f) { }
        public Cannon(Game game, Vector3 position, Vector3 rotation, Vector3 rotationA, Vector3 rotationB)
            : this(game, 1f, position, rotation, rotationA, rotationB, 1f) { }

        public Cannon(Game game, Vector3 position, Single scale)
            : this(game, 1f, position, Vector3.Empty, Vector3.Empty, Vector3.Empty, scale) { }
        public Cannon(Game game, Vector3 position, Vector3 rotation, Single scale)
            : this(game, 1f, position, rotation, Vector3.Empty, Vector3.Empty, scale) { }
        public Cannon(Game game, Vector3 position, Vector3 rotation, Vector3 rotationA, Single scale)
            : this(game, 1f, position, rotation, rotationA, Vector3.Empty, scale) { }
        public Cannon(Game game, Vector3 position, Vector3 rotation, Vector3 rotationA, Vector3 rotationB, Single scale)
            : this(game, 1f, position, rotation, rotationA, rotationB, scale) { }
        #endregion DefaultForceConstructors

        #region DefaultConstructors
        public Cannon(Game game, Single force)
            : this(game, force, Vector3.Empty, Vector3.Empty, Vector3.Empty, Vector3.Empty, 1f) { }
        public Cannon(Game game, Single force, Vector3 position)
            : this(game, force, position, Vector3.Empty, Vector3.Empty, Vector3.Empty, 1f) { }
        public Cannon(Game game, Single force, Vector3 position, Vector3 rotation)
            : this(game, force, position, rotation, Vector3.Empty, Vector3.Empty, 1f) { }
        public Cannon(Game game, Single force, Vector3 position, Vector3 rotation, Vector3 rotationA)
            : this(game, force, position, rotation, rotationA, Vector3.Empty, 1f) { }
        public Cannon(Game game, Single force, Vector3 position, Vector3 rotation, Vector3 rotationA, Vector3 rotationB)
            : this(game, force, position, rotation, rotationA, rotationB, 1f) { }

        public Cannon(Game game, Single force, Single scale)
            : this(game, force, Vector3.Empty, Vector3.Empty, Vector3.Empty, Vector3.Empty, scale) { }
        public Cannon(Game game, Single force, Vector3 position, Single scale)
            : this(game, force, position, Vector3.Empty, Vector3.Empty, Vector3.Empty, scale) { }
        public Cannon(Game game, Single force, Vector3 position, Vector3 rotation, Single scale)
            : this(game, force, position, rotation, Vector3.Empty, Vector3.Empty, scale) { }
        public Cannon(Game game, Single force, Vector3 position, Vector3 rotation, Vector3 rotationA, Single scale)
            : this(game, force, position, rotation, rotationA, Vector3.Empty, scale) { }
        public Cannon(Game game, Single force, Vector3 position, Vector3 rotation, Vector3 rotationA, Vector3 rotationB, Single scale)
        {
            var bodyMesh = game.GetMesh("Cannon");
            var baseMesh = game.GetMesh("CannonBase");
            _ObbLoad = new TgcObb() { Extents = _ObbExtents };
            _ObbLoad.SetOrientation();
            Add(new ObbPart(_ObbLoad));
            Add(new MeshStaticPart(bodyMesh));
            var bodyObb = TgcObb.computeFromAABB(bodyMesh.BoundingBox);
            var bodyE = bodyObb.Extents;
            bodyObb.Extents = new Vector3(bodyE.X * _BodyObbScaleY, bodyE.Y, bodyE.Z * _BodyObbScaleY);
            Add(new HollowObbCollider(bodyObb, bodyObb.Position - bodyMesh.Position, _MinBorderScale, _MaxBorderScale));
            var baseObb = TgcObb.computeFromAABB(baseMesh.BoundingBox);
            var baseE = baseObb.Extents;
            baseObb.Extents = new Vector3(baseE.X, baseE.Y * _BaseObbScaleY, baseE.Z);
            Add(_Base = new MeshUnRotatedPart(baseMesh));
            Add(_BaseCollider = new ObbTranslatedUnRotatedCollider(baseObb, _BaseObbTranslation));
            Scale = scale * Vector3Extension.One;
            _Base.Rotation = Rotation = rotation;
            _BaseCollider.Obb.SetOrientation(rotation);
            Position = position;
            var minZ = rotation.Z - FastMath.PI_HALF;
            var maxZ = rotation.Z + FastMath.PI_HALF;
            _RotationA = rotationA.ClampZ(minZ, maxZ);
            _RotationB = rotationB.ClampZ(minZ, maxZ);
            _Force *= force;
        }
        #endregion DefaultConstructors
        #endregion Constructors

        #region ResetMethods
        private Interactive _Load = null;
        private Boolean _IsRotationA = true;
        private Boolean _IsBaseRotating = false;
        private Vector3 _BaseRotationSaved;
        public override void SaveValues()
        {
            base.SaveValues();
            _BaseRotationSaved = _Base.Rotation;
        }
        public override void LoadValues()
        {
            base.LoadValues();
            _Base.Rotation = _BaseRotationSaved;
            _IsBaseRotating = false;
            _Load = null;
        }
        private static readonly Vector3 _MenuScale = Vector3Extension.One * .3f;
        public override void MenuTransform(Vector3 rotation, Vector3 position)
        {
            Scale = _MenuScale;
            Position = position;
            Rotation = _Base.Rotation = rotation;
        }
        #endregion ResetMethods

        #region ItemMethods
        public override void Build(Single deltaTime)
        {
            var input = GuiController.Instance.D3dInput;
            if (input.keyUp(Key.LeftControl))
                _IsBaseRotating = !_IsBaseRotating;
            if (input.keyUp(Key.Tab))
            {
                Rotation = _IsRotationA ? _RotationB : _RotationA;
                _IsRotationA = !_IsRotationA;
            }
            if (input.keyDown(Key.D))
                _BuildRotation(deltaTime, _Base.Rotation.Z - FastMath.PI_HALF);
            else if (input.keyDown(Key.A))
                _BuildRotation(deltaTime, _Base.Rotation.Z + FastMath.PI_HALF);
        }
        private void _BuildRotation(Single deltaTime, Single to)
        {
            var step = deltaTime * BuildRotationSpeed;
            if (_IsBaseRotating)
            {
                _Base.Rotation = _Base.Rotation.AdvanceZ(step, to);
                _BaseCollider.Obb.SetOrientation(_Base.Rotation);
                if (_IsRotationA)
                    _RotationB = _RotationB.AdvanceZ(step, to);
                else
                    _RotationA = _RotationA.AdvanceZ(step, to);
            }
            var r = Rotation = Rotation.AdvanceZ(step, to);
            if (_IsRotationA)
                _RotationA = r;
            else _RotationB = r;
        }

        private readonly TgcObb _ObbLoad;
        private readonly Single _Force = 100f;
        private readonly MeshUnRotatedPart _Base;
        private readonly ObbTranslatedUnRotatedCollider _BaseCollider;
        private Vector3 _RotationA = Vector3.Empty;
        private Vector3 _RotationB = Vector3.Empty;
        private Vector3 _RotationF = Vector3.Empty;
        public override void Animate(Single deltaTime)
        {
            if (_Load == null) return;
            _Load.Position = Position;
            _Load.Velocity = Vector3.Empty;
            _Load.AngularVelocity = Vector3.Empty;
            _Load.Momentum = Vector3.Empty;
            _Load.AngularMomentum = Vector3.Empty;
            var r = Rotation;
            var step = deltaTime * _RotationSpeed;
            r = Rotation = new Vector3(
                r.X.AdvanceTo(step, _RotationF.X),
                r.Y.AdvanceTo(step, _RotationF.Y),
                r.Z.AdvanceTo(step, _RotationF.Z)
            );
            if (r == _RotationF)
            {
                //TODO: PARTICULAS ACA GUSTAVO !!!
                var d = _ObbLoad.Orientation[1];
                _Load.Position += d * (_ObbExtents.Y + 1);
                _Load.Velocity = d * _Force;
                _Load = null;
            }
        }
        public override void Act(Interactive interactive, Single deltaTime)
        {
            if (_Load != null) return;
            var p = interactive.Position;
            if (_ObbLoad.ClosestPoint(p) == p)
            {
                _Load = interactive;
                _RotationF = Rotation == _RotationA ? _RotationB : _RotationA;
            }
        }
        #endregion ItemMethods
    }
}
