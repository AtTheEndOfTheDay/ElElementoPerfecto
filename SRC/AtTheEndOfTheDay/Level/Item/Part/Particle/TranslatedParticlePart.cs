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
        #region Properties
        private Vector3 _Translation;
        private Vector3 _TranslationCurrent;
        public Vector3 Translation
        {
            get { return _Translation; }
            set { _Translation = value; UpdateTranslation(); }
        }
        protected void UpdateTranslation()
        {
            _TranslationCurrent = Vector3.TransformCoordinate(_Translation.MemberwiseMult(Scale), RotationMatrix);
            if (Animation != null)
                Animation.Position = Position + _TranslationCurrent;
        }
        #endregion Properties

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
            if (Animation != null)
                Animation.Position += _TranslationCurrent;
        }
        #endregion PartMethods
    }
}
