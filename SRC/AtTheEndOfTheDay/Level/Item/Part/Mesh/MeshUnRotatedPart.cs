using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshUnRotatedPart : MeshTransformedPart
    {
        public MeshUnRotatedPart(TgcMesh mesh)
            : base(mesh) { }
        protected override void ItemRotationChanged(Item item) { }
    }
}
