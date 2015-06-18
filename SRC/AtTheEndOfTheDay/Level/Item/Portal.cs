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
    public class Portal : Item
    {
        #region Constants
        #endregion Constants

        #region Constructors
        private readonly Collider _Collider;
        private readonly MeshImmutableePart _Receptor;
        private readonly ItemPart[] _PartList = new ItemPart[3];
        private readonly TgcStaticSound _SoundEffect;
        public Portal()
        {
            var mesh = Game.Current.NewMesh("BallTextured");
            Add(_PartList[0] = new MeshStaticPart(mesh) { Texture = Game.Current.GetMaterial("BluePortal.png") });
            _PartList[1] = _Collider = new ObbCollider(mesh); Add(_Collider); 
            var receptorMesh = Game.Current.NewMesh("BallTextured");
            Add(_PartList[2] = _Receptor = new MeshImmutableePart(receptorMesh) { Texture = Game.Current.GetMaterial("OrangePortal.png") });
            _SoundEffect = Game.Current.GetSound("portal2.wav", EffectVolume);
        }
        public override void LoadValues()
        {
            if (IsEmptyOfParts)
                Add(_PartList);
            base.LoadValues();
        }
        #endregion Constructors

        #region Properties
        public Vector3 ReceptorPosition
        {
            get { return _Receptor.Position; }
            set { _Receptor.Position = value; }
        }
        public Vector3 ReceptorScale
        {
            get { return _Receptor.Scale; }
            set { _Receptor.Scale = value; }
        }
        public Vector3 ReceptorRotation
        {
            get { return _Receptor.Rotation; }
            set { _Receptor.Rotation = value; }
        }
        #endregion Properties

        #region ItemMethods
        protected override void OnCollision(ItemCollision itemCollision)
        {
            itemCollision.Interactive.Position = _Receptor.Position;
            if (_SoundEffect != null)
                _SoundEffect.play(false);
            ClearParts();
        }
        public override Boolean React(ItemContactState contactState, Single deltaTime)
        {
            return false;
        }
        #endregion ItemMethods
    }
}
