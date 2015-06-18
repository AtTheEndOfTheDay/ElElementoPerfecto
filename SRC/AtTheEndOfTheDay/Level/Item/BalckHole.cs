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
    public class BlackHole : Item
    {
        #region Constants
        private const Single _ForceFactor = 100000f;
        private const Single _AtractionFactor = 100f;
        #endregion Constants

        #region Constructors
        private TgcStaticSound _SoundEffect;
        public BlackHole()
        {
            var mesh = Game.Current.NewMesh("BallTextured");
            _SoundEffect = Game.Current.GetSound("blackhole2.wav", EffectVolume);
            Add(new MeshStaticPart(mesh) { Texture = Game.Current.GetMaterial("BlackHole.jpg") });
            Add(new SphereCollider(mesh));
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
        private readonly TranslatedParticlePart _StarStorm;
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
        public Single MaxScale { get; set; }
        public Single MinScale { get; set; }
        #endregion Properties

        #region ItemMethods
        private Boolean _IsGrowing = true;
        public override void LoadValues()
        {
            base.LoadValues();
            _StarStorm.Stop();
        }
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
        public override Boolean React(ItemCollision itemCollision, Single deltaTime)
        {
            if (itemCollision.Item != this) return false;
            var reacted = true;
            var interactive = itemCollision.Interactive;
            interactive.Position= new Vector3(0,0,Single.MaxValue);
            _SoundEffect.play(false);
            return reacted;
        }
        public override void ButtonSignal(Object[] signal)
        {
            if (signal.Length == 0)
                Position = new Vector3(0, 0, Single.MaxValue);
            else if (signal[0] is Vector3)
                Position = (Vector3)signal[0];
        }
        #endregion ItemMethods
    }
}
