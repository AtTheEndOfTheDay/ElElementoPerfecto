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
        protected readonly AnimatedQuad AnimatedQuad;
        public TgcStaticSound Efecto;
        public ParticlePart(AnimatedQuad animatedQuad)
        {
            this.AnimatedQuad = animatedQuad;
        }

        public ParticlePart(AnimatedQuad animatedQuad,TgcStaticSound sound)
        {
          this.AnimatedQuad = animatedQuad;
             Efecto=sound;
        }

        public virtual void Attach(Item item)
        {
            Detach(item);
            item.ScaleChanged += ItemScaleChanged;
            item.RotationChanged += ItemRotationChanged;
            item.PositionChanged += ItemPositionChanged;
        }
        public virtual void Detach(Item item)
        {
            item.ScaleChanged -= ItemScaleChanged;
            item.RotationChanged -= ItemRotationChanged;
            item.PositionChanged -= ItemPositionChanged;
        }

        public Vector3 Scale = Item.DefaultScale;
        public Vector3 Position = Item.DefaultPosition;
        public Vector3 Rotation = Item.DefaultRotation;

        protected virtual void ItemScaleChanged(Item item)
        {
            Single xScaleIncremente = item.Scale.X / Scale.X;
            Single yScaleIncremente = item.Scale.Y / Scale.Y;

            AnimatedQuad.Size = new Vector2(AnimatedQuad.Size.X * xScaleIncremente, AnimatedQuad.Size.Y * yScaleIncremente);
            Scale = item.Scale;
        }
        protected virtual void ItemRotationChanged(Item item)
        {
            AnimatedQuad.Rotation += item.Rotation - Rotation;
            Rotation = item.Rotation;
        }
        protected virtual void ItemPositionChanged(Item item)
        {
            AnimatedQuad.Position = Position = item.Position ;
        }

        public virtual void updateParticle()
        {
            AnimatedQuad.update();
        }

        public virtual void initParticle()
        {
            AnimatedQuad.initAnimation();
            Efecto.play(false);
        }

        public virtual void stopParticle()
        {
            AnimatedQuad.stopAnimation();
            Efecto.stop();
        }

        public void Render(Item item, Effect shader)
        {
            AnimatedQuad.render();
        }
        public void Dispose()
        {
            AnimatedQuad.dispose();
            Efecto.dispose();
        }

    }
}
