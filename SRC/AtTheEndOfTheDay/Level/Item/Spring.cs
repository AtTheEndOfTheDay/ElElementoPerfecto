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
    public class Spring : Item
    {
        #region Constants
        private const Single _ElasticityFactor = 100f;
        private const Single _SpringElasticVelocityY = 50f;
        private const Single _SpringContractedY = .5f;
        private const Single _SpringSizeInverseY = 1f / 17f;
        private static readonly Vector3 _CoverScaleFactor = new Vector3(2f, .2f, 2f);
        private static readonly Vector3 _CoverTranslation = new Vector3(0f, 8f, 0f);
        #endregion Constants

        #region Constructors
        private readonly MeshTranslatedScaledPart _Top;
        private readonly MeshTranslatedScaledPart _Bottom;
        private readonly ObbTranslatedCollider _Collider;
        public Spring()
        {
            var mesh = Game.Current.GetMesh("Spring");
            Add(new MeshStaticPart(mesh));
            Add(_Top = new MeshTranslatedScaledPart(Game.Current.NewMesh("WallRounded"), _CoverTranslation, _CoverScaleFactor) { Color = Color.FromArgb(123, 123, 123) });
            Add(_Bottom = new MeshTranslatedScaledPart(Game.Current.NewMesh("WallRounded"), -_CoverTranslation, _CoverScaleFactor) { Color = Color.FromArgb(0, 0, 0) });
            Add(_Collider = new ObbTranslatedCollider(mesh));
        }
        #endregion Constructors

        #region Properties
        private TgcStaticSound _SoundEffect;
        private String _SoundString;
        public String SoundEffect
        {
            get { return _SoundString; }
            set
            {
                _SoundString = value;
                _SoundEffect = Game.Current.GetSound(_SoundString, EffectVolume);
            }
        }
        public Single _Elasticity = 1f;
        public Single _RealElasticity = _ElasticityFactor;
        public Single Elasticity
        {
            get { return _Elasticity; }
            set
            {
                _Elasticity = value;
                _RealElasticity = _ElasticityFactor * value;
            }
        }
        #endregion Properties

        #region ResetMethods
        public override void LoadValues()
        {
            base.LoadValues();
            _TotalContraction = 0;
        }
        private static readonly Vector3 _MenuProportion = Vector3Extension.One * .7f;
        public override void MenuTransform(Vector3 scale, Vector3 rotation, Vector3 position)
        {
            base.MenuTransform(scale.MemberwiseMult(_MenuProportion), rotation, position);
        }
        #endregion ResetMethods

        #region ItemMethods
        public override void Build(Single deltaTime)
        {
            var input = GuiController.Instance.D3dInput;
            var stepR = deltaTime * BuildRotationSpeed;
            if (input.keyDown(Key.D))
                Rotation = Rotation.AddZ(-stepR);
            else if (input.keyDown(Key.A))
                Rotation = Rotation.AddZ(stepR);
        }
        public override void Animate(Single deltaTime)
        {
            if (_MaxDepthState != null)
            {
                var contraction = _MaxDepthState.ComputeApproachVelocity() * deltaTime;
                _Contract(contraction);
                _MaxDepthState = null;
                _MaxDepth = 0;
            }
            else if (!_IsContactOcurred && _TotalContraction > 0)
                _Contract(-_SpringElasticVelocityY * deltaTime * Scale.Y);
            _IsContactOcurred = false;
        }
        private Boolean _IsContactOcurred = false;
        private Single _MaxDepth = 0;
        private ItemContactState _MaxDepthState = null;
        private Single _TotalContraction = 0;
        protected override void OnContact(ItemContactState contactState, Single deltaTime)
        {
            _IsContactOcurred = true;
            if (_MaxDepth < contactState.Contact.Depth)
            {
                _MaxDepth = contactState.Contact.Depth;
                _MaxDepthState = contactState;
            }
            if (Scale.Y < _SpringContractedY || _TotalContraction < 0
            || !Vector3.Dot(_Collider.Top, contactState.Normal).TolerantEquals(1))
            {
                if (_TotalContraction < 0)
                {
                    _Contract(-_TotalContraction);
                    if (_SoundEffect != null)
                        _SoundEffect.play(false);
                    _TotalContraction = 0;
                }
                base.OnContact(contactState, deltaTime);
            }
            else contactState.Interactive.AddForceAt(contactState.Radius, _RealElasticity * _TotalContraction * _Collider.Top);
        }
        private void _Contract(Single contraction)
        {
            _TotalContraction += contraction;
            Scale = Scale.AddY(-contraction * _SpringSizeInverseY);
            Position = Position + contraction * _Collider.Bottom;
        }
        protected override void OnRestingContact(ItemContactState contactState)
        {
            if (Scale.Y < _SpringContractedY
            || !Vector3.Dot(_Collider.Top, contactState.Normal).TolerantEquals(1))
                base.OnRestingContact(contactState);
        }
        protected override void OnRestitutiveContact(ItemContactState contactState)
        {
            if (Scale.Y < _SpringContractedY
            || !Vector3.Dot(_Collider.Top, contactState.Normal).TolerantEquals(1))
                base.OnRestitutiveContact(contactState);
        }
        #endregion ItemMethods
    }
}
