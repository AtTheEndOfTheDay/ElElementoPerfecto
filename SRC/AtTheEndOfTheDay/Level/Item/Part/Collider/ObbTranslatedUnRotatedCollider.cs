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
