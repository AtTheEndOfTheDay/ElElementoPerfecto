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
    public abstract class Interactive : Item
    {
        #region Constructors
        public Interactive()
        {
            Mass = 1;
            //ScaleChanged -= Interactive_ScaleChanged;
            ScaleChanged += Interactive_ScaleChanged;
        }
        private void Interactive_ScaleChanged(Item item)
        {
            UpdateBody();
        }
        #endregion Constructors

        #region ResetMethods
        protected Vector3 SavedVelocity;
        protected Vector3 SavedAngularVelocity;
        protected Vector3 SavedMomentum;
        protected Vector3 SavedAngularMomentum;
        protected Single SavedMass;
        public override void SaveValues()
        {
            base.SaveValues();
            SavedVelocity = Velocity;
            SavedAngularVelocity = AngularVelocity;
            SavedMomentum = Momentum;
            SavedAngularMomentum = AngularMomentum;
            SavedMass = Mass;
        }
        public override void LoadValues()
        {
            base.LoadValues();
            Velocity = SavedVelocity;
            AngularVelocity = SavedAngularVelocity;
            Momentum = SavedMomentum;
            AngularMomentum = SavedAngularMomentum;
            Mass = SavedMass;
        }
        #endregion ResetMethods

        #region PhysicsProperties
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public Vector3 Momentum;
        public Vector3 AngularMomentum;

        public Matrix Body { get; private set; }
        public Matrix BodyInverse { get; private set; }
        protected abstract Matrix ComputeBody();
        protected void UpdateBody()
        {
            if (_Mass == 0)
            {
                Body = Matrix.Zero;
                BodyInverse = Matrix.Zero;
            }
            else
            {
                Body = ComputeBody();
                BodyInverse = Matrix.Invert(Body);
            }
        }

        private Single _Mass = 0;
        public Single MassInverse { get; private set; }
        public Single Mass
        {
            get { return _Mass; }
            set
            {
                if (_Mass == value || value < 0) return;
                MassInverse = value == 0 ? 0 : 1 / value;
                _Mass = value;
                UpdateBody();
            }
        }
        #endregion PhysicsProperties

        #region InteractionMethods
        public Vector3 GetVelocityAt(Vector3 r)
        {
            return Velocity + Vector3.Cross(AngularVelocity, r);
        }
        public Vector3 GetForceAt(Vector3 r)
        {
            return Momentum + Vector3.Cross(AngularMomentum, r);
        }
        public void AddVelocityAt(Vector3 r, Vector3 velocity)
        {
            AngularVelocity += Vector3.Cross(r, velocity);
            r.Normalize();
            Velocity += Vector3.Dot(r, velocity) * r;
        }
        public void AddForceAt(Vector3 r, Vector3 force)
        {
            AngularMomentum += Vector3.Cross(r, force);
            r.Normalize();
            Momentum += Vector3.Dot(r, force) * r;
        }
        public void Simulate(Single deltaTime)
        {
            Velocity += deltaTime * MassInverse * Momentum;
            Position += deltaTime * Velocity;
            Momentum = Vector3.Empty;
            var Iinv = RotationMatrix * BodyInverse * Matrix.TransposeMatrix(RotationMatrix);
            AngularVelocity += deltaTime * Iinv.Multiply(AngularMomentum);
            Rotation += deltaTime * AngularVelocity;
            AngularMomentum = Vector3.Empty;
        }
        #endregion InteractionMethods
    }
}
