using System;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Sound;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class BlackHole : Item
    {
        #region Constants
        private const Single _ForceFactor = 100000f;
        private const Single _AtractionFactor = 100f;
        private static readonly Vector3 _EndOfTheWorld = Vector3Extension.One * 1000;
        #endregion Constants

        #region Constructors
        private readonly TgcStaticSound _SoundEffect;
        private readonly TranslatedParticlePart _StarStorm;
        public BlackHole()
        {
            var mesh = Game.Current.NewMesh("BallTextured");
            Add(new MeshStaticPart(mesh) { Texture = Game.Current.GetMaterial("BlackHole.jpg") });
            Add(new SphereCollider(mesh));
            _SoundEffect = Game.Current.GetSound("BlackHole.wav", EffectVolume);
            Add(_StarStorm = new TranslatedParticlePart()
            {
                Translation = new Vector3(0, 0, -4),
                Animation = new AnimatedQuad()
                {
                    Texture = Game.Current.GetParticle("thunders.png"),
                    FrameSize = new Size(256, 256),
                    Size = new Vector2(25, 25),
                    FirstFrame = 0,
                    CurrentFrame = 0,
                    FrameRate = 15,
                    TotalFrames = 16,
                }
            });
        }
        #endregion Constructors

        #region Properties
        public Single MaxScale { get; set; }
        public Single MinScale { get; set; }
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
        private Single _AtractionDistance;
        private Single _AtractionDistancePow2;
        public Single AtractionDistance
        {
            get { return _AtractionDistance; }
            set
            {
                _AtractionDistance = value;
                _AtractionDistancePow2 = value * value;
            }
        }
        #endregion Properties

        #region ResetMethods
        public override void LoadValues()
        {
            base.LoadValues();
            _StarStorm.Stop();
        }
        #endregion ResetMethods

        #region ItemMethods
        public override void Signal(Object[] signal)
        {
            if (signal.Length == 0)
                Position = _EndOfTheWorld;
            else if (signal[0] is Vector3)
                Position = (Vector3)signal[0];
        }
        private Boolean _IsGrowing = true;
        public override void Animate(Single deltaTime)
        {
            _StarStorm.Update(deltaTime);
            var stepX = 0.2f * deltaTime; 
            var stepY = 0.3f * deltaTime;
            if (_IsGrowing)
            {
                Scale = Scale.AdvanceX(stepX, MaxScale);
                Scale = Scale.AdvanceY(stepY, MaxScale);
            }
            else
            {
                Scale = Scale.AdvanceX(stepX, MinScale);
                Scale = Scale.AdvanceY(stepY, MinScale);
            }
            if (Scale.X == MinScale || Scale.X == MaxScale)
                _IsGrowing = !_IsGrowing;
        }
        public override void Act(Interactive interactive, Single deltaTime)
        {
            var n = interactive.Position - Position;
            var d2 = n.LengthSq();
            n.Normalize();
            if (d2 < _AtractionFactor * _AtractionDistancePow2)
            {
                _StarStorm.KeepPlaying();
                interactive.Momentum -= n * (_ForceReal / d2);
            }
        }
        protected override void OnCollision(ItemCollision itemCollision)
        {
            itemCollision.Interactive.Position = _EndOfTheWorld;
            if (_SoundEffect != null)
                _SoundEffect.play(false);
        }
        #endregion ItemMethods
    }
}
