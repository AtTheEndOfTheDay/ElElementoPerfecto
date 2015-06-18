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
    public class Acelerator : Item
    {
        #region Constants
        private const Single _ForceFactor = 100f;
        #endregion Constants

        #region Constructors
        private readonly ObbCollider _Collider;
        private readonly TranslatedParticlePart _Arrows;
        private readonly TranslatedParticlePart _RedArrows;
        public Acelerator()
        {
            var mesh = Game.Current.NewMesh("WallTextured");
            Add(new MeshStaticPart(mesh) { Texture = Game.Current.GetMaterial("Acelerator.jpg") });
            Add(_Collider = new ObbCollider(mesh));
            Add(_Arrows = new TranslatedParticlePart()
            {
                Translation = new Vector3(0, 0, -4),
                Animation = new AnimatedQuad()
                {
                    Texture = Game.Current.GetParticle("RedArrows.png"),
                    FrameSize = new Size(512, 256),
                    Size = new Vector2(8, 8),
                    FirstFrame = 7,
                    CurrentFrame = 7,
                    FrameRate = 4,
                    TotalFrames = 6,
                }
            });
            Add(_RedArrows = new TranslatedParticlePart()
            {
                Translation = new Vector3(0, 0, -4.1f),
                Sound = Game.Current.GetSound("Acelerator.wav", EffectVolume),
                Animation = new AnimatedQuad()
                {
                    Texture = Game.Current.GetParticle("RedArrows.png"),
                    FrameSize = new Size(512, 256),
                    Size = new Vector2(8, 8),
                    FirstFrame = 5,
                    CurrentFrame = 5,
                    FrameRate = 1,
                    TotalFrames = 1,
                }
            });
        }
        #endregion Constructors

        #region Properties
        private Single _Force;
        private Single _ForceReal;
        public Single Force
        {
            get { return _Force; }
            set
            {
                _Force = value;
                _ForceReal = value * _ForceFactor;
            }
        }
        #endregion Properties

        #region ResetMethods
        public override void LoadValues()
        {
            _Arrows.Stop();
            _RedArrows.Stop();
            base.LoadValues();
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
            _Arrows.KeepPlaying();
            _Arrows.Update(deltaTime);
            _RedArrows.Update(deltaTime);
        }
        protected override void OnCollision(ItemCollision itemCollision)
        {
            itemCollision.Interactive.Momentum += _Collider.Right * _ForceReal;
            _RedArrows.KeepPlaying();
        }
        public override Boolean React(ItemContactState contactState, Single deltaTime)
        {
            return false;
        }
        #endregion ItemMethods
    }
}
