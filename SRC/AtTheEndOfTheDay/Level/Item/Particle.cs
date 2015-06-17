using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    class Particle : IDisposable
    {
        #region Properties
        public TgcStaticSound Sound { get; set; }
        public AnimatedQuad Animation { get; set; }



        private Vector2 _InitialSize = new Vector2(1, 1);

        public Vector2 Size
        {
            get { return _InitialSize; }
            set { _InitialSize = value; }
        }

        #endregion Properties

        #region AnimationMethods
        public void Start(Vector3 location, Single Power)
        { 
            if(Animation != null)
            {
                Animation.Position = location;
                //Animation.Size = _InitialSize * _PowerFactor * Power;
                this.Start();
            }

        }

        public void Start()
        {
            if (Animation != null)
                Animation.Start();
            if (Sound != null)
                Sound.play(false);
        }
        public void KeepPlaying()
        {
            if ((Animation == null) || (Animation._IsEnabled))
                return;
            this.Start();
        }
        public void Update(Single deltaTime)
        {
            if (Animation != null)
                Animation.Update(deltaTime);
        }
        public void Stop()
        {
            if (Animation != null)
                Animation.Stop();
            if (Sound != null)
                Sound.stop();
        }
        #endregion AnimationMethods


        public void Dispose()
        {
            if (Animation != null)
                Animation.Dispose();
            if (Sound != null)
                Sound.dispose();
        }
    }
}
