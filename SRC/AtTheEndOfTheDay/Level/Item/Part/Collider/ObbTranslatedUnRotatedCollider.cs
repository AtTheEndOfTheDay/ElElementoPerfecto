using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ObbTranslatedUnRotatedCollider : ObbTranslatedCollider
    {
        #region Constructors
        public ObbTranslatedUnRotatedCollider(TgcMesh mesh)
            : base(mesh) { }
        public ObbTranslatedUnRotatedCollider(TgcMesh mesh, Vector3 translation)
            : base(mesh, translation) { }
        public ObbTranslatedUnRotatedCollider(TgcObb obb, Vector3 translation)
            : base(obb, translation) { }
        #endregion Constructors

        #region PartMethods
        protected override void Item_RotationChanged(Item item)
        {
            UpdateTranslation();
        }
        #endregion PartMethods
    }
}
