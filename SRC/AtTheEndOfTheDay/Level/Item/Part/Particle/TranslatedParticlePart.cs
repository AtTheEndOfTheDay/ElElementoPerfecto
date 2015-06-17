using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class TranslatedParticlePart : ParticlePart
    {
        #region Constructors
        private Vector3 _Translation;
        private Vector3 _TranslationCurrent;
        public TgcStaticSound efecto;
        public TranslatedParticlePart(AnimatedQuad animatedQuad) 
            : base(animatedQuad) 
        {
            _TranslationCurrent = _Translation = animatedQuad.Position - Item.DefaultPosition;
        }

        public TranslatedParticlePart(AnimatedQuad animatedQuad, TgcStaticSound sonido)
            : base(animatedQuad ,sonido)
        {
            _TranslationCurrent = _Translation = animatedQuad.Position - Item.DefaultPosition;
            efecto = sonido;
        }
        #endregion Constructors

        #region PartMethods
        protected override void ItemScaleChanged(Item item)
        {
            base.ItemScaleChanged(item);
            UpdateTranslation();
        }
        protected override void ItemRotationChanged(Item item)
        {
            base.ItemRotationChanged(item);
            UpdateTranslation();
        }
        protected override void ItemPositionChanged(Item item)
        {
            base.ItemPositionChanged(item);
            AnimatedQuad.Position += _TranslationCurrent;
        }
        protected void UpdateTranslation()
        {
            _TranslationCurrent = Vector3.TransformCoordinate(_Translation.MemberwiseMult(Scale), Matrix.RotationYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            AnimatedQuad.Position = Position + _TranslationCurrent;
        }
        #endregion PartMethods
    }
}
