using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshTranslatedScaledPart : MeshTransformedPart
    {
        #region Constructors
        private Vector3 _ScaleFactor;
        private Vector3 _Translation;
        private Vector3 _TranslationCurrent;
        public MeshTranslatedScaledPart(TgcMesh mesh, Vector3 translation, Vector3 scaleFactor)
            : base(mesh)
        {
            Position = _TranslationCurrent = _Translation = translation;
            Scale = _ScaleFactor = scaleFactor;
        }
        #endregion Constructors

        #region PartMethods
        protected override void Item_ScaleChanged(Item item)
        {
            Scale = _ScaleFactor.MemberwiseMult(item.Scale);
            UpdateTranslation(item);
        }
        protected override void Item_RotationChanged(Item item)
        {
            base.Item_RotationChanged(item);
            UpdateTranslation(item);
        }
        protected override void Item_PositionChanged(Item item)
        {
            Position = item.Position + _TranslationCurrent;
        }
        protected void UpdateTranslation(Item item)
        {
            _TranslationCurrent = Vector3.TransformCoordinate(_Translation, RotationMatrix);
            Position = item.Position + _TranslationCurrent;
        }
        #endregion PartMethods
    }
}
