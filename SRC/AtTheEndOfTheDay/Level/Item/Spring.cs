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
        private static readonly Vector3 _CoverScaleFactor = new Vector3(2f, .2f, 2f);
        private static readonly Vector3 _CoverTranslation = new Vector3(0f, 8f, 0f);
        #endregion Constants

        #region Constructors
        private readonly ObbTranslatedCollider _Collider;
        public Spring()
        {
            var mesh = Game.Current.GetMesh("Spring");
            Add(new MeshStaticPart(mesh));
            Add(new MeshTranslatedScaledPart(Game.Current.NewMesh("WallRounded"), _CoverTranslation, _CoverScaleFactor) { Color = Color.FromArgb(123, 123, 123) });
            Add(new MeshTranslatedScaledPart(Game.Current.NewMesh("WallRounded"), -_CoverTranslation, _CoverScaleFactor) { Color = Color.FromArgb(0, 0, 0) });
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
        public Single Elasticity { get; set; }
        #endregion Properties

        #region ItemMethods
        private Boolean _IsContracting = false;
        public override void Act(Interactive interactive, Single deltaTime)
        {
            base.Act(interactive, deltaTime);
        }
        protected override void OnContact(ItemContactState contactState)
        {
            base.OnContact(contactState);
            if (_SoundEffect != null)
                _SoundEffect.play(false);
            contactState.Collision.Restitution *= 1.5f;
        }
        protected override void OnRestitutiveContact(ItemContactState contactState)
        {
        } 
        #endregion ItemMethods
    }
}
