using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshTranslatedScaledPart : MeshTransformedPart
    {
        #region Constructors
        public Vector3 ScaleFactor { get; set; }
        public Vector3 Translation { get; set; }
        private Vector3 _TranslationCurrent;
        public MeshTranslatedScaledPart(TgcMesh mesh, Vector3 translation, Vector3 scaleFactor)
            : base(mesh)
        {
            Position = _TranslationCurrent = Translation = translation;
            Scale = scaleFactor.MemberwiseMult(_ItemScale = Item.DefaultScale);
            ScaleFactor = scaleFactor;
        }
        #endregion Constructors

        #region PartMethods
        private Vector3 _ItemScale;
        protected override void Item_ScaleChanged(Item item)
        {
            Scale = ScaleFactor.MemberwiseMult(_ItemScale = item.Scale);
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
            _TranslationCurrent = Vector3.TransformCoordinate(Translation.MemberwiseMult(_ItemScale), RotationMatrix);
            Position = item.Position + _TranslationCurrent;
        }
        #endregion PartMethods
    }
}
