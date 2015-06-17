using AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ParticlePart : ItemPart
    {
        #region Constructios
        public ParticlePart()
        {
            Scale = Item.DefaultScale;
            Position = Item.DefaultPosition;
            RotationMatrix = Item.DefaultRotationMatrix;
        }
        #endregion Constructios

        #region Properties
        public TgcStaticSound Sound { get; set; }
        public AnimatedQuad Animation { get; set; }
        #endregion Properties

        #region PartMethods
        public override void Attach(Item item)
        {
            Detach(item);
            ItemScaleChanged(item);
            ItemRotationChanged(item);
            ItemPositionChanged(item);
            item.ScaleChanged += ItemScaleChanged;
            item.RotationChanged += ItemRotationChanged;
            item.PositionChanged += ItemPositionChanged;
        }
        public override void Detach(Item item)
        {
            item.ScaleChanged -= ItemScaleChanged;
            item.RotationChanged -= ItemRotationChanged;
            item.PositionChanged -= ItemPositionChanged;
        }
        public override void Render(Item item, Effect shader)
        {
            if (Animation != null)
                Animation.Render();
        }
        public override void Dispose()
        {
            if (Animation != null)
                Animation.Dispose();
            if (Sound != null)
                Sound.dispose();
        }

        public Vector3 Scale { get; set; }
        public Vector3 Position { get; set; }
        public Matrix RotationMatrix { get; set; }

        protected virtual void ItemScaleChanged(Item item)
        {
            Scale = item.Scale;
            if (Animation == null) return;
            Single xScaleIncremente = item.Scale.X / Scale.X;
            Single yScaleIncremente = item.Scale.Y / Scale.Y;
            Animation.Size = new Vector2(Animation.Size.X * xScaleIncremente, Animation.Size.Y * yScaleIncremente);
        }
        protected virtual void ItemRotationChanged(Item item)
        {
            RotationMatrix = item.RotationMatrix;
            if (Animation != null)
                Animation.RotationMatrix = item.RotationMatrix;
        }
        protected virtual void ItemPositionChanged(Item item)
        {
            Position = item.Position;
            if (Animation != null)
                Animation.Position = item.Position;
        }
        #endregion PartMethods

        #region AnimationMethods
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
    }
}
