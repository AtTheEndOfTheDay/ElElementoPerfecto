using AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    class ParticlePart : ItemPart
    {
        protected readonly AnimatedQuad animatedQuad;
        public ParticlePart(AnimatedQuad animatedQuad)
        {
            this.animatedQuad = animatedQuad;
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

            animatedQuad.Size = new Vector2(animatedQuad.Size.X * xScaleIncremente, animatedQuad.Size.Y * yScaleIncremente);
            Scale = item.Scale;
        }
        protected virtual void ItemRotationChanged(Item item)
        {
            animatedQuad.Rotation += item.Rotation - Rotation;
            Rotation = item.Rotation;
        }
        protected virtual void ItemPositionChanged(Item item)
        {
            animatedQuad.Position = Position = item.Position ;
        }

        public void Render(Item item, Effect shader)
        {
            animatedQuad.updateAndRender();
        }
        public void Dispose()
        {
            animatedQuad.dispose();
        }

    }
}
