using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ObbTranslatedCollider : ObbCollider
    {
        #region Constructors
        private Vector3 _Translation;
        private Vector3 _TranslationCurrent;
        public ObbTranslatedCollider(Game game, TgcMesh mesh)
            : base(game, mesh)
        {
            _TranslationCurrent = _Translation = Obb.Position - mesh.Position;
        }
        public ObbTranslatedCollider(Game game, TgcMesh mesh, Vector3 translation)
            : this(game, TgcObb.computeFromAABB(mesh.BoundingBox), translation) { }
        public ObbTranslatedCollider(Game game, TgcObb obb, Vector3 translation)
            : base(game, obb)
        {
            _TranslationCurrent = _Translation = translation;
        }
        #endregion Constructors

        #region PartMethods
        protected override void Item_ScaleChanged(Item item)
        {
            base.Item_ScaleChanged(item);
            UpdateTranslation();
        }
        protected override void Item_RotationChanged(Item item)
        {
            base.Item_RotationChanged(item);
            UpdateTranslation();
        }
        protected override void Item_PositionChanged(Item item)
        {
            base.Item_PositionChanged(item);
            Obb.Center += _TranslationCurrent;
        }
        protected void UpdateTranslation()
        {
            _TranslationCurrent = Vector3.TransformCoordinate(_Translation.MemberwiseMult(Scale), Obb.GetOrientation());
            Obb.Center = Position + _TranslationCurrent;
        }
        #endregion PartMethods
    }
}
