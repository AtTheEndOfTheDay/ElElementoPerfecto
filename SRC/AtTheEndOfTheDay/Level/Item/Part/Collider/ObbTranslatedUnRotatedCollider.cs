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
        public ObbTranslatedUnRotatedCollider(Game game, TgcMesh mesh)
            : base(game, mesh) { }
        public ObbTranslatedUnRotatedCollider(Game game, TgcMesh mesh, Vector3 translation)
            : base(game, mesh, translation) { }
        public ObbTranslatedUnRotatedCollider(Game game, TgcObb obb, Vector3 translation)
            : base(game, obb, translation) { }
        #endregion Constructors

        #region PartMethods
        protected override void Item_RotationChanged(Item item)
        {
            UpdateTranslation();
        }
        #endregion PartMethods
    }
}
