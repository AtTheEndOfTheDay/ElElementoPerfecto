using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    class IndependentParticlePart : ParticlePart
    {
        private readonly Vector3 _InitialUpDirection = Vector3Extension.Top;

        private Vector3 _Translation = Vector3.Empty;
        private Vector2 _InitialSize = new Vector2(1, 1);
        public Vector2 Size
        {
            get { return _InitialSize; }
            set { _InitialSize = value; }
        }

        private Single _PowerFactor = 0.01f;
        public Single PowerFactor
        {
            get { return _PowerFactor; }
            set { _PowerFactor = value; }
        }

        public Vector3 Translation
        {
            get { return _Translation; }
            set { _Translation = value; }
        }
        #region PartMethods
        public override void Attach(Item item) { }
        public override void Detach(Item item) { }

        public void Start(Vector3 location, Single Power, Vector3 newUpDirection)
        {
            if (Animation != null)
            {
                Animation.Size = _InitialSize *(_PowerFactor * Power);
                Animation.Position = location;// +newUpDirection * (Animation.Size.Y / 2);

                Single alfa = FastMath.Acos(Vector3.Dot(newUpDirection,_InitialUpDirection));
                if (newUpDirection.X > _InitialUpDirection.X)
                {
                    alfa *= -1;
                }
                Animation.RotationMatrix = Matrix.RotationZ(alfa);
                this.Start();
            }
        }

        #endregion PartMethods
    }
}
