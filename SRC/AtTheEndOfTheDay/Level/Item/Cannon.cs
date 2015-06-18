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
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Cannon : Item
    {
        #region Constants
        private const Single _ForceFactor = 100f;
        private const Single _RotationSpeed = .7f;
        private const Single _BorderScale = .2f;
        private const Single _BodyObbScaleY = .8f;
        private const Single _BaseObbScaleY = .13f;
        private static readonly Vector3 _LoadColiderExtents = Vector3Extension.One * 4f;
        private static readonly Vector3 _BaseObbTranslation = Vector3Extension.Bottom * 12f;
        private static readonly Vector3 _MinBorderScale = Vector3Extension.One * _BorderScale;
        private static readonly Vector3 _MaxBorderScale = new Vector3(1f, 0f, 1f) * _BorderScale;
        #endregion Constants

        #region Constructors
        private readonly MeshUnRotatedPart _Base;
        private readonly ObbTranslatedUnRotatedCollider _BaseCollider;
        private readonly ObbCollider _LoadColider;
        private readonly TranslatedParticlePart _Smoke;
        private TgcStaticSound _ChargeSound;
        public Cannon()
        {
            var bodyMesh = Game.Current.GetMesh("Cannon");
            _LoadColider = new ObbCollider()
            {
                Extents = _LoadColiderExtents,
                Color = ItemPart.DefaultPartColor,
            };
            Add(_LoadColider as ItemPart);
            Add(new MeshStaticPart(bodyMesh));
            var bodyObb = TgcObb.computeFromAABB(bodyMesh.BoundingBox);
            var bodyE = bodyObb.Extents;
            bodyObb.Extents = new Vector3(bodyE.X * _BodyObbScaleY, bodyE.Y, bodyE.Z * _BodyObbScaleY);
            Add(new HollowObbCollider(bodyObb, bodyObb.Position - bodyMesh.Position, _MinBorderScale, _MaxBorderScale));
            bodyObb.dispose();

            var baseMesh = Game.Current.GetMesh("CannonBase");
            var baseObb = TgcObb.computeFromAABB(baseMesh.BoundingBox);
            var baseE = baseObb.Extents;
            baseObb.Extents = new Vector3(baseE.X, baseE.Y * _BaseObbScaleY, baseE.Z);
            Add(_Base = new MeshUnRotatedPart(baseMesh));
            Add(_BaseCollider = new ObbTranslatedUnRotatedCollider(baseObb, _BaseObbTranslation));

            Add(_Smoke = new TranslatedParticlePart()
            {
                Translation = new Vector3(0, 33, -4),
                Sound = Game.Current.GetSound("Cannon.wav", EffectVolume),
                Animation = new AnimatedQuad()
                {
                    Texture = Game.Current.GetParticle("ExplosionGrey.png"),
                    FrameSize = new Size(146, 146),
                    Size = new Vector2(25, 25),
                    FirstFrame = 2,
                    CurrentFrame = 2,
                    FrameRate = 15,
                    TotalFrames = 47,
                }
            });
            RotationChanged += Cannon_RotationChanged;
            _ChargeSound = Game.Current.GetSound("CannonCharge.wav", 0);
        }
        private void Cannon_RotationChanged(Item item)
        {
            _Base.Rotation = new Vector3(Rotation.X, Rotation.Y, _Base.Rotation.Z.Clamp(Rotation.Z - FastMath.PI_HALF, Rotation.Z + FastMath.PI_HALF));
            _BaseCollider.SetOrientation(_Base.Rotation);
            if (!_IsRotationTarget)
                _StoredRotation = Rotation;
        }
        #endregion Constructors

        #region Properties
        private Single _Force = 1;
        private Single _ForceReal = _ForceFactor;
        public Single Force
        {
            get { return _Force; }
            set
            {
                _Force = value;
                _ForceReal = value * _ForceFactor;
            }
        }
        public Vector3 RotationTarget { get; set; }
        public Single BaseRotationZ
        {
            get { return _Base.Rotation.Z; }
            set
            {
                if (_Base.Rotation.Z == value) return;
                _Base.Rotation = _Base.Rotation.SetZ(value);
                Cannon_RotationChanged(this);
            }
        }
        #endregion Properties

        #region ResetMethods
        private Interactive _Load = null;
        private Vector3 _BaseRotationSaved;
        private Boolean _IsRotationTargetSaved = false;
        public override void SaveValues()
        {
            _BaseRotationSaved = _Base.Rotation;
            _IsRotationTargetSaved = _IsRotationTarget;
            base.SaveValues();
        }
        public override void LoadValues()
        {
            _ChargeSound.dispose();
            _ChargeSound = Game.Current.GetSound("CannonCharge.wav", 0);
            _Base.Rotation = _BaseRotationSaved;
            _IsRotationTarget = _IsRotationTargetSaved;
            base.LoadValues();
            OnScaleChanged();
            _Load = null;
            _Smoke.Stop();
        }
        private static readonly Vector3 _MenuProportion = new Vector3(.1f, .2f, .2f);
        public override void MenuTransform(Vector3 scale, Vector3 rotation, Vector3 position)
        {
            _Base.Rotation = rotation;
            base.MenuTransform(scale.MemberwiseMult(_MenuProportion), rotation, position);
        }
        #endregion ResetMethods

        #region ItemMethods
        private Vector3 _StoredRotation;
        private Boolean _IsRotationTarget = false;
        public override void Build(Single deltaTime)
        {
            var input = GuiController.Instance.D3dInput;
            if (input.keyUp(Key.W))
            {
                Rotation = _IsRotationTarget ? _StoredRotation : RotationTarget;
                _IsRotationTarget = !_IsRotationTarget;
            }
            if (input.keyDown(Key.D))
                _BuildRotation(deltaTime, _Base.Rotation.Z - FastMath.PI_HALF, false);
            else if (input.keyDown(Key.A))
                _BuildRotation(deltaTime, _Base.Rotation.Z + FastMath.PI_HALF, false);
            if (input.keyDown(Key.Q))
                _BuildRotation(deltaTime, _Base.Rotation.Z - FastMath.PI_HALF, true);
            else if (input.keyDown(Key.E))
                _BuildRotation(deltaTime, _Base.Rotation.Z + FastMath.PI_HALF, true);
        }
        private void _BuildRotation(Single deltaTime, Single to, Boolean isBaseRotating)
        {
            var step = deltaTime * BuildRotationSpeed;
            if (isBaseRotating)
            {
                _Base.Rotation = _Base.Rotation.AdvanceZ(step, to);
                _BaseCollider.SetOrientation(_Base.Rotation);
            }
            var r = Rotation = Rotation.AdvanceZ(step, to);
            if (_IsRotationTarget)
                RotationTarget = r;
        }
        private Vector3 _RotationF = Vector3.Empty;
        public override void Animate(Single deltaTime)
        {
            _Smoke.Update(deltaTime);
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
            
            _ChargeSound.play(false);
            if (r == _RotationF)
                _Shoot();
        }
        private void _Shoot()
            {
                _ChargeSound.dispose();
                _ChargeSound = Game.Current.GetSound("CannonCharge.wav", 0);
                var d = _LoadColider.Orientation[1];
                _Load.Position += d * (_LoadColiderExtents.Y + 1);
                _Load.Velocity = d * _ForceReal;
                _Load = null;
                _Smoke.Start();
            }
        public override void StaticCollision(Item item)
        {
            if (item != _Load
            && _Load != null)
                _Shoot();
        }
        public override void Act(Interactive interactive, Single deltaTime)
        {
            if (_Load != null) return;
            var p = interactive.Position;
            if (_LoadColider.ClosestPoint(p) == p)
            {
                _Load = interactive;
                _RotationF = _IsRotationTarget ? _StoredRotation : RotationTarget;
                _IsRotationTarget = !_IsRotationTarget;
            }
        }
        #endregion ItemMethods
    }
}
