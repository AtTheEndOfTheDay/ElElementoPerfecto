using System;
using System.Drawing;
using Microsoft.DirectX;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Repulsor : Item
    {
        #region Constants
        private const Single _ForceFactor = 100000f;
        private const Single _RepulsionFactor = 100f;
        #endregion Constants

        #region Constructors
        private readonly IndependentParticlePart _Spark;
        public Repulsor()
        {
            var mesh = Game.Current.NewMesh("Repulsor");
            Add(new MeshStaticPart(mesh) { Color = Color.FromArgb(0, 75, 0, 0) });
            Add(new SphereCollider(mesh));
            Add(_Spark = new IndependentParticlePart()
            {
                Translation = new Vector3(0, 0, -4),
                Sound = Game.Current.GetSound("Repulsor.wav", EffectVolume),
                Animation = new AnimatedQuad()
                {
                    Texture = Game.Current.GetParticle("RedSparks.png"),
                    FrameSize = new Size(256, 256),
                    FirstFrame = 0,
                    CurrentFrame = 0,
                    FrameRate = 30,
                    TotalFrames = 10,
                },
                Size = new Vector2(50, 30),
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
        private Single _RepulsionDistance;
        private Single _RepulsionDistancePow2;
        public Single RepulsionDistance
        {
            get { return _RepulsionDistance; }
            set
            {
                _RepulsionDistance = value;
                _RepulsionDistancePow2 = value * value;
            }
        }
        #endregion Properties

        #region ResetMethods
        private Single _SavedForceReal;
        public override void SaveValues()
        {
            _SavedForceReal = _ForceReal;
            base.SaveValues();
        }
        public override void LoadValues()
        {
            _ForceReal = _SavedForceReal;
            _Spark.Stop();
            base.LoadValues();
        }
        #endregion ResetMethods

        #region ItemMethods
        public override void Signal(Object[] signal)
        {
            if (signal.Length == 0) return;
            var value = signal[0];
            try { _ForceReal *= Convert.ToSingle(value); }
            catch (Exception) { }
        }
        public override void Animate(Single deltaTime)
        {
            _Spark.Update(deltaTime);
        }
        public override void Act(Interactive interactive, Single deltaTime)
        {
            var n = interactive.Position - Position;
            var d2 = n.LengthSq();
            n.Normalize();
            if (d2 < _RepulsionFactor * _RepulsionDistancePow2)
                interactive.Momentum += n * (_ForceReal / d2);
        }
        protected override void OnContact(ItemContactState contactState, Single deltaTime)
        {
            base.OnContact(contactState, deltaTime);
            _Spark.Start(contactState.Point, contactState.Approach, contactState.Normal);
        }
        #endregion ItemMethods
    }
}
