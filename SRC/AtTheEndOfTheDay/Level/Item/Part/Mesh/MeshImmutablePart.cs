using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshImmutableePart : MeshTransformedPart
    {
        public MeshImmutableePart(TgcMesh mesh)
            : base(mesh) { }
        protected override void ItemPositionChanged(Item item) { }
        protected override void ItemRotationChanged(Item item) { }
        protected override void ItemScaleChanged(Item item) { }
    }
}
